using System;

namespace UserDefinedControl.OPCUA
{
    /// <summary>
    /// OPC UA 服务管理器
    /// 提供依赖注入和服务实例管理功能
    /// </summary>
    public static class OpcUaServiceManager
    {
        #region 私有字段
        private static IOpcUaService _currentService;
        private static readonly object _lockObject = new object();
        #endregion

        #region 属性
        /// <summary>
        /// 当前活动的OPC UA服务实例
        /// 默认使用异步优化服务避免UI阻塞
        /// </summary>
        public static IOpcUaService Current
        {
            get
            {
                if (_currentService == null)
                {
                    lock (_lockObject)
                    {
                        if (_currentService == null)
                        {
                            // 默认使用异步优化服务，适合工业自动化多控件场景
                            // 如果AsyncOpcUaService不可用，则使用普通服务
                            try
                            {
                                _currentService = OpcUaServiceFactory.CreateService(OpcUaServiceFactory.ServiceType.AsyncOptimized);
                            }
                            catch
                            {
                                _currentService = OpcUaGlobalService.Instance;
                            }
                        }
                    }
                }
                return _currentService;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 设置自定义服务实现
        /// </summary>
        /// <param name="service">自定义的OPC UA服务实现</param>
        public static void SetService(IOpcUaService service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            lock (_lockObject)
            {
                _currentService = service;
            }
        }

        /// <summary>
        /// 重置为默认服务实现（异步优化服务）
        /// </summary>
        public static void ResetToDefault()
        {
            lock (_lockObject)
            {
                try
                {
                    _currentService = OpcUaServiceFactory.CreateService(OpcUaServiceFactory.ServiceType.AsyncOptimized);
                }
                catch
                {
                    _currentService = OpcUaGlobalService.Instance;
                }
            }
        }

        /// <summary>
        /// 创建新的服务实例（用于测试）
        /// </summary>
        /// <returns>新的服务实例</returns>
        public static IOpcUaService CreateService()
        {
            try
            {
                return OpcUaServiceFactory.CreateService(OpcUaServiceFactory.ServiceType.AsyncOptimized);
            }
            catch
            {
                return OpcUaGlobalService.Instance;
            }
        }

        /// <summary>
        /// 创建新的服务实例并设置为当前服务
        /// </summary>
        /// <param name="serverUrl">服务器地址</param>
        /// <returns>新的服务实例</returns>
        public static IOpcUaService CreateAndSetService(string serverUrl = null)
        {
            var service = CreateService();
            if (!string.IsNullOrWhiteSpace(serverUrl))
            {
                service.DefaultServerUrl = serverUrl;
            }
            SetService(service);
            return service;
        }
        #endregion
    }
}