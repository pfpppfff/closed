using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;
using System.Reflection.Emit;

namespace UserC
{
    public partial class CustomUIButtonLight: UserControl
    {
        // 定义写地址和读地址
        private bool _writeAddress = false;
        private bool _readAddress = false;
        private string _readName = "name";
        private Font _font= new Font("宋体", 9.0f);
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
        public CustomUIButtonLight()
        {
            InitializeComponent();
            InitializeComponent1();
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {

        }
        private void InitializeComponent1()
        {
            // 初始化按钮
            _button = new UIButton
            {
                Text = _readName,
                Font = _font,
                Dock = DockStyle.Fill,
                AutoSize = true,
                Margin = new Padding(3)
            };
            _button.Click += Button_Click;

            // 初始化灯
            _light = new UILight
            {
                BackColor = UIColor.Transparent,
                OnCenterColor = UIColor.Gray,
                OnColor = UIColor.Gray,
                Dock = DockStyle.Fill,
                Margin = new Padding(3)
            };

            // 使用 TableLayoutPanel 管理布局
            var layout = new TableLayoutPanel
            {
                ColumnCount = 2, // 设置两列
                RowCount = 1,    // 设置一行
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            // 设置列样式
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // 按钮占 50%
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F)); // 灯占 50%

            // 添加控件到布局
            layout.Controls.Add(_button, 1, 0); // 按钮放在第一列
            layout.Controls.Add(_light, 0, 0);  // 灯放在第二列

            // 将布局添加到控件
            this.Controls.Add(layout);

            // 设置控件大小
            //this.Size = new Size(300, 80); // 默认大小
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
        }

       
            // 触发事件 A 的方法
       protected virtual void OnButtonClicked(ButtonClickedEventArgs e)
        {
            ButtonClicked?.Invoke(this, e);
        }
      
        // 更新灯的颜色
        private void UpdateLightColor()
        {
            if (_currentState)
            {
                _light.OnCenterColor = UIColor.LayuiGreen;
                _light.OnColor = UIColor.Green;
                _light.State= UILightState.On;
            }
            else
            {
                _light.OnCenterColor = UIColor.Gray;
                _light.OnColor = UIColor.Gray;
                _light.State = UILightState.Off;
            }
        }

        //// 写入地址
        //private void WriteToAddress(bool state)
        //{
        //    Console.WriteLine($"Writing to address {_writeAddress}: {state}");
        //    // 模拟写入逻辑
        //}

        // 从外部更新状态
        public void UpdateStateFromExternal(bool newState)
        {
            //if (_currentState != newState)
            //{
            //    _currentState = newState;
            //    UpdateLightColor();
            //}
            _currentState = newState;
            UpdateLightColor();
        }

        //// 读取地址并更新状态
        //public void ReadFromAddress()
        //{
        //    bool readValue = GetReadAddressValue();
        //    if (readValue != _currentState)
        //    {
        //        _currentState = readValue;
        //        UpdateLightColor();
        //    }
        //}

        ////获取读地址的值（模拟）
        //private bool GetReadAddressValue()
        //{
        //    // 模拟从读地址获取值
        //    Console.WriteLine($"Reading from address {_readAddress}: {_currentState}");
        //    return _currentState;
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
                Console.WriteLine($"Write address set to: {_writeAddress}");
            }
        }

        // 属性：读地址
        [Category("Custom")]
        [Description("The read address for the light.")]
        public bool ReadAddress
        {
            get => _readAddress;
            set
            {
                _readAddress = value;
                Console.WriteLine($"Read address set to: {_readAddress}");
            }
        }
        // 属性：读地址
        [Category("Custom")]
        [Description("The read address for the light.")]
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
        [Description("btnfont")]
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
