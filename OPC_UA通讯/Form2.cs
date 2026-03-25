using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OPC_UA通讯
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            // 初始化数据表
            InitializeDataTable();

            // 绑定数据源
           dataGridView1.DataSource = _dataTable;
        }
        private DataTable _dataTable;
        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void InitializeDataTable()
        {
            _dataTable = new DataTable();
            _dataTable.Columns.Add("Timestamp", typeof(DateTime));

            // 添加Float数据列
            for (int i = 0; i < 10; i++)
            {
                _dataTable.Columns.Add($"Float{i + 1}", typeof(float));
            }

            // 添加一行初始数据
            AddNewRow();
        }
        private void AddNewRow()
        {
            DataRow newRow = _dataTable.NewRow();
            newRow["Timestamp"] = DateTime.Now;

            // 初始化所有Float值为0
            for (int i = 0; i < 10; i++)
            {
                newRow[$"Float{i + 1}"] = 0.0f;
            }

            _dataTable.Rows.Add(newRow);

            // 保持最多显示100行，防止内存占用过大
            if (_dataTable.Rows.Count > 100)
            {
                _dataTable.Rows.RemoveAt(0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddNewRow();
        }
    }
}
