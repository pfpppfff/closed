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
    public partial class 拖拽移动 : Form
    {
        private Button followButton;
        private Button staticButton;
        private bool isDragging = false;
        private Point lastMousePosition;
        public 拖拽移动()
        {
            InitializeComponent();
        }

        private void 拖拽移动_Load(object sender, EventArgs e)
        {
            InitializeUI();
        }
        private void InitializeUI()
        {
            // 设置窗体属性
            this.Text = "拖拽移动按钮示例";
            this.Size = new System.Drawing.Size(800, 600);

            // 创建跟随按钮
            followButton = new Button();
            followButton.Text = "拖拽我";
            followButton.Size = new System.Drawing.Size(100, 30);
            followButton.Location = new System.Drawing.Point(350, 280); // 窗体中央
            followButton.BackColor = System.Drawing.Color.LightBlue;
            followButton.Cursor = Cursors.Hand;

            // 创建静态按钮用于测试
            staticButton = new Button();
            staticButton.Text = "静态按钮";
            staticButton.Size = new System.Drawing.Size(100, 30);
            staticButton.Location = new System.Drawing.Point(200, 200);
            staticButton.BackColor = System.Drawing.Color.LightGreen;

            // 将按钮添加到窗体
            this.Controls.Add(followButton);
            this.Controls.Add(staticButton);

            // 添加鼠标事件处理程序
            this.MouseMove += Form1_MouseMove;
            this.MouseDown += Form1_MouseDown;
            this.MouseUp += Form1_MouseUp;

            // 为所有控件添加鼠标事件
            foreach (Control control in this.Controls)
            {
                if (control != followButton) // 除了跟随按钮
                {
                    control.MouseDown += Control_MouseDown;
                    control.MouseUp += Control_MouseUp;
                    control.MouseMove += Control_MouseMove;
                }
            }

            // 为跟随按钮添加特殊的鼠标事件
            followButton.MouseDown += FollowButton_MouseDown;
            followButton.MouseUp += FollowButton_MouseUp;
            followButton.MouseMove += FollowButton_MouseMove;
        }

        // 窗体鼠标事件
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            StartDragging(e.Location);
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            StopDragging();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                MoveFollowButtonTo(e.Location);
            }
        }

        // 跟随按钮鼠标事件
        private void FollowButton_MouseDown(object sender, MouseEventArgs e)
        {
            Point pointOnForm = followButton.PointToScreen(e.Location);
            pointOnForm = this.PointToClient(pointOnForm);
            StartDragging(pointOnForm);
        }

        private void FollowButton_MouseUp(object sender, MouseEventArgs e)
        {
            StopDragging();
        }

        private void FollowButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point pointOnForm = followButton.PointToScreen(e.Location);
                pointOnForm = this.PointToClient(pointOnForm);
                MoveFollowButtonTo(pointOnForm);
            }
        }

        // 其他控件鼠标事件
        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            Control control = sender as Control;
            Point pointOnForm = control.PointToScreen(e.Location);
            pointOnForm = this.PointToClient(pointOnForm);
            StartDragging(pointOnForm);
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            StopDragging();
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Control control = sender as Control;
                Point pointOnForm = control.PointToScreen(e.Location);
                pointOnForm = this.PointToClient(pointOnForm);
                MoveFollowButtonTo(pointOnForm);
            }
        }

        // 拖拽功能辅助方法
        private void StartDragging(Point location)
        {
            isDragging = true;
            lastMousePosition = location;
            this.Cursor = Cursors.SizeAll;
        }

        private void StopDragging()
        {
            isDragging = false;
            this.Cursor = Cursors.Default;
        }

        private void MoveFollowButtonTo(Point newLocation)
        {
            // 根据鼠标当前位置移动按钮
            int deltaX = newLocation.X - lastMousePosition.X;
            int deltaY = newLocation.Y - lastMousePosition.Y;

            followButton.Location = new Point(
                followButton.Location.X + deltaX,
                followButton.Location.Y + deltaY
            );

            lastMousePosition = newLocation;
        }
    }
}
