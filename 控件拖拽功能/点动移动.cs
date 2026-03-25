using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 控件拖拽功能
{
    public partial class 点动移动 : Form
    {
        private Button followButton;
        private Button staticButton; // 添加一个静态按钮用于测试
        public 点动移动()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            InitializeUI();
        }
        private void InitializeUI()
        {
            // 设置窗体属性
            this.Text = "精确跟随鼠标点击的按钮";
            this.Size = new System.Drawing.Size(800, 600);

            // 创建跟随按钮
            followButton = new Button();
            followButton.Text = "跟随按钮";
            followButton.Size = new System.Drawing.Size(100, 30);
            followButton.Location = new System.Drawing.Point(350, 280); // 窗体中央
            followButton.BackColor = System.Drawing.Color.LightBlue;

            // 创建静态按钮用于测试
            staticButton = new Button();
            staticButton.Text = "静态按钮";
            staticButton.Size = new System.Drawing.Size(100, 30);
            staticButton.Location = new System.Drawing.Point(200, 200);
            staticButton.BackColor = System.Drawing.Color.LightGreen;

            // 将按钮添加到窗体
            this.Controls.Add(followButton);
            this.Controls.Add(staticButton);

            // 捕获窗体上所有鼠标点击事件
            this.MouseDown += Form1_MouseDown;

            // 为所有控件添加鼠标点击事件，包括按钮
            foreach (Control control in this.Controls)
            {
                control.MouseDown += Control_MouseDown;
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            // 直接处理窗体上的点击
            MoveFollowButton(e.X, e.Y);
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            // 当点击控件时，将控件上的点击位置转换为窗体坐标
            Control control = sender as Control;
            if (control != null && control != followButton) // 不处理跟随按钮自身的点击
            {
                // 将控件坐标转换为窗体坐标
                Point pointOnForm = control.PointToScreen(new Point(e.X, e.Y));
                pointOnForm = this.PointToClient(pointOnForm);

                // 移动跟随按钮到转换后的坐标
                MoveFollowButton(pointOnForm.X, pointOnForm.Y);

                // 阻止事件冒泡，确保窗体事件不会再次处理此点击
                // (对于Windows Forms，事件不自动冒泡，但这是一个好习惯)
            }
        }

        private void MoveFollowButton(int x, int y)
        {
            // 移动跟随按钮，使其中心对准指定位置
            followButton.Location = new Point(
                x - followButton.Width / 2,
                y - followButton.Height / 2
            );
        }
    }
}
