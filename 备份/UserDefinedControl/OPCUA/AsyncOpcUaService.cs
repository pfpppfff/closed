using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UcAsp.Opc;

namespace UserDefinedControl.OPCUA
{
    /// <summary>
    /// 异步优先的 OPC UA 服务实现
    /// 提供更好的异步控制，避免UI阻塞，适合工业自动化多控件场景
    /// </summary>
    public sealed class AsyncOpcUaService : IOpcUaService
    {
        #region 单例模式实现
        private static readonly object _lockObject = new object();
        private static volatile AsyncOpcUaService _instance;

        /// <summary>
        /// 获取全局单例实例
        /// </summary>
        public static AsyncOpcUaService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new AsyncOpcUaService();
                        }
                    }
                }
                return _instance;
            }
        }

        private AsyncOpcUaService()
        {
            InitializeAsync().ConfigureAwait(false);
            InitializeDefaultPlcConfigurations();
        }
        #endregion

        #region 属性和字段
        private OpcClient _opcClient;
        private readonly SemaphoreSlim _clientSemaphore = new SemaphoreSlim(1, 1);
        private volatile bool _isConnected = false;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        
        // 异步操作队列管理
        private readonly ConcurrentQueue<Func<Task>> _operationQueue = new ConcurrentQueue<Func<Task>>();
        private readonly SemaphoreSlim _queueSemaphore = new SemaphoreSlim(5, 5); // 限制并发操作数
        private Task _queueProcessor;
        
        // 多PLC状态管理
        private readonly ConcurrentDictionary<string, PlcStatusModel> _plcStatusMap = new ConcurrentDictionary<string, PlcStatusModel>();
        private readonly SemaphoreSlim _plcStatusSemaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 默认服务器地址
        /// </summary>
        public string DefaultServerUrl { get; set; } = "opc.tcp://127.0.0.1:49320";
        
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool IsConnected => _isConnected;

        /// <summary>
        /// 连接状态变化事件
        /// </summary>
        public event EventHandler<bool> ConnectionStatusChanged;
        
        /// <summary>
        /// PLC状态变化事件
        /// </summary>
        public event EventHandler<PlcStatusChangedEventArgs> PlcStatusChanged;

        /// <summary>
        /// 操作超时时间（毫秒）
        /// </summary>
        public int OperationTimeout { get; set; } = 5000;

        /// <summary>
        /// 最大重试次数
        /// </summary>
        public int MaxRetryCount { get; set; } = 0;
        #endregion

        #region 初始化和连接管理
        /// <summary>
        /// 异步初始化连接
        /// </summary>
        private async Task InitializeAsync()
        {
            try
            {
                await _clientSemaphore.WaitAsync();
                try
                {
                    _opcClient = new OpcClient(new Uri(DefaultServerUrl));
                    _isConnected = true;
                    OnConnectionStatusChanged(true);
                    
                    // 启动操作队列处理器
                    StartQueueProcessor();
                }
                finally
                {
                    _clientSemaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _isConnected = false;
                OnConnectionStatusChanged(false);
                System.Diagnostics.Debug.WriteLine($"OPC UA 异步初始化失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 启动操作队列处理器
        /// </summary>
        private void StartQueueProcessor()
        {
            _queueProcessor = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        if (_operationQueue.TryDequeue(out var operation))
                        {
                            await _queueSemaphore.WaitAsync(_cancellationTokenSource.Token);
                            try
                            {
                                await operation();
                            }
                            finally
                            {
                                _queueSemaphore.Release();
                            }
                        }
                        else
                        {
                            await Task.Delay(10, _cancellationTokenSource.Token);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"队列处理器错误: {ex.Message}");
                    }
                }
            }, _cancellationTokenSource.Token);
        }

        public void Connect(string serverUrl = null)
        {
            _ = ConnectAsync(serverUrl);
        }

        public async Task ConnectAsync(string serverUrl = null)
        {
            try
            {
                await _clientSemaphore.WaitAsync();
                try
                {
                    if (!string.IsNullOrWhiteSpace(serverUrl))
                    {
                        DefaultServerUrl = serverUrl;
                    }
                    
                    _opcClient = new OpcClient(new Uri(DefaultServerUrl));
                    _isConnected = true;
                    OnConnectionStatusChanged(true);
                }
                finally
                {
                    _clientSemaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _isConnected = false;
                OnConnectionStatusChanged(false);
                throw new InvalidOperationException($"OPC UA 异步连接失败: {ex.Message}", ex);
            }
        }

        public void Reconnect(string serverUrl = null)
        {
            _ = ReconnectAsync(serverUrl);
        }

        public async Task ReconnectAsync(string serverUrl = null)
        {
            await DisconnectAsync();
            await ConnectAsync(serverUrl);
        }

        public void Disconnect()
        {
            _ = DisconnectAsync();
        }

        public async Task DisconnectAsync()
        {
            await _clientSemaphore.WaitAsync();
            try
            {
                _opcClient = null;
                _isConnected = false;
                OnConnectionStatusChanged(false);
            }
            finally
            {
                _clientSemaphore.Release();
            }
        }

        private void OnConnectionStatusChanged(bool connected)
        {
            ConnectionStatusChanged?.Invoke(this, connected);
        }
        #endregion

        #region 异步读写方法（优先推荐）
        /// <summary>
        /// 异步读取布尔值（推荐使用）
        /// </summary>
        public async Task<bool> ReadBoolAsync(string nodeId)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                if (!ValidateNodeId(nodeId) || !_isConnected || _opcClient == null)
                    return false;

                return await _opcClient.ReadAsync<bool>(nodeId);
            }, false);
        }
        //public async Task<bool> ReadBoolAsync(string nodeId, CancellationToken ct = default)
        //{
        //    return await ExecuteWithRetryAsync(async (token) =>
        //    {
        //        if (!ValidateNodeId(nodeId) || !_isConnected || _opcClient == null)
        //            return false;

        //        return await _opcClient.ReadAsync<bool>(nodeId, token); // ← 传入 token
        //    }, false, ct);
        //}


        /// <summary>
        /// 异步写入布尔值（推荐使用）
        /// </summary>
        public async Task<bool> WriteBoolAsync(string nodeId, bool value)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                if (!ValidateNodeId(nodeId) || !_isConnected || _opcClient == null) 
                    return false;

                await _opcClient.WriteAsync(nodeId, value);
                return true;
            }, false);
        }

        /// <summary>
        /// 异步读取浮点数（推荐使用）
        /// </summary>
        public async Task<float> ReadFloatAsync(string nodeId)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                if (!ValidateNodeId(nodeId) || !_isConnected || _opcClient == null) 
                    return 0.0f;

                return await _opcClient.ReadAsync<float>(nodeId);
            }, 0.0f); 
        }

        /// <summary>
        /// 异步写入浮点数（推荐使用）
        /// </summary>
        public async Task<bool> WriteFloatAsync(string nodeId, float value)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                if (!ValidateNodeId(nodeId) || !_isConnected || _opcClient == null) 
                    return false;

                await _opcClient.WriteAsync(nodeId, value);
                return true;
            }, false);
        }

        /// <summary>
        /// 异步读取16位整数（推荐使用）
        /// </summary>
        public async Task<short> ReadInt16Async(string nodeId)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                if (!ValidateNodeId(nodeId) || !_isConnected || _opcClient == null) 
                    return (short)0;

                return await _opcClient.ReadAsync<short>(nodeId);
            }, (short)0);
        }

        /// <summary>
        /// 异步写入16位整数（推荐使用）
        /// </summary>
        public async Task<bool> WriteInt16Async(string nodeId, short value)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                if (!ValidateNodeId(nodeId) || !_isConnected || _opcClient == null) 
                    return false;

                await _opcClient.WriteAsync(nodeId, value);
                return true;
            }, false);
        }

        /// <summary>
        /// 异步读取32位整数（推荐使用）
        /// </summary>
        public async Task<int> ReadInt32Async(string nodeId)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                if (!ValidateNodeId(nodeId) || !_isConnected || _opcClient == null) 
                    return 0;

                return await _opcClient.ReadAsync<int>(nodeId);
            }, 0);
        }

        /// <summary>
        /// 异步写入32位整数（推荐使用）
        /// </summary>
        public async Task<bool> WriteInt32Async(string nodeId, int value)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                if (!ValidateNodeId(nodeId) || !_isConnected || _opcClient == null) 
                    return false;

                await _opcClient.WriteAsync(nodeId, value);
                return true;
            }, false);
        }

        /// <summary>
        /// 异步读取字符串（推荐使用）
        /// </summary>
        public async Task<string> ReadStringAsync(string nodeId)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                if (!ValidateNodeId(nodeId) || !_isConnected || _opcClient == null) 
                    return string.Empty;

                return await _opcClient.ReadAsync<string>(nodeId);
            }, string.Empty);
        }

        /// <summary>
        /// 异步写入字符串（推荐使用）
        /// </summary>
        public async Task<bool> WriteStringAsync(string nodeId, string value)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                if (!ValidateNodeId(nodeId) || !_isConnected || _opcClient == null) 
                    return false;

                await _opcClient.WriteAsync(nodeId, value ?? string.Empty);
                return true;
            }, false);
        }

        /// <summary>
        /// 异步切换布尔值（推荐使用）
        /// </summary>
        public async Task<bool> ToggleBoolAsync(string nodeId)
        {
            try
            {
                bool currentValue = await ReadBoolAsync(nodeId);
                return await WriteBoolAsync(nodeId, !currentValue);
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 同步方法（为兼容性保留，但不推荐使用）
        public bool ReadBool(string nodeId)
        {
            try
            {
                return ReadBoolAsync(nodeId).GetAwaiter().GetResult();
            }
            catch
            {
                return false;
            }
        }

        public bool WriteBool(string nodeId, bool value)
        {
            try
            {
                return WriteBoolAsync(nodeId, value).GetAwaiter().GetResult();
            }
            catch
            {
                return false;
            }
        }

        public float ReadFloat(string nodeId)
        {
            try
            {
                return ReadFloatAsync(nodeId).GetAwaiter().GetResult();
            }
            catch
            {
                return 0.0f;
            }
        }

        public bool WriteFloat(string nodeId, float value)
        {
            try
            {
                return WriteFloatAsync(nodeId, value).GetAwaiter().GetResult();
            }
            catch
            {
                return false;
            }
        }

        public short ReadInt16(string nodeId)
        {
            try
            {
                return ReadInt16Async(nodeId).GetAwaiter().GetResult();
            }
            catch
            {
                return 0;
            }
        }

        public bool WriteInt16(string nodeId, short value)
        {
            try
            {
                return WriteInt16Async(nodeId, value).GetAwaiter().GetResult();
            }
            catch
            {
                return false;
            }
        }

        public int ReadInt32(string nodeId)
        {
            try
            {
                return ReadInt32Async(nodeId).GetAwaiter().GetResult();
            }
            catch
            {
                return 0;
            }
        }

        public bool WriteInt32(string nodeId, int value)
        {
            try
            {
                return WriteInt32Async(nodeId, value).GetAwaiter().GetResult();
            }
            catch
            {
                return false;
            }
        }

        public string ReadString(string nodeId)
        {
            try
            {
                return ReadStringAsync(nodeId).GetAwaiter().GetResult();
            }
            catch
            {
                return string.Empty;
            }
        }

        public bool WriteString(string nodeId, string value)
        {
            try
            {
                return WriteStringAsync(nodeId, value).GetAwaiter().GetResult();
            }
            catch
            {
                return false;
            }
        }

        public bool ToggleBool(string nodeId)
        {
            try
            {
                return ToggleBoolAsync(nodeId).GetAwaiter().GetResult();
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 批量异步操作
        /// <summary>
        /// 批量异步读取多个节点
        /// </summary>
        public async Task<List<OpcItemValue>> ReadMultipleAsync(params string[] nodeIds)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                if (nodeIds == null || nodeIds.Length == 0 || !_isConnected || _opcClient == null)
                    return new List<OpcItemValue>();

                await _clientSemaphore.WaitAsync();
                try
                {
                    return _opcClient.Read(nodeIds);
                }
                finally
                {
                    _clientSemaphore.Release();
                }
            }, new List<OpcItemValue>());
        }

        /// <summary>
        /// 批量异步写入多个节点
        /// </summary>
        public async Task<bool> WriteMultipleAsync(Dictionary<string, object> writeOperations)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                if (writeOperations == null || writeOperations.Count == 0 || !_isConnected || _opcClient == null)
                    return false;

                await _clientSemaphore.WaitAsync();
                try
                {
                    var tasks = new List<Task>();
                    foreach (var operation in writeOperations)
                    {
                        tasks.Add(_opcClient.WriteAsync(operation.Key, operation.Value));
                    }
                    await Task.WhenAll(tasks);
                    return true;
                }
                finally
                {
                    _clientSemaphore.Release();
                }
            }, false);
        }

        public List<OpcItemValue> ReadMultiple(params string[] nodeIds)
        {
            try
            {
                return ReadMultipleAsync(nodeIds).GetAwaiter().GetResult();
            }
            catch
            {
                return new List<OpcItemValue>();
            }
        }

        public bool WriteMultiple(Dictionary<string, object> writeOperations)
        {
            try
            {
                return WriteMultipleAsync(writeOperations).GetAwaiter().GetResult();
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 高级异步功能
        /// <summary>
        /// 带重试机制的异步执行
        /// </summary>
        private async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, T defaultValue)
        {
            //for (int attempt = 0; attempt <= MaxRetryCount; attempt++)
            //{
            try
            {
                using (var cts = new CancellationTokenSource(OperationTimeout))
                {
                    return await operation();
                }
            }
            catch (Exception ex)
            {
                //if (attempt == MaxRetryCount)
                //{
                //    System.Diagnostics.Debug.WriteLine($"操作失败，已重试{MaxRetryCount}次: {ex.Message}");
                //    return defaultValue;
                //}

                // 指数退避延迟
                //  await Task.Delay(Math.Min(100 * (int)Math.Pow(2, attempt), 1000));
                await Task.Delay(300);
                return defaultValue;
            }
            //}
            //return defaultValue;
        }
        //private async Task<T> ExecuteWithRetryAsync<T>(Func<CancellationToken, Task<T>> operation, T defaultValue, CancellationToken externalToken = default)
        //{
        //    //for (int attempt = 0; attempt <= MaxRetryCount; attempt++)
        //    //{
        //        // 合并外部 token 和内部超时 token
        //         var cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
        //        cts.CancelAfter(OperationTimeout); 

        //        try
        //        {
        //            return await operation(cts.Token);
        //        }
        //        catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
        //        {
        //            //// 超时或外部取消
        //            //if (attempt == MaxRetryCount)
        //                return defaultValue;
        //        }
        //        catch (Exception ex)
        //        {
        //            //if (attempt == MaxRetryCount)
        //            //{
        //            //    Debug.WriteLine($"操作失败，已重试{MaxRetryCount}次: {ex.Message}");
        //            //    return defaultValue;
        //            //}

        //            //await Task.Delay(Math.Min(100 * (int)Math.Pow(2, attempt), 1000), externalToken);
        //        await Task.Delay((300), externalToken);
        //        if (externalToken.IsCancellationRequested)
        //             return defaultValue;
        //        return defaultValue;
        //    }
        //    //}
           
        //}
        /// <summary>
        /// 原重试代码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operation"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private async Task<T> ExecuteWithRetryAsync1<T>(Func<Task<T>> operation, T defaultValue)
        {
            for (int attempt = 0; attempt <= MaxRetryCount; attempt++)
            {
                try
                {
                    using (var cts = new CancellationTokenSource(OperationTimeout))
                    {
                        return await operation();
                    }
                }
                catch (Exception ex)
                {
                    if (attempt == MaxRetryCount)
                    {
                        System.Diagnostics.Debug.WriteLine($"操作失败，已重试{MaxRetryCount}次: {ex.Message}");
                        return defaultValue;
                    }

                    // 指数退避延迟
                    await Task.Delay(Math.Min(100 * (int)Math.Pow(2, attempt), 1000));
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// 异步并行读取多个不同类型的数据
        /// </summary>
        public async Task<Dictionary<string, object>> ReadMixedTypesAsync(Dictionary<string, Type> nodeTypes)
        {
            var results = new Dictionary<string, object>();
            var tasks = new List<Task>();

            foreach (var nodeType in nodeTypes)
            {
                string nodeId = nodeType.Key;
                Type expectedType = nodeType.Value;

                if (expectedType == typeof(bool))
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var value = await ReadBoolAsync(nodeId);
                        lock (results) { results[nodeId] = value; }
                    }));
                }
                else if (expectedType == typeof(float))
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var value = await ReadFloatAsync(nodeId);
                        lock (results) { results[nodeId] = value; }
                    }));
                }
                else if (expectedType == typeof(int))
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var value = await ReadInt32Async(nodeId);
                        lock (results) { results[nodeId] = value; }
                    }));
                }
                else if (expectedType == typeof(string))
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var value = await ReadStringAsync(nodeId);
                        lock (results) { results[nodeId] = value; }
                    }));
                }
            }

            await Task.WhenAll(tasks);
            return results;
        }

        /// <summary>
        /// 定期异步读取数据（用于数据监控）
        /// </summary>
        public async Task StartPeriodicReadAsync(string nodeId, int intervalMs, Action<object> onDataReceived, CancellationToken cancellationToken = default)
        {
            await Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        // 根据节点名称推断数据类型并读取
                        object value = null;
                        if (nodeId.Contains("temp") || nodeId.Contains("flow") || nodeId.Contains("SetPoint"))
                        {
                            value = await ReadFloatAsync(nodeId);
                        }
                        else if (nodeId.Contains("Running") || nodeId.Contains("System"))
                        {
                            value = await ReadBoolAsync(nodeId);
                        }
                        else
                        {
                            value = await ReadStringAsync(nodeId);
                        }

                        onDataReceived?.Invoke(value);
                        await Task.Delay(intervalMs, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"定期读取失败: {ex.Message}");
                        await Task.Delay(intervalMs, cancellationToken);
                    }
                }
            }, cancellationToken);
        }
        #endregion

        #region 实用方法
        public bool IsValidNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            
            return float.TryParse(value, out _);
        }

        private bool ValidateNodeId(string nodeId)
        {
            return !string.IsNullOrWhiteSpace(nodeId);
        }
        #endregion

        #region 多PLC状态管理
        /// <summary>
        /// 初始化默认PLC配置
        /// </summary>
        public void InitializeDefaultPlcConfigurations()
        {
            // 添加默认PLC1配置
            //var defaultPlc = new PlcStatusModel(
            //    "PLC1", 
            //    "主控PLC", 
            //    "1214.PLC1._System._NoError", 
            //    "主控制系统PLC", 
            //    priority: 1
            //);
            //_plcStatusMap.TryAdd(defaultPlc.PlcId, defaultPlc);
         
        }

        /// <summary>
        /// 获取所有PLC状态信息
        /// </summary>
        public List<PlcStatusModel> GetAllPlcStatus()
        {
            var result = _plcStatusMap.Values.ToList();
            result.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            return result;
        }

        /// <summary>
        /// 获取指定PLC状态
        /// </summary>
        public PlcStatusModel GetPlcStatus(string plcId)
        {
            if (string.IsNullOrWhiteSpace(plcId)) return null;
            return _plcStatusMap.TryGetValue(plcId, out var status) ? status : null;
        }

        /// <summary>
        /// 添加或更新PLC配置
        /// </summary>
        public bool AddOrUpdatePlc(PlcStatusModel plcModel)
        {
            if (plcModel == null || string.IsNullOrWhiteSpace(plcModel.PlcId)) 
                return false;
                
            _plcStatusMap.AddOrUpdate(plcModel.PlcId, plcModel, (key, oldValue) => plcModel);
            return true;
        }

        /// <summary>
        /// 移除PLC配置
        /// </summary>
        public bool RemovePlc(string plcId)
        {
            if (string.IsNullOrWhiteSpace(plcId)) return false;
            return _plcStatusMap.TryRemove(plcId, out _);
        }

        /// <summary>
        /// 更新指定PLC的系统状态
        /// </summary>
        public async Task<bool> UpdatePlcSystemStatusAsync(string plcId)
        {
            var plcModel = GetPlcStatus(plcId);
            if (plcModel == null || !plcModel.IsEnabled)
                return false;

            try
            {
                // 使用重试机制读取系统状态
                bool currentStatus = await ExecuteWithRetryAsync(async () => 
                    await ReadBoolAsync(plcModel.SystemStatusNodeId), false);
                    
                bool previousStatus = plcModel.IsSystemNormal;
                
                // 更新状态
                plcModel.UpdateSystemStatus(currentStatus);
                
                // 如果状态发生变化，触发事件
                if (currentStatus != previousStatus)
                {
                    var eventArgs = new PlcStatusChangedEventArgs(
                        plcModel.PlcId, 
                        plcModel.PlcName, 
                        previousStatus, 
                        currentStatus
                    );
                    
                    PlcStatusChanged?.Invoke(this, eventArgs);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                // 设置连接失败状态
                bool previousStatus = plcModel.IsSystemNormal;
                plcModel.SetConnectionFailed(ex.Message);
                
                // 触发状态变化事件
                if (previousStatus != false)
                {
                    var eventArgs = new PlcStatusChangedEventArgs(
                        plcModel.PlcId, 
                        plcModel.PlcName, 
                        previousStatus, 
                        false, 
                        ex.Message
                    );
                    
                    PlcStatusChanged?.Invoke(this, eventArgs);
                }
                
                return false;
            }
        }

        /// <summary>
        /// 更新所有PLC的系统状态（异步并行）
        /// </summary>
        public async Task<int> UpdateAllPlcSystemStatusAsync()
        {
            var allPlcs = GetAllPlcStatus().Where(plc => plc.IsEnabled).ToList();
            if (!allPlcs.Any()) return 0;
            
            // 使用信号量控制并发数量，避免过多并发操作
            var semaphore = new SemaphoreSlim(Math.Min(allPlcs.Count, 10), Math.Min(allPlcs.Count, 10));
            
            try
            {
                var updateTasks = allPlcs.Select(async plc =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        return await UpdatePlcSystemStatusAsync(plc.PlcId) ? 1 : 0;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
                
                var results = await Task.WhenAll(updateTasks);
                return results.Sum();
            }
            finally
            {
                semaphore?.Dispose();
            }
        }

        /// <summary>
        /// 检查指定PLC是否可以安全读取数据
        /// </summary>
        public bool CanSafelyReadPlcData(string plcId)
        {
            var plcModel = GetPlcStatus(plcId);
            if (plcModel == null || !plcModel.IsEnabled)
                return false;
                
            return _isConnected && plcModel.IsConnected && plcModel.IsSystemNormal && 
                   !plcModel.IsStatusExpired();
        }

        /// <summary>
        /// 获取正常状态的PLC数量
        /// </summary>
        public int GetNormalPlcCount()
        {
            return _plcStatusMap.Values.Count(plc => plc.IsEnabled && plc.IsSystemNormal);
        }

        /// <summary>
        /// 获取异常状态的PLC数量
        /// </summary>
        public int GetAbnormalPlcCount()
        {
            return _plcStatusMap.Values.Count(plc => plc.IsEnabled && (!plc.IsConnected || !plc.IsSystemNormal));
        }
        #endregion

        #region 资源释放
        /// <summary>
        /// 异步释放资源
        /// </summary>
        public async Task DisposeAsync()
        {
            _cancellationTokenSource.Cancel();
            
            if (_queueProcessor != null)
            {
                try
                {
                    await _queueProcessor;
                }
                catch (OperationCanceledException)
                {
                    // 正常取消操作
                }
            }

            await _clientSemaphore.WaitAsync();
            try
            {
                _opcClient = null;
                _isConnected = false;
                OnConnectionStatusChanged(false);
            }
            finally
            {
                _clientSemaphore.Release();
            }

            _clientSemaphore?.Dispose();
            _queueSemaphore?.Dispose();
            _cancellationTokenSource?.Dispose();
        }

        public void Dispose()
        {
            _ = DisposeAsync();
        }
        #endregion
    }
}