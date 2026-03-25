using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 密码保护
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeActivation();
        }
        private static ActivationManager activationManager;
        private static System.Timers.Timer updateTimer;
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
            listBoxActivationInfo.Items.Clear();
            listBoxActivationInfo.Items.Add("=== 软件激活信息 ===");
            listBoxActivationInfo.Items.Add($"首次安装时间: {activationManager._activationInfo.FirstInstallTime:yyyy-MM-dd HH:mm:ss}");
            listBoxActivationInfo.Items.Add($"累计运行时间: {activationManager.GetFormattedRuntime()}");
            listBoxActivationInfo.Items.Add($"激活状态: {(activationManager.IsActivated() ? "已激活" : "未激活")}");
            listBoxActivationInfo.Items.Add($"剩余时间: {activationManager.GetRemainingTrialTime()}");
            //listBoxActivationInfo.Items.Add($"硬件指纹: {activationManager._activationInfo.HardwareFingerprint.Substring(0, 16)}...");
            //listBoxActivationInfo.Items.Add("");
            //listBoxActivationInfo.Items.Add("永久激活密码: PERM2024ACTIVE");
            //listBoxActivationInfo.Items.Add($"临时激活码示例: {GetTempActivationCode()}");
        }
        private string GetTempActivationCode()
        {
            // 显示临时激活码示例
            return "TEMP" + DateTime.Now.ToString("MMdd");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBoxActivation = listBoxActivationInfo;
        }
        private void InitializeActivation()
        {
            // 初始化激活管理器
            activationManager = new ActivationManager();

            // 更新界面显示
            UpdateActivationDisplay();

            // 设置定时器，每分钟更新一次
            updateTimer = new System.Timers.Timer(60000);
            updateTimer.Elapsed += UpdateTimer_Elapsed;
            updateTimer.Start();
        }
        public void fsf()
        {
            Console.WriteLine("软件激活系统演示");
            Console.WriteLine("==================");

            // 初始化激活管理器
            activationManager = new ActivationManager();

            // 显示当前激活状态
            Console.WriteLine(activationManager.GetActivationSummary());

            // 检查是否激活
            if (!activationManager.IsActivated())
            {
                Console.WriteLine("\n软件未激活或试用期已结束！");
                Console.WriteLine("请输入激活码:");
                string code = Console.ReadLine();

                if (activationManager.ActivateWithPassword(code))
                {
                    Console.WriteLine("激活成功！");
                }
                else
                {
                    Console.WriteLine("激活码无效！");
                    Console.WriteLine("程序将退出...");
                    return;
                }
            }

            // 设置定时器，每分钟更新一次运行时间
            updateTimer = new System.Timers.Timer(60000); // 60秒
            updateTimer.Elapsed += UpdateTimer_Elapsed;
            updateTimer.Start();

            Console.WriteLine("\n软件正在运行...");
            Console.WriteLine("按任意键显示状态，输入'exit'退出");

            // 主循环
            while (true)
            {
                string input = Console.ReadLine();
                if (input?.ToLower() == "exit")
                {
                    break;
                }

                Console.WriteLine(activationManager.GetActivationSummary());
            }

            // 程序退出前保存数据
            updateTimer?.Stop();
            activationManager.OnApplicationExit();
            Console.WriteLine("程序已退出");
        }
        public void fsf1()
        {
            
            // 初始化激活管理器
            activationManager = new ActivationManager();

            // 显示当前激活状态
            Console.WriteLine(activationManager.GetActivationSummary());

            // 检查是否激活
            if (!activationManager.IsActivated())
            {
                Console.WriteLine("\n软件未激活或试用期已结束！");
                Console.WriteLine("请输入激活码:");
                string code = Console.ReadLine();

                if (activationManager.ActivateWithPassword(code))
                {
                    Console.WriteLine("激活成功！");
                }
                else
                {
                    Console.WriteLine("激活码无效！");
                    Console.WriteLine("程序将退出...");
                    return;
                }
            }

            // 设置定时器，每分钟更新一次运行时间
            updateTimer = new System.Timers.Timer(60000); // 60秒
            updateTimer.Elapsed += UpdateTimer_Elapsed;
            updateTimer.Start();

            Console.WriteLine("\n软件正在运行...");
            Console.WriteLine("按任意键显示状态，输入'exit'退出");

            // 主循环
            while (true)
            {
                string input = Console.ReadLine();
                if (input?.ToLower() == "exit")
                {
                    break;
                }

                Console.WriteLine(activationManager.GetActivationSummary());
            }

            // 程序退出前保存数据
            updateTimer?.Stop();
            activationManager.OnApplicationExit();
            Console.WriteLine("程序已退出");
        }

        //private static void DisplayActivationInfoInListBox(ListBox listBox)
        //{
        //    listBox.Items.Clear();
        //    listBox.Items.Add("=== 软件激活信息 ===");
        //    listBox.Items.Add($"首次安装时间: {activationManager._activationInfo.FirstInstallTime:yyyy-MM-dd HH:mm:ss}");
        //    listBox.Items.Add($"累计运行时间: {activationManager.GetFormattedRuntime()}");
        //    listBox.Items.Add($"激活状态: {(activationManager.IsActivated() ? "已激活" : "未激活")}");
        //    listBox.Items.Add($"剩余时间: {activationManager.GetRemainingTrialTime()}");
        //    listBox.Items.Add($"硬件指纹: {activationManager._activationInfo.HardwareFingerprint.Substring(0, 8)}...");
        //}

        private void BtnActivate_Click(object sender, EventArgs e)
        {
            string code = txtActivationCode.Text.Trim();
            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("请输入激活码！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (activationManager.ActivateWithPassword(code))
            {
                MessageBox.Show("激活成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
              
                txtActivationCode.Clear();
                UpdateActivationDisplay();
            }
            else
            {
                MessageBox.Show("激活码无效！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            updateTimer?.Stop();
            activationManager?.OnApplicationExit();
            base.OnFormClosed(e);
        }
    }
}

