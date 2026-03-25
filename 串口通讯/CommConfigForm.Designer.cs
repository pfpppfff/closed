namespace 串口通讯
{
    partial class CommConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblStatus1 = new System.Windows.Forms.Label();
            this.btnStop1 = new System.Windows.Forms.Button();
            this.btnStart1 = new System.Windows.Forms.Button();
            this.cmbPort1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblStatus2 = new System.Windows.Forms.Label();
            this.btnStop2 = new System.Windows.Forms.Button();
            this.btnStart2 = new System.Windows.Forms.Button();
            this.cmbPort2 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblStatus3 = new System.Windows.Forms.Label();
            this.btnStop3 = new System.Windows.Forms.Button();
            this.btnStart3 = new System.Windows.Forms.Button();
            this.cmbPort3 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblStatus4 = new System.Windows.Forms.Label();
            this.btnStop4 = new System.Windows.Forms.Button();
            this.btnStart4 = new System.Windows.Forms.Button();
            this.txtPort4 = new System.Windows.Forms.TextBox();
            this.txtIp4 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lblStatus5 = new System.Windows.Forms.Label();
            this.btnStop5 = new System.Windows.Forms.Button();
            this.btnStart5 = new System.Windows.Forms.Button();
            this.txtPort5 = new System.Windows.Forms.TextBox();
            this.txtIp5 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lblStatus6 = new System.Windows.Forms.Label();
            this.btnStop6 = new System.Windows.Forms.Button();
            this.btnStart6 = new System.Windows.Forms.Button();
            this.txtPort6 = new System.Windows.Forms.TextBox();
            this.txtIp6 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblStatus1);
            this.groupBox1.Controls.Add(this.btnStop1);
            this.groupBox1.Controls.Add(this.btnStart1);
            this.groupBox1.Controls.Add(this.cmbPort1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(360, 92);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "仪表1 - 串口自由口通讯";
            // 
            // lblStatus1
            // 
            this.lblStatus1.AutoSize = true;
            this.lblStatus1.ForeColor = System.Drawing.Color.Red;
            this.lblStatus1.Location = new System.Drawing.Point(200, 26);
            this.lblStatus1.Name = "lblStatus1";
            this.lblStatus1.Size = new System.Drawing.Size(41, 12);
            this.lblStatus1.TabIndex = 4;
            this.lblStatus1.Text = "已停止";
            // 
            // btnStop1
            // 
            this.btnStop1.Enabled = false;
            this.btnStop1.Location = new System.Drawing.Point(179, 54);
            this.btnStop1.Name = "btnStop1";
            this.btnStop1.Size = new System.Drawing.Size(75, 28);
            this.btnStop1.TabIndex = 3;
            this.btnStop1.Text = "停止通讯";
            this.btnStop1.UseVisualStyleBackColor = true;
            this.btnStop1.Click += new System.EventHandler(this.btnStop1_Click);
            // 
            // btnStart1
            // 
            this.btnStart1.Location = new System.Drawing.Point(98, 54);
            this.btnStart1.Name = "btnStart1";
            this.btnStart1.Size = new System.Drawing.Size(75, 28);
            this.btnStart1.TabIndex = 2;
            this.btnStart1.Text = "开始通讯";
            this.btnStart1.UseVisualStyleBackColor = true;
            this.btnStart1.Click += new System.EventHandler(this.btnStart1_Click);
            // 
            // cmbPort1
            // 
            this.cmbPort1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPort1.FormattingEnabled = true;
            this.cmbPort1.Location = new System.Drawing.Point(73, 23);
            this.cmbPort1.Name = "cmbPort1";
            this.cmbPort1.Size = new System.Drawing.Size(100, 20);
            this.cmbPort1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "串口：";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblStatus2);
            this.groupBox2.Controls.Add(this.btnStop2);
            this.groupBox2.Controls.Add(this.btnStart2);
            this.groupBox2.Controls.Add(this.cmbPort2);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(12, 109);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(360, 92);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "仪表2 - Modbus RTU通讯";
            // 
            // lblStatus2
            // 
            this.lblStatus2.AutoSize = true;
            this.lblStatus2.ForeColor = System.Drawing.Color.Red;
            this.lblStatus2.Location = new System.Drawing.Point(200, 26);
            this.lblStatus2.Name = "lblStatus2";
            this.lblStatus2.Size = new System.Drawing.Size(41, 12);
            this.lblStatus2.TabIndex = 4;
            this.lblStatus2.Text = "已停止";
            // 
            // btnStop2
            // 
            this.btnStop2.Enabled = false;
            this.btnStop2.Location = new System.Drawing.Point(179, 54);
            this.btnStop2.Name = "btnStop2";
            this.btnStop2.Size = new System.Drawing.Size(75, 28);
            this.btnStop2.TabIndex = 3;
            this.btnStop2.Text = "停止通讯";
            this.btnStop2.UseVisualStyleBackColor = true;
            this.btnStop2.Click += new System.EventHandler(this.btnStop2_Click);
            // 
            // btnStart2
            // 
            this.btnStart2.Location = new System.Drawing.Point(98, 54);
            this.btnStart2.Name = "btnStart2";
            this.btnStart2.Size = new System.Drawing.Size(75, 28);
            this.btnStart2.TabIndex = 2;
            this.btnStart2.Text = "开始通讯";
            this.btnStart2.UseVisualStyleBackColor = true;
            this.btnStart2.Click += new System.EventHandler(this.btnStart2_Click);
            // 
            // cmbPort2
            // 
            this.cmbPort2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPort2.FormattingEnabled = true;
            this.cmbPort2.Location = new System.Drawing.Point(73, 23);
            this.cmbPort2.Name = "cmbPort2";
            this.cmbPort2.Size = new System.Drawing.Size(100, 20);
            this.cmbPort2.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "串口：";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblStatus3);
            this.groupBox3.Controls.Add(this.btnStop3);
            this.groupBox3.Controls.Add(this.btnStart3);
            this.groupBox3.Controls.Add(this.cmbPort3);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(12, 207);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(360, 92);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "仪表3 - 串口自由口通讯";
            // 
            // lblStatus3
            // 
            this.lblStatus3.AutoSize = true;
            this.lblStatus3.ForeColor = System.Drawing.Color.Red;
            this.lblStatus3.Location = new System.Drawing.Point(200, 26);
            this.lblStatus3.Name = "lblStatus3";
            this.lblStatus3.Size = new System.Drawing.Size(41, 12);
            this.lblStatus3.TabIndex = 4;
            this.lblStatus3.Text = "已停止";
            // 
            // btnStop3
            // 
            this.btnStop3.Enabled = false;
            this.btnStop3.Location = new System.Drawing.Point(179, 54);
            this.btnStop3.Name = "btnStop3";
            this.btnStop3.Size = new System.Drawing.Size(75, 28);
            this.btnStop3.TabIndex = 3;
            this.btnStop3.Text = "停止通讯";
            this.btnStop3.UseVisualStyleBackColor = true;
            this.btnStop3.Click += new System.EventHandler(this.btnStop3_Click);
            // 
            // btnStart3
            // 
            this.btnStart3.Location = new System.Drawing.Point(98, 54);
            this.btnStart3.Name = "btnStart3";
            this.btnStart3.Size = new System.Drawing.Size(75, 28);
            this.btnStart3.TabIndex = 2;
            this.btnStart3.Text = "开始通讯";
            this.btnStart3.UseVisualStyleBackColor = true;
            this.btnStart3.Click += new System.EventHandler(this.btnStart3_Click);
            // 
            // cmbPort3
            // 
            this.cmbPort3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPort3.FormattingEnabled = true;
            this.cmbPort3.Location = new System.Drawing.Point(73, 23);
            this.cmbPort3.Name = "cmbPort3";
            this.cmbPort3.Size = new System.Drawing.Size(100, 20);
            this.cmbPort3.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "串口：";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblStatus4);
            this.groupBox4.Controls.Add(this.btnStop4);
            this.groupBox4.Controls.Add(this.btnStart4);
            this.groupBox4.Controls.Add(this.txtPort4);
            this.groupBox4.Controls.Add(this.txtIp4);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Location = new System.Drawing.Point(12, 305);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(436, 92);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "仪表4 - TCP自由口通讯";
            // 
            // lblStatus4
            // 
            this.lblStatus4.AutoSize = true;
            this.lblStatus4.ForeColor = System.Drawing.Color.Red;
            this.lblStatus4.Location = new System.Drawing.Point(260, 26);
            this.lblStatus4.Name = "lblStatus4";
            this.lblStatus4.Size = new System.Drawing.Size(41, 12);
            this.lblStatus4.TabIndex = 6;
            this.lblStatus4.Text = "已停止";
            // 
            // btnStop4
            // 
            this.btnStop4.Enabled = false;
            this.btnStop4.Location = new System.Drawing.Point(179, 54);
            this.btnStop4.Name = "btnStop4";
            this.btnStop4.Size = new System.Drawing.Size(75, 28);
            this.btnStop4.TabIndex = 5;
            this.btnStop4.Text = "停止通讯";
            this.btnStop4.UseVisualStyleBackColor = true;
            this.btnStop4.Click += new System.EventHandler(this.btnStop4_Click);
            // 
            // btnStart4
            // 
            this.btnStart4.Location = new System.Drawing.Point(98, 54);
            this.btnStart4.Name = "btnStart4";
            this.btnStart4.Size = new System.Drawing.Size(75, 28);
            this.btnStart4.TabIndex = 4;
            this.btnStart4.Text = "开始通讯";
            this.btnStart4.UseVisualStyleBackColor = true;
            this.btnStart4.Click += new System.EventHandler(this.btnStart4_Click);
            // 
            // txtPort4
            // 
            this.txtPort4.Location = new System.Drawing.Point(200, 23);
            this.txtPort4.Name = "txtPort4";
            this.txtPort4.Size = new System.Drawing.Size(50, 21);
            this.txtPort4.TabIndex = 3;
            this.txtPort4.Text = "502";
            // 
            // txtIp4
            // 
            this.txtIp4.Location = new System.Drawing.Point(50, 23);
            this.txtIp4.Name = "txtIp4";
            this.txtIp4.Size = new System.Drawing.Size(100, 21);
            this.txtIp4.TabIndex = 2;
            this.txtIp4.Text = "192.168.1.1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(156, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "端口：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "IP：";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lblStatus5);
            this.groupBox5.Controls.Add(this.btnStop5);
            this.groupBox5.Controls.Add(this.btnStart5);
            this.groupBox5.Controls.Add(this.txtPort5);
            this.groupBox5.Controls.Add(this.txtIp5);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Location = new System.Drawing.Point(12, 402);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(436, 92);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "仪表5 - Modbus TCP读写寄存器";
            // 
            // lblStatus5
            // 
            this.lblStatus5.AutoSize = true;
            this.lblStatus5.ForeColor = System.Drawing.Color.Red;
            this.lblStatus5.Location = new System.Drawing.Point(260, 26);
            this.lblStatus5.Name = "lblStatus5";
            this.lblStatus5.Size = new System.Drawing.Size(41, 12);
            this.lblStatus5.TabIndex = 6;
            this.lblStatus5.Text = "已停止";
            // 
            // btnStop5
            // 
            this.btnStop5.Enabled = false;
            this.btnStop5.Location = new System.Drawing.Point(179, 54);
            this.btnStop5.Name = "btnStop5";
            this.btnStop5.Size = new System.Drawing.Size(75, 28);
            this.btnStop5.TabIndex = 5;
            this.btnStop5.Text = "停止通讯";
            this.btnStop5.UseVisualStyleBackColor = true;
            this.btnStop5.Click += new System.EventHandler(this.btnStop5_Click);
            // 
            // btnStart5
            // 
            this.btnStart5.Location = new System.Drawing.Point(98, 54);
            this.btnStart5.Name = "btnStart5";
            this.btnStart5.Size = new System.Drawing.Size(75, 28);
            this.btnStart5.TabIndex = 4;
            this.btnStart5.Text = "开始通讯";
            this.btnStart5.UseVisualStyleBackColor = true;
            this.btnStart5.Click += new System.EventHandler(this.btnStart5_Click);
            // 
            // txtPort5
            // 
            this.txtPort5.Location = new System.Drawing.Point(200, 23);
            this.txtPort5.Name = "txtPort5";
            this.txtPort5.Size = new System.Drawing.Size(50, 21);
            this.txtPort5.TabIndex = 3;
            this.txtPort5.Text = "502";
            // 
            // txtIp5
            // 
            this.txtIp5.Location = new System.Drawing.Point(50, 23);
            this.txtIp5.Name = "txtIp5";
            this.txtIp5.Size = new System.Drawing.Size(100, 21);
            this.txtIp5.TabIndex = 2;
            this.txtIp5.Text = "192.168.1.1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(156, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "端口：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 26);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "IP：";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.lblStatus6);
            this.groupBox6.Controls.Add(this.btnStop6);
            this.groupBox6.Controls.Add(this.btnStart6);
            this.groupBox6.Controls.Add(this.txtPort6);
            this.groupBox6.Controls.Add(this.txtIp6);
            this.groupBox6.Controls.Add(this.label8);
            this.groupBox6.Controls.Add(this.label9);
            this.groupBox6.Location = new System.Drawing.Point(12, 500);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(436, 92);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "仪表6 - TCP字符串通讯(MEAS)";
            // 
            // lblStatus6
            // 
            this.lblStatus6.AutoSize = true;
            this.lblStatus6.ForeColor = System.Drawing.Color.Red;
            this.lblStatus6.Location = new System.Drawing.Point(260, 26);
            this.lblStatus6.Name = "lblStatus6";
            this.lblStatus6.Size = new System.Drawing.Size(41, 12);
            this.lblStatus6.TabIndex = 6;
            this.lblStatus6.Text = "已停止";
            // 
            // btnStop6
            // 
            this.btnStop6.Enabled = false;
            this.btnStop6.Location = new System.Drawing.Point(179, 54);
            this.btnStop6.Name = "btnStop6";
            this.btnStop6.Size = new System.Drawing.Size(75, 28);
            this.btnStop6.TabIndex = 5;
            this.btnStop6.Text = "停止通讯";
            this.btnStop6.UseVisualStyleBackColor = true;
            this.btnStop6.Click += new System.EventHandler(this.btnStop6_Click);
            // 
            // btnStart6
            // 
            this.btnStart6.Location = new System.Drawing.Point(98, 54);
            this.btnStart6.Name = "btnStart6";
            this.btnStart6.Size = new System.Drawing.Size(75, 28);
            this.btnStart6.TabIndex = 4;
            this.btnStart6.Text = "开始通讯";
            this.btnStart6.UseVisualStyleBackColor = true;
            this.btnStart6.Click += new System.EventHandler(this.btnStart6_Click);
            // 
            // txtPort6
            // 
            this.txtPort6.Location = new System.Drawing.Point(200, 23);
            this.txtPort6.Name = "txtPort6";
            this.txtPort6.Size = new System.Drawing.Size(50, 21);
            this.txtPort6.TabIndex = 3;
            this.txtPort6.Text = "502";
            // 
            // txtIp6
            // 
            this.txtIp6.Location = new System.Drawing.Point(50, 23);
            this.txtIp6.Name = "txtIp6";
            this.txtIp6.Size = new System.Drawing.Size(100, 21);
            this.txtIp6.TabIndex = 2;
            this.txtIp6.Text = "192.168.1.1";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(156, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 12);
            this.label8.TabIndex = 1;
            this.label8.Text = "端口：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(20, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "IP：";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(297, 606);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // CommConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 646);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CommConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "通讯配置";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CommConfigForm_FormClosing);
            this.Load += new System.EventHandler(this.CommConfigForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblStatus1;
        private System.Windows.Forms.Button btnStop1;
        private System.Windows.Forms.Button btnStart1;
        private System.Windows.Forms.ComboBox cmbPort1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblStatus2;
        private System.Windows.Forms.Button btnStop2;
        private System.Windows.Forms.Button btnStart2;
        private System.Windows.Forms.ComboBox cmbPort2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblStatus3;
        private System.Windows.Forms.Button btnStop3;
        private System.Windows.Forms.Button btnStart3;
        private System.Windows.Forms.ComboBox cmbPort3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lblStatus4;
        private System.Windows.Forms.Button btnStop4;
        private System.Windows.Forms.Button btnStart4;
        private System.Windows.Forms.TextBox txtPort4;
        private System.Windows.Forms.TextBox txtIp4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label lblStatus5;
        private System.Windows.Forms.Button btnStop5;
        private System.Windows.Forms.Button btnStart5;
        private System.Windows.Forms.TextBox txtPort5;
        private System.Windows.Forms.TextBox txtIp5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label lblStatus6;
        private System.Windows.Forms.Button btnStop6;
        private System.Windows.Forms.Button btnStart6;
        private System.Windows.Forms.TextBox txtPort6;
        private System.Windows.Forms.TextBox txtIp6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnClose;
    }
}
