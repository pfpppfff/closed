﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿namespace Frm_NpshConfig
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnConfig = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelSelectedData = new System.Windows.Forms.Panel();
            this.lblSelectedDataTitle = new System.Windows.Forms.Label();
            this.lblTestNumber = new System.Windows.Forms.Label();
            this.txtTestNumber = new System.Windows.Forms.TextBox();
            this.lblFlow = new System.Windows.Forms.Label();
            this.txtFlow = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.lblHead = new System.Windows.Forms.Label();
            this.txtHead = new System.Windows.Forms.TextBox();
            this.lblRemark = new System.Windows.Forms.Label();
            this.txtRemark = new System.Windows.Forms.TextBox();
            this.panelDataGroup = new System.Windows.Forms.Panel();
            this.lblDataGroupTitle = new System.Windows.Forms.Label();
            this.dgvDataGroup = new System.Windows.Forms.DataGridView();
            this.colGroupFlow = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGroupResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelMain.SuspendLayout();
            this.panelSelectedData.SuspendLayout();
            this.panelDataGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataGroup)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConfig
            // 
            this.btnConfig.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnConfig.FlatAppearance.BorderSize = 0;
            this.btnConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfig.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConfig.ForeColor = System.Drawing.Color.White;
            this.btnConfig.Location = new System.Drawing.Point(563, 100);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(150, 50);
            this.btnConfig.TabIndex = 2;
            this.btnConfig.Text = "汽蚀试验配置";
            this.btnConfig.UseVisualStyleBackColor = false;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblTitle.Location = new System.Drawing.Point(513, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(206, 31);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "汽蚀试验管理系统";
            // 
            // lblDescription
            // 
            this.lblDescription.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblDescription.Location = new System.Drawing.Point(533, 70);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(205, 20);
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Text = "点击下方按钮进入参数配置界面";
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.lblTitle);
            this.panelMain.Controls.Add(this.lblDescription);
            this.panelMain.Controls.Add(this.btnConfig);
            this.panelMain.Controls.Add(this.panelSelectedData);
            this.panelMain.Controls.Add(this.panelDataGroup);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1326, 450);
            this.panelMain.TabIndex = 3;
            // 
            // panelSelectedData
            // 
            this.panelSelectedData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panelSelectedData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSelectedData.Controls.Add(this.lblSelectedDataTitle);
            this.panelSelectedData.Controls.Add(this.lblTestNumber);
            this.panelSelectedData.Controls.Add(this.txtTestNumber);
            this.panelSelectedData.Controls.Add(this.lblFlow);
            this.panelSelectedData.Controls.Add(this.txtFlow);
            this.panelSelectedData.Controls.Add(this.lblStatus);
            this.panelSelectedData.Controls.Add(this.txtStatus);
            this.panelSelectedData.Controls.Add(this.lblHead);
            this.panelSelectedData.Controls.Add(this.txtHead);
            this.panelSelectedData.Controls.Add(this.lblRemark);
            this.panelSelectedData.Controls.Add(this.txtRemark);
            this.panelSelectedData.Location = new System.Drawing.Point(400, 180);
            this.panelSelectedData.Name = "panelSelectedData";
            this.panelSelectedData.Size = new System.Drawing.Size(775, 250);
            this.panelSelectedData.TabIndex = 4;
            // 
            // lblSelectedDataTitle
            // 
            this.lblSelectedDataTitle.AutoSize = true;
            this.lblSelectedDataTitle.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSelectedDataTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblSelectedDataTitle.Location = new System.Drawing.Point(20, 15);
            this.lblSelectedDataTitle.Name = "lblSelectedDataTitle";
            this.lblSelectedDataTitle.Size = new System.Drawing.Size(154, 22);
            this.lblSelectedDataTitle.TabIndex = 0;
            this.lblSelectedDataTitle.Text = "当前选中的试验数据";
            // 
            // lblTestNumber
            // 
            this.lblTestNumber.AutoSize = true;
            this.lblTestNumber.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTestNumber.Location = new System.Drawing.Point(30, 60);
            this.lblTestNumber.Name = "lblTestNumber";
            this.lblTestNumber.Size = new System.Drawing.Size(51, 20);
            this.lblTestNumber.TabIndex = 1;
            this.lblTestNumber.Text = "序号：";
            // 
            // txtTestNumber
            // 
            this.txtTestNumber.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtTestNumber.Location = new System.Drawing.Point(90, 57);
            this.txtTestNumber.Name = "txtTestNumber";
            this.txtTestNumber.ReadOnly = true;
            this.txtTestNumber.Size = new System.Drawing.Size(120, 25);
            this.txtTestNumber.TabIndex = 2;
            this.txtTestNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblFlow
            // 
            this.lblFlow.AutoSize = true;
            this.lblFlow.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblFlow.Location = new System.Drawing.Point(250, 60);
            this.lblFlow.Name = "lblFlow";
            this.lblFlow.Size = new System.Drawing.Size(123, 20);
            this.lblFlow.TabIndex = 3;
            this.lblFlow.Text = "取样流量(m³/h)：";
            // 
            // txtFlow
            // 
            this.txtFlow.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtFlow.Location = new System.Drawing.Point(380, 57);
            this.txtFlow.Name = "txtFlow";
            this.txtFlow.ReadOnly = true;
            this.txtFlow.Size = new System.Drawing.Size(120, 25);
            this.txtFlow.TabIndex = 4;
            this.txtFlow.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblStatus.Location = new System.Drawing.Point(540, 60);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(51, 20);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "状态：";
            // 
            // txtStatus
            // 
            this.txtStatus.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtStatus.Location = new System.Drawing.Point(600, 57);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(80, 25);
            this.txtStatus.TabIndex = 6;
            this.txtStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblHead
            // 
            this.lblHead.AutoSize = true;
            this.lblHead.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblHead.Location = new System.Drawing.Point(30, 110);
            this.lblHead.Name = "lblHead";
            this.lblHead.Size = new System.Drawing.Size(102, 20);
            this.lblHead.TabIndex = 7;
            this.lblHead.Text = "临界扬程(m)：";
            // 
            // txtHead
            // 
            this.txtHead.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtHead.Location = new System.Drawing.Point(150, 107);
            this.txtHead.Name = "txtHead";
            this.txtHead.ReadOnly = true;
            this.txtHead.Size = new System.Drawing.Size(120, 25);
            this.txtHead.TabIndex = 8;
            this.txtHead.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblRemark
            // 
            this.lblRemark.AutoSize = true;
            this.lblRemark.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblRemark.Location = new System.Drawing.Point(30, 160);
            this.lblRemark.Name = "lblRemark";
            this.lblRemark.Size = new System.Drawing.Size(51, 20);
            this.lblRemark.TabIndex = 9;
            this.lblRemark.Text = "备注：";
            // 
            // txtRemark
            // 
            this.txtRemark.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtRemark.Location = new System.Drawing.Point(90, 157);
            this.txtRemark.Multiline = true;
            this.txtRemark.Name = "txtRemark";
            this.txtRemark.ReadOnly = true;
            this.txtRemark.Size = new System.Drawing.Size(590, 70);
            this.txtRemark.TabIndex = 10;
            // 
            // panelDataGroup
            // 
            this.panelDataGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panelDataGroup.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDataGroup.Controls.Add(this.lblDataGroupTitle);
            this.panelDataGroup.Controls.Add(this.dgvDataGroup);
            this.panelDataGroup.Location = new System.Drawing.Point(50, 180);
            this.panelDataGroup.Name = "panelDataGroup";
            this.panelDataGroup.Size = new System.Drawing.Size(330, 250);
            this.panelDataGroup.TabIndex = 5;
            // 
            // lblDataGroupTitle
            // 
            this.lblDataGroupTitle.AutoSize = true;
            this.lblDataGroupTitle.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblDataGroupTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblDataGroupTitle.Location = new System.Drawing.Point(20, 15);
            this.lblDataGroupTitle.Name = "lblDataGroupTitle";
            this.lblDataGroupTitle.Size = new System.Drawing.Size(106, 22);
            this.lblDataGroupTitle.TabIndex = 0;
            this.lblDataGroupTitle.Text = "主界面数据组";
            // 
            // dgvDataGroup
            // 
            this.dgvDataGroup.AllowUserToAddRows = false;
            this.dgvDataGroup.AllowUserToDeleteRows = false;
            this.dgvDataGroup.BackgroundColor = System.Drawing.Color.White;
            this.dgvDataGroup.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(144)))), ((int)(((byte)(220)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDataGroup.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDataGroup.ColumnHeadersHeight = 35;
            this.dgvDataGroup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvDataGroup.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colGroupFlow,
            this.colGroupResult});
            this.dgvDataGroup.EnableHeadersVisualStyles = false;
            this.dgvDataGroup.Location = new System.Drawing.Point(20, 50);
            this.dgvDataGroup.Name = "dgvDataGroup";
            this.dgvDataGroup.RowHeadersVisible = false;
            this.dgvDataGroup.RowTemplate.Height = 30;
            this.dgvDataGroup.Size = new System.Drawing.Size(290, 180);
            this.dgvDataGroup.TabIndex = 1;
            this.dgvDataGroup.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDataGroup_CellValueChanged);
            // 
            // colGroupFlow
            // 
            this.colGroupFlow.HeaderText = "流量(m³/h)";
            this.colGroupFlow.Name = "colGroupFlow";
            this.colGroupFlow.Width = 140;
            // 
            // colGroupResult
            // 
            this.colGroupResult.HeaderText = "测试结果";
            this.colGroupResult.Name = "colGroupResult";
            this.colGroupResult.Width = 140;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1326, 450);
            this.Controls.Add(this.panelMain);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "汽蚀试验管理系统";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.panelSelectedData.ResumeLayout(false);
            this.panelSelectedData.PerformLayout();
            this.panelDataGroup.ResumeLayout(false);
            this.panelDataGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataGroup)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnConfig;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelSelectedData;
        private System.Windows.Forms.Panel panelDataGroup;
        private System.Windows.Forms.Label lblDataGroupTitle;
        private System.Windows.Forms.DataGridView dgvDataGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGroupFlow;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGroupResult;
        private System.Windows.Forms.Label lblSelectedDataTitle;
        private System.Windows.Forms.Label lblTestNumber;
        private System.Windows.Forms.TextBox txtTestNumber;
        private System.Windows.Forms.Label lblFlow;
        private System.Windows.Forms.TextBox txtFlow;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label lblHead;
        private System.Windows.Forms.TextBox txtHead;
        private System.Windows.Forms.Label lblRemark;
        private System.Windows.Forms.TextBox txtRemark;
    }
}

