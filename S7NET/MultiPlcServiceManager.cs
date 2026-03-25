using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace S7NET.Services
{
    /// <summary>
    /// 多PLC通讯服务管理器 - 单例模式
    /// </summary>
    public sealed class MultiPlcServiceManager
    {
        private static readonly Lazy<MultiPlcServiceManager> _instance = new Lazy<MultiPlcServiceManager>(() => new MultiPlcServiceManager());
        private IMultiPlcCommunicationService _communicationService;
        private readonly object _lock = new object();

        public static MultiPlcServiceManager Instance => _instance.Value;

        /// <summary>
        /// 当前多PLC通讯服务
        /// </summary>
        public IMultiPlcCommunicationService CommunicationService
        {
            get
            {
                lock (_lock)
                {
                    return _communicationService;
                }
            }
        }

        /// <summary>
        /// 默认PLC ID
        /// </summary>
        public string DefaultPlcId => _communicationService?.DefaultPlcId;

        /// <summary>
        /// 所有PLC连接状态
        /// </summary>
        public IReadOnlyDictionary<string, PlcConnectionStatus> ConnectionStatuses => _communicationService?.ConnectionStatuses;

        /// <summary>
        /// PLC连接状态变化事件
        /// </summary>
        public event EventHandler<(string PlcId, bool IsConnected)> ConnectionStatusChanged;

        /// <summary>
        /// PLC通讯错误事件
        /// </summary>
        public event EventHandler<(string PlcId, string Error)> CommunicationError;

        private MultiPlcServiceManager()
        {
        }

        /// <summary>
        /// 初始化多PLC通讯服务
        /// </summary>
        /// <param name="config">多PLC配置，如果为空则从配置文件加载</param>
        public void Initialize(MultiPlcConnectionConfig config = null)
        {
            lock (_lock)
            {
                // 如果已有服务，先释放
                if (_communicationService != null)
                {
                    _communicationService.ConnectionStatusChanged -= OnConnectionStatusChanged;
                    _communicationService.CommunicationError -= OnCommunicationError;
                    _communicationService.Dispose();
                }

                // 如果没有提供配置，从配置文件加载
                if (config == null)
                {
                    config = MultiPlcConnectionConfig.LoadFromConfig();
                }

                // 创建新的通讯服务
                _communicationService = new MultiPlcCommunicationService(config);
                _communicationService.ConnectionStatusChanged += OnConnectionStatusChanged;
                _communicationService.CommunicationError += OnCommunicationError;
            }
        }

        #region 连接管理

        /// <summary>
        /// 连接指定PLC
        /// </summary>
        /// <param name="plcId">PLC ID，如果为空则连接默认PLC</param>
        /// <returns></returns>
        public async Task<bool> ConnectPlcAsync(string plcId = null)
        {
            EnsureInitialized();
            return await _communicationService.ConnectPlcAsync(plcId);
        }

        /// <summary>
        /// 断开指定PLC连接
        /// </summary>
        /// <param name="plcId">PLC ID，如果为空则断开默认PLC</param>
        /// <returns></returns>
        public async Task DisconnectPlcAsync(string plcId = null)
        {
            if (_communicationService != null)
            {
                await _communicationService.DisconnectPlcAsync(plcId);
            }
        }

        /// <summary>
        /// 连接所有启用的PLC
        /// </summary>
        /// <returns>连接结果字典</returns>
        public async Task<Dictionary<string, bool>> ConnectAllPlcsAsync()
        {
            EnsureInitialized();
            return await _communicationService.ConnectAllPlcsAsync();
        }

        /// <summary>
        /// 连接所有启用的PLC
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>连接结果字典</returns>
        public async Task<Dictionary<string, bool>> ConnectAllPlcsAsync(CancellationToken cancellationToken)
        {
            EnsureInitialized();
            return await _communicationService.ConnectAllPlcsAsync(cancellationToken);
        }

        /// <summary>
        /// 断开所有PLC连接
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectAllPlcsAsync()
        {
            if (_communicationService != null)
            {
                await _communicationService.DisconnectAllPlcsAsync();
            }
        }

        /// <summary>
        /// 检查PLC是否已连接
        /// </summary>
        /// <param name="plcId">PLC ID，如果为空则检查默认PLC</param>
        /// <returns></returns>
        public bool IsPlcConnected(string plcId = null)
        {
            return _communicationService?.IsPlcConnected(plcId) ?? false;
        }

        /// <summary>
        /// 获取所有PLC的连接状态
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, bool> GetAllConnectionStatuses()
        {
            return _communicationService?.GetAllConnectionStatuses() ?? new Dictionary<string, bool>();
        }

        /// <summary>
        /// 获取所有PLC ID列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllPlcIds()
        {
            return _communicationService?.GetAllPlcIds() ?? new List<string>();
        }

        /// <summary>
        /// 启用或禁用自动重连功能
        /// </summary>
        /// <param name="enabled">是否启用自动重连</param>
        public void SetAutoReconnect(bool enabled)
        {
            if (_communicationService != null)
            {
                // 通过底层服务设置自动重连
                // 这里需要在IMultiPlcCommunicationService中添加相应方法
            }
        }

        #endregion

        #region 数据读取

        /// <summary>
        /// 读取单个数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="address">地址</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns></returns>
        public async Task<T> ReadAsync<T>(string address, string plcId = null)
        {
            EnsureInitialized();
            return await _communicationService.ReadAsync<T>(address, plcId);
        }

        /// <summary>
        /// 读取多个数据（同一PLC）
        /// </summary>
        /// <param name="addresses">地址列表</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>地址和值的字典</returns>
        public async Task<Dictionary<string, object>> ReadMultipleAsync(string[] addresses, string plcId = null)
        {
            EnsureInitialized();
            return await _communicationService.ReadMultipleAsync(addresses, plcId);
        }

        /// <summary>
        /// 读取多个数据（跨多个PLC）
        /// </summary>
        /// <param name="addressPlcPairs">地址和PLC ID的配对列表</param>
        /// <returns>地址和值的字典</returns>
        public async Task<Dictionary<string, object>> ReadMultiplePlcsAsync(Dictionary<string, string> addressPlcPairs)
        {
            EnsureInitialized();
            return await _communicationService.ReadMultiplePlcsAsync(addressPlcPairs);
        }

        /// <summary>
        /// 读取DB块数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dbNumber">DB块号</param>
        /// <param name="startByte">起始字节</param>
        /// <param name="count">数量</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns></returns>
        public async Task<T[]> ReadDBAsync<T>(int dbNumber, int startByte, int count = 1, string plcId = null)
        {
            EnsureInitialized();
            return await _communicationService.ReadDBAsync<T>(dbNumber, startByte, count, plcId);
        }

        /// <summary>
        /// 读取位数据
        /// </summary>
        /// <param name="address">地址 (如: M0.0, I0.1, Q0.2)</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns></returns>
        public async Task<bool> ReadBitAsync(string address, string plcId = null)
        {
            EnsureInitialized();
            return await _communicationService.ReadBitAsync(address, plcId);
        }

        /// <summary>
        /// 读取Q点（输出点）bool量
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="length">读取长度（位数）</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>bool数组</returns>
        public async Task<bool[]> ReadQBitsAsync(int startByte, int startBit, int length, string plcId = null)
        {
            EnsureInitialized();
            return await _communicationService.ReadQBitsAsync(startByte, startBit, length, plcId);
        }

        /// <summary>
        /// 读取I点（输入点）bool量
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="length">读取长度（位数）</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>bool数组</returns>
        public async Task<bool[]> ReadIBitsAsync(int startByte, int startBit, int length, string plcId = null)
        {
            EnsureInitialized();
            return await _communicationService.ReadIBitsAsync(startByte, startBit, length, plcId);
        }

        /// <summary>
        /// 读取M区不同类型数据
        /// </summary>
        /// <typeparam name="T">数据类型（bool, byte, short, int, float, double等）</typeparam>
        /// <param name="startByte">起始字节</param>
        /// <param name="count">读取数量</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>指定类型的数据数组</returns>
        public async Task<T[]> ReadMAsync<T>(int startByte, int count = 1, string plcId = null)
        {
            EnsureInitialized();
            return await _communicationService.ReadMAsync<T>(startByte, count, plcId);
        }

        /// <summary>
        /// 读取M区位数据
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="length">读取长度（位数）</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>bool数组</returns>
        public async Task<bool[]> ReadMBitsAsync(int startByte, int startBit, int length, string plcId = null)
        {
            EnsureInitialized();
            return await _communicationService.ReadMBitsAsync(startByte, startBit, length, plcId);
        }

        /// <summary>
        /// 写入Q点（输出点）bool量
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="values">bool值数组</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>写入是否成功</returns>
        public async Task<bool> WriteQBitsAsync(int startByte, int startBit, bool[] values, string plcId = null)
        {
            EnsureInitialized();
            return await _communicationService.WriteQBitsAsync(startByte, startBit, values, plcId);
        }

        /// <summary>
        /// 写入单个Q点bool值
        /// </summary>
        /// <param name="byteAddress">字节地址</param>
        /// <param name="bitAddress">位地址</param>
        /// <param name="value">bool值</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>写入是否成功</returns>
        public async Task<bool> WriteQBitAsync(int byteAddress, int bitAddress, bool value, string plcId = null)
        {
            EnsureInitialized();
            return await _communicationService.WriteQBitAsync(byteAddress, bitAddress, value, plcId);
        }

        /// <summary>
        /// 写入M区不同类型数据
        /// </summary>
        /// <typeparam name="T">数据类型（byte, short, int, float, double等）</typeparam>
        /// <param name="startByte">起始字节</param>
        /// <param name="values">数据值数组</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>写入是否成功</returns>
        public async Task<bool> WriteMAsync<T>(int startByte, T[] values, string plcId = null)
        {
            EnsureInitialized();
            return await _communicationService.WriteMAsync(startByte, values, plcId);
        }

        /// <summary>
        /// 写入单个M区数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="startByte">起始字节</param>
        /// <param name="value">数据值</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>写入是否成功</returns>
        public async Task<bool> WriteMSingleAsync<T>(int startByte, T value, string plcId = null)
        {
            EnsureInitialized();
            return await _communicationService.WriteMSingleAsync(startByte, value, plcId);
        }

        /// <summary>
        /// 写入M区位数据
        /// </summary>
        /// <param name="startByte">起始字节</param>
        /// <param name="startBit">起始位</param>
        /// <param name="values">bool值数组</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns>写入是否成功</returns>
        public async Task<bool> WriteMBitsAsync(int startByte, int startBit, bool[] values, string plcId = null)
        {
            EnsureInitialized();
            return await _communicationService.WriteMBitsAsync(startByte, startBit, values, plcId);
        }

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
        public async Task<bool> WriteAsync<T>(string address, T value, string plcId = null)
        {
            EnsureInitialized();
            return await _communicationService.WriteAsync(address, value, plcId);
        }

        /// <summary>
        /// 写入多个数据（同一PLC）
        /// </summary>
        /// <param name="data">地址和值的字典</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns></returns>
        public async Task<bool> WriteMultipleAsync(Dictionary<string, object> data, string plcId = null)
        {
            EnsureInitialized();
            return await _communicationService.WriteMultipleAsync(data, plcId);
        }

        /// <summary>
        /// 写入多个数据（跨多个PLC）
        /// </summary>
        /// <param name="dataPlcPairs">地址、值和PLC ID的配对列表</param>
        /// <returns>写入结果字典</returns>
        public async Task<Dictionary<string, bool>> WriteMultiplePlcsAsync(Dictionary<string, (object Value, string PlcId)> dataPlcPairs)
        {
            EnsureInitialized();
            return await _communicationService.WriteMultiplePlcsAsync(dataPlcPairs);
        }

        /// <summary>
        /// 写入DB块数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dbNumber">DB块号</param>
        /// <param name="startByte">起始字节</param>
        /// <param name="value">值</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns></returns>
        public async Task<bool> WriteDBAsync<T>(int dbNumber, int startByte, T value, string plcId = null)
        {
            EnsureInitialized();
            return await _communicationService.WriteDBAsync(dbNumber, startByte, value, plcId);
        }

        /// <summary>
        /// 写入位数据
        /// </summary>
        /// <param name="address">地址 (如: M0.0, I0.1, Q0.2)</param>
        /// <param name="value">值</param>
        /// <param name="plcId">PLC ID，如果为空则使用默认PLC</param>
        /// <returns></returns>
        public async Task<bool> WriteBitAsync(string address, bool value, string plcId = null)
        {
            EnsureInitialized();
            return await _communicationService.WriteBitAsync(address, value, plcId);
        }

        #endregion

        #region PLC管理

        /// <summary>
        /// 添加新的PLC连接
        /// </summary>
        /// <param name="plcInfo">PLC连接信息</param>
        /// <returns></returns>
        public bool AddPlcConnection(PlcConnectionInfo plcInfo)
        {
            EnsureInitialized();
            return _communicationService.AddPlcConnection(plcInfo);
        }

        /// <summary>
        /// 移除PLC连接
        /// </summary>
        /// <param name="plcId">PLC ID</param>
        /// <returns></returns>
        public async Task<bool> RemovePlcConnectionAsync(string plcId)
        {
            if (_communicationService != null)
            {
                return await _communicationService.RemovePlcConnectionAsync(plcId);
            }
            return false;
        }

        /// <summary>
        /// 获取PLC配置信息
        /// </summary>
        /// <param name="plcId">PLC ID</param>
        /// <returns></returns>
        public PlcConnectionInfo GetPlcConfig(string plcId)
        {
            return _communicationService?.GetPlcConfig(plcId);
        }

        #endregion

        private void EnsureInitialized()
        {
            if (_communicationService == null)
                throw new InvalidOperationException("请先调用Initialize方法初始化多PLC通讯服务");
        }

        private void OnConnectionStatusChanged(object sender, (string PlcId, bool IsConnected) args)
        {
            ConnectionStatusChanged?.Invoke(this, args);
        }

        private void OnCommunicationError(object sender, (string PlcId, string Error) args)
        {
            CommunicationError?.Invoke(this, args);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            lock (_lock)
            {
                if (_communicationService != null)
                {
                    _communicationService.ConnectionStatusChanged -= OnConnectionStatusChanged;
                    _communicationService.CommunicationError -= OnCommunicationError;
                    _communicationService.Dispose();
                    _communicationService = null;
                }
            }
        }
    }
}
