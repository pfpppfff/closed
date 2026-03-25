
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;
using ClosedXML.Excel;
using System.IO;
using OfficeOpenXml.Drawing;

namespace closed
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            btnStartImport.Click += BtnStartImport_Click;
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        private void BtnStartImport_Click(object sender, EventArgs e)
        {
            // 显示进度条窗体
            ProgressForm progressForm = new ProgressForm();
            progressForm.Show();

            // 开启一个任务来处理数据导入
            Task.Run(() =>
            {
                ExcelImporter importer = new ExcelImporter(progressForm);
                importer.ImportData();
            });
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Red;
            dataGridView1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 定义文件路径

            //CreatChart();


        }

        private void createxcel3()
        {
            double[] x = { 23.18, 32.22, 39.74, 44.08, 51.98, 62.42, 66.29, 69.15, 71.49 };
            double[] y1 = { 34.95, 35.23, 33.44, 32.33, 30.09, 25.08, 23.22, 21.59, 20.19 };
            double[] y2 = { 48.59, 57.89, 60.98, 61.96, 62.53, 57.79, 54.37, 53.11, 50.77 };

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Chart");

                // Insert X, Y1, and Y2 data into the worksheet
                for (int i = 0; i < x.Length; i++)
                {
                    worksheet.Cells[i + 1, 1].Value = x[i];
                    worksheet.Cells[i + 1, 2].Value = y1[i];
                    worksheet.Cells[i + 1, 3].Value = y2[i];
                }

                // Create the chart
                var chart = worksheet.Drawings.AddChart("SecondaryChart", eChartType.XYScatter);
                chart.Title.Text = "Scatter Plot with Trendlines";
                chart.SetPosition(5, 0, 0, 0);  // Position of the chart
                chart.SetSize(600, 400);        // Size of the chart

                // Add first series (Y1) to primary axis
                var series1 = chart.Series.Add(worksheet.Cells["B1:B9"], worksheet.Cells["A1:A9"]);
                series1.Header = "Series 1";
                //series1.Marker.Style = eMarkerStyle.Dot;
                // Add second series (Y2) to secondary axis
                var series2 = chart.Series.Add(worksheet.Cells["C1:C9"], worksheet.Cells["A1:A9"]);
                series2.Header = "Series 2";
                //series1.Marker.Style = eMarkerStyle.Circle;


                //// Add trendlines to series1 (both polynomial and linear)
                //var trendline1Poly = series1.TrendLines.Add(eTrendLine.Polynomial);
                //trendline1Poly.DisplayEquation = false;
                //trendline1Poly.DisplayRSquaredValue = false;

                //trendline1Poly.Order = 2;  // Polynomial order

                ////var trendline1Linear = series1.TrendLines.Add(eTrendLine.Linear);

                //// Add trendlines to series2 (both polynomial and linear)
                //var trendline2Poly = series2.TrendLines.Add(eTrendLine.Polynomial);
                //trendline2Poly.DisplayEquation = false;
                //trendline2Poly.DisplayRSquaredValue = false;
                //trendline2Poly.Order = 2;  // Polynomial order

                //var trendline2Linear = series2.TrendLines.Add(eTrendLine.Linear);

                // Save the workbook to a file
                var fileInfo = new FileInfo("ChartWithTrendlines.xlsx");
                package.SaveAs(fileInfo);
            }
        }

        private void createxcel()
        {
            string filePath = "PerformanceCurve.xlsx";

            // 使用ClosedXML创建Excel文件并添加数据
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Performance Curve");

                // 添加数据列
                worksheet.Cell("A1").Value = "Flow(m^3/h)";
                worksheet.Cell("B1").Value = "Head (m)";
                worksheet.Cell("C1").Value = "Efficiency (%)";
                worksheet.Cell("D1").Value = "Power (kW)";

                // 添加示例数据
                var flowData = new double[] { 0, 10, 20, 30, 40, 50, 60, 70 };
                var headData = new double[] { 35, 34, 33, 32, 30, 28, 25, 20 };
                var efficiencyData = new double[] { 60, 59, 58, 57, 55, 52, 48, 43 };
                var powerData = new double[] { 5, 5.2, 5.4, 5.7, 6, 6.4, 7, 7.5 };

                for (int i = 0; i < flowData.Length; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = flowData[i];
                    worksheet.Cell(i + 2, 2).Value = headData[i];
                    worksheet.Cell(i + 2, 3).Value = efficiencyData[i];
                    worksheet.Cell(i + 2, 4).Value = powerData[i];
                }

                // 保存文件
                workbook.SaveAs(filePath);
            }

            // 使用EPPlus添加图表
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets["Performance Curve"];

                // 创建主图表 (Head vs Flow)
                var chart = worksheet.Drawings.AddChart("PerformanceCurve", eChartType.XYScatterSmooth);
                chart.SetPosition(0, 0, 0, 5); // 设置图表位置 (行，列)
                chart.SetSize(800, 600); // 设置图表大小 (宽度，高度)
                chart.Title.Text = "Performance Curve";

                // 添加第一条曲线 (Head vs Flow)
                var series1 = chart.Series.Add(worksheet.Cells["B2:B9"], worksheet.Cells["A2:A9"]);
                series1.Header = "Head (m)";
                chart.YAxis.MaxValue = 40;
                chart.YAxis.MinValue = 0;
                chart.YAxis.Title.Text = "Head (m)";

                // 创建第二个图表作为次轴图表 (Efficiency vs Flow)
                var chart2 = worksheet.Drawings.AddChart("SecondaryChart", eChartType.XYScatterSmooth);
                chart2.Series.Add(worksheet.Cells["C2:C9"], worksheet.Cells["A2:A9"]).Header = "Efficiency (%)";
                chart2.Series.Add(worksheet.Cells["D2:D9"], worksheet.Cells["A2:A9"]).Header = "Power (kW)";

                chart2.SetPosition(0, 0, 0, 5); // 覆盖在第一个图表之上
                chart2.SetSize(800, 600);

                chart2.YAxis.MaxValue = 70;
                chart2.YAxis.MinValue = 0;
                chart2.YAxis.Title.Text = "Efficiency (%) and Power (kW)";
                chart2.YAxis.CrossesAt = 1; // 在次要Y轴上交叉

                chart2.XAxis.Deleted = true; // 隐藏次要图表的X轴
                chart2.Legend.Position = eLegendPosition.Bottom; // 合并图例

                // 保存并关闭文件
                package.Save();
            }
        }


        //private void createxcel1()
        //{
        //    double[] x = { 23.18, 32.22, 39.74, 44.08, 51.98, 62.42, 66.29, 69.15, 71.49 };
        //    double[] y1 = { 34.95, 35.23, 33.44, 32.33, 30.09, 25.08, 23.22, 21.59, 20.19 };
        //    double[] y2 = { 48.59, 57.89, 60.98, 61.96, 62.53, 57.79, 54.37, 53.11, 50.77 };

        //    using (var package = new ExcelPackage())
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add("Chart");

        //        // Insert X, Y1, and Y2 data into the worksheet
        //        for (int i = 0; i < x.Length; i++)
        //        {
        //            worksheet.Cells[i + 1, 1].Value = x[i];
        //            worksheet.Cells[i + 1, 2].Value = y1[i];
        //            worksheet.Cells[i + 1, 3].Value = y2[i];
        //        }

        //        // Create the chart
        //        var chart = worksheet.Drawings.AddScatterChart("SecondaryChart", eScatterChartType.XYScatter);
        //        chart.Title.Text = "Scatter Plot with Trendlines";
        //        chart.SetPosition(5, 0, 0, 0);  // Position of the chart
        //        chart.SetSize(600, 400);        // Size of the chart
                
        //         // Add first series (Y1) to primary axis
        //          var series1 = chart.Series.Add(worksheet.Cells["B1:B9"], worksheet.Cells["A1:A9"]);
        //        series1.Header = "Series 1";
        //        series1.Marker.Style = eMarkerStyle.Dot;
                
        //        //chart.UseSecondaryAxis = true;

        //        // Add second series (Y2) to secondary axis
        //        var series2 = chart.Series.Add(worksheet.Cells["C1:C9"], worksheet.Cells["A1:A9"]);
        //        series2.Header = "Series 2";
        //        series2.Marker.Style = eMarkerStyle.Circle;
        //        //series2.us

        //        // Add trendlines to series1 (both polynomial and linear)
        //        var trendline1Poly = series1.TrendLines.Add(eTrendLine.Polynomial);
        //        trendline1Poly.DisplayEquation = false;
        //        trendline1Poly.DisplayRSquaredValue = false;

        //        trendline1Poly.Order =2;  // Polynomial order

        //        //var trendline1Linear = series1.TrendLines.Add(eTrendLine.Linear);

        //        // Add trendlines to series2 (both polynomial and linear)
        //        var trendline2Poly = series2.TrendLines.Add(eTrendLine.Polynomial);
        //        trendline2Poly.DisplayEquation = false;
        //        trendline2Poly.DisplayRSquaredValue = false;
        //        trendline2Poly.Order =2;  // Polynomial order

        //        //var trendline2Linear = series2.TrendLines.Add(eTrendLine.Linear);
                
               
        //        // Save the workbook to a file
        //        var fileInfo = new FileInfo("ChartWithTrendlines.xlsx");
        //        package.SaveAs(fileInfo);
        //    }

           
        //}
        //private void CreatChart()
        //{
        //    Color[] lineColors = { Color.Green, Color.Red, Color.Blue };
        //    Color[] makerColors = { Color.Black, Color.Black, Color.Black };
        //    double[] x = { 23.18, 32.22, 39.74, 44.08, 51.98, 62.42, 66.29, 69.15, 71.49 };
        //    double[] y1 = { 34.95, 35.23, 33.44, 32.33, 30.09, 25.08, 23.22, 21.59, 20.19 };
        //    double[] y2 = { 48.59, 57.89, 60.98, 61.96, 62.53, 57.79, 54.37, 53.11, 50.77 };
        //    double[] y3 = { 1.59, 2.89, 4.98, 9.96, 20.53, 26.79, 30.37, 32.11, 31.77 };
        //    using (var package = new ExcelPackage())
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add("Chart");

        //        // Insert X, Y1, and Y2 data into the worksheet
        //        for (int i = 0; i < x.Length; i++)
        //        {
        //            worksheet.Cells[i + 1, 1].Value = x[i];
        //            worksheet.Cells[i + 1, 2].Value = y1[i];
        //            worksheet.Cells[i + 1, 3].Value = y2[i];
        //            worksheet.Cells[i + 1, 4].Value = y3[i];
        //        }
              
        //        int chartW =10;
        //        int chartH =20;
        //        int chartRow = 0; int chartRowOffset = 0;
        //        int chartCol = 4; int chartColOffset = 0;
        //        // Create the chart
        //        var chart = worksheet.Drawings.AddScatterChart("SecondaryChart", eScatterChartType.XYScatter);
        //        chart.Title.Text = "Scatter Plot with Trendlines";
        //        chart.SetPosition(chartRow, chartRowOffset, chartCol, chartColOffset);  // Position of the chart
        //        chart.SetSize(chartW*60, chartH*20);        // Size of the chart
        //        chart.Fill.Style = OfficeOpenXml.Drawing.eFillStyle.NoFill;
        //        chart.Border.Fill.Style = OfficeOpenXml.Drawing.eFillStyle.NoFill;
        //        chart.Legend.Position =0;
        //        chart.Legend.Border.Alignment= 0;

        //        // Create the chart
        //        var chart2 = worksheet.Drawings.AddScatterChart("SecondaryChart1", eScatterChartType.XYScatter);
        //        chart2.Title.Text = "Scatter Plot with Trendlines";
        //        chart2.SetPosition(chartRow, chartRowOffset, chartCol, chartColOffset);  // Position of the chart
        //        chart2.SetSize(chartW * 60, chartH/2 * 20);        // Size of the chart
        //        chart2.Fill.Style = OfficeOpenXml.Drawing.eFillStyle.NoFill;
        //        chart2.Border.Fill.Style = OfficeOpenXml.Drawing.eFillStyle.NoFill;
        //        chart2.Legend.Position = 0;
        //        chart2.Legend.Border.Alignment = 0;

        //        var chartType1 = chart.PlotArea.ChartTypes.Add(eChartType.XYScatter);// 定义一个类型chart
        //        chartType1.UseSecondaryAxis = false;//第二坐标               
        //         var series1 = (chartType1.Series.Add(worksheet.Cells["B1:B9"], worksheet.Cells["A1:A9"]) as ExcelChartSerie);//定义序列
        //        series1.Header = "H-Q";              
        //        ((ExcelScatterChartSerie)series1).Marker.Style= eMarkerStyle.Square;
        //        ((ExcelScatterChartSerie)series1).Marker.Border.Fill.Color = makerColors[1];

        //        var chartType2 = chart.PlotArea.ChartTypes.Add(eChartType.XYScatter);
        //        chartType2.UseSecondaryAxis = true;//第二坐标                
        //        var series2 = (chartType2.Series.Add(worksheet.Cells["C1:C9"], worksheet.Cells["A1:A9"]) as ExcelChartSerie);
        //        series2.Header = "E-Q";
        //        ((ExcelScatterChartSerie)series2).Marker.Style = eMarkerStyle.X;
        //        ((ExcelScatterChartSerie)series2).Marker.Border.Fill.Color = makerColors[1];

        //        var chartType3 = chart2.PlotArea.ChartTypes.Add(eChartType.XYScatter);
        //        chartType3.UseSecondaryAxis = false;//第三坐标                
        //        var series3 = (chartType3.Series.Add(worksheet.Cells["D1:D9"], worksheet.Cells["A1:A9"]) as ExcelChartSerie);
        //        series3.Header = "E-Q";
        //        ((ExcelScatterChartSerie)series3).Marker.Style = eMarkerStyle.Circle;
        //        ((ExcelScatterChartSerie)series3).Marker.Border.Fill.Color = makerColors[1];

        //        var chartType4 = chart2.PlotArea.ChartTypes.Add(eChartType.XYScatter);
        //        chartType4.UseSecondaryAxis = true;//第四坐标
             
        //       //y轴标题
        //       //chart.YAxis.Title.Text = "H-Q(m)";               
        //       //chart.YAxis.Title.TextVertical = eTextVerticalType.Vertical270;
        //       //chart.YAxis.Title.Font.Fill.Color=Color.Transparent;
        //       ////chart.YAxis.Title.Anchor = eTextAnchoringType.Top;
        //       var secondaryYAxis = chartType2.YAxis;
        //        //secondaryYAxis.Title.Text = "E-Q(m)";
        //        //secondaryYAxis.Title.TextVertical = eTextVerticalType.Vertical270;
        //        //secondaryYAxis.Title.Font.Fill.Color = Color.Transparent;
        //        ////x轴标题
        //        //var xAxis = chart.Axis[0];
        //        //xAxis.Title.Text = "Flow(m³/min)";
        //        //xAxis.Title.TextVertical = eTextVerticalType.Horizontal;
        //        //xAxis.Title.Font.Fill.Color = Color.Black;

        //        //删除图例
        //        chart.Legend.Remove();
        //        chart2.Legend.Remove();

        //        //添加文本 "H-Q(m)"
        //        var textbox = worksheet.Drawings.AddShape("1", eShapeStyle.Rect);
        //        textbox.SetPosition(chartRow, chartRowOffset+15, chartCol, chartColOffset-20);
        //        textbox.SetSize(30,80);
        //        textbox.Font.Size = 12;
        //        textbox.Font.Fill.Color = lineColors[0];
        //        textbox.TextVertical= eTextVerticalType.Vertical270;    
        //        textbox.Fill.Style = eFillStyle.NoFill;            
        //        textbox.Border.Fill.Style= eFillStyle.NoFill;   
        //        textbox.Text = "H-Q(m)";

        //        //添加文本 "E-Q(m)"
        //        var textbox1 = worksheet.Drawings.AddShape("2", eShapeStyle.Rect);
        //        textbox1.SetPosition(chartRow+2, chartRowOffset-20, chartCol+chartW-1, chartColOffset+17);
        //        textbox1.SetSize(30, 80);
        //        textbox1.Font.Size = 12;
        //        textbox1.Font.Fill.Color = lineColors[1];
        //        textbox1.TextVertical = eTextVerticalType.Vertical270;
        //        textbox1.Fill.Style = eFillStyle.NoFill;
        //        textbox1.Border.Fill.Style = eFillStyle.NoFill;
        //        textbox1.Text = "E-Q(kW)";

        //        //添加文本 "Flow(m³/min)"
        //        var textbox2 = worksheet.Drawings.AddShape("3", eShapeStyle.Rect);
        //        textbox2.SetPosition(chartRow+chartH, chartRowOffset-15, chartCol+chartW-3, chartColOffset+20);
        //        textbox2.SetSize(120,30);
        //        textbox2.Font.Size = 12;
        //        textbox2.Font.Fill.Color = System.Drawing.Color.Black;
        //        textbox2.TextVertical = eTextVerticalType.Horizontal;
        //        textbox2.Fill.Style = eFillStyle.NoFill;
        //        textbox2.Border.Fill.Style = eFillStyle.NoFill;
        //        textbox2.Text = "Flow(m³/min)";


        //        chart.UseSecondaryAxis = true;
        //        // Add trendlines to series1 (both polynomial and linear)
        //        var trendline1Poly = series1.TrendLines.Add(eTrendLine.Polynomial);
        //        trendline1Poly.DisplayEquation = false;
        //        trendline1Poly.DisplayRSquaredValue = false;
        //        trendline1Poly.Order = 2;  // Polynomial order
        //        trendline1Poly.Border.Width =1.5;              
        //        trendline1Poly.Border.Fill.Color = lineColors[0]; ;

        //        // Add trendlines to series2 (both polynomial and linear)
        //        var trendline2Poly = series2.TrendLines.Add(eTrendLine.Polynomial);
        //        trendline2Poly.DisplayEquation = false;
        //        trendline2Poly.DisplayRSquaredValue = false;
        //        trendline2Poly.Order = 2;  // Polynomial order
        //        trendline2Poly.Border.Width = 1.5;
        //        trendline2Poly.Border.Fill.Color = lineColors[1]; ;

        //        int f =Convert.ToInt16(chart.Size);
        //        // Save the workbook to a file
        //        var fileInfo = new FileInfo("ChartWithTrendlines.xlsx");
        //         package.SaveAs(fileInfo);
        //    }
        //}

    }
    public class ExcelImporter
    {
        private readonly ProgressForm _progressForm;

        public ExcelImporter(ProgressForm progressForm)
        {
            _progressForm = progressForm;
        }

        Stopwatch stopwatch = new Stopwatch();
        public void ImportData()
        {
            stopwatch.Start();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                // 假设有3组数据，每组数据导入一次
                int totalDataSets = 3;
                int currentProgress = 0;

                for (int i = 0; i < totalDataSets; i++)
                {
                    // 模拟数据导入过程
                    Thread.Sleep(2000); // 假设每组数据需要2秒导入

                    // 更新进度条
                    currentProgress += 100 / totalDataSets;
                    _progressForm.UpdateProgress(currentProgress, stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff"));
                }
                _progressForm.UpdateProgress(100, stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff"));
                workbook.SaveAs("data.xlsx");
            }
            stopwatch.Stop();
        }
    }
}
