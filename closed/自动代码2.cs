using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;

namespace closed
{
    public partial class 自动代码2 : Form
    {
        public 自动代码2()
        {
            InitializeComponent();
        }
       
        private void 自动代码2_Load(object sender, EventArgs e)
        {
            // X轴数据(流量)
            //double[] flowData =   { 0, 2.5, 5, 7.5, 10 };

            //// 红线数据(扬程)
            //double[] headData =  { 180, 178, 175, 170, 160 };

            //// 蓝线数据(效率)
            //double[] efficiencyData =  { 0, 20, 35, 45, 48 };

            //// 绿线数据(功率)
            //double[] powerData =  { 5, 6, 7, 8, 9 };
            double[] flowData = { 0 };

            // 红线数据(扬程)
            double[] headData = { 0 };

            // 蓝线数据(效率)
            double[] efficiencyData = { 0 };

            // 绿线数据(功率)
            double[] powerData = { 0 };
            // 创建图表
            Bitmap chartImage = CreatePumpPerformanceChart(flowData, headData, efficiencyData, powerData);
            pictureBox1.Image = chartImage; 
            // 保存图片
            string filePath = "泵性能试验曲线.png";
            chartImage.Save(filePath);
            Console.WriteLine($"图表已保存到: {Path.GetFullPath(filePath)}");

            // 在窗体上显示图片
            //ShowImageInForm(chartImage);
        }
        static void ShowImageInForm(Bitmap image)
        {
            // 创建窗体和图片控件
            Form form = new Form
            {
                Text = "泵性能试验曲线",
                Size = new Size(image.Width + 40, image.Height + 60),
                StartPosition = FormStartPosition.CenterScreen
            };

            PictureBox pictureBox = new PictureBox
            {
                Image = image,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Location = new Point(20, 20)
            };

            form.Controls.Add(pictureBox);
            //Application.EnableVisualStyles();
            //Application.Run(form);
        }

        static Bitmap CreatePumpPerformanceChart(double[] flow, double[] head, double[] efficiency, double[] power)
        {
           
               
            // 图表尺寸和边距
            int width = 900;
            int height =550;
            int marginLeft = 120;
            int marginRight =100;
            int marginTop = 60;
            int marginBottom =60;
            int marginSecondLeftAxis = 45;
            float secondLeftRatio = 0.5f;
            int chartWidth = width - marginLeft - marginRight;
            int chartHeight = height - marginTop - marginBottom;
            int axisLineSize = 1;
            // 创建位图和Graphics对象
            Bitmap bitmap = new Bitmap(width, height);
            //bitmap.SetResolution(300, 300);
            Graphics g = Graphics.FromImage(bitmap);
        
            // 设置高质量绘图
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
       
            // 填充白色背景
            g.Clear(Color.White);

            // 计算X轴范围
            double xMin = 0;
            double xMax = 10f; 
                                          
            double headMin = 0;
            double headMax = 10f;
         
            double powerMin = 0;
            double powerMax = 10f;

            double efficiencyMin = 0;
            double efficiencyMax = 10f;
     
            // 绘制网格和坐标轴
            DrawGrid(g, marginLeft, marginTop, chartWidth, chartHeight, axisLineSize);

            if (flow == null || head == null || efficiency == null || power == null || flow.Length < 3 || head.Length < 3 || efficiency.Length < 3 || power.Length < 3)
            {
                // 计算X轴范围
                 xMin = 0;
                xMax =10f; 
                        
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


            Font Xaisfontsize = new Font("Arial", 9);
           
            // 绘制X轴
            DrawXAxis(g, flow, xMin, xMax, marginLeft, marginTop, chartWidth, chartHeight, Xaisfontsize);

            // 绘制左侧Y轴(扬程-红)
            DrawLeftAxis(g, headMin, headMax, marginLeft, marginTop, chartHeight, "Head (m)", Color.Red, Xaisfontsize, axisLineSize);

            // 绘制左侧第二Y轴(功率-绿) - 修改为扬程Y轴高度的一半，从底部对齐
            DrawSecondLeftAxis(g, powerMin, powerMax, marginLeft - marginSecondLeftAxis, marginTop +(int)(chartHeight*secondLeftRatio), (int)(chartHeight * secondLeftRatio), "Power (kW)", Color.Green, Xaisfontsize, axisLineSize);

            // 绘制右侧Y轴(效率-蓝)
            DrawRightAxis(g, efficiencyMin, efficiencyMax, marginLeft + chartWidth, marginTop, chartHeight, "Efficiency (%)", Color.Blue, Xaisfontsize, axisLineSize);
    
            // 绘制标题
            using (Font titleFont = new Font("楷体", 16, FontStyle.Bold))
            {
                string title = "泵性能试验曲线";
                SizeF titleSize = g.MeasureString(title, titleFont);
                g.DrawString(title, titleFont, Brushes.Black, (width - titleSize.Width) / 2, 10);
            }

            // 绘制指示标
            using (Font titleFont = new Font("Arial", 10, FontStyle.Regular))
            {                
                string[] titles = { "●", "×", "▲" };
                string[] titles1 = { "—H", "—E", "—P" };
                float fixedSymbolWidth = TextRenderer.MeasureText("W", titleFont).Width; // 使用一个宽字符来估计最大宽度
                float startX = width - marginRight + 30; // 固定的起始X坐标
            
                int[] yOffsets = { 60, 40, 20 };
                for (int i = 0; i < titles.Length-1; i++)
                {
                    // 计算当前标题的绘制起点，保证所有符号占据相同的宽度
                    float adjustedStartX1 = startX + (fixedSymbolWidth - TextRenderer.MeasureText(titles[i].ToString(), titleFont).Width) / 2;
                    // 绘制文本，从调整后的X坐标开始
                    g.DrawString(titles[i], titleFont, Brushes.Black, adjustedStartX1, height - marginBottom - yOffsets[i]);
                    g.DrawString(titles1[i], titleFont, Brushes.Black, startX + 20, height - marginBottom - yOffsets[i]);
                }
                g.DrawString(titles[2], new Font("Arial",8, FontStyle.Regular), Brushes.Black,2+ startX + (fixedSymbolWidth - TextRenderer.MeasureText(titles[2].ToString(), titleFont).Width) / 2, height - marginBottom - yOffsets[2]);
                g.DrawString(titles1[2], titleFont, Brushes.Black, startX + 20, height - marginBottom - yOffsets[2]);
            }

            if (flow == null || head == null || efficiency == null || power == null || flow.Length < 3 || head.Length < 3 || efficiency.Length < 3 || power.Length < 3)
            {
                return bitmap;
            }

            // 用二次多项式拟合数据并绘制曲线
            DrawFittedCurve(g, flow, head, xMin, xMax, headMin, headMax, marginLeft, marginTop, chartWidth, chartHeight, Color.Red, 2);
            DrawFittedCurve(g, flow, efficiency, xMin, xMax, efficiencyMin, efficiencyMax, marginLeft, marginTop, chartWidth, chartHeight, Color.Blue, 2);
            DrawFittedCurve(g, flow, power, xMin, xMax, powerMin, powerMax, marginLeft, marginTop, chartWidth, chartHeight, Color.Green, 2, true);

            // 绘制数据点
            DrawDataPoints(g, flow, head, xMin, xMax, headMin, headMax, marginLeft, marginTop, chartWidth, chartHeight, Color.Red);
            DrawDataPoints(g, flow, efficiency, xMin, xMax, efficiencyMin, efficiencyMax, marginLeft, marginTop, chartWidth, chartHeight, Color.Blue, 'x');
            DrawDataPoints(g, flow, power, xMin, xMax, powerMin, powerMax, marginLeft, marginTop, chartWidth, chartHeight, Color.Green, '^', true);
            return bitmap;
        }

        static void DrawGrid(Graphics g, int x, int y, int width, int height, int lineSize)
        {
            // 绘制网格背景
            using (Pen gridPen = new Pen(Color.FromArgb(220, 220, 220)))
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
            using (Pen borderPen = new Pen(Brushes.Black, lineSize))
            {
                g.DrawRectangle(borderPen, x, y, width, height);
            }
        }
        //new Font("Arial", fontsize)
        static void DrawXAxis(Graphics g, double[] flowData, double min, double max, int x, int y, int width, int height,Font AxisLablefont)
        {
            // using (Font axisFont = AxisLablefont)
                Font axisFont = AxisLablefont;
            using (Pen axisPen = new Pen(Color.Black, 1))
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
                    g.DrawLine(axisPen, tickX, y + height, tickX, y + height + 5);

                    // 绘制刻度值
                    g.DrawString(value.ToString("F1"), axisFont, Brushes.Black, tickX, y + height + 10, format);
                }

                // X轴标题
                using (Font labelFont = new Font("Arial", 10, FontStyle.Bold))
                {
                    format.Alignment = StringAlignment.Center;
                    g.DrawString("Flow(m³/h)", labelFont, Brushes.Black, x + width / 2, y + height + 35, format);
                }
            }
        }

        static void DrawLeftAxis(Graphics g, double min, double max, int x, int y, int height, string label, Color color, Font AxisLablefont,int lineSize)
        {
            Font axisFont = AxisLablefont;
            using (Pen axisPen = new Pen(color, lineSize))
            using (Pen axisTickPen = new Pen(color, 1))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Far;

                // 绘制主轴线
                g.DrawLine(axisPen, x, y, x, y + height);

                // Y轴刻度
                int tickCount = 10;
                for (int i = 0; i <= tickCount; i++)
                {
                    double value = max - (max - min) * i / tickCount;
                    int tickY = y + (int)(height * i / tickCount);

                    // 绘制刻度线
                    g.DrawLine(axisTickPen, x - 5, tickY, x, tickY);

                    // 绘制刻度值
                    g.DrawString(value.ToString("F0"), axisFont, new SolidBrush(color), x - 8, tickY - 6, format);
                }

                // Y轴标题
                using (Font labelFont = new Font("Arial", 10, FontStyle.Bold))
                {
                    //g.TranslateTransform(x - 50, y + height / 2);
                    //g.RotateTransform(-90);
                    //format.Alignment = StringAlignment.Center;
                    //g.DrawString(label, labelFont, new SolidBrush(color), 0, 0, format);
                    //g.ResetTransform();
                    SizeF labelSize = g.MeasureString(label, labelFont);
                    g.TranslateTransform(x - 50, y+labelSize.Width); // 移动到轴上端
                    g.RotateTransform(-90);
                    format.Alignment = StringAlignment.Near;
                    g.DrawString(label, labelFont, new SolidBrush(color), 0, 0, format);
                    g.ResetTransform();
                }
            }
        }

        static void DrawSecondLeftAxis(Graphics g, double min, double max, int x, int y, int height, string label, Color color, Font AxisLablefont, int lineSize)
        {
            Font axisFont = AxisLablefont;
            using (Pen axisPen = new Pen(color, lineSize))
            using (Pen axisTickPen = new Pen(color, 1))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Far;

                // 绘制主轴线 - 明确绘制
                g.DrawLine(axisPen, x, y, x, y + height);

                // Y轴刻度
                int tickCount = 5; // 减少刻度数量，适应更小的高度
                for (int i = 0; i <= tickCount; i++)
                {
                    double value = max - (max - min) * i / tickCount;
                    int tickY = y + (int)(height * i / tickCount);

                    // 绘制刻度线
                    g.DrawLine(axisTickPen, x - 5, tickY, x, tickY);

                    // 绘制刻度值
                    g.DrawString(value.ToString("F1"), axisFont, new SolidBrush(color), x - 8, tickY - 6, format);
                }

                // Y轴标题
                using (Font labelFont = new Font("Arial", 10, FontStyle.Bold))
                {
                    SizeF labelSize = g.MeasureString(label, labelFont);
                    g.TranslateTransform(x - 50, y + labelSize.Width);
                    g.RotateTransform(-90);
                    format.Alignment = StringAlignment.Near;
                    g.DrawString(label, labelFont, new SolidBrush(color), 0, 0, format);
                    g.ResetTransform();
                }
            }
        }

        static void DrawRightAxis(Graphics g, double min, double max, int x, int y, int height, string label, Color color, Font AxisLablefont, int lineSize)
        {
            Font axisFont = AxisLablefont;
            using (Pen axisPen = new Pen(color, lineSize))
            using (Pen axisTickPen = new Pen(color, 1))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Near;

                // 绘制主轴线
                g.DrawLine(axisPen, x, y, x, y + height);

                // Y轴刻度
                int tickCount = 10;
                for (int i = 0; i <= tickCount; i++)
                {
                    double value = max - (max - min) * i / tickCount;
                    int tickY = y + (int)(height * i / tickCount);

                    // 绘制刻度线
                    g.DrawLine(axisTickPen, x, tickY, x + 5, tickY);

                    // 绘制刻度值
                    g.DrawString(value.ToString("F0"), axisFont, new SolidBrush(color), x + 8, tickY - 6, format);
                }

                // Y轴标题
                using (Font labelFont = new Font("Arial", 10, FontStyle.Bold))
                {
                    //g.TranslateTransform(x + 50, y + height / 2);
                    //g.RotateTransform(90);
                    //format.Alignment = StringAlignment.Center;
                    //g.DrawString(label, labelFont, new SolidBrush(color), 0, 0, format);
                    //g.ResetTransform();
                    SizeF labelSize = g.MeasureString(label, labelFont);
                    g.TranslateTransform(x + 50, y );
                    g.RotateTransform(90);
                    format.Alignment = StringAlignment.Near;
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

            for (int i = 0; i <= x.Length-1; i++)
            {
                double xValue = xMin + (xMax - xMin) * i / resolution;
                double yValue = EvaluatePolynomial(x[i], coefficients);

                // 将数据点转换为屏幕坐标
                int screenX = marginLeft + (int)(chartWidth * (x[i] - xMin) / (xMax - xMin));
                int screenY;

                if (useSecondYAxis) // 如果是第二条左侧Y轴（功率）
                {
                    // 调整为在下半部分显示，从底部对齐
                    screenY = marginTop + chartHeight / 2 + (int)((chartHeight / 2) * (yMax - yValue) / (yMax - yMin));
                    // 确保screenY不超出范围
                    screenY = Math.Max(marginTop + chartHeight / 2, Math.Min(marginTop + chartHeight, screenY));
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
                //g.DrawLines(curvePen, curvePoints.ToArray());
                g.DrawCurve(curvePen, curvePoints.ToArray(), 0.5f);
            }
        }

        static void DrawDataPoints(Graphics g, double[] x, double[] y, double xMin, double xMax, double yMin, double yMax, int marginLeft, int marginTop, int chartWidth, int chartHeight, Color color, char pointType = '•', bool useSecondYAxis = false)
        {
            for (int i = 0; i < x.Length; i++)
            {
                // 将数据点转换为屏幕坐标
                int screenX = marginLeft + (int)(chartWidth * (x[i] - xMin) / (xMax - xMin));
                int screenY;

                if (useSecondYAxis) // 如果是第二条左侧Y轴（功率）
                {
                    // 调整为在下半部分显示，从底部对齐
                    screenY = marginTop + chartHeight / 2 + (int)((chartHeight / 2) * (yMax - y[i]) / (yMax - yMin));
                    // 确保screenY不超出范围
                    screenY = Math.Max(marginTop + chartHeight / 2, Math.Min(marginTop + chartHeight, screenY));
                }
                else // 其他情况
                {
                    screenY = marginTop + (int)(chartHeight * (yMax - y[i]) / (yMax - yMin));
                }

                // 根据点类型绘制不同的标记
                switch (pointType)
                {
                    case '•': // 实心圆
                        using (SolidBrush brush = new SolidBrush(color))
                        {
                            g.FillEllipse(brush, screenX - 3, screenY - 3, 6, 6);
                        }
                        break;

                    case 'x': // X标记
                        using (Pen pen = new Pen(color, 1.5f))
                        {
                            g.DrawLine(pen, screenX - 3, screenY - 3, screenX + 3, screenY + 3);
                            g.DrawLine(pen, screenX - 3, screenY + 3, screenX + 3, screenY - 3);
                        }
                        break;

                    case '^': // 三角形
                        Point[] trianglePoints = new Point[] {
                            new Point(screenX, screenY - 4),
                            new Point(screenX - 4, screenY + 2),
                            new Point(screenX + 4, screenY + 2)
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
            // A简单版本，适用于小规模数据的二次拟合

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
    }
}
