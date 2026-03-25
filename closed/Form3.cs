using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace closed
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        private List<FileInfo> allFiles; // 每页显示的记录数

        private int pageSize = 10; // 当前页码
        private int currentPage = 1; 
                                   // 总页数

        private int totalPages = 0;
        private void Form3_Load(object sender, EventArgs e)
        {
            LoadExcelFilesAsync(@"E:\C_Test\vs2022test\winform\closedXML_Eecel\closed\fffff");
        }
        private async void LoadExcelFilesAsync(string folderPath)
        {
            await Task.Run(() =>
            { // 获取并排序Excel文件，按创建时间
                allFiles = GetExcelFiles(folderPath).OrderBy(f => f.CreationTime).ToList();
                totalPages = (int)Math.Ceiling((double)allFiles.Count / pageSize);
            });
            LoadPage(currentPage);
        }
        private List<FileInfo> GetExcelFiles(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath, "*.xlsx", SearchOption.TopDirectoryOnly);
            return files.Select(file => new FileInfo(file)).ToList();
        }
        private void LoadPage(int pageNumber)
        {
            dataGridView1.Rows.Clear(); // 清除旧数据 // 根据页码获取当前页的数据
            var pageFiles = allFiles.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            // 填充DataGridView
            for (int i = 0; i < pageFiles.Count; i++)
            {
    
                FileInfo file = pageFiles[i];
                List<string> files = new List<string>();
                files.Clear();
                files = pageFiles.Select(f=>f.FullName).ToList();
                XLWorkbook xBook = new XLWorkbook(files[i]);


                dataGridView1.Rows.Add((i + 1) + (pageNumber - 1) * pageSize,
                    // 序号
                    //file.Name,
                 xBook.Properties.Author,
                    // 文件名
                    //file.CreationTime.ToString(),
                    xBook.Properties.Company,
                    // 创建时间
                    (file.Length / 1024) + " KB"
                    // 文件大小，单位KB
                    );
            }
            lblPageInfo.Text = $"Page {currentPage} of {totalPages}";
        }
        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            { currentPage--; LoadPage(currentPage);
            } 
        }
        private void btnNext_Click(object sender, EventArgs e)
        { 
            if (currentPage < totalPages)
            { currentPage++; LoadPage(currentPage);
            }
        }
        private void btnGoToPage_Click(object sender, EventArgs e)
        { 
            if (int.TryParse(txtPageNumber.Text, out int page) && page >= 1 && page <= totalPages) 
            { currentPage = page; LoadPage(currentPage); } 
            else 
            { MessageBox.Show("Invalid page number.");
            } 
        }
    }
}

