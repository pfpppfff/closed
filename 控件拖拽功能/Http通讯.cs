using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 控件拖拽功能
{
    public partial class Http通讯 : Form
    {
        public Http通讯()
        {
            InitializeComponent();
        }

        private void Http通讯_Load(object sender, EventArgs e)
        {

        }

        private async void  button1_Click(object sender, EventArgs e)
        {
            string result = await ApiHelper.GetHttp();
            // 将 JSON 字符串解析为 JObject
            JObject jsonObject = JObject.Parse(result);

            // 获取单个字段值
            string title = (string)jsonObject["title"];
            string body = (string)jsonObject["body"];
            int userId = (int)jsonObject["userId"];
        }

        private async void button2_Click(object sender, EventArgs e)
        {
           await ApiHelper.PostHttp();
        }
    }
}
