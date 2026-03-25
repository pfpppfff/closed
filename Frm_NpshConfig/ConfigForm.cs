using System;
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
    public partial class ConfigForm : Form
    {
        private CavitationTestConfig _config;
        private BindingList<CavitationTestData> _bindingList;
        private Form1 _mainForm; // 主界面引用
        
        /// <summary>
        /// 全局静态属性，存储被勾选的试验数据，供其他页面使用
        /// </summary>
        public static CavitationTestData SelectedTestData { get; private set; }
        
        /// <summary>
        /// 标记是否需要刷新显示
        /// </summary>
        public static bool RefreshDisplayRequested { get; set; } = false;
        
        /// <summary>
        /// 获取当前选中的试验数据（供其他页面调用）
        /// </summary>
        /// <returns>选中的试验数据，如果没有选中则返回null</returns>
        public static CavitationTestData GetSelectedTestData()
        {
            return SelectedTestData;
        }
        
        /// <summary>
        /// 检查是否有选中的试验数据
        /// </summary>
        /// <returns>是否有选中的数据</returns>
        public static bool HasSelectedTestData()
        {
            return SelectedTestData != null;
        }

        public ConfigForm()
        {
            InitializeComponent();
        }
        
        public ConfigForm(Form1 mainForm) : this()
        {
            _mainForm = mainForm;
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            LoadData();
            SetupButtonStyles();
            
            // 立即根据主界面数据实时刷新状态
            UpdateConfigStatusFromMainForm();
            
            // 确保刷新显示和颜色
            this.BeginInvoke(new Action(() =>
            {
                // 重新加载最新数据并刷新显示
                RefreshDataFromFile();
                
                // 重置RefreshDisplayRequested标志
                RefreshDisplayRequested = false;
            }));
        }

        /// <summary>
        /// 强制设置列头居中对齐
        /// </summary>
        private void ForceHeaderAlignment()
        {
            // 等待界面完全加载
            this.BeginInvoke(new Action(() =>
            {
                // 强制设置每个列头居中
                foreach (DataGridViewColumn col in dgvConfig.Columns)
                {
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                foreach (DataGridViewColumn item in this.dgvConfig.Columns)
                {
                    item.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    item.SortMode = DataGridViewColumnSortMode.NotSortable;//列标题右边有预留一个排序小箭头的位置，所以整个列标题就向左边多一点，而当把SortMode属性设置为NotSortable时，不使用排序，也就没有那个预留的位置，所有完全居中了

                }
                // 强制刷新显示
                dgvConfig.Invalidate();
                dgvConfig.Refresh();
            }));
        }

        /// <summary>
        /// 设置按钮样式和鼠标悬停效果
        /// </summary>
        private void SetupButtonStyles()
        {
            // 为保存按钮添加鼠标悬停效果
            btnSave.MouseEnter += (s, e) =>
            {
                btnSave.BackColor = Color.FromArgb(34, 139, 58);
            };
            btnSave.MouseLeave += (s, e) =>
            {
                btnSave.BackColor = Color.FromArgb(40, 167, 69);
            };

            // 为取消按钮添加鼠标悬停效果
            btnCancel.MouseEnter += (s, e) =>
            {
                btnCancel.BackColor = Color.FromArgb(90, 98, 104);
            };
            btnCancel.MouseLeave += (s, e) =>
            {
                btnCancel.BackColor = Color.FromArgb(108, 117, 125);
            };

            // 为添加按钮添加鼠标悬停效果
            btnAdd.MouseEnter += (s, e) =>
            {
                btnAdd.BackColor = Color.FromArgb(0, 105, 217);
            };
            btnAdd.MouseLeave += (s, e) =>
            {
                btnAdd.BackColor = Color.FromArgb(0, 123, 255);
            };
        }

        /// <summary>
        /// 根据主界面数据表更新配置状态（实时刷新）
        /// </summary>
        private void UpdateConfigStatusFromMainForm()
        {
            try
            {
                if (_mainForm != null)
                {
                    // 获取主界面数据组信息
                    var mainDataGroupInfo = _mainForm.GetDataGroupInfo();
                    
                    bool hasChanges = false;
                    
                    // 遍历配置表中的所有数据
                    foreach (var configData in _config.TestDataList)
                    {
                        // 检查配置表中的流量值是否在主界面数据表中存在
                        bool foundInMainData = false;
                        string testResult = "";
                        
                        foreach (var dataGroup in mainDataGroupInfo)
                        {
                            double groupFlow = dataGroup.Key; 
                            string result = dataGroup.Value; 
                            
                            if (Math.Abs(configData.Flow - groupFlow) < 0.01) // 浮点数比较
                            {
                                foundInMainData = true;
                                testResult = result;
                                break;
                            }
                        }
                        
                        // 根据是否在主界面数据表中找到对应流量值来更新状态
                        string newStatus;
                        if (foundInMainData)
                        {
                            // 如果在主界面数据表中找到，根据测试结果更新状态
                            newStatus = (testResult == "合格" || testResult == "不合格") ? "已测" : "待测";
                        }
                        else
                        {
                            // 如果在主界面数据表中没有找到对应流量值，设置为待测
                            newStatus = "待测";
                        }
                        
                        // 检查状态是否需要更新
                        if (configData.Status != newStatus)
                        {
                            configData.Status = newStatus;
                            hasChanges = true;
                        }
                    }
                    
                    // 如果有更改，立即保存配置并刷新显示
                    if (hasChanges)
                    {
                        DataManager.SaveConfig(_config);
                        
                        // 立即刷新绑定列表
                        if (_bindingList != null)
                        {
                            _bindingList.Clear();
                            foreach (var item in _config.TestDataList)
                            {
                                _bindingList.Add(item);
                            }
                        }
                        
                        // 立即应用颜色设置
                        SetupStatusColumnColors();
                        
                        // 强制刷新界面
                        dgvConfig.Invalidate();
                        dgvConfig.Refresh();
                    }
                    else
                    {
                        // 即使没有状态更改，也要确保颜色正确显示
                        SetupStatusColumnColors();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新配置状态失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// 从文件重新加载数据并刷新显示
        /// </summary>
        private void RefreshDataFromFile()
        {
            try
            {
                // 重新从文件加载最新数据
                _config = DataManager.LoadConfig();
                
                // 清空并重新填充绑定列表
                _bindingList.Clear();
                foreach (var item in _config.TestDataList)
                {
                    _bindingList.Add(item);
                }
                
                // 重新排列序号
                ReorderTestNumbers();
                
                // 延迟设置颜色，确保数据绑定完成
                this.BeginInvoke(new Action(() =>
                {
                    SetupStatusColumnColors();
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刷新数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// 加载数据
        /// </summary>
        private void LoadData()
        {
            try
            {
                _config = DataManager.LoadConfig();
                
                // 如果已经有绑定列表，则清空并重新填充
                if (_bindingList != null)
                {
                    _bindingList.Clear();
                    foreach (var item in _config.TestDataList)
                    {
                        _bindingList.Add(item);
                    }
                }
                else
                {
                    // 第一次创建绑定列表
                    _bindingList = new BindingList<CavitationTestData>(_config.TestDataList);
                    dgvConfig.DataSource = _bindingList;
                }

                // 设置数据网格视图的一些属性
                dgvConfig.AutoGenerateColumns = false;
                dgvConfig.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvConfig.MultiSelect = false;

                // 设置列的样式
                SetupDataGridViewStyles();
                
                // 重新排列序号确保顺序正确
                ReorderTestNumbers();
                
                // 设置状态列的颜色
                SetupStatusColumnColors();

                // 确保事件处理程序已绑定（避免重复绑定）
                // 先移除可能存在的事件处理程序
                dgvConfig.CellPainting -= DgvConfig_CellPainting;
                dgvConfig.CellValueChanged -= DgvConfig_CellValueChanged;
                dgvConfig.CurrentCellDirtyStateChanged -= DgvConfig_CurrentCellDirtyStateChanged;
                dgvConfig.DataBindingComplete -= DgvConfig_DataBindingComplete;
                
                // 重新绑定事件处理程序
                dgvConfig.CellPainting += DgvConfig_CellPainting;
                dgvConfig.CellValueChanged += DgvConfig_CellValueChanged;
                dgvConfig.CurrentCellDirtyStateChanged += DgvConfig_CurrentCellDirtyStateChanged;
                dgvConfig.DataBindingComplete += DgvConfig_DataBindingComplete;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 设置DataGridView的样式
        /// </summary>
        private void SetupDataGridViewStyles()
        {
            // 设置有层次感的列头样式
            dgvConfig.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 144, 220);
            dgvConfig.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvConfig.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft YaHei", 10F, FontStyle.Bold);
            dgvConfig.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvConfig.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(42, 134, 210);
            dgvConfig.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            // 设置列头边框样式，增加层次感
            dgvConfig.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvConfig.ColumnHeadersHeight = 45;

            // 设置所有列的对齐方式和禁用排序
            foreach (DataGridViewColumn item in this.dgvConfig.Columns)
            {
                item.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                item.SortMode = DataGridViewColumnSortMode.NotSortable; //列标题右边有预留一个排序小箭头的位置，所以整个列标题就向左边多一点，而当把 SortMode属性设置为NotSortable时，不使用排序，也就没有那个预留的位置，所有完全居中了

                // 为每个列头设置渐变效果（模拟）
                item.HeaderCell.Style.BackColor = Color.FromArgb(52, 144, 220);
                item.HeaderCell.Style.ForeColor = Color.White;
                item.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // 设置数值列的格式
            dgvConfig.Columns["colFlow"].DefaultCellStyle.Format = "F2";
            dgvConfig.Columns["colHead"].DefaultCellStyle.Format = "F2";

            // 为取样流量列设置下边框，表示是输入数据
            DataGridViewTextBoxColumn flowColumn = dgvConfig.Columns["colFlow"] as DataGridViewTextBoxColumn;
            if (flowColumn != null)
            {
                flowColumn.DefaultCellStyle.BackColor = Color.FromArgb(250, 252, 255); // 轻微的蓝色背景
                flowColumn.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
                // 通过设置边框样式来模拟下边框效果
                flowColumn.DefaultCellStyle.Padding = new Padding(0, 0, 0, 2);
            }

            // 设置删除按钮的样式
            DataGridViewButtonColumn deleteColumn = dgvConfig.Columns["colDelete"] as DataGridViewButtonColumn;
            if (deleteColumn != null)
            {
                deleteColumn.DefaultCellStyle.BackColor = Color.FromArgb(220, 53, 69);
                deleteColumn.DefaultCellStyle.ForeColor = Color.White;
                deleteColumn.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 35, 51);
                deleteColumn.DefaultCellStyle.SelectionForeColor = Color.White;
            }

            // 设置CheckBox列的样式
            DataGridViewCheckBoxColumn retestColumn = dgvConfig.Columns["colRetest"] as DataGridViewCheckBoxColumn;
            if (retestColumn != null)
            {
                retestColumn.FalseValue = false;
                retestColumn.TrueValue = true;
                retestColumn.IndeterminateValue = false;
            }
        }

        /// <summary>
        /// 添加新的数据行
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                CavitationTestData newData = new CavitationTestData
                {
                    TestNumber = "", // 先置空，后面重新排列
                    Flow = 0.0,
                    Status = "待测",
                    Head = 0.0,
                    Retest = false,
                    Remark = ""
                };

                _bindingList.Add(newData);
                
                // 重新排列序号
                ReorderTestNumbers();
                
                // 选中新添加的行
                dgvConfig.ClearSelection();
                dgvConfig.Rows[dgvConfig.Rows.Count - 1].Selected = true;
                dgvConfig.CurrentCell = dgvConfig.Rows[dgvConfig.Rows.Count - 1].Cells[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 生成新的试验编号
        /// </summary>
        /// <returns>新的试验编号</returns>
        private string GenerateNewTestNumber()
        {
            int maxNumber = 0;
            foreach (var data in _bindingList)
            {
                if (int.TryParse(data.TestNumber, out int number))
                {
                    if (number > maxNumber)
                        maxNumber = number;
                }
            }
            return (maxNumber + 1).ToString("000");
        }

        /// <summary>
        /// 重新排列所有数据的序号，从001开始顺序排列
        /// </summary>
        private void ReorderTestNumbers()
        {
            try
            {
                for (int i = 0; i < _bindingList.Count; i++)
                {
                    _bindingList[i].TestNumber = (i + 1).ToString("000");
                }

                // 刷新显示
                dgvConfig.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"重新排列序号失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 设置状态列的颜色显示
        /// </summary>
        private void SetupStatusColumnColors()
        {
            try
            {
                // 确保DataGridView已经完成数据绑定
                if (dgvConfig.Rows.Count == 0)
                    return;
                    
                foreach (DataGridViewRow row in dgvConfig.Rows)
                {
                    if (!row.IsNewRow && row.Cells["colStatus"].Value != null)
                    {
                        string status = row.Cells["colStatus"].Value.ToString();
                        if (status == "已测")
                        {
                            // 已测状态设置为深绿色
                            row.Cells["colStatus"].Style.ForeColor = Color.FromArgb(0, 128, 0);
                            row.Cells["colStatus"].Style.Font = new Font(dgvConfig.Font, FontStyle.Bold);
                        }
                        else if (status == "待测")
                        {
                            // 待测状态设置为普通颜色
                            row.Cells["colStatus"].Style.ForeColor = Color.FromArgb(73, 80, 87);
                            row.Cells["colStatus"].Style.Font = new Font(dgvConfig.Font, FontStyle.Regular);
                        }
                    }
                }
                
                // 强制刷新显示
                dgvConfig.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"设置状态颜色失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 验证数据
                if (!ValidateData())
                    return;

                // 提交正在编辑的单元格
                dgvConfig.EndEdit();

                // 更新配置数据
                _config.TestDataList = _bindingList.ToList();
                
                // 提取被勾选的试验数据
                ExtractSelectedTestData();

                // 保存到文件
                DataManager.SaveConfig(_config);

                MessageBox.Show("配置数据保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 验证数据
        /// </summary>
        /// <returns>是否验证通过</returns>
        private bool ValidateData()
        {
            for (int i = 0; i < _bindingList.Count; i++)
            {
                var data = _bindingList[i];
                if (string.IsNullOrWhiteSpace(data.TestNumber))
                {
                    MessageBox.Show($"第{i + 1}行的序号不能为空！", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgvConfig.Rows[i].Selected = true;
                    dgvConfig.CurrentCell = dgvConfig.Rows[i].Cells[0];
                    return false;
                }

                if (data.Flow < 0 || data.Head < 0)
                {
                    MessageBox.Show($"第{i + 1}行的数据不能为负数！", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgvConfig.Rows[i].Selected = true;
                    return false;
                }

                if (string.IsNullOrWhiteSpace(data.Status) || (data.Status != "待测" && data.Status != "已测"))
                {
                    MessageBox.Show($"第{i + 1}行的状态必须为“待测”或“已测”！", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgvConfig.Rows[i].Selected = true;
                    dgvConfig.CurrentCell = dgvConfig.Rows[i].Cells[2]; // 状态列
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 提取被勾选的试验数据
        /// </summary>
        private void ExtractSelectedTestData()
        {
            try
            {
                // 清空之前的选中数据
                SelectedTestData = null;
                
                // 查找被勾选的数据
                foreach (var data in _bindingList)
                {
                    if (data.Retest == true)
                    {
                        // 创建选中数据的副本，避免引用问题
                        SelectedTestData = new CavitationTestData
                        {
                            TestNumber = data.TestNumber,
                            Flow = data.Flow,
                            Status = data.Status,
                            Head = data.Head,
                            Retest = data.Retest,
                            Remark = data.Remark
                        };
                        
                        // 显示选中的数据信息
                        string selectedInfo = $"已选中试验数据：\n" +
                                             $"序号：{SelectedTestData.TestNumber}\n" +
                                             $"取样流量：{SelectedTestData.Flow:F2} m³/h\n" +
                                             $"状态：{SelectedTestData.Status}\n" +
                                             $"临界扬程：{SelectedTestData.Head:F2} m\n" +
                                             $"备注：{SelectedTestData.Remark}";
                        
                        MessageBox.Show(selectedInfo, "选中的试验数据", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break; // 只能有一个被选中
                    }
                }
                
                // 如果没有选中任何数据
                if (SelectedTestData == null)
                {
                    MessageBox.Show("没有选中任何试验数据进行重测。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"提取选中数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 取消按钮点击事件
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// 数据网格视图单元格内容点击事件（处理删除按钮）
        /// </summary>
        private void dgvConfig_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // 检查是否点击的是删除按钮列
                if (e.ColumnIndex == dgvConfig.Columns["colDelete"].Index && e.RowIndex >= 0)
                {
                    // 确认删除
                    if (MessageBox.Show("确定要删除这条数据吗？", "确认删除",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // 检查是否至少保留一行数据
                        if (_bindingList.Count <= 1)
                        {
                            MessageBox.Show("至少需要保留一条数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        _bindingList.RemoveAt(e.RowIndex);
                        
                        // 重新排列序号
                        ReorderTestNumbers();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"删除数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 自定义单元格绘制事件，为取样流量列添加下边框
        /// </summary>
        private void DgvConfig_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // 检查是否是取样流量列且不是列头
            if (e.ColumnIndex == dgvConfig.Columns["colFlow"].Index && e.RowIndex >= 0)
            {
                // 绘制默认单元格内容
                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);

                // 绘制下边框
                using (Pen pen = new Pen(Color.FromArgb(52, 144, 220), 2)) // 蓝色下边框
                {
                    int y = e.CellBounds.Bottom - 1;
                    e.Graphics.DrawLine(pen, e.CellBounds.Left + 5, y, e.CellBounds.Right - 5, y);
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// 单元格值改变事件，实现是否重测扬程的单选功能
        /// </summary>
        private void DgvConfig_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 检查是否是“是否重测扬程”列
            if (e.ColumnIndex == dgvConfig.Columns["colRetest"].Index && e.RowIndex >= 0)
            {
                // 获取当前单元格的值
                var currentValue = dgvConfig.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                
                // 如果当前单元格被勾选
                if (currentValue != null && (bool)currentValue == true)
                {
                    // 取消其他所有行的勾选
                    for (int i = 0; i < dgvConfig.Rows.Count; i++)
                    {
                        if (i != e.RowIndex) // 跳过当前行
                        {
                            dgvConfig.Rows[i].Cells["colRetest"].Value = false;
                        }
                    }
                    
                    // 更新数据源
                    for (int i = 0; i < _bindingList.Count; i++)
                    {
                        if (i != e.RowIndex)
                        {
                            _bindingList[i].Retest = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 当前单元格脏状态改变事件，立即提交CheckBox的值改变
        /// </summary>
        private void DgvConfig_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // 如果当前单元格是CheckBox并且状态变脏，立即提交更改
            if (dgvConfig.IsCurrentCellDirty && dgvConfig.CurrentCell is DataGridViewCheckBoxCell)
            {
                dgvConfig.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        
        /// <summary>
        /// 数据绑定完成事件，确保状态颜色正确应用
        /// </summary>
        private void DgvConfig_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // 延迟执行颜色设置，确保数据绑定完全完成
            this.BeginInvoke(new Action(() =>
            {
                SetupStatusColumnColors();
            }));
        }
    }
}