using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 平移法
{
    public partial class CustomButtonControl : UserControl
    {
        
        public CustomButtonControl()
        {
            InitializeComponent();
            InitializeComponent1();
            UpdateEventBinding(); // 初始化事件绑定

        }
        private Button _button;
        private int _modeSel = 1; // 默认模式为1
        private int _resultValue = 0; // 存储操作结果的值

        // 定义模式选择属性
        [Category("Custom Properties")]
        [Description("控制按钮触发的事件类型: 1=Click, 2=MouseDown")]
        public int ModeSel
        {
            get { return _modeSel; }
            set
            {
                // 确保值为1或2
                if (value != 1 && value != 2)
                {
                    throw new ArgumentException("ModeSel只能是1或2");
                }

                // 如果值发生变化，则更新模式并重新绑定事件
                if (_modeSel != value)
                {
                    _modeSel = value;
                    UpdateEventBinding();
                }
            }
        }

        // 添加结果值属性，可以从外部读取但不能设置
        [Browsable(true)]
        [Category("Custom Properties")]
        [Description("按钮操作后的结果值：Click时为1，MouseDown时为2")]
        public int ResultValue
        {
            get { return _resultValue; }
            private set { _resultValue = value; }
        }

        // 添加一个值变化事件，当ResultValue改变时触发
        public event EventHandler ResultValueChanged;

     

        private void InitializeComponent1()
        {
            // 创建按钮
            _button = new Button();
            _button.Text = "自定义按钮";
            _button.Location = new System.Drawing.Point(10, 10);
            _button.Size = new System.Drawing.Size(100, 30);

            // 添加按钮到控件
            this.Controls.Add(_button);

            // 设置控件大小
            this.Size = new System.Drawing.Size(120, 50);
        }

        // 更新事件绑定方法
        private void UpdateEventBinding()
        {
            // 先移除所有事件绑定
            _button.Click -= Button_Click;
          
            _button.MouseDown -= Button_MouseDown;
            _button.MouseUp -= Button_MouseUp;
            // 根据当前模式重新绑定事件
            if (_modeSel == 1)
            {
                _button.Click += Button_Click;
            }
            else // _modeSel == 2
            {
                _button.MouseDown += Button_MouseDown;
                _button.MouseUp += Button_MouseUp;
            }
        }

      

        // Click事件处理程序
        private void Button_Click(object sender, EventArgs e)
        {
            // 将结果值设为1
            ResultValue = 1;
            // 触发值变化事件
            MessageBox.Show("1");
            OnResultValueChanged();
        }

        // MouseDown事件处理程序
        private void Button_MouseDown(object sender, MouseEventArgs e)
        {
            // 将结果值设为2
            ResultValue = 2;
            // 触发值变化事件
           
            OnResultValueChanged();
        }
        private void Button_MouseUp(object sender, MouseEventArgs e)
        {
            ResultValue = 3;
            MessageBox.Show(ResultValue.ToString());
        }
        // 触发值变化事件的方法
        protected virtual void OnResultValueChanged()
        {
            ResultValueChanged?.Invoke(this, EventArgs.Empty);
        }

        // 公开按钮属性，以便可以自定义按钮外观
        [Browsable(true)]
        [Category("Appearance")]
        public string ButtonText
        {
            get { return _button.Text; }
            set { _button.Text = value; }
        }

        private void CustomButtonControl_Load(object sender, EventArgs e)
        {

        }
    }
}
