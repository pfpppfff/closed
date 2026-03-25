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
    public partial class Form2 : Form
    {
        private CustomButtonControl customButton;
        private Label resultLabel;
        public Form2()
        {
            InitializeComponent();
            SetupCustomControl();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
        private void SetupCustomControl()
        {
            // 创建自定义控件实例
            customButton = new CustomButtonControl();
            customButton.Location = new System.Drawing.Point(50, 50);
            customButton.ButtonText = "测试按钮";

            // 订阅值变化事件
            customButton.ResultValueChanged += CustomButton_ResultValueChanged;

            // 设置模式（默认为1，触发Click事件）
            customButton.ModeSel = 1;

            // 添加到窗体
            this.Controls.Add(customButton);

            // 创建一个用于切换模式的普通按钮
            Button toggleButton = new Button();
            toggleButton.Text = "切换模式";
            toggleButton.Location = new System.Drawing.Point(50, 120);
            toggleButton.Click += ToggleButton_Click;
            this.Controls.Add(toggleButton);

            // 创建一个标签用于显示结果值
            resultLabel = new Label();
            resultLabel.Text = "结果值: " + customButton.ResultValue;
            resultLabel.Location = new System.Drawing.Point(50, 160);
            resultLabel.AutoSize = true;
            this.Controls.Add(resultLabel);
        }

        private void CustomButton_ResultValueChanged(object sender, EventArgs e)
        {
            // 更新标签显示当前结果值
            resultLabel.Text = "结果值: " + customButton.ResultValue;

            // 可以根据不同的结果值执行不同的操作
            if (customButton.ResultValue == 1)
            {
                MessageBox.Show("执行了Click操作，ResultValue = 1");
            }
            else if (customButton.ResultValue == 2)
            {
                MessageBox.Show("执行了MouseDown操作，ResultValue = 2");
            }
        }

        private void ToggleButton_Click(object sender, EventArgs e)
        {
            // 切换模式
            customButton.ModeSel = customButton.ModeSel == 1 ? 2 : 1;
            MessageBox.Show($"模式已切换为{customButton.ModeSel}");
        }
    }
}
