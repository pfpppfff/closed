namespace 密码设置保护
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

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.BtnOpenActivateInfoForm = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.FirstInstallTime = new System.Windows.Forms.DateTimePicker();
            this.BtnModiftActivateInfo = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.txtTotalRundays = new System.Windows.Forms.TextBox();
            this.LastStartTime = new System.Windows.Forms.DateTimePicker();
            this.BtnCreateActivationFile = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BtnOpenActivateInfoForm
            // 
            this.BtnOpenActivateInfoForm.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnOpenActivateInfoForm.Location = new System.Drawing.Point(68, 98);
            this.BtnOpenActivateInfoForm.Name = "BtnOpenActivateInfoForm";
            this.BtnOpenActivateInfoForm.Size = new System.Drawing.Size(134, 44);
            this.BtnOpenActivateInfoForm.TabIndex = 0;
            this.BtnOpenActivateInfoForm.Text = "打开激活状态信息";
            this.BtnOpenActivateInfoForm.UseVisualStyleBackColor = true;
            this.BtnOpenActivateInfoForm.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(242, 111);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 1;
            // 
            // FirstInstallTime
            // 
            this.FirstInstallTime.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FirstInstallTime.Location = new System.Drawing.Point(68, 197);
            this.FirstInstallTime.Name = "FirstInstallTime";
            this.FirstInstallTime.Size = new System.Drawing.Size(154, 26);
            this.FirstInstallTime.TabIndex = 2;
            // 
            // BtnModiftActivateInfo
            // 
            this.BtnModiftActivateInfo.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnModiftActivateInfo.Location = new System.Drawing.Point(367, 364);
            this.BtnModiftActivateInfo.Name = "BtnModiftActivateInfo";
            this.BtnModiftActivateInfo.Size = new System.Drawing.Size(112, 44);
            this.BtnModiftActivateInfo.TabIndex = 3;
            this.BtnModiftActivateInfo.Text = "设置信息";
            this.BtnModiftActivateInfo.UseVisualStyleBackColor = true;
            this.BtnModiftActivateInfo.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(68, 377);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "停止";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // txtTotalRundays
            // 
            this.txtTotalRundays.Location = new System.Drawing.Point(68, 294);
            this.txtTotalRundays.Name = "txtTotalRundays";
            this.txtTotalRundays.Size = new System.Drawing.Size(100, 21);
            this.txtTotalRundays.TabIndex = 5;
            this.txtTotalRundays.Text = "0";
            // 
            // LastStartTime
            // 
            this.LastStartTime.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LastStartTime.Location = new System.Drawing.Point(68, 245);
            this.LastStartTime.Name = "LastStartTime";
            this.LastStartTime.Size = new System.Drawing.Size(154, 26);
            this.LastStartTime.TabIndex = 6;
            // 
            // BtnCreateActivationFile
            // 
            this.BtnCreateActivationFile.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnCreateActivationFile.Location = new System.Drawing.Point(583, 364);
            this.BtnCreateActivationFile.Name = "BtnCreateActivationFile";
            this.BtnCreateActivationFile.Size = new System.Drawing.Size(112, 44);
            this.BtnCreateActivationFile.TabIndex = 7;
            this.BtnCreateActivationFile.Text = "创建信息文件";
            this.BtnCreateActivationFile.UseVisualStyleBackColor = true;
            this.BtnCreateActivationFile.Click += new System.EventHandler(this.button4_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(68, 339);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 21);
            this.textBox2.TabIndex = 8;
            this.textBox2.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(208, 297);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "已经使用天数";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(208, 342);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "总使用天数";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.BtnCreateActivationFile);
            this.Controls.Add(this.LastStartTime);
            this.Controls.Add(this.txtTotalRundays);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.BtnModiftActivateInfo);
            this.Controls.Add(this.FirstInstallTime);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.BtnOpenActivateInfoForm);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnOpenActivateInfoForm;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DateTimePicker FirstInstallTime;
        private System.Windows.Forms.Button BtnModiftActivateInfo;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox txtTotalRundays;
        private System.Windows.Forms.DateTimePicker LastStartTime;
        private System.Windows.Forms.Button BtnCreateActivationFile;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

