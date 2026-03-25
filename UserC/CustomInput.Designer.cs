namespace UserC
{
    partial class CustomInput
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.txtWrite = new Sunny.UI.UIDoubleUpDown();
            this.SuspendLayout();
            // 
            // txtWrite
            // 
            this.txtWrite.AutoSize = true;
            this.txtWrite.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtWrite.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtWrite.Location = new System.Drawing.Point(0, 0);
            this.txtWrite.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtWrite.MinimumSize = new System.Drawing.Size(100, 0);
            this.txtWrite.Name = "txtWrite";
            this.txtWrite.Size = new System.Drawing.Size(105, 30);
            this.txtWrite.TabIndex = 0;
            this.txtWrite.Text = "uiDoubleUpDown1";
            this.txtWrite.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CustomInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtWrite);
            this.Name = "CustomInput";
            this.Size = new System.Drawing.Size(105, 30);
            this.Load += new System.EventHandler(this.UserControl1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Sunny.UI.UIDoubleUpDown txtWrite;
    }
}
