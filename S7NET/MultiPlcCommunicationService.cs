using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace S7NET.Services
{
    /// <summary>
    /// 多PLC通讯服务实现类
    /// </summary>
    public class MultiPlcCommunicationService : IMultiPlcCommunicationService
    {
        private readonly MultiPlcConnectionManager _connectionManager;
        private readonly MultiPlcConnectionConfig _config;
        private bool _disposed = false;

        public string DefaultPlcId => _connectionManager.DefaultPlcId;

        public IReadOnlyDictionary<string, PlcConnectionStatus> ConnectionStatuses => _connectionManager.ConnectionStatuses;

        public event EventHandler<(string PlcId, bool IsConnected)> ConnectionStatusChanged;
        public event EventHandler<(string PlcId, string Error)> CommunicationError;

        public MultiPlcCommunicationService(MultiPlcConnectionConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _connectionManager = new MultiPlcConnectionManager(config);

            // 转发事件
            _connectionManager.ConnectionStatusChanged += (sender, args) => ConnectionStatusChanged?.Invoke(this, args);
            _connectionManager.CommunicationError += (sender, args) => CommunicationError?.Invoke(this, args);
        }

        #region 连接管理

        public async Task<bool> ConnectPlcAsync(string plcId = null)
        {
            return await _connectionManager.ConnectPlcAsync(plcId, CancellationToken.None);
        }

        public async Task<bool> ConnectPlcAsync(string plcId, CancellationToken cancellationToken)
        {
            return await _connectionManager.ConnectPlcAsync(plcId, cancellationToken);
        }

        public async Task DisconnectPlcAsync(string plcId = null)
        {
            await _connectionManager.DisconnectPlcAsync(plcId);
        }

        public async Task<Dictionary<string, bool>> ConnectAllPlcsAsync()
        {
            return await _connectionManager.ConnectAllPlcsAsync(CancellationToken.None);
        }

        public async Task<Dictionary<string, bool>> ConnectAllPlcsAsync(CancellationToken cancellationToken)
        {
            return await _connectionManager.ConnectAllPlcsAsync(cancellationToken);
        }

        public async Task DisconnectAllPlcsAsync()
        {
            await _connectionManager.DisconnectAllPlcsAsync();
        }

        public bool IsPlcConnected(string plcId = null)
        {
            return _connectionManager.IsPlcConnected(plcId);
        }

        public Dictionary<string, bool> GetAllConnectionStatuses()
        {
            return _connectionManager.GetAllConnectionStatuses();
        }

        public List<string> GetAllPlcIds()
        {
            return _connectionManager.GetAllPlcIds();
        }

        #endregion

        #region 数据读取

        public async Task<T> ReadAsync<T>(string address, string plcId = null)
        {
            var service = _connectionManager.GetPlcService(plcId);
            return await service.ReadAsync<T>(address);
        }

        public async Task<Dictionary<string, object>> ReadMultipleAsync(string[] addresses, string plcId = null)
        {
            var service = _connectionManager.GetPlcService(plcId);
            return await service.ReadMultipleAsync(addresses);
        }

        public async Task<Dictionary<string, object>> ReadMultiplePlcsAsync(Dictionary<string, string> addressPlcPairs)
        {
            var results = new Dictionary<string, object>();
            var tasks = new List<Task>();

            // 按PLC分组
            var plcGroups = addressPlcPairs.GroupBy(kvp => kvp.Value ?? DefaultPlcId);

            foreach (var group in plcGroups)
            {
                var plcId = group.Key;
                var addresses = group.Select(g => g.Key).ToArray();

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var service = _connectionManager.GetPlcService(plcId);
                        var plcResults = await service.ReadMultipleAsync(addresses);

                        lock (results)
                        {
                            foreach (var result in plcResults)
                            {
                                results[result.Key] = result.Value;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 记录错误，但不中断其他PLC的读取
                        foreach (var address in addresses)
                        {
                            lock (results)
                            {
                                results[address] = null;
                            }
                        }
                        CommunicationError?.Invoke(this, (plcId, $"批量读取失败: {ex.Message}"));
                    }
                }));
            }

            await Task.WhenAll(tasks);
            return results;
        }

        public async Task<T[]> ReadDBAsync<T>(int dbNumber, int startByte, int count = 1, string plcId = null)
        {
            var service = _connectionManager.GetPlcService(plcId);
            return await service.ReadDBAsync<T>(dbNumber, startByte, count);
        }

        public async Task<bool> ReadBitAsync(string address, string plcId = null)
        {
            var service = _connectionManager.GetPlcService(plcId);
            return await service.ReadBitAsync(address);
        }

        public async Task<bool[]> ReadQBitsAsync(int startByte, int startBit, int length, string plcId = null)
        {
            var service = _connectionManager.GetPlcService(plcId);
            return await service.ReadQBitsAsync(startByte, startBit, length);
        }

        public async Task<bool[]> ReadIBitsAsync(int startByte, int startBit, int length, string plcId = null)
        {
            var service = _connectionManager.GetPlcService(plcId);
            return await service.ReadIBitsAsync(startByte, startBit, length);
        }

        public async Task<T[]> ReadMAsync<T>(int startByte, int count = 1, string plcId = null)
        {
            var service = _connectionManager.GetPlcService(plcId);
            return await service.ReadMAsync<T>(startByte, count);
        }

        public async Task<bool[]> ReadMBitsAsync(int startByte, int startBit, int length, string plcId = null)
        {
            var service = _connectionManager.GetPlcService(plcId);
            return await service.ReadMBitsAsync(startByte, startBit, length);
        }

        public async Task<bool> WriteQBitsAsync(int startByte, int startBit, bool[] values, string plcId = null)
        {
            var service = _connectionManager.GetPlcService(plcId);
            return await service.WriteQBitsAsync(startByte, startBit, values);
        }

        public async Task<bool> WriteQBitAsync(int byteAddress, int bitAddress, bool value, string plcId = null)
        {
            var service = _connectionManager.GetPlcService(plcId);
            return await service.WriteQBitAsync(byteAddress, bitAddress, value);
        }

        public async Task<bool> WriteMAsync<T>(int startByte, T[] values, string plcId = null)
        {
            var service = _connectionManager.GetPlcService(plcId);
            return await service.WriteMAsync(startByte, values);
        }

        public async Task<bool> WriteMSingleAsync<T>(int startByte, T value, string plcId = null)
        {
            var service = _connectionManager.GetPlcService(plcId);
            return await service.WriteMSingleAsync(startByte, value);
        }

        public async Task<bool> WriteMBitsAsync(int startByte, int startBit, bool[] values, string plcId = null)
        {
            var service = _connectionManager.GetPlcService(plcId);
            return await service.WriteMBitsAsync(startByte, startBit, values);
        }

        #endregion

        #region 数据写入

        public async Task<bool> WriteAsync<T>(string address, T value, string plcId = null)
        {
            var service = _connectionManager.GetPlcService(plcId);
            return await service.WriteAsync(address, value);
        }

        public async Task<bool> WriteMultipleAsync(Dictionary<string, object> data, string plcId = null)
        {
            var service = _connectionManager.GetPlcService(plcId);
            return await service.WriteMultipleAsync(data);
        }

        public async Task<Dictionary<string, bool>> WriteMultiplePlcsAsync(Dictionary<string, (object Value, string PlcId)> dataPlcPairs)
        {
            var results = new Dictionary<string, bool>();
            var tasks = new List<Task>();

            // 按PLC分组
            var plcGroups = dataPlcPairs.GroupBy(kvp => kvp.Value.PlcId ?? DefaultPlcId);

            foreach (var group in plcGroups)
            {
                var plcId = group.Key;
                var plcData = group.ToDictionary(g => g.Key, g => g.Value.Value);

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var service = _connectionManager.GetPlcService(plcId);
                        var success = await service.WriteMultipleAsync(plcData);

                        lock (results)
                        {
                            foreach (var address in plcData.Keys)
                            {
                                results[address] = success;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 记录错误
                        foreach (var address in plcData.Keys)
                        {
                            lock (results)
                            {
                                results[address] = false;
                            }
                        }
                        CommunicationError?.Invoke(this, (plcId, $"批量写入失败: {ex.Message}"));
                    }
                }));
            }

            await Task.WhenAll(tasks);
            return results;
        }

        public async Task<bool> WriteDBAsync<T>(int dbNumber, int startByte, T value, string plcId = null)
        {
            var service = _connectionManager.GetPlcService(plcId);
            return await service.WriteDBAsync(dbNumber, startByte, value);
        }

        public async Task<bool> WriteBitAsync(string address, bool value, string plcId = null)
        {
            var service = _connectionManager.GetPlcService(plcId);
            return await service.WriteBitAsync(address, value);
        }



        #endregion

        #region PLC管理

        public bool AddPlcConnection(PlcConnectionInfo plcInfo)
        {
            return _connectionManager.AddPlcConnection(plcInfo);
        }

        public async Task<bool> RemovePlcConnectionAsync(string plcId)
        {
            return await _connectionManager.RemovePlcConnectionAsync(plcId);
        }

        public PlcConnectionInfo GetPlcConfig(string plcId)
        {
            return _config.GetPlcConfig(plcId);
        }

        #endregion

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            _connectionManager?.Dispose();
        }
    }
}
