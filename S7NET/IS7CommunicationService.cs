using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace S7NET.Services
{
    /// <summary>
    /// S7通讯服务接口
    /// </summary>
    public interface IS7CommunicationService : IDisposable
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 连接状态变化事件
        /// </summary>
        event EventHandler<bool> ConnectionStatusChanged;

        /// <summary>
        /// 通讯错误事件
        /// </summary>
        event EventHandler<string> CommunicationError;

        /// <summary>
        /// 连接到PLC
        /// </summary>
        /// <returns></returns>
        Task<bool> ConnectAsync();

        /// <summary>
        /// 连接到PLC
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<bool> ConnectAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        Task DisconnectAsync();

        /// <summary>
        /// 读取单个数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="address">地址</param>
        /// <returns></returns>
        Task<T> ReadAsync<T>(string address);

        /// <summary>
        /// 读取多个数据
        /// </summary>
        /// <param name="addresses">地址列表</param>
        /// <returns>地址和值的字典</returns>
        Task<Dictionary<string, object>> ReadMultipleAsync(params string[] addresses);

        /// <summary>
        /// 写入单个数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Task<bool> WriteAsync<T>(string address, T value);

        /// <summary>
        /// 写入多个数据
        /// </summary>
        /// <param name="data">地址和值的字典</param>
        /// <returns></returns>
        Task<bool> WriteMultipleAsync(Dictionary<string, object> data);

        /// <summary>
        /// 读取DB块数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dbNumber">DB块号</param>
        /// <param name="startByte">起始字节</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        Task<T[]> ReadDBAsync<T>(int dbNumber, int startByte, int count = 1);

        /// <summary>
        /// 写入DB块数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dbNumber">DB块号</param>
        /// <param name="startByte">起始字节</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Task<bool> WriteDBAsync<T>(int dbNumber, int startByte, T value);

        /// <summary>
        /// 读取位数据
        /// </summary>
        /// <param name="address">地址 (如: M0.0, I0.1, Q0.2)</param>
        /// <returns></returns>
        Task<bool> ReadBitAsync(string address);

        /// <summary>
        /// 写入位数据
        /// </summary>
        /// <param name="address">地址 (如: M0.0, I0.1, Q0.2)</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Task<bool> WriteBitAsync(string address, bool value);

        /// <summary>
        /// 读取Q点（输出点）bool量
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="length">读取长度（位数）</param>
        /// <returns>bool数组</returns>
        Task<bool[]> ReadQBitsAsync(int startByte, int startBit, int length);

        /// <summary>
        /// 读取I点（输入点）bool量
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="length">读取长度（位数）</param>
        /// <returns>bool数组</returns>
        Task<bool[]> ReadIBitsAsync(int startByte, int startBit, int length);

        /// <summary>
        /// 读取M区不同类型数据
        /// </summary>
        /// <typeparam name="T">数据类型（bool, byte, short, int, float, double等）</typeparam>
        /// <param name="startByte">起始字节</param>
        /// <param name="count">读取数量</param>
        /// <returns>指定类型的数据数组</returns>
        Task<T[]> ReadMAsync<T>(int startByte, int count = 1);

        /// <summary>
        /// 读取M区位数据
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="length">读取长度（位数）</param>
        /// <returns>bool数组</returns>
        Task<bool[]> ReadMBitsAsync(int startByte, int startBit, int length);

        /// <summary>
        /// 写入Q点（输出点）bool量
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="values">bool值数组</param>
        /// <returns>写入是否成功</returns>
        Task<bool> WriteQBitsAsync(int startByte, int startBit, bool[] values);

        /// <summary>
        /// 写入单个Q点bool值
        /// </summary>
        /// <param name="byteAddress">字节地址</param>
        /// <param name="bitAddress">位地址</param>
        /// <param name="value">bool值</param>
        /// <returns>写入是否成功</returns>
        Task<bool> WriteQBitAsync(int byteAddress, int bitAddress, bool value);

        /// <summary>
        /// 写入M区不同类型数据
        /// </summary>
        /// <typeparam name="T">数据类型（byte, short, int, float, double等）</typeparam>
        /// <param name="startByte">起始字节</param>
        /// <param name="values">数据值数组</param>
        /// <returns>写入是否成功</returns>
        Task<bool> WriteMAsync<T>(int startByte, T[] values);

        /// <summary>
        /// 写入单个M区数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="startByte">起始字节</param>
        /// <param name="value">数据值</param>
        /// <returns>写入是否成功</returns>
        Task<bool> WriteMSingleAsync<T>(int startByte, T value);

        /// <summary>
        /// 写入M区位数据
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="values">bool值数组</param>
        /// <returns>写入是否成功</returns>
        Task<bool> WriteMBitsAsync(int startByte, int startBit, bool[] values);
    }
}
