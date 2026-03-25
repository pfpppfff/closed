using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text;

namespace S7NET.Services
{
    /// <summary>
    /// PLC连接诊断工具
    /// </summary>
    public static class PlcConnectionDiagnostics
    {
        /// <summary>
        /// 诊断PLC连接问题
        /// </summary>
        /// <param name="ipAddress">PLC IP地址</param>
        /// <param name="port">端口号（默认502）</param>
        /// <returns>诊断结果</returns>
        public static async Task<PlcDiagnosticResult> DiagnosePlcConnectionAsync(string ipAddress, int port = 502)
        {
            var result = new PlcDiagnosticResult
            {
                IpAddress = ipAddress,
                Port = port,
                DiagnosticTime = DateTime.Now
            };

            var diagnostics = new List<string>();

            try
            {
                // 1. Ping测试
                diagnostics.Add("开始Ping测试...");
                var pingResult = await PingTestAsync(ipAddress);
                result.PingSuccessful = pingResult.Success;
                result.PingTime = pingResult.RoundtripTime;
                
                if (pingResult.Success)
                {
                    diagnostics.Add($"✓ Ping成功 - 响应时间: {pingResult.RoundtripTime}ms");
                }
                else
                {
                    diagnostics.Add($"✗ Ping失败 - {pingResult.Status}");
                }

                // 2. 端口连通性测试
                diagnostics.Add($"开始端口{port}连通性测试...");
                var portResult = await TestPortConnectivityAsync(ipAddress, port);
                result.PortAccessible = portResult.Success;
                result.PortResponseTime = portResult.ResponseTime;

                if (portResult.Success)
                {
                    diagnostics.Add($"✓ 端口{port}可访问 - 响应时间: {portResult.ResponseTime}ms");
                }
                else
                {
                    diagnostics.Add($"✗ 端口{port}不可访问 - {portResult.Error}");
                }

                // 3. S7连接测试
                diagnostics.Add("开始S7连接测试...");
                var s7Result = await TestS7ConnectionAsync(ipAddress);
                result.S7ConnectionSuccessful = s7Result.Success;
                result.S7Error = s7Result.Error;

                if (s7Result.Success)
                {
                    diagnostics.Add("✓ S7连接测试成功");
                }
                else
                {
                    diagnostics.Add($"✗ S7连接测试失败 - {s7Result.Error}");
                }

                // 4. 生成建议
                result.Recommendations = GenerateRecommendations(result);

            }
            catch (Exception ex)
            {
                diagnostics.Add($"诊断过程中发生异常: {ex.Message}");
                result.DiagnosticError = ex.Message;
            }

            result.DiagnosticLog = diagnostics;
            return result;
        }

        private static async Task<(bool Success, long RoundtripTime, IPStatus Status)> PingTestAsync(string ipAddress)
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = await ping.SendPingAsync(ipAddress, 5000);
                    return (reply.Status == IPStatus.Success, reply.RoundtripTime, reply.Status);
                }
            }
            catch (Exception)
            {
                return (false, 0, IPStatus.Unknown);
            }
        }

        private static async Task<(bool Success, long ResponseTime, string Error)> TestPortConnectivityAsync(string ipAddress, int port)
        {
            try
            {
                var startTime = DateTime.Now;
                using (var client = new TcpClient())
                {
                    await client.ConnectAsync(ipAddress, port);
                    var responseTime = (long)(DateTime.Now - startTime).TotalMilliseconds;
                    return (true, responseTime, null);
                }
            }
            catch (Exception ex)
            {
                return (false, 0, ex.Message);
            }
        }

        private static async Task<(bool Success, string Error)> TestS7ConnectionAsync(string ipAddress)
        {
            try
            {
                // 尝试创建S7连接进行测试
                using (var plc = new S7.Net.Plc(S7.Net.CpuType.S71500, ipAddress, 0, 1))
                {
                    await Task.Run(() => plc.Open());
                    plc.Close();
                    return (true, null);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private static List<string> GenerateRecommendations(PlcDiagnosticResult result)
        {
            var recommendations = new List<string>();

            if (!result.PingSuccessful)
            {
                recommendations.Add("• 检查网络连接和PLC电源状态");
                recommendations.Add("• 确认IP地址配置正确");
                recommendations.Add("• 检查网络防火墙设置");
            }
            else if (result.PingTime > 100)
            {
                recommendations.Add("• 网络延迟较高，考虑优化网络环境");
            }

            if (!result.PortAccessible)
            {
                recommendations.Add("• 检查PLC是否启用了Ethernet通信");
                recommendations.Add("• 确认端口502未被防火墙阻止");
                recommendations.Add("• 检查PLC的连接数是否已满");
            }

            if (!result.S7ConnectionSuccessful)
            {
                if (result.S7Error?.Contains("连接") == true)
                {
                    recommendations.Add("• 检查PLC项目中是否启用了PUT/GET通信");
                    recommendations.Add("• 确认机架号和插槽号配置正确");
                    recommendations.Add("• 检查是否有其他程序占用PLC连接");
                }
            }

            if (recommendations.Count == 0)
            {
                recommendations.Add("• 连接诊断正常，如仍有问题请检查程序逻辑");
            }

            return recommendations;
        }
    }

    /// <summary>
    /// PLC诊断结果
    /// </summary>
    public class PlcDiagnosticResult
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public DateTime DiagnosticTime { get; set; }
        
        public bool PingSuccessful { get; set; }
        public long PingTime { get; set; }
        
        public bool PortAccessible { get; set; }
        public long PortResponseTime { get; set; }
        
        public bool S7ConnectionSuccessful { get; set; }
        public string S7Error { get; set; }
        
        public List<string> DiagnosticLog { get; set; } = new List<string>();
        public List<string> Recommendations { get; set; } = new List<string>();
        public string DiagnosticError { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"PLC连接诊断报告 - {IpAddress}:{Port}");
            sb.AppendLine($"诊断时间: {DiagnosticTime:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();
            
            sb.AppendLine("诊断结果:");
            sb.AppendLine($"  Ping测试: {(PingSuccessful ? "成功" : "失败")} ({PingTime}ms)");
            sb.AppendLine($"  端口连通性: {(PortAccessible ? "成功" : "失败")} ({PortResponseTime}ms)");
            sb.AppendLine($"  S7连接: {(S7ConnectionSuccessful ? "成功" : "失败")}");
            
            if (!string.IsNullOrEmpty(S7Error))
            {
                sb.AppendLine($"  S7错误: {S7Error}");
            }
            
            sb.AppendLine();
            sb.AppendLine("建议措施:");
            foreach (var recommendation in Recommendations)
            {
                sb.AppendLine(recommendation);
            }
            
            return sb.ToString();
        }
    }
}
