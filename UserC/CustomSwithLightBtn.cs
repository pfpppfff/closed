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
using static UserC.CustomUIButtonLight;

namespace UserC
{
    public partial class CustomSwithLightBtn : UserControl
    {
        private bool _readEnable = false;
        // 定义写地址和读地址
        private string _writeAddress;
        private bool _readAddress = false;
        private string _readName = "name";
        private int _intWriteValue = 0;
        private string _typeSel="1" ;
        private Font _font = new Font("宋体", 9.0f);
        // 定义当前状态
        private bool _currentState = false;

        // 按钮和灯的控件
        private UIButton _button;
        private UILight _light;

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
        public CustomSwithLightBtn()
        {
            InitializeComponent();
            InitializeComponent1();
        }

        private void InitializeComponent1()
        {
            _button = Btn;
            _light = Lig;
            _button.Click += Button_Click;
            _light.BackColor = UIColor.Transparent;
            _light.OnCenterColor = UIColor.Gray;
            _light.OnColor = UIColor.Gray;

        }
        private void Button_Click(object sender, EventArgs e)
        {
            ToggleState();
         
            OnButtonClicked(new ButtonClickedEventArgs(_readName));
        }

        // 切换状态
        private void ToggleState()
        {
            if (!_readEnable)
                return;
            _currentState = !_currentState;
      
          switch(_typeSel)
            {
                case "1":
                    OpcUa.BoolSwith(_writeAddress);
                    break;
                case "2":
                    OpcUa.IntSwith(_writeAddress, (Int16)_intWriteValue);
                    break;
                case "3":
                    OpcUa.IntWrite(_writeAddress, (Int16)_intWriteValue);
                    break;
            }
          
        }


        // 触发事件 A 的方法
        protected virtual void OnButtonClicked(ButtonClickedEventArgs e)
        {
            ButtonClicked?.Invoke(this, e);
        }

        // 更新灯的颜色
        private void UpdateLightColor()
        {
            //if (_currentState)
            //{
            //    _light.OnCenterColor = UIColor.LayuiGreen;
            //    _light.OnColor = UIColor.Green;
            //    _light.State = UILightState.On;
            //}
            //else
            //{
            //    _light.OnCenterColor = UIColor.Gray;
            //    _light.OnColor = UIColor.Gray;
            //    _light.State = UILightState.Off;
            //}
            var newOnCenterColor = _currentState ? UIColor.LayuiGreen : UIColor.Gray;
            var newOnColor = _currentState ? UIColor.Green : UIColor.Gray;
            var newState = _currentState ? UILightState.On : UILightState.Off;
            // 只有当颜色真正发生变化时，才进行更新
            if (_light.OnCenterColor != newOnCenterColor)
            {
                _light.OnCenterColor = newOnCenterColor;
            }

            if (_light.OnColor != newOnColor)
            {
                _light.OnColor = newOnColor;
            }
            if (_light.State != newState)
            {
                _light.State = newState;
            }
        }


        private static readonly Color DefaultHoverColor = Color.FromArgb(115, 179, 255); 
        // 从外部更新状态
        public void UpdateBoolStateFromExternal(bool newState)
        {
            if (!_readEnable)
                return;
            if (_typeSel=="1")
            {
                _currentState = newState;
                UpdateLightColor();
              
             }
            
        }
        // 从外部更新状态
        public void UpdateIntStateFromExternal(int newState)
        {
            if (!_readEnable)
                return;
            if (_typeSel == "2")
            {
                if (newState==_intWriteValue)
                {
                    _currentState =true;
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
              //  OnReadEnableChanged(EventArgs.Empty);
            }
        }

        // 属性：类型选择
        [Category("Custom")]
        [Description("控制类型选择，1-布尔写入，2-int切换，3-int写入")]
        public string TypeSel
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
                // Console.WriteLine($"Write address set to: {_writeAddress}");
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
    }
}
