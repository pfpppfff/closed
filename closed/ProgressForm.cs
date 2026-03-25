using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace closed
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
            btnConfirm.Visible = false;
            btnConfirm.Enabled = false;
            
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
           
        }
        // 更新进度条的方法
        public void UpdateProgress(int percent,string times)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<int, string>(UpdateProgress), percent, times);
            }
            else
            {

                uiProcessBar1.Value = percent;
               textBox1.Text = times;
         
                if (percent == 100)
                {
                    btnConfirm.Visible = true;
                    btnConfirm.Enabled = true;
                }


            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void progressBar1_Paint(object sender, PaintEventArgs e)
        {
            ProgressBar progressBar = sender as ProgressBar;
            float percent = ((float)progressBar.Value / (float)progressBar.Maximum) * 100;
            string text = $"{percent:0}%"; // 显示百分比文本

            using (Font font = new Font("Arial", 10))
            {
                SizeF textSize = e.Graphics.MeasureString(text, font);
                PointF location = new PointF((progressBar.Width / 2) - (textSize.Width / 2),
                                             (progressBar.Height / 2) - (textSize.Height / 2));
                e.Graphics.DrawString(text, font, Brushes.Black, location);
            }
        }
    }
}
