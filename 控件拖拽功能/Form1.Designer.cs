namespace 控件拖拽功能
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
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.customDoubleUpDown1 = new UserC.CustomDoubleUpDown();
            this.userControl31 = new UserC.CustomSwithLightBtn();
            this.customUIButtonLight2 = new UserC.CustomUIButtonLight();
            this.customUIButtonLight1 = new UserC.CustomUIButtonLight();
            this.customUIGataValve2 = new UserC.CustomUIGateValve();
            this.customUIGataValve1 = new UserC.CustomUIGateValve();
            this.customGateValveBtn1 = new UserC.CustomGateValveBtn();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(41, 99);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button1_MouseDown);
            this.button1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.button1_MouseMove);
            this.button1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button1_MouseUp);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(680, 99);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(680, 162);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 21);
            this.textBox2.TabIndex = 2;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(627, 215);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(120, 220);
            this.listBox1.TabIndex = 3;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(26, 336);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "打开界面2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(419, 286);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 32);
            this.button3.TabIndex = 7;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(505, 369);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 10;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click_1);
            this.button4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button4_MouseDown);
            this.button4.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button4_MouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(150, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(150, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "label2";
            // 
            // customDoubleUpDown1
            // 
            this.customDoubleUpDown1.ControlName = null;
            this.customDoubleUpDown1.FBValue = 0D;
            this.customDoubleUpDown1.Location = new System.Drawing.Point(444, 47);
            this.customDoubleUpDown1.Name = "customDoubleUpDown1";
            this.customDoubleUpDown1.OutValue = 0D;
            this.customDoubleUpDown1.ReadEnable = false;
            this.customDoubleUpDown1.Size = new System.Drawing.Size(198, 58);
            this.customDoubleUpDown1.Step = 1D;
            this.customDoubleUpDown1.TabIndex = 21;
            this.customDoubleUpDown1.WriteAdr = null;
            // 
            // userControl31
            // 
            this.userControl31.BtnName = "name";
            this.userControl31.IntWriteValue = 0;
            this.userControl31.Location = new System.Drawing.Point(366, 215);
            this.userControl31.Name = "userControl31";
            this.userControl31.ReadAddress = false;
            this.userControl31.ReadEnable = false;
            this.userControl31.Size = new System.Drawing.Size(95, 33);
            this.userControl31.TabIndex = 20;
            this.userControl31.TxtFont = new System.Drawing.Font("宋体", 9F);
            this.userControl31.TypeSel = "1";
            this.userControl31.WriteAddress = null;
            // 
            // customUIButtonLight2
            // 
            this.customUIButtonLight2.BtnName = "name";
            this.customUIButtonLight2.Location = new System.Drawing.Point(221, 153);
            this.customUIButtonLight2.Name = "customUIButtonLight2";
            this.customUIButtonLight2.ReadAddress = false;
            this.customUIButtonLight2.Size = new System.Drawing.Size(125, 41);
            this.customUIButtonLight2.TabIndex = 19;
            this.customUIButtonLight2.TxtFont = new System.Drawing.Font("宋体", 9F);
            this.customUIButtonLight2.WriteAddress = false;
            // 
            // customUIButtonLight1
            // 
            this.customUIButtonLight1.BtnName = "name";
            this.customUIButtonLight1.Location = new System.Drawing.Point(221, 77);
            this.customUIButtonLight1.Name = "customUIButtonLight1";
            this.customUIButtonLight1.ReadAddress = false;
            this.customUIButtonLight1.Size = new System.Drawing.Size(118, 43);
            this.customUIButtonLight1.TabIndex = 18;
            this.customUIButtonLight1.TxtFont = new System.Drawing.Font("宋体", 9F);
            this.customUIButtonLight1.WriteAddress = false;
            // 
            // customUIGataValve2
            // 
            this.customUIGataValve2.BtnCloseName = "nameClose1";
            this.customUIGataValve2.BtnOpenName = "nameOpen1";
            this.customUIGataValve2.Location = new System.Drawing.Point(110, 369);
            this.customUIGataValve2.Name = "customUIGataValve2";
            this.customUIGataValve2.ReadCloseAddress = false;
            this.customUIGataValve2.ReadCloseLimAddress = false;
            this.customUIGataValve2.ReadOpenAddress = false;
            this.customUIGataValve2.ReadOpenLimAddress = false;
            this.customUIGataValve2.Size = new System.Drawing.Size(109, 66);
            this.customUIGataValve2.TabIndex = 11;
            this.customUIGataValve2.TxtFont = new System.Drawing.Font("宋体", 9F);
            this.customUIGataValve2.WriteAddress = false;
            // 
            // customUIGataValve1
            // 
            this.customUIGataValve1.BtnCloseName = "nameClose";
            this.customUIGataValve1.BtnOpenName = "nameOpen";
            this.customUIGataValve1.Location = new System.Drawing.Point(89, 247);
            this.customUIGataValve1.Name = "customUIGataValve1";
            this.customUIGataValve1.ReadCloseAddress = false;
            this.customUIGataValve1.ReadCloseLimAddress = false;
            this.customUIGataValve1.ReadOpenAddress = false;
            this.customUIGataValve1.ReadOpenLimAddress = false;
            this.customUIGataValve1.Size = new System.Drawing.Size(102, 54);
            this.customUIGataValve1.TabIndex = 9;
            this.customUIGataValve1.TxtFont = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customUIGataValve1.WriteAddress = false;
            // 
            // customGateValveBtn1
            // 
            this.customGateValveBtn1.BtnCloseName = "nameClose";
            this.customGateValveBtn1.BtnOpenName = "nameOpen";
            this.customGateValveBtn1.ControlMode = UserC.CustomGateValveBtn.ControlModeOptions.SetMode;
            this.customGateValveBtn1.Location = new System.Drawing.Point(235, 257);
            this.customGateValveBtn1.Name = "customGateValveBtn1";
            this.customGateValveBtn1.ReadCloseAddress = false;
            this.customGateValveBtn1.ReadCloseLimAddress = false;
            this.customGateValveBtn1.ReadEnable = false;
            this.customGateValveBtn1.ReadOpenAddress = false;
            this.customGateValveBtn1.ReadOpenLimAddress = false;
            this.customGateValveBtn1.Size = new System.Drawing.Size(127, 70);
            this.customGateValveBtn1.TabIndex = 22;
            this.customGateValveBtn1.TxtFont = new System.Drawing.Font("宋体", 9F);
            this.customGateValveBtn1.UpdataMode = 1;
            this.customGateValveBtn1.WriteCloseAddress = null;
            this.customGateValveBtn1.WriteOpenAddress = null;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.customGateValveBtn1);
            this.Controls.Add(this.customDoubleUpDown1);
            this.Controls.Add(this.userControl31);
            this.Controls.Add(this.customUIButtonLight2);
            this.Controls.Add(this.customUIButtonLight1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.customUIGataValve2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.customUIGataValve1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private UserC.CustomUIGateValve customUIGataValve1;
        private System.Windows.Forms.Button button4;
        private UserC.CustomUIGateValve customUIGataValve2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private UserC.CustomUIButtonLight customUIButtonLight1;
        private UserC.CustomUIButtonLight customUIButtonLight2;
        private UserC.CustomSwithLightBtn userControl31;
        private UserC.CustomDoubleUpDown customDoubleUpDown1;
        private UserC.CustomGateValveBtn customGateValveBtn1;
    }
}

