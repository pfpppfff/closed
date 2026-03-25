
using System;
using System.Drawing;
using System.Windows.Forms;

namespace S7NET
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnOpenReadWrite;
        private System.Windows.Forms.Button btnDiagnose;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.GroupBox grpPLC1;
        private System.Windows.Forms.GroupBox grpPLC2;
        private System.Windows.Forms.Label lblPLC1Status;
        private System.Windows.Forms.Label lblPLC2Status;
        private System.Windows.Forms.Label lblPLC1Data1;
        private System.Windows.Forms.Label lblPLC1Data2;
        private System.Windows.Forms.Label lblPLC2Data1;
        private System.Windows.Forms.Label lblPLC2Data2;

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // 窗体设置
            this.Text = "S7.NET 多PLC通讯演示";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // 状态标签
            lblStatus = new Label
            {
                Text = "状态: 未初始化",
                Location = new Point(12, 12),
                Size = new Size(400, 23),
                Font = new Font("Microsoft YaHei", 9F, FontStyle.Bold)
            };
            this.Controls.Add(lblStatus);

            // 连接按钮
            btnConnect = new Button
            {
                Text = "连接所有PLC",
                Location = new Point(12, 45),
                Size = new Size(120, 35),
                UseVisualStyleBackColor = true
            };
            btnConnect.Click += new EventHandler(this.btnConnect_Click);
            this.Controls.Add(btnConnect);

            // 断开按钮
            btnDisconnect = new Button
            {
                Text = "断开所有PLC",
                Location = new Point(142, 45),
                Size = new Size(120, 35),
                UseVisualStyleBackColor = true
            };
            btnDisconnect.Click += new EventHandler(this.btnDisconnect_Click);
            this.Controls.Add(btnDisconnect);

            // 打开读写界面按钮
            btnOpenReadWrite = new Button
            {
                Text = "打开读写界面",
                Location = new Point(272, 45),
                Size = new Size(120, 35),
                UseVisualStyleBackColor = true
            };
            btnOpenReadWrite.Click += new EventHandler(this.btnOpenReadWrite_Click);
            this.Controls.Add(btnOpenReadWrite);

            // 连接诊断按钮
            btnDiagnose = new Button
            {
                Text = "连接诊断",
                Location = new Point(402, 45),
                Size = new Size(120, 35),
                UseVisualStyleBackColor = true,
                BackColor = Color.LightBlue
            };
            btnDiagnose.Click += new EventHandler(this.btnDiagnose_Click);
            this.Controls.Add(btnDiagnose);

            // PLC1组框
            grpPLC1 = new GroupBox
            {
                Text = "PLC1 (192.168.2.10)",
                Location = new Point(12, 95),
                Size = new Size(380, 120),
                Font = new Font("Microsoft YaHei", 9F, FontStyle.Bold)
            };
            this.Controls.Add(grpPLC1);

            lblPLC1Status = new Label
            {
                Text = "未连接",
                Location = new Point(15, 25),
                Size = new Size(100, 20),
                ForeColor = Color.Red
            };
            grpPLC1.Controls.Add(lblPLC1Status);

            lblPLC1Data1 = new Label
            {
                Text = "DB15.DBD0: --",
                Location = new Point(15, 50),
                Size = new Size(200, 20)
            };
            grpPLC1.Controls.Add(lblPLC1Data1);

            lblPLC1Data2 = new Label
            {
                Text = "DB15.DBD4: --",
                Location = new Point(15, 75),
                Size = new Size(200, 20)
            };
            grpPLC1.Controls.Add(lblPLC1Data2);

            // PLC2组框
            grpPLC2 = new GroupBox
            {
                Text = "PLC2 (192.168.2.11)",
                Location = new Point(402, 95),
                Size = new Size(380, 120),
                Font = new Font("Microsoft YaHei", 9F, FontStyle.Bold)
            };
            this.Controls.Add(grpPLC2);

            lblPLC2Status = new Label
            {
                Text = "未连接",
                Location = new Point(15, 25),
                Size = new Size(100, 20),
                ForeColor = Color.Red
            };
            grpPLC2.Controls.Add(lblPLC2Status);

            lblPLC2Data1 = new Label
            {
                Text = "DB15.DBD0: --",
                Location = new Point(15, 50),
                Size = new Size(200, 20)
            };
            grpPLC2.Controls.Add(lblPLC2Data1);

            lblPLC2Data2 = new Label
            {
                Text = "DB15.DBD4: --",
                Location = new Point(15, 75),
                Size = new Size(200, 20)
            };
            grpPLC2.Controls.Add(lblPLC2Data2);

            // 日志列表框
            var lblLog = new Label
            {
                Text = "运行日志:",
                Location = new Point(12, 230),
                Size = new Size(100, 20),
                Font = new Font("Microsoft YaHei", 9F, FontStyle.Bold)
            };
            this.Controls.Add(lblLog);

            lstLog = new ListBox
            {
                Location = new Point(12, 255),
                Size = new Size(770, 300),
                Font = new Font("Consolas", 9F)
            };
            this.Controls.Add(lstLog);

            //
            // Form1
            //
            this.AutoScaleDimensions = new SizeF(6F, 12F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 600);
            this.Name = "Form1";
            this.Load += new EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
        }

        #endregion
    }
}

