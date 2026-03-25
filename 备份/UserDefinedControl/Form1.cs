﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UcAsp.Opc;
using UserDefinedControl.OPCUA;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Timer = System.Windows.Forms.Timer;
// 添加MyS7Connect库的引用
// using MyS7Connect; // 请根据您的MyS7Connect库的实际命名空间调整

namespace UserDefinedControl
{
    public partial class Form1 : Form
    {
        private Timer simulationTimer;
        private Timer signalBUpdateTimer; // 新增：更新SignalB状态的定时器
        private IOpcUaService _opcService;
        private CancellationTokenSource _cancellationTokenSource;
        private bool plcStaus=false;
     
        // 模拟的地址存储器（用于演示，实际使用时可以移除）
        private Dictionary<string, bool> addressStorage = new Dictionary<string, bool>();
        
        public Form1()
        {
            InitializeComponent();
            InitializeControls();
            InitializeOpcService();
        }

        #region 控件初始化 刷新
        private void InitializeControls()
        {
            // 创建信号更新定时器（同时更新SignalA和SignalB）
            signalBUpdateTimer = new Timer();
            signalBUpdateTimer.Interval = 200; // 500毫秒更新一次
            signalBUpdateTimer.Tick += SignalUpdateTimer_Tick;
            signalBUpdateTimer.Start();
            
            // 设置窗体属性
            this.Text = "自定义指示灯按钮控件演示 - PLC地址控制版";
            this.StartPosition = FormStartPosition.CenterScreen;
            SetupButtonControl(buttonControl1, "1214.PLC1.PowerCtrl.Motor.item1.PC_PowerSel");
            SetupButtonControl( buttonControl2, "1214.PLC1.PowerCtrl.Motor.item1.PC_PowerSel");
            SetupButtonControl(buttonControl4, "1214.PLC1.Val.Switch.item1.SwitchVal_Open");
            SetupButtonControl(buttonControl3, "1214.PLC1.Val.Switch.item1.SwitchVal_Open");

            // 为每个控件设置真实PLC地址和委托方法
            SetupIndicatorButton(indicatorButton1, "1214.PLC1.ADDR_BTN1", "1214.PLC1.SIGNAL_A1", "1214.PLC1.SIGNAL_B1");
            SetupIndicatorButton(indicatorButton2, "1214.PLC1.ADDR_BTN2", "1214.PLC1.SIGNAL_A2", "1214.PLC1.SIGNAL_B2");
            SetupIndicatorButton(indicatorButton3, "1214.PLC1.ADDR_BTN3", "1214.PLC1.SIGNAL_A3", "1214.PLC1.SIGNAL_B3");
            
            // 设置数值输入控件的委托方法
            SetupNumericInputControl(numericInput1, "1214.PLC1.Val.Ctrl.item1.PDegree", "1214.PLC1.Val.Ctrl.item1.PDegree_FB");
            SetupNumericInputControl(numericInput2, "1214.PLC1.Val.Ctrl.item2.PDegree", "1214.PLC1.Val.Ctrl.item2.PDegree_FB");

            SetupIndicatorButton(IbtcVal_DN40Flow_Open, "1214.PLC1.Val.Switch.item2.SwitchVal_Open", "1214.PLC1.Val.Switch.item2.SwitchValUpper", "1214.PLC1.Val.Switch.item2.GataVal_STA_Open");
            SetupIndicatorButton(IbtcVal_DN40Flow_Close, "1214.PLC1.Val.Switch.item2.SwitchVal_Close", "1214.PLC1.Val.Switch.item2.SwitchValLower", "1214.PLC1.Val.Switch.item2.GataVal_STA_Close");
            SetupIndicatorButton(IbtcVal_DN80Flow_Open, "1214.PLC1.Val.Switch.item1.SwitchVal_Open", "1214.PLC1.Val.Switch.item1.SwitchValUpper", "1214.PLC1.Val.Switch.item1.GataVal_STA_Open");
            SetupIndicatorButton(IbtcVal_DN80Flow_Close, "1214.PLC1.Val.Switch.item1.SwitchVal_Close", "1214.PLC1.Val.Switch.item1.SwitchValLower", "1214.PLC1.Val.Switch.item1.GataVal_STA_Close");
            SetupIndicatorButton(IbtcVal_DN150Flow_Open, "1214.PLC1.Val.Switch.item8.SwitchVal_Open", "1214.PLC1.Val.Switch.item8.SwitchValUpper", "1214.PLC1.Val.Switch.item8.GataVal_STA_Open");
            SetupIndicatorButton(IbtcVal_DN150Flow_Close, "1214.PLC1.Val.Switch.item8.SwitchVal_Close", "1214.PLC1.Val.Switch.item8.SwitchValLower", "1214.PLC1.Val.Switch.item8.GataVal_STA_Close");

            SetupIndicatorButton(IbtcVal_DN50InLet_Open, "1214.PLC1.Val.Switch.item13.SwitchVal_Open", "1214.PLC1.Val.Switch.item13.SwitchValUpper", "1214.PLC1.Val.Switch.item13.GataVal_STA_Open");
            SetupIndicatorButton(IbtcVal_DN50InLet_Close, "1214.PLC1.Val.Switch.item13.SwitchVal_Close", "1214.PLC1.Val.Switch.item13.SwitchValLower", "1214.PLC1.Val.Switch.item13.GataVal_STA_Close");
            SetupIndicatorButton(IbtcVal_DN100InLet_Open, "1214.PLC1.Val.Switch.item12.SwitchVal_Open", "1214.PLC1.Val.Switch.item12.SwitchValUpper", "1214.PLC1.Val.Switch.item12.GataVal_STA_Open");
            SetupIndicatorButton(IbtcVal_DN100InLet_Close, "1214.PLC1.Val.Switch.item12.SwitchVal_Close", "1214.PLC1.Val.Switch.item12.SwitchValLower", "1214.PLC1.Val.Switch.item12.GataVal_STA_Close");
            SetupIndicatorButton(IbtcVal_DN200InLet_Open, "1214.PLC1.Val.Switch.item11.SwitchVal_Open", "1214.PLC1.Val.Switch.item11.SwitchValUpper", "1214.PLC1.Val.Switch.item11.GataVal_STA_Open");
            SetupIndicatorButton(IbtcVal_DN200InLet_Close, "1214.PLC1.Val.Switch.item11.SwitchVal_Close", "1214.PLC1.Val.Switch.item11.SwitchValLower", "1214.PLC1.Val.Switch.item11.GataVal_STA_Close");

            SetupIndicatorButton(IbtcVal_DN50OutLet_Open, "1214.PLC1.Val.Switch.item5.SwitchVal_Open", "1214.PLC1.Val.Switch.item5.SwitchValUpper", "1214.PLC1.Val.Switch.item5.GataVal_STA_Open");
            SetupIndicatorButton(IbtcVal_DN50OutLet_Close, "1214.PLC1.Val.Switch.item5.SwitchVal_Close", "1214.PLC1.Val.Switch.item5.SwitchValLower", "1214.PLC1.Val.Switch.item5.GataVal_STA_Close");
            SetupIndicatorButton(IbtcVal_DN100OutLet_Open, "1214.PLC1.Val.Switch.item4.SwitchVal_Open", "1214.PLC1.Val.Switch.item4.SwitchValUpper", "1214.PLC1.Val.Switch.item4.GataVal_STA_Open");
            SetupIndicatorButton(IbtcVal_DN100OutLet_Close, "1214.PLC1.Val.Switch.item4.SwitchVal_Close", "1214.PLC1.Val.Switch.item4.SwitchValLower", "1214.PLC1.Val.Switch.item4.GataVal_STA_Close");
            SetupIndicatorButton(IbtcVal_DN150OutLet_Open, "1214.PLC1.Val.Switch.item3.SwitchVal_Open", "1214.PLC1.Val.Switch.item3.SwitchValUpper", "1214.PLC1.Val.Switch.item3.GataVal_STA_Open");
            SetupIndicatorButton(IbtcVal_DN150OutLet_Close, "1214.PLC1.Val.Switch.item3.SwitchVal_Close", "1214.PLC1.Val.Switch.item3.SwitchValLower", "1214.PLC1.Val.Switch.item3.GataVal_STA_Close");

            SetupIndicatorButton(IbtcVal_DN150Main_Open, "1214.PLC1.Val.Switch.item10.SwitchVal_Open", "1214.PLC1.Val.Switch.item10.SwitchValUpper", "1214.PLC1.Val.Switch.item10.GataVal_STA_Open");
            SetupIndicatorButton(IbtcVal_DN150Main_Close, "1214.PLC1.Val.Switch.item10.SwitchVal_Close", "1214.PLC1.Val.Switch.item10.SwitchValLower", "1214.PLC1.Val.Switch.item10.GataVal_STA_Close");
            SetupIndicatorButton(IbtcVal_DN150Drain_Open, "1214.PLC1.Val.Switch.item9.SwitchVal_Open", "1214.PLC1.Val.Switch.item9.SwitchValUpper", "1214.PLC1.Val.Switch.item9.GataVal_STA_Open");
            SetupIndicatorButton(IbtcVal_DN150Drain_Close, "1214.PLC1.Val.Switch.item9.SwitchVal_Close", "1214.PLC1.Val.Switch.item9.SwitchValLower", "1214.PLC1.Val.Switch.item9.GataVal_STA_Close");

            SetupIndicatorButton(IbtcVal_DN50Opnout_Open, "1214.PLC1.Val.Switch.item6.SwitchVal_Open", "1214.PLC1.Val.Switch.item6.SwitchValUpper", "1214.PLC1.Val.Switch.item6.GataVal_STA_Open");
            SetupIndicatorButton(IbtcVal_DN50Opnout_Close, "1214.PLC1.Val.Switch.item6.SwitchVal_Close", "1214.PLC1.Val.Switch.item6.SwitchValLower", "1214.PLC1.Val.Switch.item6.GataVal_STA_Close");
            SetupIndicatorButton(IbtcVal_DN150Opnout_Open, "1214.PLC1.Val.Switch.item7.SwitchVal_Open", "1214.PLC1.Val.Switch.item7.SwitchValUpper", "1214.PLC1.Val.Switch.item7.GataVal_STA_Open");
            SetupIndicatorButton(IbtcVal_DN150Opnout_Close, "1214.PLC1.Val.Switch.item7.SwitchVal_Close", "1214.PLC1.Val.Switch.item7.SwitchValLower", "1214.PLC1.Val.Switch.item7.GataVal_STA_Close");

            controls =  GetAllUpdatableControls(this.Controls);
            // 初始化后立即更新一次信号状态
            // _ = UpdateAllSignalStatesAsync();
        }

        List<Control> controls = new List<Control>();

        private List<Control> GetAllUpdatableControls(Control.ControlCollection controls)
        {
            var list = new List<Control>();

            foreach (Control ctrl in controls)
            {
                if (ctrl is IndicatorButtonControl || ctrl is ButtonControl)
                    list.Add(ctrl);

                if (ctrl.HasChildren)
                    list.AddRange(GetAllUpdatableControls(ctrl.Controls));
            }

            return list;
        }

        private void SignalUpdateTimer_Tick(object sender, EventArgs e)
        {
            // 定期更新所有控件的信号状态，使其与地址状态同步
            _ = UpdateAllSignalStatesAsync();
        }
        
        private void IndicatorButton_SignalBChanged(object sender, bool value)
        {
            IndicatorButtonControl control = sender as IndicatorButtonControl;
           // MessageBox.Show($"控件 {control.Name} 的 SignalB 状态改变为: {value}");
        }
        
        private void IndicatorButton_SignalCChanged(object sender, bool value)
        {
            IndicatorButtonControl control = sender as IndicatorButtonControl;
            this.BeginInvoke(new Action(() => {
                labelStatus.Text = $"控件 {control.Name} 的 SignalC 状态: {value}";
            }));
        }
        
        private void IndicatorButton_ButtonClick(object sender, EventArgs e)
        {
            IndicatorButtonControl control = sender as IndicatorButtonControl;
            Console.WriteLine($"控件 {control.Name} 按钮被点击");
        }

        #endregion


        PlcStatusModel plc1Status;
        PlcStatusModel plc2Status;
        private void Form1_Load(object sender, EventArgs e)
        {
            // 窗体加载时初始化
            //if (OpcUa.objUa.Connect == OpcStatus.Connected)
            //{
            //    MessageBox.Show("连接成功！");

            //}
            //else
            //{

            //    MessageBox.Show("连接失败！");

            //}

            plc1Status = new PlcStatusModel("PLC1", "主控PLC1", "1214.PLC1._System._NoError", "主控制系统PLC");
            _opcService.AddOrUpdatePlc(plc1Status);
            plc2Status = new PlcStatusModel("PLC2", "主控PLC2", "1214.PLC2._System._NoError", "主控制系统PLC");
            _opcService.AddOrUpdatePlc(plc2Status);
            ConnectOpc();
        }

        #region opc服务初始化 ，plc状态刷新订阅，opc连接，opc读取数据
        private void InitializeOpcService()
        {
            try
            {
                // 使用异步优化的服务
                _opcService = OpcUaServiceManager.Current;

                // 注册连接状态变化事件
                _opcService.ConnectionStatusChanged += OnConnectionStatusChanged;

                // 注册PLC状态变化事件
                _opcService.PlcStatusChanged += OnPlcStatusChanged;

                LogMessage("OPC UA 服务初始化完成");
            }
            catch (Exception ex)
            {
                LogMessage($"OPC UA 服务初始化失败: {ex.Message}");
            }
        }

        private void OnConnectionStatusChanged(object sender, bool isConnected)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<object, bool>(OnConnectionStatusChanged), sender, isConnected);
                return;
            }
            //_startButton.Enabled = isConnected && !_isMonitoring;
            //_connectButton.Text = isConnected ? "已连接" : "连接OPC服务";
            //_connectButton.Enabled = !isConnected;

            //if (!isConnected && _isMonitoring)
            //{
            //    _ = StopMonitoringAsync();
            //}

            //LogMessage($"OPC连接状态变化: {(isConnected ? "已连接" : "已断开")}");
        }

        /// <summary>
        /// PLC状态变化事件处理
        /// </summary>
        private void OnPlcStatusChanged(object sender, PlcStatusChangedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<object, PlcStatusChangedEventArgs>(OnPlcStatusChanged), sender, e);
                return;
            }
            // 记录PLC状态变化
            LogMessage($"PLC状态变化: {e.GetStatusChangeDescription()}");
            // 如果是主控PLC，更新显示
            if (e.PlcId == "PLC1")
            {
                if (e.IsDegraded)
                {
                    LogMessage("⚠️ 警告: 主控PLC状态异常！");
                    // 可以添加声音提醒或其他告警机制
                }
                else if (e.IsRecovered)
                {
                    LogMessage("✅ 恢复: 主控PLC状态恢复正常");
                }
            }
        }
        private void ConnectOpc()
        {
            try
            {
                // 使用服务管理器连接
                if (_opcService != null)
                {
                    _opcService.Connect();
                    StartReadAsync();
                    MessageBox.Show("成功");
                }
                else
                {
                    throw new InvalidOperationException("OPC UA服务未初始化");
                }
            }
            catch (Exception ex)
            {
               
                MessageBox.Show($"连接失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 开始读取数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartReadAsync()
        {
            if (!_opcService.IsConnected)
            {
                MessageBox.Show("请先连接OPC UA服务器", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await StartMonitoringAsync();
        }

        bool _isMonitoring;
        /// <summary>
        /// 启动监控读取
        /// </summary>
        /// <returns></returns>
        private async Task StartMonitoringAsync()
        {
            if (_isMonitoring) return;

            try
            {
                _isMonitoring = true;
                _cancellationTokenSource = new CancellationTokenSource();

                // 更新UI状态
                //UpdateUI(() =>
                //{
                //    _startButton.Enabled = false;
                //    _stopButton.Enabled = true;
                //    _progressBar.Visible = true;
                //});

                LogMessage("开始实时数据监控...");

                // 启动数据监控任务
                await StartDataMonitoringLoop();
            }
            catch (Exception ex)
            {
                LogMessage($"启动监控失败: {ex.Message}");
                await StopMonitoringAsync();
            }
        }

        /// <summary>
        /// 停止数据监控
        /// </summary>
        private async Task StopMonitoringAsync()
        {
            if (!_isMonitoring) return;

            _isMonitoring = false;
            _cancellationTokenSource?.Cancel();

            // 更新UI状态
            UpdateUI(() =>
            {
                //_startButton.Enabled = _opcService.IsConnected;
                //_stopButton.Enabled = false;
                //_progressBar.Visible = false;
            });

            LogMessage("数据监控已停止");
        }

        /// <summary>
        /// 数据监控循环
        /// </summary>
        private async Task StartDataMonitoringLoop()
        {
            await Task.Run(async () =>
            {
                while (_isMonitoring && !_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        await ReadOpcDataAsync();
                        await Task.Delay(500, _cancellationTokenSource.Token); // 500ms 更新间隔
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"数据读取错误: {ex.Message}");
                        await Task.Delay(1000, _cancellationTokenSource.Token); // 错误时延长间隔
                    }
                }
            }, _cancellationTokenSource.Token);
        }

       

        /// <summary>
        /// 读取OPC数据（根据系统状态智能读取）
        /// 支持多PLC状态管理
        /// </summary>
        private async Task ReadOpcDataAsync()
        {
            try
            {
                // 获取主控PLC状态
                //var plc1Status = _opcService.GetPlcStatus("PLC1");
                //var plc2Status = _opcService.GetPlcStatus("PLC2");

                // 更新主控PLC状态
                await _opcService.UpdatePlcSystemStatusAsync("PLC1");
               // await _opcService.UpdatePlcSystemStatusAsync("PLC2");

                // 获取更新后的状态
                plc1Status = _opcService.GetPlcStatus("PLC1");
                plc2Status = _opcService.GetPlcStatus("PLC2");
                bool systemNoError1 = plc1Status.IsSystemNormal;
                bool systemNoError2 = plc2Status.IsSystemNormal;
                //_systemNoError = systemNoError1;

                // 更新系统状态显示
                UpdateUI(() =>
                {
                    string statusText = $"系统状态: {(systemNoError1 ? "正常" : "错误")}";
                    if (!plc1Status.IsConnected)
                    {
                        statusText += " (连接断开)";
                    }

                    //_systemStatusLabel.Text = statusText;
                    //_systemStatusLabel.ForeColor = systemNoError ? Color.Green : Color.Red;
                    //_systemStatusLabel.BackColor = systemNoError ? Color.FromArgb(220, 255, 220) : Color.FromArgb(255, 220, 220);
                });

                // 根据系统状态决定是否读取其他数据
                if (_opcService.CanSafelyReadPlcData("PLC1"))
                {
                    // 系统正常，读取流量数据
                    var flowData = await ReadFlowDataAsync();

                    // 更新流量显示
                    UpdateUI(() =>
                    {
                        txtFlow1.Text = $"流量1: {flowData["Flow_1"]:F2} L/min";
                        txtFlow2.Text = $"流量2: {flowData["Flow_2"]:F2} L/min";
                        //_flow3Label.Text = $"流量3: {flowData["Flow_3"]:F2} L/min";

                        txtFlow1.ForeColor = Color.Blue;
                        txtFlow1.ForeColor = Color.Blue;
                        //_flow3Label.ForeColor = Color.Blue;
                    });

                    //_currentData.Clear();
                    //_currentData["1214.PLC1._System._NoError"] = systemNoError;
                    //_currentData["1214.PLC1.DisData.Flow.Flow_1"] = flowData["Flow_1"];
                    //_currentData["1214.PLC1.DisData.Flow.Flow_2"] = flowData["Flow_2"];
                    //_currentData["1214.PLC1.DisData.Flow.Flow_3"] = flowData["Flow_3"];
                }
                else
                {
                    // 系统错误或连接失败，不读取流量数据，显示为不可用
                    UpdateUI(() =>
                    {
                        string errorStatus = plc1Status.IsConnected ? "系统错误" : "连接断开";
                        txtFlow1.Text = $"流量1: {errorStatus}";
                        txtFlow2.Text = $"流量2: {errorStatus}";
                        //_flow3Label.Text = $"流量3: {errorStatus}";

                        txtFlow1.ForeColor = Color.Red;
                        txtFlow1.ForeColor = Color.Red;
                        //_flow3Label.ForeColor = Color.Red;
                    });

                    LogMessage($"系统状态异常，跳过流量数据读取 - {plc1Status.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"读取数据异常: {ex.Message}");

                // 更新UI显示错误状态
                UpdateUI(() =>
                {
                    //_systemStatusLabel.Text = "系统状态: 读取失败";
                    //_systemStatusLabel.ForeColor = Color.Red;
                    txtFlow1.Text = "流量1: 读取失败";
                    txtFlow2.Text = "流量2: 读取失败";
                    //_flow3Label.Text = "流量3: 读取失败";
                });
            }
        }

        /// <summary>
        /// 并行读取流量数据
        /// </summary>
        private async Task<Dictionary<string, float>> ReadFlowDataAsync()
        {
            var flowResults = new Dictionary<string, float>();

            try
            {
                // 并行读取所有流量数据
                var flow1Task = _opcService.ReadFloatAsync("1214.PLC1.DisData.Flow.Flow_1");
                var flow2Task = _opcService.ReadFloatAsync("1214.PLC1.DisData.Flow.Flow_2");
                var flow3Task = _opcService.ReadFloatAsync("1214.PLC1.DisData.Flow.Flow_3");

                // 等待所有读取任务完成
                await Task.WhenAll(flow1Task, flow2Task, flow3Task);

                flowResults["Flow_1"] = await flow1Task;
                flowResults["Flow_2"] = await flow2Task;
                flowResults["Flow_3"] = await flow3Task;
            }
            catch (Exception ex)
            {
                LogMessage($"读取流量数据失败: {ex.Message}");

                // 返回默认值
                flowResults["Flow_1"] = 0f;
                flowResults["Flow_2"] = 0f;
                flowResults["Flow_3"] = 0f;
            }

            return flowResults;
        }
        #endregion

        #region 控件委托方法绑定
        private void SetupButtonControl(ButtonControl control, string controlAddress)
        {
           
            control.ControlAddress = controlAddress;            
            control.WriteIntAddressMethod = MyS7Connect_WriteIntAsync;
            control.ReadIntAddressMethod = MyS7Connect_ReadIntAsync;
            control.WriteBoolAddressMethod = MyS7Connect_WriteBoolAsync;
            control.ReadBoolAddressMethod = MyS7Connect_ReadBoolAsync;
        }
       
        /// <summary>
        /// 设置指示灯按钮控件的PLC地址和委托方法
        /// </summary>
        /// <param name="control">控件实例</param>
        /// <param name="controlAddress">控制地址</param>
        /// <param name="signalAAddress">SignalA读取地址</param>
        /// <param name="signalBAddress">SignalB读取地址</param>
        private void SetupIndicatorButton(IndicatorButtonControl control, string controlAddress, string signalAAddress, string signalBAddress)
        {
            // 设置控制地址
            control.ControlAddress = controlAddress;
            
            // 设置SignalA和SignalB读取地址
            control.SignalAAddress = signalAAddress;
            control.SignalBAddress = signalBAddress;
            
            // 绑定您的MyS7Connect库中的实际异步方法
            control.WriteAddressMethod = MyS7Connect_WriteBoolAsync;
            control.ReadAddressMethod = MyS7Connect_ReadBoolAsync;
        }
        
        /// <summary>
        /// 设置数值输入控件的委托方法
        /// </summary>
        /// <param name="control">数值输入控件实例</param>
        private void SetupNumericInputControl(NumericInputControl control,string setValueAddress ,string feedbackAddress)
        {
            control.SetValueAddress = setValueAddress;
            control.FeedbackAddress = feedbackAddress;
            // 绑定浮点数读写方法
            control.WriteFloatAddressMethod = MyS7Connect_FloatWriteAsync;
            control.ReadFloatAddressMethod = MyS7Connect_ReadFloatAsync;
        }
        
        /// <summary>
        /// MyS7Connect库的异步写入方法包装器
        /// </summary>
        /// <param name="adrName">地址名称</param>
        /// <param name="value">要写入的值</param>
        /// <returns></returns>
        private async Task MyS7Connect_WriteBoolAsync(string adrName, bool value)
        {
           
            try
            {
               if (!_opcService.CanSafelyReadPlcData("PLC1"))  return;
               await _opcService.WriteBoolAsync(adrName, value);
             
                // 记录日志
                Console.WriteLine($"MyS7Connect BoolWrite: {adrName} = {value}");
            }
            catch (Exception ex)
            {
                // 错误处理
                Console.WriteLine($"MyS7Connect BoolWrite 错误: {ex.Message}");
                this.BeginInvoke(new Action(() => {
                    labelStatus.Text = $"写入失败: {adrName} - {ex.Message}";
                }));
                throw;
            }
        }
        
        /// <summary>
        /// MyS7Connect库的异步读取方法包装器
        /// </summary>
        /// <param name="adrName">地址名称</param>
        /// <returns>读取的值</returns>
        private async Task<bool> MyS7Connect_ReadBoolAsync(string adrName)
        {
            try
            {
                if (!_opcService.CanSafelyReadPlcData("PLC1")) return false;
                bool result;
                result = await _opcService.ReadBoolAsync( adrName);
              
                // 记录日志
                Console.WriteLine($"MyS7Connect ReadBool: {adrName} = {result}");
                
                return result;
            }
            catch (Exception ex)
            {
                // 错误处理
                Console.WriteLine($"MyS7Connect ReadBool 错误: {ex.Message}");
                this.BeginInvoke(new Action(() => {
                    labelStatus.Text = $"读取失败: {adrName} - {ex.Message}";
                }));
                return false; // 默认返回值
            }
        }

        /// <summary>
        /// MyS7Connect库的异步写入方法包装器
        /// </summary>
        /// <param name="adrName">地址名称</param>
        /// <param name="value">要写入的值</param>
        /// <returns></returns>
        private async Task MyS7Connect_WriteIntAsync(string adrName, Int16 value)
        {
            try
            {
                if (!_opcService.CanSafelyReadPlcData("PLC1")) return;
                await _opcService.WriteInt16Async( adrName, value);
             
                // 记录日志
                Console.WriteLine($"MyS7Connect BoolWrite: {adrName} = {value}");
            }
            catch (Exception ex)
            {
                // 错误处理
                Console.WriteLine($"MyS7Connect BoolWrite 错误: {ex.Message}");
                this.BeginInvoke(new Action(() => {
                    labelStatus.Text = $"写入失败: {adrName} - {ex.Message}";
                }));
                throw;
            }
        }

        /// <summary>
        /// MyS7Connect库的异步读取方法包装器
        /// </summary>
        /// <param name="adrName">地址名称</param>
        /// <returns>读取的值</returns>
        private async Task<Int16> MyS7Connect_ReadIntAsync(string adrName)
        {
            try
            {
                
                Int16 result=0;
                if (!_opcService.CanSafelyReadPlcData("PLC1")) return result;
                result = await _opcService.ReadInt16Async( adrName);
             
                // 记录日志
                Console.WriteLine($"MyS7Connect ReadBool: {adrName} = {result}");

                return result;
            }
            catch (Exception ex)
            {
                // 错误处理
                Console.WriteLine($"MyS7Connect ReadBool 错误: {ex.Message}");
                this.BeginInvoke(new Action(() => {
                    labelStatus.Text = $"读取失败: {adrName} - {ex.Message}";
                }));
                return 0; // 默认返回值
            }
        }


        /// <summary>
        /// MyS7Connect库的异步浮点数写入方法包装器
        /// </summary>
        /// <param name="adrName">地址名称</param>
        /// <param name="value">要写入的浮点数值</param>
        /// <returns></returns>
        private async Task MyS7Connect_FloatWriteAsync(string adrName, double value)
        {
            try
            {
                if (!_opcService.CanSafelyReadPlcData("PLC1")) return ;
                await _opcService.WriteFloatAsync(adrName, (float)value);
                
                // 记录日志
                Console.WriteLine($"MyS7Connect FloatWrite: {adrName} = {value}");
            }
            catch (Exception ex)
            {
                // 错误处理
                Console.WriteLine($"MyS7Connect FloatWrite 错误: {ex.Message}");
                this.BeginInvoke(new Action(() => {
                    labelStatus.Text = $"浮点数写入失败: {adrName} - {ex.Message}";
                }));
                throw;
            }
        }
        
        /// <summary>
        /// MyS7Connect库的异步浮点数读取方法包装器
        /// </summary>
        /// <param name="adrName">地址名称</param>
        /// <returns>读取的浮点数值</returns>
        private async Task<double> MyS7Connect_ReadFloatAsync(string adrName)
        {
            try
            {
                if (!_opcService.CanSafelyReadPlcData("PLC1")) return 0f;
                float result = await _opcService.ReadFloatAsync( adrName);
                // 记录日志
                Console.WriteLine($"MyS7Connect ReadFloat: {adrName} = {result}");
                return (double)result;
            }
            catch (Exception ex)
            {
                // 错误处理
                Console.WriteLine($"MyS7Connect ReadFloat 错误: {ex.Message}");
                this.BeginInvoke(new Action(() => {
                    labelStatus.Text = $"浮点数读取失败: {adrName} - {ex.Message}";
                }));
                return 0.0; // 默认返回值
            }
        }
        #endregion


        /// <summary>
        /// 更新所有控件的信号状态（SignalA和SignalB）（异步）
        /// </summary>
        private async Task UpdateAllSignalStatesAsync()
        {
            //List<Task> updateTasks = new List<Task>();

            //foreach (Control control in this.Controls)
            //{
            //    if (control is GroupBox groupBox)
            //    {
            //        foreach (Control childControl in groupBox.Controls)
            //        {
            //            if (childControl is IndicatorButtonControl indicatorButton)
            //            {
            //                updateTasks.Add(indicatorButton.UpdateAllSignalsFromAddressAsync());
            //            }
            //            if (childControl is ButtonControl buttonControl)
            //            {
            //                updateTasks.Add(buttonControl.UpdateAllSignalsFromAddressAsync());
            //            }
            //        }
            //    }
            //}
            //await Task.WhenAll(updateTasks);

            if (!_opcService.CanSafelyReadPlcData("PLC1")) return;
            var tasks = controls.Select(async ctrl =>
            {
                if (ctrl is IndicatorButtonControl i) await i.UpdateAllSignalsFromAddressAsync();
                else if (ctrl is ButtonControl b) await b.UpdateAllSignalsFromAddressAsync();
            }).ToList();

            await Task.WhenAll(tasks);
        }   

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                simulationTimer?.Dispose();
                signalBUpdateTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            numericInput3.StepValue = Convert.ToSingle(textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (OpcUa.objUa.Connect == OpcStatus.Connected)
            {
                MessageBox.Show("连接成功！");

            }
            else
            {

                MessageBox.Show("连接失败！");

            }
        }

        private void UpdateUI(Action action)
        {
            // 如果窗体还未初始化或已释放，不执行UI操作
            if (this.IsDisposed || !this.IsHandleCreated)
                return;

            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(action);
                }
                catch (ObjectDisposedException)
                {
                    // 窗体已释放，忽略操作
                }
                catch (InvalidOperationException)
                {
                    // 窗体句柄未创建，忽略操作
                }
            }
            else
            {
                try
                {
                    action();
                }
                catch (ObjectDisposedException)
                {
                    // 控件已释放，忽略操作
                }
            }
        }


        /// <summary>
        /// 记录日志消息
        /// </summary>
        private void LogMessage(string message)
        {
            // 如果控件还未初始化，使用控制台输出
            //if (_logTextBox == null)
            //{
            //    string timeStamp = DateTime.Now.ToString("HH:mm:ss.fff");
            //    string logEntry = $"[{timeStamp}] {message}";
            //    System.Diagnostics.Debug.WriteLine(logEntry);
            //    Console.WriteLine(logEntry);
            //    return;
            //}

            //UpdateUI(() =>
            //{
            //    string timeStamp = DateTime.Now.ToString("HH:mm:ss.fff");
            //    string logEntry = $"[{timeStamp}] {message}";

            //    _logTextBox.AppendText(logEntry + Environment.NewLine);
            //    _logTextBox.SelectionStart = _logTextBox.Text.Length;
            //    _logTextBox.ScrollToCaret();

            //    // 限制日志长度，避免内存占用过大
            //    if (_logTextBox.Lines.Length > 100)
            //    {
            //        var lines = _logTextBox.Lines.Skip(20).ToArray();
            //        _logTextBox.Lines = lines;
            //    }
            //});
        }
    }
}
