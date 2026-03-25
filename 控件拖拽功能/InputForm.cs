using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 控件拖拽功能
{
    public partial class InputForm : Form
    {
        public string ButtonName { get; private set; }
        public string ButtonClickContent { get; private set; }
        public int ButtonWidth { get; private set; }
        public int ButtonHeight { get; private set; }
        public InputForm()
        {
            InitializeComponent();
        }
        private void buttonSave_Click(object sender, EventArgs e)
        {
            // 获取用户输入的值
            ButtonName = textBoxButtonName.Text.Trim();
            ButtonClickContent = textBoxButtonClickContent.Text.Trim();
            ButtonWidth = Convert.ToInt16(txtBtnWidth.Text.Trim());
            ButtonHeight = Convert.ToInt16(txtBtnHight.Text.Trim());
            // 关闭窗体
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void InputForm_Load(object sender, EventArgs e)
        {

        }
    }
}
