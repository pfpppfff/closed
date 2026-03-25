namespace 平移法
{
    partial class Form2
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
            this.customButtonControl1 = new 平移法.CustomButtonControl();
            this.customButtonControl2 = new 平移法.CustomButtonControl();
            this.SuspendLayout();
            // 
            // customButtonControl1
            // 
            this.customButtonControl1.ButtonText = "自定义按钮";
            this.customButtonControl1.Location = new System.Drawing.Point(571, 117);
            this.customButtonControl1.ModeSel = 1;
            this.customButtonControl1.Name = "customButtonControl1";
            this.customButtonControl1.Size = new System.Drawing.Size(120, 50);
            this.customButtonControl1.TabIndex = 0;
            // 
            // customButtonControl2
            // 
            this.customButtonControl2.ButtonText = "自定义按钮";
            this.customButtonControl2.Location = new System.Drawing.Point(571, 203);
            this.customButtonControl2.ModeSel = 2;
            this.customButtonControl2.Name = "customButtonControl2";
            this.customButtonControl2.Size = new System.Drawing.Size(120, 50);
            this.customButtonControl2.TabIndex = 1;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.customButtonControl2);
            this.Controls.Add(this.customButtonControl1);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CustomButtonControl customButtonControl1;
        private CustomButtonControl customButtonControl2;
    }
}