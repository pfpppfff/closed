using RecipeManager.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecipeManager
{
    public partial class Frm_EditPumpModelForm : Form
    {
        public PumpModel PumpModel { get; private set; }

        public Frm_EditPumpModelForm(PumpModel pumpModel)
        {
            InitializeComponent();

            PumpModel = pumpModel;
            txtModelName.Text = PumpModel.ModelName;
            SetupDataGridView();
            // 初始化数据
            for (int i = 0; i < 15; i++)
            {
                PumpModel.PowerEfficiencyList.Add(new PowerEfficiency());
            }

            dataGridViewPowerEfficiency.DataSource = PumpModel.PowerEfficiencyList;
        }

        private void SetupDataGridView()
        {
            // 清空现有列
            dataGridViewPowerEfficiency.Columns.Clear();

         

            // 添加
            DataGridViewTextBoxColumn powerColumn = new DataGridViewTextBoxColumn();
            powerColumn.HeaderText = "输入功率(kW)";
            powerColumn.Name = "Power"; // 绑定到 ModelName 属性
            powerColumn.DataPropertyName = "Power";
            powerColumn.Width = 100;
            dataGridViewPowerEfficiency.Columns.Add(powerColumn);

            DataGridViewTextBoxColumn effColumn = new DataGridViewTextBoxColumn();
            effColumn.HeaderText = "电机效率(%)";
            effColumn.Name = "Efficiency"; // 绑定到 ModelName 属性
            effColumn.DataPropertyName = "Efficiency";
            effColumn.Width =100;
            dataGridViewPowerEfficiency.Columns.Add(effColumn);

            dataGridViewPowerEfficiency.AllowUserToResizeRows = false;
            dataGridViewPowerEfficiency.RowHeadersVisible = false;
            dataGridViewPowerEfficiency.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (DataGridViewColumn column in dataGridViewPowerEfficiency.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            }      
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(txtModelName.Text!="" && txtModelName.Text !=null)
            {

                PumpModel.ModelName = txtModelName.Text;
                this.DialogResult = DialogResult.OK; // 确认保存
                this.Close();
            }
            else
            {
                MessageBox.Show("泵编号输入为空值！");
            }

        }

        private void EditPumpModelForm_Load(object sender, EventArgs e)
        {

        }
    }
}
