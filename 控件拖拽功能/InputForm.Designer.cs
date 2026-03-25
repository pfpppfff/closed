namespace 控件拖拽功能
{
    partial class InputForm
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
            this.textBoxButtonName = new System.Windows.Forms.TextBox();
            this.textBoxButtonClickContent = new System.Windows.Forms.TextBox();
            this.按钮名称 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBtnHight = new System.Windows.Forms.TextBox();
            this.txtBtnWidth = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxButtonName
            // 
            this.textBoxButtonName.Location = new System.Drawing.Point(292, 42);
            this.textBoxButtonName.Name = "textBoxButtonName";
            this.textBoxButtonName.Size = new System.Drawing.Size(100, 21);
            this.textBoxButtonName.TabIndex = 0;
            // 
            // textBoxButtonClickContent
            // 
            this.textBoxButtonClickContent.Location = new System.Drawing.Point(292, 104);
            this.textBoxButtonClickContent.Name = "textBoxButtonClickContent";
            this.textBoxButtonClickContent.Size = new System.Drawing.Size(100, 21);
            this.textBoxButtonClickContent.TabIndex = 1;
            // 
            // 按钮名称
            // 
            this.按钮名称.AutoSize = true;
            this.按钮名称.Location = new System.Drawing.Point(219, 45);
            this.按钮名称.Name = "按钮名称";
            this.按钮名称.Size = new System.Drawing.Size(53, 12);
            this.按钮名称.TabIndex = 2;
            this.按钮名称.Text = "按钮名称";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(219, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "写入地址";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(219, 166);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "读取地址";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(292, 161);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 21);
            this.textBox3.TabIndex = 4;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(212, 209);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 6;
            this.buttonSave.Text = "保存";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 168);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "按钮高度";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "按钮宽度";
            // 
            // txtBtnHight
            // 
            this.txtBtnHight.Location = new System.Drawing.Point(85, 163);
            this.txtBtnHight.Name = "txtBtnHight";
            this.txtBtnHight.Size = new System.Drawing.Size(100, 21);
            this.txtBtnHight.TabIndex = 8;
            // 
            // txtBtnWidth
            // 
            this.txtBtnWidth.Location = new System.Drawing.Point(85, 101);
            this.txtBtnWidth.Name = "txtBtnWidth";
            this.txtBtnWidth.Size = new System.Drawing.Size(100, 21);
            this.txtBtnWidth.TabIndex = 7;
            // 
            // InputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 244);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtBtnHight);
            this.Controls.Add(this.txtBtnWidth);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.按钮名称);
            this.Controls.Add(this.textBoxButtonClickContent);
            this.Controls.Add(this.textBoxButtonName);
            this.Name = "InputForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InputForm";
            this.Load += new System.EventHandler(this.InputForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxButtonName;
        private System.Windows.Forms.TextBox textBoxButtonClickContent;
        private System.Windows.Forms.Label 按钮名称;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtBtnHight;
        private System.Windows.Forms.TextBox txtBtnWidth;
    }
}