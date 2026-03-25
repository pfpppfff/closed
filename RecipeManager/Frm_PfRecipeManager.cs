using RecipeManager.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace RecipeManager
{
    public partial class Frm_PfRecipeManager : Form
    {
        private List<PumpModel> pumpModels;
       
        public Frm_PfRecipeManager()
        {
            InitializeComponent();
            pumpModels = new List<PumpModel>();
            
            SetupDataGridView();
            LoadPumpModels();


        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ///*InitLoadPumpModels*/(); // 加载已有泵型号
        }

        private void SetupDataGridView()
        {
            // 清空现有列
            dataGridViewPumpModels.Columns.Clear();
            dataGridViewPumpModels.AutoGenerateColumns = false;
            // 添加选择框列
            DataGridViewCheckBoxColumn selectColumn = new DataGridViewCheckBoxColumn();
            selectColumn.HeaderText = "选择";
            selectColumn.Width = 50;
            selectColumn.Name = "IsSelected";
            selectColumn.DataPropertyName = "IsSelected";
            dataGridViewPumpModels.Columns.Add(selectColumn);

            // 添加泵编号列
            DataGridViewTextBoxColumn modelNameColumn = new DataGridViewTextBoxColumn();
            modelNameColumn.HeaderText = "泵编号";
            modelNameColumn.Name = "ModelName"; // 绑定到 ModelName 属性
            modelNameColumn.DataPropertyName= "ModelName";
            modelNameColumn.Width = 200;
            dataGridViewPumpModels.Columns.Add(modelNameColumn);

            // 添加编辑按钮列
            DataGridViewButtonColumn editButtonColumn = new DataGridViewButtonColumn();
            editButtonColumn.HeaderText = "编辑";
            editButtonColumn.Text = "编辑";
            editButtonColumn.Name= "编辑";
            editButtonColumn.Width = 80;
            editButtonColumn.UseColumnTextForButtonValue = true;
            dataGridViewPumpModels.Columns.Add(editButtonColumn);

            // 添加删除按钮列
            DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
            deleteButtonColumn.HeaderText = "删除";
            deleteButtonColumn.Name = "删除";
            deleteButtonColumn.Text = "删除";
            deleteButtonColumn.Width = 80;
            deleteButtonColumn.UseColumnTextForButtonValue = true;
            dataGridViewPumpModels.Columns.Add(deleteButtonColumn);

            dataGridViewPumpModels.AllowUserToAddRows = false;
            dataGridViewPumpModels.RowHeadersVisible = false;
            dataGridViewPumpModels.AllowUserToResizeRows = false;
            dataGridViewPumpModels.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (DataGridViewColumn column in dataGridViewPumpModels.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
               
            }

            // 添加事件处理程序
            //dataGridViewPumpModels.CellClick += DataGridViewPumpModels_CellClick;
            dataGridViewPumpModels.CellValueChanged += DataGridViewPumpModels_CellValueChanged; // 处理选择框值变化事件
            dataGridViewPumpModels.CurrentCellDirtyStateChanged += DataGridViewPumpModels_CurrentCellDirtyStateChanged; // 处理当前单元格状态
        }

        
        //private void InitLoadPumpModels()
        //{
        //    //TODO: 从XML文件加载数据
        //    pumpModels = LoadFromXml();
        //    dataGridViewPumpModels.DataSource =new BindingList<PumpModel>(pumpModels);
        //    //加载选择状态到 DataGridView
        //    var selectionStates = LoadSelectionStates();
        //    for (int i = 0; i < selectionStates.Count; i++)
        //    {
        //        if (selectionStates[i].ModelName != null)
        //        {
        //            dataGridViewPumpModels.Rows[i].Cells[0].Value = selectionStates[i].IsSelected; // 恢复选择状态
        //            int fsffs = 1;
        //        }
        //    }

        //    int fsfsfs = 1;
        //}
        private void LoadPumpModels()
        {
            // TODO: 从XML文件加载数据
            pumpModels = LoadFromXml();         
            dataGridViewPumpModels.DataSource = new BindingList<PumpModel>(pumpModels);

            int fsfsfs = 1;
        }
       
        // 当选择框状态改变时，确保只有一个被选中
        private void DataGridViewPumpModels_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var bll = dataGridViewPumpModels.Rows[e.RowIndex].Cells[0].Value;
            // 确保是选择框列在触发
            if (e.ColumnIndex == 0 && e.RowIndex >= 0 && bll!=null)
            {
                var currentCheckState = Convert.ToBoolean(dataGridViewPumpModels.Rows[e.RowIndex].Cells[0].Value);

                if (currentCheckState)
                {
                    // 遍历其他行，将其他行的选择框取消勾选
                    foreach (DataGridViewRow row in dataGridViewPumpModels.Rows)
                    {
                        if (row.Index != e.RowIndex)
                        {
                            row.Cells[0].Value = false;
                        }
                    }
                }
            }
        }

        // 处理当前单元格的状态，确保单元格变更立即触发
        private void DataGridViewPumpModels_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridViewPumpModels.CurrentCell is DataGridViewCheckBoxCell)
            {
                dataGridViewPumpModels.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var editForm = new Frm_EditPumpModelForm(new PumpModel());
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                pumpModels.Add(editForm.PumpModel);
                SaveToXml(); // 保存到XML
                LoadPumpModels(); // 刷新显示
            }
        }
      

        private void dataGridViewPumpModels_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 检查行索引是否有效
            if (e.RowIndex < 0) return;

            // 检查是否点击了编辑按钮
            if (e.ColumnIndex == 2) // 编辑按钮列的索引
            {
                if (pumpModels != null && pumpModels.Count > e.RowIndex)
                {
                    var selectedPumpModel = pumpModels[e.RowIndex];
                    var editForm = new Frm_EditPumpModelForm(selectedPumpModel);
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        // 更新数据
                        pumpModels[e.RowIndex] = editForm.PumpModel;
                        SaveToXml();
                        LoadPumpModels();
                    }
                }
            }

            // 检查是否点击了删除按钮
            if (e.ColumnIndex == 3) // 删除按钮列的索引
            {
                var result = MessageBox.Show("确定要删除此泵编号吗？", "确认", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    if (pumpModels != null && pumpModels.Count > e.RowIndex)
                    {
                        pumpModels.RemoveAt(e.RowIndex); // 删除对应条目
                        SaveToXml(); // 保存到XML
                        LoadPumpModels(); // 重新加载数据
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 获取选中条目的功率和效率数据组
            var selectedData = new List<object>(); // 用于存储选中的数据
            for (int i = 0; i < dataGridViewPumpModels.Rows.Count; i++)
            {
                // 检查当前行的选择框是否被选中，并确保 Cells[0] 有值
                if (dataGridViewPumpModels.Rows[i].Cells[0].Value is bool isSelected && isSelected)
                {
                    if (pumpModels.Count > i)
                    {
                        var model = pumpModels[i];
                        // 选择有效的功率和效率数据，排除效率为 0 的组
                        var efficiencies = model.PowerEfficiencyList
                            .Where(x => x.Efficiency > 0) // 过滤掉效率为0的组
                            .Select(x => new
                            {
                                model.ModelName,
                                Power = x.Power,
                                Efficiency = x.Efficiency
                            });

                        selectedData.AddRange(efficiencies);
                    }
                }
            }
            // 遍历 DataGridView 行
            foreach (DataGridViewRow row in dataGridViewPumpModels.Rows)
            {
                // 检查选择框是否被选中
                var isSelected = Convert.ToBoolean(row.Cells[0].Value); // 第一列为选择框
                var model = (PumpModel)row.DataBoundItem;

                if (isSelected)
                {
                    // 移除效率为0的组
                    //model.Efficiencies.RemoveAll(e => e.Efficiency == 0);
                }
            }

            SaveToXml(); // 保存数据到XML
            // 处理选中的数据（例如，保存到文件、显示等）
            if (selectedData.Any())
            {
                // 在此处可以进行保存操作或其它逻辑
                MessageBox.Show("已获取到选中泵编号的功率和效率数据组。");
            }
            else
            {
                MessageBox.Show("没有选中条目或所有效率均为0。");
            }

        }


        private void SaveToXml()
        {
            var serializer = new XmlSerializer(typeof(List<PumpModel>));
            using (var stream = new FileStream("pumpModels.xml", FileMode.Create))
            {
                serializer.Serialize(stream, pumpModels);
            }

            //SaveSelectionStates();
        }

        private List<PumpModel> LoadFromXml()
        {
            if (File.Exists("pumpModels.xml"))
            {
                var serializer = new XmlSerializer(typeof(List<PumpModel>));
                using (var stream = new FileStream("pumpModels.xml", FileMode.Open))
                {
                    return (List<PumpModel>)serializer.Deserialize(stream);
                }
            }
            return new List<PumpModel>(); // 返回空列表
        }
        //private void SaveSelectionStates()
        //{
        //    var selectionStates = new List<PumpSelectionState>();

        //    if(dataGridViewPumpModels.Rows.Count>1)
        //    {
        //        for (int i = 0; i < dataGridViewPumpModels.Rows.Count; i++)
        //        {
        //            var modelName = dataGridViewPumpModels.Rows[i].Cells[1].Value.ToString(); // 假设泵编号在第二列
        //            var isSelected = Convert.ToBoolean(dataGridViewPumpModels.Rows[i].Cells[0].Value); // 假设选择框在第一列
        //            selectionStates.Add(new PumpSelectionState { ModelName = modelName, IsSelected = isSelected });
        //        }

        //        var serializer = new XmlSerializer(typeof(List<PumpSelectionState>));
        //        using (var stream = new FileStream("pumpSelectionStates.xml", FileMode.Create))
        //        {
        //            serializer.Serialize(stream, selectionStates);
        //        }
        //    }
            
        //}

        //private List<PumpSelectionState> LoadSelectionStates()
        //{
        //    if (!File.Exists("pumpSelectionStates.xml"))
        //        return new List<PumpSelectionState>();

        //    var serializer = new XmlSerializer(typeof(List<PumpSelectionState>));
        //    using (var stream = new FileStream("pumpSelectionStates.xml", FileMode.Open))
        //    {
        //        return (List<PumpSelectionState>)serializer.Deserialize(stream);
        //    }
        //}

     
    }
}
