using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OPC_UA通讯
{
    public partial class 订阅 : Form
    {
        private Session _session;
        private Subscription _subscription;
        private MonitoredItem _monitoredItem;
        private bool _isConnected = false;
        
        // 性能监控变量
        private DateTime _lastUpdateTime = DateTime.MinValue;
        private List<double> _updateIntervals = new List<double>();
        private const int MAX_INTERVALS_TO_TRACK = 100;
        private int _updatesUnder50ms = 0;
        private int _totalUpdates = 0;

        // 可配置的服务器URL和节点ID
        private string _serverUrl = "opc.tcp://127.0.0.1:49320";
        private string _nodeId = "ns=2;s=1214.PLC1.DisData.Flow.Flow_1";

        public 订阅()
        {
            InitializeComponent();
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_isConnected)
                {
                    _serverUrl = txtServerUrl.Text.Trim();
                    if (string.IsNullOrEmpty(_serverUrl))
                    {
                        MessageBox.Show("请输入服务器URL", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    await ConnectToOpcServerAsync();
                }
                else
                {
                    Cleanup();
                }
            }
            catch (Exception ex)
            {
                AppendLog($"❌ 操作失败: {ex.Message}");
                MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [Obsolete]
        private async Task ConnectToOpcServerAsync()
        {
            AppendLog($"正在连接 OPC UA 服务器: {_serverUrl}...");

            try
            {
                // 使用简单可靠的连接方式
                var endpoint = new ConfiguredEndpoint(null, new EndpointDescription(_serverUrl));

                _session = await Session.Create(
                  new ApplicationConfiguration()
                  {
                      ClientConfiguration = new ClientConfiguration()
                  },
                  new ConfiguredEndpoint(null, new EndpointDescription(_serverUrl)),
                  true,
                  "fsfg",
                  5000,
                  new UserIdentity(),
                 new List<string> { }
                  );      // 使用同步方式

                // 订阅会话事件
                _session.KeepAlive += Session_KeepAlive;

                _isConnected = true;
                AppendLog("✅ OPC UA 连接成功！");
                UpdateConnectionStatus();
                
                // 创建订阅
                CreateSubscription();
            }
            catch (Exception ex)
            {
                _isConnected = false;
                AppendLog($"❌ OPC UA 连接失败: {ex.Message}");
                UpdateConnectionStatus();
                throw;
            }
        }

        // KeepAlive事件处理方法
        private void Session_KeepAlive(ISession session, KeepAliveEventArgs e)
        {
            if (e.Status != null && ServiceResult.IsNotGood(e.Status))
            {
                AppendLog($"⚠️ 会话状态异常: {e.Status}");
            }
        }

        private void CreateSubscription()
        {
            if (!_isConnected) return;

            try
            {
                var subscription = new Subscription(_session.DefaultSubscription)
                {
                    DisplayName = "RealTimeDataSub",
                    PublishingInterval = 10, // 降低发布间隔到10ms以实现<50ms的更新
                    KeepAliveCount = 10,
                    LifetimeCount = 30,
                    PublishingEnabled = true,
                    Priority = 100 // 设置最高优先级
                };

                _monitoredItem = new MonitoredItem(subscription.DefaultItem)
                {
                    DisplayName = "MonitoredItem",
                    StartNodeId = _nodeId,
                    AttributeId = Attributes.Value,
                    SamplingInterval = 10, // 降低采样间隔到10ms以实现<50ms的更新
                    QueueSize = 1, // 减小队列大小以减少延迟
                    DiscardOldest = true,
                    MonitoringMode = MonitoringMode.Reporting // 确保使用报告模式
                };

                _monitoredItem.Notification += OnDataChange;
                subscription.AddItem(_monitoredItem);

                _session.AddSubscription(subscription);
                subscription.Create();

                _subscription = subscription;
                AppendLog($"✅ 订阅已创建...");
            }
            catch (Exception ex)
            {
                AppendLog($"❌ 创建订阅失败: {ex.Message}");
                throw;
            }
        }

        private void OnDataChange(MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<MonitoredItem, MonitoredItemNotificationEventArgs>(OnDataChange), item, e);
                return;
            }

            try
            {
                // 获取当前时间用于性能测量
                DateTime now = DateTime.Now;
                
                // 计算更新间隔时间
                if (_lastUpdateTime != DateTime.MinValue)
                {
                    double intervalMs = (now - _lastUpdateTime).TotalMilliseconds;
                    
                    // 记录更新间隔
                    _updateIntervals.Add(intervalMs);
                    if (_updateIntervals.Count > MAX_INTERVALS_TO_TRACK)
                    {
                        _updateIntervals.RemoveAt(0);
                    }
                    
                    // 统计性能
                    _totalUpdates++;
                    if (intervalMs < 50)
                    {
                        _updatesUnder50ms++;
                    }
                    
                    // 计算性能百分比
                    double performancePercent = _totalUpdates > 0 
                        ? (double)_updatesUnder50ms / _totalUpdates * 100 
                        : 0;
                    
                    // 更新性能统计显示
                    lblPerformance.Text = $"性能: {performancePercent:F1}% 更新 < 50ms (间隔: {intervalMs:F1}ms)";
                    lblPerformance.ForeColor = intervalMs < 50 ? Color.Green : Color.Red;
                }
                
                // 更新最后更新时间
                _lastUpdateTime = now;

                // 直接处理通知值
                if (e.NotificationValue is MonitoredItemNotification notification)
                {
                    var dataValue = notification.Value;
                    var serverTimestamp = dataValue.SourceTimestamp == DateTime.MinValue
                        ? DateTime.UtcNow
                        : dataValue.SourceTimestamp;

                    // 计算"扫描时间"：当前时间 - 服务器时间戳
                    var scanDelayMs = (long)(DateTime.UtcNow - serverTimestamp).TotalMilliseconds;

                    // 获取值
                    var value = dataValue.Value?.ToString() ?? "N/A";
                    
                    // 显示数据
                    lblValue.Text = $"值: {value}";
                    lblScanTime.Text = $"扫描延迟: {scanDelayMs} ms";

                    // 日志记录
                    AppendLog($"[{now:HH:mm:ss.fff}] 值: {value}, 扫描延迟: {scanDelayMs} ms, 更新间隔: {(_updateIntervals.Count > 0 ? _updateIntervals.Last().ToString("F1") : "N/A")} ms");

                    // 高亮超时
                    lblScanTime.ForeColor = scanDelayMs > 50 ? Color.Red : Color.Green;
                }
            }
            catch (Exception ex)
            {
                AppendLog($"❌ 处理数据变更通知时出错: {ex.Message}");
            }
        }

        private void Cleanup()
        {
            try
            {
                _subscription?.Delete(true);
                _subscription?.Dispose();
                _subscription = null;
                
                _monitoredItem = null;
                
                if (_session != null)
                {
                    // 移除事件处理程序
                    _session.KeepAlive -= Session_KeepAlive;
                    _session.Close();
                    _session.Dispose();
                    _session = null;
                }
                
                _isConnected = false;
                UpdateConnectionStatus();
                AppendLog("✅ 连接已断开");
            }
            catch (Exception ex)
            {
                AppendLog($"❌ 清理资源时出错: {ex.Message}");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Cleanup();
            base.OnFormClosing(e);
        }

        private void UpdateConnectionStatus()
        {
            if (_isConnected)
            {
                btnConnect.Text = "断开连接";
                lblConnectionStatus.Text = "已连接";
                lblConnectionStatus.ForeColor = Color.Green;
            }
            else
            {
                btnConnect.Text = "连接";
                lblConnectionStatus.Text = "未连接";
                lblConnectionStatus.ForeColor = Color.Red;
            }
        }

        private void AppendLog(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(AppendLog), message);
                return;
            }

            try
            {
                var logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}\r\n";
                
                // 限制日志行数
                if (txtLog.Lines.Length > 100)
                {
                    var lines = txtLog.Lines;
                    txtLog.Clear();
                    for (int i = lines.Length - 99; i < lines.Length; i++)
                    {
                        txtLog.AppendText(lines[i]);
                    }
                }
                
                txtLog.AppendText(logLine);
                txtLog.ScrollToCaret();
            }
            catch (Exception ex)
            {
                // 忽略日志记录错误
                System.Diagnostics.Debug.WriteLine($"记录日志时出错: {ex.Message}");
            }
        }

        private void 订阅_Load(object sender, EventArgs e)
        {
            try
            {
                UpdateConnectionStatus();
                txtServerUrl.Text = _serverUrl;
                AppendLog("✅ OPC UA 订阅客户端已启动");
            }
            catch (Exception ex)
            {
                AppendLog($"❌ 启动失败: {ex.Message}");
            }
        }
    }
}