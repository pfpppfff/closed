using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserDefinedControl
{
    public partial class ButtonControl : UserControl
    {
        #region 私有字段
       
        private bool _signalB = false;      // 按钮状态信号
        private bool _signalC = false;      // 模式2控制的地址变量
        private Int16 _initValue = 0;
        private Int16 _targetValue = 1;
        private ButtonMode _buttonMode = ButtonMode.Toggle;
        private ButtonControlValueTypeMode _buttonControlValueTypeMode = ButtonControl.ButtonControlValueTypeMode.Int;
        private bool _isMouseDown = false;
        private bool _isMouseOver = false;
        private string _buttonText = "按钮";
        private Font _buttonFont;
        private int _buttonRadius = 3;
        private Timer _updateTimer;

        // 颜色设置
      
        private Color _buttonOnColor = Color.FromArgb(70, 130, 180);        // 按钮有信号颜色（蓝色）
        private Color _buttonOffColor = Color.FromArgb(176, 196, 222);      // 按钮无信号颜色（浅蓝灰）
        private Color _buttonTextOnColor = Color.White;                     // 按钮有信号时文字颜色
        private Color _buttonTextOffColor = Color.Black;                    // 按钮无信号时文字颜色

        // 地址和委托设置
        private string _controlAddress = "";                               // 控制地址
     
        private string _signalBAddress = "";                               // SignalB读取地址 按钮读取信号地址
        private WriteBoolAddressAsyncDelegate _writeBoolAddressMethod;              // 写bool地址方法（异步）
        private ReadBoolAddressAsyncDelegate _readBoolAddressMethod;               // 读bool地址方法（异步）
        private WriteIntAddressAsyncDelegate _writeIntAddressMethod;              // 写int地址方法（异步）
        private ReadIntAddressAsyncDelegate _readIntAddressMethod;               // 读int地址方法（异步）

        #endregion

        #region 枚举定义
        /// <summary>
        /// 按钮操作模式
        /// </summary>
        public enum ButtonMode
        {
            /// <summary>
            /// 切换模式 - 点击切换SignalB状态
            /// </summary>
            Toggle = 1,
            /// <summary>
            /// 按下松开模式 - 按下时SignalC为true，松开时为false
            /// </summary>
            PressRelease = 2
        }

        /// <summary>
        /// 按钮操作模式
        /// </summary>
        public enum ButtonControlValueTypeMode
        {
            /// <summary>
            /// 控制类型为int
            /// </summary>
            Int = 1,
            /// <summary>
            /// 控制类型为bool
            /// </summary>
            Bool = 2
        }
        #endregion

        #region 事件定义
        /// <summary>
        /// SignalB状态改变事件
        /// </summary>
        public event EventHandler<bool> SignalBChanged;

        /// <summary>
        /// SignalC状态改变事件
        /// </summary>
        public event EventHandler<bool> SignalCChanged;

        /// <summary>
        /// 按钮点击事件
        /// </summary>
        public new event EventHandler ButtonClick;
        #endregion

        #region 委托定义
        /// <summary>
        /// 地址写入方法委托（异步）
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">要写入的值</param>
        public delegate Task WriteBoolAddressAsyncDelegate(string address, bool value);

        /// <summary>
        /// 地址读取方法委托（异步）
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>读取的值</returns>
        public delegate Task<bool> ReadBoolAddressAsyncDelegate(string address);

        public delegate Task WriteIntAddressAsyncDelegate(string address, Int16 value);

        public delegate Task<Int16> ReadIntAddressAsyncDelegate(string address);
        #endregion

        #region 公共属性


        /// <summary>
        /// 按钮状态信号 (影响按钮颜色)
        /// </summary>
        [Category("自定义属性")]
        [Description("按钮状态信号，影响按钮显示颜色")]
        public bool SignalB
        {
            get => _signalB;
            set
            {
                if (_signalB != value)
                {
                    _signalB = value;
                    InvalidateButton();
                    SignalBChanged?.Invoke(this, _signalB);
                }
            }
        }

        /// <summary>
        /// 模式2使用的地址变量
        /// </summary>
        [Category("自定义属性")]
        [Description("模式2使用的地址变量")]
        public bool SignalC
        {
            get => _signalC;
            set
            {
                if (_signalC != value)
                {
                    _signalC = value;
                    SignalCChanged?.Invoke(this, _signalC);
                }
            }
        }

        /// <summary>
        /// int类型初始值
        /// </summary>
        [Category("自定义属性")]
        [Description("int类型初始值")]
        public Int16 InitValue
        {
            get => _initValue;
            set
            {
                if (_initValue != value)
                {
                    _initValue = value;
                  
                }
            }
        }
        /// <summary>
        /// int类型写入值
        /// </summary>
        [Category("自定义属性")]
        [Description("int类型写入值")]
        public Int16 TargetValue
        {
            get => _targetValue;
            set
            {
                if (_targetValue != value)
                {
                    _targetValue = value;
                }
            }
        }
        /// <summary>
        /// 按钮操作模式
        /// </summary>
        [Category("自定义属性")]
        [Description("按钮操作模式：1=切换模式，2=按下松开模式")]
        public ButtonMode Mode
        {
            get => _buttonMode;
            set
            {
                _buttonMode = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 按钮操作模式
        /// </summary>
        [Category("自定义属性")]
        [Description("按钮操作控制值模式：1=切换模式，2=按下松开模式")]
        public ButtonControlValueTypeMode ControlValueTypeMode
        {
            get => _buttonControlValueTypeMode;
            set
            {
                _buttonControlValueTypeMode = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 按钮文本
        /// </summary>
        [Category("自定义属性")]
        [Description("按钮显示的文本")]
        public string ButtonText
        {
            get => _buttonText;
            set
            {
                if (_buttonText != value)
                {
                    _buttonText = value ?? "";
                    InvalidateButton();
                }
            }
        }

        /// <summary>
        /// 按钮字体
        /// </summary>
        [Category("自定义属性")]
        [Description("按钮文本字体")]
        public Font ButtonFont
        {
            get => _buttonFont ?? Font;
            set
            {
                _buttonFont = value;
                InvalidateButton();
            }
        }

        /// <summary>
        /// 按钮圆角
        /// </summary>
        [Category("自定义属性")]
        [Description("按钮圆角")]
        public int ButtonRadius
        {
            get => _buttonRadius ;
            set
            {
                _buttonRadius = value;
                InvalidateButton();
            }
        }



        /// <summary>
        /// 按钮有信号时的背景颜色
        /// </summary>
        [Category("颜色设置")]
        [Description("按钮有信号时的背景颜色（默认蓝色）")]
        public Color ButtonOnColor
        {
            get => _buttonOnColor;
            set
            {
                if (_buttonOnColor != value)
                {
                    _buttonOnColor = value;
                    InvalidateButton();
                }
            }
        }

        /// <summary>
        /// 按钮无信号时的背景颜色
        /// </summary>
        [Category("颜色设置")]
        [Description("按钮无信号时的背景颜色（默认浅蓝灰）")]
        public Color ButtonOffColor
        {
            get => _buttonOffColor;
            set
            {
                if (_buttonOffColor != value)
                {
                    _buttonOffColor = value;
                    InvalidateButton();
                }
            }
        }

        /// <summary>
        /// 按钮有信号时的文字颜色
        /// </summary>
        [Category("颜色设置")]
        [Description("按钮有信号时的文字颜色（默认白色）")]
        public Color ButtonTextOnColor
        {
            get => _buttonTextOnColor;
            set
            {
                if (_buttonTextOnColor != value)
                {
                    _buttonTextOnColor = value;
                    InvalidateButton();
                }
            }
        }

        /// <summary>
        /// 按钮无信号时的文字颜色
        /// </summary>
        [Category("颜色设置")]
        [Description("按钮无信号时的文字颜色（默认黑色）")]
        public Color ButtonTextOffColor
        {
            get => _buttonTextOffColor;
            set
            {
                if (_buttonTextOffColor != value)
                {
                    _buttonTextOffColor = value;
                    InvalidateButton();
                }
            }
        }

        /// <summary>
        /// 控制地址
        /// </summary>
        [Category("控制设置")]
        [Description("按钮控制的地址字符串")]
        public string ControlAddress
        {
            get => _controlAddress;
            set
            {
                _controlAddress = value ?? "";
            }
        }

        /// <summary>
        /// 写地址方法（异步）
        /// </summary>
        [Category("控制设置")]
        [Description("写入bool地址的方法委托（异步）")]
        [Browsable(false)] // 不在属性面板中显示，只能通过代码设置
        public WriteBoolAddressAsyncDelegate WriteBoolAddressMethod
        {
            get => _writeBoolAddressMethod;
            set => _writeBoolAddressMethod = value;
        }

        /// <summary>
        /// 读地址方法（异步）
        /// </summary>
        [Category("控制设置")]
        [Description("读取bool地址的方法委托（异步）")]
        [Browsable(false)] // 不在属性面板中显示，只能通过代码设置
        public ReadBoolAddressAsyncDelegate ReadBoolAddressMethod
        {
            get => _readBoolAddressMethod;
            set => _readBoolAddressMethod = value;
        }

        /// <summary>
        /// 写int地址方法（异步）
        /// </summary>
        [Category("控制设置")]
        [Description("写入Int地址的方法委托（异步）")]
        [Browsable(false)] // 不在属性面板中显示，只能通过代码设置
        public WriteIntAddressAsyncDelegate WriteIntAddressMethod
        {
            get => _writeIntAddressMethod;
            set => _writeIntAddressMethod = value;
        }

        /// <summary>
        /// 读int地址方法（异步）
        /// </summary>
        [Category("控制设置")]
        [Description("读取Int地址的方法委托（异步）")]
        [Browsable(false)] // 不在属性面板中显示，只能通过代码设置
        public ReadIntAddressAsyncDelegate ReadIntAddressMethod
        {
            get => _readIntAddressMethod;
            set => _readIntAddressMethod = value;
        }

        /// <summary>
        /// SignalB读取地址
        /// </summary>
        [Category("控制设置")]
        [Description("SignalB信号的读取地址字符串")]
        public string SignalBAddress
        {
            get => _signalBAddress;
            set
            {
                _signalBAddress = value ?? "";
            }
        }
        #endregion

        #region 构造函数
        public ButtonControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            this.Size = new Size(120, 40);
            this.BackColor = Color.Transparent;

            // 初始化更新定时器（优化性能，避免频繁重绘）
            _updateTimer = new Timer();
            _updateTimer.Interval = 50; // 20FPS更新频率
            _updateTimer.Tick += UpdateTimer_Tick;
        }
        #endregion

        #region 重写方法
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // 绘制指示灯
            //DrawIndicator(g);

            // 绘制按钮
            DrawButton(g);
        }

        //protected override void OnClick(EventArgs e)
        //{
        //    var mousePos = PointToClient(Control.MousePosition);
        //    if (!GetButtonRect().Contains(mousePos))
        //    {
        //        return; // 点击不在按钮有效区域，不处理
        //    }
        //    base.OnClick(e);
        //    if (_buttonMode == ButtonMode.Toggle)
        //    {
        //        // 模式1：切换模式 - 读取SignalB状态，然后对地址写入相反值（异步）
        //        _ = PerformToggleOperationAsync();
        //    }
        //    else if (_buttonMode == ButtonMode.PressRelease)
        //    {
        //        // 模式2：按下松开模式 - 松开时对地址写false（异步）
        //        if (_buttonControlValueTypeMode == ButtonControlValueTypeMode.Bool)
        //        {
        //            _ = WriteBoolToAddressAsync(false);
        //        }
        //        else
        //        {

        //        }
        //    }
        //    ButtonClick?.Invoke(this, EventArgs.Empty);
        //    InvalidateButton();
        //}

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && GetButtonRect().Contains(e.Location))
            {
                _isMouseDown = true;
                if(_buttonControlValueTypeMode==ButtonControlValueTypeMode.Bool)
                 {//bool
                    if (_buttonMode == ButtonMode.PressRelease)
                    {
                        // 模式2：按下时对地址写true（异步）
                        _ = WriteBoolToAddressAsync(true);
                    }
                    else
                    {
                        _ = PerformToggleOperationAsync();
                    }
                }
                else//int
                {
                    if (_buttonMode == ButtonMode.PressRelease)
                    {
                        // 模式2：按下时对地址写true（异步）
                        _ = WriteIntToAddressAsync(_targetValue);
                    }
                    else
                    {
                        _ = PerformToggleOperationAsync();
                    }
                }
               InvalidateButton();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_isMouseDown)
            {
                _isMouseDown = false;
                if (GetButtonRect().Contains(e.Location))
                {
                    if (_buttonMode == ButtonMode.Toggle)
                    {
                        // 模式1：切换模式 - 读取SignalB状态，然后对地址写入相反值（异步）
                       
                    }
                    else if (_buttonMode == ButtonMode.PressRelease)
                    {
                        // 模式2：按下松开模式 - 松开时对地址写false（异步）
                        if(_buttonControlValueTypeMode==ButtonControlValueTypeMode.Bool)
                        {
                            _ = WriteBoolToAddressAsync(false);
                        }
                        else
                        {

                        }
                    }
                    ButtonClick?.Invoke(this, EventArgs.Empty);
                }
                else if (_buttonMode == ButtonMode.PressRelease)
                {
                    // 鼠标移出按钮区域时松开，对地址写false（异步）
                    _ = WriteBoolToAddressAsync(false);
                }

                InvalidateButton();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            bool wasMouseOver = _isMouseOver;
            _isMouseOver = GetButtonRect().Contains(e.Location);

            if (wasMouseOver != _isMouseOver)
            {
                InvalidateButton();
            }

            // 如果是按下松开模式且鼠标按下，根据鼠标位置更新地址状态（异步）
            if (_buttonMode == ButtonMode.PressRelease && _isMouseDown)
            {
                bool shouldBeActive = GetButtonRect().Contains(e.Location);
                // 鼠标在按钮内时写true，外时写false
                _ = WriteBoolToAddressAsync(shouldBeActive);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            _isMouseOver = false;

            // 如果是按下松开模式且鼠标按下，鼠标离开时对地址写false（异步）
            if (_buttonMode == ButtonMode.PressRelease && _isMouseDown)
            {
                _ = WriteBoolToAddressAsync(false);
            }

            InvalidateButton();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _updateTimer?.Stop();
                _updateTimer.Tick -= UpdateTimer_Tick;
                _updateTimer?.Dispose();
                _buttonFont?.Dispose();
                _updateTimer=null;
                _buttonFont = null;
            }
            base.Dispose(disposing);
        }
        #endregion

        #region 私有方法
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // IndicatorButtonControl
            // 
            this.Name = "IndicatorButtonControl";
            this.Load += new System.EventHandler(this.IndicatorButtonControl_Load);
            this.ResumeLayout(false);

        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            _updateTimer.Stop();
            Invalidate();
        }

        //private Rectangle GetIndicatorRect()
        //{
        //    int size = Math.Min(Height - 8, 24);
        //    return new Rectangle(4, (Height - size) / 2, size, size);
        //}

        private Rectangle GetButtonRect()
        {
            //Rectangle indicatorRect = GetIndicatorRect();
            //int buttonLeft = indicatorRect.Right + 8;
            return new Rectangle(4, 4, Width -8, Height - 8);
        }

        //private void DrawIndicator(Graphics g)
        //{
        //    Rectangle rect = GetIndicatorRect();

        //    // 绘制指示灯外圈
        //    using (Pen borderPen = new Pen(Color.FromArgb(100, 100, 100), 1.5f))
        //    {
        //        g.DrawEllipse(borderPen, rect);
        //    }

        //    // 绘制指示灯内部，使用自定义颜色
        //    rect.Inflate(-2, -2);
        //    Color indicatorColor = _signalA ? _indicatorOnColor : _indicatorOffColor;

        //    using (SolidBrush brush = new SolidBrush(indicatorColor))
        //    {
        //        g.FillEllipse(brush, rect);
        //    }

        //    // 绘制高光效果（仅在有信号时显示）
        //    if (_signalA)
        //    {
        //        Rectangle highlightRect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width / 3, rect.Height / 3);
        //        using (SolidBrush highlightBrush = new SolidBrush(Color.FromArgb(180, Color.White)))
        //        {
        //            g.FillEllipse(highlightBrush, highlightRect);
        //        }
        //    }
        //}

        private void DrawButton(Graphics g)
        {
            Rectangle rect = GetButtonRect();
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // 确定按钮颜色，使用自定义颜色
            Color baseColor = _signalB ? _buttonOnColor : _buttonOffColor;
            Color buttonColor = baseColor;

            if (_isMouseDown)
            {
                buttonColor = Color.FromArgb(Math.Max(0, baseColor.R - 30),
                                           Math.Max(0, baseColor.G - 30),
                                           Math.Max(0, baseColor.B - 30));
            }
            else if (_isMouseOver)
            {
                buttonColor = Color.FromArgb(Math.Min(255, baseColor.R + 20),
                                           Math.Min(255, baseColor.G + 20),
                                           Math.Min(255, baseColor.B + 20));
            }

            // 绘制按钮背景（圆角矩形）
            using (GraphicsPath path = CreateRoundedRectangle(rect, _buttonRadius))
            {
                using (SolidBrush brush = new SolidBrush(buttonColor))
                {
                    g.FillPath(brush, path);
                }

                using (Pen borderPen = new Pen(Color.FromArgb(100, 100, 100), 1f))
                {
                    g.DrawPath(borderPen, path);
                }
            }

            // 绘制按钮文本，使用自定义文字颜色
            if (!string.IsNullOrEmpty(_buttonText))
            {
                Color textColor = _signalB ? _buttonTextOnColor : _buttonTextOffColor;
                using (SolidBrush textBrush = new SolidBrush(textColor))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    g.DrawString(_buttonText, ButtonFont, textBrush, rect, sf);
                }
            }

            // 绘制模式指示器（小图标）
            DrawModeIndicator(g, rect);
        }

        private void DrawModeIndicator(Graphics g, Rectangle buttonRect)
        {
            Rectangle indicatorRect = new Rectangle(buttonRect.Right - 16, buttonRect.Y + 2, 12, 8);

            // 使用按钮文字颜色来绘制模式指示器
            Color iconColor = _signalB ? _buttonTextOnColor : _buttonTextOffColor;
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(150, iconColor)))
            {
                if (_buttonMode == ButtonMode.Toggle)
                {
                    // 绘制切换图标（⇄）
                    Point[] points = {
                        new Point(indicatorRect.X, indicatorRect.Y + 2),
                        new Point(indicatorRect.X + 4, indicatorRect.Y),
                        new Point(indicatorRect.X + 4, indicatorRect.Y + 1),
                        new Point(indicatorRect.X + 8, indicatorRect.Y + 1),
                        new Point(indicatorRect.X + 8, indicatorRect.Y + 3),
                        new Point(indicatorRect.X + 4, indicatorRect.Y + 3),
                        new Point(indicatorRect.X + 4, indicatorRect.Y + 4),
                        new Point(indicatorRect.X, indicatorRect.Y + 2)
                    };
                    g.FillPolygon(brush, points);

                    Point[] points2 = {
                        new Point(indicatorRect.Right, indicatorRect.Y + 6),
                        new Point(indicatorRect.Right - 4, indicatorRect.Bottom),
                        new Point(indicatorRect.Right - 4, indicatorRect.Bottom - 1),
                        new Point(indicatorRect.Right - 8, indicatorRect.Bottom - 1),
                        new Point(indicatorRect.Right - 8, indicatorRect.Bottom - 3),
                        new Point(indicatorRect.Right - 4, indicatorRect.Bottom - 3),
                        new Point(indicatorRect.Right - 4, indicatorRect.Bottom - 4),
                        new Point(indicatorRect.Right, indicatorRect.Y + 6)
                    };
                    g.FillPolygon(brush, points2);
                }
                else
                {
                    // 绘制按下图标（↓）
                    Point[] points = {
                        new Point(indicatorRect.X + 4, indicatorRect.Y),
                        new Point(indicatorRect.X + 8, indicatorRect.Y),
                        new Point(indicatorRect.X + 8, indicatorRect.Y + 4),
                        new Point(indicatorRect.Right, indicatorRect.Y + 4),
                        new Point(indicatorRect.X + 6, indicatorRect.Bottom),
                        new Point(indicatorRect.X, indicatorRect.Y + 4),
                        new Point(indicatorRect.X + 4, indicatorRect.Y + 4)
                    };
                    g.FillPolygon(brush, points);
                }
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(rect.Location, size);

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

        //private void InvalidateIndicator()
        //{
        //    if (!_updateTimer.Enabled)
        //    {
        //        Rectangle rect = GetIndicatorRect();
        //        rect.Inflate(2, 2);
        //        Invalidate(rect);
        //    }
        //}

        private void InvalidateButton()
        {
            if (!_updateTimer.Enabled)
            {
                Rectangle rect = GetButtonRect();
                rect.Inflate(2, 2);
                Invalidate(rect);
            }
        }

        /// <summary>
        /// 向地址写入值（异步）
        /// </summary>
        /// <param name="value">要写入的值</param>
        private async Task WriteBoolToAddressAsync(bool value)
        {
            if (_writeBoolAddressMethod != null && !string.IsNullOrEmpty(_controlAddress))
            {
                try
                {
                    await _writeBoolAddressMethod(_controlAddress, value);
                }
                catch (Exception ex)
                {
                    // 可以在这里记录错误或触发错误事件
                    System.Diagnostics.Debug.WriteLine($"写入地址失败: {ex.Message}");
                }
            }
        }
        private async Task WriteIntToAddressAsync(Int16 value)
        {
            if (_writeIntAddressMethod != null && !string.IsNullOrEmpty(_controlAddress))
            {
                try
                {
                    await _writeIntAddressMethod(_controlAddress, value);
                }
                catch (Exception ex)
                {
                    // 可以在这里记录错误或触发错误事件
                    System.Diagnostics.Debug.WriteLine($"写入地址失败: {ex.Message}");
                }
            }
        }
        /// <summary>
        /// 从地址读取值（异步）
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>读取的值</returns>
        private async Task<bool> ReadBoolFromAddressAsync(string address)
        {
            if (_readBoolAddressMethod != null && !string.IsNullOrEmpty(address))
            {
                try
                {
                    return await _readBoolAddressMethod(address);
                }
                catch (Exception ex)
                {
                    // 可以在这里记录错误或触发错误事件
                    System.Diagnostics.Debug.WriteLine($"读取地址失败: {ex.Message}");
                    return false;
                }
            }
            return false;
        }
        private async Task<Int16> ReadIntFromAddressAsync(string address)
        {
            if (_readIntAddressMethod != null && !string.IsNullOrEmpty(address))
            {
                try
                {
                    return await _readIntAddressMethod(address);
                }
                catch (Exception ex)
                {
                    // 可以在这里记录错误或触发错误事件
                    System.Diagnostics.Debug.WriteLine($"读取地址失败: {ex.Message}");
                    return 0;
                }
            }
            return 0;
        }
        /// <summary>
        /// 执行切换操作（模式1）（异步）
        /// </summary>
        private async Task PerformToggleOperationAsync()
        {
            if(_buttonControlValueTypeMode==ButtonControlValueTypeMode.Bool)
            {
                if (_readBoolAddressMethod != null && _writeBoolAddressMethod != null &&
               !string.IsNullOrEmpty(_controlAddress) && !string.IsNullOrEmpty(_controlAddress))
                {
                    try
                    {
                        // 从 SignalB地址读取当前状态
                        bool currentState = await ReadBoolFromAddressAsync(_controlAddress);
                        // 对控制地址写入相反的值
                        await WriteBoolToAddressAsync(!currentState);

                        // 更新SignalB显示状态（反映地址的新状态）
                        SignalB = !currentState;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"切换操作失败: {ex.Message}");
                    }
                }
            }
            else
            {
                if (_readIntAddressMethod != null && _writeIntAddressMethod != null &&
                 !string.IsNullOrEmpty(_controlAddress) && !string.IsNullOrEmpty(_controlAddress))
                {
                    try
                    {
                        // 从 SignalB地址读取当前状态
                        Int16 currentState = await ReadIntFromAddressAsync(_controlAddress);
                        // 对控制地址写入相反的值
                        if(currentState==_initValue)
                        {
                            await WriteIntToAddressAsync(_targetValue);
                        }
                        else if(currentState == _targetValue)
                        {
                            await WriteIntToAddressAsync(_initValue);
                        }
                        else
                        {
                            MessageBox.Show("先关闭其他状态！");
                        }
                            // 更新SignalB显示状态（反映地址的新状态）
                            //SignalB = !currentState;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"切换操作失败: {ex.Message}");
                    }
                }
            }
           
        }

        /// <summary>
        /// 更新SignalB状态以反映地址的当前值（异步）
        /// </summary>
        public async Task UpdateSignalBFromAddressAsync()
        {
            if (_buttonMode == ButtonMode.Toggle)//切换时 使用控制地址本身反馈按钮信号
            {
                if (  !string.IsNullOrEmpty(_controlAddress))
                {
                    try
                    {   
                        if(_buttonControlValueTypeMode==ButtonControlValueTypeMode.Bool)
                        {
                            if (_readBoolAddressMethod == null) return;
                            bool addressValue = await ReadBoolFromAddressAsync(_controlAddress);
                            if (_signalB != addressValue)
                            {
                                _signalB = addressValue;
                                // 使用BeginInvoke确保在UI线程上执行
                                if (this.InvokeRequired)
                                {
                                    this.BeginInvoke(new Action(() =>
                                    {
                                        InvalidateButton();
                                        SignalBChanged?.Invoke(this, _signalB);
                                    }));
                                }
                                else
                                {
                                    InvalidateButton();
                                    SignalBChanged?.Invoke(this, _signalB);
                                }
                            }
                        }
                        else
                        {
                            if (_readIntAddressMethod == null) return;
                            Int16 addressValue = await ReadIntFromAddressAsync(_controlAddress);
                            bool newSignalB = (addressValue ==_targetValue); 

                            if (_signalB != newSignalB)
                            {
                                _signalB = newSignalB;
                                // 使用BeginInvoke确保在UI线程上执行
                                if (this.InvokeRequired)
                                {
                                    this.BeginInvoke(new Action(() =>
                                    {
                                        InvalidateButton();
                                        SignalBChanged?.Invoke(this, _signalB);
                                    }));
                                }
                                else
                                {
                                    InvalidateButton();
                                    SignalBChanged?.Invoke(this, _signalB);
                                }
                            }
                        }
                       
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"更新SignalB失败: {ex.Message}");
                    }
                }
            }
            else  //点动时 使用外部SignalB状态反馈按钮信号
            {
                if (_readBoolAddressMethod != null && !string.IsNullOrEmpty(_controlAddress) && _buttonControlValueTypeMode == ButtonControlValueTypeMode.Bool)
                {
                    try
                    {
                        bool addressValue = await ReadBoolFromAddressAsync(_controlAddress);
                        if (_signalB != addressValue)
                        {
                            _signalB = addressValue;
                            // 使用BeginInvoke确保在UI线程上执行
                            if (this.InvokeRequired)
                            {
                                this.BeginInvoke(new Action(() =>
                                {
                                    InvalidateButton();
                                    SignalBChanged?.Invoke(this, _signalB);
                                }));
                            }
                            else
                            {
                                InvalidateButton();
                                SignalBChanged?.Invoke(this, _signalB);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"更新SignalB失败: {ex.Message}");
                    }
                }
            }

        }

        /// <summary>
        /// 更新所有信号状态（SignalA和SignalB）（异步）
        /// </summary>
        public async Task UpdateAllSignalsFromAddressAsync()
        {
            try
            {
                await UpdateSignalBFromAddressAsync();
            }
            catch (Exception ex)
            {
                // 记录异常但不抛出，避免影响其他控件
                System.Diagnostics.Debug.WriteLine($"更新控件信号时发生异常: {ex.Message}");
            }
        }
        #endregion

        private void IndicatorButtonControl_Load(object sender, EventArgs e)
        {

        }
    }
}
