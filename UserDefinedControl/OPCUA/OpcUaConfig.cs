using System;

namespace UserDefinedControl.OPCUA
{
    /// <summary>
    /// OPC UA 配置管理类
    /// 用于管理连接配置和系统设置
    /// </summary>
    public class OpcUaConfig
    {
        #region 默认配置
        /// <summary>
        /// 默认服务器地址
        /// </summary>
        public const string DefaultServerUrl = "opc.tcp://127.0.0.1:49320";
        
        /// <summary>
        /// 默认连接超时时间（毫秒）
        /// </summary>
        public const int DefaultTimeout = 5000;
        
        /// <summary>
        /// 默认会话超时时间（毫秒）
        /// </summary>
        public const int DefaultSessionTimeout = 60000;
        #endregion

        #region 属性
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServerUrl { get; set; } = DefaultServerUrl;
        
        /// <summary>
        /// 连接超时时间
        /// </summary>
        public int ConnectionTimeout { get; set; } = DefaultTimeout;
        
        /// <summary>
        /// 会话超时时间
        /// </summary>
        public int SessionTimeout { get; set; } = DefaultSessionTimeout;
        
        /// <summary>
        /// 是否启用自动重连
        /// </summary>
        public bool AutoReconnect { get; set; } = true;
        
        /// <summary>
        /// 重连间隔时间（毫秒）
        /// </summary>
        public int ReconnectInterval { get; set; } = 3000;
        
        /// <summary>
        /// 最大重连尝试次数
        /// </summary>
        public int MaxReconnectAttempts { get; set; } = 5;
        #endregion

        #region 方法
        /// <summary>
        /// 从配置文件加载配置
        /// </summary>
        /// <returns>配置实例</returns>
        public static OpcUaConfig LoadFromConfig()
        {
            var config = new OpcUaConfig();
            
            try
            {
                // 由于可能缺少 System.Configuration 引用，暂时使用默认配置
                // 如需使用配置文件，请添加对 System.Configuration 的引用
                
                // config.ServerUrl = ConfigurationManager.AppSettings["OpcUa.ServerUrl"] ?? DefaultServerUrl;
                // if (int.TryParse(ConfigurationManager.AppSettings["OpcUa.ConnectionTimeout"], out int connTimeout))
                //     config.ConnectionTimeout = connTimeout;
                // if (int.TryParse(ConfigurationManager.AppSettings["OpcUa.SessionTimeout"], out int sessionTimeout))
                //     config.SessionTimeout = sessionTimeout;
                // if (bool.TryParse(ConfigurationManager.AppSettings["OpcUa.AutoReconnect"], out bool autoReconnect))
                //     config.AutoReconnect = autoReconnect;
                // if (int.TryParse(ConfigurationManager.AppSettings["OpcUa.ReconnectInterval"], out int reconnectInterval))
                //     config.ReconnectInterval = reconnectInterval;
                // if (int.TryParse(ConfigurationManager.AppSettings["OpcUa.MaxReconnectAttempts"], out int maxAttempts))
                //     config.MaxReconnectAttempts = maxAttempts;
                
                // 使用默认配置
                config.ServerUrl = DefaultServerUrl;
                config.ConnectionTimeout = DefaultTimeout;
                config.SessionTimeout = DefaultSessionTimeout;
                config.AutoReconnect = true;
                config.ReconnectInterval = 3000;
                config.MaxReconnectAttempts = 5;
            }
            catch (Exception ex)
            {
                // 配置加载失败时使用默认配置
                System.Diagnostics.Debug.WriteLine($"配置加载失败，使用默认配置: {ex.Message}");
            }
            
            return config;
        }
        
        /// <summary>
        /// 验证配置是否有效
        /// </summary>
        /// <returns>是否有效</returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ServerUrl) &&
                   Uri.TryCreate(ServerUrl, UriKind.Absolute, out _) &&
                   ConnectionTimeout > 0 &&
                   SessionTimeout > 0 &&
                   ReconnectInterval > 0 &&
                   MaxReconnectAttempts > 0;
        }
        
        /// <summary>
        /// 获取配置摘要信息
        /// </summary>
        /// <returns>配置摘要</returns>
        public override string ToString()
        {
            return $"OPC UA 配置 - 服务器: {ServerUrl}, 连接超时: {ConnectionTimeout}ms, 会话超时: {SessionTimeout}ms, 自动重连: {AutoReconnect}";
        }
        #endregion
    }
}