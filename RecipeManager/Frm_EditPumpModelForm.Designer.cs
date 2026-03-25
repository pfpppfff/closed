namespace RecipeManager
{
    partial class Frm_EditPumpModelForm
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
            this.dataGridViewPowerEfficiency = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.txtModelName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPowerEfficiency)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewPowerEfficiency
            // 
            this.dataGridViewPowerEfficiency.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPowerEfficiency.Location = new System.Drawing.Point(12, 12);
            this.dataGridViewPowerEfficiency.Name = "dataGridViewPowerEfficiency";
            this.dataGridViewPowerEfficiency.RowHeadersWidth = 51;
            this.dataGridViewPowerEfficiency.RowTemplate.Height = 27;
            this.dataGridViewPowerEfficiency.Size = new System.Drawing.Size(410, 426);
            this.dataGridViewPowerEfficiency.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(469, 117);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 38);
            this.button1.TabIndex = 1;
            this.button1.Text = "保存";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtModelName
            // 
            this.txtModelName.Location = new System.Drawing.Point(469, 31);
            this.txtModelName.Name = "txtModelName";
            this.txtModelName.Size = new System.Drawing.Size(100, 25);
            this.txtModelName.TabIndex = 2;
            // 
            // EditPumpModelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(593, 450);
            this.Controls.Add(this.txtModelName);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridViewPowerEfficiency);
            this.Name = "EditPumpModelForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EditPumpModel";
            this.Load += new System.EventHandler(this.EditPumpModelForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPowerEfficiency)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewPowerEfficiency;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtModelName;
    }
}