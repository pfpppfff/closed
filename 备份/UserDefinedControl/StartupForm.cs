using System;
using System.Drawing;
using System.Windows.Forms;

namespace UserDefinedControl
{
    /// <summary>
    /// 启动选择窗体
    /// 让用户选择启动Form1还是Form2
    /// </summary>
    public partial class StartupForm : Form
    {
        public StartupForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // 采用简洁美观的设计风格
            this.Text = "UserDefinedControl - 启动选择";
            this.Size = new Size(400, 200);
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var titleLabel = new Label
            {
                Text = "请选择要启动的窗体",
                Location = new Point(50, 30),
                Size = new Size(300, 30),
                Font = new Font("微软雅黑", 12F, FontStyle.Bold),
                ForeColor = Color.DarkGray,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var form1Button = new Button
            {
                Text = "Form1 - 控件演示",
                Location = new Point(50, 80),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(200, 230, 255),
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false,
                Font = new Font("微软雅黑", 9F, FontStyle.Regular),
                ForeColor = Color.DarkGray
            };
            form1Button.FlatAppearance.BorderColor = Color.LightGray;

            var form2Button = new Button
            {
                Text = "Form2 - OPC UA监控",
                Location = new Point(220, 80),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(200, 255, 200),
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false,
                Font = new Font("微软雅黑", 9F, FontStyle.Regular),
                ForeColor = Color.DarkGray
            };
            form2Button.FlatAppearance.BorderColor = Color.LightGray;

            var descLabel = new Label
            {
                Text = "Form1: 自定义控件演示 | Form2: OPC UA实时数据监控",
                Location = new Point(20, 130),
                Size = new Size(360, 20),
                Font = new Font("微软雅黑", 8F, FontStyle.Regular),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // 绑定事件
            form1Button.Click += (s, e) =>
            {
                this.Hide();
                var form1 = new Form1();
                form1.FormClosed += (sender, args) => this.Close();
                form1.Show();
            };

            form2Button.Click += (s, e) =>
            {
                this.Hide();
                var form2 = new Form2();
                form2.FormClosed += (sender, args) => this.Close();
                form2.Show();
            };

            this.Controls.AddRange(new Control[] { titleLabel, form1Button, form2Button, descLabel });
        }
    }
}