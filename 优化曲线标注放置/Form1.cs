﻿﻿﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace 优化曲线标注放置
{
    public partial class Form1 : Form
    {
        private CurveLabelOptimizer optimizer;
        private PictureBox pictureBox;
        
        public Form1()
        {
            InitializeComponent();
            SetupUI();
            SetupCurves();
        }
        
        private void SetupUI()
        {
            // 设置窗体属性
            this.Text = "优化曲线标注放置";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // 创建PictureBox控件
            pictureBox = new PictureBox();
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.BackColor = Color.White;
            pictureBox.Paint += PictureBox_Paint;
            pictureBox.MouseDown += PictureBox_MouseDown;
            
            // 添加按钮面板
            Panel buttonPanel = new Panel();
            buttonPanel.Height = 40;
            buttonPanel.Dock = DockStyle.Bottom;
            
            Button optimizeButton = new Button();
            optimizeButton.Text = "优化标注";
            optimizeButton.Size = new Size(80, 30);
            optimizeButton.Location = new Point(10, 5);
            optimizeButton.Click += OptimizeButton_Click;
            
            Button resetButton = new Button();
            resetButton.Text = "重置";
            resetButton.Size = new Size(80, 30);
            resetButton.Location = new Point(100, 5);
            resetButton.Click += ResetButton_Click;
            
            buttonPanel.Controls.Add(optimizeButton);
            buttonPanel.Controls.Add(resetButton);
            
            // 添加控件到窗体
            this.Controls.Add(pictureBox);
            this.Controls.Add(buttonPanel);
        }
        
        private void SetupCurves()
        {
            // 初始化优化器
            optimizer = new CurveLabelOptimizer(new RectangleF(50, 50, 700, 500), 30);
            
            // 添加4条示例曲线
            optimizer.AddCurve(new QuadraticCurve(1, 0.001, -0.2, 100, Color.Red));
            optimizer.AddCurve(new QuadraticCurve(2, 0.002, -0.4, 150, Color.Blue));
            optimizer.AddCurve(new QuadraticCurve(3, 0.0015, -0.3, 200, Color.Green));
            optimizer.AddCurve(new QuadraticCurve(4, 0.0008, -0.1, 250, Color.Orange));
        }
        
        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // 绘制坐标系
            DrawCoordinateSystem(g);
            
            // 绘制曲线
            DrawCurves(g);
            
            // 绘制标注
            DrawLabels(g);
        }
        
        private void DrawCoordinateSystem(Graphics g)
        {
            Pen pen = new Pen(Color.LightGray, 1);
            
            // 绘制网格线
            for (int x = 50; x <= 750; x += 50)
            {
                g.DrawLine(pen, x, 50, x, 550);
            }
            
            for (int y = 50; y <= 550; y += 50)
            {
                g.DrawLine(pen, 50, y, 750, y);
            }
            
            pen.Dispose();
        }
        
        private void DrawCurves(Graphics g)
        {
            foreach (var curve in optimizer.Curves)
            {
                DrawCurve(g, curve);
            }
        }
        
        private void DrawCurve(Graphics g, QuadraticCurve curve)
        {
            Pen pen = new Pen(curve.Color, 2);
            
            // 绘制曲线
            PointF[] points = new PointF[701];
            int index = 0;
            for (int x = 50; x <= 750; x++)
            {
                double y = curve.CalculateY(x);
                if (y >= 50 && y <= 550)
                {
                    points[index++] = new PointF(x, (float)y);
                }
            }
            
            if (index > 1)
            {
                g.DrawLines(pen, points.Take(index).ToArray());
            }
            
            pen.Dispose();
        }
        
        private void DrawLabels(Graphics g)
        {
            foreach (var label in optimizer.Labels)
            {
                DrawLabel(g, label);
            }
        }
        
        private void DrawLabel(Graphics g, LabelAnnotation label)
        {
            // 绘制标注正方形
            Pen pen = new Pen(Color.Black, 2);
            Brush brush = new SolidBrush(Color.FromArgb(128, label.Curve.Color));
            
            // 绘制四边形
            g.DrawPolygon(pen, label.Corners);
            g.FillPolygon(brush, label.Corners);
            
            // 绘制锚点
            Brush anchorBrush = new SolidBrush(Color.Black);
            g.FillEllipse(anchorBrush, label.AnchorPoint.X - 3, label.AnchorPoint.Y - 3, 6, 6);
            
            // 绘制标签文本
            Font font = new Font("Arial", 10, FontStyle.Bold);
            Brush textBrush = new SolidBrush(Color.Black);
            string text = $"C{label.Curve.Id}";
            SizeF textSize = g.MeasureString(text, font);
            g.DrawString(text, font, textBrush, label.Center.X - textSize.Width/2, label.Center.Y - textSize.Height/2);
            
            pen.Dispose();
            brush.Dispose();
            anchorBrush.Dispose();
            font.Dispose();
            textBrush.Dispose();
        }
        
        private void OptimizeButton_Click(object sender, EventArgs e)
        {
            bool success = optimizer.OptimizeLabels();
            pictureBox.Invalidate();
            
            if (!success)
            {
                MessageBox.Show("无法为所有曲线找到合适的标注位置，请调整参数后重试。", "优化失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void ResetButton_Click(object sender, EventArgs e)
        {
            optimizer.Labels.Clear();
            pictureBox.Invalidate();
        }
        
        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            // 强制重绘
            pictureBox.Invalidate();
        }
    }
}
