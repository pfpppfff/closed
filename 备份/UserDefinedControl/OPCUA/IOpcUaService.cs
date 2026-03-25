using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UcAsp.Opc;

namespace UserDefinedControl.OPCUA
{
    /// <summary>
    /// OPC UA 通讯服务接口
    /// 提供标准化的OPC UA操作接口，支持依赖注入和单元测试
    /// </summary>
    public interface IOpcUaService
    {
        #region 连接管理
        /// <summary>
        /// 连接状态
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 默认服务器地址
        /// </summary>
        string DefaultServerUrl { get; set; }

        /// <summary>
        /// 连接状态变化事件
        /// </summary>
        event EventHandler<bool> ConnectionStatusChanged;

        /// <summary>
        /// 连接到指定服务器
        /// </summary>
        /// <param name="serverUrl">服务器地址，为空则使用默认地址</param>
        void Connect(string serverUrl = null);

        /// <summary>
        /// 重新连接
        /// </summary>
        /// <param name="serverUrl">服务器地址，为空则使用默认地址</param>
        void Reconnect(string serverUrl = null);

        /// <summary>
        /// 断开连接
        /// </summary>
        void Disconnect();
        #endregion

        #region 同步读写方法
        /// <summary>
        /// 读取布尔值
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>布尔值，读取失败返回false</returns>
        bool ReadBool(string nodeId);

        /// <summary>
        /// 写入布尔值
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        bool WriteBool(string nodeId, bool value);

        /// <summary>
        /// 读取浮点数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>浮点数值，读取失败返回0.0f</returns>
        float ReadFloat(string nodeId);

        /// <summary>
        /// 写入浮点数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        bool WriteFloat(string nodeId, float value);

        /// <summary>
        /// 读取16位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>整数值，读取失败返回0</returns>
        short ReadInt16(string nodeId);

        /// <summary>
        /// 写入16位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        bool WriteInt16(string nodeId, short value);

        /// <summary>
        /// 读取32位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>整数值，读取失败返回0</returns>
        int ReadInt32(string nodeId);

        /// <summary>
        /// 写入32位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        bool WriteInt32(string nodeId, int value);

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>字符串值，读取失败返回空字符串</returns>
        string ReadString(string nodeId);

        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        bool WriteString(string nodeId, string value);
        #endregion

        #region 异步读写方法
        /// <summary>
        /// 异步读取布尔值
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>布尔值，读取失败返回false</returns>
        Task<bool> ReadBoolAsync(string nodeId);

        /// <summary>
        /// 异步写入布尔值
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        Task<bool> WriteBoolAsync(string nodeId, bool value);

        /// <summary>
        /// 异步读取浮点数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>浮点数值，读取失败返回0.0f</returns>
        Task<float> ReadFloatAsync(string nodeId);

        /// <summary>
        /// 异步写入浮点数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        Task<bool> WriteFloatAsync(string nodeId, float value);

        /// <summary>
        /// 异步读取16位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>整数值，读取失败返回0</returns>
        Task<short> ReadInt16Async(string nodeId);

        /// <summary>
        /// 异步写入16位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        Task<bool> WriteInt16Async(string nodeId, short value);

        /// <summary>
        /// 异步读取32位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>整数值，读取失败返回0</returns>
        Task<int> ReadInt32Async(string nodeId);

        /// <summary>
        /// 异步写入32位整数
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        Task<bool> WriteInt32Async(string nodeId, int value);

        /// <summary>
        /// 异步读取字符串
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>字符串值，读取失败返回空字符串</returns>
        Task<string> ReadStringAsync(string nodeId);

        /// <summary>
        /// 异步写入字符串
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        Task<bool> WriteStringAsync(string nodeId, string value);
        #endregion

        #region 批量操作
        /// <summary>
        /// 批量读取多个节点
        /// </summary>
        /// <param name="nodeIds">节点地址数组</param>
        /// <returns>读取结果列表</returns>
        List<OpcItemValue> ReadMultiple(params string[] nodeIds);

        /// <summary>
        /// 批量写入多个节点
        /// </summary>
        /// <param name="writeOperations">写入操作字典(节点地址->值)</param>
        /// <returns>是否全部写入成功</returns>
        bool WriteMultiple(Dictionary<string, object> writeOperations);
        #endregion

        #region 多PLC状态管理
        /// <summary>
        /// 获取所有PLC状态信息
        /// </summary>
        /// <returns>PLC状态列表</returns>
        List<PlcStatusModel> GetAllPlcStatus();

        /// <summary>
        /// 获取指定PLC状态
        /// </summary>
        /// <param name="plcId">PLC标识符</param>
        /// <returns>PLC状态信息，不存在返回null</returns>
        PlcStatusModel GetPlcStatus(string plcId);

        /// <summary>
        /// 添加或更新PLC配置
        /// </summary>
        /// <param name="plcModel">PLC状态模型</param>
        /// <returns>是否操作成功</returns>
        bool AddOrUpdatePlc(PlcStatusModel plcModel);

        /// <summary>
        /// 移除PLC配置
        /// </summary>
        /// <param name="plcId">PLC标识符</param>
        /// <returns>是否移除成功</returns>
        bool RemovePlc(string plcId);

        /// <summary>
        /// 更新指定PLC的系统状态
        /// </summary>
        /// <param name="plcId">PLC标识符</param>
        /// <returns>系统状态更新结果</returns>
        Task<bool> UpdatePlcSystemStatusAsync(string plcId);

        /// <summary>
        /// 更新所有PLC的系统状态
        /// </summary>
        /// <returns>更新成功的PLC数量</returns>
        Task<int> UpdateAllPlcSystemStatusAsync();

        /// <summary>
        /// 检查指定PLC是否可以安全读取数据
        /// </summary>
        /// <param name="plcId">PLC标识符</param>
        /// <returns>是否可以安全读取</returns>
        bool CanSafelyReadPlcData(string plcId);

        /// <summary>
        /// 获取正常状态的PLC数量
        /// </summary>
        /// <returns>正常状态的PLC数量</returns>
        int GetNormalPlcCount();

        /// <summary>
        /// 获取异常状态的PLC数量
        /// </summary>
        /// <returns>异常状态的PLC数量</returns>
        int GetAbnormalPlcCount();

        /// <summary>
        /// PLC状态变化事件
        /// </summary>
        event EventHandler<PlcStatusChangedEventArgs> PlcStatusChanged;
        #endregion

        #region 实用方法
        /// <summary>
        /// 切换布尔值（读取当前值并写入相反值）
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>是否操作成功</returns>
        bool ToggleBool(string nodeId);

        /// <summary>
        /// 异步切换布尔值
        /// </summary>
        /// <param name="nodeId">节点地址</param>
        /// <returns>是否操作成功</returns>
        Task<bool> ToggleBoolAsync(string nodeId);

        /// <summary>
        /// 验证数字字符串
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <returns>是否为有效数字</returns>
        bool IsValidNumber(string value);
        #endregion
    }
}