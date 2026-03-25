namespace UserC
{
    partial class CustomGateValveBtn
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
            this.UI_Open = new Sunny.UI.UILight();
            this.UI_Close = new Sunny.UI.UILight();
            this.BtnOpen = new Sunny.UI.UIButton();
            this.BtnClose = new Sunny.UI.UIButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.84564F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.15437F));
            this.tableLayoutPanel1.Controls.Add(this.UI_Open, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.UI_Close, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.BtnOpen, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.BtnClose, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(127, 70);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // UI_Open
            // 
            this.UI_Open.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UI_Open.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.UI_Open.Location = new System.Drawing.Point(3, 3);
            this.UI_Open.MinimumSize = new System.Drawing.Size(1, 1);
            this.UI_Open.Name = "UI_Open";
            this.UI_Open.Radius = 28;
            this.UI_Open.Size = new System.Drawing.Size(28, 29);
            this.UI_Open.TabIndex = 0;
            this.UI_Open.Text = "uiLight1";
            // 
            // UI_Close
            // 
            this.UI_Close.CenterColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.UI_Close.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UI_Close.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.UI_Close.Location = new System.Drawing.Point(3, 38);
            this.UI_Close.MinimumSize = new System.Drawing.Size(1, 1);
            this.UI_Close.Name = "UI_Close";
            this.UI_Close.OnCenterColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.UI_Close.OnColor = System.Drawing.Color.Red;
            this.UI_Close.Radius = 28;
            this.UI_Close.Size = new System.Drawing.Size(28, 29);
            this.UI_Close.TabIndex = 1;
            this.UI_Close.Text = "uiLight2";
            // 
            // BtnOpen
            // 
            this.BtnOpen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnOpen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnOpen.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnOpen.Location = new System.Drawing.Point(37, 3);
            this.BtnOpen.MinimumSize = new System.Drawing.Size(1, 1);
            this.BtnOpen.Name = "BtnOpen";
            this.BtnOpen.Size = new System.Drawing.Size(87, 29);
            this.BtnOpen.TabIndex = 2;
            this.BtnOpen.Text = "BtnOpen";
            this.BtnOpen.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // BtnClose
            // 
            this.BtnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnClose.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnClose.Location = new System.Drawing.Point(37, 38);
            this.BtnClose.MinimumSize = new System.Drawing.Size(1, 1);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(87, 29);
            this.BtnClose.TabIndex = 3;
            this.BtnClose.Text = "BtnClose";
            this.BtnClose.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // CustomGateValveBtn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CustomGateValveBtn";
            this.Size = new System.Drawing.Size(127, 70);
            this.Load += new System.EventHandler(this.CustomGateValveBtn_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Sunny.UI.UILight UI_Open;
        private Sunny.UI.UILight UI_Close;
        private Sunny.UI.UIButton BtnOpen;
        private Sunny.UI.UIButton BtnClose;
    }
}
