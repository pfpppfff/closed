using System;
using System.Threading.Tasks;

namespace UserDefinedControl.OPCUA
{
    /// <summary>
    /// OPC UA 快捷访问助手类
    /// 提供简化的访问接口，方便在各个控件中使用
    /// 支持服务接口和依赖注入
    /// </summary>
    public static class OpcUaHelper
    {
        /// <summary>
        /// 获取当前服务实例
        /// </summary>
        private static IOpcUaService Service => OpcUaServiceManager.Current;

        #region 连接管理
        /// <summary>
        /// 检查连接状态
        /// </summary>
        public static bool IsConnected => Service.IsConnected;

        /// <summary>
        /// 连接到指定服务器
        /// </summary>
        /// <param name="serverUrl">服务器地址，为空则使用默认地址</param>
        public static void Connect(string serverUrl = null)
        {
            Service.Connect(serverUrl);
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        /// <param name="serverUrl">服务器地址，为空则使用默认地址</param>
        public static void Reconnect(string serverUrl = null)
        {
            Service.Reconnect(serverUrl);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public static void Disconnect()
        {
            Service.Disconnect();
        }
        #endregion

        #region 布尔值操作
        /// <summary>
        /// 读取布尔值
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>布尔值</returns>
        public static bool ReadBool(string nodeId) => Service.ReadBool(nodeId);

        /// <summary>
        /// 写入布尔值
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        public static bool WriteBool(string nodeId, bool value) => Service.WriteBool(nodeId, value);

        /// <summary>
        /// 切换布尔值
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>是否成功</returns>
        public static bool ToggleBool(string nodeId) => Service.ToggleBool(nodeId);

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

        #region 数值操作
        /// <summary>
        /// 读取浮点数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>浮点数值</returns>
        public static float ReadFloat(string nodeId) => Service.ReadFloat(nodeId);

        /// <summary>
        /// 写入浮点数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        public static bool WriteFloat(string nodeId, float value) => Service.WriteFloat(nodeId, value);

        /// <summary>
        /// 从字符串写入浮点数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">字符串值</param>
        /// <returns>是否成功</returns>
        public static bool WriteFloatFromString(string nodeId, string value)
        {
            if (Service.IsValidNumber(value))
            {
                return Service.WriteFloat(nodeId, Convert.ToSingle(value));
            }
            return false;
        }

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
        /// 读取16位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>整数值</returns>
        public static short ReadInt16(string nodeId) => Service.ReadInt16(nodeId);

        /// <summary>
        /// 写入16位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        public static bool WriteInt16(string nodeId, short value) => Service.WriteInt16(nodeId, value);

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
        /// 读取32位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>整数值</returns>
        public static int ReadInt32(string nodeId) => Service.ReadInt32(nodeId);

        /// <summary>
        /// 写入32位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        public static bool WriteInt32(string nodeId, int value) => Service.WriteInt32(nodeId, value);

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

        #region 字符串操作
        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>字符串值</returns>
        public static string ReadString(string nodeId) => Service.ReadString(nodeId);

        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        public static bool WriteString(string nodeId, string value) => Service.WriteString(nodeId, value);

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
        #endregion

        #region 常用操作封装
        /// <summary>
        /// 安全设置操作（读取当前状态，只在关闭时写入新值）
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要设置的值</param>
        /// <param name="onErrorMessage">错误时显示的消息</param>
        /// <returns>是否成功</returns>
        public static bool SafeSetOperation(string nodeId, short value, string onErrorMessage = "先关闭才能写入！")
        {
            try
            {
                short currentValue = ReadInt16(nodeId);
                if (currentValue == 0)
                {
                    return WriteInt16(nodeId, value);
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

        /// <summary>
        /// 数值范围限制写入
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>是否成功</returns>
        public static bool WriteFloatWithRange(string nodeId, float value, float min = 0f, float max = 100f)
        {
            float clampedValue = Math.Max(min, Math.Min(max, value));
            return WriteFloat(nodeId, clampedValue);
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
            return await WriteFloatAsync(nodeId, clampedValue);
        }
        #endregion
    }
}