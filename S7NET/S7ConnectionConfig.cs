using S7.Net;
using System.Configuration;

namespace S7NET.Services
{
    /// <summary>
    /// S7连接配置类
    /// </summary>
    public class S7ConnectionConfig
    {
        /// <summary>
        /// CPU类型
        /// </summary>
        public CpuType CpuType { get; set; } = CpuType.S71500;

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; } = "192.168.1.1";

        /// <summary>
        /// 机架号
        /// </summary>
        public short Rack { get; set; } = 0;

        /// <summary>
        /// 插槽号
        /// </summary>
        public short Slot { get; set; } = 1;

        /// <summary>
        /// 连接超时时间(毫秒)
        /// </summary>
        public int ConnectionTimeout { get; set; } = 5000;

        /// <summary>
        /// 心跳检测间隔(毫秒)
        /// </summary>
        public int HeartbeatInterval { get; set; } = 5000;

        /// <summary>
        /// 自动重连
        /// </summary>
        public bool AutoReconnect { get; set; } = true;

        /// <summary>
        /// 重连间隔(毫秒)
        /// </summary>
        public int ReconnectInterval { get; set; } = 3000;

        /// <summary>
        /// 最大重连次数
        /// </summary>
        public int MaxReconnectAttempts { get; set; } = 5;

        /// <summary>
        /// 从配置文件加载配置
        /// </summary>
        /// <returns></returns>
        public static S7ConnectionConfig LoadFromConfig()
        {
            var config = new S7ConnectionConfig();

            try
            {
                // 从App.config读取配置
                var cpuTypeStr = ConfigurationManager.AppSettings["S7_CpuType"];
                if (!string.IsNullOrEmpty(cpuTypeStr) && System.Enum.TryParse<CpuType>(cpuTypeStr, out var cpuType))
                {
                    config.CpuType = cpuType;
                }

                var ipAddress = ConfigurationManager.AppSettings["S7_IpAddress"];
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    config.IpAddress = ipAddress;
                }

                var rackStr = ConfigurationManager.AppSettings["S7_Rack"];
                if (!string.IsNullOrEmpty(rackStr) && short.TryParse(rackStr, out var rack))
                {
                    config.Rack = rack;
                }

                var slotStr = ConfigurationManager.AppSettings["S7_Slot"];
                if (!string.IsNullOrEmpty(slotStr) && short.TryParse(slotStr, out var slot))
                {
                    config.Slot = slot;
                }

                var timeoutStr = ConfigurationManager.AppSettings["S7_ConnectionTimeout"];
                if (!string.IsNullOrEmpty(timeoutStr) && int.TryParse(timeoutStr, out var timeout))
                {
                    config.ConnectionTimeout = timeout;
                }

                var heartbeatStr = ConfigurationManager.AppSettings["S7_HeartbeatInterval"];
                if (!string.IsNullOrEmpty(heartbeatStr) && int.TryParse(heartbeatStr, out var heartbeat))
                {
                    config.HeartbeatInterval = heartbeat;
                }

                var autoReconnectStr = ConfigurationManager.AppSettings["S7_AutoReconnect"];
                if (!string.IsNullOrEmpty(autoReconnectStr) && bool.TryParse(autoReconnectStr, out var autoReconnect))
                {
                    config.AutoReconnect = autoReconnect;
                }

                var reconnectIntervalStr = ConfigurationManager.AppSettings["S7_ReconnectInterval"];
                if (!string.IsNullOrEmpty(reconnectIntervalStr) && int.TryParse(reconnectIntervalStr, out var reconnectInterval))
                {
                    config.ReconnectInterval = reconnectInterval;
                }

                var maxReconnectStr = ConfigurationManager.AppSettings["S7_MaxReconnectAttempts"];
                if (!string.IsNullOrEmpty(maxReconnectStr) && int.TryParse(maxReconnectStr, out var maxReconnect))
                {
                    config.MaxReconnectAttempts = maxReconnect;
                }
            }
            catch (System.Exception)
            {
                // 如果读取配置失败，使用默认值
            }

            return config;
        }

        /// <summary>
        /// 验证配置是否有效
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(IpAddress))
                return false;

            if (ConnectionTimeout <= 0)
                return false;

            if (HeartbeatInterval <= 0)
                return false;

            if (ReconnectInterval <= 0)
                return false;

            if (MaxReconnectAttempts < 0)
                return false;

            return true;
        }

        /// <summary>
        /// 获取配置描述
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"CPU: {CpuType}, IP: {IpAddress}, Rack: {Rack}, Slot: {Slot}";
        }
    }
}
