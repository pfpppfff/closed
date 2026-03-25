using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace S7NET.Services
{
    /// <summary>
    /// 多PLC通讯服务接口
    /// </summary>
    public interface IMultiPlcCommunicationService : IDisposable
    {
        /// <summary>
        /// 默认PLC ID
        /// </summary>
        string DefaultPlcId { get; }

        /// <summary>
        /// 所有PLC连接状态
        /// </summary>
        IReadOnlyDictionary<string, PlcConnectionStatus> ConnectionStatuses { get; }

        /// <summary>
        /// PLC连接状态变化事件
        /// </summary>
        event EventHandler<(string PlcId, bool IsConnected)> ConnectionStatusChanged;

        /// <summary>
        /// PLC通讯错误事件
        /// </summary>
        event EventHandler<(string PlcId, string Error)> CommunicationError;

        #region 连接管理

        /// <summary>
        /// 连接指定PLC
        /// </summary>
        /// <param name="plcId">PLC ID，如果为空则连接默认PLC</param>
        /// <returns></returns>
        Task<bool> ConnectPlcAsync(string plcId = null);

        /// <summary>
        /// 断开指定PLC连接
        /// </summary>
        /// <param name="plcId">PLC ID，如果为空则断开默认PLC</param>
        /// <returns></returns>
        Task DisconnectPlcAsync(string plcId = null);

        /// <summary>
        /// 连接所有启用的PLC
        /// </summary>
        /// <returns>连接结果字典</returns>
        Task<Dictionary<string, bool>> ConnectAllPlcsAsync();

        /// <summary>
        /// 连接所有启用的PLC
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>连接结果字典</returns>
        Task<Dictionary<string, bool>> ConnectAllPlcsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 断开所有PLC连接
        /// </summary>
        /// <returns></returns>
        Task DisconnectAllPlcsAsync();

        /// <summary>
        /// 检查PLC是否已连接
        /// </summary>
        /// <param name="plcId">PLC ID，如果为空则检查默认PLC</param>
        /// <returns></returns>
        bool IsPlcConnected(string plcId = null);

        /// <summary>
        /// 获取所有PLC的连接状态
        /// </summary>
        /// <returns></returns>
        Dictionary<string, bool> GetAllConnectionStatuses();

        /// <summary>
        /// 获取所有PLC ID列表
        /// </summary>
        /// <returns></returns>
        List<string> GetAllPlcIds();

        #endregion

        #region 数据读取

        /// <summary>
        /// 读取单个数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="address">地址</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns></returns>
        Task<T> ReadAsync<T>(string address, string plcId = null);

        /// <summary>
        /// 读取多个数据（同一PLC）
        /// </summary>
        /// <param name="addresses">地址列表</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>地址和值的字典</returns>
        Task<Dictionary<string, object>> ReadMultipleAsync(string[] addresses, string plcId = null);

        /// <summary>
        /// 读取多个数据（跨多个PLC）
        /// </summary>
        /// <param name="addressPlcPairs">地址和PLC ID的配对列表</param>
        /// <returns>地址和值的字典</returns>
        Task<Dictionary<string, object>> ReadMultiplePlcsAsync(Dictionary<string, string> addressPlcPairs);

        /// <summary>
        /// 读取DB块数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dbNumber">DB块号</param>
        /// <param name="startByte">起始字节</param>
        /// <param name="count">数量</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns></returns>
        Task<T[]> ReadDBAsync<T>(int dbNumber, int startByte, int count = 1, string plcId = null);

        /// <summary>
        /// 读取位数据
        /// </summary>
        /// <param name="address">地址 (如: M0.0, I0.1, Q0.2)</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns></returns>
        Task<bool> ReadBitAsync(string address, string plcId = null);

        /// <summary>
        /// 读取Q点（输出点）bool量
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="length">读取长度（位数）</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>bool数组</returns>
        Task<bool[]> ReadQBitsAsync(int startByte, int startBit, int length, string plcId = null);

        /// <summary>
        /// 读取I点（输入点）bool量
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="length">读取长度（位数）</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>bool数组</returns>
        Task<bool[]> ReadIBitsAsync(int startByte, int startBit, int length, string plcId = null);

        /// <summary>
        /// 读取M区不同类型数据
        /// </summary>
        /// <typeparam name="T">数据类型（bool, byte, short, int, float, double等）</typeparam>
        /// <param name="startByte">起始字节</param>
        /// <param name="count">读取数量</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>指定类型的数据数组</returns>
        Task<T[]> ReadMAsync<T>(int startByte, int count = 1, string plcId = null);

        /// <summary>
        /// 读取M区位数据
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="length">读取长度（位数）</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>bool数组</returns>
        Task<bool[]> ReadMBitsAsync(int startByte, int startBit, int length, string plcId = null);

        /// <summary>
        /// 写入Q点（输出点）bool量
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="values">bool值数组</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>写入是否成功</returns>
        Task<bool> WriteQBitsAsync(int startByte, int startBit, bool[] values, string plcId = null);

        /// <summary>
        /// 写入单个Q点bool值
        /// </summary>
        /// <param name="byteAddress">字节地址</param>
        /// <param name="bitAddress">位地址</param>
        /// <param name="value">bool值</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>写入是否成功</returns>
        Task<bool> WriteQBitAsync(int byteAddress, int bitAddress, bool value, string plcId = null);

        /// <summary>
        /// 写入M区不同类型数据
        /// </summary>
        /// <typeparam name="T">数据类型（byte, short, int, float, double等）</typeparam>
        /// <param name="startByte">起始字节</param>
        /// <param name="values">数据值数组</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>写入是否成功</returns>
        Task<bool> WriteMAsync<T>(int startByte, T[] values, string plcId = null);

        /// <summary>
        /// 写入单个M区数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="startByte">起始字节</param>
        /// <param name="value">数据值</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>写入是否成功</returns>
        Task<bool> WriteMSingleAsync<T>(int startByte, T value, string plcId = null);

        /// <summary>
        /// 写入M区位数据
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="values">bool值数组</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>写入是否成功</returns>
        Task<bool> WriteMBitsAsync(int startByte, int startBit, bool[] values, string plcId = null);

        #endregion

        #region 数据写入

        /// <summary>
        /// 写入单个数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns></returns>
        Task<bool> WriteAsync<T>(string address, T value, string plcId = null);

        /// <summary>
        /// 写入多个数据（同一PLC）
        /// </summary>
        /// <param name="data">地址和值的字典</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns></returns>
        Task<bool> WriteMultipleAsync(Dictionary<string, object> data, string plcId = null);

        /// <summary>
        /// 写入多个数据（跨多个PLC）
        /// </summary>
        /// <param name="dataPlcPairs">地址、值和PLC ID的配对列表</param>
        /// <returns>写入结果字典</returns>
        Task<Dictionary<string, bool>> WriteMultiplePlcsAsync(Dictionary<string, (object Value, string PlcId)> dataPlcPairs);

        /// <summary>
        /// 写入DB块数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dbNumber">DB块号</param>
        /// <param name="startByte">起始字节</param>
        /// <param name="value">值</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns></returns>
        Task<bool> WriteDBAsync<T>(int dbNumber, int startByte, T value, string plcId = null);

        /// <summary>
        /// 写入位数据
        /// </summary>
        /// <param name="address">地址 (如: M0.0, I0.1, Q0.2)</param>
        /// <param name="value">值</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns></returns>
        Task<bool> WriteBitAsync(string address, bool value, string plcId = null);



        #endregion

        #region PLC管理

        /// <summary>
        /// 添加新的PLC连接
        /// </summary>
        /// <param name="plcInfo">PLC连接信息</param>
        /// <returns></returns>
        bool AddPlcConnection(PlcConnectionInfo plcInfo);

        /// <summary>
        /// 移除PLC连接
        /// </summary>
        /// <param name="plcId">PLC ID</param>
        /// <returns></returns>
        Task<bool> RemovePlcConnectionAsync(string plcId);

        /// <summary>
        /// 获取PLC配置信息
        /// </summary>
        /// <param name="plcId">PLC ID</param>
        /// <returns></returns>
        PlcConnectionInfo GetPlcConfig(string plcId);

        #endregion
    }
}
