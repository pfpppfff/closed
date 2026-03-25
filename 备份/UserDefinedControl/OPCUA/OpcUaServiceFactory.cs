using System;

namespace UserDefinedControl.OPCUA
{
    /// <summary>
    /// OPC UA 服务工厂
    /// 用于创建不同类型的OPC UA服务实现
    /// </summary>
    public static class OpcUaServiceFactory
    {
        /// <summary>
        /// 服务类型枚举
        /// </summary>
        public enum ServiceType
        {
            /// <summary>
            /// 真实OPC UA服务（默认）
            /// </summary>
            Real,
            /// <summary>
            /// 模拟服务（用于测试）
            /// </summary>
            Mock,
            /// <summary>
            /// 异步优先服务（推荐，避免UI阻塞）
            /// </summary>
            AsyncOptimized
        }

        /// <summary>
        /// 创建指定类型的服务实例
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serverUrl">服务器地址</param>
        /// <returns>服务实例</returns>
        public static IOpcUaService CreateService(ServiceType serviceType, string serverUrl = null)
        {
            IOpcUaService service;

            switch (serviceType)
            {
                case ServiceType.Mock:
                    service = new MockOpcUaService();
                    break;
                case ServiceType.AsyncOptimized:
                    try
                    {
                        // 使用反射创建 AsyncOpcUaService 实例
                        var asyncServiceType = Type.GetType("UserDefinedControl.OPCUA.AsyncOpcUaService, " + typeof(OpcUaServiceFactory).Assembly.FullName);
                        if (asyncServiceType != null)
                        {
                            var instanceProperty = asyncServiceType.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                            service = (IOpcUaService)instanceProperty?.GetValue(null);
                        }
                        else
                        {
                            service = OpcUaGlobalService.Instance;
                        }
                    }
                    catch
                    {
                        service = OpcUaGlobalService.Instance;
                    }
                    break;
                case ServiceType.Real:
                default:
                    service = OpcUaGlobalService.Instance;
                    break;
            }

            if (!string.IsNullOrWhiteSpace(serverUrl))
            {
                service.DefaultServerUrl = serverUrl;
            }

            return service;
        }

        /// <summary>
        /// 创建真实OPC UA服务
        /// </summary>
        /// <param name="serverUrl">服务器地址</param>
        /// <returns>真实服务实例</returns>
        public static IOpcUaService CreateRealService(string serverUrl = null)
        {
            return CreateService(ServiceType.Real, serverUrl);
        }

        /// <summary>
        /// 创建异步优化的OPC UA服务（推荐）
        /// </summary>
        /// <param name="serverUrl">服务器地址</param>
        /// <returns>异步优化服务实例</returns>
        public static IOpcUaService CreateAsyncService(string serverUrl = null)
        {
            return CreateService(ServiceType.AsyncOptimized, serverUrl);
        }

        /// <summary>
        /// 创建模拟OPC UA服务
        /// </summary>
        /// <param name="serverUrl">服务器地址（模拟用）</param>
        /// <returns>模拟服务实例</returns>
        public static IOpcUaService CreateMockService(string serverUrl = null)
        {
            return CreateService(ServiceType.Mock, serverUrl);
        }

        /// <summary>
        /// 创建并配置服务实例
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="config">配置对象</param>
        /// <returns>配置好的服务实例</returns>
        public static IOpcUaService CreateServiceWithConfig(ServiceType serviceType, OpcUaConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var service = CreateService(serviceType, config.ServerUrl);
            
            // 这里可以根据配置设置更多属性
            // 如果将来需要支持更多配置项，可以在这里扩展
            
            return service;
        }

        /// <summary>
        /// 根据环境自动选择服务类型（优先选择异步服务）
        /// </summary>
        /// <param name="isTestEnvironment">是否为测试环境</param>
        /// <param name="serverUrl">服务器地址</param>
        /// <returns>适合的服务实例</returns>
        public static IOpcUaService CreateAutoService(bool isTestEnvironment = false, string serverUrl = null)
        {
            ServiceType type;
            if (isTestEnvironment)
            {
                type = ServiceType.Mock;
            }
            else
            {
                // 在生产环境中优先使用异步优化服务避免UI阻塞
                type = ServiceType.AsyncOptimized;
            }
            return CreateService(type, serverUrl);
        }
    }
}