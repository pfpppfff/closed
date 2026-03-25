using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;

namespace 串口通讯
{
    public partial class CommConfigForm : Form
    {
        // 保存配置信息
        public string[] PortNames { get; private set; }
        public bool[] IsCommunicating { get; private set; }
        
        // 定义委托用于传递控制命令
        public delegate void ControlCommandHandler(int instrumentIndex, bool start, string portName);
        public event ControlCommandHandler ControlCommand;
        
        // 定义TCP控制命令委托
        public delegate void TcpControlCommandHandler(int instrumentIndex, bool start, string ipAddress, int port);
        public event TcpControlCommandHandler TcpControlCommand;

        public CommConfigForm()
        {
            InitializeComponent();
            
            // 初始化配置数组 (6个仪表: 3个串口 + 3个TCP)
            PortNames = new string[6];
            IsCommunicating = new bool[6];
        }

        private void CommConfigForm_Load(object sender, EventArgs e)
        {
            // 获取可用的串口列表并填充到下拉框
            string[] ports = SerialPort.GetPortNames();

            cmbPort1.Items.Clear();
            cmbPort2.Items.Clear();
            cmbPort3.Items.Clear();

            foreach (string port in ports)
            {
                cmbPort1.Items.Add(port);
                cmbPort2.Items.Add(port);
                cmbPort3.Items.Add(port);
            }

            // 如果有串口，设置默认选择
            if (ports.Length > 0)
            {
                cmbPort1.SelectedIndex = 0;
                if (ports.Length > 1)
                    cmbPort2.SelectedIndex = Math.Min(1, ports.Length - 1);
                if (ports.Length > 2)
                    cmbPort3.SelectedIndex = Math.Min(2, ports.Length - 1);
            }
        }

        // 仪表1开始通讯
        private void btnStart1_Click(object sender, EventArgs e)
        {
            string selectedPort = cmbPort1.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedPort))
            {
                MessageBox.Show("请选择仪表1的串口！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PortNames[0] = selectedPort;
            IsCommunicating[0] = true;
            btnStart1.Enabled = false;
            btnStop1.Enabled = true;
            lblStatus1.Text = "通讯中";
            lblStatus1.ForeColor = Color.Green;
            
            // 发送控制命令到主窗体
            ControlCommand?.Invoke(0, true, selectedPort);
        }

        // 仪表1停止通讯
        private void btnStop1_Click(object sender, EventArgs e)
        {
            IsCommunicating[0] = false;
            btnStart1.Enabled = true;
            btnStop1.Enabled = false;
            lblStatus1.Text = "已停止";
            lblStatus1.ForeColor = Color.Red;
            
            // 发送控制命令到主窗体
            ControlCommand?.Invoke(0, false, PortNames[0]);
        }

        // 仪表2开始通讯
        private void btnStart2_Click(object sender, EventArgs e)
        {
            string selectedPort = cmbPort2.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedPort))
            {
                MessageBox.Show("请选择仪表2的串口！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PortNames[1] = selectedPort;
            IsCommunicating[1] = true;
            btnStart2.Enabled = false;
            btnStop2.Enabled = true;
            lblStatus2.Text = "通讯中";
            lblStatus2.ForeColor = Color.Green;
            
            // 发送控制命令到主窗体
            ControlCommand?.Invoke(1, true, selectedPort);
        }

        // 仪表2停止通讯
        private void btnStop2_Click(object sender, EventArgs e)
        {
            IsCommunicating[1] = false;
            btnStart2.Enabled = true;
            btnStop2.Enabled = false;
            lblStatus2.Text = "已停止";
            lblStatus2.ForeColor = Color.Red;
            
            // 发送控制命令到主窗体
            ControlCommand?.Invoke(1, false, PortNames[1]);
        }

        // 仪表3开始通讯
        private void btnStart3_Click(object sender, EventArgs e)
        {
            string selectedPort = cmbPort3.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedPort))
            {
                MessageBox.Show("请选择仪表3的串口！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PortNames[2] = selectedPort;
            IsCommunicating[2] = true;
            btnStart3.Enabled = false;
            btnStop3.Enabled = true;
            lblStatus3.Text = "通讯中";
            lblStatus3.ForeColor = Color.Green;
            
            // 发送控制命令到主窗体
            ControlCommand?.Invoke(2, true, selectedPort);
        }

        // 仪表3停止通讯
        private void btnStop3_Click(object sender, EventArgs e)
        {
            IsCommunicating[2] = false;
            btnStart3.Enabled = true;
            btnStop3.Enabled = false;
            lblStatus3.Text = "已停止";
            lblStatus3.ForeColor = Color.Red;
            
            // 发送控制命令到主窗体
            ControlCommand?.Invoke(2, false, PortNames[2]);
        }

        // 关闭按钮
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide(); // 隐藏窗体而不是关闭
        }

        // 窗体关闭事件
        private void CommConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 只隐藏窗体，不停止通讯
            e.Cancel = true;
            this.Hide();
        }
        
        // 更新仪表状态显示
        public void UpdateInstrumentStatus(int instrumentIndex, bool isConnected, string message)
        {
            switch (instrumentIndex)
            {
                case 0:
                    lblStatus1.Text = isConnected ? message : "连接失败";
                    lblStatus1.ForeColor = isConnected ? Color.Green : Color.Red;
                    if (!isConnected)
                    {
                        IsCommunicating[0] = false;
                        btnStart1.Enabled = true;
                        btnStop1.Enabled = false;
                    }
                    break;
                case 1:
                    lblStatus2.Text = isConnected ? message : "连接失败";
                    lblStatus2.ForeColor = isConnected ? Color.Green : Color.Red;
                    if (!isConnected)
                    {
                        IsCommunicating[1] = false;
                        btnStart2.Enabled = true;
                        btnStop2.Enabled = false;
                    }
                    break;
                case 2:
                    lblStatus3.Text = isConnected ? message : "连接失败";
                    lblStatus3.ForeColor = isConnected ? Color.Green : Color.Red;
                    if (!isConnected)
                    {
                        IsCommunicating[2] = false;
                        btnStart3.Enabled = true;
                        btnStop3.Enabled = false;
                    }
                    break;
                case 3:
                    lblStatus4.Text = isConnected ? message : "连接失败";
                    lblStatus4.ForeColor = isConnected ? Color.Green : Color.Red;
                    if (!isConnected)
                    {
                        IsCommunicating[3] = false;
                        btnStart4.Enabled = true;
                        btnStop4.Enabled = false;
                    }
                    break;
                case 4:
                    lblStatus5.Text = isConnected ? message : "连接失败";
                    lblStatus5.ForeColor = isConnected ? Color.Green : Color.Red;
                    if (!isConnected)
                    {
                        IsCommunicating[4] = false;
                        btnStart5.Enabled = true;
                        btnStop5.Enabled = false;
                    }
                    break;
            }
        }
        
        // 更新所有仪表的UI状态
        public void UpdateAllInstrumentUI()
        {
            for (int i = 0; i < 5; i++)
            {
                switch (i)
                {
                    case 0:
                        btnStart1.Enabled = !IsCommunicating[i];
                        btnStop1.Enabled = IsCommunicating[i];
                        lblStatus1.Text = IsCommunicating[i] ? "通讯中" : "已停止";
                        lblStatus1.ForeColor = IsCommunicating[i] ? Color.Green : Color.Red;
                        break;
                    case 1:
                        btnStart2.Enabled = !IsCommunicating[i];
                        btnStop2.Enabled = IsCommunicating[i];
                        lblStatus2.Text = IsCommunicating[i] ? "通讯中" : "已停止";
                        lblStatus2.ForeColor = IsCommunicating[i] ? Color.Green : Color.Red;
                        break;
                    case 2:
                        btnStart3.Enabled = !IsCommunicating[i];
                        btnStop3.Enabled = IsCommunicating[i];
                        lblStatus3.Text = IsCommunicating[i] ? "通讯中" : "已停止";
                        lblStatus3.ForeColor = IsCommunicating[i] ? Color.Green : Color.Red;
                        break;
                    case 3:
                        btnStart4.Enabled = !IsCommunicating[i];
                        btnStop4.Enabled = IsCommunicating[i];
                        lblStatus4.Text = IsCommunicating[i] ? "通讯中" : "已停止";
                        lblStatus4.ForeColor = IsCommunicating[i] ? Color.Green : Color.Red;
                        break;
                    case 4:
                        btnStart5.Enabled = !IsCommunicating[i];
                        btnStop5.Enabled = IsCommunicating[i];
                        lblStatus5.Text = IsCommunicating[i] ? "通讯中" : "已停止";
                        lblStatus5.ForeColor = IsCommunicating[i] ? Color.Green : Color.Red;
                        break;
                }
            }
        }
        
        // 仪表4开始通讯 (TCP自由口)
        private void btnStart4_Click(object sender, EventArgs e)
        {
            string ip = txtIp4.Text.Trim();
            string portStr = txtPort4.Text.Trim();
            
            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(portStr))
            {
                MessageBox.Show("请输入仪表4的IP地址和端口！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (!int.TryParse(portStr, out int port) || port < 1 || port > 65535)
            {
                MessageBox.Show("请输入有效的端口号(1-65535)！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            IsCommunicating[3] = true;
            btnStart4.Enabled = false;
            btnStop4.Enabled = true;
            lblStatus4.Text = "通讯中";
            lblStatus4.ForeColor = Color.Green;
            
            // 发送TCP控制命令到主窗体
            TcpControlCommand?.Invoke(3, true, ip, port);
        }

        // 仪表4停止通讯
        private void btnStop4_Click(object sender, EventArgs e)
        {
            IsCommunicating[3] = false;
            btnStart4.Enabled = true;
            btnStop4.Enabled = false;
            lblStatus4.Text = "已停止";
            lblStatus4.ForeColor = Color.Red;
            
            TcpControlCommand?.Invoke(3, false, "", 0);
        }
        
        // 仪表5开始通讯 (Modbus TCP)
        private void btnStart5_Click(object sender, EventArgs e)
        {
            string ip = txtIp5.Text.Trim();
            string portStr = txtPort5.Text.Trim();
            
            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(portStr))
            {
                MessageBox.Show("请输入仪表5的IP地址和端口！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (!int.TryParse(portStr, out int port) || port < 1 || port > 65535)
            {
                MessageBox.Show("请输入有效的端口号(1-65535)！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            IsCommunicating[4] = true;
            btnStart5.Enabled = false;
            btnStop5.Enabled = true;
            lblStatus5.Text = "通讯中";
            lblStatus5.ForeColor = Color.Green;
            
            // 发送TCP控制命令到主窗体
            TcpControlCommand?.Invoke(4, true, ip, port);
        }

        // 仪表5停止通讯
        private void btnStop5_Click(object sender, EventArgs e)
        {
            IsCommunicating[4] = false;
            btnStart5.Enabled = true;
            btnStop5.Enabled = false;
            lblStatus5.Text = "已停止";
            lblStatus5.ForeColor = Color.Red;
            
            TcpControlCommand?.Invoke(4, false, "", 0);
        }
        
        // 仪表6开始通讯 (TCP字符串通讯)
        private void btnStart6_Click(object sender, EventArgs e)
        {
            string ip = txtIp6.Text.Trim();
            string portStr = txtPort6.Text.Trim();
            
            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(portStr))
            {
                MessageBox.Show("请输入仪表6的IP地址和端口！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (!int.TryParse(portStr, out int port) || port < 1 || port > 65535)
            {
                MessageBox.Show("请输入有效的端口号(1-65535)！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            IsCommunicating[5] = true;
            btnStart6.Enabled = false;
            btnStop6.Enabled = true;
            lblStatus6.Text = "通讯中";
            lblStatus6.ForeColor = Color.Green;
            
            // 发送TCP控制命令到主窗体
            TcpControlCommand?.Invoke(5, true, ip, port);
        }

        // 仪表6停止通讯
        private void btnStop6_Click(object sender, EventArgs e)
        {
            IsCommunicating[5] = false;
            btnStart6.Enabled = true;
            btnStop6.Enabled = false;
            lblStatus6.Text = "已停止";
            lblStatus6.ForeColor = Color.Red;
            
            TcpControlCommand?.Invoke(5, false, "", 0);
        }
        
        // 更新仪表状态消息（包含接收数据长度）
        public void UpdateStatusMessage(int instrumentIndex, int receivedLength)
        {
            string message = $"通讯中，接收数据长度：{receivedLength} 字节";
            switch (instrumentIndex)
            {
                case 0:
                    if (IsCommunicating[0])
                    {
                        lblStatus1.Text = message;
                        lblStatus1.ForeColor = Color.Green;
                    }
                    break;
                case 1:
                    if (IsCommunicating[1])
                    {
                        lblStatus2.Text = message;
                        lblStatus2.ForeColor = Color.Green;
                    }
                    break;
                case 2:
                    if (IsCommunicating[2])
                    {
                        lblStatus3.Text = message;
                        lblStatus3.ForeColor = Color.Green;
                    }
                    break;
                case 3:
                    if (IsCommunicating[3])
                    {
                        lblStatus4.Text = message;
                        lblStatus4.ForeColor = Color.Green;
                    }
                    break;
                case 4:
                    if (IsCommunicating[4])
                    {
                        lblStatus5.Text = message;
                        lblStatus5.ForeColor = Color.Green;
                    }
                    break;
                case 5:
                    if (IsCommunicating[5])
                    {
                        lblStatus6.Text = message;
                        lblStatus6.ForeColor = Color.Green;
                    }
                    break;
            }
        }
    }
}