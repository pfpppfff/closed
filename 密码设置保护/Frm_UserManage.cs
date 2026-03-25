using OtpNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 密码设置保护
{
    public partial class Frm_UserManage : Form
    {
        public Frm_UserManage()
        {
            InitializeComponent();
        }

        private void Frm_UserManage_Load(object sender, EventArgs e)
        {
            InitializeActivation();
        }
        public delegate void UnLockMesToMain(string strCode, bool unlock, byte[] generatecode);
        public event UnLockMesToMain unLockMesToMainEvent;
        private static System.Timers.Timer updateTimer;
        private void InitializeActivation()
        {

            updateTimer = new System.Timers.Timer(5000);
            updateTimer.Elapsed += UpdateTimer_Elapsed;
            updateTimer.Start();
        }
        private void UpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {


            // 跨线程更新UI
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => {
                    listBox1.Items.Clear();

                    listBox1.Items.Add(common.listActionIfo[0]);
                    listBox1.Items.Add(common.listActionIfo[1]);
                    listBox1.Items.Add(common.listActionIfo[2]);
                    listBox1.Items.Add(common.listActionIfo[3]);
                    listBox1.Items.Add(common.listActionIfo[4]);
                    listBox1.Items.Add(common.listActionIfo[5]);
                    listBox1.Items.Add(common.listActionIfo[6]);
                    listBox1.Items.Add(common.listActionIfo[7]);

                }));
            }
            else
            {
                listBox1.DataSource = common.listActionIfo;
            }
        }
      
        private void button1_Click(object sender, EventArgs e)
        {
            string userInput = txtCode.Text.Trim();
            
            if (userInput!= null)
            {
                unLockMesToMainEvent?.BeginInvoke(userInput, true, _sharedSecret, null, null);
                //this.Close();
            }
        }

       
        byte[] _sharedSecret;
        Totp totp=null;
        private void BtnGenerateRandomKey_Click(object sender, EventArgs e)
        {
            string deviceId = "zjdpingABC";

            _sharedSecret = KeyGeneration.GenerateRandomKey(20); 
             byte[]  deviceIdSecret = TotpHelper.DeriveSecret(_sharedSecret, deviceId);
            string base32Secret = Base32Encoding.ToString(deviceIdSecret);

            txtRandomKey.Text = base32Secret;

             totp = new Totp(deviceIdSecret);
            string currentCode = totp.ComputeTotp();

            textBox1.Text = currentCode;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string base32Key = txtRandomKey.Text.Trim();

            if (string.IsNullOrEmpty(base32Key))
            {
                MessageBox.Show("请输入厂家提供的密钥！");
                return;
            }

            try
            {
                // 从 Base32 字符串还原密钥
                byte[] secret = Base32Encoding.ToBytes(base32Key);
                var totp = new Totp(secret);

                // 生成当前 TOTP（6位数字）
                string code = totp.ComputeTotp();
                textBox2.Text = code;

                // 自动复制到剪贴板（方便发送）
                Clipboard.SetText(code);
                MessageBox.Show("动态密码已生成并复制到剪贴板！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("密钥格式错误：" + ex.Message);
            }
        }
    }
   
}
