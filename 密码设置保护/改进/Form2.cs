using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 密码设置保护.改进
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            InitializeActivation();
        }
        private static ActivationManager activationManager;
        private static System.Timers.Timer updateTimer;

        List<string> listBoxActivationInfo = new List<string>();
        private void InitializeActivation()
        {
            // 初始化激活管理器
            activationManager = new ActivationManager();

            // 更新界面显示
            UpdateActivationDisplay();

            // 设置定时器，每分钟更新一次
            updateTimer = new System.Timers.Timer(5000);
            updateTimer.Elapsed += UpdateTimer_Elapsed;
            updateTimer.Start();
        }
        private void UpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            activationManager.UpdateRuntime();

            // 跨线程更新UI
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(UpdateActivationDisplay));
            }
            else
            {
                UpdateActivationDisplay();
            }
        }
        private void UpdateActivationDisplay()
        {
            listBoxActivationInfo.Clear();
            listBoxActivationInfo.Add("=== 软件激活信息 ===");
            listBoxActivationInfo.Add($"首次安装时间:");
            listBoxActivationInfo.Add($"{activationManager._activationInfo.FirstInstallTime:yyyy-MM-dd HH:mm:ss}");
            listBoxActivationInfo.Add($"累计运行时间:");
            listBoxActivationInfo.Add($"{activationManager.GetFormattedRuntime()}");
            listBoxActivationInfo.Add($"激活状态: {(activationManager.IsActivated() ? "已激活" : "未激活")}");
            listBoxActivationInfo.Add($"剩余时间: ");
            listBoxActivationInfo.Add($"{activationManager.GetRemainingTrialTime()}");
            //listBoxActivationInfo.Items.Add($"硬件指纹: {activationManager._activationInfo.HardwareFingerprint.Substring(0, 16)}...");
            //listBoxActivationInfo.Items.Add("");
            //listBoxActivationInfo.Items.Add("永久激活密码: PERM2024ACTIVE");
            //listBoxActivationInfo.Items.Add($"临时激活码示例: {GetTempActivationCode()}");
            common.listActionIfo = listBoxActivationInfo;
            if (activationManager != null)
            {
                if (!activationManager.IsActivated())
                {
                    //textBox1.Visible = true;
                    textBox1.Text = "软件未激活！";
                    textBox1.ForeColor = Color.Red;
                    common.unActionIfo = true;
                }
                else
                {
                    textBox1.Text = "已经激活！";
                    common.unActionIfo = false;
                    textBox1.ForeColor = Color.Green;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Frm_UserManage2 frm_UserManage = new Frm_UserManage2();
            frm_UserManage.unLockMesToMainEvent += unLockMesFromChild;
            frm_UserManage.ShowDialog();
            frm_UserManage.unLockMesToMainEvent -= unLockMesFromChild;
        }
        private void unLockMesFromChild(string strCode, bool unlock, byte[] generatecode)
        {

            BtnActivate(strCode, generatecode);
        }
        private void BtnActivate(string txtActivationCode, byte[] generatecode)
        {
            this.Invoke((MethodInvoker)delegate {
                string inputcode = txtActivationCode;
                if (string.IsNullOrEmpty(inputcode))
                {
                    MessageBox.Show("请输入激活码！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (activationManager.ActivateWithPassword(inputcode, generatecode))
                {
                    MessageBox.Show("激活成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateActivationDisplay();
                }
                else
                {
                    MessageBox.Show("激活码无效！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ActivationInfo activationInfo = new ActivationInfo();
            activationInfo.TotalRunDays = Convert.ToInt16(txtTotalRundays.Text);
            activationInfo.SetDays = Convert.ToInt16(textBox2.Text);
            activationInfo.FirstInstallTime = FirstInstallTime.Value;
            activationInfo.LastStartTime = LastStartTime.Value;
            activationInfo.IsPermanentActivated = false;
            activationInfo.HardwareFingerprint = activationManager.GenerateHardwareFingerprint();
            activationManager.SetActivationIfo(activationInfo);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            updateTimer.Stop();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            activationManager.CreateNewActivationInfo();
        }
    }
}
