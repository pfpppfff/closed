using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace closed
{
    public partial class 自制曲线图 : Form
    {
        public 自制曲线图()
        {
            InitializeComponent();
        }

        private void 自制曲线图_Load(object sender, EventArgs e)
        {
            Curve2D cuv2D = new Curve2D();
            cuv2D.Fit();
            Bitmap bitmap = cuv2D.CreateImage();
            this.pictureBox1.Image = cuv2D.CreateImage();
            cuv2D.CreateImage().Save(@"D://image默认分辨率.bmp");
            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);
            using (var workbook = new XLWorkbook())
            {
                // 添加一个工作表
                var worksheet = workbook.AddWorksheet("Sheet1");

                // 在单元格中写入一些数据
                worksheet.Cell("A1").Value = "This is a sample image:";

                // 定义图片路径
                string imagePath = "sample_image.png"; // 替换为你的图片路径

                // 插入图片
                var image = worksheet.AddPicture(memoryStream, "GeneratedImage")
                                     .MoveTo(worksheet.Cell("B2"))  // 移动图片到某个单元格的位置
                                     .WithSize(bitmap.Width, bitmap.Height); // 设置图片大小

                // 保存Excel文件
                string filePath = "output_with_image.xlsx";
                workbook.SaveAs(filePath);


            }
        }

    }
    public class Curve2D
    {
        private Graphics objGraphics; //Graphics 类提供将对象绘制到显示设备的方法
        private Bitmap objBitmap; //位图对象

        private float fltWidth =600 ; //图像宽度
        private float fltHeight = 400 ; //图像高度
        private float resolutionW = 96;
        private float resolutionH =96;
        private float fltXSlice = 20 ; //X轴刻度宽度
        private float fltYSlice = 20 ; //Y轴刻度宽度
        private float fltYSliceValue = 20 ; //Y轴刻度的数值宽度

        private float fltTension = 0.5f ;
        private string strTitle = "泵性能试验曲线"; //标题
        private string strXAxisText = "Flow(m³/h)"; //X轴说明文字
        private string[] strYAxisText = { "Head(m)", "Efficiency(%)", "Power(kW)" }; //Y轴说明文字
        private int xSpaceNum = 10;//x轴10段
        private int[] ySpaceNum = { 10, 10, 5 };//y轴10段
     
        private float[] fltsXValues = new float[] { 0.00f, 2.83f, 4.99f, 6.62f, 8.90f, 10.24f, 12.03f, 12.94f, 14.07f, 15.08f, 16.54f, 17.42f, 19.23f, 21.10f, 22.91f };
        private float[] fltsY1Values = new float[] { 182.06f, 176.00f, 180.38f, 176.79f, 162.02f, 154.92f, 144.47f, 138.70f, 129.93f, 121.67f, 104.48f, 93.37f, 68.75f, 36.00f, 28.34f };
        private float[] fltsY2Values = new float[] { 0, 21.13668f, 33.15213f, 39.87827f, 44.91978f, 47.13524f, 48.52813f, 48.75133f, 48.40665f, 47.50113f, 43.81768f, 40.75647f, 32.91607f, 19.02138f, 16.91179f };
        private float[] fltsY3Values = new float[] { 5.832163f, 6.411861f, 7.397513f, 7.997725f, 8.740602f, 9.165549f, 9.750181f, 10.02746f, 10.28194f, 10.52059f, 10.74103f, 10.86845f, 10.94007f, 10.87709f, 10.45594f };
        private float[] fltYSliceBegin = { 0, 0, 0 }; //Y轴刻度开始值
        private float[] fltYSliceEnd = { 160, 180, 100 };
        private float fltXSliceBegin = 0;
        private float fltXSliceEnd = 150;
        private double tensiony3 = 0.4;//y3轴比y1轴的倍率系数
        private Color clrBgColor = Color.White; //背景色
        private Color clrTextColor = Color.Black; //文字颜色
        private Color clrBorderColor = Color.Black; //整体边框颜色
        private Color clrAxisColor = Color.Black; //轴线颜色
        private Color[] clrAxisTextColor = new Color[] { Color.Black, Color.Red, Color.Blue, Color.Green }; //轴说明文字颜色
        private Color clrSliceTextColor = Color.Black; //刻度文字颜色
        private Color clrSliceColor = Color.Black; //刻度颜色
        private Color[] clrsCurveColors = new Color[] { Color.Red, Color.Blue, Color.Green }; //曲线颜色
        private float fltXSpaceRightOffSet = 40f ; //图像左距离边缘距离偏移值 相对于fltXSpace的偏移
        private float fltXSpaceLeftOffSet = 60f; //图像右距离边缘距离偏移值 Y3轴使用
        private float fltXSpace = 100f ; //图像左右距离边缘距离
        private float fltYSpace = 65f ; //图像上下距离边缘距离
        private int intFontSize = 9 ; //字体大小号数
        private float fltXRotateAngle = 0f ; //X轴文字旋转角度
        private float fltYRotateAngle = 0f; //Y轴文字旋转角度
        private int intCurveSize = 2; //曲线线条大小
        private int intFontSpace = 0; //intFontSpace 是字体大小和距离调整出来的一个比较适合的数字

        #region 公共属性

        /// <summary>
        /// 图像的宽度
        /// </summary>
        public float Width
        {
            set
            {
                //if (value < 100)
                //{
                //    fltWidth = 100;
                //}
                //else
                //{
                    fltWidth = value;
                //}
            }
            get
            {
                //if (fltWidth <= 100)
                //{
                //    return 100;
                //}
                //else
                //{
                    return fltWidth*resolutionW/96;
                //}
            }
        }

        /// <summary>
        /// 图像的高度
        /// </summary>
        public float Height
        {
            set
            {
                //if (value < 100)
                //{
                //    fltHeight = 100;
                //}
                //else
                //{
                    fltHeight = value;
                //}
            }
            get
            {
                //if (fltHeight <= 100)
                //{
                //    return 100;
                //}
                //else
                //{
                    return fltHeight ;
                //}
            }
        }
        public float ResolutionW
        {
            set { resolutionW = value; }
            get { return resolutionW; }
        }
        public float ResolutionH
        {
            set { resolutionH = value; }
            get { return resolutionH; }
        }
        /// <summary>
        /// X轴刻度宽度
        /// </summary>
        public float XSlice
        {
            set { fltXSlice = value; }
            get { return fltXSlice; }
        }

        /// <summary>
        /// Y轴刻度宽度
        /// </summary>
        public float YSlice
        {
            set { fltYSlice = value; }
            get { return fltYSlice ; }
        }

        /// <summary>
        /// Y轴刻度的数值宽度
        /// </summary>
        public float YSliceValue
        {
            set { fltYSliceValue = value; }
            get { return fltYSliceValue ; }
        }

        /// <summary>
        /// Y1轴刻度开始值
        /// </summary>
        public float[] YSliceBegin
        {
            set { fltYSliceBegin = value; }
            get { return fltYSliceBegin ; }
        }
        /// <summary>
        /// Y1轴刻度结束
        /// </summary>
        public float[] YSliceEnd
        {
            set { fltYSliceEnd = value; }
            get { return fltYSliceEnd; }
        }
        /// <summary>
        /// x轴刻度开始
        /// </summary>
        public float XSliceBegin
        {
            set { fltXSliceBegin = value; }
            get { return fltXSliceBegin ; }
        }
        /// <summary>
        /// x轴刻度结束
        /// </summary>
        public float XSliceEnd
        {
            set { fltXSliceEnd = value; }
            get { return fltXSliceEnd ; }
        }
        /// <summary>
        /// 第三轴比第一轴的倍率
        /// </summary>
        public double Tensiony3
        {
            set { tensiony3 = value; }
            get { return tensiony3; }
        }
        /// <summary>
        /// 张力系数
        /// </summary>
        public float Tension
        {
            set
            {
                if (value < 0.0f && value > 1.0f)
                {
                    fltTension = 0.5f;
                }
                else
                {
                    fltTension = value;
                }
            }
            get
            {
                return fltTension;
            }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            set { strTitle = value; }
            get { return strTitle; }
        }

        /// <summary>
        /// 键，X轴数据
        /// </summary>
        public float[] FltsXValues
        {
            set { fltsXValues = value; }
            get { return fltsXValues; }
        }

        /// <summary>
        /// 值，Y轴数据
        /// </summary>
        public float[] FltsY1Values
        {
            set { fltsY1Values = value; }
            get { return fltsY1Values; }
        }
        public float[] FltsY2Values
        {
            set { fltsY2Values = value; }
            get { return fltsY2Values; }
        }
        public float[] FltsY3Values
        {
            set { fltsY3Values = value; }
            get { return fltsY3Values; }
        }
        /// <summary>
        /// 背景色
        /// </summary>
        public Color BgColor
        {
            set { clrBgColor = value; }
            get { return clrBgColor; }
        }

        /// <summary>
        /// 文字颜色
        /// </summary>
        public Color TextColor
        {
            set { clrTextColor = value; }
            get { return clrTextColor; }
        }

        /// <summary>
        /// 整体边框颜色
        /// </summary>
        public Color BorderColor
        {
            set { clrBorderColor = value; }
            get { return clrBorderColor; }
        }

        /// <summary>
        /// 轴线颜色
        /// </summary>
        public Color AxisColor
        {
            set { clrAxisColor = value; }
            get { return clrAxisColor; }
        }

        /// <summary>
        /// X轴说明文字
        /// </summary>
        public string XAxisText
        {
            set { strXAxisText = value; }
            get { return strXAxisText; }
        }

        /// <summary>
        /// Y轴说明文字
        /// </summary>
        public string[] YAxisText
        {
            set { strYAxisText = value; }
            get { return strYAxisText; }
        }

        /// <summary>
        /// 轴说明文字颜色
        /// </summary>
        public Color[] AxisTextColor
        {
            set { clrAxisTextColor = value; }
            get { return clrAxisTextColor; }
        }

        /// <summary>
        /// 刻度文字颜色
        /// </summary>
        public Color SliceTextColor
        {
            set { clrSliceTextColor = value; }
            get { return clrSliceTextColor; }
        }

        /// <summary>
        /// 刻度颜色
        /// </summary>
        public Color SliceColor
        {
            set { clrSliceColor = value; }
            get { return clrSliceColor; }
        }

        /// <summary>
        /// 曲线颜色
        /// </summary>
        public Color[] CurveColors
        {
            set { clrsCurveColors = value; }
            get { return clrsCurveColors; }
        }

        /// <summary>
        /// X轴文字旋转角度
        /// </summary>
        public float XRotateAngle
        {
            get { return fltXRotateAngle; }
            set { fltXRotateAngle = value; }
        }

        /// <summary>
        /// Y轴文字旋转角度
        /// </summary>
        public float YRotateAngle
        {
            get { return fltYRotateAngle; }
            set { fltYRotateAngle = value; }
        }

        /// <summary>
        /// 图像右距离边缘距离偏移
        /// </summary>
        public float XSpaceRightOffSet
        {
            get { return fltXSpaceRightOffSet; }
            set { fltXSpaceRightOffSet = value ; }
        }

        /// <summary>
        /// 图像左距离边缘距离偏移
        /// </summary>
        public float XSpaceLeftOffSet
        {
            get { return fltXSpaceLeftOffSet; }
            set { fltXSpaceLeftOffSet = value ; }
        }
        /// <summary>
        /// 图像左右距离边缘距离
        /// </summary>
        public float XSpace
        {
            get { return fltXSpace; }
            set { fltXSpace = value ; }
        }

        /// <summary>
        /// x轴刻度段数
        /// </summary>
        public int XSpaceNum
        {
            get { return xSpaceNum; }
            set { xSpaceNum = value; }
        }

        /// <summary>
        /// y轴刻度段数
        /// </summary>
        public int[] YSpaceNum
        {
            get { return ySpaceNum; }
            set { ySpaceNum = value; }
        }
        /// <summary>
        /// 图像上下距离边缘距离
        /// </summary>
        public float YSpace
        {
            get { return fltYSpace; }
            set { fltYSpace = value ; }
        }

        /// <summary>
        /// 字体大小号数
        /// </summary>
        public int FontSize
        {
            get { return intFontSize; }
            set { intFontSize = value; }
        }

        /// <summary>
        /// 曲线线条大小
        /// </summary>
        public int CurveSize
        {
            get { return intCurveSize; }
            set { intCurveSize = value; }
        }

        #endregion

        /// <summary>
        /// 自动根据参数调整图像大小
        /// </summary>
        public void Fit()
        {
            //计算字体距离
            intFontSpace = FontSize + 5;
            //计算图像边距
            float fltSpace = Math.Min(Width / 6, Height / 6);
            
            YSpace = fltSpace;
            GetAutoScaleAxis();
            //YSlice = (Height - 2 * YSpace) / intYSliceCount/3;
        }

        /// <summary>
        /// 生成图像并返回bmp图像对象
        /// </summary>
        /// <returns></returns>
        public Bitmap CreateImage()
        {
            InitializeGraph();
            DrawContent(ref objGraphics, CurveColors);
            //}
            //else
            //{
            //    objGraphics.DrawString("发生错误，Values的长度必须是Keys的整数倍!", new Font("宋体", FontSize + 5), new SolidBrush(TextColor), new Point((int)XSpace, (int)(Height / 2)));
            //}

            return objBitmap;
        }

        /// <summary>
        /// 初始化和填充图像区域，画出边框，初始标题
        /// </summary>
        private void InitializeGraph()
        {

            //根据给定的高度和宽度创建一个位图图像
            objBitmap = new Bitmap((int)Width, (int)Height);
            objBitmap.SetResolution(ResolutionW, ResolutionH);

            //从指定的 objBitmap 对象创建 objGraphics 对象 (即在objBitmap对象中画图)
            objGraphics = Graphics.FromImage(objBitmap);

            //根据给定颜色(LightGray)填充图像的矩形区域 (背景)
            objGraphics.DrawRectangle(new Pen(BorderColor, 1), 0, 0, Width - 1, Height - 1); //画边框
            objGraphics.FillRectangle(new SolidBrush(BgColor), 1, 1, Width - 2, Height - 2); //填充边框

            objGraphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            objGraphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            //objGraphics.SmoothingMode = SmoothingMode.HighQuality;  //图片柔顺模式选择
            //objGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;//高质量

            //objGraphics.CompositingQuality = CompositingQuality.HighQuality;//再加一点


            //画X轴,注意图像的原始X轴和Y轴计算是以左上角为原点，向右和向下计算的
            float fltX1 = XSpace;
            float fltY1 = Height - YSpace;
            float fltX2 = Width - XSpace + XSpaceRightOffSet/*+ XSlice / 2*/;
            float fltY2 = fltY1;
            objGraphics.DrawLine(new Pen(new SolidBrush(AxisColor), 1.5f), fltX1, fltY1, fltX2, fltY2);

            //画Y1轴
            fltX1 = XSpace;
            fltY1 = Height - YSpace;
            fltX2 = XSpace;
            fltY2 = YSpace /*- YSlice / 2*/;
            objGraphics.DrawLine(new Pen(new SolidBrush(AxisColor), 1.5f), fltX1, fltY1, fltX2, fltY2);

            //画Y2轴
            fltX1 = Width - XSpace + XSpaceRightOffSet;
            fltY1 = Height - YSpace;
            fltX2 = Width - XSpace + XSpaceRightOffSet;
            fltY2 = YSpace /*- YSlice / 2*/;
            objGraphics.DrawLine(new Pen(new SolidBrush(AxisColor), 1.5f), fltX1, fltY1, fltX2, fltY2);

            //画Y3轴
            fltX1 = XSpace - XSpaceLeftOffSet;
            fltY1 = Height - YSpace;
            fltX2 = XSpace - XSpaceLeftOffSet;
            fltY2 = Convert.ToSingle(Height - YSpace - Tensiony3 * (Height - 2 * YSpace)); /*- YSlice / 2*/;
            objGraphics.DrawLine(new Pen(new SolidBrush(AxisColor), 1.5f), fltX1, fltY1, fltX2, fltY2);



            //初始化Y轴上的刻度和文字
            SetY1Axis(ref objGraphics);
            SetY2Axis(ref objGraphics);
            SetY3Axis(ref objGraphics);


            //初始化轴线说明文字
            SetAxisText(ref objGraphics);

            //初始化X轴上的刻度和文字
            SetXAxis(ref objGraphics);
       
            //初始化标题
            CreateTitle(ref objGraphics);
        }

        /// <summary>
        /// 初始化轴线说明文字
        /// </summary>
        /// <param name="objGraphics"></param>
        private void SetAxisText(ref Graphics objGraphics)
        {
           
            Font fontFlt = new Font("宋体", FontSize);
            Font fontStr = new Font("楷体", FontSize, FontStyle.Bold);
      
            Graphics graphics_x = Graphics.FromHwnd(IntPtr.Zero);
            string Text_x = $"{XAxisText}";
            SizeF sizeFx = graphics_x.MeasureString(Text_x, fontFlt);

            float fltSliceWidth = YSlice / 10 * 1.5f; //刻度线的宽度
            float fltX = Width - XSpace + XSpaceRightOffSet - sizeFx.Width;
            float fltY = Height - YSpace + intFontSpace + 9;
            objGraphics.DrawString(XAxisText, new Font("宋体", FontSize), new SolidBrush(AxisTextColor[0]), fltX, fltY);



            ///////
            Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
            string y1Text = $"{YSliceEnd[0]}";
            SizeF sizeF = graphics.MeasureString(y1Text, fontFlt);


            Graphics graphics1 = Graphics.FromHwnd(IntPtr.Zero);
            string y1TextS = $"{YAxisText[0]}";
            SizeF sizeF1 = graphics1.MeasureString(y1TextS, fontStr);

            fltX = XSpace - YSlice / 10 * 1.5f - sizeF.Width - 20;
            fltY = YSpace + sizeF1.Width - 8;
            objGraphics.TranslateTransform(fltX, fltY);
            objGraphics.RotateTransform(270);
            objGraphics.DrawString(YAxisText[0], fontStr, new SolidBrush(AxisTextColor[1]), 0, 0);
            objGraphics.ResetTransform();


            /////
            Graphics graphics2 = Graphics.FromHwnd(IntPtr.Zero);
            y1Text = $"{YSliceEnd[1]}";
            sizeF = graphics2.MeasureString(y1Text, fontFlt);


            Graphics graphics3 = Graphics.FromHwnd(IntPtr.Zero);
            y1TextS = $"{YAxisText[1]}";
            sizeF1 = graphics3.MeasureString(y1TextS, fontStr);

            fltX = Width - XSpace + XSpaceRightOffSet + YSlice / 10 * 1.5f + sizeF.Width + 5;
            fltY = YSpace + sizeF1.Width - 8;
            objGraphics.TranslateTransform(fltX, fltY);
            objGraphics.RotateTransform(270);
            objGraphics.DrawString(YAxisText[1], fontStr, new SolidBrush(AxisTextColor[2]), 0, 0);
            objGraphics.ResetTransform();

            ////
            Graphics graphics4 = Graphics.FromHwnd(IntPtr.Zero);
            y1TextS = $"{YAxisText[2]}";
            sizeF1 = graphics4.MeasureString(y1TextS, fontStr);

            fltX = XSpace - XSpaceRightOffSet /*/*+ YSlice / 10 * 1.5f*/ - sizeF.Height;
            fltY = Height - YSpace;
            objGraphics.TranslateTransform(fltX, fltY);
            objGraphics.RotateTransform(270);
            objGraphics.DrawString(YAxisText[2], fontStr, new SolidBrush(AxisTextColor[3]), 0, 0);
            objGraphics.ResetTransform();
        }

        /// <summary>
        /// 向上获取是10的整数倍的数
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private int RoundUpToNearestTen(float n)
        {
            //float flt = (((int)n) + 9) / 10 * 10;
            //return (int)flt;
            // 优先尝试向上取5的倍数
            double multipleOf5 = Math.Ceiling(n / 5) * 5;
            if (multipleOf5 > n)
            {
                return (int)multipleOf5;
            }

            // 如果不能取5的倍数，尝试向上取6的倍数
            double multipleOf6 = Math.Ceiling(n / 6) * 6;
            if (multipleOf6 > n)
            {
                return (int)multipleOf6;
            }

            // 如果不能取6的倍数，取10的倍数
            double multipleOf10 = Math.Ceiling(n / 10) * 10;
            return (int)multipleOf10;
        }

        /// <summary>
        ///  向下获取是10的整数倍的数
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private int RoundDownToNearestTen(float n)
        {
            
            float flt = ((int)n) / 10 * 10;
            return (int)flt;
        }

        public  void AutoScaleAxis(float[] data, out double axisMin, out double axisMax, out double spaceNum, out double tickInterval)
        {
            // Step 1: 获取数据的最小值和最大值
            double dataMin = data.Min();
            double dataMax = data.Max();
            //foreach (var value in data)
            //{
            //    if (value < dataMin) dataMin = value;
            //    if (value > dataMax) dataMax = value;
            //}

            // Step 2: 确定数据范围
            double dataRange = dataMax - dataMin;

            // 如果范围太小，我们提供一个最小的范围
            if (dataRange == 0)
            {
                dataRange = Math.Abs(dataMin) * 0.1; // 防止范围为零
            }

            // Step 3: 确定初步的轴最小值和最大值，扩大一些范围
            //axisMin = Math.Floor(dataMin - dataRange * 0.1); // 将最小值往下调整10%
            axisMax = Math.Ceiling(dataMax + dataRange * 0.05); // 将最大值往上调整10%
           
            // Step 4: 计算合适的刻度间隔
            double rawTickInterval = dataRange / 5; // 我们希望有大约5到10个刻度
            tickInterval = CalculateNiceNumber(rawTickInterval, true); // 调整刻度到“漂亮”的数值


            // Step 5: 调整最小值和最大值，使其是刻度间隔的整数倍
            axisMin = 0;// Math.Floor(axisMin / tickInterval) * tickInterval;
            axisMax = Math.Ceiling(axisMax / tickInterval) * tickInterval;

            spaceNum = Math.Ceiling((axisMax - axisMin) / tickInterval);
        }

        // 计算漂亮的刻度数，比如 1, 2, 5, 10 的倍数
        private double CalculateNiceNumber(double range, bool round)
        {
            double exponent = Math.Floor(Math.Log10(range)); // 获取10的次幂
            double fraction = range / Math.Pow(10, exponent); // 计算小数部分

            double niceFraction;
            if (round)
            {
                if (fraction < 1 )
                    
                     niceFraction = 1;
                else if (fraction < 2 )
                    
                     niceFraction = 2;
                else if (fraction < 5 )
                    
                     niceFraction = 5;

                else
                    niceFraction = 10;
            }
            else
            {
                if (fraction <= 1 )
                    niceFraction = 1;
                else if (fraction <= 2 )
                    niceFraction = 2;
                else if (fraction <= 5 )
                    niceFraction = 5 ;
                else
                    niceFraction = 10;
            }

            return niceFraction * Math.Pow(10, exponent);
        }

        /// <summary>
        /// 获取数组最大 最小
        /// </summary>  
        private void GetAutoScaleAxis()
        {
            AutoScaleAxis(FltsXValues, out double axisMinx, out double axisMaxx, out double ySpaceNumx, out double tickIntervalx);
            AutoScaleAxis(FltsY1Values, out double axisMiny1, out double axisMaxy1, out double ySpaceNumy1, out double tickIntervaly1);
            AutoScaleAxis(FltsY2Values, out double axisMiny2, out double axisMaxy2, out double ySpaceNumy2, out double tickIntervaly2);
            AutoScaleAxis(FltsY3Values, out double axisMiny3, out double axisMaxy3, out double ySpaceNumy3, out double tickIntervaly3);
            //float maxInFltY1 = FltsY1Values.Max();
            //float minInFltY1 = FltsY1Values.Min();
            //YSliceEnd[0] = RoundUpToNearestTen(maxInFltY1);
            //YSliceBegin[0] = 0; RoundDownToNearestTen(minInFltY1);

            XSliceEnd = Convert.ToSingle(axisMaxx);
             XSliceBegin = Convert.ToSingle(axisMinx);
            YSliceEnd[0] = Convert.ToSingle(axisMaxy1);
            YSliceBegin[0] = Convert.ToSingle(axisMiny1);
            YSliceEnd[1] = Convert.ToSingle(axisMaxy2);
            YSliceBegin[1] = Convert.ToSingle(axisMiny2);
            YSliceEnd[2] = Convert.ToSingle(axisMaxy3);
            YSliceBegin[2] = Convert.ToSingle(axisMiny3);
            XSpaceNum = (int)ySpaceNumx;
            YSpaceNum[0] = (int)ySpaceNumy1;
            YSpaceNum[1] = (int)ySpaceNumy2;
            YSpaceNum[2] = (int)ySpaceNumy3;
            //float maxInFltY2 = FltsY2Values.Max();
            //float minInFltY2 = FltsY2Values.Min();
            //YSliceEnd[1] = RoundUpToNearestTen(maxInFltY2);
            //YSliceBegin[1] = 0; RoundDownToNearestTen(minInFltY2);

            //float maxInFltY3 = FltsY3Values.Max();
            //float minInFltY3 = FltsY3Values.Min();
            //YSliceEnd[2] = RoundUpToNearestTen(maxInFltY3);
            //YSliceBegin[2] = 0; RoundDownToNearestTen(minInFltY3);

            //float maxInFltX = FltsXValues.Max();
            //float minInFltX = FltsXValues.Min();
            //XSliceEnd = RoundUpToNearestTen(maxInFltX);
            //XSliceBegin = RoundDownToNearestTen(minInFltX);

            float actuallen = Width - 2 * XSpace + XSpaceRightOffSet;
            float minAxisLen = (actuallen + 9) / 10 * 10;
            int fsf = 1;
        }

        /// <summary>
        /// 初始化X轴上的刻度和文字
        /// </summary>
        /// <param name="objGraphics"></param>
        private void SetXAxis(ref Graphics objGraphics)
        {

            float fltX1 = XSpace;
            float fltY1 = Height - YSpace;
            float fltX2 = XSpace;
            float fltY2 = Height - YSpace;
            int iCount = 0;
            int iSliceCount = 1;
            float Scale = 0;
            float iWidth = (Width - 2 * XSpace + XSpaceRightOffSet) / (XSpaceNum * 5); //每等份又分为5份  总分成XSpaceNum*5份的长度
            float fltSliceHeight = XSlice / 10; //刻度线的高度
            float xPerLen = (XSliceEnd - XSliceBegin) / XSpaceNum;//x轴每一等分显示长度
            objGraphics.TranslateTransform(fltX1, fltY1); //平移图像(原点)
            objGraphics.RotateTransform(XRotateAngle, MatrixOrder.Prepend); //旋转图像
            Font font = new Font("宋体", FontSize);
            objGraphics.DrawString(XSliceBegin.ToString(), font, new SolidBrush(SliceTextColor), -(int)font.GetHeight() / 2, (int)font.GetHeight() / 2);
            objGraphics.ResetTransform(); //重置图像

            for (int i = 0; i <= XSpaceNum * 5; i++)
            {
                Scale = i * iWidth;
                if (iCount == 5)
                {
                    //画主网格刻度
                    objGraphics.DrawLine(new Pen(new SolidBrush(AxisColor)), fltX1 + Scale, fltY1 + fltSliceHeight * 1.5f, fltX2 + Scale, fltY2 - fltSliceHeight * 1.5f);
                    //画网格虚线
                    Pen penDashed = new Pen(new SolidBrush(AxisColor));
                    penDashed.DashStyle = DashStyle.Dash;
                    objGraphics.DrawLine(penDashed, fltX1 + Scale, fltY1, fltX2 + Scale, YSpace /*- YSlice / 2*/);
                    //这里显示X轴刻度字符串
                    //if (iSliceCount <= FltsXValues.Length - 1)
                    //{
                    objGraphics.TranslateTransform(fltX1 + Scale, fltY1);
                    objGraphics.RotateTransform(XRotateAngle, MatrixOrder.Prepend);
                    objGraphics.DrawString((XSliceBegin + xPerLen * (iSliceCount)).ToString(), font, new SolidBrush(SliceTextColor), -(int)font.GetHeight() / 2, (int)font.GetHeight() / 2);
                    objGraphics.ResetTransform();
                    //}
                    //else
                    //{
                    //    //超过范围，不画任何刻度文字
                    //}
                    iCount = 0;
                    iSliceCount++;
                    //if (fltX1 + Scale > Width - XSpace)
                    //{
                    //    break;
                    //}
                }
                else
                {
                    //画次网格刻度
                    // objGraphics.DrawLine(new Pen(new SolidBrush(SliceColor)), fltX1 + Scale, fltY1 + fltSliceHeight, fltX2 + Scale, fltY2 - fltSliceHeight);
                }
                iCount++;
            }
        }

        /// <summary>
        /// 初始化Y1轴上的刻度和文字
        /// </summary>
        /// <param name="objGraphics"></param>
        private void SetY1Axis(ref Graphics objGraphics)
        {
            float fltX1 = XSpace;
            float fltY1 = Height - YSpace;
            float fltX2 = XSpace;
            float fltY2 = Height - YSpace;
            int iCount = 0;
            float Scale = 0;
            int iSliceCount = 1;
            float iHeight = (Height - 2 * YSpace) / (YSpaceNum[0] * 5);
            float yPerLen = (YSliceEnd[0] - YSliceBegin[0]) / YSpaceNum[0];//y轴每一等分显示长度
            float fltSliceWidth = YSlice / 10; //刻度线的宽度
            string strSliceText = string.Empty;

            objGraphics.TranslateTransform(XSpace - intFontSpace * YSliceBegin[0].ToString().Length, Height - YSpace); //平移图像(原点)
            objGraphics.RotateTransform(YRotateAngle, MatrixOrder.Prepend); //旋转图像
            Font font = new Font("宋体", FontSize);
            //0点刻度值
            objGraphics.DrawString(YSliceBegin[0].ToString(), font, new SolidBrush(SliceTextColor), 0, -(int)font.GetHeight() / 2);
            objGraphics.ResetTransform(); //重置图像

            for (int i = 0; i <= YSpaceNum[0] * 5; i++)
            {
                Scale = i * iHeight;

                if (iCount == 5)
                {
                    //画主网格刻度
                    objGraphics.DrawLine(new Pen(new SolidBrush(AxisColor)), fltX1 - fltSliceWidth * 1.5f, fltY1 - Scale, fltX2 + fltSliceWidth * 1.5f, fltY2 - Scale);
                    //画网格虚线
                    Pen penDashed = new Pen(new SolidBrush(AxisColor));
                    penDashed.DashStyle = DashStyle.Dash;
                    objGraphics.DrawLine(penDashed, XSpace, fltY1 - Scale, Width - XSpace + XSpaceRightOffSet /*+ XSlice / 2*/, fltY2 - Scale);
                    //这里显示Y轴刻度数值
                    strSliceText = Convert.ToString(YSliceBegin[0] + yPerLen * (iSliceCount));
                    objGraphics.TranslateTransform(XSpace - intFontSize * strSliceText.Length, fltY1 - Scale); //平移图像(原点)
                    objGraphics.RotateTransform(YRotateAngle, MatrixOrder.Prepend); //旋转图像
                    int higtOffset = -(int)font.GetHeight() / 2;
                    objGraphics.DrawString((YSliceBegin[0] + yPerLen * (iSliceCount)).ToString(), font, new SolidBrush(SliceTextColor), 0, higtOffset);
                    objGraphics.ResetTransform(); //重置图像

                    iCount = 0;
                    iSliceCount++;
                }
                else
                {
                    //画次网格刻度
                    // objGraphics.DrawLine(new Pen(new SolidBrush(SliceColor)), fltX1 - fltSliceWidth, fltY1 - Scale, fltX2 + fltSliceWidth, fltY2 - Scale);
                }
                iCount++;
            }
        }
        /// <summary>
        /// 初始化Y2轴上的刻度和文字
        /// </summary>
        /// <param name="objGraphics"></param>
        private void SetY2Axis(ref Graphics objGraphics)
        {
            float fltX1 = Width - XSpace + XSpaceRightOffSet;
            float fltY1 = Height - YSpace;
            float fltX2 = Width - XSpace + XSpaceRightOffSet;
            float fltY2 = Height - YSpace;
            int iCount = 0;
            float Scale = 0;
            int iSliceCount = 1;
            float iHeight = (Height - 2 * YSpace) / (YSpaceNum[1] * 5);
            float yPerLen = (YSliceEnd[1] - YSliceBegin[1]) / YSpaceNum[1];//y轴每一等分显示长度
            float fltSliceWidth = YSlice / 10; //刻度线的宽度
            string strSliceText = string.Empty;
            Font font = new Font("宋体", FontSize);
            int higtOffset = -(int)font.GetHeight() / 2;
            objGraphics.TranslateTransform(Width - XSpace + XSpaceRightOffSet + fltSliceWidth * 1.5f, Height - YSpace); //平移图像(原点)
            objGraphics.RotateTransform(YRotateAngle, MatrixOrder.Prepend); //旋转图像
            objGraphics.DrawString(YSliceBegin[1].ToString(), font, new SolidBrush(SliceTextColor), 0, higtOffset);
            objGraphics.ResetTransform(); //重置图像

            for (int i = 0; i <= YSpaceNum[1] * 5; i++)
            {
                Scale = i * iHeight;

                if (iCount == 5)
                {
                    objGraphics.DrawLine(new Pen(new SolidBrush(AxisColor)), fltX1 - fltSliceWidth * 1.5f, fltY1 - Scale, fltX2 + fltSliceWidth * 1.5f, fltY2 - Scale);
                    ////画网格虚线
                    //Pen penDashed = new Pen(new SolidBrush(AxisColor));
                    //penDashed.DashStyle = DashStyle.Dash;
                    //objGraphics.DrawLine(penDashed, XSpace, fltY1 - Scale, Width - XSpace + XSlice / 2, fltY2 - Scale);
                    //这里显示Y轴刻度
                    strSliceText = Convert.ToString(YSliceBegin[1] + yPerLen * (iSliceCount));
                    objGraphics.TranslateTransform(Width - XSpace + XSpaceRightOffSet + fltSliceWidth * 1.5f, fltY1 - Scale); //平移图像(原点)
                    objGraphics.RotateTransform(YRotateAngle, MatrixOrder.Prepend); //旋转图像
                    objGraphics.DrawString((YSliceBegin[1] + yPerLen * (iSliceCount)).ToString(), font, new SolidBrush(SliceTextColor), 0, higtOffset);
                    objGraphics.ResetTransform(); //重置图像

                    iCount = 0;
                    iSliceCount++;
                }
                else
                {
                    //objGraphics.DrawLine(new Pen(new SolidBrush(SliceColor)), fltX1 - fltSliceWidth, fltY1 - Scale, fltX2 + fltSliceWidth, fltY2 - Scale);
                }
                iCount++;
            }
        }

        /// <summary>
        /// 初始化Y3轴上的刻度和文字
        /// </summary>
        /// <param name="objGraphics"></param>
        private void SetY3Axis(ref Graphics objGraphics)
        {
            float fltX1 = XSpace - XSpaceLeftOffSet; ;
            float fltY1 = Height - YSpace;
            float fltX2 = XSpace - XSpaceLeftOffSet;
            float fltY2 = Height - YSpace;
            int iCount = 0;
            float Scale = 0;
            int iSliceCount = 1;
            float iHeight = Convert.ToSingle(tensiony3 * (Height - 2 * YSpace) / (YSpaceNum[2] * 5));
            float yPerLen = (YSliceEnd[2] - YSliceBegin[2]) / YSpaceNum[2];//y轴每一等分显示长度
            float fltSliceWidth = YSlice / 10; //刻度线的宽度
            string strSliceText = string.Empty;

            objGraphics.TranslateTransform(fltX1 - intFontSpace * YSliceBegin[2].ToString().Length, fltY1); //平移图像(原点)
            objGraphics.RotateTransform(YRotateAngle, MatrixOrder.Prepend); //旋转图像
            Font font = new Font("宋体", FontSize);

            //0点刻度值
            objGraphics.DrawString(YSliceBegin[2].ToString(), font, new SolidBrush(SliceTextColor), 0, -(int)font.GetHeight() / 2);
            objGraphics.ResetTransform(); //重置图像
                                          //0刻度
            objGraphics.DrawLine(new Pen(new SolidBrush(AxisColor)), fltX1 - fltSliceWidth * 1.5f, fltY1, fltX2 + fltSliceWidth * 1f, fltY2);


            for (int i = 0; i <= YSpaceNum[2] * 5; i++)
            {
                Scale = i * iHeight;

                if (iCount == 5)
                {
                    //画主网格刻度
                    objGraphics.DrawLine(new Pen(new SolidBrush(AxisColor)), fltX1 - fltSliceWidth * 1.5f, fltY1 - Scale, fltX2 + fltSliceWidth * 1f, fltY2 - Scale);

                    //这里显示Y轴刻度数值
                    strSliceText = Convert.ToString(YSliceBegin[2] + yPerLen * (iSliceCount));
                    objGraphics.TranslateTransform(fltX1 - intFontSize * strSliceText.Length, fltY1 - Scale); //平移图像(原点)
                    objGraphics.RotateTransform(YRotateAngle, MatrixOrder.Prepend); //旋转图像
                    int higtOffset = -(int)font.GetHeight() / 2;
                    objGraphics.DrawString((YSliceBegin[0] + yPerLen * (iSliceCount)).ToString(), font, new SolidBrush(SliceTextColor), 0, higtOffset);
                    objGraphics.ResetTransform(); //重置图像

                    iCount = 0;
                    iSliceCount++;
                }
                else
                {
                    //画次网格刻度
                    // objGraphics.DrawLine(new Pen(new SolidBrush(SliceColor)), fltX1 - fltSliceWidth, fltY1 - Scale, fltX2 + fltSliceWidth, fltY2 - Scale);
                }
                iCount++;
            }
        }
        /// <summary>
        /// 画曲线
        /// </summary>
        /// <param name="objGraphics"></param>
        private void DrawContent(ref Graphics objGraphics/*, float[] fltCurrentValue*/, Color[] clrCurrentColor)
        {


            Pen CurvePen1 = new Pen(clrCurrentColor[0], CurveSize);
            Pen CurvePen2 = new Pen(clrCurrentColor[1], CurveSize);
            Pen CurvePen3 = new Pen(clrCurrentColor[2], CurveSize);

            objGraphics.SmoothingMode = SmoothingMode.HighQuality;  //图片柔顺模式选择
            objGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;//高质量
            objGraphics.CompositingQuality = CompositingQuality.HighQuality;//再加一点

            //设置裁剪区域 矩形
            Rectangle clipresct = new Rectangle(Convert.ToInt16(XSpace - XSpaceLeftOffSet), Convert.ToInt16(YSpace), Convert.ToInt16(Width - XSpace + XSpaceRightOffSet), Convert.ToInt16(Height - 2 * YSpace));
            objGraphics.SetClip(clipresct);


            PointF[] CurvePointF = new PointF[FltsXValues.Length];
            List<PointF[]> pointFs = ScalePoint(FltsXValues, FltsY1Values, FltsY2Values, FltsY3Values);
            objGraphics.DrawCurve(CurvePen1, pointFs[0], Tension);
            objGraphics.DrawCurve(CurvePen2, pointFs[1], Tension);
            // 定义"⚪"的大小
            int circleDiameter = 8;
            Brush brush = new SolidBrush(clrCurrentColor[0]);
            foreach (PointF point in pointFs[3])
            {
                objGraphics.FillEllipse(brush, point.X - circleDiameter / 2, point.Y - circleDiameter / 2, circleDiameter, circleDiameter);
            }
            // 定义"X"的大小
            int size = 8;
            Pen pen = new Pen(clrCurrentColor[1], 2);
            // 绘制每个坐标上的"X"符号
            foreach (PointF point in pointFs[4])
            {
                // 绘制两条对角线形成X符号
                objGraphics.DrawLine(pen, point.X - size / 2, point.Y - size / 2, point.X + size / 2, point.Y + size / 2);
                objGraphics.DrawLine(pen, point.X - size / 2, point.Y + size / 2, point.X + size / 2, point.Y - size / 2);
            }
            objGraphics.ResetClip();

            //设置裁剪区域 矩形
            clipresct = new Rectangle(Convert.ToInt16(XSpace - XSpaceLeftOffSet), Convert.ToInt16(Height - YSpace - Tensiony3 * (Height - 2 * YSpace)), Convert.ToInt16(Width - XSpace + XSpaceRightOffSet), Convert.ToInt16(Tensiony3 * (Height - 2 * YSpace)));
            objGraphics.SetClip(clipresct);

            // 定义三角形的边长
            int sideLength = 8; // 可以调整为你需要的边长
            double height = sideLength * Math.Sqrt(3) / 2;
            pen = new Pen(clrCurrentColor[2], 2);
            // 遍历每个点
            foreach (PointF point in pointFs[5])
            {
                // 计算三角形的三个顶点
                Point[] triangle = {
                new Point(Convert.ToInt16(point.X), Convert.ToInt16(point.Y - (int)(height / 2))), // 顶点
                new Point(Convert.ToInt16(point.X - sideLength / 2), Convert.ToInt16(point.Y + (int)(height / 2))), // 左下点
                new Point(Convert.ToInt16(point.X + sideLength / 2), Convert.ToInt16(point.Y + (int)(height / 2))) }; // 右下点         
                // 绘制三角形
                objGraphics.DrawPolygon(pen, triangle);
            }
            objGraphics.DrawCurve(CurvePen3, pointFs[2], Tension);

            objGraphics.ResetClip();
        }

        private List<PointF[]> ScalePoint(float[] x, float[] y1, float[] y2, float[] y3)
        {
            double[] fx = Array.ConvertAll(x, item => (double)item);
            double[] fy1 = Array.ConvertAll(y1, item => (double)item);
            double[] fy2 = Array.ConvertAll(y2, item => (double)item);
            double[] fy3 = Array.ConvertAll(y3, item => (double)item);

            double[] ratio1 = new double[3];
            double[] ratio2 = new double[3];
            double[] ratio3 = new double[3];

            ratio1 = FittingFunct.TowTimesCurve(fy1, fx);
            ratio2 = FittingFunct.TowTimesCurve(fy2, fx);
            ratio3 = FittingFunct.TowTimesCurve(fy3, fx);


            double[] y1_fit = new double[x.Length];
            double[] y2_fit = new double[x.Length];
            double[] y3_fit = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                y1_fit[i] = ratio1[0] + ratio1[1] * x[i] + ratio1[2] * x[i] * x[i];
                y2_fit[i] = ratio2[0] + ratio2[1] * x[i] + ratio2[2] * x[i] * x[i];
                y3_fit[i] = ratio3[0] + ratio3[1] * x[i] + ratio3[2] * x[i] * x[i];
            }

            List<PointF[]> listPointFs = new List<PointF[]>();
            listPointFs.Clear();
            PointF[] CurvePointF1 = new PointF[x.Length];
            PointF[] CurvePointF2 = new PointF[x.Length];
            PointF[] CurvePointF3 = new PointF[x.Length];
            PointF[] CurvePointF1_Init = new PointF[x.Length];
            PointF[] CurvePointF2_Init = new PointF[x.Length];
            PointF[] CurvePointF3_Init = new PointF[x.Length];
            float xScale = 0;
            float y1Scale = 0;
            float y2Scale = 0;
            float y3Scale = 0;
            float y4Scale = 0;
            float y5Scale = 0;
            float y6Scale = 0;
            for (int i = 0; i < x.Length; i++)
            {
                xScale = Convert.ToSingle((Width - 2 * XSpace + XSpaceRightOffSet) * (x[i] - XSliceBegin) / (XSliceEnd - XSliceBegin) + XSpace);

                y1Scale = Convert.ToSingle(Height - YSpace - (Height - 2 * YSpace) * (y1_fit[i] - YSliceBegin[0]) / (YSliceEnd[0] - YSliceBegin[0]));
                y2Scale = Convert.ToSingle(Height - YSpace - (Height - 2 * YSpace) * (y2_fit[i] - YSliceBegin[1]) / (YSliceEnd[1] - YSliceBegin[1]));
                y3Scale = Convert.ToSingle(Height - YSpace - Tensiony3 * (Height - 2 * YSpace) * (y3_fit[i] - YSliceBegin[2]) / (YSliceEnd[2] - YSliceBegin[2]));

                y4Scale = Convert.ToSingle(Height - YSpace - (Height - 2 * YSpace) * (y1[i] - YSliceBegin[0]) / (YSliceEnd[0] - YSliceBegin[0]));
                y5Scale = Convert.ToSingle(Height - YSpace - (Height - 2 * YSpace) * (y2[i] - YSliceBegin[1]) / (YSliceEnd[1] - YSliceBegin[1]));
                y6Scale = Convert.ToSingle(Height - YSpace - Tensiony3 * (Height - 2 * YSpace) * (y3[i] - YSliceBegin[2]) / (YSliceEnd[2] - YSliceBegin[2]));
                CurvePointF1[i] = new PointF(xScale, y1Scale);
                CurvePointF2[i] = new PointF(xScale, y2Scale);
                CurvePointF3[i] = new PointF(xScale, y3Scale);
                CurvePointF1_Init[i] = new PointF(xScale, y4Scale);
                CurvePointF2_Init[i] = new PointF(xScale, y5Scale);
                CurvePointF3_Init[i] = new PointF(xScale, y6Scale);
            }
            listPointFs.Add(CurvePointF1);
            listPointFs.Add(CurvePointF2);
            listPointFs.Add(CurvePointF3);
            listPointFs.Add(CurvePointF1_Init);
            listPointFs.Add(CurvePointF2_Init);
            listPointFs.Add(CurvePointF3_Init);
            return listPointFs;
        }
        /// <summary>
        /// 初始化标题
        /// </summary>
        /// <param name="objGraphics"></param>
        private void CreateTitle(ref Graphics objGraphics)
        {
        
            objGraphics.DrawString(Title, new Font("宋体", 15, FontStyle.Bold), new SolidBrush(TextColor), new Point((int)(0.5 * Width - 0.5 * intFontSize * (Title.Length + 4)), (int)(YSlice / 2 + intFontSpace)));
        }
    }
}
