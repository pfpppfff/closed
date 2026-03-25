using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UcAsp.Opc;

namespace UserDefinedControl.OPCUA
{
    /// <summary>
    /// OPC UA 全局单例通讯服务类
    /// 提供线程安全的全局访问接口，支持多种数据类型的读写操作
    /// 实现 IOpcUaService 接口
    /// </summary>
    public sealed class OpcUaGlobalService : IOpcUaService
    {
        #region 单例模式实现
        private static readonly object _lockObject = new object();
        private static volatile OpcUaGlobalService _instance;

        /// <summary>
        /// 获取全局单例实例
        /// </summary>
        public static OpcUaGlobalService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new OpcUaGlobalService();
                        }
                    }
                }
                return _instance;
            }
        }

        private OpcUaGlobalService()
        {
            InitializeConnection();
            InitializeDefaultPlcConfigurations();
        }
        #endregion

        #region 属性和字段
        private OpcClient _opcClient;
        private readonly object _clientLock = new object();
        private bool _isConnected = false;
        
        // 多PLC状态管理
        private readonly Dictionary<string, PlcStatusModel> _plcStatusMap = new Dictionary<string, PlcStatusModel>();
        private readonly object _plcStatusLock = new object();
        
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
        #endregion

        #region 连接管理
        /// <summary>
        /// 初始化连接
        /// </summary>
        private void InitializeConnection()
        {
            try
            {
                lock (_clientLock)
                {
                    _opcClient = new OpcClient(new Uri(DefaultServerUrl));
                    _isConnected = true;
                    OnConnectionStatusChanged(true);
                }
            }
            catch (Exception ex)
            {
                _isConnected = false;
                OnConnectionStatusChanged(false);
                throw new InvalidOperationException($"OPC UA 连接初始化失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 连接到指定服务器
        /// </summary>
        /// <param name="serverUrl">服务器地址，为空则使用默认地址</param>
        public void Connect(string serverUrl = null)
        {
            Reconnect(serverUrl);
        }

        /// <summary>
        /// 重新连接到指定服务器
        /// </summary>
        /// <param name="serverUrl">服务器地址</param>
        public void Reconnect(string serverUrl = null)
        {
            try
            {
                lock (_clientLock)
                {
                    if (!string.IsNullOrWhiteSpace(serverUrl))
                    {
                        DefaultServerUrl = serverUrl;
                    }
                    
                    // OpcClient 可能不实现 IDisposable，设置为 null 即可
                    // _opcClient?.Dispose();
                    _opcClient = new OpcClient(new Uri(DefaultServerUrl));
                    _isConnected = true;
                    OnConnectionStatusChanged(true);
                }
            }
            catch (Exception ex)
            {
                _isConnected = false;
                OnConnectionStatusChanged(false);
                throw new InvalidOperationException($"OPC UA 重连失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            lock (_clientLock)
            {
                // OpcClient 可能不实现 IDisposable，设置为 null 即可
                // _opcClient?.Dispose();
                _opcClient = null;
                _isConnected = false;
                OnConnectionStatusChanged(false);
            }
        }

        private void OnConnectionStatusChanged(bool connected)
        {
            ConnectionStatusChanged?.Invoke(this, connected);
        }
        #endregion

        #region 同步读写方法
        /// <summary>
        /// 读取布尔值
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>布尔值，读取失败返回false</returns>
        public bool ReadBool(string nodeId)
        {
            if (!ValidateNodeId(nodeId)) return false;
            
            try
            {
                lock (_clientLock)
                {
                    if (!_isConnected || _opcClient == null) return false;
                    
                    var result = _opcClient.Read(new[] { nodeId });
                    return result?[0]?.Value != null ? (bool)result[0].Value : false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 写入布尔值
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        public bool WriteBool(string nodeId, bool value)
        {
            if (!ValidateNodeId(nodeId)) return false;
            
            try
            {
                lock (_clientLock)
                {
                    if (!_isConnected || _opcClient == null) return false;
                    
                    _opcClient.Write(nodeId, value);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 读取浮点数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>浮点数值，读取失败返回0.0f</returns>
        public float ReadFloat(string nodeId)
        {
            if (!ValidateNodeId(nodeId)) return 0.0f;
            
            try
            {
                lock (_clientLock)
                {
                    if (!_isConnected || _opcClient == null) return 0.0f;
                    
                    var result = _opcClient.Read(new[] { nodeId });
                    return result?[0]?.Value != null ? Convert.ToSingle(result[0].Value) : 0.0f;
                }
            }
            catch
            {
                return 0.0f;
            }
        }

        /// <summary>
        /// 写入浮点数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        public bool WriteFloat(string nodeId, float value)
        {
            if (!ValidateNodeId(nodeId)) return false;
            
            try
            {
                lock (_clientLock)
                {
                    if (!_isConnected || _opcClient == null) return false;
                    
                    _opcClient.Write(nodeId, value);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 读取16位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>整数值，读取失败返回0</returns>
        public short ReadInt16(string nodeId)
        {
            if (!ValidateNodeId(nodeId)) return 0;
            
            try
            {
                lock (_clientLock)
                {
                    if (!_isConnected || _opcClient == null) return 0;
                    
                    var result = _opcClient.Read(new[] { nodeId });
                    return result?[0]?.Value != null ? Convert.ToInt16(result[0].Value) : (short)0;
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 写入16位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        public bool WriteInt16(string nodeId, short value)
        {
            if (!ValidateNodeId(nodeId)) return false;
            
            try
            {
                lock (_clientLock)
                {
                    if (!_isConnected || _opcClient == null) return false;
                    
                    _opcClient.Write(nodeId, value);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 读取32位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>整数值，读取失败返回0</returns>
        public int ReadInt32(string nodeId)
        {
            if (!ValidateNodeId(nodeId)) return 0;
            
            try
            {
                lock (_clientLock)
                {
                    if (!_isConnected || _opcClient == null) return 0;
                    
                    var result = _opcClient.Read(new[] { nodeId });
                    return result?[0]?.Value != null ? Convert.ToInt32(result[0].Value) : 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 写入32位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        public bool WriteInt32(string nodeId, int value)
        {
            if (!ValidateNodeId(nodeId)) return false;
            
            try
            {
                lock (_clientLock)
                {
                    if (!_isConnected || _opcClient == null) return false;
                    
                    _opcClient.Write(nodeId, value);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>字符串值，读取失败返回空字符串</returns>
        public string ReadString(string nodeId)
        {
            if (!ValidateNodeId(nodeId)) return string.Empty;
            
            try
            {
                lock (_clientLock)
                {
                    if (!_isConnected || _opcClient == null) return string.Empty;
                    
                    var result = _opcClient.Read(new[] { nodeId });
                    return result?[0]?.Value?.ToString() ?? string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        public bool WriteString(string nodeId, string value)
        {
            if (!ValidateNodeId(nodeId)) return false;
            
            try
            {
                lock (_clientLock)
                {
                    if (!_isConnected || _opcClient == null) return false;
                    
                    _opcClient.Write(nodeId, value ?? string.Empty);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 异步读写方法
        /// <summary>
        /// 异步读取布尔值
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>布尔值，读取失败返回false</returns>
        public async Task<bool> ReadBoolAsync(string nodeId)
        {
            if (!ValidateNodeId(nodeId)) return false;
            
            try
            {
                if (!_isConnected || _opcClient == null) return false;
                
                return await _opcClient.ReadAsync<bool>(nodeId);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 异步写入布尔值
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        public async Task<bool> WriteBoolAsync(string nodeId, bool value)
        {
            if (!ValidateNodeId(nodeId)) return false;
            
            try
            {
                if (!_isConnected || _opcClient == null) return false;
                
                await _opcClient.WriteAsync(nodeId, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 异步读取浮点数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>浮点数值，读取失败返回0.0f</returns>
        public async Task<float> ReadFloatAsync(string nodeId)
        {
            if (!ValidateNodeId(nodeId)) return 0.0f;
            
            try
            {
                if (!_isConnected || _opcClient == null) return 0.0f;
                
                return await _opcClient.ReadAsync<float>(nodeId);
            }
            catch
            {
                return 0.0f;
            }
        }

        /// <summary>
        /// 异步写入浮点数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        public async Task<bool> WriteFloatAsync(string nodeId, float value)
        {
            if (!ValidateNodeId(nodeId)) return false;
            
            try
            {
                if (!_isConnected || _opcClient == null) return false;
                
                await _opcClient.WriteAsync(nodeId, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 异步读取16位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>整数值，读取失败返回0</returns>
        public async Task<short> ReadInt16Async(string nodeId)
        {
            if (!ValidateNodeId(nodeId)) return 0;
            
            try
            {
                if (!_isConnected || _opcClient == null) return 0;
                
                return await _opcClient.ReadAsync<short>(nodeId);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 异步写入16位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        public async Task<bool> WriteInt16Async(string nodeId, short value)
        {
            if (!ValidateNodeId(nodeId)) return false;
            
            try
            {
                if (!_isConnected || _opcClient == null) return false;
                
                await _opcClient.WriteAsync(nodeId, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 异步读取32位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>整数值，读取失败返回0</returns>
        public async Task<int> ReadInt32Async(string nodeId)
        {
            if (!ValidateNodeId(nodeId)) return 0;
            
            try
            {
                if (!_isConnected || _opcClient == null) return 0;
                
                return await _opcClient.ReadAsync<int>(nodeId);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 异步写入32位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        public async Task<bool> WriteInt32Async(string nodeId, int value)
        {
            if (!ValidateNodeId(nodeId)) return false;
            
            try
            {
                if (!_isConnected || _opcClient == null) return false;
                
                await _opcClient.WriteAsync(nodeId, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 异步读取字符串
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>字符串值，读取失败返回空字符串</returns>
        public async Task<string> ReadStringAsync(string nodeId)
        {
            if (!ValidateNodeId(nodeId)) return string.Empty;
            
            try
            {
                if (!_isConnected || _opcClient == null) return string.Empty;
                
                return await _opcClient.ReadAsync<string>(nodeId);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 异步写入字符串
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        public async Task<bool> WriteStringAsync(string nodeId, string value)
        {
            if (!ValidateNodeId(nodeId)) return false;
            
            try
            {
                if (!_isConnected || _opcClient == null) return false;
                
                await _opcClient.WriteAsync(nodeId, value ?? string.Empty);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 批量读写操作
        /// <summary>
        /// 批量读取多个节点
        /// </summary>
        /// <param name="nodeIds">节点地址数组</param>
        /// <returns>读取结果列表</returns>
        public List<OpcItemValue> ReadMultiple(params string[] nodeIds)
        {
            if (nodeIds == null || nodeIds.Length == 0) 
                return new List<OpcItemValue>();
            
            try
            {
                lock (_clientLock)
                {
                    if (!_isConnected || _opcClient == null) 
                        return new List<OpcItemValue>();
                    
                    return _opcClient.Read(nodeIds);
                }
            }
            catch
            {
                return new List<OpcItemValue>();
            }
        }

        /// <summary>
        /// 批量写入多个节点
        /// </summary>
        /// <param name="writeOperations">写入操作字典(节点地址->值)</param>
        /// <returns>是否全部写入成功</returns>
        public bool WriteMultiple(Dictionary<string, object> writeOperations)
        {
            if (writeOperations == null || writeOperations.Count == 0) 
                return false;
            
            try
            {
                lock (_clientLock)
                {
                    if (!_isConnected || _opcClient == null) return false;
                    
                    foreach (var operation in writeOperations)
                    {
                        _opcClient.Write(operation.Key, operation.Value);
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 实用工具方法
        /// <summary>
        /// 切换布尔值（读取当前值并写入相反值）
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>是否操作成功</returns>
        public bool ToggleBool(string nodeId)
        {
            try
            {
                bool currentValue = ReadBool(nodeId);
                return WriteBool(nodeId, !currentValue);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 异步切换布尔值
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>是否操作成功</returns>
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

        /// <summary>
        /// 验证节点地址是否有效
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>是否有效</returns>
        private bool ValidateNodeId(string nodeId)
        {
            return !string.IsNullOrWhiteSpace(nodeId);
        }

        /// <summary>
        /// 验证数字字符串
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>是否为有效数字</returns>
        public bool IsValidNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            
            return float.TryParse(value, out _);
        }
        #endregion

        #region 多PLC状态管理
        /// <summary>
        /// 初始化默认PLC配置
        /// </summary>
        private void InitializeDefaultPlcConfigurations()
        {
            // 添加默认PLC1配置
            var defaultPlc = new PlcStatusModel(
                "PLC1", 
                "主控PLC", 
                "1214.PLC1._System._NoError", 
                "主控制系统PLC", 
                priority: 1
            );
            
            lock (_plcStatusLock)
            {
                _plcStatusMap[defaultPlc.PlcId] = defaultPlc;
            }
        }

        /// <summary>
        /// 获取所有PLC状态信息
        /// </summary>
        /// <returns>PLC状态列表</returns>
        public List<PlcStatusModel> GetAllPlcStatus()
        {
            lock (_plcStatusLock)
            {
                var result = new List<PlcStatusModel>(_plcStatusMap.Values);
                // 按优先级排序
                result.Sort((a, b) => a.Priority.CompareTo(b.Priority));
                return result;
            }
        }

        /// <summary>
        /// 获取指定PLC状态
        /// </summary>
        /// <param name="plcId">PLC标识符</param>
        /// <returns>PLC状态信息，不存在返回null</returns>
        public PlcStatusModel GetPlcStatus(string plcId)
        {
            if (string.IsNullOrWhiteSpace(plcId)) return null;
            
            lock (_plcStatusLock)
            {
                return _plcStatusMap.TryGetValue(plcId, out var status) ? status : null;
            }
        }

        /// <summary>
        /// 添加或更新PLC配置
        /// </summary>
        /// <param name="plcModel">PLC状态模型</param>
        /// <returns>是否操作成功</returns>
        public bool AddOrUpdatePlc(PlcStatusModel plcModel)
        {
            if (plcModel == null || string.IsNullOrWhiteSpace(plcModel.PlcId)) 
                return false;
                
            lock (_plcStatusLock)
            {
                _plcStatusMap[plcModel.PlcId] = plcModel;
                return true;
            }
        }

        /// <summary>
        /// 移除PLC配置
        /// </summary>
        /// <param name="plcId">PLC标识符</param>
        /// <returns>是否移除成功</returns>
        public bool RemovePlc(string plcId)
        {
            if (string.IsNullOrWhiteSpace(plcId)) return false;
            
            lock (_plcStatusLock)
            {
                return _plcStatusMap.Remove(plcId);
            }
        }

        /// <summary>
        /// 更新指定PLC的系统状态
        /// </summary>
        /// <param name="plcId">PLC标识符</param>
        /// <returns>系统状态更新结果</returns>
        public async Task<bool> UpdatePlcSystemStatusAsync(string plcId)
        {
            var plcModel = GetPlcStatus(plcId);
            if (plcModel == null || !plcModel.IsEnabled)
                return false;

            try
            {
                // 读取系统状态
                bool currentStatus = await ReadBoolAsync(plcModel.SystemStatusNodeId);
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
        /// 更新所有PLC的系统状态
        /// </summary>
        /// <returns>更新成功的PLC数量</returns>
        public async Task<int> UpdateAllPlcSystemStatusAsync()
        {
            var allPlcs = GetAllPlcStatus();
            int successCount = 0;
            
            // 并行更新所有PLC状态
            var updateTasks = allPlcs.Where(plc => plc.IsEnabled)
                                   .Select(async plc =>
                                   {
                                       bool success = await UpdatePlcSystemStatusAsync(plc.PlcId);
                                       return success ? 1 : 0;
                                   });
            
            var results = await Task.WhenAll(updateTasks);
            successCount = results.Sum();
            
            return successCount;
        }

        /// <summary>
        /// 检查指定PLC是否可以安全读取数据
        /// </summary>
        /// <param name="plcId">PLC标识符</param>
        /// <returns>是否可以安全读取</returns>
        public bool CanSafelyReadPlcData(string plcId)
        {
            var plcModel = GetPlcStatus(plcId);
            if (plcModel == null || !plcModel.IsEnabled)
                return false;
                
            return plcModel.IsConnected && plcModel.IsSystemNormal && 
                   !plcModel.IsStatusExpired();
        }

        /// <summary>
        /// 获取正常状态的PLC数量
        /// </summary>
        /// <returns>正常状态的PLC数量</returns>
        public int GetNormalPlcCount()
        {
            lock (_plcStatusLock)
            {
                return _plcStatusMap.Values.Count(plc => plc.IsEnabled && plc.IsSystemNormal);
            }
        }

        /// <summary>
        /// 获取异常状态的PLC数量
        /// </summary>
        /// <returns>异常状态的PLC数量</returns>
        public int GetAbnormalPlcCount()
        {
            lock (_plcStatusLock)
            {
                return _plcStatusMap.Values.Count(plc => plc.IsEnabled && (!plc.IsConnected || !plc.IsSystemNormal));
            }
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 根据PLC ID自动创建默认的PLC配置
        /// </summary>
        /// <param name="plcId">PLC标识符</param>
        /// <returns>创建的PLC模型</returns>
        public PlcStatusModel CreateDefaultPlcConfig(string plcId)
        {
            if (string.IsNullOrWhiteSpace(plcId)) return null;
            
            var plcModel = new PlcStatusModel(
                plcId,
                $"PLC_{plcId}",
                $"1214.{plcId}._System._NoError",
                $"PLC {plcId} 系统"
            );
            
            AddOrUpdatePlc(plcModel);
            return plcModel;
        }

        /// <summary>
        /// 批量添加PLC配置
        /// </summary>
        /// <param name="plcConfigs">PLC配置列表</param>
        /// <returns>成功添加的数量</returns>
        public int BatchAddPlcConfigs(List<PlcStatusModel> plcConfigs)
        {
            if (plcConfigs == null) return 0;
            
            int successCount = 0;
            foreach (var config in plcConfigs)
            {
                if (AddOrUpdatePlc(config))
                    successCount++;
            }
            
            return successCount;
        }
        #endregion

        #region 资源释放
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            lock (_clientLock)
            {
                // OpcClient 可能不实现 IDisposable，设置为 null 即可
                // _opcClient?.Dispose();
                _opcClient = null;
                _isConnected = false;
                OnConnectionStatusChanged(false);
            }
        }
        #endregion
    }
}