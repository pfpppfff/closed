namespace 密码保护
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
            this.listBoxActivationInfo = new System.Windows.Forms.ListBox();
            this.BtnActivate = new System.Windows.Forms.Button();
            this.txtActivationCode = new System.Windows.Forms.TextBox();
            this.lblActivationCode = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBoxActivationInfo
            // 
            this.listBoxActivationInfo.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBoxActivationInfo.FormattingEnabled = true;
            this.listBoxActivationInfo.ItemHeight = 16;
            this.listBoxActivationInfo.Location = new System.Drawing.Point(216, 151);
            this.listBoxActivationInfo.Name = "listBoxActivationInfo";
            this.listBoxActivationInfo.Size = new System.Drawing.Size(400, 244);
            this.listBoxActivationInfo.TabIndex = 0;
            // 
            // BtnActivate
            // 
            this.BtnActivate.Location = new System.Drawing.Point(541, 73);
            this.BtnActivate.Name = "BtnActivate";
            this.BtnActivate.Size = new System.Drawing.Size(75, 23);
            this.BtnActivate.TabIndex = 1;
            this.BtnActivate.Text = "button1";
            this.BtnActivate.UseVisualStyleBackColor = true;
            this.BtnActivate.Click += new System.EventHandler(this.BtnActivate_Click);
            // 
            // txtActivationCode
            // 
            this.txtActivationCode.Location = new System.Drawing.Point(318, 75);
            this.txtActivationCode.Name = "txtActivationCode";
            this.txtActivationCode.Size = new System.Drawing.Size(187, 21);
            this.txtActivationCode.TabIndex = 2;
            // 
            // lblActivationCode
            // 
            this.lblActivationCode.AutoSize = true;
            this.lblActivationCode.Location = new System.Drawing.Point(99, 75);
            this.lblActivationCode.Name = "lblActivationCode";
            this.lblActivationCode.Size = new System.Drawing.Size(41, 12);
            this.lblActivationCode.TabIndex = 3;
            this.lblActivationCode.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblActivationCode);
            this.Controls.Add(this.txtActivationCode);
            this.Controls.Add(this.BtnActivate);
            this.Controls.Add(this.listBoxActivationInfo);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxActivation;
        private System.Windows.Forms.ListBox listBoxActivationInfo;
        private System.Windows.Forms.Button BtnActivate;
        private System.Windows.Forms.TextBox txtActivationCode;
        private System.Windows.Forms.Label lblActivationCode;
    }
}

