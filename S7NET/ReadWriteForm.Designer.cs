using System;
using System.Drawing;
using System.Windows.Forms;

namespace S7NET
{
    partial class ReadWriteForm
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
            this.SuspendLayout();
            
            // 窗体设置
            this.Text = "PLC数据读写";
            this.Size = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // PLC选择标签
            var lblPlc = new Label
            {
                Text = "选择PLC:",
                Location = new Point(12, 15),
                Size = new Size(80, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblPlc);

            // PLC选择下拉框
            cmbPlc = new ComboBox
            {
                Location = new Point(100, 12),
                Size = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cmbPlc);

            // 数据类型标签
            var lblDataType = new Label
            {
                Text = "数据类型:",
                Location = new Point(270, 15),
                Size = new Size(80, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblDataType);

            // 数据类型下拉框
            cmbDataType = new ComboBox
            {
                Location = new Point(360, 12),
                Size = new Size(100, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cmbDataType);

            // 地址标签
            var lblAddress = new Label
            {
                Text = "地址:",
                Location = new Point(12, 55),
                Size = new Size(80, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblAddress);

            // 地址输入框
            txtAddress = new TextBox
            {
                Location = new Point(100, 52),
                Size = new Size(200, 23),
                Font = new Font("Consolas", 9F)
            };
            this.Controls.Add(txtAddress);

            // 值标签
            var lblValue = new Label
            {
                Text = "值:",
                Location = new Point(320, 55),
                Size = new Size(40, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblValue);

            // 值输入框
            txtValue = new TextBox
            {
                Location = new Point(360, 52),
                Size = new Size(150, 23),
                Font = new Font("Consolas", 9F)
            };
            this.Controls.Add(txtValue);

            // 读取按钮
            btnRead = new Button
            {
                Text = "读取",
                Location = new Point(530, 52),
                Size = new Size(70, 30),
                UseVisualStyleBackColor = true
            };
            btnRead.Click += btnRead_Click;
            this.Controls.Add(btnRead);

            // 写入按钮
            btnWrite = new Button
            {
                Text = "写入",
                Location = new Point(610, 52),
                Size = new Size(70, 30),
                UseVisualStyleBackColor = true
            };
            btnWrite.Click += btnWrite_Click;
            this.Controls.Add(btnWrite);

            // 结果标签
            var lblResult = new Label
            {
                Text = "结果:",
                Location = new Point(12, 95),
                Size = new Size(80, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblResult);

            // 结果显示框
            txtResult = new TextBox
            {
                Location = new Point(100, 92),
                Size = new Size(580, 23),
                ReadOnly = true,
                BackColor = SystemColors.Control,
                Font = new Font("Consolas", 9F)
            };
            this.Controls.Add(txtResult);

            // 常用地址示例标签
            var lblExamples = new Label
            {
                Text = "地址示例: DB15.DBD0 (浮点数), DB15.DBW4 (字), DB15.DBB8 (字节), M0.0 (位)",
                Location = new Point(12, 125),
                Size = new Size(650, 20),
                ForeColor = Color.Gray,
                Font = new Font("Microsoft YaHei", 8F)
            };
            this.Controls.Add(lblExamples);

            // 操作历史标签
            var lblHistory = new Label
            {
                Text = "操作历史:",
                Location = new Point(12, 155),
                Size = new Size(100, 23),
                Font = new Font("Microsoft YaHei", 9F, FontStyle.Bold)
            };
            this.Controls.Add(lblHistory);

            // 操作历史列表框
            lstHistory = new ListBox
            {
                Location = new Point(12, 185),
                Size = new Size(668, 370),
                Font = new Font("Consolas", 9F)
            };
            this.Controls.Add(lstHistory);

            this.ResumeLayout(false);
        }

        #endregion
    }
}
