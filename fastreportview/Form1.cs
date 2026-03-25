using FastReport;
using FastReport.Data;
using FastReport.Export.OoXML;
using FastReport.Export.Pdf;
using FastReport.RichTextParser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace fastreportview
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 创建 Chart 控件（如果已在设计器中添加可省略）
            //Chart chart1 = new Chart();
            //chart1.Dock = DockStyle.Fill;
            //this.Controls.Add(chart1);

            // 添加 ChartArea
            ChartArea chartArea = new ChartArea("MainArea");
            chart1.ChartAreas.Add(chartArea);

            // 获取 Y 轴
            Axis yAxis = chartArea.AxisY;

            // 设置 Y 轴范围为 -6 到 6
            yAxis.Minimum = -6;
            yAxis.Maximum = 6;

            // 禁用自动缩放
            yAxis.IsStartedFromZero = false;

            // 隐藏所有默认刻度标签
            yAxis.LabelStyle.Enabled = false;

            // 添加自定义标签：-6
            CustomLabel labelMin = new CustomLabel();
            labelMin.FromPosition = -6.1;  // 包含 -6 的范围
            labelMin.ToPosition = -5.9;
            labelMin.Text = "-6";
            labelMin.RowIndex = 1;         // 避免标签重叠
            yAxis.CustomLabels.Add(labelMin);

            // 添加自定义标签：6
            CustomLabel labelMax = new CustomLabel();
            labelMax.FromPosition = 5.9;
            labelMax.ToPosition = 6.1;
            labelMax.Text = "6";
            labelMax.RowIndex = 1;
            yAxis.CustomLabels.Add(labelMax);

            // 【可选】隐藏 Y 轴线和刻度线
            // yAxis.LineWidth = 0;                     // 隐藏轴线
            // yAxis.MajorTickMark.Enabled = false;     // 隐藏主刻度线
            // yAxis.MinorTickMark.Enabled = false;     // 隐藏次刻度线

            // 添加一个示例数据系列
            Series series = new Series("Data");
            series.ChartType = SeriesChartType.Line;
            series.Points.AddXY(1, -3);
            series.Points.AddXY(2, 0);
            series.Points.AddXY(3, 4);
            series.Points.AddXY(4, 2);
            chart1.Series.Add(series);

            // 将 Series 绑定到 ChartArea
            series.ChartArea = "MainArea";

            // 显示图例
            chart1.Legends.Add(new Legend("Legend"));
            chart1.Series["Data"].IsVisibleInLegend = true;
            chart1.Series["Data"].Legend = "Legend";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EXCELT();

            //// 1. 创建数据源（可以是 DataTable 或 List<T>）
            //var dataTable = new DataTable("baseInfo");
            //dataTable.Columns.Add("name", typeof(string));
            //dataTable.Columns.Add("sex", typeof(string));
            //dataTable.Columns.Add("age", typeof(string)); 
            //dataTable.Columns.Add("PumpNum", typeof(string));
            //dataTable.Rows.Add("张三", "男", "30", "P12345");

            //FastReport.Report report = new FastReport.Report();
            //report.Load("demo.frx");
            //report.RegisterData(dataTable, "baseInfo");
            //report.GetDataSource("baseInfo").Enabled = true;
            //report.Prepare();

            //// 获取报表中的 TextObject（名称为 Text1）
            ////var textObject = report.FindObject("Text4") as FastReport.TextObject;

            ////if (textObject != null)
            ////{
            ////    textObject.Text = "这是通过代码写入的文本内容。";
            ////}
            //report.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // create report instance
                Report report = new Report();

                report.Load("demo.frx");
                FastReport.Export.OoXML.Excel2007Export xlsExport = new FastReport.Export.OoXML.Excel2007Export();
                xlsExport.ShowProgress = false;

                MemoryStream strm = new MemoryStream();
                report.Export(xlsExport, strm);

                strm.Position = 0;
                if (txtFilePath.Text.Trim() == "")//路径为空的时候默认放到桌面
                {
                    txtFilePath.Text = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                }
                if (txtFileName.Text.Trim() == "")
                {
                    txtFileName.Text = string.Format("检测报告{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                }
                string strPath = this.txtFilePath.Text + "\\" + this.txtFileName.Text;
                if (!strPath.EndsWith(".xlsx"))
                {
                    MessageBox.Show("请输入xls格式的文件名！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                FileStream fs = new FileStream(strPath, FileMode.Create);
                strm.WriteTo(fs);
                strm.Close();
                fs.Close();

                xlsExport.Dispose();
                strm.Dispose();
                fs.Dispose();
                report.Dispose();
                MessageBox.Show("结果保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
               
                MessageBox.Show("输入的路径无法访问，请重新输入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtFilePath_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 加载报表模板
            string reportPath = "demo.frx";
            Report report = new Report();
            report.Load(reportPath);

        
            report.Prepare();

            // 导出为Excel文件
            string exportPath = Environment.CurrentDirectory + @"\导出.xlsx";
            Excel2007Export export = new Excel2007Export();
            report.Export(export, exportPath);

            Console.WriteLine("报表已成功导出为Excel文件！");
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }


        private void EXCELT()
        {
            DataSet dataSet = null;
            Report report = null;
            DataTable dataTable = null;
            try
            {

                // 创建DataSet
                dataSet = new DataSet("baseInfo");

                // 创建DataTable
                dataTable = new DataTable("baseInfo");
                dataTable.Columns.Add("name", typeof(string));
                dataTable.Columns.Add("sex", typeof(string));
                dataTable.Columns.Add("pumpNum", typeof(string));

                // 添加数据
                dataTable.Rows.Add("张三", "男", "P12345");

                // 将DataTable添加到DataSet
                dataSet.Tables.Add(dataTable);

                // 创建报表
                report = new Report();
                report.Load("demo.frx");

                // 注册DataSet
                report.RegisterData(dataSet);
                // 准备并显示报表
                //report.Prepare();
                //report.Show();
                report.Prepare();

                // 导出为Excel文件
                string exportPath = Environment.CurrentDirectory + @"\导出.xlsx";
                Excel2007Export export = new Excel2007Export();
                report.Export(export, exportPath);

                Console.WriteLine("报表已成功导出为Excel文件！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生异常: {ex.Message}");
                Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
            }
            finally
            {
                dataSet?.Dispose(); 
                dataTable?.Dispose();
                report?.Dispose();
            }
        }
        private void PDFT()
        {
          
            Report report = null;
            DataSet dataSet = null;
            DataTable dataTable = null;
            DataSet dataSet1 = null;
            DataTable dataTable1 = null;
            //try
            //{
                // 创建DataSet
                dataSet = new DataSet("baseInfo");
                // 创建DataTable
                dataTable = new DataTable("baseInfo");
                dataTable.Columns.Add("name", typeof(string));
                dataTable.Columns.Add("sex", typeof(string));
                dataTable.Columns.Add("pumpNum", typeof(string));
                // 添加数据
                dataTable.Rows.Add("张三", "男", "P12345");
                // 将DataTable添加到DataSet
                dataSet.Tables.Add(dataTable);

                // 创建DataSet1
                dataSet1 = new DataSet("tableData");
                // 创建DataTable
                dataTable1 = new DataTable("tableData");
                //dataTable1.Columns.Add("ID", typeof(string));
                dataTable1.Columns.Add("Speed", typeof(string));
                dataTable1.Columns.Add("Flow", typeof(string));
                dataTable1.Columns.Add("Head", typeof(string));
                dataTable1.Columns.Add("Torque", typeof(string));
                dataTable1.Columns.Add("ShaftPower", typeof(string));

                // 添加数据
                dataTable1.Rows.Add( "2900", "12", "33", "34", "35");
                dataTable1.Rows.Add( "2901", "13", "34", "365", "37");
                dataTable1.Rows.Add( "2901", "13", "34", "365", "37");
                // 将DataTable添加到DataSet
                dataSet1.Tables.Add(dataTable1);
                // 创建报表
                report = new Report();
                report.Load("demo.frx");

           
               // 注册DataSet
               report.RegisterData(dataSet);
                report.RegisterData(dataSet1);

            //DataBand masterBand = report.FindObject("Table4") as DataBand;
            //masterBand.DataSource = report.GetDataSource("ataSet1");
            // 准备并显示报表
            report.Prepare();
           
                string exportPath = Environment.CurrentDirectory + @"\导出.pdf";
                PDFExport export = new PDFExport();
                export.ShowProgress = false;                           
                export.Outline = false;
                export.Background = false;
                export.PrintOptimized = true; // 优化打印
                export.JpegQuality = 95; // 高质量
                report.Export(export, exportPath);
                Console.WriteLine("报表已成功导出为pdf文件！");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"发生异常: {ex.Message}");
            //    Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
            //}
            //finally
            //{
            //    dataSet?.Dispose();
            //    dataTable?.Dispose();
            //    report?.Dispose();
            //}
        }
        // 启用所有数据源
        private void EnableDataSources(Report report)
        {
            try
            {
                foreach (FastReport.Data.DataSourceBase dataSource in report.Dictionary.DataSources)
                {
                    dataSource.Enabled = true;
                    Console.WriteLine($"数据源 '{dataSource.Name}' 已启用");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"启用数据源失败: {ex.Message}");
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            PDFT();
        }

        private void gsfh()
        {
            Report report = null;
            DataSet dataSet = null;
            DataTable dataTable = null;
            DataSet dataSet1 = null;
            DataTable dataTable1 = null;
            DataSet chartDataSet = null;
            DataTable chartTable = null;
            //try
            //{
            // 创建DataSet
            dataSet = new DataSet("baseInfo");
            // 创建DataTable
            dataTable = new DataTable("baseInfo");
            dataTable.Columns.Add("name", typeof(string));
            dataTable.Columns.Add("sex", typeof(string));
            dataTable.Columns.Add("pumpNum", typeof(string));
            // 添加数据
            dataTable.Rows.Add("张三", "男", "P12345");
            // 将DataTable添加到DataSet
            dataSet.Tables.Add(dataTable);

            // 创建DataSet1
            dataSet1 = new DataSet("tableData");           
            dataTable1 = new DataTable("tableData");
            dataTable1.Columns.Add("ID", typeof(string));
            dataTable1.Columns.Add("Speed", typeof(string));
            dataTable1.Columns.Add("Flow", typeof(string));
            dataTable1.Columns.Add("Head", typeof(string));
            dataTable1.Columns.Add("Torque", typeof(string));
            dataTable1.Columns.Add("ShaftPower", typeof(string));
            dataTable1.Rows.Add("1", "2900", "12", "33", "34", "35");
            dataTable1.Rows.Add("2", "2901", "13", "34", "365", "37");
            dataTable1.Rows.Add("3", "2901", "13", "34", "365", "37");
          
            dataSet1.Tables.Add(dataTable1);

            //***chartdata
             chartDataSet = new DataSet("chartData");
            chartTable = new DataTable("chartData");
            chartTable.Columns.Add("ID", typeof(Int32));
            chartTable.Columns.Add("SpeedErr", typeof(Single));
            chartTable.Columns.Add("FlowErr", typeof(Single));
            chartTable.Columns.Add("HeadErr", typeof(Single));
            chartTable.Columns.Add("TorqueErr", typeof(Single));
            chartTable.Columns.Add("ShaftPowerErr", typeof(Single));

            chartTable.Rows.Add(1, 5.0f, 4f, -4.5f, 5.2f, -12.9f);
            chartTable.Rows.Add(2, 5.3f, -3.5f, -4.1f, 2f, 4f);
            chartTable.Rows.Add(3, -5.1f, 2f, 6f, -3f, 10f);
            chartDataSet.Tables.Add(chartTable);
            // 创建报表
            report = new Report();
            report.Load("demo.frx");


            // 注册DataSet
            report.RegisterData(dataSet);
            report.RegisterData(dataSet1);
            report.RegisterData(chartDataSet);
            //report.GetDataSource("tableData").Enabled = true;
            //EnableDataSources(report);
            //typeof(System.Single)
            // 准备并显示报表
            report.Prepare();
            report.Show();  
            //string exportPath = Environment.CurrentDirectory + @"\导出.pdf";
            //PDFExport export = new PDFExport();
            //export.ShowProgress = false;
            //export.Outline = false;
            //export.Background = false;
            //export.PrintOptimized = true; // 优化打印
            //export.JpegQuality = 95; // 高质量
            //report.Export(export, exportPath);
            //Console.WriteLine("报表已成功导出为pdf文件！");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"发生异常: {ex.Message}");
            //    Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
            //}
            //finally
            //{
            //    dataSet?.Dispose();
            //    dataTable?.Dispose();
            //    report?.Dispose();
            //}
        }

        private void button5_Click(object sender, EventArgs e)
        {
            gsfh();
        }
    }
}
