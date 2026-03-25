using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 截图获取数据
{
    public partial class ScreenshotForm : Form
    {
        public event EventHandler<CaptureCompletedEventArgs> CaptureCompleted;

        private Point startPoint;
        private Point endPoint;
        private bool isDrawing = false;
        private Bitmap screenBitmap;
        public ScreenshotForm()
        {
            InitializeComponent();
            InitializeComponent1();
            // 截取全屏
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            screenBitmap = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(screenBitmap))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
        private void InitializeComponent1()
        {
            this.SuspendLayout();

            // ScreenCaptureForm
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            this.Cursor = Cursors.Cross;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "ScreenCaptureForm";
            this.Opacity = 0.3;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.WindowState = FormWindowState.Maximized;
            this.KeyDown += new KeyEventHandler(this.ScreenCaptureForm_KeyDown);
            this.MouseDown += new MouseEventHandler(this.ScreenCaptureForm_MouseDown);
            this.MouseMove += new MouseEventHandler(this.ScreenCaptureForm_MouseMove);
            this.MouseUp += new MouseEventHandler(this.ScreenCaptureForm_MouseUp);
            this.Paint += new PaintEventHandler(this.ScreenCaptureForm_Paint);

            this.ResumeLayout(false);
        }

        private void ScreenCaptureForm_KeyDown(object sender, KeyEventArgs e)
        {
            // 按ESC键退出截图
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                OnCaptureCompleted(null);
                this.Close();
            }
        }

        private void ScreenCaptureForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                startPoint = e.Location;
                isDrawing = true;
            }
        }

        private void ScreenCaptureForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                endPoint = e.Location;
                this.Invalidate(); // 触发重绘
            }
        }

        private void ScreenCaptureForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                endPoint = e.Location;
                isDrawing = false;

                // 计算选择区域
                Rectangle captureRect = GetCaptureRectangle();

                if (captureRect.Width > 5 && captureRect.Height > 5)
                {
                    try
                    {
                        // 从屏幕截图中裁剪出选定区域
                        Bitmap capturedImage = new Bitmap(captureRect.Width, captureRect.Height);
                        using (Graphics g = Graphics.FromImage(capturedImage))
                        {
                            g.DrawImage(screenBitmap,
                                        new Rectangle(0, 0, captureRect.Width, captureRect.Height),
                                        captureRect,
                                        GraphicsUnit.Pixel);
                        }

                        OnCaptureCompleted(capturedImage);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("截图失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void ScreenCaptureForm_Paint(object sender, PaintEventArgs e)
        {
            if (isDrawing)
            {
                // 绘制选择框
                Rectangle captureRect = GetCaptureRectangle();

                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, captureRect);
                }

                // 显示选择框的大小信息
                string sizeInfo = $"{captureRect.Width} x {captureRect.Height}";
                Font font = new Font("Arial", 10);
                SolidBrush brush = new SolidBrush(Color.Red);
                Point textLocation = new Point(captureRect.Right + 5, captureRect.Bottom + 5);

                if (textLocation.X + 100 > this.Width)
                {
                    textLocation.X = captureRect.Left;
                }

                if (textLocation.Y + 20 > this.Height)
                {
                    textLocation.Y = captureRect.Top - 20;
                }

                e.Graphics.DrawString(sizeInfo, font, brush, textLocation);
            }
        }

        private Rectangle GetCaptureRectangle()
        {
            int x = Math.Min(startPoint.X, endPoint.X);
            int y = Math.Min(startPoint.Y, endPoint.Y);
            int width = Math.Abs(endPoint.X - startPoint.X);
            int height = Math.Abs(endPoint.Y - startPoint.Y);

            return new Rectangle(x, y, width, height);
        }

        protected virtual void OnCaptureCompleted(Image capturedImage)
        {
            CaptureCompleted?.Invoke(this, new CaptureCompletedEventArgs(capturedImage));
        }
    }
}
