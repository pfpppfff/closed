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

namespace 密码设置保护.改进
{
    public partial class Frm_UserManage2 : Form
    {
        public Frm_UserManage2()
        {
            InitializeComponent();
        }

        private void Frm_UserManage2_Load(object sender, EventArgs e)
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

            if (userInput != null)
            {
                unLockMesToMainEvent?.BeginInvoke(userInput, true, _finalSecret, null, null);
                //this.Close();
            }
        }

        private const string ActivationChars = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ"; // 32 个字符
        private const int ActivationCodeLength = 8; // 8位足够安全
        private static readonly byte[] MasterKey =
    Encoding.UTF8.GetBytes("MySuperSecureMasterKey1234567890");
        byte[] _sharedSecret;
        byte[] _finalSecret;
        Totp totp = null;
        private void BtnGenerateRandomKey_Click(object sender, EventArgs e)
        {
            //string deviceId = "zjdpingABC";

            //_sharedSecret = KeyGeneration.GenerateRandomKey(20);
            //byte[] deviceIdSecret = TotpHelper.DeriveSecret(_sharedSecret, deviceId);
            //string base32Secret = Base32Encoding.ToString(deviceIdSecret);

            //txtRandomKey.Text = base32Secret;

            //totp = new Totp(deviceIdSecret);
            //string currentCode = totp.ComputeTotp();

            //textBox1.Text = currentCode;


            string deviceId = "zjdpingABC";

            // 1. 生成随机原始密钥（20字节）
            byte[] rawSecret = KeyGeneration.GenerateRandomKey(20);

            // 2. 派生最终密钥（带 deviceId）
            byte[] finalSecret = TotpHelper.DeriveSecret(rawSecret, deviceId);

            // 3. 用 MasterKey 加密 rawSecret（注意：加密 rawSecret，不是 finalSecret）
            string shortToken = KeyProtector.ProtectKey(rawSecret, MasterKey);

            // 4. 显示“短码”给用户（Base64 约 44 字符，已比 Base32 短且可复制）
            txtRandomKey.Text = shortToken;

            // 5. 保存 finalSecret 用于后续验证
            _finalSecret = finalSecret;

            // 6. 测试 TOTP
            var totp = new Totp(finalSecret);
            textBox1.Text = totp.ComputeTotp();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //string base32Key = txtRandomKey.Text.Trim();

            //if (string.IsNullOrEmpty(base32Key))
            //{
            //    MessageBox.Show("请输入厂家提供的密钥！");
            //    return;
            //}

            //try
            //{
            //    // 从 Base32 字符串还原密钥
            //    byte[] secret = Base32Encoding.ToBytes(base32Key);
            //    var totp = new Totp(secret);

            //    // 生成当前 TOTP（6位数字）
            //    string code = totp.ComputeTotp();
            //    textBox2.Text = code;

            //    // 自动复制到剪贴板（方便发送）
            //    Clipboard.SetText(code);
            //    MessageBox.Show("动态密码已生成并复制到剪贴板！");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("密钥格式错误：" + ex.Message);
            //}
            string protectedKey = txtRandomKey.Text.Trim();
            if (string.IsNullOrEmpty(protectedKey))
            {
                MessageBox.Show("请输入用户提供的激活码！");
                return;
            }

            try
            {
                // 1. 解密得到 rawSecret
                byte[] rawSecret = KeyProtector.UnprotectKey(protectedKey, MasterKey);

                // 2. 用相同逻辑派生 finalSecret
                string deviceId = "zjdpingABC";
                byte[] finalSecret = TotpHelper.DeriveSecret(rawSecret, deviceId);

                // 3. 生成 TOTP
                var totp = new Totp(finalSecret);
                string code = totp.ComputeTotp();

                textBox2.Text = code;
                Clipboard.SetText(code);
                MessageBox.Show("动态密码已生成！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("无效激活码：" + ex.Message);
            }
        }
    }
}
