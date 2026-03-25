using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace closed
{
    public partial class 重绘曲线2 : Form
    {
        public 重绘曲线2()
        {
            InitializeComponent();
        }

        private void 重绘曲线2_Load(object sender, EventArgs e)
        {

            int Width1 =1000;
            int Height1 = 1000;
            Bitmap bitmap = new Bitmap(Width1, Height1);
            bitmap.SetResolution(1000f, 1000f); // 设置高分辨率

            int x0 = 10;
            int y0 = 10;
            int x1 = Width1 - 10;
            int y1 = Height1 - 10;

            Graphics objGraphics = Graphics.FromImage(bitmap);
            objGraphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            objGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            objGraphics.SmoothingMode = SmoothingMode.HighQuality;
            objGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            objGraphics.CompositingQuality = CompositingQuality.HighQuality;

            // 调整边框线条的宽度（增加到3，使得更明显）
            objGraphics.DrawRectangle(new Pen(Color.Black, 3), 0, 0, Width1 - 1, Height1 - 1);

            // 调整绘制线条的宽度（增加到3）
            objGraphics.DrawLine(new Pen(new SolidBrush(Color.Black), 3), x0, y0, x1, y1);

            // 设置字体大小为20，确保颜色不透明
            Font font = new Font("宋体", 4, FontStyle.Bold);

            // 使用不透明的绿色绘制文本，设置Alpha值为255
            SolidBrush solidBrush = new SolidBrush(Color.FromArgb(255, 0, 128, 0)); // 完全不透明的绿色
            objGraphics.DrawString("Flow(m3)", font, solidBrush, 50, 50);

            // 将生成的图像显示在 PictureBox 中
            this.pictureBox1.Image = bitmap;

            // 保存图片到指定路径
            bitmap.Save(@"D://image_高分辨率.png", System.Drawing.Imaging.ImageFormat.Png);

            // 释放Graphics对象
            objGraphics.Dispose();


        }

        private float fltXSpace = 100f;
        public float XSpace
        {
            get { return fltXSpace /** 960 / 96*/; }
            set { fltXSpace = value ; }
        }
    }
}
