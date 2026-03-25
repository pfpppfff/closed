using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace excel文件遍历显示
{
    public partial class Form_FileQuery : Form
    {
        private class ExcelFileInfo
        {
            public int Id { get; set; }
            public string FileName { get; set; }
            public string CreationTime { get; set; }
            public string FileSize { get; set; }
        
            public string Company { get; set; }
        }
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string folderPath = txtFolderPath.Text;

            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("请输入有效的文件夹路径！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 准备开始搜索
            btnSearch.Enabled = false;
            progressBar.Visible = true;
            statusLabel.Text = "正在搜索Excel文件...";

            // 取消之前的操作（如果有）
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }

            _cts = new CancellationTokenSource();

            try
            {
                // 清空现有数据
                dataGridView1.Rows.Clear();

                // 创建进度报告对象
                var progress = new Progress<(int current, int total, string message)>(report =>
                {
                    progressBar.Maximum = report.total;
                    progressBar.Value = report.current;
                    statusLabel.Text = report.message;
                });

                // 异步执行Excel文件搜索和读取
                var excelFiles = await FindAndReadExcelFilesAsync(folderPath, progress, _cts.Token);

                // 将结果绑定到DataGridView
                BindDataToGrid(excelFiles);

                statusLabel.Text = $"找到 {excelFiles.Count} 个Excel文件";
            }
            catch (OperationCanceledException)
            {
                statusLabel.Text = "操作已取消";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"处理文件时出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "搜索过程中出错";
            }
            finally
            {
                progressBar.Visible = false;
                btnSearch.Enabled = true;
            }
        }
        // 取消操作的标记
        private CancellationTokenSource _cts;
        public Form_FileQuery()
        {
            InitializeComponent();
        }

        private void Form_FileQuery_Load(object sender, EventArgs e)
        {
            SetupDataGridView();
        }

        private async Task<List<ExcelFileInfo>> FindAndReadExcelFilesAsync(
            string folderPath,
            IProgress<(int current, int total, string message)> progress,
            CancellationToken cancellationToken)
            {
            return await Task.Run(() =>
            {
                var result = new List<ExcelFileInfo>();
                int id = 1;

                // 获取所有Excel文件
                var excelFiles = Directory.GetFiles(folderPath, "*.xlsx", SearchOption.AllDirectories).ToList();

                int totalFiles = excelFiles.Count;
                int processedFiles = 0;

                foreach (var filePath in excelFiles)
                {
                    // 检查是否取消操作
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        var fileInfo = new FileInfo(filePath);
                        string author = "未知";
                        string company = "未知";

                        // 使用ClosedXML读取Excel属性
                        using (var workbook = new XLWorkbook(filePath))
                        {
                            author = workbook.Properties.Author ?? "未知";
                            company = workbook.Properties.Company ?? "未知";
                        }

                        // 添加到结果列表
                        result.Add(new ExcelFileInfo
                        {
                            Id = id++,
                            FileName = fileInfo.Name,
                            CreationTime = author,
                            FileSize = FormatFileSize(fileInfo.Length),
                          
                            Company = company
                        });
            }
                    catch (Exception ex)
                    {
                // 记录错误但继续处理
                System.Diagnostics.Debug.WriteLine($"读取文件 {filePath} 时出错: {ex.Message}");
            }

            // 更新进度
            processedFiles++;
                    progress.Report((processedFiles, totalFiles, $"正在处理: {processedFiles}/{totalFiles} - {Path.GetFileName(filePath)}"));
                }

                return result;
            }, cancellationToken);
        }

        // 在绑定数据前确保已设置列
        private void BindDataToGrid(List<ExcelFileInfo> files)
        {
            // 确保列已设置
            if (dataGridView1.Columns.Count == 0)
             {
                SetupDataGridView();
              }

            // 清除现有行
            dataGridView1.Rows.Clear();

            // 添加数据行
            foreach (var file in files)
            {
                dataGridView1.Rows.Add(
                    file.Id,
                 file.FileName,
                    file.CreationTime,
                    file.FileSize,                  
                    file.Company
                );
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB" };
            int counter = 0;
            decimal number = bytes;

            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }

            return $"{number:n1}{suffixes[counter]}";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                statusLabel.Text = "正在取消操作...";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
        }

        // 在初始化表单或加载事件中设置DataGridView的列
        private void SetupDataGridView()
        {
            // 清除现有列（如果有）
            dataGridView1.Columns.Clear();

            // 添加所需的列
            dataGridView1.Columns.Add("colId", "编号");
            dataGridView1.Columns.Add("colFileName", "报告编号");
            dataGridView1.Columns.Add("colCreationTime", "创建时间");
            dataGridView1.Columns.Add("colFileSize", "文件大小");
        
            dataGridView1.Columns.Add("colCompany", "客户单位");

            // 设置列的属性
            dataGridView1.Columns["colId"].Width = 60;
            dataGridView1.Columns["colFileName"].Width = 250;
            dataGridView1.Columns["colCreationTime"].Width = 150;
            dataGridView1.Columns["colFileSize"].Width = 100;
       
            dataGridView1.Columns["colCompany"].Width = 150;
        }

       
    }
}
