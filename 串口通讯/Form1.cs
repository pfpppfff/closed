using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 串口通讯
{
    public partial class Form1 : Form
    {
        // 为每个仪表创建独立的通讯服务实例
        private CommunicationService[] _commServices;
        private bool[] _isConnected;
        private bool[] _isCommunicating;
        private string[] _portNames;
        private string[] _tcpIpAddresses;
        private int[] _tcpPorts;
        private CommConfigForm _configForm;
        
        // 自动重连相关
        private bool[] _autoReconnectEnabled;  // 是否启用自动重连
        private Timer[] _reconnectTimers;       // 重连计时器
        private int[] _reconnectAttempts;       // 重连尝试次数
        private const int MAX_RECONNECT_ATTEMPTS = 5;  // 最大重连次数
        private const int RECONNECT_INTERVAL = 3000;   // 重连间隔(毫秒)

        public Form1()
        {
            InitializeComponent();
            // 初始化通讯服务数组
            _commServices = new CommunicationService[6];
            _isConnected = new bool[6];
            _isCommunicating = new bool[6];
            _portNames = new string[6];
            _tcpIpAddresses = new string[6];
            _tcpPorts = new int[6];
            
            // 初始化自动重连相关
            _autoReconnectEnabled = new bool[6];
            _reconnectTimers = new Timer[6];
            _reconnectAttempts = new int[6];
            
            for (int i = 0; i < 6; i++)
            {
                _commServices[i] = new CommunicationService();
                _isConnected[i] = false;
                _isCommunicating[i] = false;
                _autoReconnectEnabled[i] = false;
                _reconnectAttempts[i] = 0;
                
                // 创建重连计时器
                int index = i;
                _reconnectTimers[i] = new Timer();
                _reconnectTimers[i].Interval = RECONNECT_INTERVAL;
                _reconnectTimers[i].Tick += (s, e) => ReconnectTimer_Tick(index);
            }
        }

        // 打开通讯配置窗体
        private void btnOpenConfig_Click(object sender, EventArgs e)
        {
            if (_configForm == null || _configForm.IsDisposed)
            {
                _configForm = new CommConfigForm();
                _configForm.ControlCommand += ConfigForm_ControlCommand;
                _configForm.TcpControlCommand += ConfigForm_TcpControlCommand;
            }
            
            // 显示窗体并恢复之前的UI状态
            _configForm.Show();
            _configForm.UpdateAllInstrumentUI();
            _configForm.BringToFront();
        }

        // 处理配置窗体传来的控制命令
        private void ConfigForm_ControlCommand(int instrumentIndex, bool start, string portName)
        {
            if (start)
            {
                // 开始通讯
                StartCommunication(instrumentIndex, portName);
            }
            else
            {
                // 停止通讯
                StopCommunication(instrumentIndex);
            }
        }
        
        // 处理TCP控制命令
        private void ConfigForm_TcpControlCommand(int instrumentIndex, bool start, string ipAddress, int port)
        {
            if (start)
            {
                if (instrumentIndex == 4)
                {
                    // 开始Modbus TCP通讯
                    StartModbusTcpCommunication(instrumentIndex, ipAddress, port);
                }
                else if (instrumentIndex == 5)
                {
                    // 开始TCP字符串通讯 (仪表6)
                    StartTcpStringCommunication(instrumentIndex, ipAddress, port);
                }
                else
                {
                    // 开始TCP自由口通讯
                    StartTcpCommunication(instrumentIndex, ipAddress, port);
                }
            }
            else
            {
                if (instrumentIndex == 4)
                {
                    // 停止Modbus TCP通讯
                    StopModbusTcpCommunication(instrumentIndex);
                }
                else if (instrumentIndex == 5)
                {
                    // 停止TCP字符串通讯 (仪表6)
                    StopTcpStringCommunication(instrumentIndex);
                }
                else
                {
                    // 停止TCP自由口通讯
                    StopTcpCommunication(instrumentIndex);
                }
            }
        }

        // 开始通讯
        private async void StartCommunication(int instrumentIndex, string portName)
        {
            try
            {
                // 保存端口名称
                _portNames[instrumentIndex] = portName;
                
                // 启用自动重连
                _autoReconnectEnabled[instrumentIndex] = true;
                _reconnectAttempts[instrumentIndex] = 0;
                
                // 打开串口
                _isConnected[instrumentIndex] = _commServices[instrumentIndex].OpenSerialPort(
                    portName, 9600, 8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One);
                
                if (_isConnected[instrumentIndex])
                {
                    _isCommunicating[instrumentIndex] = true;
                    
                    // 重置接收数据统计
                    _commServices[instrumentIndex].ResetReceivedBytesCounter();
                    
                    // 更新配置窗体的状态显示
                    if (_configForm != null && !_configForm.IsDisposed)
                    {
                        _configForm.Invoke(new Action(() => {
                            _configForm.UpdateInstrumentStatus(instrumentIndex, true, "通讯中，接收数据长度为0");
                        }));
                    }
                    
                    txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: 串口 {portName} 打开成功\r\n");
                    
                    // 启动对应的定时器
                    switch (instrumentIndex)
                    {
                        case 0:
                            timer1.Start();
                            break;
                        case 1:
                            timer2.Start();
                            break;
                        case 2:
                            timer3.Start();
                            break;
                    }
                }
                else
                {
                    // 更新配置窗体的状态显示
                    if (_configForm != null && !_configForm.IsDisposed)
                    {
                        _configForm.Invoke(new Action(() => {
                            _configForm.UpdateInstrumentStatus(instrumentIndex, false, "连接失败");
                        }));
                    }
                    
                    txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: 串口 {portName} 打开失败\r\n");
                    
                    // 启动自动重连
                    StartAutoReconnect(instrumentIndex);
                }
            }
            catch (Exception ex)
            {
                txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: 打开串口时出错: {ex.Message}\r\n");
                
                // 更新配置窗体的状态显示
                if (_configForm != null && !_configForm.IsDisposed)
                {
                    _configForm.Invoke(new Action(() => {
                        _configForm.UpdateInstrumentStatus(instrumentIndex, false, "连接异常");
                    }));
                }
                
                // 启动自动重连
                StartAutoReconnect(instrumentIndex);
            }
        }

        // 停止通讯
        private void StopCommunication(int instrumentIndex)
        {
            // 禁用自动重连
            StopAutoReconnect(instrumentIndex);
            
            _isCommunicating[instrumentIndex] = false;
            
            // 停止对应的定时器
            switch (instrumentIndex)
            {
                case 0:
                    timer1.Stop();
                    break;
                case 1:
                    timer2.Stop();
                    break;
                case 2:
                    timer3.Stop();
                    break;
            }
            
            // 关闭串口
            if (_isConnected[instrumentIndex])
            {
                _commServices[instrumentIndex].CloseSerialPort();
                _isConnected[instrumentIndex] = false;
                txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: 串口 {_portNames[instrumentIndex]} 已关闭\r\n");
            }
        }
        
        // 开始TCP通讯
        private async void StartTcpCommunication(int instrumentIndex, string ipAddress, int port)
        {
            try
            {
                // 保存TCP配置
                _tcpIpAddresses[instrumentIndex] = ipAddress;
                _tcpPorts[instrumentIndex] = port;
                
                // 启用自动重连
                _autoReconnectEnabled[instrumentIndex] = true;
                _reconnectAttempts[instrumentIndex] = 0;
                
                // 连接TCP服务器
                _isConnected[instrumentIndex] = await _commServices[instrumentIndex].ConnectToTcpFreePort(ipAddress, port);
                
                if (_isConnected[instrumentIndex])
                {
                    _isCommunicating[instrumentIndex] = true;
                    
                    // 重置接收数据统计
                    _commServices[instrumentIndex].ResetReceivedBytesCounter();
                    
                    // 更新配置窗体的状态显示
                    if (_configForm != null && !_configForm.IsDisposed)
                    {
                        _configForm.Invoke(new Action(() => {
                            _configForm.UpdateInstrumentStatus(instrumentIndex, true, "通讯中,接收数据长度为0");
                        }));
                    }
                    
                    txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: TCP {ipAddress}:{port} 连接成功\r\n");
                    
                    // 启动定时器4
                    if (instrumentIndex == 3)
                    {
                        timer4.Start();
                    }
                }
                else
                {
                    // 更新配置窗体的状态显示
                    if (_configForm != null && !_configForm.IsDisposed)
                    {
                        _configForm.Invoke(new Action(() => {
                            _configForm.UpdateInstrumentStatus(instrumentIndex, false, "连接失败");
                        }));
                    }
                    
                    txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: TCP {ipAddress}:{port} 连接失败\r\n");
                    
                    // 启动自动重连
                    StartAutoReconnect(instrumentIndex);
                }
            }
            catch (Exception ex)
            {
                txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: 连接TCP时出错: {ex.Message}\r\n");
                
                // 更新配置窗体的状态显示
                if (_configForm != null && !_configForm.IsDisposed)
                {
                    _configForm.Invoke(new Action(() => {
                        _configForm.UpdateInstrumentStatus(instrumentIndex, false, "连接异常");
                    }));
                }
                
                // 启动自动重连
                StartAutoReconnect(instrumentIndex);
            }
        }
        
        // 停止TCP通讯
        private void StopTcpCommunication(int instrumentIndex)
        {
            // 禁用自动重连
            StopAutoReconnect(instrumentIndex);
            
            _isCommunicating[instrumentIndex] = false;
            
            // 停止定时器
            if (instrumentIndex == 3)
            {
                timer4.Stop();
            }
            
            // 断开TCP连接
            if (_isConnected[instrumentIndex])
            {
                _commServices[instrumentIndex].DisconnectTcpFreePort();
                _isConnected[instrumentIndex] = false;
                txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: TCP {_tcpIpAddresses[instrumentIndex]}:{_tcpPorts[instrumentIndex]} 已断开\r\n");
            }
        }
        
        // 开始Modbus TCP通讯
        private async void StartModbusTcpCommunication(int instrumentIndex, string ipAddress, int port)
        {
            try
            {
                // 保存TCP配置
                _tcpIpAddresses[instrumentIndex] = ipAddress;
                _tcpPorts[instrumentIndex] = port;
                
                // 启用自动重连
                _autoReconnectEnabled[instrumentIndex] = true;
                _reconnectAttempts[instrumentIndex] = 0;
                
                // 连接Modbus TCP服务器
                _isConnected[instrumentIndex] = await _commServices[instrumentIndex].ConnectToModbusTcp(ipAddress, port);
                
                if (_isConnected[instrumentIndex])
                {
                    _isCommunicating[instrumentIndex] = true;
                    
                    // 重置接收数据统计
                    _commServices[instrumentIndex].ResetReceivedBytesCounter();
                    
                    // 更新配置窗体的状态显示
                    if (_configForm != null && !_configForm.IsDisposed)
                    {
                        _configForm.Invoke(new Action(() => {
                            _configForm.UpdateInstrumentStatus(instrumentIndex, true, "通讯中,接收数据长度为0");
                        }));
                    }
                    
                    txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: Modbus TCP {ipAddress}:{port} 连接成功\r\n");
                    
                    // 启动定时器5
                    timer5.Start();
                }
                else
                {
                    // 更新配置窗体的状态显示
                    if (_configForm != null && !_configForm.IsDisposed)
                    {
                        _configForm.Invoke(new Action(() => {
                            _configForm.UpdateInstrumentStatus(instrumentIndex, false, "连接失败");
                        }));
                    }
                    
                    txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: Modbus TCP {ipAddress}:{port} 连接失败\r\n");
                    
                    // 启动自动重连
                    StartAutoReconnect(instrumentIndex);
                }
            }
            catch (Exception ex)
            {
                txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: 连接Modbus TCP时出错: {ex.Message}\r\n");
                
                // 更新配置窗体的状态显示
                if (_configForm != null && !_configForm.IsDisposed)
                {
                    _configForm.Invoke(new Action(() => {
                        _configForm.UpdateInstrumentStatus(instrumentIndex, false, "连接异常");
                    }));
                }
                
                // 启动自动重连
                StartAutoReconnect(instrumentIndex);
            }
        }
        
        // 停止Modbus TCP通讯
        private void StopModbusTcpCommunication(int instrumentIndex)
        {
            // 禁用自动重连
            StopAutoReconnect(instrumentIndex);
            
            _isCommunicating[instrumentIndex] = false;
            
            // 停止定时器
            timer5.Stop();
            
            // 断开连接
            if (_isConnected[instrumentIndex])
            {
                _commServices[instrumentIndex].DisconnectModbusTcp();
                _isConnected[instrumentIndex] = false;
                txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: Modbus TCP {_tcpIpAddresses[instrumentIndex]}:{_tcpPorts[instrumentIndex]} 已断开\r\n");
            }
        }
        
        // 开始TCP字符串通讯 (仪表6)
        private async void StartTcpStringCommunication(int instrumentIndex, string ipAddress, int port)
        {
            try
            {
                // 保存TCP配置
                _tcpIpAddresses[instrumentIndex] = ipAddress;
                _tcpPorts[instrumentIndex] = port;
                
                // 启用自动重连
                _autoReconnectEnabled[instrumentIndex] = true;
                _reconnectAttempts[instrumentIndex] = 0;
                
                // 连接TCP服务器
                _isConnected[instrumentIndex] = await _commServices[instrumentIndex].ConnectToTcpFreePort(ipAddress, port);
                
                if (_isConnected[instrumentIndex])
                {
                    _isCommunicating[instrumentIndex] = true;
                    
                    // 重置接收数据统计
                    _commServices[instrumentIndex].ResetReceivedBytesCounter();
                    
                    // 更新配置窗体的状态显示
                    if (_configForm != null && !_configForm.IsDisposed)
                    {
                        _configForm.Invoke(new Action(() => {
                            _configForm.UpdateInstrumentStatus(instrumentIndex, true, "通讯中,接收数据长度为0");
                        }));
                    }
                    
                    txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: TCP字符串 {ipAddress}:{port} 连接成功\r\n");
                    
                    // 启动定时器6
                    timer6.Start();
                }
                else
                {
                    // 更新配置窗体的状态显示
                    if (_configForm != null && !_configForm.IsDisposed)
                    {
                        _configForm.Invoke(new Action(() => {
                            _configForm.UpdateInstrumentStatus(instrumentIndex, false, "连接失败");
                        }));
                    }
                    
                    txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: TCP字符串 {ipAddress}:{port} 连接失败\r\n");
                    
                    // 启动自动重连
                    StartAutoReconnect(instrumentIndex);
                }
            }
            catch (Exception ex)
            {
                txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: 连接TCP字符串时出错: {ex.Message}\r\n");
                
                // 更新配置窗体的状态显示
                if (_configForm != null && !_configForm.IsDisposed)
                {
                    _configForm.Invoke(new Action(() => {
                        _configForm.UpdateInstrumentStatus(instrumentIndex, false, "连接异常");
                    }));
                }
                
                // 启动自动重连
                StartAutoReconnect(instrumentIndex);
            }
        }
        
        // 停止TCP字符串通讯 (仪表6)
        private void StopTcpStringCommunication(int instrumentIndex)
        {
            // 禁用自动重连
            StopAutoReconnect(instrumentIndex);
            
            _isCommunicating[instrumentIndex] = false;
            
            // 停止定时器
            timer6.Stop();
            
            // 断开TCP连接
            if (_isConnected[instrumentIndex])
            {
                _commServices[instrumentIndex].DisconnectTcpFreePort();
                _isConnected[instrumentIndex] = false;
                txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: TCP字符串 {_tcpIpAddresses[instrumentIndex]}:{_tcpPorts[instrumentIndex]} 已断开\r\n");
            }
        }
        
        #region 自动重连功能
        
        // 启动自动重连
        private void StartAutoReconnect(int instrumentIndex)
        {
            if (_autoReconnectEnabled[instrumentIndex] && _reconnectAttempts[instrumentIndex] < MAX_RECONNECT_ATTEMPTS)
            {
                _reconnectAttempts[instrumentIndex]++;
                
                // 更新状态显示
                if (_configForm != null && !_configForm.IsDisposed)
                {
                    _configForm.Invoke(new Action(() => {
                        _configForm.UpdateInstrumentStatus(instrumentIndex, false, 
                            $"重连中({_reconnectAttempts[instrumentIndex]}/{MAX_RECONNECT_ATTEMPTS})...");
                    }));
                }
                
                txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: 第{_reconnectAttempts[instrumentIndex]}次重连尝试，{RECONNECT_INTERVAL/1000}秒后重试...\r\n");
                
                // 启动重连计时器
                _reconnectTimers[instrumentIndex].Start();
            }
            else if (_reconnectAttempts[instrumentIndex] >= MAX_RECONNECT_ATTEMPTS)
            {
                // 达到最大重连次数，停止重连
                txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: 重连失败，已达到最大重试次数({MAX_RECONNECT_ATTEMPTS})\r\n");
                
                // 更新状态显示
                if (_configForm != null && !_configForm.IsDisposed)
                {
                    _configForm.Invoke(new Action(() => {
                        _configForm.UpdateInstrumentStatus(instrumentIndex, false, "重连失败");
                        _configForm.IsCommunicating[instrumentIndex] = false;
                        _configForm.UpdateAllInstrumentUI();
                    }));
                }
                
                _autoReconnectEnabled[instrumentIndex] = false;
            }
        }
        
        // 停止自动重连
        private void StopAutoReconnect(int instrumentIndex)
        {
            _autoReconnectEnabled[instrumentIndex] = false;
            _reconnectTimers[instrumentIndex].Stop();
            _reconnectAttempts[instrumentIndex] = 0;
        }
        
        // 重连计时器回调
        private void ReconnectTimer_Tick(int instrumentIndex)
        {
            _reconnectTimers[instrumentIndex].Stop();
            
            if (!_autoReconnectEnabled[instrumentIndex])
                return;
            
            txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: 正在尝试重新连接...\r\n");
            
            // 根据仪表类型执行重连
            if (instrumentIndex <= 2)
            {
                // 串口通讯 (仪表1-3)
                TryReconnectSerial(instrumentIndex);
            }
            else if (instrumentIndex == 3)
            {
                // TCP自由口通讯 (仪表4)
                TryReconnectTcp(instrumentIndex);
            }
            else if (instrumentIndex == 4)
            {
                // Modbus TCP通讯 (仪表5)
                TryReconnectModbusTcp(instrumentIndex);
            }
            else if (instrumentIndex == 5)
            {
                // TCP字符串通讯 (仪表6)
                TryReconnectTcpString(instrumentIndex);
            }
        }
        
        // 尝试重连串口
        private void TryReconnectSerial(int instrumentIndex)
        {
            try
            {
                // 先关闭之前的连接
                if (_commServices[instrumentIndex] != null)
                {
                    _commServices[instrumentIndex].CloseSerialPort();
                }
                
                // 重新打开串口
                _isConnected[instrumentIndex] = _commServices[instrumentIndex].OpenSerialPort(
                    _portNames[instrumentIndex], 9600, 8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One);
                
                if (_isConnected[instrumentIndex])
                {
                    _isCommunicating[instrumentIndex] = true;
                    _reconnectAttempts[instrumentIndex] = 0;
                    _commServices[instrumentIndex].ResetReceivedBytesCounter();
                    
                    // 更新状态显示
                    if (_configForm != null && !_configForm.IsDisposed)
                    {
                        _configForm.Invoke(new Action(() => {
                            _configForm.UpdateInstrumentStatus(instrumentIndex, true, "通讯中，接收数据长度为0");
                        }));
                    }
                    
                    txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: 重连成功\r\n");
                    
                    // 重新启动定时器
                    switch (instrumentIndex)
                    {
                        case 0: timer1.Start(); break;
                        case 1: timer2.Start(); break;
                        case 2: timer3.Start(); break;
                    }
                }
                else
                {
                    // 重连失败，继续尝试
                    StartAutoReconnect(instrumentIndex);
                }
            }
            catch (Exception ex)
            {
                txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: 重连出错: {ex.Message}\r\n");
                StartAutoReconnect(instrumentIndex);
            }
        }
        
        // 尝试重连TCP
        private async void TryReconnectTcp(int instrumentIndex)
        {
            try
            {
                // 先关闭之前的连接
                if (_commServices[instrumentIndex] != null)
                {
                    _commServices[instrumentIndex].DisconnectTcpFreePort();
                }
                
                // 重新连接
                _isConnected[instrumentIndex] = await _commServices[instrumentIndex].ConnectToTcpFreePort(
                    _tcpIpAddresses[instrumentIndex], _tcpPorts[instrumentIndex]);
                
                if (_isConnected[instrumentIndex])
                {
                    _isCommunicating[instrumentIndex] = true;
                    _reconnectAttempts[instrumentIndex] = 0;
                    _commServices[instrumentIndex].ResetReceivedBytesCounter();
                    
                    // 更新状态显示
                    if (_configForm != null && !_configForm.IsDisposed)
                    {
                        _configForm.Invoke(new Action(() => {
                            _configForm.UpdateInstrumentStatus(instrumentIndex, true, "通讯中,接收数据长度为0");
                        }));
                    }
                    
                    txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: 重连成功\r\n");
                    
                    // 重新启动定时器
                    timer4.Start();
                }
                else
                {
                    // 重连失败，继续尝试
                    StartAutoReconnect(instrumentIndex);
                }
            }
            catch (Exception ex)
            {
                txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: 重连出错: {ex.Message}\r\n");
                StartAutoReconnect(instrumentIndex);
            }
        }
        
        // 尝试重连Modbus TCP
        private async void TryReconnectModbusTcp(int instrumentIndex)
        {
            try
            {
                // 先关闭之前的连接
                if (_commServices[instrumentIndex] != null)
                {
                    _commServices[instrumentIndex].DisconnectModbusTcp();
                }
                
                // 重新连接
                _isConnected[instrumentIndex] = await _commServices[instrumentIndex].ConnectToModbusTcp(
                    _tcpIpAddresses[instrumentIndex], _tcpPorts[instrumentIndex]);
                
                if (_isConnected[instrumentIndex])
                {
                    _isCommunicating[instrumentIndex] = true;
                    _reconnectAttempts[instrumentIndex] = 0;
                    _commServices[instrumentIndex].ResetReceivedBytesCounter();
                    
                    // 更新状态显示
                    if (_configForm != null && !_configForm.IsDisposed)
                    {
                        _configForm.Invoke(new Action(() => {
                            _configForm.UpdateInstrumentStatus(instrumentIndex, true, "通讯中,接收数据长度为0");
                        }));
                    }
                    
                    txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: 重连成功\r\n");
                    
                    // 重新启动定时器
                    timer5.Start();
                }
                else
                {
                    // 重连失败，继续尝试
                    StartAutoReconnect(instrumentIndex);
                }
            }
            catch (Exception ex)
            {
                txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: 重连出错: {ex.Message}\r\n");
                StartAutoReconnect(instrumentIndex);
            }
        }
        
        // 尝试重连TCP字符串通讯 (仪表6)
        private async void TryReconnectTcpString(int instrumentIndex)
        {
            try
            {
                // 先关闭之前的连接
                if (_commServices[instrumentIndex] != null)
                {
                    _commServices[instrumentIndex].DisconnectTcpFreePort();
                }
                
                // 重新连接
                _isConnected[instrumentIndex] = await _commServices[instrumentIndex].ConnectToTcpFreePort(
                    _tcpIpAddresses[instrumentIndex], _tcpPorts[instrumentIndex]);
                
                if (_isConnected[instrumentIndex])
                {
                    _isCommunicating[instrumentIndex] = true;
                    _reconnectAttempts[instrumentIndex] = 0;
                    _commServices[instrumentIndex].ResetReceivedBytesCounter();
                    
                    // 更新状态显示
                    if (_configForm != null && !_configForm.IsDisposed)
                    {
                        _configForm.Invoke(new Action(() => {
                            _configForm.UpdateInstrumentStatus(instrumentIndex, true, "通讯中,接收数据长度为0");
                        }));
                    }
                    
                    txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: 重连成功\r\n");
                    
                    // 重新启动定时器
                    timer6.Start();
                }
                else
                {
                    // 重连失败，继续尝试
                    StartAutoReconnect(instrumentIndex);
                }
            }
            catch (Exception ex)
            {
                txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表{instrumentIndex + 1}: 重连出错: {ex.Message}\r\n");
                StartAutoReconnect(instrumentIndex);
            }
        }
        
        #endregion

        // 将16进制字符串转换为字节数组
        private byte[] HexStringToByteArray(string hex)
        {
            hex = hex.Replace(" ", "").Replace("-", "");
            if (hex.Length % 2 != 0)
                throw new ArgumentException("十六进制字符串长度必须为偶数");

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        // 仪表1定时器事件 (发送 0x55)
        private async void timer1_Tick(object sender, EventArgs e)
        {
            if (!_isCommunicating[0] || !_isConnected[0])
                return;

            try
            {
                // 发送16进制指令 0x55
                byte[] command = new byte[] { 0x55 };
                bool result = await _commServices[0].SendSerialDataAsync(command);
                
                string data = $"[{DateTime.Now:HH:mm:ss}] 仪表1 ({_portNames[0]}): 发送指令 [55] {(result ? "成功" : "失败")}\r\n";
                
                // 在UI线程上更新显示
                this.Invoke(new Action(() => {
                    txtDisplay.AppendText(data);
                    // 自动滚动到最新内容
                    txtDisplay.SelectionStart = txtDisplay.Text.Length;
                    txtDisplay.ScrollToCaret();
                }));
                
                // 更新配置窗体的接收数据统计
                if (_configForm != null && !_configForm.IsDisposed)
                {
                    int receivedLength = _commServices[0].LastReceivedLength;
                    _configForm.Invoke(new Action(() => {
                        _configForm.UpdateInstrumentStatus(0, true, $"通讯中，接收数据长度为{receivedLength}");
                    }));
                }
                
                // 如果发送成功，等待接收返回数据
                if (result)
                {
                    // 等待一段时间让设备响应
                    await Task.Delay(10);
                    
                    // 尝试读取返回的数据
                    byte[] response = await _commServices[0].ReadReceivedDataAsync(500);
                    if (response != null && response.Length > 0)
                    {
                        string responseData = $"[{DateTime.Now:HH:mm:ss}] 仪表1 ({_portNames[0]}): 返回数据 [{BitConverter.ToString(response).Replace("-", " ")}]\r\n";
                        this.Invoke(new Action(() => {
                            txtDisplay.AppendText(responseData);
                            txtDisplay.SelectionStart = txtDisplay.Text.Length;
                            txtDisplay.ScrollToCaret();
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() => {
                    txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表1: 通讯异常: {ex.Message}\r\n");
                }));
                
                // 通讯异常，停止定时器并启动自动重连
                timer1.Stop();
                _isConnected[0] = false;
                _isCommunicating[0] = false;
                StartAutoReconnect(0);
            }
        }

        // 仪表2定时器事件 (发送 0x77)
        private async void timer2_Tick(object sender, EventArgs e)
        {
            if (!_isCommunicating[1] || !_isConnected[1])
                return;

            try
            {
                // 发送16进制指令 0x77
                byte[] command = new byte[] { 0x77 };
                bool result = await _commServices[1].SendSerialDataAsync(command);
                
                string data = $"[{DateTime.Now:HH:mm:ss}] 仪表2 ({_portNames[1]}): 发送指令 [77] {(result ? "成功" : "失败")}\r\n";
                
                // 在UI线程上更新显示
                this.Invoke(new Action(() => {
                    txtDisplay.AppendText(data);
                    // 自动滚动到最新内容
                    txtDisplay.SelectionStart = txtDisplay.Text.Length;
                    txtDisplay.ScrollToCaret();
                }));
                
                // 更新配置窗体的接收数据统计
                if (_configForm != null && !_configForm.IsDisposed)
                {
                    int receivedLength = _commServices[1].LastReceivedLength;
                    _configForm.Invoke(new Action(() => {
                        _configForm.UpdateInstrumentStatus(1, true, $"通讯中，接收数据长度为{receivedLength}");
                    }));
                }
                
                // 如果发送成功，等待接收返回数据
                if (result)
                {
                    // 等待一段时间让设备响应
                    await Task.Delay(100);
                    
                    // 尝试读取返回的数据
                    byte[] response = await _commServices[1].ReadReceivedDataAsync(500);
                    if (response != null && response.Length > 0)
                    {
                        string responseData = $"[{DateTime.Now:HH:mm:ss}] 仪表2 ({_portNames[1]}): 返回数据 [{BitConverter.ToString(response).Replace("-", " ")}]\r\n";
                        this.Invoke(new Action(() => {
                            txtDisplay.AppendText(responseData);
                            txtDisplay.SelectionStart = txtDisplay.Text.Length;
                            txtDisplay.ScrollToCaret();
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() => {
                    txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表2: 通讯异常: {ex.Message}\r\n");
                }));
                
                // 通讯异常，停止定时器并启动自动重连
                timer2.Stop();
                _isConnected[1] = false;
                _isCommunicating[1] = false;
                StartAutoReconnect(1);
            }
        }

        // 仪表3定时器事件 (RTU通讯)
        private async void timer3_Tick(object sender, EventArgs e)
        {
            if (!_isCommunicating[2] || !_isConnected[2])
                return;

            try
            {
                // 发送RTU读取命令（从地址0开始读取2个寄存器）
                short[] response = await _commServices[2].ReadHoldingRegistersAsShortRtu(1, 0, 3);
                
                string data;
                if (response != null && response.Length > 0)
                {
                    // 解析响应数据
                    //string hexData = BitConverter.ToString(response).Replace("-", " ");
                    //data = $"[{DateTime.Now:HH:mm:ss}] 仪表3 ({_portNames[2]}): 接收到RTU数据 [{hexData}]\r\n";
                }
                else
                {
                    data = $"[{DateTime.Now:HH:mm:ss}] 仪表3 ({_portNames[2]}): 未接收到数据\r\n";
                }
                
                // 在UI线程上更新显示
                //this.Invoke(new Action(() => {
                //    txtDisplay.AppendText(data);
                //    // 自动滚动到最新内容
                //    txtDisplay.SelectionStart = txtDisplay.Text.Length;
                //    txtDisplay.ScrollToCaret();
                //}));
                
                // 更新配置窗体的接收数据统计
                if (_configForm != null && !_configForm.IsDisposed)
                {
                    int receivedLength = _commServices[2].LastReceivedLength;
                    _configForm.Invoke(new Action(() => {
                        _configForm.UpdateInstrumentStatus(2, true, $"通讯中，接收数据长度为{receivedLength}");
                    }));
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() => {
                    txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表3: 通讯异常: {ex.Message}\r\n");
                }));
                
                // 通讯异常，停止定时器并启动自动重连
                timer3.Stop();
                _isConnected[2] = false;
                _isCommunicating[2] = false;
                StartAutoReconnect(2);
            }
        }

        // 仪表4定时器事件 (TCP自由口通讯)
        private async void timer4_Tick(object sender, EventArgs e)
        {
            if (!_isCommunicating[3] || !_isConnected[3])
                return;

            try
            {
                // 发送TCP测试数据
                byte[] command = new byte[] { 0x12 , 0x31, 0x23, 0x45, 0x46, 0x34, 0x34, 0x56, 0x77, 0x08 };
                bool result = await _commServices[3].SendTcpDataAsync(command);
                
                string data = $"[{DateTime.Now:HH:mm:ss}] 仪表4 (TCP {_tcpIpAddresses[3]}:{_tcpPorts[3]}): 发送数据 {(result ? "成功" : "失败")}\r\n";
                
                // 在UI线程上更新显示
                this.Invoke(new Action(() => {
                    txtDisplay.AppendText(data);
                    txtDisplay.SelectionStart = txtDisplay.Text.Length;
                    txtDisplay.ScrollToCaret();
                }));
                
                // 更新配置窗体的接收数据统计
                if (_configForm != null && !_configForm.IsDisposed)
                {
                    int receivedLength = _commServices[3].LastReceivedLength;
                    _configForm.Invoke(new Action(() => {
                        _configForm.UpdateInstrumentStatus(3, true, $"通讯中,接收数据长度为{receivedLength}");
                    }));
                }
                
                // 尝试读取返回数据
                if (result)
                {
                    await Task.Delay(100);
                    byte[] response = await _commServices[3].ReadTcpReceivedDataAsync(500);
                    if (response != null && response.Length > 0)
                    {
                        string responseData = $"[{DateTime.Now:HH:mm:ss}] 仪表4: 返回数据 [{BitConverter.ToString(response).Replace("-", " ")}]\r\n";
                        this.Invoke(new Action(() => {
                            txtDisplay.AppendText(responseData);
                            txtDisplay.SelectionStart = txtDisplay.Text.Length;
                            txtDisplay.ScrollToCaret();
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() => {
                    txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表4: 通讯异常: {ex.Message}\r\n");
                }));
                
                // 通讯异常，停止定时器并启动自动重连
                timer4.Stop();
                _isConnected[3] = false;
                _isCommunicating[3] = false;
                StartAutoReconnect(3);
            }
        }

        // 仪表5定时器事件 (Modbus TCP读写寄存器)
        private async void timer5_Tick(object sender, EventArgs e)
        {
            if (!_isCommunicating[4] || !_isConnected[4])
                return;

            try
            {
                // 读取保持寄存器（从地址0开始读取10个寄存器）
                ushort[] response = await _commServices[4].ModbusTcp_ReadHoldingRegisters(1, 0, 10);
                
                if (response != null && response.Length > 0)
                {
                    // 格式化显示寄存器值
                    string regValues = string.Join(", ", response.Select((v, i) => $"[{i}]={v}"));
                    string data = $"[{DateTime.Now:HH:mm:ss}] 仪表5 (Modbus TCP {_tcpIpAddresses[4]}:{_tcpPorts[4]}): 读取寄存器 {regValues}\r\n";
                    
                    // 在UI线程上更新显示
                    this.Invoke(new Action(() => {
                        txtDisplay.AppendText(data);
                        txtDisplay.SelectionStart = txtDisplay.Text.Length;
                        txtDisplay.ScrollToCaret();
                    }));
                }
                
                // 更新配置窗体的接收数据统计
                if (_configForm != null && !_configForm.IsDisposed)
                {
                    int receivedLength = _commServices[4].LastReceivedLength;
                    _configForm.Invoke(new Action(() => {
                        _configForm.UpdateInstrumentStatus(4, true, $"通讯中,接收数据长度为{receivedLength}");
                    }));
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() => {
                    txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表5: 通讯异常: {ex.Message}\r\n");
                }));
                
                // 通讯异常，停止定时器并启动自动重连
                timer5.Stop();
                _isConnected[4] = false;
                _isCommunicating[4] = false;
                StartAutoReconnect(4);
            }
        }

        // 仪表6定时器事件 (TCP字符串通讯，发送MEAS? U1,I1,P1)
        private async void timer6_Tick(object sender, EventArgs e)
        {
            if (!_isCommunicating[5])
                return;

            try
            {
                // 发送字符串命令 "MEAS? U1,I1,P1" (转为字节数组发送，使用ASCII编码)
                string command = ":MEAS? U1,I1,P1";
                byte[] commandBytes = System.Text.Encoding.ASCII.GetBytes(command);
                
                string data = $"[{DateTime.Now:HH:mm:ss}] 仪表6 (TCP {_tcpIpAddresses[5]}:{_tcpPorts[5]}): 发送 \"{command}\"\r\n";
                
                // 在UI线程上更新显示
                this.Invoke(new Action(() => {
                    txtDisplay.AppendText(data);
                    txtDisplay.SelectionStart = txtDisplay.Text.Length;
                    txtDisplay.ScrollToCaret();
                }));
                
                // 使用独立连接方式发送接收（每次新建连接，避免后台循环干扰）
                byte[] response = await _commServices[5].SendAndReceiveWithNewConnectionAsync(
                    _tcpIpAddresses[5], _tcpPorts[5], commandBytes, 2000);
                
                if (response != null && response.Length > 0)
                {
                    string responseStr = System.Text.Encoding.ASCII.GetString(response);
                    string responseData = $"[{DateTime.Now:HH:mm:ss}] 仪表6: 返回数据 \"{responseStr.Trim()}\"\r\n";
                    this.Invoke(new Action(() => {
                        txtDisplay.AppendText(responseData);
                        txtDisplay.SelectionStart = txtDisplay.Text.Length;
                        txtDisplay.ScrollToCaret();
                    }));
                }
                else
                {
                    this.Invoke(new Action(() => {
                        txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表6: 未收到响应数据\r\n");
                    }));
                }
                
                // 更新配置窗体的接收数据统计
                if (_configForm != null && !_configForm.IsDisposed)
                {
                    int receivedLength = _commServices[5].LastReceivedLength;
                    _configForm.Invoke(new Action(() => {
                        _configForm.UpdateInstrumentStatus(5, true, $"通讯中,接收数据长度为{receivedLength}");
                    }));
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() => {
                    txtDisplay.AppendText($"[{DateTime.Now:HH:mm:ss}] 仪表6: 通讯异常: {ex.Message}\r\n");
                }));
            }
        }

        // 窗体关闭事件
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 停止所有通讯和自动重连
            for (int i = 0; i < 6; i++)
            {
                // 停止自动重连
                StopAutoReconnect(i);
                
                _isCommunicating[i] = false;
                
                // 停止定时器
                switch (i)
                {
                    case 0:
                        timer1.Stop();
                        break;
                    case 1:
                        timer2.Stop();
                        break;
                    case 2:
                        timer3.Stop();
                        break;
                    case 3:
                        timer4.Stop();
                        break;
                    case 4:
                        timer5.Stop();
                        break;
                    case 5:
                        timer6.Stop();
                        break;
                }
                
                // 关闭串口或TCP
                if (_isConnected[i])
                {
                    if (i < 3)
                    {
                        _commServices[i].CloseSerialPort();
                    }
                    else if (i == 3 || i == 5)
                    {
                        _commServices[i].DisconnectTcpFreePort();
                    }
                    else
                    {
                        _commServices[i].DisconnectModbusTcp();
                    }
                    _isConnected[i] = false;
                }
            }
            
            // 清理资源
            for (int i = 0; i < 6; i++)
            {
                _commServices[i]?.Dispose();
            }
            
            // 关闭配置窗体
            if (_configForm != null && !_configForm.IsDisposed)
            {
                _configForm.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}