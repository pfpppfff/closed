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
    public partial class CustomSwithBtn : UserControl
    {
        private bool _readEnable = false;
        // 定义写地址和读地址
        private string _writeAddress;
        private bool _readAddress = false;
        private string _readName = "name";
        private int _intWriteValue = 0;
       
        private Font _font = new Font("宋体", 9.0f);
        // 定义当前状态
        private bool _currentState = false;

        // 按钮的控件
        private UIButton _button;
       

        // 自定义事件参数类
        public class ButtonClickedEventArgs : EventArgs
        {
            public string ReadAddress { get; set; }

            public ButtonClickedEventArgs(string readAddress)
            {
                ReadAddress = readAddress;
            }
        }
        // 定义事件 A，使用自定义事件参数
        public event EventHandler<ButtonClickedEventArgs> ButtonClicked;
        public CustomSwithBtn()
        {
            InitializeComponent();
            InitializeComponent1();
          
        }

        private void InitializeComponent1()
        {
            _button = Btn;
         
            _button.Click += Button_Click;
           // _button.FillColor = Color.DarkGray;

        }
        private void Button_Click(object sender, EventArgs e)
        {
            ToggleState();
            //UpdateLightColor();
            OnButtonClicked(new ButtonClickedEventArgs(_readName));
        }

        // 切换状态
        private void ToggleState()
        {
            _currentState = !_currentState;
            //WriteToAddress(_currentState);
            //BoolWrite(_readName);

            switch (_typeSel)
            {
                case ControlModeOptions.boolSwith:
                    OpcUa.BoolSwith(_writeAddress);
                    break;
                case ControlModeOptions.intSwith:
                    OpcUa.IntSwith(_writeAddress, (Int16)_intWriteValue);
                    break;
                case ControlModeOptions.intWrite:
                    OpcUa.IntWrite(_writeAddress, (Int16)_intWriteValue);
                    break;
            }

        }


        // 触发事件 A 的方法
        protected virtual void OnButtonClicked(ButtonClickedEventArgs e)
        {
            ButtonClicked?.Invoke(this, e);
        }
        private static readonly Color DefaultHoverColor = Color.FromArgb(115, 179, 255); // 预先计算的颜色值

        // 更新灯的颜色
        private void UpdateLightColor()
        {
            //if (_currentState)
            //{
            //   _button.FillColor = Color.Green;
            //    _button.FillHoverColor = Color.Green;
            //}
            //else
            //{
            //    _button.FillColor = Color.DarkGray;
            //    _button.FillHoverColor = Color.FromArgb(115, 179, 255);
            //}
            var newFillColor = _currentState ? Color.Green : Color.LightSkyBlue;
            var newFillHoverColor = _currentState ? Color.Green : DefaultHoverColor;
            var newFontColor = _currentState ? Color.White : Color.DimGray;

            if (_button.FillColor != newFillColor)
            {
                _button.FillColor = newFillColor;
            }

            if (_button.FillHoverColor != newFillHoverColor)
            {
                _button.FillHoverColor = newFillHoverColor;
            }
            if (_button.ForeColor != newFontColor)
            {
                _button.ForeColor = newFontColor;
            }
        }



        // 从外部更新状态
        public void UpdateBoolStateFromExternal(bool newState)
        {
            if (_typeSel == ControlModeOptions.boolSwith)
            {
                _currentState = newState;
                UpdateLightColor();
            }

        }
        // 从外部更新状态
        public void UpdateIntStateFromExternal(int newState)
        {
            if (_typeSel == ControlModeOptions.intSwith)
            {
                if (newState == _intWriteValue)
                {
                    _currentState = true;
                }
                else
                {
                    _currentState = false;
                }

                UpdateLightColor();
            }

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
            [Description("开关量切换")]
            boolSwith,
            [Description("int切换")]
            intSwith,
            [Description("int写入")]
            intWrite
        }

        private ControlModeOptions _typeSel = ControlModeOptions.boolSwith;
        // 属性：类型选择
        [Category("Custom")]
        [Description("控制类型选择，1-布尔切换，2-int切换，2-int写入")]
        public ControlModeOptions TypeSel
        {
            get => _typeSel;
            set
            {
                _typeSel = value;

            }
        }


        // 属性：写地址
        [Category("Custom")]
        [Description("int类型写入值")]
        public int IntWriteValue
        {
            get => _intWriteValue;
            set
            {
                _intWriteValue = value;
               
            }
        }

        // 属性：写地址
        [Category("Custom")]
        [Description("写入地址")]
        public string WriteAddress
        {
            get => _writeAddress;
            set
            {
                _writeAddress = value;
              
            }
        }

        // 属性：读地址
        [Category("Custom")]
        [Description("灯信号读取地址")]
        public bool ReadAddress
        {
            get => _readAddress;
            set
            {
                _readAddress = value;

            }
        }
        // 属性：读地址
        [Category("Custom")]
        [Description("控件名")]
        public string BtnName
        {
            get => _readName;
            set
            {
                _readName = value;
                _button.Text = _readName;
            }
        }
        [Category("Custom")]
        [Description("按钮字体")]
        public Font TxtFont
        {
            get => _font;
            set
            {
                _font = value;
                _button.Font = value;
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // 如果需要额外的缩放逻辑，可以在这里实现
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
    }
}
