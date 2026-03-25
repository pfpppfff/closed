using System;
using System.Windows.Forms;

namespace Frm_NpshConfig
{
    /// <summary>
    /// 示例：其他页面如何使用选中的试验数据
    /// </summary>
    public partial class OtherForm : Form
    {
        public OtherForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 示例：在其他页面中获取选中的试验数据
        /// </summary>
        private void UseSelectedTestData()
        {
            // 方法1：直接访问静态属性
            var selectedData = ConfigForm.SelectedTestData;
            
            // 方法2：使用静态方法获取
            var selectedData2 = ConfigForm.GetSelectedTestData();
            
            // 方法3：先检查是否有选中数据
            if (ConfigForm.HasSelectedTestData())
            {
                var data = ConfigForm.GetSelectedTestData();
                
                // 使用选中的数据
                string info = $"正在处理试验数据：\n" +
                             $"序号：{data.TestNumber}\n" +
                             $"取样流量：{data.Flow:F2} m³/h\n" +
                             $"状态：{data.Status}\n" +
                             $"临界扬程：{data.Head:F2} m\n" +
                             $"备注：{data.Remark}";
                
                MessageBox.Show(info, "使用选中的试验数据", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("没有选中任何试验数据进行重测！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 示例：在按钮点击事件中使用选中数据
        /// </summary>
        private void btnUseSelectedData_Click(object sender, EventArgs e)
        {
            UseSelectedTestData();
        }

        /// <summary>
        /// 示例：将选中的数据用于计算或处理
        /// </summary>
        private void ProcessSelectedData()
        {
            if (!ConfigForm.HasSelectedTestData())
            {
                MessageBox.Show("请先在配置页面选择需要重测的试验数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var data = ConfigForm.GetSelectedTestData();
            
            // 示例：基于选中数据进行计算
            double calculatedValue = data.Flow * data.Head * 0.001; // 示例计算
            
            string result = $"基于选中数据的计算结果：\n" +
                           $"试验编号：{data.TestNumber}\n" +
                           $"计算值：{calculatedValue:F3}";
            
            MessageBox.Show(result, "计算结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // OtherForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.Name = "OtherForm";
            this.Text = "其他页面示例";
            this.ResumeLayout(false);
        }
    }
}