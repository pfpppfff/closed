using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserDefinedControl
{
    /// <summary>
    /// PLC 数据类型枚举
    /// </summary>
    public enum PlcDataType
    {
        Bool = 0,
        Short = 1
    }

    /// <summary>
    /// 自定义 PLC 纯净版指示灯控件 (支持 Bool 和 Short 多状态模式)
    /// </summary>
    public partial class PlcIndicatorLightControl : UserControl
    {
        #region 私有字段
        // 通讯与控制
        private string _valueAddress = "";
        private bool _enablePlcComm = true;
        private PlcDataType _dataType = PlcDataType.Bool;
        private Timer _updateTimer;

        // 当前值
        private bool _currentBoolValue = false;
        private short _currentShortValue = 0;

        // 颜色映射 (Bool)
        private Color _colorTrue = Color.FromArgb(50, 205, 50);   // 亮绿色
        private Color _colorFalse = Color.FromArgb(128, 128, 128); // 灰色

        // 颜色映射 (Short)
        private short[] _stateValues = new short[] { 0, 1, 2 };
        private Color[] _stateColors = new Color[] { Color.FromArgb(128, 128, 128), Color.FromArgb(50, 205, 50), Color.FromArgb(220, 20, 60) };

        // 外观配置
        private int _lightRadius = 12; // 默认与 IndicatorButtonControl 相同

        // 委托
        private ReadBoolAddressAsyncDelegate _readBoolAddressMethod;
        private ReadShortAddressAsyncDelegate _readShortAddressMethod;
        #endregion

        #region 委托定义
        public delegate Task<bool> ReadBoolAddressAsyncDelegate(string address);
        public delegate Task<short> ReadShortAddressAsyncDelegate(string address);
        #endregion

        #region 事件定义
        public event EventHandler<bool> BoolValueChanged;
        public event EventHandler<short> ShortValueChanged;
        #endregion

        #region 公共属性 (数据与控制)
        [Category("控制设置"), Description("PLC读取的数据类型 (Bool 或 Short)")]
        public PlcDataType DataType
        {
            get => _dataType;
            set { _dataType = value; Invalidate(); }
        }

        [Category("控制设置"), Description("读取的PLC地址")]
        public string ValueAddress
        {
            get => _valueAddress;
            set => _valueAddress = value ?? "";
        }

        [Category("控制设置"), Description("是否启用后台定时读取")]
        public bool EnablePlcComm
        {
            get => _enablePlcComm;
            set => _enablePlcComm = value;
        }

        [Browsable(false)]
        public ReadBoolAddressAsyncDelegate ReadBoolAddressMethod
        {
            get => _readBoolAddressMethod;
            set => _readBoolAddressMethod = value;
        }

        [Browsable(false)]
        public ReadShortAddressAsyncDelegate ReadShortAddressMethod
        {
            get => _readShortAddressMethod;
            set => _readShortAddressMethod = value;
        }
        #endregion

        #region 公共属性 (状态与映射)
        [Category("状态设置 - Bool模式"), Description("当读取值为 True 时的指示灯颜色")]
        public Color ColorTrue
        {
            get => _colorTrue;
            set { _colorTrue = value; Invalidate(); }
        }

        [Category("状态设置 - Bool模式"), Description("当读取值为 False 时的指示灯颜色")]
        public Color ColorFalse
        {
            get => _colorFalse;
            set { _colorFalse = value; Invalidate(); }
        }

        [Category("状态设置 - Short模式"), Description("Short 状态值序列数组 (如 0, 1, 2)")]
        public short[] StateValues
        {
            get => _stateValues;
            set { _stateValues = value; Invalidate(); }
        }

        [Category("状态设置 - Short模式"), Description("与 Short 值对应的颜色序列数组")]
        public Color[] StateColors
        {
            get => _stateColors;
            set { _stateColors = value; Invalidate(); }
        }

        [Browsable(false)]
        public bool CurrentBoolValue
        {
            get => _currentBoolValue;
            set
            {
                if (_currentBoolValue != value)
                {
                    _currentBoolValue = value;
                    Invalidate();
                    BoolValueChanged?.Invoke(this, _currentBoolValue);
                }
            }
        }

        [Browsable(false)]
        public short CurrentShortValue
        {
            get => _currentShortValue;
            set
            {
                if (_currentShortValue != value)
                {
                    _currentShortValue = value;
                    Invalidate();
                    ShortValueChanged?.Invoke(this, _currentShortValue);
                }
            }
        }
        #endregion

        #region 公共属性 (外观)
        [Category("外观设置"), Description("指示灯的半径大小")]
        public int LightRadius
        {
            get => _lightRadius;
            set
            {
                if (value > 0 && _lightRadius != value)
                {
                    _lightRadius = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region 构造函数
        public PlcIndicatorLightControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            // 默认尺寸设为刚好包裹指示灯的大小
            this.Size = new Size(30, 30);
            this.BackColor = Color.Transparent; // 彻底透明

            _updateTimer = new Timer();
            _updateTimer.Interval = 500;
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "PlcIndicatorLightControl";
            this.ResumeLayout(false);
        }
        #endregion

        #region 重写重绘事件
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // 获取当前颜色
            Color lightColor = GetCurrentLightColor();

            // 计算灯的矩形，使其始终在控件中心
            int size = _lightRadius * 2;
            int x = (Width - size) / 2;
            int y = (Height - size) / 2;
            Rectangle rect = new Rectangle(x, y, size, size);

            // 1. 绘制指示灯外圈 (与 IndicatorButtonControl 风格保持一致)
            using (Pen borderPen = new Pen(Color.FromArgb(100, 100, 100), 1.5f))
            {
                g.DrawEllipse(borderPen, rect);
            }

            // 2. 绘制指示灯内部底色
            rect.Inflate(-2, -2);
            using (SolidBrush brush = new SolidBrush(lightColor))
            {
                g.FillEllipse(brush, rect);
            }

            // 3. 绘制高光效果（当信号被激活时显示，提升质感）
            // 逻辑：Bool为true时显示高光；Short为非0时(通常0为暗/停机)显示高光。
            bool showHighlight = (_dataType == PlcDataType.Bool && _currentBoolValue) ||
                                 (_dataType == PlcDataType.Short && _currentShortValue != 0);

            if (showHighlight)
            {
                Rectangle highlightRect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width / 3, rect.Height / 3);
                using (SolidBrush highlightBrush = new SolidBrush(Color.FromArgb(180, Color.White)))
                {
                    g.FillEllipse(highlightBrush, highlightRect);
                }
            }
        }

        /// <summary>
        /// 获取当前应该显示的灯光颜色
        /// </summary>
        private Color GetCurrentLightColor()
        {
            if (_dataType == PlcDataType.Bool)
            {
                return _currentBoolValue ? _colorTrue : _colorFalse;
            }
            else // Short 模式
            {
                if (_stateValues != null && _stateColors != null)
                {
                    for (int i = 0; i < _stateValues.Length; i++)
                    {
                        if (_stateValues[i] == _currentShortValue)
                        {
                            // 防越界保护
                            if (i < _stateColors.Length)
                                return _stateColors[i];
                        }
                    }
                }
                // 如果没有匹配项，返回灰色默认状态
                return _colorFalse;
            }
        }
        #endregion

        #region PLC 通讯更新
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (_enablePlcComm)
            {
                _ = UpdateValueFromAddressAsync();
            }
        }

        private async Task UpdateValueFromAddressAsync()
        {
            if (string.IsNullOrEmpty(_valueAddress)) return;

            try
            {
                if (_dataType == PlcDataType.Bool && _readBoolAddressMethod != null)
                {
                    bool val = await _readBoolAddressMethod(_valueAddress);
                    if (_currentBoolValue != val)
                    {
                        if (this.InvokeRequired)
                            this.BeginInvoke(new Action(() => CurrentBoolValue = val));
                        else
                            CurrentBoolValue = val;
                    }
                }
                else if (_dataType == PlcDataType.Short && _readShortAddressMethod != null)
                {
                    short val = await _readShortAddressMethod(_valueAddress);
                    if (_currentShortValue != val)
                    {
                        if (this.InvokeRequired)
                            this.BeginInvoke(new Action(() => CurrentShortValue = val));
                        else
                            CurrentShortValue = val;
                    }
                }
            }
            catch (Exception)
            {
                // 忽略异常或记录
            }
        }
        #endregion

        #region 图形辅助方法
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _updateTimer?.Stop();
                _updateTimer.Tick -= UpdateTimer_Tick;
                _updateTimer?.Dispose();
                _updateTimer = null;
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}