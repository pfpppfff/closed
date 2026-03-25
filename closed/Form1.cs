using closed;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace closed
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            progressBar1.Value = 0;

            // 使用多线程处理任务
            await Task.Run(() =>
            {
                // 加载Excel模板文件
               
               
                    XLWorkbook workbook = new XLWorkbook("template.xlsx");
                    // 获取第一个Sheet
                    IXLWorksheet worksheet1 = workbook.Worksheet(1);

                    // 假设我们有大量数据要导入
                    for (int i = 1; i <= 10000; i++)
                    {
                        worksheet1.Cell(i, 1).Value = "Data " + i;

                        // 更新进度条（假设每100行更新一次）
                        if (i % 100 == 0)
                        {
                            Invoke(new Action(() => progressBar1.Value = i / 100));
                        }
                    }
               
                // 保存新的文件，包含第1和第2个Sheet
                XLWorkbook newWorkbook = new XLWorkbook();
                string sheet1Name = "Sheet1_Renamed";
                string sheet2Name = "Sheet2_Renamed";
                if (newWorkbook.Worksheets.Contains(sheet1Name))
                {
                    sheet1Name = sheet1Name + "_Copy";
                }
                if (newWorkbook.Worksheets.Contains(sheet2Name))
                {
                    sheet2Name = sheet2Name + "_Copy";
                }
               
               
                newWorkbook.AddWorksheet(workbook.Worksheet(4));
                newWorkbook.AddWorksheet(workbook.Worksheet(3));
             
                newWorkbook.Properties.Author= "hello w";
                newWorkbook.Properties.Company= "hello www";
                newWorkbook.SaveAs("newFile.xlsx");
                  
               
            });

            progressBar1.Value = progressBar1.Maximum;
            MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            progressBar1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var originalWorkbook = new XLWorkbook("template.xlsx");
            var originalWorksheet = originalWorkbook.Worksheet("Sheet3");

            // 创建新工作簿并复制工作表
            var newWorkbook = new XLWorkbook();
            originalWorksheet.CopyTo(newWorkbook, "CopiedSheet");

            // 保存新工作簿
            newWorkbook.SaveAs("newFile1.xlsx");

            Console.WriteLine("工作表已复制并另存为新文件。");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // 定义文件路径
            string sourceFilePath = "template.xlsx";
            string newFilePath = "newfile2.xlsx";

            // 使用 ClosedXML 加载 Excel 文件
            using (var workbook = new XLWorkbook(sourceFilePath))
            {
                // 获取第三个工作表
                var thirdSheet = workbook.Worksheet(3);

                // 创建一个新的工作簿
                var newWorkbook = new XLWorkbook();

                // 复制第三个工作表到新工作簿
                var newSheet = thirdSheet.CopyTo(newWorkbook, thirdSheet.Name);

                // 修改公式，使其引用原始文件
                UpdateFormulas(newSheet, sourceFilePath);

                // 保存新工作簿
                newWorkbook.SaveAs(newFilePath);
            }

            Console.WriteLine("新文件已保存到: " + newFilePath);

            MessageBox.Show("新文件已保存到: " + newFilePath);
        }
        static void UpdateFormulas(IXLWorksheet worksheet, string sourceFilePath)
        {
            foreach (var cell in worksheet.CellsUsed())
            {
                if (cell.HasFormula)
                {
                    // 获取当前公式
                    string formula = cell.FormulaA1;

                    // 假设公式引用第一页（例如：Sheet1），你可以根据实际情况调整
                    formula = formula.Replace("Sheet1!", $"'[{sourceFilePath}]Sheet1'!");

                    // 更新公式
                    cell.FormulaA1 = formula;
                }
            }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            progressBar1.Value = 0;

            // 使用多线程处理任务
            await Task.Run(() =>
            {
                // 加载Excel模板文件
                XLWorkbook workbook = new XLWorkbook("template.xlsx");
                // 获取第一个Sheet
                IXLWorksheet worksheet1 = workbook.Worksheet(1);
              
                // 假设我们有大量数据要导入
                for (int i = 1; i <= 10000; i++)
                {
                    worksheet1.Cell(i, 1).Value = "Data " + i;
                    // 更新进度条（假设每100行更新一次）
                    if (i % 100 == 0)
                    {
                        Invoke(new Action(() => progressBar1.Value = i / 100));
                    }
                }

                worksheet1.Hide();
                workbook.Properties.Author = "hello w";
                workbook.Properties.Company = "hello www";              
                workbook.SaveAs("newFile.xlsx");
            });

            progressBar1.Value = progressBar1.Maximum;
            MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            progressBar1.Visible = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }
    }

   
}


