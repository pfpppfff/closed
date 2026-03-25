using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace epplus
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private static string filePathFunc = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\muban.xlsx";
        private static string savefilePathFunc = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\";
        public static async void DgvEpplusFuncToExcel()
        {
            await Task.Run(() =>
            {
                //try
                //{
                  
                    string filePath = filePathFunc;
                  
                   
                   
                        ExcelPackage excelPackage = new ExcelPackage(new FileInfo(filePath));
                        var xBook = excelPackage.Workbook;
                        // 获取第一个Sheet
                        var xSheet = xBook.Worksheets["Sheet1"];
                        string SaveFileName = DateTime.Now.Date.ToString("yyyy", DateTimeFormatInfo.InvariantInfo) + DateTime.Now.Date.ToString("MM", DateTimeFormatInfo.InvariantInfo);
                        SaveFileName += DateTime.Now.Date.ToString("dd", DateTimeFormatInfo.InvariantInfo) + DateTime.Now.ToString("HH", DateTimeFormatInfo.InvariantInfo);
                        SaveFileName += DateTime.Now.ToString("mm", DateTimeFormatInfo.InvariantInfo) + DateTime.Now.ToString("ss", DateTimeFormatInfo.InvariantInfo);
                        string dateTime = DateTime.Now.Date.ToString("yyyy", DateTimeFormatInfo.InvariantInfo) + "." + DateTime.Now.Date.ToString("MM", DateTimeFormatInfo.InvariantInfo) + "." + DateTime.Now.Date.ToString("dd", DateTimeFormatInfo.InvariantInfo);
                        string createTime = DateTime.Now.Date.ToString("yyyy", DateTimeFormatInfo.InvariantInfo) + "/" + DateTime.Now.Date.ToString("MM", DateTimeFormatInfo.InvariantInfo) + "/" +
                        DateTime.Now.Date.ToString("dd", DateTimeFormatInfo.InvariantInfo) + " " + DateTime.Now.ToString("hh", DateTimeFormatInfo.InvariantInfo) + ":" +
                        DateTime.Now.ToString("mm", DateTimeFormatInfo.InvariantInfo) + ":" + DateTime.Now.ToString("ss", DateTimeFormatInfo.InvariantInfo);
                        int y_excel = 2;
                        int x_excel = 2;//excel起始点
                    for (int i = 0; i <= 5; i++)//遍历表格控件的所有行
                    {
                        for (int j = 0; j < 14; j++)//遍历表格控件的所有列
                        {
                            xSheet.Cells[y_excel, j + x_excel].Value = Convert.ToDecimal(i);
                        }
                        y_excel++;//获取excel表中有数据的最大行数

                    }
                        var workSheetNames = xBook.Worksheets.Select(x => x.Name).ToList();
                        string fs1 = xBook.Worksheets[1].Name;
                        string fs2 = xBook.Worksheets[2].Name;
                        string fs3 = xBook.Worksheets[2].Name;
                   
                        //   1"检测报告封面"                      
                        //   2"说明"
                        //   3"检测报告"
                        //   4"机组效率-换算"
                        //   5"机组效率-不换算"
                        //   6"泵效率-换算"
                        //   7"泵效率-不换算 "
                        //   8"NPSH-换算 "
                        //   9"NPSH-不换算 "
                        //   10 npsh 泵效率 换算
                        //   11 npsh 泵效率 不换算
                        //   12"声明
                        //   13"数据输入"


                      


                        string nameSel = "";
                        string isFullRep = "";
                        int selSheet = 6;
                 
                        string[] listSheetNames = { "机组效率-换算", "NPSH机组效率-换算", "机组效率-不换算", "NPSH机组效率-不换算", "泵效率-不换算", "泵效率-不换算", "NPSH泵效率-换算", "NPSH泵效率-不换算" };
                        int len = xBook.Worksheets.Count;
                        string xSheetNameSel = xBook.Worksheets[selSheet].Name;
                        // 
                    
                        //judResult.xAxisArry.CopyTo(newXAxisArry, 0);
                        //newXAxisArry[judResult.xAxisArry.Length] = flowULine;
                  
                    


                        List<string> nameMainSheet = new List<string>();
                        nameMainSheet.Clear();
                        //for (int i = 4; i <= 11; i++)
                        //{

                        //    nameMainSheet.Add(xBook.Worksheets[i].Name);

                        //}
                        //for (int i = 0; i < nameMainSheet.Count; i++)
                        //{
                        //    if (nameMainSheet[i] != xSheetNameSel)
                        //    {
                        //        string na = xBook.Worksheets[nameMainSheet[i]].Name;
                        //        xBook.Worksheets.Delete(nameMainSheet[i]);
                        //    }
                        //}                   
                        
                            xBook.Worksheets.Delete("Sheet2");
                            xBook.Worksheets.Delete("Sheet3");
                            xBook.Worksheets.Delete("Sheet4");
                xBook.Worksheets.Delete("Sheet5");
                xBook.Worksheets.Delete("Sheet6");
                //xSheet.Hidden = eWorkSheetHidden.Hidden;
                       
                      string mainPath = savefilePathFunc +"456" + ".xlsx";
                      excelPackage.SaveAs(new FileInfo(mainPath));
                  
                        GC.Collect();
                
                        MessageBox.Show("报告保存成功");

                        //}                   
                   

                //}
                //catch (Exception ex)
                //{ MessageBox.Show(ex.ToString()); };
            }
            );

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DgvEpplusFuncToExcel();
        }
    }
}
