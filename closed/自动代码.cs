using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using Encoder = System.Drawing.Imaging.Encoder;
using Font = System.Drawing.Font;

namespace closed
{
    public partial class 自动代码 : Form
    {
        public 自动代码()
        {
            InitializeComponent();
        }

        private void 自动代码_Load(object sender, EventArgs e)
        {
            // 测试数据 - 可以根据需要替换为实际数据
            // X轴数据(流量)
            double[] flowData = { 0, 2.5, 5, 7.5, 10, 12.5, 15, 17.5, 20, 22.5 };

            //红线数据(扬程)
            double[] headData = { 180, 178, 175, 170, 160, 145, 130, 115, 95, 75 };

            //蓝线数据(效率)
            double[] efficiencyData = { 0, 20, 35, 45, 48, 50, 49, 45, 32, 18 };

            //绿线数据(功率)
            double[] powerData = { 5, 6, 7, 8, 9, 10, 11, 11.5, 12, 34 };
            //double[] flowData = { 0 };

            ////红线数据(扬程)
            //double[] headData = { 0 };

            ////蓝线数据(效率)
            //double[] efficiencyData = { 0 };

            ////绿线数据(功率)
            //double[] powerData = { 0 };

            // 创建并保存高分辨率图表
            string filePath = "泵性能试验曲线1.png";
            Bitmap bitmap=   CreatePumpPerformanceChart(flowData, headData, efficiencyData, powerData, filePath, 1600,1200); // 高分辨率设置
            pictureBox1.Image = bitmap;
            Console.WriteLine($"图表已保存到: {Path.GetFullPath(filePath)}");
        }
        static  Bitmap CreatePumpPerformanceChart(double[] flow, double[] head, double[] efficiency, double[] power, string outputPath, int width = 800, int height = 600)
        {
            // 图表边距
            int marginLeft = width / 7;
            int marginRight = width / 8;
            int marginTop =height/7;
            int marginBottom = height / 7;

            int chartWidth = width - marginLeft - marginRight;
            int chartHeight = height - marginTop - marginBottom;

            // 字体大小基于图像尺寸进行缩放
            float fontScaleFactor = Math.Min(width / 800f, height / 600f);

            // 创建位图和Graphics对象
            Bitmap bitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bitmap);
            
                // 设置高质量绘图
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // 填充白色背景
                g.Clear(Color.White);

                // 绘制网格和坐标轴
                DrawGrid(g, marginLeft, marginTop, chartWidth, chartHeight);

            //// 计算X轴范围
            //double xMin = 0;
            //double xMax = flow.Max() * 1.1; // 留一些额外空间

            //// 计算Y轴范围
            //double headMin = 0;
            //double headMax = head.Max() * 1.1;

            //double powerMin = 0;
            //double powerMax = power.Max() * 1.1;

            //double efficiencyMin = 0;
            //double efficiencyMax = efficiency.Max() * 1.1;
            // 计算X轴范围
            double xMin = 0;
            double xMax = 10f;

            double headMin = 0;
            double headMax = 10f;

            double powerMin = 0;
            double powerMax = 10f;

            double efficiencyMin = 0;
            double efficiencyMax = 10f;
            if (flow == null || head == null || efficiency == null || power == null || flow.Length < 3 || head.Length < 3 || efficiency.Length < 3 || power.Length < 3)
            {
                // 计算X轴范围
                xMin = 0;
                xMax = 10f;

                headMin = 0;
                headMax = 10f;

                powerMin = 0;
                powerMax = 10f;

                efficiencyMin = 0;
                efficiencyMax = 10f;

            }
            else
            {
                // 计算X轴范围
                xMin = 0;
                xMax = flow.Max() * 1.1; // 轴线留一些额外空间

                headMin = 0;
                headMax = head.Max() * 1.1;
                //double headCurveMax = head.Max() * 1.2;

                powerMin = 0;
                powerMax = power.Max() * 1.1;
                //double powerCurveMax = power.Max() * 1.2;

                efficiencyMin = 0;
                efficiencyMax = efficiency.Max() * 1.1;
                //double efficiencyCurveMax = efficiency.Max() * 1.2;
            }

            // 绘制X轴
            DrawXAxis(g, xMin, xMax, marginLeft, marginTop, chartWidth, chartHeight, fontScaleFactor);

                // 绘制左侧Y轴(扬程-红)
                DrawLeftAxis(g, headMin, headMax, marginLeft, marginTop, chartHeight, "Head (m)", Color.Red, fontScaleFactor);

                // 绘制左侧第二Y轴(功率-绿) - 高度为扬程Y轴的一半，位于下半部分
                DrawPowerAxis(g, powerMin, powerMax, marginLeft - (int)(40 * fontScaleFactor), marginTop, chartHeight, "Power (kW)", Color.Green, fontScaleFactor);

                // 绘制右侧Y轴(效率-蓝)
                DrawRightAxis(g, efficiencyMin, efficiencyMax, marginLeft + chartWidth, marginTop, chartHeight, "Efficiency (%)", Color.Blue, fontScaleFactor);

                // 绘制标题
                using (Font titleFont = new Font("Arial", 16 * fontScaleFactor, FontStyle.Bold))
                {
                    string title = "泵性能试验曲线";
                    SizeF titleSize = g.MeasureString(title, titleFont);
                    g.DrawString(title, titleFont, Brushes.Black, (width - titleSize.Width) / 2, marginTop / 3);
                }

            // 绘制指示标
            using (Font titleFont = new Font("Arial", 15, FontStyle.Regular))
            {
                string[] titles = { "●", "×", "▲" };
                string[] titles1 = { "—H", "—E", "—P" };
                float fixedSymbolWidth = TextRenderer.MeasureText("W", titleFont).Width; // 使用一个宽字符来估计最大宽度
                float startX = width - marginRight + 40; // 固定的起始X坐标

                int[] yOffsets = { 80, 50, 20 };
                for (int i = 0; i < titles.Length - 1; i++)
                {
                    // 计算当前标题的绘制起点，保证所有符号占据相同的宽度
                    float adjustedStartX1 = startX + (fixedSymbolWidth - TextRenderer.MeasureText(titles[i].ToString(), titleFont).Width) / 2;
                    // 绘制文本，从调整后的X坐标开始
                    g.DrawString(titles[i], titleFont, Brushes.Black, adjustedStartX1, height - marginBottom - yOffsets[i]);
                    g.DrawString(titles1[i], titleFont, Brushes.Black, startX + 20, height - marginBottom - yOffsets[i]);
                }
                g.DrawString(titles[2], new Font("Arial", 13, FontStyle.Regular), Brushes.Black, 2 + startX + (fixedSymbolWidth - TextRenderer.MeasureText(titles[2].ToString(), titleFont).Width) / 2, height - marginBottom - yOffsets[2]);
                g.DrawString(titles1[2], titleFont, Brushes.Black, startX + 20, height - marginBottom - yOffsets[2]);
            }
            //using (Font titleFont = new Font("Arial", 10*fontScaleFactor, FontStyle.Regular))
            //{
            //    string[] titles = { "●", "×", "▴" };
            //    string[] titles1 = { "—H", "—E", "—P" };
            //    float fixedSymbolWidth = TextRenderer.MeasureText("W", titleFont).Width * fontScaleFactor; // 使用一个宽字符来估计最大宽度
            //    float startX = width - marginRight + 15 * fontScaleFactor; // 固定的起始X坐标
            //    float adjustedStartPowerX1 = 0;
            //    int[] yOffsets = { (int)(60 * fontScaleFactor), (int)(40 * fontScaleFactor), (int)(20 * fontScaleFactor) };
            //    for (int i = 0; i < 2; i++)
            //    {
            //        // 计算当前标题的绘制起点，保证所有符号占据相同的宽度
            //        float adjustedStartX1 = startX + (fixedSymbolWidth - fontScaleFactor * TextRenderer.MeasureText(titles[i].ToString(), titleFont).Width) / 2;
            //        adjustedStartPowerX1 =4* fontScaleFactor+ startX + (fixedSymbolWidth - fontScaleFactor * TextRenderer.MeasureText(titles[2].ToString(), titleFont).Width) / 2;
            //        // 绘制文本，从调整后的X坐标开始
            //        g.DrawString(titles[i], titleFont, Brushes.Black, adjustedStartX1, height - marginBottom - yOffsets[i] );
            //        g.DrawString(titles1[i], titleFont, Brushes.Black, startX + 30 * fontScaleFactor, height - marginBottom  - yOffsets[i]);
            //    }
            //    g.DrawString(titles[2], new Font("Arial", 10 * fontScaleFactor, FontStyle.Regular), Brushes.Black, adjustedStartPowerX1, height - marginBottom - yOffsets[2]);
            //    g.DrawString(titles1[2], titleFont, Brushes.Black, startX + 30 * fontScaleFactor, height - marginBottom - yOffsets[2]);

            //}

            if (flow == null || head == null || efficiency == null || power == null || flow.Length < 3 || head.Length < 3 || efficiency.Length < 3 || power.Length < 3)
            {
                return bitmap;
            }

            // 用二次多项式拟合数据并绘制曲线
            DrawFittedCurve(g, flow, head, xMin, xMax, headMin, headMax, marginLeft, marginTop, chartWidth, chartHeight, Color.Red, 2 * fontScaleFactor);
            DrawFittedCurve(g, flow, efficiency, xMin, xMax, efficiencyMin, efficiencyMax, marginLeft, marginTop, chartWidth, chartHeight, Color.Blue, 2 * fontScaleFactor);
            DrawFittedCurve(g, flow, power, xMin, xMax, powerMin, powerMax, marginLeft, marginTop, chartWidth, chartHeight, Color.Green, 2 * fontScaleFactor, true);

            // 绘制数据点
            DrawDataPoints(g, flow, head, xMin, xMax, headMin, headMax, marginLeft, marginTop, chartWidth, chartHeight, Color.Red, '•', fontScaleFactor);
            DrawDataPoints(g, flow, efficiency, xMin, xMax, efficiencyMin, efficiencyMax, marginLeft, marginTop, chartWidth, chartHeight, Color.Blue, 'x', fontScaleFactor);
            DrawDataPoints(g, flow, power, xMin, xMax, powerMin, powerMax, marginLeft, marginTop, chartWidth, chartHeight, Color.Green, '^', fontScaleFactor, true);
            // 保存图片
            bitmap.Save(outputPath);
                return bitmap;     
        }

        static void DrawGrid(Graphics g, int x, int y, int width, int height)
        {
            // 绘制网格背景
            using (Pen gridPen = new Pen(Color.FromArgb(220, 220, 220), 1))
            {
                gridPen.DashStyle = DashStyle.Dot;

                // 垂直网格线
                int gridCount = 10;
                for (int i = 0; i <= gridCount; i++)
                {
                    int lineX = x + (width * i / gridCount);
                    g.DrawLine(gridPen, lineX, y, lineX, y + height);
                }

                // 水平网格线
                for (int i = 0; i <= gridCount; i++)
                {
                    int lineY = y + (height * i / gridCount);
                    g.DrawLine(gridPen, x, lineY, x + width, lineY);
                }
            }

            // 绘制边框
            using (Pen borderPen = new Pen(Brushes.Black, 1))
            {
                g.DrawRectangle(borderPen, x, y, width, height);
            }
           
        }

        static void DrawXAxis(Graphics g, double min, double max, int x, int y, int width, int height, float fontScale)
        {
            using (Font axisFont = new Font("Arial", 8 * fontScale))
            using (Pen axisPen = new Pen(Color.Black, 1 * fontScale))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;

                // X轴刻度
                int tickCount = 10;  // 刻度数量
                for (int i = 0; i <= tickCount; i++)
                {
                    double value = min + (max - min) * i / tickCount;
                    int tickX = x + (int)(width * i / tickCount);

                    // 绘制刻度线
                    g.DrawLine(axisPen, tickX, y + height, tickX, y + height + (int)(5 * fontScale));

                    // 绘制刻度值
                    g.DrawString(value.ToString("F1"), axisFont, Brushes.Black, tickX, y + height + (int)(10 * fontScale), format);
                }

                // X轴标题
                using (Font labelFont = new Font("Arial", 10 * fontScale, FontStyle.Bold))
                {
                    format.Alignment = StringAlignment.Center;
                    g.DrawString("Flow(m³/h)", labelFont, Brushes.Black, x + width / 2, y + height + (int)(40 * fontScale), format);
                }
            }
        }

        static void DrawLeftAxis(Graphics g, double min, double max, int x, int y, int height, string label, Color color, float fontScale)
        {
            using (Font axisFont = new Font("Arial", 8 * fontScale))
            using (Pen axisPen = new Pen(color, 1 * fontScale))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Far;

                // Y轴刻度
                int tickCount = 10;
                for (int i = 0; i <= tickCount; i++)
                {
                    double value = max - (max - min) * i / tickCount;
                    int tickY = y + (int)(height * i / tickCount);

                    // 绘制刻度线
                    g.DrawLine(axisPen, x - (int)(5 * fontScale), tickY, x, tickY);

                    // 绘制刻度值
                    g.DrawString(value.ToString("F0"), axisFont, new SolidBrush(color), x - (int)(8 * fontScale), tickY - (int)(6 * fontScale), format);
                }

                // 绘制Y轴主轴线
                g.DrawLine(axisPen, x, y, x, y + height);

                // Y轴标题 - 与轴上端对齐
                using (Font labelFont = new Font("Arial", 10 * fontScale, FontStyle.Bold))
                {
                    SizeF labelSize = g.MeasureString(label, labelFont);
                    g.TranslateTransform(x - (int)(50 * fontScale), y + labelSize.Width/2 );
                    g.RotateTransform(-90);
                    format.Alignment = StringAlignment.Center;
                    g.DrawString(label, labelFont, new SolidBrush(color), 0, 0, format);
                    g.ResetTransform();
                }
            }
        }

        static void DrawPowerAxis(Graphics g, double min, double max, int x, int y, int height, string label, Color color, float fontScale)
        {
            // 功率Y轴位置 - 位于图表高度的下半部分
            int powerAxisHeight = height / 2;  // 高度为总高度的一半
            int powerAxisY = y + height - powerAxisHeight;  // 从底部向上显示

            using (Font axisFont = new Font("Arial", 8 * fontScale))
            using (Pen axisPen = new Pen(color, 1 * fontScale))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Far;

                // 绘制功率Y轴主轴线
                g.DrawLine(axisPen, x, powerAxisY, x, powerAxisY + powerAxisHeight);

                // Y轴刻度
                int tickCount = 5;  // 刻度数量减半，适应较小的高度
                for (int i = 0; i <= tickCount; i++)
                {
                    double value = max - (max - min) * i / tickCount;
                    int tickY = powerAxisY + (int)(powerAxisHeight * i / tickCount);

                    // 绘制刻度线
                    g.DrawLine(axisPen, x - (int)(5 * fontScale), tickY, x, tickY);

                    // 绘制刻度值
                    g.DrawString(value.ToString("F1"), axisFont, new SolidBrush(color), x - (int)(8 * fontScale), tickY - (int)(6 * fontScale), format);
                }

                // Y轴标题 - 与轴上端对齐
                using (Font labelFont = new Font("Arial", 10 * fontScale, FontStyle.Bold))
                {
                    SizeF labelSize = g.MeasureString(label, labelFont);
                    g.TranslateTransform(x - (int)(50 * fontScale), powerAxisY+ labelSize.Width/2);
                    g.RotateTransform(-90);
                    format.Alignment = StringAlignment.Center;
                    g.DrawString(label, labelFont, new SolidBrush(color), 0, 0, format);
                    g.ResetTransform();
                }
            }
        }

        static void DrawRightAxis(Graphics g, double min, double max, int x, int y, int height, string label, Color color, float fontScale)
        {
            using (Font axisFont = new Font("Arial", 8 * fontScale))
            using (Pen axisPen = new Pen(color, 1 * fontScale))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Near;

                // 绘制Y轴主轴线
                g.DrawLine(axisPen, x, y, x, y + height);

                // Y轴刻度
                int tickCount = 10;
                for (int i = 0; i <= tickCount; i++)
                {
                    double value = max - (max - min) * i / tickCount;
                    int tickY = y + (int)(height * i / tickCount);

                    // 绘制刻度线
                    g.DrawLine(axisPen, x, tickY, x + (int)(5 * fontScale), tickY);

                    // 绘制刻度值
                    g.DrawString(value.ToString("F0"), axisFont, new SolidBrush(color), x + (int)(8 * fontScale), tickY - (int)(6 * fontScale), format);
                }

                // Y轴标题 - 与轴上端对齐
                using (Font labelFont = new Font("Arial", 10 * fontScale, FontStyle.Bold))
                {
                    SizeF labelSize = g.MeasureString(label, labelFont);
                    g.TranslateTransform(x + (int)(50 * fontScale), y+ labelSize.Width/ 2);
                    g.RotateTransform(90);
                    format.Alignment = StringAlignment.Center;
                    g.DrawString(label, labelFont, new SolidBrush(color), 0, 0, format);
                    g.ResetTransform();
                }
            }
        }

        static void DrawFittedCurve(Graphics g, double[] x, double[] y, double xMin, double xMax, double yMin, double yMax, int marginLeft, int marginTop, int chartWidth, int chartHeight, Color color, float penWidth = 2, bool useSecondYAxis = false)
        {
            // 多项式拟合（二次）
            double[] coefficients = PolynomialFit(x, y, 2);

            // 根据拟合系数生成平滑曲线点
            List<Point> curvePoints = new List<Point>();
            int resolution = 100;

            for (int i = 0; i <= x.Length - 1; i++)
            {
                double xValue = xMin + (xMax - xMin) * i / resolution;
                double yValue = EvaluatePolynomial(x[i], coefficients);

                // 将数据点转换为屏幕坐标
                int screenX = marginLeft + (int)(chartWidth * (x[i] - xMin) / (xMax - xMin));
                int screenY;

                if (useSecondYAxis) // 如果是第二条左侧Y轴（功率）
                {
                    // 功率Y轴位于下半部分，高度为一半
                    int powerAxisHeight = chartHeight / 2;
                    int powerAxisY = marginTop + chartHeight - powerAxisHeight;
                    screenY = powerAxisY + (int)(powerAxisHeight * (yMax - yValue) / (yMax - yMin));
                }
                else // 其他情况
                {
                    screenY = marginTop + (int)(chartHeight * (yMax - yValue) / (yMax - yMin));
                }

                curvePoints.Add(new Point(screenX, screenY));
            }

            // 绘制平滑曲线
            using (Pen curvePen = new Pen(color, penWidth))
            {
                g.DrawCurve(curvePen, curvePoints.ToArray(), 0.5f);
            }
        }

        static void DrawDataPoints(Graphics g, double[] x, double[] y, double xMin, double xMax, double yMin, double yMax, int marginLeft, int marginTop, int chartWidth, int chartHeight, Color color, char pointType = '•', float fontScale = 1, bool useSecondYAxis = false)
        {
            for (int i = 0; i < x.Length; i++)
            {
                // 将数据点转换为屏幕坐标
                int screenX = marginLeft + (int)(chartWidth * (x[i] - xMin) / (xMax - xMin));
                int screenY;

                if (useSecondYAxis) // 如果是第二条左侧Y轴（功率）
                {
                    // 功率Y轴位于下半部分，高度为一半
                    int powerAxisHeight = chartHeight / 2;
                    int powerAxisY = marginTop + chartHeight - powerAxisHeight;
                    screenY = powerAxisY + (int)(powerAxisHeight * (yMax - y[i]) / (yMax - yMin));
                }
                else // 其他情况
                {
                    screenY = marginTop + (int)(chartHeight * (yMax - y[i]) / (yMax - yMin));
                }

                // 点的大小基于缩放因子
                int pointSize = (int)(3 * fontScale);

                // 根据点类型绘制不同的标记
                switch (pointType)
                {
                    case '•': // 实心圆
                        using (SolidBrush brush = new SolidBrush(color))
                        {
                            g.FillEllipse(brush, screenX - pointSize, screenY - pointSize, pointSize * 2, pointSize * 2);
                        }
                        break;

                    case 'x': // X标记
                        using (Pen pen = new Pen(color, 1.5f * fontScale))
                        {
                            g.DrawLine(pen, screenX - pointSize, screenY - pointSize, screenX + pointSize, screenY + pointSize);
                            g.DrawLine(pen, screenX - pointSize, screenY + pointSize, screenX + pointSize, screenY - pointSize);
                        }
                        break;

                    case '^': // 三角形
                        Point[] trianglePoints = new Point[] {
                            new Point(screenX, screenY - (int)(4 * fontScale)),
                            new Point(screenX - (int)(4 * fontScale), screenY + (int)(2 * fontScale)),
                            new Point(screenX + (int)(4 * fontScale), screenY + (int)(2 * fontScale))
                        };
                        using (SolidBrush brush = new SolidBrush(color))
                        {
                            g.FillPolygon(brush, trianglePoints);
                        }
                        break;
                }
            }
        }

        static double[] PolynomialFit(double[] x, double[] y, int degree)
        {
            // 实现多项式拟合，返回多项式系数（从低到高次幂）
            // 简单版本，适用于小规模数据的二次拟合

            int n = x.Length;
            double[,] matrix = new double[degree + 1, degree + 2];

            // 构建增广矩阵
            for (int i = 0; i <= degree; i++)
            {
                for (int j = 0; j <= degree; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < n; k++)
                    {
                        sum += Math.Pow(x[k], i + j);
                    }
                    matrix[i, j] = sum;
                }

                double sumY = 0;
                for (int k = 0; k < n; k++)
                {
                    sumY += y[k] * Math.Pow(x[k], i);
                }
                matrix[i, degree + 1] = sumY;
            }

            // 高斯消元求解
            for (int i = 0; i <= degree; i++)
            {
                // 将对角线元素归一化
                double div = matrix[i, i];
                for (int j = i; j <= degree + 1; j++)
                {
                    matrix[i, j] /= div;
                }

                // 消元
                for (int j = 0; j <= degree; j++)
                {
                    if (i != j)
                    {
                        double factor = matrix[j, i];
                        for (int k = i; k <= degree + 1; k++)
                        {
                            matrix[j, k] -= factor * matrix[i, k];
                        }
                    }
                }
            }

            // 提取解
            double[] coefficients = new double[degree + 1];
            for (int i = 0; i <= degree; i++)
            {
                coefficients[i] = matrix[i, degree + 1];
            }

            return coefficients;
        }

        static double EvaluatePolynomial(double x, double[] coefficients)
        {
            double result = 0;
            for (int i = 0; i < coefficients.Length; i++)
            {
                result += coefficients[i] * Math.Pow(x, i);
            }
            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form form = new 自动代码2();
            form.ShowDialog();
        }
    }

    
}
