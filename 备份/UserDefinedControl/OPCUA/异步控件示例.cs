using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserDefinedControl.OPCUA;

namespace UserDefinedControl.Examples
{
    /// <summary>
    /// 异步优化的用户控件示例
    /// 采用异步读写避免UI阻塞，符合工业自动化多控件需求
    /// </summary>
    public partial class AsyncOptimizedControl : UserControl
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

        /// <summary>
        /// 数据刷新间隔（毫秒）
        /// </summary>
        public int RefreshInterval { get; set; } = 500;

        /// <summary>
        /// 当前值
        /// </summary>
        public object CurrentValue { get; private set; }
        #endregion

        #region 字段
        private CancellationTokenSource _cancellationTokenSource;
        private Label _statusLabel;
        private Button _actionButton;
        private ProgressBar _progressBar;
        private bool _isUpdating = false;
        #endregion

        #region 构造函数
        public AsyncOptimizedControl() : this(null)
        {
        }

        public AsyncOptimizedControl(IOpcUaService opcService)
        {
            _opcService = opcService;
            InitializeControl();
            StartAsyncUpdates();
        }

        private void InitializeControl()
        {
            // 采用简洁美观的设计风格 - 扁平化设计、纯白背景、深灰文字、浅灰边框
            this.Size = new Size(280, 100);
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;

            _statusLabel = new Label
            {
                Text = "状态: 初始化中...",
                Location = new Point(10, 10),
                Size = new Size(260, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.DarkGray,
                Font = new Font("微软雅黑", 9F, FontStyle.Regular)
            };

            _progressBar = new ProgressBar
            {
                Location = new Point(10, 40),
                Size = new Size(260, 8),
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Visible = false
            };

            _actionButton = new Button
            {
                Text = "执行操作",
                Location = new Point(10, 60),
                Size = new Size(260, 30),
                BackColor = Color.FromArgb(240, 240, 240),
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false,
                Font = new Font("微软雅黑", 9F, FontStyle.Regular),
                ForeColor = Color.DarkGray
            };
            _actionButton.FlatAppearance.BorderColor = Color.LightGray;
            _actionButton.FlatAppearance.BorderSize = 1;

            _actionButton.Click += ActionButton_ClickAsync;

            this.Controls.AddRange(new Control[] { _statusLabel, _progressBar, _actionButton });
        }

        private void StartAsyncUpdates()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            
            // 启动异步数据监控
            _ = Task.Run(async () =>
            {
                await AsyncOpcUaHelper.StartPeriodicReadAsync(
                    NodeId, 
                    RefreshInterval, 
                    OnDataReceived, 
                    _cancellationTokenSource.Token
                );
            });
        }
        #endregion

        #region 异步事件处理
        private async void ActionButton_ClickAsync(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NodeId) || !OpcService.IsConnected || _isUpdating)
                return;

            await ExecuteActionAsync();
        }

        private async Task ExecuteActionAsync()
        {
            _isUpdating = true;
            
            // 显示进度指示
            UpdateUI(() =>
            {
                _progressBar.Visible = true;
                _actionButton.Enabled = false;
                _statusLabel.Text = "状态: 执行操作中...";
                _statusLabel.ForeColor = Color.Orange;
            });

            try
            {
                // 根据当前值类型执行不同操作
                if (CurrentValue is bool boolValue)
                {
                    bool success = await AsyncOpcUaHelper.ToggleBoolAsync(NodeId);
                    UpdateUI(() =>
                    {
                        _statusLabel.Text = success ? "状态: 布尔值切换成功" : "状态: 布尔值切换失败";
                        _statusLabel.ForeColor = success ? Color.Green : Color.Red;
                    });
                }
                else if (CurrentValue is float floatValue)
                {
                    // 示例：增加10%的值
                    float newValue = floatValue * 1.1f;
                    bool success = await AsyncOpcUaHelper.WriteFloatWithRangeAsync(NodeId, newValue, 0f, 100f);
                    UpdateUI(() =>
                    {
                        _statusLabel.Text = success ? $"状态: 数值设置为 {newValue:F1}" : "状态: 数值设置失败";
                        _statusLabel.ForeColor = success ? Color.Green : Color.Red;
                    });
                }
                else
                {
                    // 对于其他类型，执行一个简单的读取操作
                    var value = await AsyncOpcUaHelper.ReadStringAsync(NodeId);
                    UpdateUI(() =>
                    {
                        _statusLabel.Text = $"状态: 读取到 '{value}'";
                        _statusLabel.ForeColor = Color.Blue;
                    });
                }

                // 模拟操作延迟
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                UpdateUI(() =>
                {
                    _statusLabel.Text = $"状态: 操作异常 - {ex.Message}";
                    _statusLabel.ForeColor = Color.Red;
                });
            }
            finally
            {
                _isUpdating = false;
                
                // 隐藏进度指示
                UpdateUI(() =>
                {
                    _progressBar.Visible = false;
                    _actionButton.Enabled = true;
                });
            }
        }

        private void OnDataReceived(object value)
        {
            CurrentValue = value;
            
            UpdateUI(() =>
            {
                if (!_isUpdating) // 避免在操作过程中覆盖状态
                {
                    if (value is bool boolValue)
                    {
                        _statusLabel.Text = $"状态: {(boolValue ? "开启" : "关闭")}";
                        _statusLabel.ForeColor = boolValue ? Color.Green : Color.Red;
                        _actionButton.Text = boolValue ? "关闭" : "开启";
                        _actionButton.BackColor = boolValue ? Color.FromArgb(255, 200, 200) : Color.FromArgb(200, 255, 200);
                    }
                    else if (value is float floatValue)
                    {
                        _statusLabel.Text = $"状态: {floatValue:F2}";
                        _statusLabel.ForeColor = Color.Blue;
                        _actionButton.Text = "增加10%";
                        _actionButton.BackColor = Color.FromArgb(200, 230, 255);
                    }
                    else if (value is int intValue)
                    {
                        _statusLabel.Text = $"状态: {intValue}";
                        _statusLabel.ForeColor = Color.Purple;
                        _actionButton.Text = "读取数据";
                        _actionButton.BackColor = Color.FromArgb(230, 200, 255);
                    }
                    else
                    {
                        _statusLabel.Text = $"状态: {value}";
                        _statusLabel.ForeColor = Color.DarkGray;
                        _actionButton.Text = "刷新数据";
                        _actionButton.BackColor = Color.FromArgb(240, 240, 240);
                    }
                }
            });
        }

        /// <summary>
        /// 线程安全的UI更新
        /// </summary>
        private void UpdateUI(Action action)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 异步设置新的节点地址
        /// </summary>
        /// <param name="nodeId">新的节点地址</param>
        public async Task SetNodeIdAsync(string nodeId)
        {
            if (string.IsNullOrWhiteSpace(nodeId))
                return;

            NodeId = nodeId;
            
            // 重启数据监控
            _cancellationTokenSource?.Cancel();
            await Task.Delay(100); // 等待当前监控结束
            
            StartAsyncUpdates();
            
            UpdateUI(() =>
            {
                _statusLabel.Text = $"状态: 切换到节点 {nodeId}";
                _statusLabel.ForeColor = Color.Blue;
            });
        }

        /// <summary>
        /// 手动刷新数据
        /// </summary>
        public async Task RefreshDataAsync()
        {
            if (string.IsNullOrWhiteSpace(NodeId) || !OpcService.IsConnected)
                return;

            try
            {
                // 根据节点名称推断数据类型
                if (NodeId.Contains("temp") || NodeId.Contains("flow") || NodeId.Contains("SetPoint"))
                {
                    var value = await AsyncOpcUaHelper.ReadFloatAsync(NodeId);
                    OnDataReceived(value);
                }
                else if (NodeId.Contains("Running") || NodeId.Contains("System"))
                {
                    var value = await AsyncOpcUaHelper.ReadBoolAsync(NodeId);
                    OnDataReceived(value);
                }
                else if (NodeId.Contains("Code") || NodeId.Contains("Mode"))
                {
                    var value = await AsyncOpcUaHelper.ReadInt32Async(NodeId);
                    OnDataReceived(value);
                }
                else
                {
                    var value = await AsyncOpcUaHelper.ReadStringAsync(NodeId);
                    OnDataReceived(value);
                }
            }
            catch (Exception ex)
            {
                UpdateUI(() =>
                {
                    _statusLabel.Text = $"状态: 刷新失败 - {ex.Message}";
                    _statusLabel.ForeColor = Color.Red;
                });
            }
        }
        #endregion

        #region 资源清理
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }

    /// <summary>
    /// 异步优化的数值输入控件
    /// 专为工业自动化场景设计，避免UI阻塞
    /// </summary>
    public partial class AsyncNumericInputControl : UserControl
    {
        #region 属性
        /// <summary>
        /// 节点地址
        /// </summary>
        public string NodeId { get; set; } = "1214.PLC1.Control.SetPoint";

        /// <summary>
        /// 最小值
        /// </summary>
        public float MinValue { get; set; } = 0f;

        /// <summary>
        /// 最大值
        /// </summary>
        public float MaxValue { get; set; } = 100f;

        /// <summary>
        /// 小数位数
        /// </summary>
        public int DecimalPlaces { get; set; } = 1;

        /// <summary>
        /// 当前值
        /// </summary>
        public float CurrentValue { get; private set; }
        #endregion

        #region 字段
        private TextBox _valueTextBox;
        private Button _writeButton;
        private Button _readButton;
        private Label _rangeLabel;
        private Label _statusLabel;
        private Timer _readTimer;
        private bool _isWriting = false;
        #endregion

        #region 构造函数
        public AsyncNumericInputControl()
        {
            InitializeControl();
            StartPeriodicRead();
        }

        private void InitializeControl()
        {
            // 简洁美观的设计风格
            this.Size = new Size(300, 120);
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;

            _rangeLabel = new Label
            {
                Text = $"范围: {MinValue:F1} - {MaxValue:F1}",
                Location = new Point(10, 10),
                Size = new Size(280, 20),
                ForeColor = Color.DarkGray,
                Font = new Font("微软雅黑", 8F, FontStyle.Regular)
            };

            _valueTextBox = new TextBox
            {
                Location = new Point(10, 35),
                Size = new Size(200, 25),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("微软雅黑", 10F, FontStyle.Regular),
                TextAlign = HorizontalAlignment.Right
            };

            _writeButton = new Button
            {
                Text = "写入",
                Location = new Point(220, 35),
                Size = new Size(70, 25),
                BackColor = Color.FromArgb(200, 255, 200),
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false,
                Font = new Font("微软雅黑", 9F, FontStyle.Regular),
                ForeColor = Color.DarkGray
            };
            _writeButton.FlatAppearance.BorderColor = Color.LightGray;

            _readButton = new Button
            {
                Text = "读取",
                Location = new Point(220, 65),
                Size = new Size(70, 25),
                BackColor = Color.FromArgb(200, 230, 255),
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false,
                Font = new Font("微软雅黑", 9F, FontStyle.Regular),
                ForeColor = Color.DarkGray
            };
            _readButton.FlatAppearance.BorderColor = Color.LightGray;

            _statusLabel = new Label
            {
                Text = "状态: 准备就绪",
                Location = new Point(10, 95),
                Size = new Size(280, 20),
                ForeColor = Color.Green,
                Font = new Font("微软雅黑", 8F, FontStyle.Regular)
            };

            // 绑定事件
            _writeButton.Click += WriteButton_ClickAsync;
            _readButton.Click += ReadButton_ClickAsync;
            _valueTextBox.KeyPress += ValueTextBox_KeyPress;

            this.Controls.AddRange(new Control[] {
                _rangeLabel, _valueTextBox, _writeButton, _readButton, _statusLabel
            });
        }

        private void StartPeriodicRead()
        {
            _readTimer = new Timer();
            _readTimer.Interval = 2000; // 每2秒自动读取一次
            _readTimer.Tick += async (s, e) => await ReadValueAsync();
            _readTimer.Start();
        }
        #endregion

        #region 异步事件处理
        private async void WriteButton_ClickAsync(object sender, EventArgs e)
        {
            await WriteValueAsync();
        }

        private async void ReadButton_ClickAsync(object sender, EventArgs e)
        {
            await ReadValueAsync();
        }

        private async void ValueTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                await WriteValueAsync();
                e.Handled = true;
            }
        }

        private async Task WriteValueAsync()
        {
            if (_isWriting || string.IsNullOrWhiteSpace(NodeId))
                return;

            _isWriting = true;
            
            try
            {
                UpdateUI(() =>
                {
                    _writeButton.Enabled = false;
                    _statusLabel.Text = "状态: 写入中...";
                    _statusLabel.ForeColor = Color.Orange;
                });

                string inputText = "";
                UpdateUI(() => inputText = _valueTextBox.Text.Trim());

                if (!AsyncOpcUaHelper.IsValidNumber(inputText))
                {
                    UpdateUI(() =>
                    {
                        _statusLabel.Text = "状态: 输入格式错误";
                        _statusLabel.ForeColor = Color.Red;
                    });
                    return;
                }

                float value = Convert.ToSingle(inputText);
                bool success = await AsyncOpcUaHelper.WriteFloatWithRangeAsync(NodeId, value, MinValue, MaxValue);

                UpdateUI(() =>
                {
                    if (success)
                    {
                        CurrentValue = Math.Max(MinValue, Math.Min(MaxValue, value));
                        _statusLabel.Text = $"状态: 成功写入 {CurrentValue.ToString($"F{DecimalPlaces}")}";
                        _statusLabel.ForeColor = Color.Green;
                        _valueTextBox.Text = CurrentValue.ToString($"F{DecimalPlaces}");
                    }
                    else
                    {
                        _statusLabel.Text = "状态: 写入失败";
                        _statusLabel.ForeColor = Color.Red;
                    }
                });
            }
            catch (Exception ex)
            {
                UpdateUI(() =>
                {
                    _statusLabel.Text = $"状态: 写入异常 - {ex.Message}";
                    _statusLabel.ForeColor = Color.Red;
                });
            }
            finally
            {
                _isWriting = false;
                UpdateUI(() => _writeButton.Enabled = true);
            }
        }

        private async Task ReadValueAsync()
        {
            if (_isWriting || string.IsNullOrWhiteSpace(NodeId))
                return;

            try
            {
                float value = await AsyncOpcUaHelper.ReadFloatAsync(NodeId);
                CurrentValue = value;

                UpdateUI(() =>
                {
                    _valueTextBox.Text = value.ToString($"F{DecimalPlaces}");
                    _statusLabel.Text = $"状态: 读取成功 {value.ToString($"F{DecimalPlaces}")}";
                    _statusLabel.ForeColor = Color.Blue;
                });
            }
            catch (Exception ex)
            {
                UpdateUI(() =>
                {
                    _statusLabel.Text = $"状态: 读取失败 - {ex.Message}";
                    _statusLabel.ForeColor = Color.Red;
                });
            }
        }

        private void UpdateUI(Action action)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }
        #endregion

        #region 资源清理
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _readTimer?.Stop();
                _readTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}