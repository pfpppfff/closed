using AForge.Imaging.Filters;
using PaddleOCRSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using TesseractOCR;
using TesseractOCR.Renderers;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Application = System.Windows.Forms.Application;
using Button = System.Windows.Forms.Button;
using Image = System.Drawing.Image;


namespace 截图获取数据
{
    public partial class MainForm : Form
    {
        private ScreenshotForm captureForm;
        public MainForm()
        {
            InitializeComponent();
        }
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        [DllImport("user32", EntryPoint = "SetWindowPos")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndlnsertAfter, int X, int Y, int cx, int cy, int flags);

        OCRModelConfig config = null;
        OCRParameter oCRParameter = new OCRParameter();
        OCRResult ocrResult = new OCRResult();
        //建议程序全局初始化一次即可，不必每次识别都初始化，容易报错。
        PaddleOCREngine engine;
        private void Form1_Load(object sender, EventArgs e)
        {

            engine = new PaddleOCREngine(config, oCRParameter);
            //// zoomTrackBar
            //this.zoomTrackBar.Location = new Point(300, 12);
            //this.zoomTrackBar.Name = "zoomTrackBar";
            //this.zoomTrackBar.Size = new Size(150, 45);
            //this.zoomTrackBar.Minimum = 1;
            //this.zoomTrackBar.Maximum = 10;
            //this.zoomTrackBar.Value = 2;

            //// thresholdTrackBar
            //this.thresholdTrackBar.Location = new Point(300, 45);
            //this.thresholdTrackBar.Name = "thresholdTrackBar";
            //this.thresholdTrackBar.Size = new Size(150, 45);
            //this.thresholdTrackBar.Minimum = 0;
            //this.thresholdTrackBar.Maximum = 255;
            //this.thresholdTrackBar.Value = 128;

            //// noiseTrackBar
            //this.noiseTrackBar.Location = new Point(500, 45);
            //this.noiseTrackBar.Name = "noiseTrackBar";
            //this.noiseTrackBar.Size = new Size(150, 45);
            //this.noiseTrackBar.Minimum = 0;
            //this.noiseTrackBar.Maximum = 10;
            //this.noiseTrackBar.Value = 3;


            captureButton.Click += new EventHandler(this.captureButton_Click);

           
            saveButton.Click += new EventHandler(this.saveButton_Click);

        }

   

        private void captureButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

            // 等待主窗体最小化
            Application.DoEvents();
            System.Threading.Thread.Sleep(200);

            // 创建截图窗体
            captureForm = new ScreenshotForm();
            captureForm.CaptureCompleted += CaptureForm_CaptureCompleted;
            captureForm.ShowDialog();
        }
        private Image capturedImage;
   
        private void CaptureForm_CaptureCompleted(object sender, CaptureCompletedEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;

            if (e.CapturedImage != null)
            {
                capturedPictureBox.Image = e.CapturedImage;
                capturedImage = e.CapturedImage;


                //// 使用Tesseract进行文字识别
                //string extractedText = ExtractTextFromImage(new Bitmap(capturedPictureBox.Image));
                // paddle 进行文字识别
               
                ocrResult = engine.DetectText(new Bitmap(capturedPictureBox.Image));
                List<TextBlock> textBlocks = ocrResult.TextBlocks;
                string result="";
                for (int i = 0; i < textBlocks.Count; i++)
                {
                    result += textBlocks[i].Text + "\r\n";

                }

                //// 提取数值
                string extractedNumbers = ExtractNumbers(result);

                //// 复制到剪贴板
                Clipboard.SetText(extractedNumbers);
                textBox1.AppendText(extractedNumbers + "\r\n");
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (capturedPictureBox.Image == null)
            {
                MessageBox.Show("没有可保存的图片！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "PNG图片|*.png|JPG图片|*.jpg|BMP图片|*.bmp";
            saveDialog.Title = "保存截图";
            saveDialog.DefaultExt = "png";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                string extension = Path.GetExtension(saveDialog.FileName).ToLower();
               System.Drawing.Imaging.ImageFormat  format = System.Drawing.Imaging.ImageFormat.Png;

                if (extension == ".jpg" || extension == ".jpeg")
                {
                    format = System.Drawing.Imaging.ImageFormat.Jpeg;
                }
                else if (extension == ".bmp")
                {
                    format = System.Drawing.Imaging.ImageFormat.Bmp;
                }

                capturedPictureBox.Image.Save(saveDialog.FileName, format);
               
                MessageBox.Show("图片已保存！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
       

        private string ExtractTextFromImage(Bitmap image)
        {
            try
            {
                using (var engine = new TesseractEngine(@"E:\C_Test\vs2022test\winform\closedXML_Eecel\tessdata", "chi_sim", EngineMode.TesseractAndLstm))
                {
                  
                    using (var page = engine.Process(image))
                    {
                        return page.GetText();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"OCR识别错误: {ex.Message}", "错误");
                return string.Empty;
            }
        }

        private string ExtractNumbers(string text)
        {
            // 使用正则表达式提取数值
            var matches = Regex.Matches(text, @"[-+]?[0-9]*\.?[0-9]+");
            var numbers = matches.Cast<Match>().Select(m => m.Value).ToArray();
            return string.Join(" ", numbers);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter= "图文件(*.*)|*.jpg;*.png;*.jpeg;*.bmp";
            dialog.RestoreDirectory = true;
            dialog.Multiselect = false;
            if(dialog.ShowDialog()==DialogResult.OK)
            {
                textBox1.Invoke(new Action(() => { Ocr(dialog.FileName); }));
               
            }
        }

        public void Ocr(string filepath)
        {
            textBox1.Clear();
            using (var engine = new TesseractEngine(@"E:\C_Test\vs2022test\winform\closedXML_Eecel\tessdata", "chi_sim", EngineMode.TesseractAndLstm))
            {
                using (var page = Pix.LoadFromFile(filepath))
                {
                    using (var page1 = engine.Process(page))
                    {
                        var text =page1.GetText();
                        // 提取数值
                        string extractedNumbers = ExtractNumbers(text);

                        // 复制到剪贴板
                        Clipboard.SetText(extractedNumbers);
                        textBox1.AppendText(extractedNumbers + "\r\n");  
                    }
                }
            }
        }
      

       
       
        private void button1_Click(object sender, EventArgs e)
        {
            if (capturedImage != null)
            {
                try
                {
                    statusLabel.Text = "正在处理图像...";
                    // 加载图片
                    Bitmap originalImage1 = new Bitmap(capturedImage);
                    Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
                    //Bitmap processedImage1 = PreprocessForOCR(originalImage1);
                    // 预处理
                    Bitmap processedImage1 = ImagePreprocessor.EnhanceImage(originalImage1);
                 
                    capturedPictureBox.Image = processedImage1;

                    // 使用Tesseract进行文字识别
                    string extractedText = ExtractTextFromImage(new Bitmap(capturedPictureBox.Image));
                    // 提取数值
                    string extractedNumbers = ExtractNumbers(extractedText);

                    // 复制到剪贴板
                    Clipboard.SetText(extractedNumbers);
                    textBox1.Clear();
                    textBox1.AppendText(extractedNumbers + "\r\n");
                    statusLabel.Text = "处理完成...";
                }
                catch (Exception ex)
                {
                    statusLabel.Text = "处理图像时出错: " + ex.Message;
                }
            }
            else
            {
                statusLabel.Text = "没有可处理的图像，请先截取图像。";
            }

           
          
        }

        public  Bitmap PreprocessForOCR(Bitmap input)
        {
            // 1. 转换为灰度图 (减少色彩干扰)
            Grayscale grayscaleFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = grayscaleFilter.Apply(input);

            // 2. 对比度增强 (利用 ContrastStretch 提升文字与背景的对比)
            ContrastStretch contrastFilter = new ContrastStretch();
            Bitmap contrastImage = contrastFilter.Apply(grayImage);

            // 3. 二值化处理 (使用 OtsuThreshold 自动选取阈值，转换为黑白图像)
            OtsuThreshold otsuFilter = new OtsuThreshold();
            Bitmap binaryImage = otsuFilter.Apply(contrastImage);

            // 4. 降噪处理 (使用 Median 滤波器去除孤立噪点)
            Median medianFilter = new Median();
            Bitmap processedImage = medianFilter.Apply(binaryImage);

            return processedImage;
        }

       
    }

    public class CaptureCompletedEventArgs : EventArgs
    {
        public Image CapturedImage { get; set; }

        public CaptureCompletedEventArgs(Image capturedImage)
        {
            CapturedImage = capturedImage;
        }
    }
}


