using S7.Net;
using System;
using System.Threading.Tasks;

namespace S7NET.Services
{
    /// <summary>
    /// S7通讯服务管理器 - 单例模式
    /// </summary>
    public sealed class S7ServiceManager
    {
        private static readonly Lazy<S7ServiceManager> _instance = new Lazy<S7ServiceManager>(() => new S7ServiceManager());
        private IS7CommunicationService _communicationService;
        private readonly object _lock = new object();

        public static S7ServiceManager Instance => _instance.Value;

        /// <summary>
        /// 当前通讯服务
        /// </summary>
        public IS7CommunicationService CommunicationService
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
        /// 连接状态
        /// </summary>
        public bool IsConnected => _communicationService?.IsConnected ?? false;

        /// <summary>
        /// 连接状态变化事件
        /// </summary>
        public event EventHandler<bool> ConnectionStatusChanged;

        /// <summary>
        /// 通讯错误事件
        /// </summary>
        public event EventHandler<string> CommunicationError;

        private S7ServiceManager()
        {
        }

        /// <summary>
        /// 初始化通讯服务
        /// </summary>
        /// <param name="cpuType">CPU类型</param>
        /// <param name="ip">IP地址</param>
        /// <param name="rack">机架号</param>
        /// <param name="slot">插槽号</param>
        public void Initialize(CpuType cpuType, string ip, short rack = 0, short slot = 1)
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

                // 创建新的通讯服务
                _communicationService = new S7CommunicationService(cpuType, ip, rack, slot);
                _communicationService.ConnectionStatusChanged += OnConnectionStatusChanged;
                _communicationService.CommunicationError += OnCommunicationError;
            }
        }

        /// <summary>
        /// 连接到PLC
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ConnectAsync()
        {
            if (_communicationService == null)
                throw new InvalidOperationException("请先调用Initialize方法初始化通讯服务");

            return await _communicationService.ConnectAsync();
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectAsync()
        {
            if (_communicationService != null)
            {
                await _communicationService.DisconnectAsync();
            }
        }

        /// <summary>
        /// 读取单个数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public async Task<T> ReadAsync<T>(string address)
        {
            if (_communicationService == null)
                throw new InvalidOperationException("请先调用Initialize方法初始化通讯服务");

            return await _communicationService.ReadAsync<T>(address);
        }

        /// <summary>
        /// 写入单个数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="address">地址</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<bool> WriteAsync<T>(string address, T value)
        {
            if (_communicationService == null)
                throw new InvalidOperationException("请先调用Initialize方法初始化通讯服务");

            return await _communicationService.WriteAsync(address, value);
        }

        /// <summary>
        /// 读取DB块数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dbNumber">DB块号</param>
        /// <param name="startByte">起始字节</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public async Task<T[]> ReadDBAsync<T>(int dbNumber, int startByte, int count = 1)
        {
            if (_communicationService == null)
                throw new InvalidOperationException("请先调用Initialize方法初始化通讯服务");

            return await _communicationService.ReadDBAsync<T>(dbNumber, startByte, count);
        }

        /// <summary>
        /// 写入DB块数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dbNumber">DB块号</param>
        /// <param name="startByte">起始字节</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<bool> WriteDBAsync<T>(int dbNumber, int startByte, T value)
        {
            if (_communicationService == null)
                throw new InvalidOperationException("请先调用Initialize方法初始化通讯服务");

            return await _communicationService.WriteDBAsync(dbNumber, startByte, value);
        }

        /// <summary>
        /// 读取位数据
        /// </summary>
        /// <param name="address">地址 (如: M0.0, I0.1, Q0.2)</param>
        /// <returns></returns>
        public async Task<bool> ReadBitAsync(string address)
        {
            if (_communicationService == null)
                throw new InvalidOperationException("请先调用Initialize方法初始化通讯服务");

            return await _communicationService.ReadBitAsync(address);
        }

        /// <summary>
        /// 写入位数据
        /// </summary>
        /// <param name="address">地址 (如: M0.0, I0.1, Q0.2)</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<bool> WriteBitAsync(string address, bool value)
        {
            if (_communicationService == null)
                throw new InvalidOperationException("请先调用Initialize方法初始化通讯服务");

            return await _communicationService.WriteBitAsync(address, value);
        }

        private void OnConnectionStatusChanged(object sender, bool isConnected)
        {
            ConnectionStatusChanged?.Invoke(this, isConnected);
        }

        private void OnCommunicationError(object sender, string error)
        {
            CommunicationError?.Invoke(this, error);
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
