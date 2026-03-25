namespace RecipeManager
{
    partial class Frm_PfRecipeManager
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
            this.dataGridViewPumpModels = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPumpModels)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewPumpModels
            // 
            this.dataGridViewPumpModels.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPumpModels.Location = new System.Drawing.Point(84, 66);
            this.dataGridViewPumpModels.Name = "dataGridViewPumpModels";
            this.dataGridViewPumpModels.RowHeadersWidth = 51;
            this.dataGridViewPumpModels.RowTemplate.Height = 27;
            this.dataGridViewPumpModels.Size = new System.Drawing.Size(625, 430);
            this.dataGridViewPumpModels.TabIndex = 0;
            this.dataGridViewPumpModels.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewPumpModels_CellContentClick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(84, 23);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "添加泵编号";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(619, 13);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "保存";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(843, 593);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridViewPumpModels);
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPumpModels)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button daa;
        private System.Windows.Forms.DataGridView dataGridViewPumpModels;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

