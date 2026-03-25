using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserDefinedControl.OPCUA;

namespace UserDefinedControl.Examples
{
    /// <summary>
    /// OPC UA 全局单例服务使用示例
    /// 展示如何在实际项目中使用新的通讯服务
    /// </summary>
    public partial class OpcUaExampleForm : Form
    {
        #region 字段和属性
        private Timer _statusTimer;
        private Label _connectionStatusLabel;
        private Button _connectButton;
        private Button _disconnectButton;
        private TextBox _serverUrlTextBox;
        private Button _readBoolButton;
        private Button _writeBoolButton;
        private Button _toggleBoolButton;
        private TextBox _boolNodeIdTextBox;
        private Label _boolValueLabel;
        private TextBox _floatNodeIdTextBox;
        private TextBox _floatValueTextBox;
        private Button _readFloatButton;
        private Button _writeFloatButton;
        private Label _floatDisplayLabel;
        #endregion

        #region 构造函数和初始化
        public OpcUaExampleForm()
        {
            InitializeComponent();
            InitializeOpcUaEvents();
            StartStatusTimer();
        }

        private void InitializeComponent()
        {
            // 采用用户偏好的简洁美观设计风格
            this.Text = "OPC UA 全局服务示例";
            this.Size = new Size(600, 500);
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // 连接状态区域
            var connectionGroup = new GroupBox
            {
                Text = "连接管理",
                Location = new Point(10, 10),
                Size = new Size(560, 100),
                ForeColor = Color.DarkGray
            };

            _connectionStatusLabel = new Label
            {
                Text = "状态: 未连接",
                Location = new Point(10, 20),
                Size = new Size(200, 23),
                ForeColor = Color.Red
            };

            _serverUrlTextBox = new TextBox
            {
                Text = "opc.tcp://127.0.0.1:49320",
                Location = new Point(10, 50),
                Size = new Size(300, 23),
                BorderStyle = BorderStyle.FixedSingle
            };

            _connectButton = new Button
            {
                Text = "连接",
                Location = new Point(320, 50),
                Size = new Size(80, 23),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };

            _disconnectButton = new Button
            {
                Text = "断开",
                Location = new Point(410, 50),
                Size = new Size(80, 23),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };

            connectionGroup.Controls.AddRange(new Control[] {
                _connectionStatusLabel, _serverUrlTextBox, _connectButton, _disconnectButton
            });

            // 布尔值操作区域
            var boolGroup = new GroupBox
            {
                Text = "布尔值操作",
                Location = new Point(10, 120),
                Size = new Size(560, 120),
                ForeColor = Color.DarkGray
            };

            var boolNodeLabel = new Label
            {
                Text = "节点地址:",
                Location = new Point(10, 25),
                Size = new Size(80, 23)
            };

            _boolNodeIdTextBox = new TextBox
            {
                Text = "1214.PLC1.System.Running",
                Location = new Point(100, 25),
                Size = new Size(300, 23),
                BorderStyle = BorderStyle.FixedSingle
            };

            _readBoolButton = new Button
            {
                Text = "读取",
                Location = new Point(10, 55),
                Size = new Size(80, 23),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };

            _writeBoolButton = new Button
            {
                Text = "写入True",
                Location = new Point(100, 55),
                Size = new Size(80, 23),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };

            _toggleBoolButton = new Button
            {
                Text = "切换",
                Location = new Point(190, 55),
                Size = new Size(80, 23),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };

            _boolValueLabel = new Label
            {
                Text = "值: --",
                Location = new Point(300, 55),
                Size = new Size(100, 23),
                ForeColor = Color.DarkGray
            };

            boolGroup.Controls.AddRange(new Control[] {
                boolNodeLabel, _boolNodeIdTextBox, _readBoolButton, 
                _writeBoolButton, _toggleBoolButton, _boolValueLabel
            });

            // 浮点数操作区域
            var floatGroup = new GroupBox
            {
                Text = "浮点数操作",
                Location = new Point(10, 250),
                Size = new Size(560, 120),
                ForeColor = Color.DarkGray
            };

            var floatNodeLabel = new Label
            {
                Text = "节点地址:",
                Location = new Point(10, 25),
                Size = new Size(80, 23)
            };

            _floatNodeIdTextBox = new TextBox
            {
                Text = "1214.PLC1.DisData.Temp.temp_1",
                Location = new Point(100, 25),
                Size = new Size(300, 23),
                BorderStyle = BorderStyle.FixedSingle
            };

            var floatValueLabel = new Label
            {
                Text = "写入值:",
                Location = new Point(10, 55),
                Size = new Size(60, 23)
            };

            _floatValueTextBox = new TextBox
            {
                Text = "25.5",
                Location = new Point(80, 55),
                Size = new Size(100, 23),
                BorderStyle = BorderStyle.FixedSingle
            };

            _readFloatButton = new Button
            {
                Text = "读取",
                Location = new Point(200, 55),
                Size = new Size(80, 23),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };

            _writeFloatButton = new Button
            {
                Text = "写入",
                Location = new Point(290, 55),
                Size = new Size(80, 23),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };

            _floatDisplayLabel = new Label
            {
                Text = "当前值: --",
                Location = new Point(10, 85),
                Size = new Size(200, 23),
                ForeColor = Color.DarkGray
            };

            floatGroup.Controls.AddRange(new Control[] {
                floatNodeLabel, _floatNodeIdTextBox, floatValueLabel,
                _floatValueTextBox, _readFloatButton, _writeFloatButton, _floatDisplayLabel
            });

            // 添加所有组到窗体
            this.Controls.AddRange(new Control[] { connectionGroup, boolGroup, floatGroup });

            // 绑定事件
            _connectButton.Click += ConnectButton_Click;
            _disconnectButton.Click += DisconnectButton_Click;
            _readBoolButton.Click += ReadBoolButton_Click;
            _writeBoolButton.Click += WriteBoolButton_Click;
            _toggleBoolButton.Click += ToggleBoolButton_Click;
            _readFloatButton.Click += ReadFloatButton_Click;
            _writeFloatButton.Click += WriteFloatButton_Click;
        }

        private void InitializeOpcUaEvents()
        {
            // 注册连接状态变化事件
            OpcUaHelper.RegisterConnectionStatusChanged(OnConnectionStatusChanged);
        }

        private void StartStatusTimer()
        {
            _statusTimer = new Timer();
            _statusTimer.Interval = 1000; // 每秒更新一次状态
            _statusTimer.Tick += StatusTimer_Tick;
            _statusTimer.Start();
        }
        #endregion

        #region 事件处理
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

                OpcUaHelper.Reconnect(serverUrl);
                MessageBox.Show("连接成功！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"连接失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            OpcUaHelper.Disconnect();
            MessageBox.Show("已断开连接！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ReadBoolButton_Click(object sender, EventArgs e)
        {
            string nodeId = _boolNodeIdTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(nodeId))
            {
                MessageBox.Show("请输入节点地址！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool value = OpcUaHelper.ReadBool(nodeId);
            _boolValueLabel.Text = $"值: {value}";
            _boolValueLabel.ForeColor = value ? Color.Green : Color.Red;
        }

        private async void WriteBoolButton_Click(object sender, EventArgs e)
        {
            string nodeId = _boolNodeIdTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(nodeId))
            {
                MessageBox.Show("请输入节点地址！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = await OpcUaHelper.WriteBoolAsync(nodeId, true);
            if (success)
            {
                MessageBox.Show("写入成功！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReadBoolButton_Click(sender, e); // 自动刷新显示
            }
            else
            {
                MessageBox.Show("写入失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ToggleBoolButton_Click(object sender, EventArgs e)
        {
            string nodeId = _boolNodeIdTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(nodeId))
            {
                MessageBox.Show("请输入节点地址！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = await OpcUaHelper.ToggleBoolAsync(nodeId);
            if (success)
            {
                MessageBox.Show("切换成功！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReadBoolButton_Click(sender, e); // 自动刷新显示
            }
            else
            {
                MessageBox.Show("切换失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ReadFloatButton_Click(object sender, EventArgs e)
        {
            string nodeId = _floatNodeIdTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(nodeId))
            {
                MessageBox.Show("请输入节点地址！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            float value = await OpcUaHelper.ReadFloatAsync(nodeId);
            _floatDisplayLabel.Text = $"当前值: {value:F2}";
            _floatDisplayLabel.ForeColor = Color.Green;
        }

        private async void WriteFloatButton_Click(object sender, EventArgs e)
        {
            string nodeId = _floatNodeIdTextBox.Text.Trim();
            string valueText = _floatValueTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(nodeId))
            {
                MessageBox.Show("请输入节点地址！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!OpcUaHelper.IsValidNumber(valueText))
            {
                MessageBox.Show("请输入有效的数字！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            float value = Convert.ToSingle(valueText);
            bool success = await OpcUaHelper.WriteFloatAsync(nodeId, value);
            
            if (success)
            {
                MessageBox.Show("写入成功！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReadFloatButton_Click(sender, e); // 自动刷新显示
            }
            else
            {
                MessageBox.Show("写入失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnConnectionStatusChanged(object sender, bool isConnected)
        {
            // 在UI线程中更新状态显示
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<object, bool>(OnConnectionStatusChanged), sender, isConnected);
                return;
            }

            _connectionStatusLabel.Text = $"状态: {(isConnected ? "已连接" : "未连接")}";
            _connectionStatusLabel.ForeColor = isConnected ? Color.Green : Color.Red;
            
            // 根据连接状态启用/禁用按钮
            _connectButton.Enabled = !isConnected;
            _disconnectButton.Enabled = isConnected;
        }

        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            // 定期更新连接状态显示
            bool isConnected = OpcUaHelper.IsConnected;
            if (_connectionStatusLabel.Text != $"状态: {(isConnected ? "已连接" : "未连接")}")
            {
                OnConnectionStatusChanged(this, isConnected);
            }
        }
        #endregion

        #region 资源清理
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // 取消注册事件
            OpcUaHelper.UnregisterConnectionStatusChanged(OnConnectionStatusChanged);
            
            // 停止定时器
            _statusTimer?.Stop();
            _statusTimer?.Dispose();
            
            base.OnFormClosed(e);
        }
        #endregion
    }

    /// <summary>
    /// 简单的用户控件示例
    /// 展示如何在自定义控件中使用 OPC UA 服务
    /// </summary>
    public partial class OpcUaBooleanControl : UserControl
    {
        #region 属性
        /// <summary>
        /// OPC UA 节点地址
        /// </summary>
        public string NodeId { get; set; } = "1214.PLC1.System.Running";

        /// <summary>
        /// 当前布尔值
        /// </summary>
        public bool CurrentValue { get; private set; }
        #endregion

        #region 字段
        private Timer _updateTimer;
        private Button _toggleButton;
        private Label _statusLabel;
        #endregion

        #region 构造函数
        public OpcUaBooleanControl()
        {
            InitializeControl();
            StartUpdateTimer();
        }

        private void InitializeControl()
        {
            // 采用简洁美观的设计风格
            this.Size = new Size(200, 60);
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;

            _statusLabel = new Label
            {
                Text = "状态: --",
                Location = new Point(10, 10),
                Size = new Size(180, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.DarkGray
            };

            _toggleButton = new Button
            {
                Text = "切换",
                Location = new Point(10, 35),
                Size = new Size(180, 20),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };

            _toggleButton.Click += ToggleButton_Click;

            this.Controls.AddRange(new Control[] { _statusLabel, _toggleButton });
        }

        private void StartUpdateTimer()
        {
            _updateTimer = new Timer();
            _updateTimer.Interval = 500; // 每500ms更新一次
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }
        #endregion

        #region 事件处理
        private async void ToggleButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NodeId))
            {
                MessageBox.Show("未设置节点地址！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = await OpcUaHelper.ToggleBoolAsync(NodeId);
            if (!success)
            {
                MessageBox.Show("操作失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NodeId) || !OpcUaHelper.IsConnected)
            {
                _statusLabel.Text = "状态: 未连接";
                _statusLabel.ForeColor = Color.Red;
                CurrentValue = false;
                return;
            }

            try
            {
                bool newValue = await OpcUaHelper.ReadBoolAsync(NodeId);
                if (newValue != CurrentValue)
                {
                    CurrentValue = newValue;
                    _statusLabel.Text = $"状态: {(CurrentValue ? "开启" : "关闭")}";
                    _statusLabel.ForeColor = CurrentValue ? Color.Green : Color.Red;
                    
                    // 更新按钮背景色
                    _toggleButton.BackColor = CurrentValue ? Color.LightGreen : Color.LightGray;
                }
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"状态: 错误 - {ex.Message}";
                _statusLabel.ForeColor = Color.Red;
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