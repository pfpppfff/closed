using S7.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace S7NET.Services
{
    /// <summary>
    /// S7通讯服务实现类
    /// </summary>
    public class S7CommunicationService : IS7CommunicationService
    {
        private readonly Plc _plc;
        private readonly string _ipAddress;
        private readonly System.Threading.Timer _heartbeatTimer;
        private readonly SemaphoreSlim _connectionSemaphore;
        private readonly ConcurrentQueue<Exception> _errorQueue;
        private readonly ConcurrentDictionary<string, (object Value, DateTime Timestamp)> _readCache;
        private readonly int _cacheExpirationMs = 100; // 缓存过期时间100ms
        private bool _disposed = false;
        private bool _isConnecting = false;
        private bool _autoReconnectEnabled = true; // 是否启用自动重连
        private DateTime _lastDisconnectTime = DateTime.MinValue; // 上次断开时间
        private int _reconnectAttempts = 0; // 重连尝试次数
        private readonly int _maxReconnectAttempts = 3; // 最大重连次数

        public bool IsConnected => _plc?.IsConnected ?? false;

        public event EventHandler<bool> ConnectionStatusChanged;
        public event EventHandler<string> CommunicationError;

        /// <summary>
        /// 启用或禁用自动重连
        /// </summary>
        /// <param name="enabled">是否启用自动重连</param>
        public void SetAutoReconnect(bool enabled)
        {
            _autoReconnectEnabled = enabled;
            if (enabled)
            {
                OnCommunicationError($"已启用PLC {_ipAddress} 自动重连功能");
            }
            else
            {
                OnCommunicationError($"已禁用PLC {_ipAddress} 自动重连功能");
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cpuType">CPU类型</param>
        /// <param name="ip">IP地址</param>
        /// <param name="rack">机架号</param>
        /// <param name="slot">插槽号</param>
        public S7CommunicationService(CpuType cpuType, string ip, short rack = 0, short slot = 1)
        {
            _plc = new Plc(cpuType, ip, rack, slot);
            _ipAddress = ip;
            _connectionSemaphore = new SemaphoreSlim(1, 1);
            _errorQueue = new ConcurrentQueue<Exception>();
            _readCache = new ConcurrentDictionary<string, (object Value, DateTime Timestamp)>();

            // 心跳检测定时器，每5秒检测一次连接状态
            _heartbeatTimer = new System.Threading.Timer(HeartbeatCheck, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }

        public async Task<bool> ConnectAsync()
        {
            return await ConnectAsync(CancellationToken.None);
        }

        public async Task<bool> ConnectAsync(CancellationToken cancellationToken)
        {
            if (_isConnecting || IsConnected) return IsConnected;

            await _connectionSemaphore.WaitAsync(cancellationToken);
            try
            {
                _isConnecting = true;

                // 设置连接超时时间为5秒
                using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
                using (var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token))
                {
                    try
                    {
                        // 使用Task.Run配合超时来执行连接
                        var connectTask = Task.Run(async () =>
                        {
                            try
                            {
                                await _plc.OpenAsync();
                                return true;
                            }
                            catch (Exception ex)
                            {
                                // 检查是否是连接失败的异常
                                if (IsConnectionFailedException(ex))
                                {
                                    OnCommunicationError($"连接失败: PLC {_ipAddress} 不可达或未响应");
                                }
                                else
                                {
                                    OnCommunicationError($"连接失败: {ex.Message}");
                                }
                                return false;
                            }
                        });

                        // 等待连接完成或超时（5秒）
                        var timeoutTask = Task.Delay(5000, combinedCts.Token);
                        var completedTask = await Task.WhenAny(connectTask, timeoutTask);

                        if (completedTask == connectTask)
                        {
                            // 连接任务完成
                            var result = await connectTask;
                            if (!result)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            // 超时了，强制关闭连接
                            try
                            {
                                _plc?.Close();
                            }
                            catch { }

                            OnCommunicationError($"连接超时: 无法在5秒内连接到 {_ipAddress}");
                            return false;
                        }

                        // 检查取消请求
                        combinedCts.Token.ThrowIfCancellationRequested();
                    }
                    catch (OperationCanceledException)
                    {
                        // 强制关闭连接
                        try
                        {
                            _plc?.Close();
                        }
                        catch { }

                        if (timeoutCts.Token.IsCancellationRequested)
                        {
                            OnCommunicationError($"连接超时: 无法在5秒内连接到 {_ipAddress}");
                        }
                        else
                        {
                            OnCommunicationError($"连接已取消: {_ipAddress}");
                        }
                        return false;
                    }
                }

                var connected = IsConnected;
                OnConnectionStatusChanged(connected);
                return connected;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                OnCommunicationError($"连接异常: {ex.Message}");
                return false;
            }
            finally
            {
                _isConnecting = false;
                _connectionSemaphore.Release();
            }
        }

        public async Task DisconnectAsync()
        {
            await _connectionSemaphore.WaitAsync();
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        _plc?.Close();
                    }
                    catch (Exception ex)
                    {
                        OnCommunicationError($"断开连接异常: {ex.Message}");
                    }
                });

                OnConnectionStatusChanged(false);
            }
            finally
            {
                _connectionSemaphore.Release();
            }
        }

        /// <summary>
        /// 强制断开连接（用于连接异常时）
        /// </summary>
        private async Task ForceDisconnectAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        _plc?.Close();
                    }
                    catch { }
                });
            }
            catch { }
        }

        public async Task<T> ReadAsync<T>(string address)
        {
            if (!await EnsureConnectionAsync())
                throw new InvalidOperationException("PLC未连接");

            // 检查缓存
            var cacheKey = $"{address}_{typeof(T).Name}";
            if (_readCache.TryGetValue(cacheKey, out var cachedData))
            {
                var age = DateTime.Now - cachedData.Timestamp;
                if (age.TotalMilliseconds < _cacheExpirationMs)
                {
                    return (T)cachedData.Value;
                }
                else
                {
                    // 移除过期缓存
                    _readCache.TryRemove(cacheKey, out _);
                }
            }

            try
            {
                var addressInfo = ParseAddress(address);
                var data = await _plc.ReadAsync(addressInfo.DataType, addressInfo.DbNumber, addressInfo.StartByte,
                                               GetVarType<T>(), 1);
                var result = (T)data;

                // 更新缓存
                _readCache.TryAdd(cacheKey, (result, DateTime.Now));

                return result;
            }
            catch (Exception ex)
            {
                // 检查是否是连接断开异常
                if (IsConnectionLostException(ex))
                {
                    OnCommunicationError($"PLC连接已断开 [{address}]: {ex.Message}");
                    OnConnectionStatusChanged(false);
                    throw new InvalidOperationException("PLC连接已断开", ex);
                }
                else
                {
                    OnCommunicationError($"读取数据失败 [{address}]: {ex.Message}");
                    throw;
                }
            }
        }

        public async Task<Dictionary<string, object>> ReadMultipleAsync(params string[] addresses)
        {
            if (!await EnsureConnectionAsync()) 
                throw new InvalidOperationException("PLC未连接");

            var results = new Dictionary<string, object>();
            var tasks = new List<Task>();

            foreach (var address in addresses)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var value = await ReadAsync<object>(address);
                        lock (results)
                        {
                            results[address] = value;
                        }
                    }
                    catch (Exception ex)
                    {
                        OnCommunicationError($"读取多个数据失败 [{address}]: {ex.Message}");
                        lock (results)
                        {
                            results[address] = null;
                        }
                    }
                }));
            }

            await Task.WhenAll(tasks);
            return results;
        }

        public async Task<bool> WriteAsync<T>(string address, T value)
        {
            if (!await EnsureConnectionAsync()) 
                return false;

            try
            {
                var addressInfo = ParseAddress(address);
                await _plc.WriteAsync(addressInfo.DataType, addressInfo.DbNumber, addressInfo.StartByte, value);
                // 如果没有抛出异常，认为写入成功
                return true;
            }
            catch (Exception ex)
            {
                // 检查是否是连接断开异常
                if (IsConnectionLostException(ex))
                {
                    OnCommunicationError($"PLC连接已断开 [{address}]: {ex.Message}");
                    OnConnectionStatusChanged(false);
                }
                else
                {
                    OnCommunicationError($"写入数据失败 [{address}]: {ex.Message}");
                }
                return false;
            }
        }

        public async Task<bool> WriteMultipleAsync(Dictionary<string, object> data)
        {
            if (!await EnsureConnectionAsync()) 
                return false;

            var tasks = data.Select(async kvp =>
            {
                try
                {
                    return await WriteAsync(kvp.Key, kvp.Value);
                }
                catch (Exception ex)
                {
                    OnCommunicationError($"批量写入失败 [{kvp.Key}]: {ex.Message}");
                    return false;
                }
            });

            var results = await Task.WhenAll(tasks);
            return results.All(r => r);
        }

        public async Task<T[]> ReadDBAsync<T>(int dbNumber, int startByte, int count = 1)
        {
            // 先检查基本连接状态
            if (!IsConnected)
            {
                throw new InvalidOperationException("PLC未连接");
            }

            try
            {
                // 在读取前再次检查连接状态
                if (!IsConnected)
                {
                    throw new InvalidOperationException("PLC连接已断开");
                }

                var result = await _plc.ReadAsync(DataType.DataBlock, dbNumber, startByte, GetVarType<T>(), count);
                if (result is T[] array)
                    return array;
                else if (result is T single)
                    return new T[] { single };
                else
                    return new T[] { (T)result };
            }
            catch (Exception ex)
            {
                // 检查是否是连接断开异常（包括内部异常）
                if (IsConnectionLostException(ex) || IsConnectionLostException(ex.InnerException))
                {
                    OnCommunicationError($"检测到PLC连接断开: {_ipAddress}");

                    // 强制断开连接并更新状态
                    await ForceDisconnectAsync();
                    OnConnectionStatusChanged(false);

                    // 抛出友好的异常信息
                    throw new InvalidOperationException("PLC连接已断开", ex);
                }
                else
                {
                    OnCommunicationError($"读取DB块失败 [DB{dbNumber}.DBX{startByte}]: {ex.Message}");
                    throw;
                }
            }
        }

        public async Task<bool> WriteDBAsync<T>(int dbNumber, int startByte, T value)
        {
            if (!await EnsureConnectionAsync())
                return false;

            try
            {
                await _plc.WriteAsync(DataType.DataBlock, dbNumber, startByte, value);
                // 如果没有抛出异常，认为写入成功
                return true;
            }
            catch (Exception ex)
            {
                // 检查是否是连接断开异常
                if (IsConnectionLostException(ex))
                {
                    OnCommunicationError($"PLC连接已断开 [DB{dbNumber}.DBX{startByte}]: {ex.Message}");
                    OnConnectionStatusChanged(false);
                }
                else
                {
                    OnCommunicationError($"写入DB块失败 [DB{dbNumber}.DBX{startByte}]: {ex.Message}");
                }
                return false;
            }
        }

        public async Task<bool> ReadBitAsync(string address)
        {
            if (!await EnsureConnectionAsync()) 
                throw new InvalidOperationException("PLC未连接");

            try
            {
                var addressInfo = ParseBitAddress(address);
                var result = await _plc.ReadAsync(addressInfo.DataType, addressInfo.DbNumber, addressInfo.StartByte,
                                                 VarType.Bit, 1, addressInfo.BitNumber);
                return (bool)result;
            }
            catch (Exception ex)
            {
                OnCommunicationError($"读取位数据失败 [{address}]: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> WriteBitAsync(string address, bool value)
        {
            if (!await EnsureConnectionAsync()) 
                return false;

            try
            {
                var addressInfo = ParseBitAddress(address);
                await _plc.WriteAsync(addressInfo.DataType, addressInfo.DbNumber, addressInfo.StartByte,
                                     value, addressInfo.BitNumber);
                // 如果没有抛出异常，认为写入成功
                return true;
            }
            catch (Exception ex)
            {
                OnCommunicationError($"写入位数据失败 [{address}]: {ex.Message}");
                return false;
            }
        }

        #region 扩展读取方法

        /// <summary>
        /// 读取Q点（输出点）bool量
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="length">读取长度（位数）</param>
        /// <returns>bool数组</returns>
        public async Task<bool[]> ReadQBitsAsync(int startByte, int startBit, int length)
        {
            if (!await EnsureConnectionAsync())
                throw new InvalidOperationException("PLC未连接");

            try
            {
                var results = new bool[length];
                for (int i = 0; i < length; i++)
                {
                    int currentByte = startByte + (startBit + i) / 8;
                    int currentBit = (startBit + i) % 8;

                    var result = await _plc.ReadAsync(DataType.Output, 0, currentByte, VarType.Bit, 1, (byte)currentBit);
                    results[i] = (bool)result;
                }
                return results;
            }
            catch (Exception ex)
            {
                if (IsConnectionLostException(ex))
                {
                    OnCommunicationError($"PLC连接已断开 [Q{startByte}.{startBit}]: {ex.Message}");
                    OnConnectionStatusChanged(false);
                    throw new InvalidOperationException("PLC连接已断开", ex);
                }
                else
                {
                    OnCommunicationError($"读取Q点失败 [Q{startByte}.{startBit}]: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// 读取I点（输入点）bool量
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="length">读取长度（位数）</param>
        /// <returns>bool数组</returns>
        public async Task<bool[]> ReadIBitsAsync(int startByte, int startBit, int length)
        {
            if (!await EnsureConnectionAsync())
                throw new InvalidOperationException("PLC未连接");

            try
            {
                var results = new bool[length];
                for (int i = 0; i < length; i++)
                {
                    int currentByte = startByte + (startBit + i) / 8;
                    int currentBit = (startBit + i) % 8;

                    var result = await _plc.ReadAsync(DataType.Input, 0, currentByte, VarType.Bit, 1, (byte)currentBit);
                    results[i] = (bool)result;
                }
                return results;
            }
            catch (Exception ex)
            {
                if (IsConnectionLostException(ex))
                {
                    OnCommunicationError($"PLC连接已断开 [I{startByte}.{startBit}]: {ex.Message}");
                    OnConnectionStatusChanged(false);
                    throw new InvalidOperationException("PLC连接已断开", ex);
                }
                else
                {
                    OnCommunicationError($"读取I点失败 [I{startByte}.{startBit}]: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// 读取M区不同类型数据
        /// </summary>
        /// <typeparam name="T">数据类型（bool, byte, short, int, float, double等）</typeparam>
        /// <param name="startByte">起始字节</param>
        /// <param name="count">读取数量</param>
        /// <returns>指定类型的数据数组</returns>
        public async Task<T[]> ReadMAsync<T>(int startByte, int count = 1)
        {
            if (!await EnsureConnectionAsync())
                throw new InvalidOperationException("PLC未连接");

            try
            {
                var result = await _plc.ReadAsync(DataType.Memory, 0, startByte, GetVarType<T>(), count);
                if (result is T[] array)
                    return array;
                else if (result is T single)
                    return new T[] { single };
                else
                    return new T[] { (T)result };
            }
            catch (Exception ex)
            {
                if (IsConnectionLostException(ex))
                {
                    OnCommunicationError($"PLC连接已断开 [M{startByte}]: {ex.Message}");
                    OnConnectionStatusChanged(false);
                    throw new InvalidOperationException("PLC连接已断开", ex);
                }
                else
                {
                    OnCommunicationError($"读取M区数据失败 [M{startByte}]: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// 读取M区位数据
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="length">读取长度（位数）</param>
        /// <returns>bool数组</returns>
        public async Task<bool[]> ReadMBitsAsync(int startByte, int startBit, int length)
        {
            if (!await EnsureConnectionAsync())
                throw new InvalidOperationException("PLC未连接");

            try
            {
                var results = new bool[length];
                for (int i = 0; i < length; i++)
                {
                    int currentByte = startByte + (startBit + i) / 8;
                    int currentBit = (startBit + i) % 8;

                    var result = await _plc.ReadAsync(DataType.Memory, 0, currentByte, VarType.Bit, 1, (byte)currentBit);
                    results[i] = (bool)result;
                }
                return results;
            }
            catch (Exception ex)
            {
                if (IsConnectionLostException(ex))
                {
                    OnCommunicationError($"PLC连接已断开 [M{startByte}.{startBit}]: {ex.Message}");
                    OnConnectionStatusChanged(false);
                    throw new InvalidOperationException("PLC连接已断开", ex);
                }
                else
                {
                    OnCommunicationError($"读取M区位数据失败 [M{startByte}.{startBit}]: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// 写入Q点（输出点）bool量
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="values">bool值数组</param>
        /// <returns>写入是否成功</returns>
        public async Task<bool> WriteQBitsAsync(int startByte, int startBit, bool[] values)
        {
            if (!await EnsureConnectionAsync())
                return false;

            try
            {
                for (int i = 0; i < values.Length; i++)
                {
                    int currentByte = startByte + (startBit + i) / 8;
                    int currentBit = (startBit + i) % 8;

                    await _plc.WriteAsync(DataType.Output, 0, currentByte, values[i], (byte)currentBit);
                }
                return true;
            }
            catch (Exception ex)
            {
                if (IsConnectionLostException(ex))
                {
                    OnCommunicationError($"PLC连接已断开 [Q{startByte}.{startBit}]: {ex.Message}");
                    OnConnectionStatusChanged(false);
                    return false;
                }
                else
                {
                    OnCommunicationError($"写入Q点失败 [Q{startByte}.{startBit}]: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// 写入单个Q点bool值
        /// </summary>
        /// <param name="byteAddress">字节地址</param>
        /// <param name="bitAddress">位地址</param>
        /// <param name="value">bool值</param>
        /// <returns>写入是否成功</returns>
        public async Task<bool> WriteQBitAsync(int byteAddress, int bitAddress, bool value)
        {
            if (!await EnsureConnectionAsync())
                return false;

            try
            {
                await _plc.WriteAsync(DataType.Output, 0, byteAddress, value, (byte)bitAddress);
                return true;
            }
            catch (Exception ex)
            {
                if (IsConnectionLostException(ex))
                {
                    OnCommunicationError($"PLC连接已断开 [Q{byteAddress}.{bitAddress}]: {ex.Message}");
                    OnConnectionStatusChanged(false);
                    return false;
                }
                else
                {
                    OnCommunicationError($"写入Q点失败 [Q{byteAddress}.{bitAddress}]: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// 写入M区不同类型数据
        /// </summary>
        /// <typeparam name="T">数据类型（byte, short, int, float, double等）</typeparam>
        /// <param name="startByte">起始字节</param>
        /// <param name="values">数据值数组</param>
        /// <returns>写入是否成功</returns>
        public async Task<bool> WriteMAsync<T>(int startByte, T[] values)
        {
            if (!await EnsureConnectionAsync())
                return false;

            try
            {
                for (int i = 0; i < values.Length; i++)
                {
                    int currentByte = startByte + i * GetTypeSize<T>();
                    await _plc.WriteAsync(DataType.Memory, 0, currentByte, values[i]);
                }
                return true;
            }
            catch (Exception ex)
            {
                if (IsConnectionLostException(ex))
                {
                    OnCommunicationError($"PLC连接已断开 [M{startByte}]: {ex.Message}");
                    OnConnectionStatusChanged(false);
                    return false;
                }
                else
                {
                    OnCommunicationError($"写入M区数据失败 [M{startByte}]: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// 写入单个M区数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="startByte">起始字节</param>
        /// <param name="value">数据值</param>
        /// <returns>写入是否成功</returns>
        public async Task<bool> WriteMSingleAsync<T>(int startByte, T value)
        {
            if (!await EnsureConnectionAsync())
                return false;

            try
            {
                await _plc.WriteAsync(DataType.Memory, 0, startByte, value);
                return true;
            }
            catch (Exception ex)
            {
                if (IsConnectionLostException(ex))
                {
                    OnCommunicationError($"PLC连接已断开 [M{startByte}]: {ex.Message}");
                    OnConnectionStatusChanged(false);
                    return false;
                }
                else
                {
                    OnCommunicationError($"写入M区数据失败 [M{startByte}]: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// 写入M区位数据
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="values">bool值数组</param>
        /// <returns>写入是否成功</returns>
        public async Task<bool> WriteMBitsAsync(int startByte, int startBit, bool[] values)
        {
            if (!await EnsureConnectionAsync())
                return false;

            try
            {
                for (int i = 0; i < values.Length; i++)
                {
                    int currentByte = startByte + (startBit + i) / 8;
                    int currentBit = (startBit + i) % 8;

                    await _plc.WriteAsync(DataType.Memory, 0, currentByte, values[i], (byte)currentBit);
                }
                return true;
            }
            catch (Exception ex)
            {
                if (IsConnectionLostException(ex))
                {
                    OnCommunicationError($"PLC连接已断开 [M{startByte}.{startBit}]: {ex.Message}");
                    OnConnectionStatusChanged(false);
                    return false;
                }
                else
                {
                    OnCommunicationError($"写入M区位数据失败 [M{startByte}.{startBit}]: {ex.Message}");
                    return false;
                }
            }
        }

        #endregion

        private async Task<bool> EnsureConnectionAsync()
        {
            if (IsConnected) return true;

            // 尝试重连，最多3次
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    var success = await ConnectAsync();
                    if (success) return true;

                    // 如果连接失败，等待1秒后重试
                    if (i < 2) // 不是最后一次尝试
                    {
                        await Task.Delay(1000);
                    }
                }
                catch (Exception ex)
                {
                    OnCommunicationError($"连接尝试 {i + 1}/3 失败: {ex.Message}");
                    if (i < 2) // 不是最后一次尝试
                    {
                        await Task.Delay(1000);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 检查异常是否表示连接已断开
        /// </summary>
        private bool IsConnectionLostException(Exception ex)
        {
            if (ex == null) return false;

            // 递归检查内部异常
            if (CheckSingleException(ex))
                return true;

            // 检查内部异常
            var innerEx = ex.InnerException;
            while (innerEx != null)
            {
                if (CheckSingleException(innerEx))
                    return true;
                innerEx = innerEx.InnerException;
            }

            return false;
        }

        private bool CheckSingleException(Exception ex)
        {
            if (ex == null) return false;

            var message = ex.Message?.ToLower() ?? "";
            var typeName = ex.GetType().Name.ToLower();

            // 检查S7.Net.PlcException类型
            if (typeName.Contains("plcexception"))
            {
                return message.Contains("无法将数据写入传输连接") ||
                       message.Contains("无法格式化据写入传输连接") ||
                       message.Contains("远程主机强迫关闭了一个现有的连接") ||
                       message.Contains("unable to write data to the transport connection") ||
                       message.Contains("unable to format data to the transport connection") ||
                       message.Contains("connection was forcibly closed") ||
                       message.Contains("an existing connection was forcibly closed");
            }

            // 检查SocketException类型
            if (ex is System.Net.Sockets.SocketException socketEx)
            {
                return socketEx.SocketErrorCode == System.Net.Sockets.SocketError.ConnectionAborted ||
                       socketEx.SocketErrorCode == System.Net.Sockets.SocketError.ConnectionReset ||
                       socketEx.SocketErrorCode == System.Net.Sockets.SocketError.HostUnreachable ||
                       socketEx.SocketErrorCode == System.Net.Sockets.SocketError.NetworkUnreachable ||
                       socketEx.SocketErrorCode == System.Net.Sockets.SocketError.ConnectionRefused;
            }

            // 检查常见的连接断开异常消息
            return message.Contains("远程主机强迫关闭了一个现有的连接") ||
                   message.Contains("connection was forcibly closed") ||
                   message.Contains("an existing connection was forcibly closed") ||
                   message.Contains("连接被重置") ||
                   message.Contains("connection reset") ||
                   message.Contains("网络不可达") ||
                   message.Contains("network unreachable") ||
                   message.Contains("连接超时") ||
                   message.Contains("connection timeout") ||
                   message.Contains("无法连接") ||
                   message.Contains("cannot connect") ||
                   message.Contains("无法将数据写入传输连接") ||
                   message.Contains("无法格式化据写入传输连接") ||
                   message.Contains("unable to write data to the transport connection") ||
                   message.Contains("unable to format data to the transport connection") ||
                   ex is System.IO.IOException;
        }

        /// <summary>
        /// 检查异常是否表示连接失败（PLC不可达）
        /// </summary>
        private bool IsConnectionFailedException(Exception ex)
        {
            if (ex == null) return false;

            var message = ex.Message?.ToLower() ?? "";

            // 检查连接失败的异常消息
            return message.Contains("couldn't establish the connection") ||
                   message.Contains("连接尝试失败") ||
                   message.Contains("connection attempt failed") ||
                   message.Contains("没有正确答复") ||
                   message.Contains("主机没有反应") ||
                   message.Contains("host did not respond") ||
                   message.Contains("connection timed out") ||
                   message.Contains("network is unreachable") ||
                   message.Contains("无法格式化据写入传输连接") ||
                   message.Contains("远程主机强迫关闭了一个现有的连接") ||
                   message.Contains("an existing connection was forcibly closed") ||
                   message.Contains("connection was aborted") ||
                   message.Contains("连接被中止") ||
                   message.Contains("socket error") ||
                   message.Contains("套接字错误") ||
                   (ex is System.Net.Sockets.SocketException socketEx &&
                    (socketEx.SocketErrorCode == System.Net.Sockets.SocketError.TimedOut ||
                     socketEx.SocketErrorCode == System.Net.Sockets.SocketError.HostUnreachable ||
                     socketEx.SocketErrorCode == System.Net.Sockets.SocketError.NetworkUnreachable ||
                     socketEx.SocketErrorCode == System.Net.Sockets.SocketError.ConnectionRefused ||
                     socketEx.SocketErrorCode == System.Net.Sockets.SocketError.ConnectionAborted ||
                     socketEx.SocketErrorCode == System.Net.Sockets.SocketError.ConnectionReset)) ||
                   (ex is S7.Net.PlcException plcEx &&
                    (message.Contains("plc") || message.Contains("s7") || message.Contains("siemens")));
        }

        private void HeartbeatCheck(object state)
        {
            if (_disposed) return;

            try
            {
                var wasConnected = IsConnected;
                var currentlyConnected = IsConnected;

                // 检测连接状态变化
                if (wasConnected && !currentlyConnected)
                {
                    // 连接断开了
                    _lastDisconnectTime = DateTime.Now;
                    _reconnectAttempts = 0;
                    OnConnectionStatusChanged(false);
                    OnCommunicationError($"检测到PLC连接断开: {_ipAddress}");
                }
                else if (!wasConnected && !currentlyConnected && _autoReconnectEnabled)
                {
                    // 尝试自动重连
                    TryAutoReconnect();
                }
                else if (!wasConnected && currentlyConnected)
                {
                    // 重连成功
                    _reconnectAttempts = 0;
                    OnConnectionStatusChanged(true);
                    OnCommunicationError($"PLC重连成功: {_ipAddress}");
                }

                // 清理过期缓存
                CleanExpiredCache();
            }
            catch (Exception ex)
            {
                OnCommunicationError($"心跳检测异常: {ex.Message}");
            }
        }

        private void TryAutoReconnect()
        {
            // 检查是否应该尝试重连
            if (_reconnectAttempts >= _maxReconnectAttempts)
            {
                return; // 已达到最大重连次数
            }

            // 检查距离上次断开是否已经过了足够时间（至少5秒）
            var timeSinceDisconnect = DateTime.Now - _lastDisconnectTime;
            if (timeSinceDisconnect.TotalSeconds < 5)
            {
                return; // 等待更长时间再重连
            }

            // 检查是否正在连接
            if (_isConnecting)
            {
                return; // 已经在连接中
            }

            _reconnectAttempts++;
            OnCommunicationError($"尝试自动重连PLC {_ipAddress} (第{_reconnectAttempts}次)...");

            // 异步执行重连，不阻塞心跳检测
            Task.Run(async () =>
            {
                try
                {
                    var success = await ConnectAsync(CancellationToken.None);
                    if (success)
                    {
                        OnCommunicationError($"PLC {_ipAddress} 自动重连成功");
                    }
                    else
                    {
                        OnCommunicationError($"PLC {_ipAddress} 自动重连失败");
                    }
                }
                catch (Exception ex)
                {
                    OnCommunicationError($"PLC {_ipAddress} 自动重连异常: {ex.Message}");
                }
            });
        }

        private void CleanExpiredCache()
        {
            var now = DateTime.Now;
            var expiredKeys = new List<string>();

            foreach (var kvp in _readCache)
            {
                var age = now - kvp.Value.Timestamp;
                if (age.TotalMilliseconds > _cacheExpirationMs * 2) // 清理超过2倍过期时间的缓存
                {
                    expiredKeys.Add(kvp.Key);
                }
            }

            foreach (var key in expiredKeys)
            {
                _readCache.TryRemove(key, out _);
            }
        }

        private VarType GetVarType<T>()
        {
            var type = typeof(T);
            
            if (type == typeof(bool)) return VarType.Bit;
            if (type == typeof(byte)) return VarType.Byte;
            if (type == typeof(short)) return VarType.Int;
            if (type == typeof(int)) return VarType.DInt;
            if (type == typeof(float)) return VarType.Real;
            if (type == typeof(double)) return VarType.LReal;
            if (type == typeof(string)) return VarType.String;
            if (type == typeof(ushort)) return VarType.Word;
            if (type == typeof(uint)) return VarType.DWord;
            
            return VarType.Byte; // 默认
        }

        private int GetTypeSize<T>()
        {
            var type = typeof(T);

            if (type == typeof(bool)) return 1;    // 1 bit, 但按字节计算
            if (type == typeof(byte)) return 1;    // 1 byte
            if (type == typeof(short)) return 2;   // 2 bytes
            if (type == typeof(int)) return 4;     // 4 bytes
            if (type == typeof(float)) return 4;   // 4 bytes
            if (type == typeof(double)) return 8;  // 8 bytes
            if (type == typeof(ushort)) return 2;  // 2 bytes
            if (type == typeof(uint)) return 4;    // 4 bytes

            return 1; // 默认1字节
        }

        private (DataType DataType, int DbNumber, int StartByte) ParseAddress(string address)
        {
            // 简单的地址解析，支持格式如: DB1.DBW0, MW0, IW0, QW0等
            address = address.ToUpper().Trim();
            
            if (address.StartsWith("DB"))
            {
                var parts = address.Split('.');
                var dbNumber = int.Parse(parts[0].Substring(2));
                var offset = int.Parse(parts[1].Substring(3)); // 去掉DBW/DBD等前缀
                return (DataType.DataBlock, dbNumber, offset);
            }
            else if (address.StartsWith("M"))
            {
                var offset = int.Parse(address.Substring(2));
                return (DataType.Memory, 0, offset);
            }
            else if (address.StartsWith("I"))
            {
                var offset = int.Parse(address.Substring(2));
                return (DataType.Input, 0, offset);
            }
            else if (address.StartsWith("Q"))
            {
                var offset = int.Parse(address.Substring(2));
                return (DataType.Output, 0, offset);
            }
            
            throw new ArgumentException($"不支持的地址格式: {address}");
        }

        private (DataType DataType, int DbNumber, int StartByte, byte BitNumber) ParseBitAddress(string address)
        {
            // 解析位地址，如: M0.0, I0.1, Q0.2, DB1.DBX0.0
            address = address.ToUpper().Trim();
            var parts = address.Split('.');
            
            if (address.StartsWith("DB"))
            {
                var dbNumber = int.Parse(parts[0].Substring(2));
                var byteOffset = int.Parse(parts[1].Substring(3));
                var bitNumber = byte.Parse(parts[2]);
                return (DataType.DataBlock, dbNumber, byteOffset, bitNumber);
            }
            else
            {
                var byteOffset = int.Parse(parts[0].Substring(1));
                var bitNumber = byte.Parse(parts[1]);
                
                if (address.StartsWith("M"))
                    return (DataType.Memory, 0, byteOffset, bitNumber);
                else if (address.StartsWith("I"))
                    return (DataType.Input, 0, byteOffset, bitNumber);
                else if (address.StartsWith("Q"))
                    return (DataType.Output, 0, byteOffset, bitNumber);
            }
            
            throw new ArgumentException($"不支持的位地址格式: {address}");
        }

        protected virtual void OnConnectionStatusChanged(bool isConnected)
        {
            ConnectionStatusChanged?.Invoke(this, isConnected);
        }

        protected virtual void OnCommunicationError(string error)
        {
            CommunicationError?.Invoke(this, error);
        }

        public void Dispose()
        {
            if (_disposed) return;
            
            _disposed = true;
            _heartbeatTimer?.Dispose();
            _connectionSemaphore?.Dispose();
            
            try
            {
                _plc?.Close();
                // S7.NET库的Plc类可能没有公共的Dispose方法
                // 只需要调用Close方法即可释放连接资源
            }
            catch { }
        }
    }
}