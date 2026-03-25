﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frm_NpshConfig
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SetupButtonStyles();

            // 初始化数据组
            InitializeDataGroup();

            // 初始化显示选中数据
            UpdateSelectedDataDisplay();
        }

        /// <summary>
        /// 设置按钮样式和鼠标悬停效果
        /// </summary>
        private void SetupButtonStyles()
        {
            // 为配置按钮添加鼠标悬停效果
            btnConfig.MouseEnter += (s, e) =>
            {
                btnConfig.BackColor = Color.FromArgb(0, 105, 217);
            };
            btnConfig.MouseLeave += (s, e) =>
            {
                btnConfig.BackColor = Color.FromArgb(0, 123, 255);
            };
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            ConfigForm configForm = new ConfigForm(this); // 传递主界面引用
            var result = configForm.ShowDialog();

            // 如果用户点击了保存，更新显示数据
            if (result == DialogResult.OK)
            {
                UpdateSelectedDataDisplay();
                // 同步更新配置表的状态
                SyncConfigurationStatus();
            }
        }

        /// <summary>
        /// 更新主界面显示的选中数据
        /// </summary>
        private void UpdateSelectedDataDisplay()
        {
            try
            {
                if (ConfigForm.HasSelectedTestData())
                {
                    var selectedData = ConfigForm.GetSelectedTestData();

                    // 更新文本框显示
                    txtTestNumber.Text = selectedData.TestNumber;
                    txtFlow.Text = selectedData.Flow.ToString("F2");
                    txtStatus.Text = selectedData.Status;
                    txtHead.Text = selectedData.Head.ToString("F2");
                    txtRemark.Text = selectedData.Remark;

                    // 设置有数据时的样式
                    SetDataAvailableStyle();
                }
                else
                {
                    // 清空文本框
                    txtTestNumber.Text = "未选中";
                    txtFlow.Text = "未选中";
                    txtStatus.Text = "未选中";
                    txtHead.Text = "未选中";
                    txtRemark.Text = "请在配置页面选择需要重测的试验数据";

                    // 设置无数据时的样式
                    SetNoDataStyle();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新显示数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 设置有数据时的样式
        /// </summary>
        private void SetDataAvailableStyle()
        {
            foreach (Control control in panelSelectedData.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.BackColor = Color.FromArgb(240, 248, 255);
                    textBox.ForeColor = Color.FromArgb(51, 122, 183);
                }
            }
        }

        /// <summary>
        /// 设置无数据时的样式
        /// </summary>
        private void SetNoDataStyle()
        {
            foreach (Control control in panelSelectedData.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.BackColor = Color.FromArgb(248, 249, 250);
                    textBox.ForeColor = Color.FromArgb(108, 117, 125);
                }
            }
        }

        /// <summary>
        /// 初始化主界面数据组
        /// </summary>
        private void InitializeDataGroup()
        {
            try
            {
                // 添加示例数据（模拟实际测试结果）
                dgvDataGroup.Rows.Add("95.00", "合格");
                dgvDataGroup.Rows.Add("33.00", "合格");
                dgvDataGroup.Rows.Add("120.00", "不合格");
                dgvDataGroup.Rows.Add("150.00", "合格");

                // 设置列居中对齐
                foreach (DataGridViewColumn column in dgvDataGroup.Columns)
                {
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"初始化数据组失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 数据组单元格值改变事件
        /// </summary>
        private void dgvDataGroup_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // 同步更新配置表的状态
                SyncConfigurationStatus();
            }
        }

        /// <summary>
        /// 获取主界面数据组信息，供配置页面使用
        /// </summary>
        /// <returns>数据组信息（流量值，测试结果）</returns>
        public Dictionary<double, string> GetDataGroupInfo()
        {
            var dataGroupInfo = new Dictionary<double, string>();
            
            try
            {
                foreach (DataGridViewRow row in dgvDataGroup.Rows)
                {
                    if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                    {
                        if (double.TryParse(row.Cells[0].Value.ToString(), out double flow))
                        {
                            string result = row.Cells[1].Value.ToString();
                            dataGroupInfo[flow] = result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取数据组信息失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            return dataGroupInfo;
        }
        
        /// <summary>
        /// 同步更新配置表中对应流量值的状态
        /// </summary>
        private void SyncConfigurationStatus()
        {
            try
            {
                // 获取当前配置数据
                var config = DataManager.LoadConfig();
                bool hasChanges = false;

                // 遍历主界面数据组
                foreach (DataGridViewRow row in dgvDataGroup.Rows)
                {
                    if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                    {
                        if (double.TryParse(row.Cells[0].Value.ToString(), out double groupFlow))
                        {
                            string result = row.Cells[1].Value.ToString();

                            // 在配置表中查找对应的流量值
                            foreach (var configData in config.TestDataList)
                            {
                                if (Math.Abs(configData.Flow - groupFlow) < 0.01) // 浮点数比较
                                {
                                    // 根据测试结果更新状态
                                    string newStatus = (result == "合格" || result == "不合格") ? "已测" : "待测";
                                    if (configData.Status != newStatus)
                                    {
                                        configData.Status = newStatus;
                                      hasChanges = true;
                                    }
                                }
                            }
                        }
                    }
                }

                // 如果有更改，保存配置
                if (hasChanges)
                {
                    DataManager.SaveConfig(config);
                    // 通知ConfigForm刷新显示
                    ConfigForm.RefreshDisplayRequested = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"同步状态失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           // SyncConfigurationStatus();
        }
    }
}
