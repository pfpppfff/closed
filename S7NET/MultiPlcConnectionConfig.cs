using S7.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace S7NET.Services
{
    /// <summary>
    /// 单个PLC连接配置
    /// </summary>
    public class PlcConnectionInfo
    {
        /// <summary>
        /// PLC唯一标识符
        /// </summary>
        public string PlcId { get; set; }

        /// <summary>
        /// PLC名称/描述
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// CPU类型
        /// </summary>
        public CpuType CpuType { get; set; } = CpuType.S71500;

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }

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
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 验证配置是否有效
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(PlcId))
                return false;

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
            return $"[{PlcId}] {Name} - CPU: {CpuType}, IP: {IpAddress}, Rack: {Rack}, Slot: {Slot}";
        }
    }

    /// <summary>
    /// 多PLC连接配置管理器
    /// </summary>
    public class MultiPlcConnectionConfig
    {
        /// <summary>
        /// PLC连接配置列表
        /// </summary>
        public List<PlcConnectionInfo> PlcConnections { get; set; } = new List<PlcConnectionInfo>();

        /// <summary>
        /// 默认PLC ID
        /// </summary>
        public string DefaultPlcId { get; set; }

        /// <summary>
        /// 全局缓存过期时间(毫秒)
        /// </summary>
        public int GlobalCacheExpirationMs { get; set; } = 100;

        /// <summary>
        /// 从配置文件加载多PLC配置
        /// </summary>
        /// <returns></returns>
        public static MultiPlcConnectionConfig LoadFromConfig()
        {
            var config = new MultiPlcConnectionConfig();

            try
            {
                // 读取全局设置
                var defaultPlcId = ConfigurationManager.AppSettings["S7_DefaultPlcId"];
                if (!string.IsNullOrEmpty(defaultPlcId))
                {
                    config.DefaultPlcId = defaultPlcId;
                }

                var cacheExpirationStr = ConfigurationManager.AppSettings["S7_GlobalCacheExpirationMs"];
                if (!string.IsNullOrEmpty(cacheExpirationStr) && int.TryParse(cacheExpirationStr, out var cacheExpiration))
                {
                    config.GlobalCacheExpirationMs = cacheExpiration;
                }

                // 读取PLC数量
                var plcCountStr = ConfigurationManager.AppSettings["S7_PlcCount"];
                if (!string.IsNullOrEmpty(plcCountStr) && int.TryParse(plcCountStr, out var plcCount))
                {
                    for (int i = 1; i <= plcCount; i++)
                    {
                        var plcInfo = LoadPlcConfigFromIndex(i);
                        if (plcInfo != null && plcInfo.IsValid())
                        {
                            config.PlcConnections.Add(plcInfo);
                        }
                    }
                }

                // 如果没有配置PLC数量，尝试加载默认的两个PLC
                if (config.PlcConnections.Count == 0)
                {
                    var plc1 = LoadPlcConfigFromIndex(1);
                    var plc2 = LoadPlcConfigFromIndex(2);

                    if (plc1 != null && plc1.IsValid())
                        config.PlcConnections.Add(plc1);
                    if (plc2 != null && plc2.IsValid())
                        config.PlcConnections.Add(plc2);
                }

                // 如果仍然没有PLC配置，创建默认配置
                if (config.PlcConnections.Count == 0)
                {
                    config.PlcConnections.Add(new PlcConnectionInfo
                    {
                        PlcId = "PLC1",
                        Name = "主PLC",
                        IpAddress = "192.168.2.10",
                        CpuType = CpuType.S71500
                    });

                    config.PlcConnections.Add(new PlcConnectionInfo
                    {
                        PlcId = "PLC2",
                        Name = "副PLC",
                        IpAddress = "192.168.2.11",
                        CpuType = CpuType.S71500
                    });
                }

                // 设置默认PLC
                if (string.IsNullOrEmpty(config.DefaultPlcId) && config.PlcConnections.Count > 0)
                {
                    config.DefaultPlcId = config.PlcConnections[0].PlcId;
                }
            }
            catch (Exception)
            {
                // 如果读取配置失败，使用默认值
            }

            return config;
        }

        private static PlcConnectionInfo LoadPlcConfigFromIndex(int index)
        {
            try
            {
                var plcId = ConfigurationManager.AppSettings[$"S7_Plc{index}_Id"];
                if (string.IsNullOrEmpty(plcId))
                    return null;

                var plcInfo = new PlcConnectionInfo
                {
                    PlcId = plcId,
                    Name = ConfigurationManager.AppSettings[$"S7_Plc{index}_Name"] ?? $"PLC{index}"
                };

                var cpuTypeStr = ConfigurationManager.AppSettings[$"S7_Plc{index}_CpuType"];
                if (!string.IsNullOrEmpty(cpuTypeStr) && Enum.TryParse<CpuType>(cpuTypeStr, out var cpuType))
                {
                    plcInfo.CpuType = cpuType;
                }

                var ipAddress = ConfigurationManager.AppSettings[$"S7_Plc{index}_IpAddress"];
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    plcInfo.IpAddress = ipAddress;
                }

                var rackStr = ConfigurationManager.AppSettings[$"S7_Plc{index}_Rack"];
                if (!string.IsNullOrEmpty(rackStr) && short.TryParse(rackStr, out var rack))
                {
                    plcInfo.Rack = rack;
                }

                var slotStr = ConfigurationManager.AppSettings[$"S7_Plc{index}_Slot"];
                if (!string.IsNullOrEmpty(slotStr) && short.TryParse(slotStr, out var slot))
                {
                    plcInfo.Slot = slot;
                }

                var timeoutStr = ConfigurationManager.AppSettings[$"S7_Plc{index}_ConnectionTimeout"];
                if (!string.IsNullOrEmpty(timeoutStr) && int.TryParse(timeoutStr, out var timeout))
                {
                    plcInfo.ConnectionTimeout = timeout;
                }

                var heartbeatStr = ConfigurationManager.AppSettings[$"S7_Plc{index}_HeartbeatInterval"];
                if (!string.IsNullOrEmpty(heartbeatStr) && int.TryParse(heartbeatStr, out var heartbeat))
                {
                    plcInfo.HeartbeatInterval = heartbeat;
                }

                var autoReconnectStr = ConfigurationManager.AppSettings[$"S7_Plc{index}_AutoReconnect"];
                if (!string.IsNullOrEmpty(autoReconnectStr) && bool.TryParse(autoReconnectStr, out var autoReconnect))
                {
                    plcInfo.AutoReconnect = autoReconnect;
                }

                var enabledStr = ConfigurationManager.AppSettings[$"S7_Plc{index}_Enabled"];
                if (!string.IsNullOrEmpty(enabledStr) && bool.TryParse(enabledStr, out var enabled))
                {
                    plcInfo.Enabled = enabled;
                }

                return plcInfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据ID获取PLC配置
        /// </summary>
        /// <param name="plcId"></param>
        /// <returns></returns>
        public PlcConnectionInfo GetPlcConfig(string plcId)
        {
            return PlcConnections.FirstOrDefault(p => p.PlcId.Equals(plcId, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 获取默认PLC配置
        /// </summary>
        /// <returns></returns>
        public PlcConnectionInfo GetDefaultPlcConfig()
        {
            if (!string.IsNullOrEmpty(DefaultPlcId))
            {
                var defaultPlc = GetPlcConfig(DefaultPlcId);
                if (defaultPlc != null)
                    return defaultPlc;
            }

            return PlcConnections.FirstOrDefault();
        }

        /// <summary>
        /// 验证配置是否有效
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return PlcConnections.Count > 0 && PlcConnections.All(p => p.IsValid());
        }

        /// <summary>
        /// 获取所有启用的PLC ID
        /// </summary>
        /// <returns></returns>
        public List<string> GetEnabledPlcIds()
        {
            return PlcConnections.Where(p => p.Enabled).Select(p => p.PlcId).ToList();
        }
    }
}
