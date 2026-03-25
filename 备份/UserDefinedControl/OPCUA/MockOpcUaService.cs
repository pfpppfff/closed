using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UcAsp.Opc;

namespace UserDefinedControl.OPCUA
{
    /// <summary>
    /// OPC UA 模拟服务实现
    /// 用于测试和开发环境，不需要实际的OPC UA服务器
    /// </summary>
    public class MockOpcUaService : IOpcUaService
    {
        #region 私有字段
        private readonly Dictionary<string, object> _virtualNodes = new Dictionary<string, object>();
        private readonly Dictionary<string, PlcStatusModel> _plcStatusMap = new Dictionary<string, PlcStatusModel>();
        private readonly object _lockObject = new object();
        private bool _isConnected = false;
        #endregion

        #region 属性
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool IsConnected => _isConnected;

        /// <summary>
        /// 默认服务器地址
        /// </summary>
        public string DefaultServerUrl { get; set; } = "mock://127.0.0.1:49320";

        /// <summary>
        /// 连接状态变化事件
        /// </summary>
        public event EventHandler<bool> ConnectionStatusChanged;
        
        /// <summary>
        /// PLC状态变化事件
        /// </summary>
        public event EventHandler<PlcStatusChangedEventArgs> PlcStatusChanged;
        #endregion

        #region 构造函数
        public MockOpcUaService()
        {
            InitializeMockData();
            InitializeDefaultPlcConfigurations();
        }

        private void InitializeMockData()
        {
            // 初始化一些模拟数据
            _virtualNodes["1214.PLC1.System.Running"] = true;
            _virtualNodes["1214.PLC1.DisData.Temp.temp_1"] = 25.5f;
            _virtualNodes["1214.PLC1.DisData.Temp.temp_2"] = 26.0f;
            _virtualNodes["1214.PLC1.DisData.Flow.Flow_1"] = 15.2f;
            _virtualNodes["1214.PLC1.Control.SetPoint"] = 30.0f;
            _virtualNodes["1214.PLC1.Control.Mode"] = (short)1;
            _virtualNodes["1214.PLC1.Status.Code"] = 100;
            _virtualNodes["1214.PLC1.System.StatusText"] = "系统正常";
        }
        #endregion

        #region 连接管理
        public void Connect(string serverUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(serverUrl))
            {
                DefaultServerUrl = serverUrl;
            }
            
            _isConnected = true;
            OnConnectionStatusChanged(true);
        }

        public void Reconnect(string serverUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(serverUrl))
            {
                DefaultServerUrl = serverUrl;
            }
            
            _isConnected = true;
            OnConnectionStatusChanged(true);
        }

        public void Disconnect()
        {
            _isConnected = false;
            OnConnectionStatusChanged(false);
        }

        private void OnConnectionStatusChanged(bool connected)
        {
            ConnectionStatusChanged?.Invoke(this, connected);
        }
        #endregion

        #region 同步读写方法
        public bool ReadBool(string nodeId)
        {
            if (!_isConnected || string.IsNullOrWhiteSpace(nodeId))
                return false;

            if (_virtualNodes.TryGetValue(nodeId, out object value))
            {
                return value is bool boolValue ? boolValue : false;
            }
            
            // 如果节点不存在，返回随机值模拟实际情况
            return new Random().Next(0, 2) == 1;
        }

        public bool WriteBool(string nodeId, bool value)
        {
            if (!_isConnected || string.IsNullOrWhiteSpace(nodeId))
                return false;

            _virtualNodes[nodeId] = value;
            return true;
        }

        public float ReadFloat(string nodeId)
        {
            if (!_isConnected || string.IsNullOrWhiteSpace(nodeId))
                return 0.0f;

            if (_virtualNodes.TryGetValue(nodeId, out object value))
            {
                return value is float floatValue ? floatValue : 
                       value is double doubleValue ? (float)doubleValue :
                       value is int intValue ? (float)intValue : 0.0f;
            }
            
            // 返回随机值模拟传感器数据
            return (float)(new Random().NextDouble() * 100);
        }

        public bool WriteFloat(string nodeId, float value)
        {
            if (!_isConnected || string.IsNullOrWhiteSpace(nodeId))
                return false;

            _virtualNodes[nodeId] = value;
            return true;
        }

        public short ReadInt16(string nodeId)
        {
            if (!_isConnected || string.IsNullOrWhiteSpace(nodeId))
                return 0;

            if (_virtualNodes.TryGetValue(nodeId, out object value))
            {
                return value is short shortValue ? shortValue :
                       value is int intValue ? (short)intValue : (short)0;
            }
            
            return (short)new Random().Next(0, 1000);
        }

        public bool WriteInt16(string nodeId, short value)
        {
            if (!_isConnected || string.IsNullOrWhiteSpace(nodeId))
                return false;

            _virtualNodes[nodeId] = value;
            return true;
        }

        public int ReadInt32(string nodeId)
        {
            if (!_isConnected || string.IsNullOrWhiteSpace(nodeId))
                return 0;

            if (_virtualNodes.TryGetValue(nodeId, out object value))
            {
                return value is int intValue ? intValue :
                       value is short shortValue ? shortValue : 0;
            }
            
            return new Random().Next(0, 10000);
        }

        public bool WriteInt32(string nodeId, int value)
        {
            if (!_isConnected || string.IsNullOrWhiteSpace(nodeId))
                return false;

            _virtualNodes[nodeId] = value;
            return true;
        }

        public string ReadString(string nodeId)
        {
            if (!_isConnected || string.IsNullOrWhiteSpace(nodeId))
                return string.Empty;

            if (_virtualNodes.TryGetValue(nodeId, out object value))
            {
                return value?.ToString() ?? string.Empty;
            }
            
            return $"Mock_{nodeId}";
        }

        public bool WriteString(string nodeId, string value)
        {
            if (!_isConnected || string.IsNullOrWhiteSpace(nodeId))
                return false;

            _virtualNodes[nodeId] = value ?? string.Empty;
            return true;
        }
        #endregion

        #region 异步读写方法
        public async Task<bool> ReadBoolAsync(string nodeId)
        {
            // 模拟异步延迟
            await Task.Delay(10);
            return ReadBool(nodeId);
        }

        public async Task<bool> WriteBoolAsync(string nodeId, bool value)
        {
            await Task.Delay(10);
            return WriteBool(nodeId, value);
        }

        public async Task<float> ReadFloatAsync(string nodeId)
        {
            await Task.Delay(10);
            return ReadFloat(nodeId);
        }

        public async Task<bool> WriteFloatAsync(string nodeId, float value)
        {
            await Task.Delay(10);
            return WriteFloat(nodeId, value);
        }

        public async Task<short> ReadInt16Async(string nodeId)
        {
            await Task.Delay(10);
            return ReadInt16(nodeId);
        }

        public async Task<bool> WriteInt16Async(string nodeId, short value)
        {
            await Task.Delay(10);
            return WriteInt16(nodeId, value);
        }

        public async Task<int> ReadInt32Async(string nodeId)
        {
            await Task.Delay(10);
            return ReadInt32(nodeId);
        }

        public async Task<bool> WriteInt32Async(string nodeId, int value)
        {
            await Task.Delay(10);
            return WriteInt32(nodeId, value);
        }

        public async Task<string> ReadStringAsync(string nodeId)
        {
            await Task.Delay(10);
            return ReadString(nodeId);
        }

        public async Task<bool> WriteStringAsync(string nodeId, string value)
        {
            await Task.Delay(10);
            return WriteString(nodeId, value);
        }
        #endregion

        #region 批量操作
        public List<OpcItemValue> ReadMultiple(params string[] nodeIds)
        {
            var results = new List<OpcItemValue>();
            
            if (!_isConnected || nodeIds == null)
                return results;

            foreach (string nodeId in nodeIds)
            {
                object value = null;
                if (_virtualNodes.TryGetValue(nodeId, out value))
                {
                    // 这里需要模拟 OpcItemValue 结构
                    // 由于我们没有访问真实的OpcItemValue构造函数，先返回空列表
                    // 在实际使用中需要根据UcAsp.Opc库的实际结构调整
                }
            }
            
            return results;
        }

        public bool WriteMultiple(Dictionary<string, object> writeOperations)
        {
            if (!_isConnected || writeOperations == null)
                return false;

            try
            {
                foreach (var operation in writeOperations)
                {
                    _virtualNodes[operation.Key] = operation.Value;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 实用方法
        public bool ToggleBool(string nodeId)
        {
            bool currentValue = ReadBool(nodeId);
            return WriteBool(nodeId, !currentValue);
        }

        public async Task<bool> ToggleBoolAsync(string nodeId)
        {
            bool currentValue = await ReadBoolAsync(nodeId);
            return await WriteBoolAsync(nodeId, !currentValue);
        }

        public bool IsValidNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            
            return float.TryParse(value, out _);
        }
        #endregion

        #region 模拟特定方法
        /// <summary>
        /// 设置模拟节点值
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">值</param>
        public void SetMockValue(string nodeId, object value)
        {
            _virtualNodes[nodeId] = value;
        }

        /// <summary>
        /// 获取所有模拟节点
        /// </summary>
        /// <returns>节点字典</returns>
        public Dictionary<string, object> GetAllMockValues()
        {
            return new Dictionary<string, object>(_virtualNodes);
        }

        /// <summary>
        /// 清除所有模拟数据
        /// </summary>
        public void ClearMockData()
        {
            _virtualNodes.Clear();
        }
        #endregion

        #region 多PLC状态管理
        /// <summary>
        /// 初始化默认PLC配置
        /// </summary>
        private void InitializeDefaultPlcConfigurations()
        {
            lock (_lockObject)
            {
                // 添加默认PLC1配置
                var defaultPlc = new PlcStatusModel(
                    "PLC1", 
                    "主控PLC", 
                    "1214.PLC1._System._NoError", 
                    "主控制系统PLC", 
                    priority: 1
                );
                
                _plcStatusMap[defaultPlc.PlcId] = defaultPlc;
                
                // 初始化系统状态节点数据
                _virtualNodes["1214.PLC1._System._NoError"] = true;
            }
        }

        /// <summary>
        /// 获取所有PLC状态信息
        /// </summary>
        public List<PlcStatusModel> GetAllPlcStatus()
        {
            lock (_lockObject)
            {
                var result = new List<PlcStatusModel>(_plcStatusMap.Values);
                result.Sort((a, b) => a.Priority.CompareTo(b.Priority));
                return result;
            }
        }

        /// <summary>
        /// 获取指定PLC状态
        /// </summary>
        public PlcStatusModel GetPlcStatus(string plcId)
        {
            if (string.IsNullOrWhiteSpace(plcId)) return null;
            
            lock (_lockObject)
            {
                return _plcStatusMap.TryGetValue(plcId, out var status) ? status : null;
            }
        }

        /// <summary>
        /// 添加或更新PLC配置
        /// </summary>
        public bool AddOrUpdatePlc(PlcStatusModel plcModel)
        {
            if (plcModel == null || string.IsNullOrWhiteSpace(plcModel.PlcId)) 
                return false;
                
            lock (_lockObject)
            {
                _plcStatusMap[plcModel.PlcId] = plcModel;
                // 初始化模拟系统状态节点
                _virtualNodes[plcModel.SystemStatusNodeId] = true;
                return true;
            }
        }

        /// <summary>
        /// 移除PLC配置
        /// </summary>
        public bool RemovePlc(string plcId)
        {
            if (string.IsNullOrWhiteSpace(plcId)) return false;
            
            lock (_lockObject)
            {
                if (_plcStatusMap.TryGetValue(plcId, out var plc))
                {
                    _plcStatusMap.Remove(plcId);
                    // 移除相关的模拟节点
                    _virtualNodes.Remove(plc.SystemStatusNodeId);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 更新指定PLC的系统状态（模拟实现）
        /// </summary>
        public async Task<bool> UpdatePlcSystemStatusAsync(string plcId)
        {
            var plcModel = GetPlcStatus(plcId);
            if (plcModel == null || !plcModel.IsEnabled)
                return false;

            try
            {
                // 模拟异步延迟
                await Task.Delay(50);
                
                // 读取模拟系统状态
                bool currentStatus = ReadBool(plcModel.SystemStatusNodeId);
                bool previousStatus = plcModel.IsSystemNormal;
                
                // 模拟随机状态变化（小概率）
                if (new Random().Next(0, 100) < 5) // 5%概率变化状态
                {
                    currentStatus = !currentStatus;
                    _virtualNodes[plcModel.SystemStatusNodeId] = currentStatus;
                }
                
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
                // 模拟连接失败
                bool previousStatus = plcModel.IsSystemNormal;
                plcModel.SetConnectionFailed(ex.Message);
                
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
        /// 更新所有PLC的系统状态（模拟实现）
        /// </summary>
        public async Task<int> UpdateAllPlcSystemStatusAsync()
        {
            var allPlcs = GetAllPlcStatus().Where(plc => plc.IsEnabled).ToList();
            if (!allPlcs.Any()) return 0;
            
            // 模拟并行更新
            var updateTasks = allPlcs.Select(async plc =>
            {
                return await UpdatePlcSystemStatusAsync(plc.PlcId) ? 1 : 0;
            });
            
            var results = await Task.WhenAll(updateTasks);
            return results.Sum();
        }

        /// <summary>
        /// 检查指定PLC是否可以安全读取数据（模拟实现）
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
            lock (_lockObject)
            {
                return _plcStatusMap.Values.Count(plc => plc.IsEnabled && plc.IsSystemNormal);
            }
        }

        /// <summary>
        /// 获取异常状态的PLC数量
        /// </summary>
        public int GetAbnormalPlcCount()
        {
            lock (_lockObject)
            {
                return _plcStatusMap.Values.Count(plc => plc.IsEnabled && (!plc.IsConnected || !plc.IsSystemNormal));
            }
        }
        #endregion
    }
}