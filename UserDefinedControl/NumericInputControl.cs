using System;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserDefinedControl
{
    /// <summary>
    /// 自定义数值输入控件 - 类似NumericUpDown
    /// </summary>
    public partial class NumericInputControl : UserControl
    {
        #region 私有字段
        private double _feedbackValue = 0.0;          // 反馈值
        private double _setValue = 0.0;               // 设置值
        private double _stepValue = 1.0;              // 步值
        private double _minValue = 0.0;               // 最小值
        private double _maxValue = 100.0;             // 最大值
        private string _unit = "";                    // 单位
        private string _controlName = "控件";          // 控件名称
        private string _feedbackAddress = "";         // 反馈值地址
        private string _setValueAddress = "";         // 设置值地址
        private bool _isMouseOverMinus = false;       // 鼠标悬停在减号按钮
        private bool _isMouseOverPlus = false;        // 鼠标悬停在加号按钮
        private bool _isMouseDownMinus = false;       // 鼠标按下减号按钮
        private bool _isMouseDownPlus = false;        // 鼠标按下加号按钮
        private Timer _updateTimer;                   // 更新定时器
        private bool _isEditing = false;              // 是否正在编辑
        private TextBox _inputTextBox;                // 隐藏的输入文本框
        private bool _isInputActive = false;          // 输入框是否激活
        private int _buttonRadius = 3;

        // 工业风格设计颜色
        private Color _backgroundColor = Color.FromArgb(240, 240, 240);        // 整体背景色（浅灰）
        private Color _feedbackBackColor = Color.FromArgb(50, 50, 50);        // 反馈区域背景色（深灰）
        private Color _feedbackTextColor = Color.FromArgb(0, 255, 128);       // 反馈值文字颜色（绿色LED风格）
        private Color _setValueBackColor = Color.White;                        // 设置值区域背景色（白色）
        private Color _setValueTextColor = Color.FromArgb(64, 64, 64);         // 设置值文字颜色（深灰）
        private Color _buttonNormalColor = Color.FromArgb(70, 130, 180);      // 按钮正常颜色（钢蓝色）
        private Color _buttonHoverColor = Color.FromArgb(100, 149, 237);      // 按钮悬停颜色（矢车菊蓝）
        private Color _buttonPressColor = Color.FromArgb(65, 105, 225);       // 按钮按下颜色（皇家蓝）
        private Color _borderColor = Color.FromArgb(169, 169, 169);           // 边框颜色（深灰）
        private Color _controlNameColor = Color.FromArgb(105, 105, 105);      // 控件名颜色（暗灰）
        
        // 委托
        private WriteFloatAddressAsyncDelegate _writeFloatAddressMethod;
        private ReadFloatAddressAsyncDelegate _readFloatAddressMethod;
        #endregion

        #region 委托定义
        /// <summary>
        /// 浮点数地址写入方法委托（异步）
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">要写入的值</param>
        public delegate Task WriteFloatAddressAsyncDelegate(string address, double value);
        
        /// <summary>
        /// 浮点数地址读取方法委托（异步）
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>读取的值</returns>
        public delegate Task<double> ReadFloatAddressAsyncDelegate(string address);
        #endregion

        #region 事件定义
        /// <summary>
        /// 设置值改变事件
        /// </summary>
        public event EventHandler<double> SetValueChanged;

        /// <summary>
        /// 反馈值改变事件
        /// </summary>
        public event EventHandler<double> FeedbackValueChanged;
        #endregion

        #region 公共属性
        /// <summary>
        /// 反馈值（只读）
        /// </summary>
        [Category("数值属性")]
        [Description("当前的反馈值")]
        [ReadOnly(true)]
        public double FeedbackValue
        {
            get => _feedbackValue;
            private set
            {
                if (Math.Abs(_feedbackValue - value) > double.Epsilon)
                {
                    _feedbackValue = value;
                    Invalidate(); // 直接重绘整个控件
                    FeedbackValueChanged?.Invoke(this, _feedbackValue);
                }
            }
        }

        /// <summary>
        /// 设置值
        /// </summary>
        [Category("数值属性")]
        [Description("设置的目标值")]
        public double SetValue
        {
            get => _setValue;
            set
            {
                double clampedValue = Math.Max(_minValue, Math.Min(_maxValue, value));
                if (Math.Abs(_setValue - clampedValue) > double.Epsilon)
                {
                    _setValue = clampedValue;
                    Invalidate(); // 直接重绘
                    SetValueChanged?.Invoke(this, _setValue);
                    _ = WriteSetValueToAddressAsync();
                }
            }
        }

        /// <summary>
        /// 步值
        /// </summary>
        [Category("数值属性")]
        [Description("加减按钮的步长值")]
        public double StepValue
        {
            get => _stepValue;
            set => _stepValue = value > 0 ? value : 1.0;
        }

        /// <summary>
        /// 最小值
        /// </summary>
        [Category("数值属性")]
        [Description("设置值的最小限制")]
        public double MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
                if (_setValue < _minValue)
                    SetValue = _minValue;
            }
        }

        /// <summary>
        /// 最大值
        /// </summary>
        [Category("数值属性")]
        [Description("设置值的最大限制")]
        public double MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                if (_setValue > _maxValue)
                    SetValue = _maxValue;
            }
        }

        /// <summary>
        /// 单位
        /// </summary>
        [Category("显示属性")]
        [Description("数值的单位")]
        public string Unit
        {
            get => _unit;
            set
            {
                _unit = value ?? "";
                Invalidate(); // 直接重绘整个控件
            }
        }

        /// <summary>
        /// 控件名称
        /// </summary>
        [Category("显示属性")]
        [Description("显示在反馈框上方的控件名称")]
        public string ControlName
        {
            get => _controlName;
            set
            {
                _controlName = value ?? "控件";
                Invalidate();
            }
        }

        /// <summary>
        /// 按钮圆角
        /// </summary>
        [Category("自定义属性")]
        [Description("按钮圆角")]
        public int ButtonRadius
        {
            get => _buttonRadius;
            set
            {
                _buttonRadius = value;
                Invalidate();
            }
        }


        /// <summary>
        /// 反馈值地址
        /// </summary>
        [Category("控制设置")]
        [Description("反馈值的读取地址")]
        public string FeedbackAddress
        {
            get => _feedbackAddress;
            set => _feedbackAddress = value ?? "";
        }

        /// <summary>
        /// 设置值地址
        /// </summary>
        [Category("控制设置")]
        [Description("设置值的读写地址")]
        public string SetValueAddress
        {
            get => _setValueAddress;
            set => _setValueAddress = value ?? "";
        }

        /// <summary>
        /// 反馈值背景颜色
        /// </summary>
        [Category("颜色设置")]
        [Description("反馈值显示区域的背景颜色")]
        public Color FeedbackBackColor
        {
            get => _feedbackBackColor;
            set
            {
                if (_feedbackBackColor != value)
                {
                    _feedbackBackColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// 反馈值文字颜色
        /// </summary>
        [Category("颜色设置")]
        [Description("反馈值文字的颜色")]
        public Color FeedbackTextColor
        {
            get => _feedbackTextColor;
            set
            {
                if (_feedbackTextColor != value)
                {
                    _feedbackTextColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// 设置值背景颜色
        /// </summary>
        [Category("颜色设置")]
        [Description("设置值显示区域的背景颜色")]
        public Color SetValueBackColor
        {
            get => _setValueBackColor;
            set
            {
                if (_setValueBackColor != value)
                {
                    _setValueBackColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// 设置值文字颜色
        /// </summary>
        [Category("颜色设置")]
        [Description("设置值文字的颜色")]
        public Color SetValueTextColor
        {
            get => _setValueTextColor;
            set
            {
                if (_setValueTextColor != value)
                {
                    _setValueTextColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// 写浮点数地址方法
        /// </summary>
        [Category("控制设置")]
        [Description("写入浮点数地址的方法委托")]
        [Browsable(false)]
        public WriteFloatAddressAsyncDelegate WriteFloatAddressMethod
        {
            get => _writeFloatAddressMethod;
            set => _writeFloatAddressMethod = value;
        }
        
        /// <summary>
        /// 读浮点数地址方法
        /// </summary>
        [Category("控制设置")]
        [Description("读取浮点数地址的方法委托")]
        [Browsable(false)]
        public ReadFloatAddressAsyncDelegate ReadFloatAddressMethod
        {
            get => _readFloatAddressMethod;
            set => _readFloatAddressMethod = value;
        }
        #endregion

        #region 构造函数
        public NumericInputControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            this.Size = new Size(250, 60); // 调整尺寸以适应新工业风格布局
            this.BackColor = _backgroundColor;
            this.Font = new Font("微软雅黑", 9F, FontStyle.Regular);
            
            // 初始化隐藏输入文本框
            InitializeInputTextBox();
            
            // 初始化更新定时器
            _updateTimer = new Timer();
            _updateTimer.Interval = 1000; // 1秒更新一次
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }
        #endregion

        #region 重写方法
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // 绘制三个区域
            DrawThreeRegions(g);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            if (e.Button == MouseButtons.Left)
            {
                Rectangle minusRect = GetMinusButtonRect();
                Rectangle plusRect = GetPlusButtonRect();
                Rectangle setValueRect = GetSetValueRect();
                
                if (minusRect.Contains(e.Location))
                {
                    _isMouseDownMinus = true;
                    SetValue -= _stepValue;
                    Invalidate();
                }
                else if (plusRect.Contains(e.Location))
                {
                    _isMouseDownPlus = true;
                    SetValue += _stepValue;
                    Invalidate();
                }
                else if (setValueRect.Contains(e.Location))
                {
                    // 点击输入框区域，激活输入模式
                    ActivateInputMode();
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            
            if (_isMouseDownMinus || _isMouseDownPlus)
            {
                _isMouseDownMinus = false;
                _isMouseDownPlus = false;
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            bool wasOverMinus = _isMouseOverMinus;
            bool wasOverPlus = _isMouseOverPlus;
            
            Rectangle minusRect = GetMinusButtonRect();
            Rectangle plusRect = GetPlusButtonRect();
            
            _isMouseOverMinus = minusRect.Contains(e.Location);
            _isMouseOverPlus = plusRect.Contains(e.Location);
            
            if (wasOverMinus != _isMouseOverMinus || wasOverPlus != _isMouseOverPlus)
            {
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            
            if (_isMouseOverMinus || _isMouseOverPlus)
            {
                _isMouseOverMinus = false;
                _isMouseOverPlus = false;
                Invalidate();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _updateTimer?.Stop();
                _updateTimer.Tick -= UpdateTimer_Tick;
                _updateTimer?.Dispose();
                _inputTextBox?.Dispose();
                _updateTimer=null;
                _inputTextBox = null;
            }
            base.Dispose(disposing);
        }
        #endregion

        #region 私有方法
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "NumericInputControl";
            this.ResumeLayout(false);
        }

        private Rectangle GetControlNameRect()
        {
            int groupHeight = 32; // 与控制组保持一致的高度
            int startY = (Height - groupHeight) / 2; // 垂直居中
            int nameHeight = 12; // 控件名称高度
            return new Rectangle(0, startY, Width / 2, nameHeight);
        }

        private Rectangle GetFeedbackRect()
        {
            int groupHeight = 32; // 与右侧控制组保持一致的高度
            int startY = (Height - groupHeight) / 2; // 垂直居中对齐
            int nameHeight = 12; // 控件名称高度
            return new Rectangle(0, startY + nameHeight, Width / 2, groupHeight - nameHeight);
        }

        private Rectangle GetSetValueRect()
        {
            int buttonWidth = 30; // 按钮宽度
            int groupHeight = 32; // 控制组高度
            int startY = (Height - groupHeight) / 2; // 垂直居中
            int leftButtonX = Width / 2 + 2;
            int rightButtonX = Width - buttonWidth;
            return new Rectangle(leftButtonX + buttonWidth, startY, rightButtonX - leftButtonX - buttonWidth, groupHeight);
        }

        private Rectangle GetMinusButtonRect()
        {
            int buttonWidth = 30;
            int groupHeight = 32; // 控制组高度
            int startY = (Height - groupHeight) / 2; // 垂直居中
            return new Rectangle(Width / 2 + 2, startY, buttonWidth, groupHeight);
        }

        private Rectangle GetPlusButtonRect()
        {
            int buttonWidth = 30;
            int groupHeight = 32; // 控制组高度
            int startY = (Height - groupHeight) / 2; // 垂直居中
            return new Rectangle(Width - buttonWidth, startY, buttonWidth, groupHeight);
        }

        /// <summary>
        /// 获取整体控制区域（输入框+按钮组合）
        /// </summary>
        private Rectangle GetControlGroupRect()
        {
            int groupHeight = 32; // 控制组高度
            int startY = (Height - groupHeight) / 2; // 垂直居中
            return new Rectangle(Width / 2 + 2, startY, Width / 2 - 4, groupHeight);
        }

        private void DrawThreeRegions(Graphics g)
        {
            Rectangle controlNameRect = GetControlNameRect();
            Rectangle feedbackRect = GetFeedbackRect();
            Rectangle setValueRect = GetSetValueRect();
            Rectangle minusRect = GetMinusButtonRect();
            Rectangle plusRect = GetPlusButtonRect();
            Rectangle controlGroupRect = GetControlGroupRect();

            // 绘制控件名称
            using (SolidBrush textBrush = new SolidBrush(_controlNameColor))
            using (Font font = new Font("微软雅黑", 8.5F, FontStyle.Bold))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(_controlName, font, textBrush, controlNameRect, sf);
            }

            // 绘制反馈值区域（透明背景）
            // 不绘制背景和边框，只绘制文字
            
            // 绘制反馈值文本（包含单位）
            string feedbackText = _feedbackValue.ToString("F2");
            if (!string.IsNullOrEmpty(_unit))
                feedbackText += " " + _unit;
                
            using (SolidBrush textBrush = new SolidBrush(_feedbackTextColor))
            using (Font font = new Font("微软雅黑", 11F, FontStyle.Bold))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(feedbackText, font, textBrush, feedbackRect, sf);
            }

            // 绘制整体控制区域背景（输入框+按钮的统一背景）
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(248, 248, 248)))
            {
                GraphicsPath path = GetRoundedRectanglePath(controlGroupRect, _buttonRadius);
                g.FillPath(brush, path);
            }
            // 添加内阴影效果
            //using (Pen innerShadowPen = new Pen(Color.FromArgb(50, 0, 0, 0), 1f))
            //{
            //    Rectangle innerRect = new Rectangle(controlGroupRect.X + 1, controlGroupRect.Y + 1, 
            //                                        controlGroupRect.Width - 2, controlGroupRect.Height - 2);
            //    GraphicsPath innerPath = GetRoundedRectanglePath(innerRect, 5);
            //    g.DrawPath(innerShadowPen, innerPath);
            //}
            // 绘制主边框 - 加粗并使用更深的颜色
            using (Pen borderPen = new Pen(Color.FromArgb(100, 100, 100), 2.5f))
            {
                GraphicsPath path = GetRoundedRectanglePath(controlGroupRect, _buttonRadius);
                g.DrawPath(borderPen, path);
            }

            // 绘制减号按钮（作为整体的一部分）
            Color minusColor = _isMouseDownMinus ? _buttonPressColor : 
                              _isMouseOverMinus ? _buttonHoverColor : _buttonNormalColor;
            using (SolidBrush brush = new SolidBrush(minusColor))
            {
                // 只绘制左侧圆角
                GraphicsPath path = GetLeftRoundedRectanglePath(minusRect, _buttonRadius-1);
                g.FillPath(brush, path);
            }
            
            // 绘制减号符号
            using (Pen linePen = new Pen(Color.White, 3f))
            {
                linePen.StartCap = LineCap.Round;
                linePen.EndCap = LineCap.Round;
                int centerX = minusRect.X + minusRect.Width / 2;
                int centerY = minusRect.Y + minusRect.Height / 2;
                g.DrawLine(linePen, centerX - 6, centerY, centerX + 6, centerY);
            }

            // 绘制设置值区域（中间部分）
            using (SolidBrush brush = new SolidBrush(_setValueBackColor))
            {
                g.FillRectangle(brush, setValueRect);
            }
            //// 添加更加明显的分割线 输入框左右2边有阴影
            //using (Pen separatorPen = new Pen(Color.FromArgb(160, 160, 160), 1.5f))
            //{
            //    // 左侧分割线
            //    g.DrawLine(separatorPen, minusRect.Right, minusRect.Top + 6, minusRect.Right, minusRect.Bottom - 6);
            //    // 右侧分割线
            //    g.DrawLine(separatorPen, plusRect.Left, plusRect.Top + 6, plusRect.Left, plusRect.Bottom - 6);
            //}
            
            // 绘制设置值文本（不包含单位）
            if (!_isInputActive) // 只有在非输入模式下才绘制文本
            {
                string setValueText = _setValue.ToString("F1");
                    
                using (SolidBrush textBrush = new SolidBrush(_setValueTextColor))
                using (Font font = new Font("微软雅黑", 11F, FontStyle.Bold))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(setValueText, font, textBrush, setValueRect, sf);
                }
            }

            // 绘制加号按钮（作为整体的一部分）
            Color plusColor = _isMouseDownPlus ? _buttonPressColor : 
                             _isMouseOverPlus ? _buttonHoverColor : _buttonNormalColor;
            using (SolidBrush brush = new SolidBrush(plusColor))
            {
                // 只绘制右侧圆角
                GraphicsPath path = GetRightRoundedRectanglePath(plusRect, _buttonRadius-1);
                g.FillPath(brush, path);
            }
            
            // 绘制加号符号
            using (Pen linePen = new Pen(Color.White, 3f))
            {
                linePen.StartCap = LineCap.Round;
                linePen.EndCap = LineCap.Round;
                int centerX = plusRect.X + plusRect.Width / 2;
                int centerY = plusRect.Y + plusRect.Height / 2;
                g.DrawLine(linePen, centerX - 6, centerY, centerX + 6, centerY);
                g.DrawLine(linePen, centerX, centerY - 6, centerX, centerY + 6);
            }
        }

        /// <summary>
        /// 获取圆角矩形路径
        /// </summary>
        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            Rectangle arc = new Rectangle(rect.X, rect.Y, diameter, diameter);

            // 左上角
            path.AddArc(arc, 180, 90);

            // 右上角
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // 右下角
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // 左下角
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// 获取左侧圆角矩形路径（只有左侧圆角）
        /// </summary>
        private GraphicsPath GetLeftRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            Rectangle arc = new Rectangle(rect.X, rect.Y, diameter, diameter);

            // 左上角
            path.AddArc(arc, 180, 90);
            // 直线到右上角
            path.AddLine(rect.Right, rect.Top, rect.Right, rect.Bottom);
            // 左下角
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// 获取右侧圆角矩形路径（只有右侧圆角）
        /// </summary>
        private GraphicsPath GetRightRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            
            // 从左上角开始
            path.AddLine(rect.Left, rect.Top, rect.Right - radius, rect.Top);
            
            // 右上角
            Rectangle arc = new Rectangle(rect.Right - diameter, rect.Y, diameter, diameter);
            path.AddArc(arc, 270, 90);

            // 右下角
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            
            // 直线到左下角
            path.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top);

            path.CloseFigure();
            return path;
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            // 定期更新反馈值和设置值
            _ = UpdateValuesFromAddressAsync();
        }

        /// <summary>
        /// 从地址更新数值（异步）
        /// </summary>
        public async Task UpdateValuesFromAddressAsync()
        {
            if (_readFloatAddressMethod != null)
            {
                try
                {
                    // 更新反馈值
                    if (!string.IsNullOrEmpty(_feedbackAddress))
                    {
                        double feedbackValue = await _readFloatAddressMethod(_feedbackAddress);
                        if (Math.Abs(FeedbackValue - feedbackValue) > double.Epsilon)
                        {
                            this.BeginInvoke(new Action(() => {
                                FeedbackValue = feedbackValue;
                            }));
                        }
                    }

                    // 更新设置值（不再需要检查是否正在编辑）
                    if (!string.IsNullOrEmpty(_setValueAddress) && !_isInputActive)
                    {
                        double setValue = await _readFloatAddressMethod(_setValueAddress);
                        if (Math.Abs(_setValue - setValue) > double.Epsilon)
                        {
                            this.BeginInvoke(new Action(() => {
                                _setValue = setValue;
                                Invalidate();
                            }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"更新数值失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 写设置值到地址（异步）
        /// </summary>
        private async Task WriteSetValueToAddressAsync()
        {
            if (_writeFloatAddressMethod != null && !string.IsNullOrEmpty(_setValueAddress))
            {
                try
                {
                    await _writeFloatAddressMethod(_setValueAddress, _setValue);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"写入设置值失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 初始化输入文本框
        /// </summary>
        private void InitializeInputTextBox()
        {
            _inputTextBox = new TextBox();
            _inputTextBox.BorderStyle = BorderStyle.None;
            _inputTextBox.BackColor = Color.White;
            _inputTextBox.ForeColor = _setValueTextColor;
            _inputTextBox.Font = new Font("微软雅黑", 11F, FontStyle.Bold);
            _inputTextBox.TextAlign = HorizontalAlignment.Center;
            _inputTextBox.Visible = false;
            
            // 绑定事件
            _inputTextBox.KeyDown += InputTextBox_KeyDown;
            _inputTextBox.Leave += InputTextBox_Leave;
            _inputTextBox.TextChanged += InputTextBox_TextChanged;
            
            this.Controls.Add(_inputTextBox);
        }

        /// <summary>
        /// 激活输入模式
        /// </summary>
        private void ActivateInputMode()
        {
            Rectangle setValueRect = GetSetValueRect();
            
            // 设置输入框位置和尺寸
            _inputTextBox.Bounds = new Rectangle(
                setValueRect.X + 2, 
                setValueRect.Y + 2, 
                setValueRect.Width - 4, 
                setValueRect.Height - 4
            );
            
            // 设置当前值
            _inputTextBox.Text = _setValue.ToString("F1");
            _inputTextBox.Visible = true;
            _inputTextBox.Focus();
            _inputTextBox.SelectAll();
            
            _isInputActive = true;
        }

        /// <summary>
        /// 退出输入模式
        /// </summary>
        private void DeactivateInputMode()
        {
            _inputTextBox.Visible = false;
            _isInputActive = false;
            this.Focus(); // 返回焦点给主控件
            Invalidate(); // 重绘显示正常文本
        }

        /// <summary>
        /// 输入框键盘事件
        /// </summary>
        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // 回车键确认输入
                ApplyInputValue();
                DeactivateInputMode();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                // ESC键取消输入
                DeactivateInputMode();
                e.Handled = true;
            }
        }

        /// <summary>
        /// 输入框失去焦点事件
        /// </summary>
        private void InputTextBox_Leave(object sender, EventArgs e)
        {
            ApplyInputValue();
            DeactivateInputMode();
        }

        /// <summary>
        /// 输入框文本改变事件
        /// </summary>
        private void InputTextBox_TextChanged(object sender, EventArgs e)
        {
            // 可以在这里添加实时验证逻辑
        }

        /// <summary>
        /// 应用输入的数值
        /// </summary>
        private void ApplyInputValue()
        {
            if (double.TryParse(_inputTextBox.Text, out double newValue))
            {
                SetValue = newValue; // 使用属性设置，会自动限制范围并触发事件
            }
            else
            {
                // 输入无效，恢复原值
                System.Diagnostics.Debug.WriteLine("输入的数值无效");
            }
        }
        #endregion
    }
}