using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace S7NET.Services
{
    /// <summary>
    /// PLC连接状态信息
    /// </summary>
    public class PlcConnectionStatus
    {
        public string PlcId { get; set; }
        public string Name { get; set; }
        public bool IsConnected { get; set; }
        public DateTime LastConnectTime { get; set; }
        public DateTime LastHeartbeatTime { get; set; }
        public string LastError { get; set; }
        public int ReconnectAttempts { get; set; }
    }

    /// <summary>
    /// 多PLC连接管理器
    /// </summary>
    public class MultiPlcConnectionManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, IS7CommunicationService> _plcConnections;
        private readonly ConcurrentDictionary<string, PlcConnectionStatus> _connectionStatus;
        private readonly MultiPlcConnectionConfig _config;
        private bool _disposed = false;

        /// <summary>
        /// 连接状态变化事件
        /// </summary>
        public event EventHandler<(string PlcId, bool IsConnected)> ConnectionStatusChanged;

        /// <summary>
        /// 通讯错误事件
        /// </summary>
        public event EventHandler<(string PlcId, string Error)> CommunicationError;

        /// <summary>
        /// 所有PLC连接状态
        /// </summary>
        public IReadOnlyDictionary<string, PlcConnectionStatus> ConnectionStatuses => _connectionStatus;

        /// <summary>
        /// 默认PLC ID
        /// </summary>
        public string DefaultPlcId => _config.DefaultPlcId;

        public MultiPlcConnectionManager(MultiPlcConnectionConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _plcConnections = new ConcurrentDictionary<string, IS7CommunicationService>();
            _connectionStatus = new ConcurrentDictionary<string, PlcConnectionStatus>();

            InitializePlcConnections();
        }

        private void InitializePlcConnections()
        {
            foreach (var plcInfo in _config.PlcConnections.Where(p => p.Enabled))
            {
                try
                {
                    var service = new S7CommunicationService(plcInfo.CpuType, plcInfo.IpAddress, plcInfo.Rack, plcInfo.Slot);
                    
                    // 订阅事件
                    service.ConnectionStatusChanged += (sender, isConnected) => OnPlcConnectionStatusChanged(plcInfo.PlcId, isConnected);
                    service.CommunicationError += (sender, error) => OnPlcCommunicationError(plcInfo.PlcId, error);

                    _plcConnections.TryAdd(plcInfo.PlcId, service);
                    _connectionStatus.TryAdd(plcInfo.PlcId, new PlcConnectionStatus
                    {
                        PlcId = plcInfo.PlcId,
                        Name = plcInfo.Name,
                        IsConnected = false,
                        LastConnectTime = DateTime.MinValue,
                        LastHeartbeatTime = DateTime.MinValue,
                        ReconnectAttempts = 0
                    });
                }
                catch (Exception ex)
                {
                    OnPlcCommunicationError(plcInfo.PlcId, $"初始化PLC连接失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 获取PLC通讯服务
        /// </summary>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns></returns>
        public IS7CommunicationService GetPlcService(string plcId = null)
        {
            if (string.IsNullOrEmpty(plcId))
                plcId = DefaultPlcId;

            if (string.IsNullOrEmpty(plcId))
                throw new InvalidOperationException("未指定PLC ID且没有默认PLC");

            if (_plcConnections.TryGetValue(plcId, out var service))
                return service;

            throw new ArgumentException($"未找到PLC连接: {plcId}");
        }

        /// <summary>
        /// 连接指定PLC
        /// </summary>
        /// <param name="plcId">PLC ID，如果为空则连接默认PLC</param>
        /// <returns></returns>
        public async Task<bool> ConnectPlcAsync(string plcId = null)
        {
            return await ConnectPlcAsync(plcId, CancellationToken.None);
        }

        /// <summary>
        /// 连接指定PLC
        /// </summary>
        /// <param name="plcId">PLC ID，如果为空则连接默认PLC</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task<bool> ConnectPlcAsync(string plcId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(plcId))
                plcId = DefaultPlcId;

            var service = GetPlcService(plcId);
            var result = await service.ConnectAsync(cancellationToken);

            if (_connectionStatus.TryGetValue(plcId, out var status))
            {
                status.LastConnectTime = DateTime.Now;
                if (result)
                {
                    status.ReconnectAttempts = 0;
                    status.LastError = null;
                }
                else
                {
                    status.ReconnectAttempts++;
                }
            }

            return result;
        }

        /// <summary>
        /// 断开指定PLC连接
        /// </summary>
        /// <param name="plcId">PLC ID，如果为空则断开默认PLC</param>
        /// <returns></returns>
        public async Task DisconnectPlcAsync(string plcId = null)
        {
            if (string.IsNullOrEmpty(plcId))
                plcId = DefaultPlcId;

            var service = GetPlcService(plcId);
            await service.DisconnectAsync();
        }

        /// <summary>
        /// 连接所有启用的PLC
        /// </summary>
        /// <returns>连接结果字典</returns>
        public async Task<Dictionary<string, bool>> ConnectAllPlcsAsync()
        {
            return await ConnectAllPlcsAsync(CancellationToken.None);
        }

        /// <summary>
        /// 连接所有启用的PLC
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>连接结果字典</returns>
        public async Task<Dictionary<string, bool>> ConnectAllPlcsAsync(CancellationToken cancellationToken)
        {
            var results = new Dictionary<string, bool>();
            var tasks = new List<Task>();

            foreach (var plcId in _config.GetEnabledPlcIds())
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var result = await ConnectPlcAsync(plcId, cancellationToken);
                        lock (results)
                        {
                            results[plcId] = result;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        lock (results)
                        {
                            results[plcId] = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        OnPlcCommunicationError(plcId, $"连接失败: {ex.Message}");
                        lock (results)
                        {
                            results[plcId] = false;
                        }
                    }
                }, cancellationToken));
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                // 连接被取消，返回当前结果
            }

            return results;
        }

        /// <summary>
        /// 断开所有PLC连接
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectAllPlcsAsync()
        {
            var tasks = new List<Task>();

            foreach (var plcId in _plcConnections.Keys)
            {
                tasks.Add(DisconnectPlcAsync(plcId));
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 检查PLC是否已连接
        /// </summary>
        /// <param name="plcId">PLC ID</param>
        /// <returns></returns>
        public bool IsPlcConnected(string plcId = null)
        {
            if (string.IsNullOrEmpty(plcId))
                plcId = DefaultPlcId;

            try
            {
                var service = GetPlcService(plcId);
                return service.IsConnected;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取所有PLC的连接状态
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, bool> GetAllConnectionStatuses()
        {
            var statuses = new Dictionary<string, bool>();

            foreach (var kvp in _plcConnections)
            {
                statuses[kvp.Key] = kvp.Value.IsConnected;
            }

            return statuses;
        }

        /// <summary>
        /// 添加新的PLC连接
        /// </summary>
        /// <param name="plcInfo">PLC连接信息</param>
        /// <returns></returns>
        public bool AddPlcConnection(PlcConnectionInfo plcInfo)
        {
            if (plcInfo == null || !plcInfo.IsValid())
                return false;

            if (_plcConnections.ContainsKey(plcInfo.PlcId))
                return false;

            try
            {
                var service = new S7CommunicationService(plcInfo.CpuType, plcInfo.IpAddress, plcInfo.Rack, plcInfo.Slot);
                
                // 订阅事件
                service.ConnectionStatusChanged += (sender, isConnected) => OnPlcConnectionStatusChanged(plcInfo.PlcId, isConnected);
                service.CommunicationError += (sender, error) => OnPlcCommunicationError(plcInfo.PlcId, error);

                _plcConnections.TryAdd(plcInfo.PlcId, service);
                _connectionStatus.TryAdd(plcInfo.PlcId, new PlcConnectionStatus
                {
                    PlcId = plcInfo.PlcId,
                    Name = plcInfo.Name,
                    IsConnected = false,
                    LastConnectTime = DateTime.MinValue,
                    LastHeartbeatTime = DateTime.MinValue,
                    ReconnectAttempts = 0
                });

                return true;
            }
            catch (Exception ex)
            {
                OnPlcCommunicationError(plcInfo.PlcId, $"添加PLC连接失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 移除PLC连接
        /// </summary>
        /// <param name="plcId">PLC ID</param>
        /// <returns></returns>
        public async Task<bool> RemovePlcConnectionAsync(string plcId)
        {
            if (string.IsNullOrEmpty(plcId))
                return false;

            try
            {
                if (_plcConnections.TryRemove(plcId, out var service))
                {
                    await service.DisconnectAsync();
                    service.Dispose();
                }

                _connectionStatus.TryRemove(plcId, out _);
                return true;
            }
            catch (Exception ex)
            {
                OnPlcCommunicationError(plcId, $"移除PLC连接失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 获取所有PLC ID列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllPlcIds()
        {
            return _plcConnections.Keys.ToList();
        }

        private void OnPlcConnectionStatusChanged(string plcId, bool isConnected)
        {
            if (_connectionStatus.TryGetValue(plcId, out var status))
            {
                status.IsConnected = isConnected;
                status.LastHeartbeatTime = DateTime.Now;
            }

            ConnectionStatusChanged?.Invoke(this, (plcId, isConnected));
        }

        private void OnPlcCommunicationError(string plcId, string error)
        {
            if (_connectionStatus.TryGetValue(plcId, out var status))
            {
                status.LastError = error;
            }

            CommunicationError?.Invoke(this, (plcId, error));
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            foreach (var service in _plcConnections.Values)
            {
                try
                {
                    service.DisconnectAsync().Wait(1000);
                    service.Dispose();
                }
                catch { }
            }

            _plcConnections.Clear();
            _connectionStatus.Clear();
        }
    }
}
