using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 控件拖拽功能;

namespace UserC
{
    public partial class CustomGateValveBtn : UserControl
    {
        private bool _readEnable = false;
        // 定义写地址和读地址
        private int _updatlMode=1;
        private string _writeCloseAddress ;
        private string _writeOpenAddress;
        private bool _readOpenLimAddress = false;
        private bool _readCloseLimAddress = false;
        private bool _readOpenAddress = false;
        private bool _readCloseAddress = false;
        private string _readOpenName = "nameOpen";
        private string _readCloseName = "nameClose";
        private Font _font = new Font("宋体", 9.0f);
        // 定义当前状态
        private bool _currentOpenLimState = false;
        private bool _currentCloseLimState = false;
        private bool _currentOpenState = false;
        private bool _currentCloseState = false;
        // 按钮和灯的控件
        private UIButton _buttonClose;
        private UIButton _buttonOpen;
        private UILight _lightOpenLimt;
        private UILight _lightCloseLimt;

        //// 自定义事件参数类
        //public class MouseActionEventArgs : EventArgs
        //{
        //    public string ReadAddress { get; set; }
        //    public string ValveDir { get; set; }
        //    public MouseActionEventArgs(string readAddress, string valveDir)
        //    {
        //        ReadAddress = readAddress;
        //        ValveDir = valveDir;
        //    }
        //}
        //// 定义事件 A，使用自定义事件参数
        //// 定义 MouseDown 和 MouseUp 事件
        //public event EventHandler<MouseActionEventArgs> MouseDownUpEvent;
        //public event EventHandler<MouseActionEventArgs> MouseUpEvent;
        public CustomGateValveBtn()
        {
            InitializeComponent();
            InitializeComponent1();
        }

        private void InitializeComponent1()
        {
            // 初始化按钮
            _buttonOpen = BtnOpen;
            _buttonOpen.Text = _readOpenName;
            _buttonOpen.Font = _font;
            _buttonClose = BtnClose;
            _buttonOpen.Text = _readOpenName;
            _buttonClose.Font = _font;


          
         

        

            // 初始化灯
            _lightOpenLimt = UI_Open;

            _lightOpenLimt.BackColor = UIColor.Transparent;
            _lightOpenLimt.OnCenterColor = UIColor.Gray;
            _lightOpenLimt.OnColor = UIColor.Gray;
       
         

            _lightCloseLimt = UI_Close;

            _lightCloseLimt.BackColor = UIColor.Transparent;
            _lightCloseLimt.OnCenterColor = UIColor.Gray;
            _lightCloseLimt.OnColor = UIColor.Gray;

          
        }
        // 更新事件绑定方法
        private void UpdateEventBinding()
        {
            // 先移除所有事件绑定
            _buttonOpen.Click -= ButtonOpen_Click;
            _buttonClose.Click -= ButtonClose_Click;
            _buttonOpen.MouseDown -= ButtonOpen_MouseDown;  
            _buttonOpen.MouseUp -= _ButtonOpen_MouseUp;

            _buttonClose.MouseDown -= _ButtonClose_MouseDown;
            _buttonClose.MouseUp -= _ButtonClose_MouseUp;
           
            switch (_controlMode)
            {
                case ControlModeOptions.SetMode:
                    _buttonOpen.Click += ButtonOpen_Click;
                    _buttonClose.Click += ButtonClose_Click;
                    break;
                case ControlModeOptions.JogMode:
                    _buttonOpen.MouseDown += ButtonOpen_MouseDown;
                    _buttonOpen.MouseUp += _ButtonOpen_MouseUp;
                    _buttonClose.MouseDown += _ButtonClose_MouseDown;
                    _buttonClose.MouseUp += _ButtonClose_MouseUp;
                    break;

            }
        }

      
        private void ButtonOpen_Click(object sender, EventArgs e)
        {
            if(!_readEnable)
                return;
            OpcUa.BoolSwith(_writeOpenAddress);
          
        }
        private void ButtonClose_Click(object sender, EventArgs e)
        {
            if (!_readEnable)
                return;
            OpcUa.BoolSwith(_writeCloseAddress);
        }
        private void _ButtonClose_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_readEnable)
                return;
            OpcUa.BoolWrite(_writeCloseAddress, false);
        }

        private void _ButtonClose_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_readEnable)
                return;
            OpcUa.BoolWrite(_writeCloseAddress, true);
        }

        private void _ButtonOpen_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_readEnable)
                return;
            OpcUa.BoolWrite(_writeOpenAddress, false);
        }

        private void ButtonOpen_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_readEnable)
                return;
            OpcUa.BoolWrite(_writeOpenAddress, true);
        }

        // 切换状态
        private void ToggleState()
        {
            //_currentOpenState = !_currentOpenState;
            //WriteToAddress(_currentOpenState);

        }


        private static readonly Color DefaultHoverColor = Color.FromArgb(115, 179, 255);
        // 按钮操作更新灯的颜色
        private void UpdateLightColor()
        {
            //if (_currentOpenLimState)
            //{
            //    _lightOpenLimt.OnCenterColor = UIColor.LayuiGreen;
            //    _lightOpenLimt.OnColor = UIColor.Green;
            //    _lightOpenLimt.State = UILightState.On;
            //}
            //else
            //{
            //    _lightOpenLimt.OnCenterColor = UIColor.Gray;
            //    _lightOpenLimt.OnColor = UIColor.Gray;
            //    _lightOpenLimt.State = UILightState.Off;
            //}
            //if (_currentCloseLimState)
            //{
            //    _lightCloseLimt.OnCenterColor = UIColor.LayuiRed;
            //    _lightCloseLimt.OnColor = UIColor.Red;
            //    _lightCloseLimt.State = UILightState.On;
            //}
            //else
            //{
            //    _lightCloseLimt.OnCenterColor = UIColor.Gray;
            //    _lightCloseLimt.OnColor = UIColor.Gray;
            //    _lightCloseLimt.State = UILightState.Off;
            //}
            //if (_currentOpenState)
            //{
            //    _buttonOpen.FillColor = Color.Green;
            //    _buttonOpen.FillHoverColor = Color.Green;
            //}
            //else
            //{
            //    _buttonOpen.FillColor = Color.DarkGray;
            //    _buttonOpen.FillHoverColor = Color.FromArgb(115, 179, 255);
            //}
            //if (_currentCloseState)
            //{
            //    _buttonClose.FillColor = Color.Green;
            //    _buttonClose.FillHoverColor = Color.Green;
            //}
            //else
            //{
            //    _buttonClose.FillColor = Color.DarkGray;
            //    _buttonClose.FillHoverColor = Color.FromArgb(115, 179, 255);
            //}
         
            switch (_updatlMode)
            {
                case 1:
                    var newOpenFillColor = _currentOpenState ? Color.Green : Color.LightSkyBlue;
                    var newOpenFillHoverColor = _currentOpenState ? Color.Green : DefaultHoverColor;
                    var newOpenFontColor = _currentOpenState ? Color.White : Color.DimGray;

                    if (_buttonOpen.FillColor != newOpenFillColor)
                    {
                        _buttonOpen.FillColor = newOpenFillColor;
                    }

                    if (_buttonOpen.FillHoverColor != newOpenFillHoverColor)
                    {
                        _buttonOpen.FillHoverColor = newOpenFillHoverColor;
                    }
                    if (_buttonOpen.ForeColor != newOpenFontColor)
                    {
                        _buttonOpen.ForeColor = newOpenFontColor;
                    }
                    var newCloseFillColor = _currentCloseState ? Color.Green : Color.LightSkyBlue;
                    var newCloseFillHoverColor = _currentCloseState ? Color.Green : DefaultHoverColor;
                    var newCloseFontColor = _currentCloseState ? Color.White : Color.DimGray;

                    if (_buttonClose.FillColor != newCloseFillColor)
                    {
                        _buttonClose.FillColor = newCloseFillColor;
                    }

                    if (_buttonClose.FillHoverColor != newCloseFillHoverColor)
                    {
                        _buttonClose.FillHoverColor = newCloseFillHoverColor;
                    }
                    if (_buttonClose.ForeColor != newCloseFontColor)
                    {
                        _buttonClose.ForeColor = newCloseFontColor;
                    }
                    //
                    var newOpenOnCenterColor = _currentOpenLimState ? UIColor.LayuiGreen : UIColor.Gray;
                    var newOpenOnColor = _currentOpenLimState ? UIColor.Green : UIColor.Gray;
                    var newOpenState = _currentOpenLimState ? UILightState.On : UILightState.Off;

                    if (_lightOpenLimt.OnCenterColor != newOpenOnCenterColor)
                    {
                        _lightOpenLimt.OnCenterColor = newOpenOnCenterColor;
                    }

                    if (_lightOpenLimt.OnColor != newOpenOnColor)
                    {
                        _lightOpenLimt.OnColor = newOpenOnColor;
                    }
                    if (_lightOpenLimt.State != newOpenState)
                    {
                        _lightOpenLimt.State = newOpenState;
                    }
                    //
                    var newCloseOnCenterColor = _currentCloseLimState ? UIColor.LayuiRed : UIColor.Gray;
                    var newCloseOnColor = _currentCloseLimState ? UIColor.Red : UIColor.Gray;
                    var newCloseState = _currentCloseLimState ? UILightState.On : UILightState.Off;

                    if (_lightCloseLimt.OnCenterColor != newCloseOnCenterColor)
                    {
                        _lightCloseLimt.OnCenterColor = newCloseOnCenterColor;
                    }

                    if (_lightCloseLimt.OnColor != newCloseOnColor)
                    {
                        _lightCloseLimt.OnColor = newCloseOnColor;
                    }
                    if (_lightCloseLimt.State != newCloseState)
                    {
                        _lightCloseLimt.State = newCloseState;
                    }
                    break;
                case 2:

                    var newOpenOnCenterColor1 = _currentOpenLimState ? UIColor.LayuiGreen : UIColor.Gray;
                    var newOpenOnColor1 = _currentOpenLimState ? UIColor.Green : UIColor.Gray;
                    var newOpenState1 = _currentOpenLimState ? UILightState.On : UILightState.Off;

                    if (_lightOpenLimt.OnCenterColor != newOpenOnCenterColor1)
                    {
                        _lightOpenLimt.OnCenterColor = newOpenOnCenterColor1;
                    }

                    if (_lightOpenLimt.OnColor != newOpenOnColor1)
                    {
                        _lightOpenLimt.OnColor = newOpenOnColor1;
                    }
                    if (_lightOpenLimt.State != newOpenState1)
                    {
                        _lightOpenLimt.State = newOpenState1;
                    }
                    //
                    var newCloseOnCenterColor1 = _currentCloseLimState ? UIColor.LayuiRed : UIColor.Gray;
                    var newCloseOnColor1 = _currentCloseLimState ? UIColor.Red : UIColor.Gray;
                    var newCloseState1 = _currentCloseLimState ? UILightState.On : UILightState.Off;

                    if (_lightCloseLimt.OnCenterColor != newCloseOnCenterColor1)
                    {
                        _lightCloseLimt.OnCenterColor = newCloseOnCenterColor1;
                    }

                    if (_lightCloseLimt.OnColor != newCloseOnColor1)
                    {
                        _lightCloseLimt.OnColor = newCloseOnColor1;
                    }
                    if (_lightCloseLimt.State != newCloseState1)
                    {
                        _lightCloseLimt.State = newCloseState1;
                    }
                    break;
            }
          
        }

     

        // 从外部更新状态
        public void UpdateStateFromExternal(bool newOpenLimState, bool newCloseLimState, bool newOpenState, bool newCloseState)
        {
            if (!_readEnable)
                return;
            _currentOpenLimState = newOpenLimState;
            _currentCloseLimState = newCloseLimState;
            _currentOpenState = newOpenState;
            _currentCloseState = newCloseState;         
            UpdateLightColor();
        }

        [Browsable(true)]
        [Category("Custom")]
        [Description("读取使能")]
        public bool ReadEnable
        {
            get => _readEnable;
            set
            {
                _readEnable = value;
                //OnReadEnableChanged(EventArgs.Empty);
            }
        }
        public enum ControlModeOptions
        {
            [Description("setbit")]
            SetMode,
            [Description("jog")]
            JogMode
        }
        // 属性：写地址
        [Category("Custom")]
        [Description("反馈模式选择,1-全反馈，2-按钮不反馈")]
        public int UpdataMode
        {
            get => _updatlMode;
            set
            {
                _updatlMode = value;
                UpdateEventBinding();
            }
        }
        private ControlModeOptions _controlMode = ControlModeOptions.SetMode;
        // 属性：写地址
        [Category("Custom")]
        [Description("控制模式选择")]
        public ControlModeOptions ControlMode
        {
            get => _controlMode;
            set
            {
                _controlMode = value;
                UpdateEventBinding();
            }
        }

        // 属性：写地址
        [Category("Custom")]
        [Description("写入关变量地址")]
        public string WriteCloseAddress
        {
            get => _writeCloseAddress;
            set
            {
                _writeCloseAddress = value;

            }
        }
        // 属性：写地址
        [Category("Custom")]
        [Description("写入开变量地址")]
        public string WriteOpenAddress
        {
            get => _writeOpenAddress;
            set
            {
                _writeOpenAddress = value;

            }
        }
        // 属性：读地址
        [Category("Custom")]
        [Description("The read address for the OpenLimitlight.")]
        public bool ReadOpenLimAddress
        {
            get => _readOpenLimAddress;
            set
            {
                _readOpenLimAddress = value;

            }
        }
        // 属性：读地址
        [Category("Custom")]
        [Description("The read address for the CloseLimitlight.")]
        public bool ReadCloseLimAddress
        {
            get => _readCloseLimAddress;
            set
            {
                _readCloseLimAddress = value;

            }
        }
        // 属性：读地址
        [Category("Custom")]
        [Description("The read address for the BtnOpen.")]
        public bool ReadOpenAddress
        {
            get => _readOpenAddress;
            set
            {
                _readOpenAddress = value;

            }
        }
        // 属性：读地址
        [Category("Custom")]
        [Description("The read address for the BtnClose.")]
        public bool ReadCloseAddress
        {
            get => _readCloseAddress;
            set
            {
                _readCloseAddress = value;

            }
        }
        // 属性：按钮名
        [Category("Custom")]
        [Description("The btopen name")]
        public string BtnOpenName
        {
            get => _readOpenName;
            set
            {
                _readOpenName = value;
                _buttonOpen.Text = _readOpenName;
            }
        }

        [Category("Custom")]
        [Description("The btclose name")]
        public string BtnCloseName
        {
            get => _readCloseName;
            set
            {
                _readCloseName = value;
                _buttonClose.Text = _readCloseName;
            }
        }
        [Category("Custom")]
        [Description("btnfont")]
        public Font TxtFont
        {
            get => _font;
            set
            {
                _font = value;
                _buttonOpen.Font = value;
                _buttonClose.Font = value;
            }
        }
        public event EventHandler ReadEnableChanged;
        // 触发事件的方法
        protected virtual void OnReadEnableChanged(EventArgs e)
        {
            ReadEnableChanged?.Invoke(this, e);
        }
        private void OnReadEnableChangedHandler(object sender, EventArgs e)
        {
           
        }
        private void CustomGateValveBtn_Load(object sender, EventArgs e)
        {
            //this.ReadEnableChanged += OnReadEnableChangedHandler;
        }
    }
}
