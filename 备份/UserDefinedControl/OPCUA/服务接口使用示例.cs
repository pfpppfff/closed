using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserDefinedControl.OPCUA;

namespace UserDefinedControl.Examples
{
    /// <summary>
    /// OPC UA 服务接口使用示例
    /// 展示如何使用接口化的服务架构
    /// </summary>
    public partial class ServiceExampleForm : Form
    {
        #region 字段
        private IOpcUaService _opcService;
        private Timer _statusTimer;
        private ComboBox _serviceTypeCombo;
        private Label _serviceStatusLabel;
        private Button _switchServiceButton;
        private TextBox _serverUrlTextBox;
        private Button _connectButton;
        private Button _disconnectButton;
        private GroupBox _testGroup;
        private Button _testReadButton;
        private Button _testWriteButton;
        private TextBox _nodeIdTextBox;
        private TextBox _valueTextBox;
        private Label _resultLabel;
        #endregion

        #region 构造函数
        public ServiceExampleForm()
        {
            InitializeComponent();
            InitializeService();
            StartStatusTimer();
        }

        private void InitializeComponent()
        {
            // 采用简洁美观的设计风格 - 扁平化设计、纯白背景、深灰文字、浅灰边框
            this.Text = "OPC UA 服务接口示例";
            this.Size = new Size(650, 500);
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // 服务类型选择区域
            var serviceGroup = new GroupBox
            {
                Text = "服务类型选择",
                Location = new Point(10, 10),
                Size = new Size(610, 80),
                ForeColor = Color.DarkGray,
                FlatStyle = FlatStyle.Flat
            };

            var typeLabel = new Label
            {
                Text = "服务类型:",
                Location = new Point(10, 25),
                Size = new Size(80, 23),
                ForeColor = Color.DarkGray
            };

            _serviceTypeCombo = new ComboBox
            {
                Location = new Point(100, 25),
                Size = new Size(120, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            _serviceTypeCombo.Items.AddRange(new[] { "真实服务", "模拟服务" });
            _serviceTypeCombo.SelectedIndex = 0;

            _serviceStatusLabel = new Label
            {
                Text = "当前: 真实服务",
                Location = new Point(240, 25),
                Size = new Size(150, 23),
                ForeColor = Color.Green
            };

            _switchServiceButton = new Button
            {
                Text = "切换服务",
                Location = new Point(400, 25),
                Size = new Size(80, 23),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };

            serviceGroup.Controls.AddRange(new Control[] {
                typeLabel, _serviceTypeCombo, _serviceStatusLabel, _switchServiceButton
            });

            // 连接管理区域
            var connectionGroup = new GroupBox
            {
                Text = "连接管理",
                Location = new Point(10, 100),
                Size = new Size(610, 80),
                ForeColor = Color.DarkGray,
                FlatStyle = FlatStyle.Flat
            };

            var urlLabel = new Label
            {
                Text = "服务器地址:",
                Location = new Point(10, 25),
                Size = new Size(80, 23),
                ForeColor = Color.DarkGray
            };

            _serverUrlTextBox = new TextBox
            {
                Text = "opc.tcp://127.0.0.1:49320",
                Location = new Point(100, 25),
                Size = new Size(300, 23),
                BorderStyle = BorderStyle.FixedSingle
            };

            _connectButton = new Button
            {
                Text = "连接",
                Location = new Point(410, 25),
                Size = new Size(80, 23),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };

            _disconnectButton = new Button
            {
                Text = "断开",
                Location = new Point(500, 25),
                Size = new Size(80, 23),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };

            connectionGroup.Controls.AddRange(new Control[] {
                urlLabel, _serverUrlTextBox, _connectButton, _disconnectButton
            });

            // 测试操作区域
            _testGroup = new GroupBox
            {
                Text = "测试操作",
                Location = new Point(10, 190),
                Size = new Size(610, 160),
                ForeColor = Color.DarkGray,
                FlatStyle = FlatStyle.Flat
            };

            var nodeLabel = new Label
            {
                Text = "节点地址:",
                Location = new Point(10, 25),
                Size = new Size(80, 23),
                ForeColor = Color.DarkGray
            };

            _nodeIdTextBox = new TextBox
            {
                Text = "1214.PLC1.System.Running",
                Location = new Point(100, 25),
                Size = new Size(300, 23),
                BorderStyle = BorderStyle.FixedSingle
            };

            var valueLabel = new Label
            {
                Text = "写入值:",
                Location = new Point(10, 55),
                Size = new Size(60, 23),
                ForeColor = Color.DarkGray
            };

            _valueTextBox = new TextBox
            {
                Text = "true",
                Location = new Point(100, 55),
                Size = new Size(100, 23),
                BorderStyle = BorderStyle.FixedSingle
            };

            _testReadButton = new Button
            {
                Text = "读取测试",
                Location = new Point(220, 55),
                Size = new Size(80, 23),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };

            _testWriteButton = new Button
            {
                Text = "写入测试",
                Location = new Point(310, 55),
                Size = new Size(80, 23),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };

            _resultLabel = new Label
            {
                Text = "结果: 等待操作...",
                Location = new Point(10, 85),
                Size = new Size(580, 60),
                ForeColor = Color.DarkGray,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke
            };

            _testGroup.Controls.AddRange(new Control[] {
                nodeLabel, _nodeIdTextBox, valueLabel, _valueTextBox,
                _testReadButton, _testWriteButton, _resultLabel
            });

            // 添加到窗体
            this.Controls.AddRange(new Control[] { serviceGroup, connectionGroup, _testGroup });

            // 绑定事件
            _switchServiceButton.Click += SwitchServiceButton_Click;
            _connectButton.Click += ConnectButton_Click;
            _disconnectButton.Click += DisconnectButton_Click;
            _testReadButton.Click += TestReadButton_Click;
            _testWriteButton.Click += TestWriteButton_Click;
        }

        private void InitializeService()
        {
            // 默认使用真实服务
            _opcService = OpcUaServiceFactory.CreateRealService();
            _opcService.ConnectionStatusChanged += OnConnectionStatusChanged;
        }

        private void StartStatusTimer()
        {
            _statusTimer = new Timer();
            _statusTimer.Interval = 1000;
            _statusTimer.Tick += StatusTimer_Tick;
            _statusTimer.Start();
        }
        #endregion

        #region 事件处理
        private void SwitchServiceButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 取消注册旧服务的事件
                if (_opcService != null)
                {
                    _opcService.ConnectionStatusChanged -= OnConnectionStatusChanged;
                }

                // 创建新服务
                var isRealService = _serviceTypeCombo.SelectedIndex == 0;
                var serviceType = isRealService ? 
                    OpcUaServiceFactory.ServiceType.Real : 
                    OpcUaServiceFactory.ServiceType.Mock;

                _opcService = OpcUaServiceFactory.CreateService(serviceType, _serverUrlTextBox.Text);
                _opcService.ConnectionStatusChanged += OnConnectionStatusChanged;

                // 更新状态显示
                _serviceStatusLabel.Text = $"当前: {(isRealService ? "真实服务" : "模拟服务")}";
                _serviceStatusLabel.ForeColor = isRealService ? Color.Green : Color.Blue;

                // 设置到服务管理器（如果需要全局使用）
                OpcUaServiceManager.SetService(_opcService);

                _resultLabel.Text = $"结果: 已切换到{(isRealService ? "真实" : "模拟")}服务";
                _resultLabel.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                _resultLabel.Text = $"结果: 切换服务失败 - {ex.Message}";
                _resultLabel.ForeColor = Color.Red;
            }
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                string serverUrl = _serverUrlTextBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(serverUrl))
                {
                    MessageBox.Show("请输入服务器地址！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _opcService.Connect(serverUrl);
                _resultLabel.Text = "结果: 连接成功";
                _resultLabel.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                _resultLabel.Text = $"结果: 连接失败 - {ex.Message}";
                _resultLabel.ForeColor = Color.Red;
            }
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            _opcService.Disconnect();
            _resultLabel.Text = "结果: 已断开连接";
            _resultLabel.ForeColor = Color.Orange;
        }

        private async void TestReadButton_Click(object sender, EventArgs e)
        {
            string nodeId = _nodeIdTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(nodeId))
            {
                MessageBox.Show("请输入节点地址！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 尝试读取不同类型的数据
                if (nodeId.Contains("temp") || nodeId.Contains("flow") || nodeId.Contains("SetPoint"))
                {
                    float value = await _opcService.ReadFloatAsync(nodeId);
                    _resultLabel.Text = $"结果: 读取浮点数 {nodeId} = {value}";
                }
                else if (nodeId.Contains("Running") || nodeId.Contains("System"))
                {
                    bool value = await _opcService.ReadBoolAsync(nodeId);
                    _resultLabel.Text = $"结果: 读取布尔值 {nodeId} = {value}";
                }
                else if (nodeId.Contains("Mode") || nodeId.Contains("Code"))
                {
                    int value = await _opcService.ReadInt32Async(nodeId);
                    _resultLabel.Text = $"结果: 读取整数 {nodeId} = {value}";
                }
                else
                {
                    string value = await _opcService.ReadStringAsync(nodeId);
                    _resultLabel.Text = $"结果: 读取字符串 {nodeId} = {value}";
                }
                
                _resultLabel.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                _resultLabel.Text = $"结果: 读取失败 - {ex.Message}";
                _resultLabel.ForeColor = Color.Red;
            }
        }

        private async void TestWriteButton_Click(object sender, EventArgs e)
        {
            string nodeId = _nodeIdTextBox.Text.Trim();
            string valueText = _valueTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(nodeId) || string.IsNullOrWhiteSpace(valueText))
            {
                MessageBox.Show("请输入节点地址和值！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool success = false;

                // 根据值的格式判断数据类型
                if (bool.TryParse(valueText, out bool boolValue))
                {
                    success = await _opcService.WriteBoolAsync(nodeId, boolValue);
                    _resultLabel.Text = $"结果: 写入布尔值 {nodeId} = {boolValue} {(success ? "成功" : "失败")}";
                }
                else if (float.TryParse(valueText, out float floatValue))
                {
                    success = await _opcService.WriteFloatAsync(nodeId, floatValue);
                    _resultLabel.Text = $"结果: 写入浮点数 {nodeId} = {floatValue} {(success ? "成功" : "失败")}";
                }
                else if (int.TryParse(valueText, out int intValue))
                {
                    success = await _opcService.WriteInt32Async(nodeId, intValue);
                    _resultLabel.Text = $"结果: 写入整数 {nodeId} = {intValue} {(success ? "成功" : "失败")}";
                }
                else
                {
                    success = await _opcService.WriteStringAsync(nodeId, valueText);
                    _resultLabel.Text = $"结果: 写入字符串 {nodeId} = {valueText} {(success ? "成功" : "失败")}";
                }

                _resultLabel.ForeColor = success ? Color.Green : Color.Red;
            }
            catch (Exception ex)
            {
                _resultLabel.Text = $"结果: 写入失败 - {ex.Message}";
                _resultLabel.ForeColor = Color.Red;
            }
        }

        private void OnConnectionStatusChanged(object sender, bool isConnected)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<object, bool>(OnConnectionStatusChanged), sender, isConnected);
                return;
            }

            _connectButton.Enabled = !isConnected;
            _disconnectButton.Enabled = isConnected;
            _testGroup.Enabled = isConnected;

            var connectionStatus = isConnected ? "已连接" : "未连接";
            this.Text = $"OPC UA 服务接口示例 - {connectionStatus}";
        }

        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            // 定期更新连接状态
            if (_opcService != null)
            {
                bool isConnected = _opcService.IsConnected;
                if (_connectButton.Enabled == isConnected) // 状态不一致时更新
                {
                    OnConnectionStatusChanged(this, isConnected);
                }
            }
        }
        #endregion

        #region 资源清理
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_opcService != null)
            {
                _opcService.ConnectionStatusChanged -= OnConnectionStatusChanged;
            }

            _statusTimer?.Stop();
            _statusTimer?.Dispose();

            base.OnFormClosed(e);
        }
        #endregion
    }

    /// <summary>
    /// 带依赖注入的用户控件示例
    /// 展示如何在控件中使用服务接口
    /// </summary>
    public partial class ServiceBasedControl : UserControl
    {
        #region 属性
        private IOpcUaService _opcService;
        
        /// <summary>
        /// OPC UA 服务实例
        /// </summary>
        public IOpcUaService OpcService 
        { 
            get => _opcService ?? OpcUaServiceManager.Current;
            set => _opcService = value;
        }

        /// <summary>
        /// 节点地址
        /// </summary>
        public string NodeId { get; set; } = "1214.PLC1.System.Running";
        #endregion

        #region 字段
        private Timer _updateTimer;
        private Label _statusLabel;
        private Button _actionButton;
        private object _currentValue;
        #endregion

        #region 构造函数
        public ServiceBasedControl() : this(null)
        {
        }

        public ServiceBasedControl(IOpcUaService opcService)
        {
            _opcService = opcService;
            InitializeControl();
            StartUpdateTimer();
        }

        private void InitializeControl()
        {
            // 简洁美观的设计
            this.Size = new Size(250, 80);
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;

            _statusLabel = new Label
            {
                Text = "状态: 等待连接...",
                Location = new Point(10, 10),
                Size = new Size(230, 30),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.DarkGray,
                BorderStyle = BorderStyle.None
            };

            _actionButton = new Button
            {
                Text = "操作",
                Location = new Point(10, 45),
                Size = new Size(230, 25),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };

            _actionButton.Click += ActionButton_Click;

            this.Controls.AddRange(new Control[] { _statusLabel, _actionButton });
        }

        private void StartUpdateTimer()
        {
            _updateTimer = new Timer();
            _updateTimer.Interval = 500;
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }
        #endregion

        #region 事件处理
        private async void ActionButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NodeId) || !OpcService.IsConnected)
                return;

            try
            {
                // 如果当前值是布尔类型，执行切换操作
                if (_currentValue is bool)
                {
                    bool success = await OpcService.ToggleBoolAsync(NodeId);
                    if (!success)
                    {
                        _statusLabel.Text = "状态: 操作失败";
                        _statusLabel.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"状态: 错误 - {ex.Message}";
                _statusLabel.ForeColor = Color.Red;
            }
        }

        private async void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NodeId) || !OpcService.IsConnected)
            {
                _statusLabel.Text = "状态: 未连接";
                _statusLabel.ForeColor = Color.Red;
                _actionButton.Enabled = false;
                return;
            }

            try
            {
                // 根据节点类型读取不同的数据
                if (NodeId.Contains("Running") || NodeId.Contains("System"))
                {
                    bool value = await OpcService.ReadBoolAsync(NodeId);
                    _currentValue = value;
                    _statusLabel.Text = $"状态: {(value ? "运行中" : "停止")}";
                    _statusLabel.ForeColor = value ? Color.Green : Color.Red;
                    _actionButton.Text = value ? "停止" : "启动";
                    _actionButton.BackColor = value ? Color.LightCoral : Color.LightGreen;
                }
                else if (NodeId.Contains("temp") || NodeId.Contains("flow"))
                {
                    float value = await OpcService.ReadFloatAsync(NodeId);
                    _currentValue = value;
                    _statusLabel.Text = $"值: {value:F2}";
                    _statusLabel.ForeColor = Color.Green;
                    _actionButton.Text = "数值显示";
                    _actionButton.BackColor = Color.LightBlue;
                }

                _actionButton.Enabled = true;
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"状态: 读取错误";
                _statusLabel.ForeColor = Color.Red;
                _actionButton.Enabled = false;
            }
        }
        #endregion

        #region 资源清理
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _updateTimer?.Stop();
                _updateTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}