using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserDefinedControl.OPCUA;

namespace UserDefinedControl.OPCUA
{
    /// <summary>
    /// 多PLC状态监控窗体示例
    /// 演示如何使用优化后的OPC UA服务管理多个PLC
    /// </summary>
    public partial class MultiPlcMonitorForm : Form
    {
        #region 字段和属性
        private IOpcUaService _opcService;
        private Timer _statusUpdateTimer;
        private DataGridView _plcStatusGrid;
        private Label _summaryLabel;
        private Button _addPlcButton;
        private Button _removeSelectedButton;
        private Button _updateAllButton;
        private GroupBox _controlGroup;
        private GroupBox _statusGroup;
        private TextBox _logTextBox;
        #endregion

        #region 构造函数
        public MultiPlcMonitorForm()
        {
            InitializeComponent();
            InitializeOpcService();
            StartStatusMonitoring();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MultiPlcMonitorForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "MultiPlcMonitorForm";
            this.Load += new System.EventHandler(this.MultiPlcMonitorForm_Load);
            this.ResumeLayout(false);

        }

        private void SetupDataGridColumns()
        {
            _plcStatusGrid.Columns.Clear();
            _plcStatusGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PlcId",
                HeaderText = "PLC ID",
                Width = 80,
                DataPropertyName = "PlcId"
            });
            _plcStatusGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PlcName",
                HeaderText = "PLC名称",
                Width = 120,
                DataPropertyName = "PlcName"
            });
            _plcStatusGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SystemStatus",
                HeaderText = "系统状态",
                Width = 80,
                DataPropertyName = "SystemStatus"
            });
            _plcStatusGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ConnectionStatus",
                HeaderText = "连接状态",
                Width = 80,
                DataPropertyName = "ConnectionStatus"
            });
            _plcStatusGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "LastUpdateTime",
                HeaderText = "最后更新",
                Width = 140,
                DataPropertyName = "LastUpdateTime"
            });
            _plcStatusGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SystemStatusNodeId",
                HeaderText = "状态节点地址",
                Width = 200,
                DataPropertyName = "SystemStatusNodeId"
            });
            _plcStatusGrid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "IsEnabled",
                HeaderText = "启用",
                Width = 60,
                DataPropertyName = "IsEnabled"
            });
        }
        #endregion

        #region 初始化和服务管理
        private void InitializeOpcService()
        {
            try
            {
                _opcService = OpcUaServiceManager.Current;
                
                // 注册PLC状态变化事件
                _opcService.PlcStatusChanged += OnPlcStatusChanged;
                
                // 添加示例PLC配置
                SetupExamplePlcConfigurations();
                
                LogMessage("多PLC监控服务初始化完成");
            }
            catch (Exception ex)
            {
                LogMessage($"服务初始化失败: {ex.Message}");
            }
        }

        private void SetupExamplePlcConfigurations()
        {
            // 添加示例PLC配置
            var examplePlcs = new List<PlcStatusModel>
            {
                new PlcStatusModel("PLC1", "主控PLC", "1214.PLC1._System._NoError", "主控制系统PLC", 1),
                new PlcStatusModel("PLC2", "辅助PLC", "1214.PLC2._System._NoError", "辅助控制系统PLC", 2),
                new PlcStatusModel("PLC3", "监控PLC", "1214.PLC3._System._NoError", "数据监控PLC", 3)
            };

            foreach (var plc in examplePlcs)
            {
                _opcService.AddOrUpdatePlc(plc);
            }
        }

        private void StartStatusMonitoring()
        {
            _statusUpdateTimer = new Timer
            {
                Interval = 2000 // 每2秒更新一次
            };
            _statusUpdateTimer.Tick += StatusUpdateTimer_Tick;
            _statusUpdateTimer.Start();
        }
        #endregion

        #region 事件处理
        private async void StatusUpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // 更新所有PLC状态
                await _opcService.UpdateAllPlcSystemStatusAsync();
                
                // 刷新显示
                RefreshPlcStatusDisplay();
                UpdateSummaryInfo();
            }
            catch (Exception ex)
            {
                LogMessage($"状态更新异常: {ex.Message}");
            }
        }

        private void OnPlcStatusChanged(object sender, PlcStatusChangedEventArgs e)
        {
            // 在UI线程中更新日志
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnPlcStatusChanged(sender, e)));
                return;
            }

            LogMessage(e.GetStatusChangeDescription());
            
            // 如果状态恶化，改变显示颜色
            if (e.IsDegraded)
            {
                // 可以添加声音提醒或其他告警机制
                LogMessage($"⚠️ 警告: {e.PlcName} 出现异常！");
            }
            else if (e.IsRecovered)
            {
                LogMessage($"✅ 恢复: {e.PlcName} 状态恢复正常");
            }
        }

        private void AddPlcButton_Click(object sender, EventArgs e)
        {
            // 弹出对话框添加新PLC
            using (var dialog = new AddPlcDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var newPlc = dialog.GetPlcModel();
                    if (_opcService.AddOrUpdatePlc(newPlc))
                    {
                        LogMessage($"成功添加PLC: {newPlc.PlcName}");
                        RefreshPlcStatusDisplay();
                    }
                    else
                    {
                        LogMessage($"添加PLC失败: {newPlc.PlcName}");
                    }
                }
            }
        }

        private void RemoveSelectedButton_Click(object sender, EventArgs e)
        {
            if (_plcStatusGrid.SelectedRows.Count > 0)
            {
                var selectedRow = _plcStatusGrid.SelectedRows[0];
                var plcId = selectedRow.Cells["PlcId"].Value?.ToString();
                
                if (!string.IsNullOrEmpty(plcId))
                {
                    if (_opcService.RemovePlc(plcId))
                    {
                        LogMessage($"成功删除PLC: {plcId}");
                        RefreshPlcStatusDisplay();
                    }
                    else
                    {
                        LogMessage($"删除PLC失败: {plcId}");
                    }
                }
            }
        }

        private async void UpdateAllButton_Click(object sender, EventArgs e)
        {
            try
            {
                _updateAllButton.Enabled = false;
                _updateAllButton.Text = "更新中...";
                
                int successCount = await _opcService.UpdateAllPlcSystemStatusAsync();
                LogMessage($"状态更新完成，成功更新 {successCount} 个PLC");
                
                RefreshPlcStatusDisplay();
                UpdateSummaryInfo();
            }
            catch (Exception ex)
            {
                LogMessage($"批量更新失败: {ex.Message}");
            }
            finally
            {
                _updateAllButton.Enabled = true;
                _updateAllButton.Text = "更新所有状态";
            }
        }

        private void MultiPlcMonitorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _statusUpdateTimer?.Stop();
            _statusUpdateTimer?.Dispose();
            
            if (_opcService != null)
            {
                _opcService.PlcStatusChanged -= OnPlcStatusChanged;
            }
        }
        #endregion

        #region 显示更新方法
        private void RefreshPlcStatusDisplay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(RefreshPlcStatusDisplay));
                return;
            }

            var allPlcs = _opcService.GetAllPlcStatus();
            var displayData = allPlcs.Select(plc => new
            {
                PlcId = plc.PlcId,
                PlcName = plc.PlcName,
                SystemStatus = plc.IsSystemNormal ? "正常" : "异常",
                ConnectionStatus = plc.IsConnected ? "已连接" : "断开",
                LastUpdateTime = plc.LastUpdateTime == DateTime.MinValue ? "--" : plc.LastUpdateTime.ToString("HH:mm:ss"),
                SystemStatusNodeId = plc.SystemStatusNodeId,
                IsEnabled = plc.IsEnabled
            }).ToList();

            _plcStatusGrid.DataSource = displayData;
            
            // 根据状态设置行颜色
            for (int i = 0; i < _plcStatusGrid.Rows.Count; i++)
            {
                var row = _plcStatusGrid.Rows[i];
                var systemStatus = row.Cells["SystemStatus"].Value?.ToString();
                var connectionStatus = row.Cells["ConnectionStatus"].Value?.ToString();
                
                if (connectionStatus != "已连接")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220); // 淡红色
                }
                else if (systemStatus != "正常")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 240, 200); // 淡黄色
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220); // 淡绿色
                }
            }
        }

        private void UpdateSummaryInfo()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateSummaryInfo));
                return;
            }

            int normalCount = _opcService.GetNormalPlcCount();
            int abnormalCount = _opcService.GetAbnormalPlcCount();
            int totalCount = normalCount + abnormalCount;
            
            _summaryLabel.Text = $"正常: {normalCount} | 异常: {abnormalCount} | 总计: {totalCount}";
            
            // 根据状态设置颜色
            if (abnormalCount > 0)
            {
                _summaryLabel.ForeColor = Color.Red;
            }
            else if (normalCount > 0)
            {
                _summaryLabel.ForeColor = Color.Green;
            }
            else
            {
                _summaryLabel.ForeColor = Color.DarkGray;
            }
        }

        private void LogMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(LogMessage), message);
                return;
            }

            string timeStamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string logEntry = $"[{timeStamp}] {message}";
            
            _logTextBox.AppendText(logEntry + Environment.NewLine);
            _logTextBox.SelectionStart = _logTextBox.Text.Length;
            _logTextBox.ScrollToCaret();
            
            // 限制日志长度
            if (_logTextBox.Lines.Length > 100)
            {
                var lines = _logTextBox.Lines.Skip(20).ToArray();
                _logTextBox.Lines = lines;
            }
        }
        #endregion

        private void MultiPlcMonitorForm_Load(object sender, EventArgs e)
        {

        }
    }

    /// <summary>
    /// 添加PLC对话框（简化版本）
    /// </summary>
    public class AddPlcDialog : Form
    {
        private TextBox _plcIdTextBox;
        private TextBox _plcNameTextBox;
        private TextBox _statusNodeTextBox;
        private TextBox _descriptionTextBox;
        private NumericUpDown _priorityNumeric;
        private Button _okButton;
        private Button _cancelButton;

        public AddPlcDialog()
        {
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            this.Text = "添加PLC配置";
            this.Size = new Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            var labels = new[]
            {
                new Label { Text = "PLC ID:", Location = new Point(20, 20), Size = new Size(80, 23) },
                new Label { Text = "PLC名称:", Location = new Point(20, 50), Size = new Size(80, 23) },
                new Label { Text = "状态节点:", Location = new Point(20, 80), Size = new Size(80, 23) },
                new Label { Text = "描述:", Location = new Point(20, 110), Size = new Size(80, 23) },
                new Label { Text = "优先级:", Location = new Point(20, 140), Size = new Size(80, 23) }
            };

            _plcIdTextBox = new TextBox { Location = new Point(110, 20), Size = new Size(250, 23) };
            _plcNameTextBox = new TextBox { Location = new Point(110, 50), Size = new Size(250, 23) };
            _statusNodeTextBox = new TextBox { Location = new Point(110, 80), Size = new Size(250, 23), Text = "1214.PLC1._System._NoError" };
            _descriptionTextBox = new TextBox { Location = new Point(110, 110), Size = new Size(250, 23) };
            _priorityNumeric = new NumericUpDown { Location = new Point(110, 140), Size = new Size(100, 23), Minimum = 0, Maximum = 999, Value = 1 };

            _okButton = new Button
            {
                Text = "确定",
                Location = new Point(200, 200),
                Size = new Size(75, 30),
                DialogResult = DialogResult.OK
            };

            _cancelButton = new Button
            {
                Text = "取消",
                Location = new Point(285, 200),
                Size = new Size(75, 30),
                DialogResult = DialogResult.Cancel
            };

            this.Controls.AddRange(labels);
            this.Controls.AddRange(new Control[] {
                _plcIdTextBox, _plcNameTextBox, _statusNodeTextBox,
                _descriptionTextBox, _priorityNumeric, _okButton, _cancelButton
            });

            this.AcceptButton = _okButton;
            this.CancelButton = _cancelButton;
        }

        public PlcStatusModel GetPlcModel()
        {
            return new PlcStatusModel(
                _plcIdTextBox.Text,
                _plcNameTextBox.Text,
                _statusNodeTextBox.Text,
                _descriptionTextBox.Text,
                (int)_priorityNumeric.Value
            );
        }
    }
}