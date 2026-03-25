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
using static UserC.CustomUIGateValve;

namespace UserC
{
    public partial class CustomUIGateBtn : UserControl
    {

        // 定义写地址和读地址
        private bool _writeAddress = false;
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

        // 自定义事件参数类
        public class MouseActionEventArgs : EventArgs
        {
            public string ReadAddress { get; set; }
            public string ValveDir { get; set; }
            public MouseActionEventArgs(string readAddress, string valveDir)
            {
                ReadAddress = readAddress;
                ValveDir = valveDir;
            }
        }
        // 定义事件 A，使用自定义事件参数
        // 定义 MouseDown 和 MouseUp 事件
        public event EventHandler<MouseActionEventArgs> MouseDownUpEvent;
        //public event EventHandler<MouseActionEventArgs> MouseUpEvent;
        public CustomUIGateBtn()
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
            //Dock = DockStyle.Fill,
            //    Margin = new Padding(3)
           
            //  _buttonOpen.Click += Button_Click;
            _buttonOpen.MouseDown += ButtonOpen_MouseDown;  // 绑定 MouseDown 事件
            _buttonOpen.MouseUp += _ButtonOpen_MouseUp;

            _buttonClose = BtnClose;

            _buttonClose.Text = _readCloseName;
            _buttonClose.Font = _font;
            //Dock = DockStyle.Fill,
            //    Margin = new Padding(3)
           

            _buttonClose.MouseDown += _ButtonClose_MouseDown;
            _buttonClose.MouseUp += _ButtonClose_MouseUp;

            // 初始化灯
            _lightOpenLimt = uiLightOpen;
            //{
                _lightOpenLimt.BackColor = UIColor.Transparent;
            _lightOpenLimt.OnCenterColor = UIColor.Gray;
            _lightOpenLimt.OnColor = UIColor.Gray;
            //Dock = DockStyle.Fill,
            //Margin = new Padding(3)
            //};

            _lightCloseLimt = uiLightClose;

            _lightCloseLimt.BackColor = UIColor.Transparent;
            _lightCloseLimt.OnCenterColor = UIColor.Gray;
            _lightCloseLimt.OnColor = UIColor.Gray;



            //// 使用 TableLayoutPanel 管理布局
            //var layout = new TableLayoutPanel
            //{
            //    ColumnCount = 2, // 设置两列
            //    RowCount = 2,    // 设置2行
            //    Dock = DockStyle.Fill,
            //    Margin = new Padding(0),
            //    Padding = new Padding(0)
            //};

            //// 设置列样式
            //layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // 按钮占 50%
            //layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F)); // 灯占 50%

            //layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            //layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            //// 添加控件到布局
            //layout.Controls.Add(_buttonOpen, 1, 0); // 按钮放在第一列
            //layout.Controls.Add(_lightOpenLimt, 0, 0);  // 灯放在第二列
            //layout.Controls.Add(_buttonClose, 1, 1);
            //layout.Controls.Add(_lightCloseLimt, 0, 1);

            //// 将布局添加到控件
            //this.Controls.Add(layout);

            ////// 设置控件大小
            ////this.Size = new Size(300, 200); // 默认大小
        }

        private void _ButtonClose_MouseUp(object sender, MouseEventArgs e)
        {
            OnMouseDownUp(new MouseActionEventArgs(_readCloseName, "Up"));
        }

        private void _ButtonClose_MouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDownUp(new MouseActionEventArgs(_readCloseName, "Down"));
        }

        private void _ButtonOpen_MouseUp(object sender, MouseEventArgs e)
        {
            OnMouseDownUp(new MouseActionEventArgs(_readOpenName, "Up"));
        }

        private void ButtonOpen_MouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDownUp(new MouseActionEventArgs(_readOpenName, "Down"));
        }



        //private void Button_Click(object sender, EventArgs e)
        //{
        //    ToggleState();
        //    UpdateLightColor();
        //}

        // 切换状态
        private void ToggleState()
        {
            //_currentOpenState = !_currentOpenState;
            //WriteToAddress(_currentOpenState);

        }

        // 触发 MouseDownEvent 的方法
        protected virtual void OnMouseDownUp(MouseActionEventArgs e)
        {
            MouseDownUpEvent?.Invoke(this, e);
        }

        // 触发 MouseUpEvent 的方法
        //protected virtual void OnMouseUp(MouseActionEventArgs e)
        //{
        //    MouseDownUpEvent?.Invoke(this, e);
        //}


        // 按钮操作更新灯的颜色
        private void UpdateLightColor()
        {
            if (_currentOpenLimState)
            {
                _lightOpenLimt.OnCenterColor = UIColor.LayuiGreen;
                _lightOpenLimt.OnColor = UIColor.Green;
                _lightOpenLimt.State = UILightState.On;
            }
            else
            {
                _lightOpenLimt.OnCenterColor = UIColor.Gray;
                _lightOpenLimt.OnColor = UIColor.Gray;
                _lightOpenLimt.State = UILightState.Off;
            }
            if (_currentCloseLimState)
            {
                _lightCloseLimt.OnCenterColor = UIColor.LayuiRed;
                _lightCloseLimt.OnColor = UIColor.Red;
                _lightCloseLimt.State = UILightState.On;
            }
            else
            {
                _lightCloseLimt.OnCenterColor = UIColor.Gray;
                _lightCloseLimt.OnColor = UIColor.Gray;
                _lightCloseLimt.State = UILightState.Off;
            }
            if (_currentOpenState)
            {
                _buttonOpen.FillColor = Color.Green;
                _buttonOpen.FillHoverColor = Color.Green;
            }
            else
            {
                _buttonOpen.FillColor = Color.DarkGray;
                _buttonOpen.FillHoverColor = Color.FromArgb(115, 179, 255);
            }
            if (_currentCloseState)
            {
                _buttonClose.FillColor = Color.Green;
                _buttonClose.FillHoverColor = Color.Green;
            }
            else
            {
                _buttonClose.FillColor = Color.DarkGray;
                _buttonClose.FillHoverColor = Color.FromArgb(115, 179, 255);
            }
        }

        // 写入地址
        private void WriteToAddress(bool state)
        {
            Console.WriteLine($"Writing to address {_writeAddress}: {state}");
            // 模拟写入逻辑
        }

        // 从外部更新状态
        public void UpdateStateFromExternal(bool newOpenLimState, bool newCloseLimState, bool newOpenState, bool newCloseState)
        {
            //if (_currentOpenLimState != newOpenLimState)
            //{
            //    _currentOpenLimState = newOpenLimState;
            //    UpdateLightColor();
            //}
            //if (_currentCloseLimState != newCloseLimState)
            //{
            //    _currentCloseLimState = newCloseLimState;
            //    UpdateLightColor();
            //}
            //if (_currentOpenState != newOpenState)
            //{
            //    _currentOpenState = newOpenState;
            //    UpdateLightColor();
            //}
            //if (_currentCloseState != newCloseState)
            //{
            //    _currentCloseState = newCloseState;
            //    UpdateLightColor();
            //}

            _currentOpenLimState = newOpenLimState;
            _currentCloseLimState = newCloseLimState;
            _currentOpenState = newOpenState;
            _currentCloseState = newCloseState;
            UpdateLightColor();
        }

        // 读取地址并更新状态
        //public void ReadFromAddress()
        //{
        //    bool readValue = GetReadAddressValue();
        //    if (readValue != _currentOpenState)
        //    {
        //        _currentOpenState = readValue;
        //        UpdateLightColor();
        //    }
        //}

        ////获取读地址的值（模拟）
        //private bool GetReadAddressValue()
        //{
        //    // 模拟从读地址获取值
        //    Console.WriteLine($"Reading from address {_readOpenAddress}: {_currentOpenState}");
        //    return _currentOpenState;
        //}

        // 属性：写地址
        [Category("Custom")]
        [Description("btnname")]
        public bool WriteAddress
        {
            get => _writeAddress;
            set
            {
                _writeAddress = value;

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
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // 如果需要额外的缩放逻辑，可以在这里实现
        }
    }
}
