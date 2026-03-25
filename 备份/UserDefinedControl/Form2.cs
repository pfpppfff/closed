using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserDefinedControl.OPCUA;

namespace UserDefinedControl
{
    /// <summary>
    /// OPC UA实时数据监控窗体
    /// 根据系统状态智能读取数据，符合工业自动化安全要求
    /// </summary>
    public partial class Form2 : Form
    {
        #region 字段和属性
        private IOpcUaService _opcService;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isMonitoring = false;
        
        // 监控的OPC地址和数据类型
        private readonly Dictionary<string, Type> _monitoringNodes = new Dictionary<string, Type>
        {
            { "1214.PLC1._System._NoError", typeof(bool) },
            { "1214.PLC1.DisData.Flow.Flow_1", typeof(float) },
            { "1214.PLC1.DisData.Flow.Flow_2", typeof(float) },
            { "1214.PLC1.DisData.Flow.Flow_3", typeof(float) }
        };
        
        // 存储当前读取的数据
        private Dictionary<string, object> _currentData = new Dictionary<string, object>();
        private bool _systemNoError = false; // PLC系统状态
        
        // UI控件
        private Label _systemStatusLabel;
        private Label _flow1Label;
        private Label _flow2Label;
        private Label _flow3Label;
        private Button _startButton;
        private Button _stopButton;
        private Button _connectButton;
        private TextBox _logTextBox;
        private GroupBox _dataGroup;
        private GroupBox _controlGroup;
        private ProgressBar _progressBar;
        #endregion

        #region 构造函数和初始化
        public Form2()
        {
            InitializeComponent();
            CreateUIControls(); // 创建必要的UI控件
            InitializeOpcService();
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(468, 214);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form2
            // 
            this.ClientSize = new System.Drawing.Size(560, 261);
            this.Controls.Add(this.button1);
            this.Name = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);

        }

        /// <summary>
        /// 创建必要的UI控件，确保在InitializeOpcService之前初始化
        /// </summary>
        private void CreateUIControls()
        {
            // 采用用户偏好的简洁美观设计风格
            this.Text = "OPC UA 实时数据监控";
            this.Size = new Size(600, 500);
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // 控制区域
            _controlGroup = new GroupBox
            {
                Text = "连接控制",
                Location = new Point(10, 10),
                Size = new Size(560, 80),
                ForeColor = Color.DarkGray,
                Font = new Font("微软雅黑", 9F, FontStyle.Regular)
            };

            _connectButton = new Button
            {
                Text = "连接OPC服务",
                Location = new Point(10, 25),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(200, 255, 200),
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false,
                Font = new Font("微软雅黑", 9F, FontStyle.Regular),
                ForeColor = Color.DarkGray
            };
            _connectButton.FlatAppearance.BorderColor = Color.LightGray;

            _startButton = new Button
            {
                Text = "开始监控",
                Location = new Point(120, 25),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(200, 230, 255),
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false,
                Font = new Font("微软雅黑", 9F, FontStyle.Regular),
                ForeColor = Color.DarkGray,
                Enabled = false
            };
            _startButton.FlatAppearance.BorderColor = Color.LightGray;

            _stopButton = new Button
            {
                Text = "停止监控",
                Location = new Point(230, 25),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(255, 220, 220),
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false,
                Font = new Font("微软雅黑", 9F, FontStyle.Regular),
                ForeColor = Color.DarkGray,
                Enabled = false
            };
            _stopButton.FlatAppearance.BorderColor = Color.LightGray;

            _progressBar = new ProgressBar
            {
                Location = new Point(350, 25),
                Size = new Size(200, 30),
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Visible = false
            };

            _controlGroup.Controls.AddRange(new Control[] {
                _connectButton, _startButton, _stopButton, _progressBar
            });

            // 数据显示区域
            _dataGroup = new GroupBox
            {
                Text = "实时数据",
                Location = new Point(10, 100),
                Size = new Size(560, 150),
                ForeColor = Color.DarkGray,
                Font = new Font("微软雅黑", 9F, FontStyle.Regular)
            };

            _systemStatusLabel = new Label
            {
                Text = "系统状态: 未连接",
                Location = new Point(10, 25),
                Size = new Size(540, 25),
                ForeColor = Color.Red,
                Font = new Font("微软雅黑", 10F, FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _flow1Label = new Label
            {
                Text = "流量1: --",
                Location = new Point(10, 55),
                Size = new Size(175, 25),
                ForeColor = Color.DarkGray,
                Font = new Font("微软雅黑", 9F, FontStyle.Regular),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _flow2Label = new Label
            {
                Text = "流量2: --",
                Location = new Point(190, 55),
                Size = new Size(175, 25),
                ForeColor = Color.DarkGray,
                Font = new Font("微软雅黑", 9F, FontStyle.Regular),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _flow3Label = new Label
            {
                Text = "流量3: --",
                Location = new Point(370, 55),
                Size = new Size(175, 25),
                ForeColor = Color.DarkGray,
                Font = new Font("微软雅黑", 9F, FontStyle.Regular),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var updateTimeLabel = new Label
            {
                Text = "更新频率: 500ms | 当系统状态为False时停止读取流量数据",
                Location = new Point(10, 90),
                Size = new Size(540, 20),
                ForeColor = Color.Gray,
                Font = new Font("微软雅黑", 8F, FontStyle.Regular)
            };

            _dataGroup.Controls.AddRange(new Control[] {
                _systemStatusLabel, _flow1Label, _flow2Label, _flow3Label, updateTimeLabel
            });

            // 日志区域
            var logGroup = new GroupBox
            {
                Text = "操作日志",
                Location = new Point(10, 260),
                Size = new Size(560, 180),
                ForeColor = Color.DarkGray,
                Font = new Font("微软雅黑", 9F, FontStyle.Regular)
            };

            _logTextBox = new TextBox
            {
                Location = new Point(10, 20),
                Size = new Size(540, 150),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 8F, FontStyle.Regular)
            };

            logGroup.Controls.Add(_logTextBox);

            // 添加到窗体
            this.Controls.AddRange(new Control[] { _controlGroup, _dataGroup, logGroup });

            // 绑定事件
            _connectButton.Click += ConnectButton_Click;
            _startButton.Click += StartButton_Click;
            _stopButton.Click += StopButton_Click;
            this.FormClosing += Form2_FormClosing;
        }

        private void InitializeOpcService()
        {
            try
            {
                // 使用异步优化的服务
                _opcService = OpcUaServiceManager.Current;
                
                // 注册连接状态变化事件
                _opcService.ConnectionStatusChanged += OnConnectionStatusChanged;
                
                // 注册PLC状态变化事件
                _opcService.PlcStatusChanged += OnPlcStatusChanged;
                
                LogMessage("OPC UA 服务初始化完成");
            }
            catch (Exception ex)
            {
                LogMessage($"OPC UA 服务初始化失败: {ex.Message}");
            }
        }
        #endregion

        #region 事件处理
        private async void ConnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                _connectButton.Enabled = false;
                LogMessage("正在连接OPC UA服务器...");
                
                // 使用服务管理器连接
                if (_opcService != null)
                {
                    _opcService.Connect();
                }
                else
                {
                    throw new InvalidOperationException("OPC UA服务未初始化");
                }
                
                LogMessage("OPC UA服务器连接成功");
            }
            catch (Exception ex)
            {
                LogMessage($"连接失败: {ex.Message}");
                MessageBox.Show($"连接失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _connectButton.Enabled = true;
            }
        }

        private async void StartButton_Click(object sender, EventArgs e)
        {
            if (!_opcService.IsConnected)
            {
                MessageBox.Show("请先连接OPC UA服务器", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await StartMonitoringAsync();
        }

        private async void StopButton_Click(object sender, EventArgs e)
        {
            await StopMonitoringAsync();
        }

        private void OnConnectionStatusChanged(object sender, bool isConnected)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<object, bool>(OnConnectionStatusChanged), sender, isConnected);
                return;
            }

            _startButton.Enabled = isConnected && !_isMonitoring;
            _connectButton.Text = isConnected ? "已连接" : "连接OPC服务";
            _connectButton.Enabled = !isConnected;

            if (!isConnected && _isMonitoring)
            {
                _ = StopMonitoringAsync();
            }
            
            LogMessage($"OPC连接状态变化: {(isConnected ? "已连接" : "已断开")}");
        }

        /// <summary>
        /// PLC状态变化事件处理
        /// </summary>
        private void OnPlcStatusChanged(object sender, PlcStatusChangedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<object, PlcStatusChangedEventArgs>(OnPlcStatusChanged), sender, e);
                return;
            }

            // 记录PLC状态变化
            LogMessage($"PLC状态变化: {e.GetStatusChangeDescription()}");
            
            // 如果是主控PLC，更新显示
            if (e.PlcId == "PLC1")
            {
                if (e.IsDegraded)
                {
                    LogMessage("⚠️ 警告: 主控PLC状态异常！");
                    // 可以添加声音提醒或其他告警机制
                }
                else if (e.IsRecovered)
                {
                    LogMessage("✅ 恢复: 主控PLC状态恢复正常");
                }
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            _ = StopMonitoringAsync();
        }
        #endregion

        #region 数据监控方法
        /// <summary>
        /// 开始数据监控
        /// </summary>
        private async Task StartMonitoringAsync()
        {
            if (_isMonitoring) return;

            try
            {
                _isMonitoring = true;
                _cancellationTokenSource = new CancellationTokenSource();

                // 更新UI状态
                UpdateUI(() =>
                {
                    _startButton.Enabled = false;
                    _stopButton.Enabled = true;
                    _progressBar.Visible = true;
                });

                LogMessage("开始实时数据监控...");

                // 启动数据监控任务
                await StartDataMonitoringLoop();
            }
            catch (Exception ex)
            {
                LogMessage($"启动监控失败: {ex.Message}");
                await StopMonitoringAsync();
            }
        }

        /// <summary>
        /// 停止数据监控
        /// </summary>
        private async Task StopMonitoringAsync()
        {
            if (!_isMonitoring) return;

            _isMonitoring = false;
            _cancellationTokenSource?.Cancel();

            // 更新UI状态
            UpdateUI(() =>
            {
                _startButton.Enabled = _opcService.IsConnected;
                _stopButton.Enabled = false;
                _progressBar.Visible = false;
            });

            LogMessage("数据监控已停止");
        }

        /// <summary>
        /// 数据监控循环
        /// </summary>
        private async Task StartDataMonitoringLoop()
        {
            await Task.Run(async () =>
            {
                while (_isMonitoring && !_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        await ReadOpcDataAsync();
                        await Task.Delay(500, _cancellationTokenSource.Token); // 500ms 更新间隔
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"数据读取错误: {ex.Message}");
                        await Task.Delay(1000, _cancellationTokenSource.Token); // 错误时延长间隔
                    }
                }
            }, _cancellationTokenSource.Token);
        }


        PlcStatusModel plc1Status;
        PlcStatusModel plc2Status;
        /// <summary>
        /// 读取OPC数据（根据系统状态智能读取）
        /// 支持多PLC状态管理
        /// </summary>
        private async Task ReadOpcDataAsync()
        {
            try
            {
                // 获取主控PLC状态
                //var plc1Status = _opcService.GetPlcStatus("PLC1");
                //var plc2Status = _opcService.GetPlcStatus("PLC2");
               
                // 更新主控PLC状态
                await _opcService.UpdatePlcSystemStatusAsync("PLC1");
                await _opcService.UpdatePlcSystemStatusAsync("PLC2");

                // 获取更新后的状态
                plc1Status = _opcService.GetPlcStatus("PLC1");
                plc2Status = _opcService.GetPlcStatus("PLC2");
                bool systemNoError = plc1Status.IsSystemNormal;
                bool systemNoError1 = plc2Status.IsSystemNormal;
                _systemNoError = systemNoError;

                // 更新系统状态显示
                UpdateUI(() =>
                {
                    string statusText = $"系统状态: {(systemNoError ? "正常" : "错误")}";
                    if (!plc1Status.IsConnected)
                    {
                        statusText += " (连接断开)";
                    }
                    
                    _systemStatusLabel.Text = statusText;
                    _systemStatusLabel.ForeColor = systemNoError ? Color.Green : Color.Red;
                    _systemStatusLabel.BackColor = systemNoError ? Color.FromArgb(220, 255, 220) : Color.FromArgb(255, 220, 220);
                });

                // 根据系统状态决定是否读取其他数据
                if (_opcService.CanSafelyReadPlcData("PLC1"))
                {
                    // 系统正常，读取流量数据
                    var flowData = await ReadFlowDataAsync();
                    
                    // 更新流量显示
                    UpdateUI(() =>
                    {
                        _flow1Label.Text = $"流量1: {flowData["Flow_1"]:F2} L/min";
                        _flow2Label.Text = $"流量2: {flowData["Flow_2"]:F2} L/min";
                        _flow3Label.Text = $"流量3: {flowData["Flow_3"]:F2} L/min";
                        
                        _flow1Label.ForeColor = Color.Blue;
                        _flow2Label.ForeColor = Color.Blue;
                        _flow3Label.ForeColor = Color.Blue;
                    });

                    _currentData.Clear();
                    _currentData["1214.PLC1._System._NoError"] = systemNoError;
                    _currentData["1214.PLC1.DisData.Flow.Flow_1"] = flowData["Flow_1"];
                    _currentData["1214.PLC1.DisData.Flow.Flow_2"] = flowData["Flow_2"];
                    _currentData["1214.PLC1.DisData.Flow.Flow_3"] = flowData["Flow_3"];
                }
                else
                {
                    // 系统错误或连接失败，不读取流量数据，显示为不可用
                    UpdateUI(() =>
                    {
                        string errorStatus = plc1Status.IsConnected ? "系统错误" : "连接断开";
                        _flow1Label.Text = $"流量1: {errorStatus}";
                        _flow2Label.Text = $"流量2: {errorStatus}";
                        _flow3Label.Text = $"流量3: {errorStatus}";
                        
                        _flow1Label.ForeColor = Color.Red;
                        _flow2Label.ForeColor = Color.Red;
                        _flow3Label.ForeColor = Color.Red;
                    });

                    LogMessage($"系统状态异常，跳过流量数据读取 - {plc1Status.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"读取数据异常: {ex.Message}");
                
                // 更新UI显示错误状态
                UpdateUI(() =>
                {
                    _systemStatusLabel.Text = "系统状态: 读取失败";
                    _systemStatusLabel.ForeColor = Color.Red;
                    _flow1Label.Text = "流量1: 读取失败";
                    _flow2Label.Text = "流量2: 读取失败";
                    _flow3Label.Text = "流量3: 读取失败";
                });
            }
        }

        /// <summary>
        /// 并行读取流量数据
        /// </summary>
        private async Task<Dictionary<string, float>> ReadFlowDataAsync()
        {
            var flowResults = new Dictionary<string, float>();
            
            try
            {
                // 并行读取所有流量数据
                var flow1Task = _opcService.ReadFloatAsync("1214.PLC1.DisData.Flow.Flow_1");
                var flow2Task = _opcService.ReadFloatAsync("1214.PLC1.DisData.Flow.Flow_2");
                var flow3Task = _opcService.ReadFloatAsync("1214.PLC1.DisData.Flow.Flow_3");
                
                // 等待所有读取任务完成
                await Task.WhenAll(flow1Task, flow2Task, flow3Task);
                
                flowResults["Flow_1"] = await flow1Task;
                flowResults["Flow_2"] = await flow2Task;
                flowResults["Flow_3"] = await flow3Task;
            }
            catch (Exception ex)
            {
                LogMessage($"读取流量数据失败: {ex.Message}");
                
                // 返回默认值
                flowResults["Flow_1"] = 0f;
                flowResults["Flow_2"] = 0f;
                flowResults["Flow_3"] = 0f;
            }
            
            return flowResults;
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 线程安全的UI更新
        /// </summary>
        private void UpdateUI(Action action)
        {
            // 如果窗体还未初始化或已释放，不执行UI操作
            if (this.IsDisposed || !this.IsHandleCreated)
                return;
                
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(action);
                }
                catch (ObjectDisposedException)
                {
                    // 窗体已释放，忽略操作
                }
                catch (InvalidOperationException)
                {
                    // 窗体句柄未创建，忽略操作
                }
            }
            else
            {
                try
                {
                    action();
                }
                catch (ObjectDisposedException)
                {
                    // 控件已释放，忽略操作
                }
            }
        }

        /// <summary>
        /// 记录日志消息
        /// </summary>
        private void LogMessage(string message)
        {
            // 如果控件还未初始化，使用控制台输出
            if (_logTextBox == null)
            {
                string timeStamp = DateTime.Now.ToString("HH:mm:ss.fff");
                string logEntry = $"[{timeStamp}] {message}";
                System.Diagnostics.Debug.WriteLine(logEntry);
                Console.WriteLine(logEntry);
                return;
            }
            
            UpdateUI(() =>
            {
                string timeStamp = DateTime.Now.ToString("HH:mm:ss.fff");
                string logEntry = $"[{timeStamp}] {message}";
                
                _logTextBox.AppendText(logEntry + Environment.NewLine);
                _logTextBox.SelectionStart = _logTextBox.Text.Length;
                _logTextBox.ScrollToCaret();
                
                // 限制日志长度，避免内存占用过大
                if (_logTextBox.Lines.Length > 100)
                {
                    var lines = _logTextBox.Lines.Skip(20).ToArray();
                    _logTextBox.Lines = lines;
                }
            });
        }

        /// <summary>
        /// 获取当前读取的数据
        /// </summary>
        public Dictionary<string, object> GetCurrentData()
        {
            return new Dictionary<string, object>(_currentData);
        }

        /// <summary>
        /// 获取系统状态
        /// </summary>
        public bool IsSystemNoError()
        {
            return _systemNoError;
        }
        #endregion

        #region 资源清理
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                
                if (_opcService != null)
                {
                    _opcService.ConnectionStatusChanged -= OnConnectionStatusChanged;
                    _opcService.PlcStatusChanged -= OnPlcStatusChanged;
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        private void Form2_Load(object sender, EventArgs e)
        {
            // 窗体加载时的初始化操作
            LogMessage("窗体加载完成");
         
              
                plc1Status = new PlcStatusModel("PLC1", "主控PLC", "1214.PLC1._System._NoError", "主控制系统PLC");
                _opcService.AddOrUpdatePlc(plc1Status);
          
         
              
                plc2Status = new PlcStatusModel("PLC2", "主控PLC2", "1214.PLC2._System._NoError", "主控制系统PLC");
                _opcService.AddOrUpdatePlc(plc2Status);
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.ShowDialog();
        }
    }
}