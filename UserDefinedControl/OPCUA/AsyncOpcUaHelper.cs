using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UserDefinedControl.OPCUA
{
    /// <summary>
    /// 异步优先的 OPC UA 快捷访问助手类
    /// 提供简化的异步接口，避免UI阻塞，适合工业自动化多控件场景
    /// </summary>
    public static class AsyncOpcUaHelper
    {
        /// <summary>
        /// 获取异步优化的服务实例
        /// </summary>
        private static IOpcUaService Service => OpcUaServiceManager.Current;

        #region 连接管理
        /// <summary>
        /// 检查连接状态
        /// </summary>
        public static bool IsConnected => Service.IsConnected;

        /// <summary>
        /// 异步连接到指定服务器
        /// </summary>
        /// <param name="serverUrl">服务器地址，为空则使用默认地址</param>
        public static async Task ConnectAsync(string serverUrl = null)
        {
            // 检查是否为异步服务
            var asyncServiceType = Service.GetType();
            if (asyncServiceType.Name == "AsyncOpcUaService")
            {
                var connectAsyncMethod = asyncServiceType.GetMethod("ConnectAsync");
                if (connectAsyncMethod != null)
                {
                    var task = (Task)connectAsyncMethod.Invoke(Service, new object[] { serverUrl });
                    await task;
                    return;
                }
            }
            
            // 如果不是异步服务，使用普通连接
            Service.Connect(serverUrl);
        }

        /// <summary>
        /// 异步重新连接
        /// </summary>
        /// <param name="serverUrl">服务器地址，为空则使用默认地址</param>
        public static async Task ReconnectAsync(string serverUrl = null)
        {
            var asyncServiceType = Service.GetType();
            if (asyncServiceType.Name == "AsyncOpcUaService")
            {
                var reconnectAsyncMethod = asyncServiceType.GetMethod("ReconnectAsync");
                if (reconnectAsyncMethod != null)
                {
                    var task = (Task)reconnectAsyncMethod.Invoke(Service, new object[] { serverUrl });
                    await task;
                    return;
                }
            }
            
            Service.Reconnect(serverUrl);
        }

        /// <summary>
        /// 异步断开连接
        /// </summary>
        public static async Task DisconnectAsync()
        {
            var asyncServiceType = Service.GetType();
            if (asyncServiceType.Name == "AsyncOpcUaService")
            {
                var disconnectAsyncMethod = asyncServiceType.GetMethod("DisconnectAsync");
                if (disconnectAsyncMethod != null)
                {
                    var task = (Task)disconnectAsyncMethod.Invoke(Service, null);
                    await task;
                    return;
                }
            }
            
            Service.Disconnect();
        }

        /// <summary>
        /// 同步连接（不推荐，可能阻塞UI）
        /// </summary>
        public static void Connect(string serverUrl = null) => Service.Connect(serverUrl);

        /// <summary>
        /// 同步重连（不推荐，可能阻塞UI）
        /// </summary>
        public static void Reconnect(string serverUrl = null) => Service.Reconnect(serverUrl);

        /// <summary>
        /// 同步断开（不推荐，可能阻塞UI）
        /// </summary>
        public static void Disconnect() => Service.Disconnect();
        #endregion

        #region 异步布尔值操作（推荐使用）
        /// <summary>
        /// 异步读取布尔值
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>布尔值</returns>
        public static async Task<bool> ReadBoolAsync(string nodeId) => await Service.ReadBoolAsync(nodeId);

        /// <summary>
        /// 异步写入布尔值
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> WriteBoolAsync(string nodeId, bool value) => await Service.WriteBoolAsync(nodeId, value);

        /// <summary>
        /// 异步切换布尔值
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> ToggleBoolAsync(string nodeId) => await Service.ToggleBoolAsync(nodeId);
        #endregion

        #region 异步数值操作（推荐使用）
        /// <summary>
        /// 异步读取浮点数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>浮点数值</returns>
        public static async Task<float> ReadFloatAsync(string nodeId) => await Service.ReadFloatAsync(nodeId);

        /// <summary>
        /// 异步写入浮点数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> WriteFloatAsync(string nodeId, float value) => await Service.WriteFloatAsync(nodeId, value);

        /// <summary>
        /// 异步从字符串写入浮点数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">字符串值</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> WriteFloatFromStringAsync(string nodeId, string value)
        {
            if (Service.IsValidNumber(value))
            {
                return await Service.WriteFloatAsync(nodeId, Convert.ToSingle(value));
            }
            return false;
        }

        /// <summary>
        /// 异步读取16位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>整数值</returns>
        public static async Task<short> ReadInt16Async(string nodeId) => await Service.ReadInt16Async(nodeId);

        /// <summary>
        /// 异步写入16位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> WriteInt16Async(string nodeId, short value) => await Service.WriteInt16Async(nodeId, value);

        /// <summary>
        /// 异步读取32位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>整数值</returns>
        public static async Task<int> ReadInt32Async(string nodeId) => await Service.ReadInt32Async(nodeId);

        /// <summary>
        /// 异步写入32位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> WriteInt32Async(string nodeId, int value) => await Service.WriteInt32Async(nodeId, value);
        #endregion

        #region 异步字符串操作（推荐使用）
        /// <summary>
        /// 异步读取字符串
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>字符串值</returns>
        public static async Task<string> ReadStringAsync(string nodeId) => await Service.ReadStringAsync(nodeId);

        /// <summary>
        /// 异步写入字符串
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> WriteStringAsync(string nodeId, string value) => await Service.WriteStringAsync(nodeId, value);
        #endregion

        #region 高级异步功能
        /// <summary>
        /// 异步批量读取不同类型的数据
        /// </summary>
        /// <param name="nodeTypes">节点和类型的映射</param>
        /// <returns>读取结果</returns>
        public static async Task<Dictionary<string, object>> ReadMixedTypesAsync(Dictionary<string, Type> nodeTypes)
        {
            var asyncServiceType = Service.GetType();
            if (asyncServiceType.Name == "AsyncOpcUaService")
            {
                var readMixedTypesAsyncMethod = asyncServiceType.GetMethod("ReadMixedTypesAsync");
                if (readMixedTypesAsyncMethod != null)
                {
                    var task = (Task<Dictionary<string, object>>)readMixedTypesAsyncMethod.Invoke(Service, new object[] { nodeTypes });
                    return await task;
                }
            }
            
            // 如果不是异步服务，则串行读取
            var results = new Dictionary<string, object>();
            foreach (var nodeType in nodeTypes)
            {
                string nodeId = nodeType.Key;
                Type expectedType = nodeType.Value;

                if (expectedType == typeof(bool))
                {
                    results[nodeId] = await Service.ReadBoolAsync(nodeId);
                }
                else if (expectedType == typeof(float))
                {
                    results[nodeId] = await Service.ReadFloatAsync(nodeId);
                }
                else if (expectedType == typeof(int))
                {
                    results[nodeId] = await Service.ReadInt32Async(nodeId);
                }
                else if (expectedType == typeof(string))
                {
                    results[nodeId] = await Service.ReadStringAsync(nodeId);
                }
            }
            return results;
        }

        /// <summary>
        /// 启动定期异步读取数据（用于数据监控）
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="intervalMs">读取间隔（毫秒）</param>
        /// <param name="onDataReceived">数据接收回调</param>
        /// <param name="cancellationToken">取消令牌</param>
        public static async Task StartPeriodicReadAsync(string nodeId, int intervalMs, Action<object> onDataReceived, CancellationToken cancellationToken = default)
        {
            var asyncServiceType = Service.GetType();
            if (asyncServiceType.Name == "AsyncOpcUaService")
            {
                var startPeriodicReadAsyncMethod = asyncServiceType.GetMethod("StartPeriodicReadAsync");
                if (startPeriodicReadAsyncMethod != null)
                {
                    var task = (Task)startPeriodicReadAsyncMethod.Invoke(Service, new object[] { nodeId, intervalMs, onDataReceived, cancellationToken });
                    await task;
                    return;
                }
            }
            
            // 基本的定期读取实现
            await Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        object value;
                        if (nodeId.Contains("temp") || nodeId.Contains("flow") || nodeId.Contains("SetPoint"))
                        {
                            value = await Service.ReadFloatAsync(nodeId);
                        }
                        else if (nodeId.Contains("Running") || nodeId.Contains("System"))
                        {
                            value = await Service.ReadBoolAsync(nodeId);
                        }
                        else
                        {
                            value = await Service.ReadStringAsync(nodeId);
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

        /// <summary>
        /// 异步数值范围限制写入
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> WriteFloatWithRangeAsync(string nodeId, float value, float min = 0f, float max = 100f)
        {
            float clampedValue = Math.Max(min, Math.Min(max, value));
            return await Service.WriteFloatAsync(nodeId, clampedValue);
        }

        /// <summary>
        /// 异步安全设置操作（读取当前状态，只在关闭时写入新值）
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要设置的值</param>
        /// <param name="onErrorMessage">错误时显示的消息</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> SafeSetOperationAsync(string nodeId, short value, string onErrorMessage = "先关闭才能写入！")
        {
            try
            {
                short currentValue = await Service.ReadInt16Async(nodeId);
                if (currentValue == 0)
                {
                    return await Service.WriteInt16Async(nodeId, value);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(onErrorMessage);
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 同步方法（兼容性保留，不推荐使用）
        /// <summary>
        /// 读取布尔值（不推荐，可能阻塞UI）
        /// </summary>
        public static bool ReadBool(string nodeId) => Service.ReadBool(nodeId);

        /// <summary>
        /// 写入布尔值（不推荐，可能阻塞UI）
        /// </summary>
        public static bool WriteBool(string nodeId, bool value) => Service.WriteBool(nodeId, value);

        /// <summary>
        /// 切换布尔值（不推荐，可能阻塞UI）
        /// </summary>
        public static bool ToggleBool(string nodeId) => Service.ToggleBool(nodeId);

        /// <summary>
        /// 读取浮点数（不推荐，可能阻塞UI）
        /// </summary>
        public static float ReadFloat(string nodeId) => Service.ReadFloat(nodeId);

        /// <summary>
        /// 写入浮点数（不推荐，可能阻塞UI）
        /// </summary>
        public static bool WriteFloat(string nodeId, float value) => Service.WriteFloat(nodeId, value);

        /// <summary>
        /// 从字符串写入浮点数（不推荐，可能阻塞UI）
        /// </summary>
        public static bool WriteFloatFromString(string nodeId, string value)
        {
            if (Service.IsValidNumber(value))
            {
                return Service.WriteFloat(nodeId, Convert.ToSingle(value));
            }
            return false;
        }

        /// <summary>
        /// 读取16位整数（不推荐，可能阻塞UI）
        /// </summary>
        public static short ReadInt16(string nodeId) => Service.ReadInt16(nodeId);

        /// <summary>
        /// 写入16位整数（不推荐，可能阻塞UI）
        /// </summary>
        public static bool WriteInt16(string nodeId, short value) => Service.WriteInt16(nodeId, value);

        /// <summary>
        /// 读取32位整数（不推荐，可能阻塞UI）
        /// </summary>
        public static int ReadInt32(string nodeId) => Service.ReadInt32(nodeId);

        /// <summary>
        /// 写入32位整数（不推荐，可能阻塞UI）
        /// </summary>
        public static bool WriteInt32(string nodeId, int value) => Service.WriteInt32(nodeId, value);

        /// <summary>
        /// 读取字符串（不推荐，可能阻塞UI）
        /// </summary>
        public static string ReadString(string nodeId) => Service.ReadString(nodeId);

        /// <summary>
        /// 写入字符串（不推荐，可能阻塞UI）
        /// </summary>
        public static bool WriteString(string nodeId, string value) => Service.WriteString(nodeId, value);
        #endregion

        #region 实用方法
        /// <summary>
        /// 验证数字字符串
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>是否为有效数字</returns>
        public static bool IsValidNumber(string value) => Service.IsValidNumber(value);

        /// <summary>
        /// 注册连接状态变化事件
        /// </summary>
        /// <param name="handler">事件处理器</param>
        public static void RegisterConnectionStatusChanged(EventHandler<bool> handler)
        {
            Service.ConnectionStatusChanged += handler;
        }

        /// <summary>
        /// 取消注册连接状态变化事件
        /// </summary>
        /// <param name="handler">事件处理器</param>
        public static void UnregisterConnectionStatusChanged(EventHandler<bool> handler)
        {
            Service.ConnectionStatusChanged -= handler;
        }

        /// <summary>
        /// 设置为异步优化服务（推荐）
        /// </summary>
        /// <param name="serverUrl">服务器地址</param>
        public static void UseAsyncService(string serverUrl = null)
        {
            var asyncService = OpcUaServiceFactory.CreateAsyncService(serverUrl);
            OpcUaServiceManager.SetService(asyncService);
        }

        /// <summary>
        /// 设置为模拟服务（测试用）
        /// </summary>
        /// <param name="serverUrl">服务器地址</param>
        public static void UseMockService(string serverUrl = null)
        {
            var mockService = OpcUaServiceFactory.CreateMockService(serverUrl);
            OpcUaServiceManager.SetService(mockService);
        }
        #endregion
    }
}