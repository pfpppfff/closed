namespace UserC
{
    partial class CustomUIGateBtn
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnOpen = new Sunny.UI.UIButton();
            this.BtnClose = new Sunny.UI.UIButton();
            this.uiLightOpen = new Sunny.UI.UILight();
            this.uiLightClose = new Sunny.UI.UILight();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.tableLayoutPanel1.Controls.Add(this.BtnOpen, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.BtnClose, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.uiLightOpen, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.uiLightClose, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(120, 70);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // BtnOpen
            // 
            this.BtnOpen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnOpen.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnOpen.Location = new System.Drawing.Point(43, 3);
            this.BtnOpen.MinimumSize = new System.Drawing.Size(1, 1);
            this.BtnOpen.Name = "BtnOpen";
            this.BtnOpen.Size = new System.Drawing.Size(74, 29);
            this.BtnOpen.TabIndex = 0;
            this.BtnOpen.Text = "uiButton1";
            this.BtnOpen.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // BtnClose
            // 
            this.BtnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnClose.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnClose.Location = new System.Drawing.Point(43, 38);
            this.BtnClose.MinimumSize = new System.Drawing.Size(1, 1);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(74, 29);
            this.BtnClose.TabIndex = 1;
            this.BtnClose.Text = "uiButton2";
            this.BtnClose.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // uiLightOpen
            // 
            this.uiLightOpen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLightOpen.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLightOpen.Location = new System.Drawing.Point(3, 3);
            this.uiLightOpen.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiLightOpen.Name = "uiLightOpen";
            this.uiLightOpen.Radius = 29;
            this.uiLightOpen.Size = new System.Drawing.Size(34, 29);
            this.uiLightOpen.TabIndex = 2;
            this.uiLightOpen.Text = "uiLight1";
            // 
            // uiLightClose
            // 
            this.uiLightClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLightClose.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLightClose.Location = new System.Drawing.Point(3, 38);
            this.uiLightClose.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiLightClose.Name = "uiLightClose";
            this.uiLightClose.Radius = 29;
            this.uiLightClose.Size = new System.Drawing.Size(34, 29);
            this.uiLightClose.TabIndex = 3;
            this.uiLightClose.Text = "uiLight2";
            // 
            // ControlGataBtn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ControlGataBtn";
            this.Size = new System.Drawing.Size(120, 70);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Sunny.UI.UIButton BtnOpen;
        private Sunny.UI.UIButton BtnClose;
        private Sunny.UI.UILight uiLightOpen;
        private Sunny.UI.UILight uiLightClose;
    }
}
