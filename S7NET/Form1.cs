
using S7NET.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S7NET
{
    public partial class Form1 : Form
    {
        private readonly MultiPlcServiceManager _multiPlcService;
        private System.Windows.Forms.Timer _readTimer;
        private CancellationTokenSource _connectionCancellationTokenSource;

        public Form1()
        {
            InitializeComponent();
            _multiPlcService = MultiPlcServiceManager.Instance;

            // 订阅多PLC通讯事件
            _multiPlcService.ConnectionStatusChanged += OnConnectionStatusChanged;
            _multiPlcService.CommunicationError += OnCommunicationError;
        }



        private async void btnConnect_Click(object sender, EventArgs e)
        {
            // 如果正在连接，则取消连接
            if (_connectionCancellationTokenSource != null)
            {
                _connectionCancellationTokenSource.Cancel();
                return;
            }

            try
            {
                // 创建新的取消令牌
                _connectionCancellationTokenSource = new CancellationTokenSource();

                btnConnect.Text = "取消连接";
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;

                UpdateStatus("正在连接PLC...");
                AddLog("开始连接所有PLC...");

                // 连接所有PLC，支持取消
                var connectResults = await _multiPlcService.ConnectAllPlcsAsync(_connectionCancellationTokenSource.Token);

                // 检查是否被取消
                if (_connectionCancellationTokenSource.Token.IsCancellationRequested)
                {
                    AddLog("连接操作已取消");
                    UpdateStatus("连接已取消");
                    return;
                }

                foreach (var result in connectResults)
                {
                    AddLog($"PLC {result.Key}: {(result.Value ? "连接成功" : "连接失败")}");
                }

                UpdatePlcStatus();

                // 如果有PLC连接成功，启动定时读取
                var connectedCount = connectResults.Values.Count(v => v);
                if (connectedCount > 0)
                {
                    UpdateStatus($"已连接 {connectedCount} 个PLC");
                    StartPeriodicReading();
                }
                else
                {
                    UpdateStatus("所有PLC连接失败");
                }
            }
            catch (OperationCanceledException)
            {
                AddLog("连接操作已取消");
                UpdateStatus("连接已取消");
            }
            catch (Exception ex)
            {
                AddLog($"连接异常: {ex.Message}");
                UpdateStatus("连接异常");
            }
            finally
            {
                // 重置按钮状态
                btnConnect.Text = "连接所有PLC";
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = true;

                // 清理取消令牌
                _connectionCancellationTokenSource?.Dispose();
                _connectionCancellationTokenSource = null;
            }
        }

        private async void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                // 如果正在连接，先取消连接操作
                if (_connectionCancellationTokenSource != null)
                {
                    _connectionCancellationTokenSource.Cancel();
                    AddLog("已取消连接操作");
                }

                btnDisconnect.Enabled = false;
                StopPeriodicReading();

                UpdateStatus("正在断开连接...");
                await _multiPlcService.DisconnectAllPlcsAsync();

                AddLog("已断开所有PLC连接");
                UpdateStatus("已断开连接");
                UpdatePlcStatus();
            }
            catch (Exception ex)
            {
                AddLog($"断开连接异常: {ex.Message}");
            }
            finally
            {
                btnDisconnect.Enabled = true;
            }
        }

        private void btnOpenReadWrite_Click(object sender, EventArgs e)
        {
            var readWriteForm = new ReadWriteForm(_multiPlcService);
            readWriteForm.Show();
        }

        private async void btnDiagnose_Click(object sender, EventArgs e)
        {
            try
            {
                btnDiagnose.Enabled = false;
                btnDiagnose.Text = "诊断中...";

                AddLog("开始PLC连接诊断...");

                // 获取所有PLC的IP地址进行诊断
                var plcIds = _multiPlcService.GetAllPlcIds();

                foreach (var plcId in plcIds)
                {
                    var plcConfig = _multiPlcService.GetPlcConfig(plcId);
                    if (plcConfig != null)
                    {
                        AddLog($"正在诊断 {plcId} ({plcConfig.IpAddress})...");

                        var diagnosticResult = await PlcConnectionDiagnostics.DiagnosePlcConnectionAsync(plcConfig.IpAddress);

                        // 显示诊断结果
                        var resultForm = new Form
                        {
                            Text = $"PLC连接诊断结果 - {plcId}",
                            Size = new Size(600, 500),
                            StartPosition = FormStartPosition.CenterParent
                        };

                        var textBox = new TextBox
                        {
                            Multiline = true,
                            ScrollBars = ScrollBars.Vertical,
                            Dock = DockStyle.Fill,
                            Font = new Font("Consolas", 9),
                            Text = diagnosticResult.ToString()
                        };

                        resultForm.Controls.Add(textBox);
                        resultForm.ShowDialog(this);

                        AddLog($"{plcId} 诊断完成");
                    }
                }

                AddLog("所有PLC诊断完成");
            }
            catch (Exception ex)
            {
                AddLog($"诊断过程中发生错误: {ex.Message}");
                MessageBox.Show($"诊断失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnDiagnose.Enabled = true;
                btnDiagnose.Text = "连接诊断";
            }
        }

        //private void StartPeriodicReading()
        //{
        //    if (_readTimer != null) return;

        //    _readTimer = new System.Windows.Forms.Timer
        //    {
        //        Interval = 10 // 每0.5秒读取一次
        //    };
        //    _readTimer.Tick += async (s, e) => await ReadDataAsync();
        //    _readTimer.Start();
        //    AddLog("开始定时读取数据");
        //}

        //private void StopPeriodicReading()
        //{
        //    if (_readTimer != null)
        //    {
        //        _readTimer.Stop();
        //        _readTimer.Dispose();
        //        _readTimer = null;
        //        AddLog("停止定时读取数据");
        //    }
        //}

        private readonly Queue<DateTime> _readTimestamps = new Queue<DateTime>();
        private readonly object _lock = new object(); // 线程安全锁
        private CancellationTokenSource _cts;
        private Task _pollingTask;
        private void StartPeriodicReading()
        {
            if (_cts != null) return; // 已启动

            _cts = new CancellationTokenSource();
            _pollingTask = Task.Run(async () =>
            {
                AddLog("开始定时读取数据（Task.Delay 循环）");

                var token = _cts.Token;
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        await ReadDataAsync();
                        lock (_lock)
                        {
                            var now = DateTime.Now;
                            _readTimestamps.Enqueue(now);

                            // 清理 2 秒前的旧数据（保留足够窗口）
                            while (_readTimestamps.Count > 0 && (now - _readTimestamps.Peek()).TotalSeconds > 1.0)
                            {
                                _readTimestamps.Dequeue();
                            }
                        }
                        // 可选：触发刷新率更新（或用独立定时器更新 UI）
                        await UpdateRefreshRateDisplayAsync();
                    }
                    catch (Exception ex)
                    {
                        
                    }

                    await Task.Delay(10, token); // ← 修改这里调整刷新率
                }
            }, _cts.Token);
        }

        private void StopPeriodicReading()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            // 可选：等待任务结束（加超时避免卡死）
            _pollingTask?.Wait(1000);
            _pollingTask = null;

            AddLog("停止定时读取数据");
        }
        private async Task ReadDataAsync()
        {
            try
            {
                // 获取所有连接的PLC
                var connectedPlcs = _multiPlcService.GetAllConnectionStatuses()
                    .Where(kvp => kvp.Value)
                    .Select(kvp => kvp.Key)
                    .ToList();

                if (connectedPlcs.Count == 0) return;

                // 读取PLC1数据
                if (connectedPlcs.Contains("PLC1"))
                {
                    try
                    {
                        var data1 = await _multiPlcService.ReadDBAsync<float>(15, 0, 2, "PLC1");
                        //var data2 = await _multiPlcService.ReadDBAsync<int>(15, 4, 1, "PLC1");
                        //var qBits = await _multiPlcService.ReadQBitsAsync(100, 0, 8, "PLC1");
                        //var iBits = await _multiPlcService.ReadIBitsAsync(0, 0, 8, "PLC1");
                        UpdatePLC1Data($"DB15.DBD0: {data1[0]:F2}","0" );
                        AddLog($"{DateTime.Now.ToString("HH:mm:ss.fff")}--{data1[0]:F2}");
                    }
                    catch (InvalidOperationException ex) when (ex.Message.Contains("PLC连接已断开"))
                    {
                        // PLC连接已断开，更新状态但不停止定时器（等待自动重连）
                        UpdatePLC1Data("连接断开", "等待重连...");
                        // 不添加日志，避免日志过多
                    }
                    catch (Exception ex)
                    {
                        // 检查是否是连接相关异常
                        var isConnectionError = ex.Message.Contains("连接") ||
                                              ex.Message.Contains("远程主机") ||
                                              ex.Message.Contains("PLC连接已断开") ||
                                              ex.GetType().Name.Contains("PlcException") ||
                                              ex.GetType().Name.Contains("SocketException");

                        if (isConnectionError)
                        {
                            UpdatePLC1Data("连接断开", "等待重连...");
                            // 不记录连接异常日志，避免日志过多
                        }
                        else
                        {
                            UpdatePLC1Data("读取失败", ex.Message.Length > 20 ? ex.Message.Substring(0, 20) + "..." : ex.Message);
                            AddLog($"PLC1读取异常: {ex.Message}");
                        }
                    }
                }

                // 读取PLC2数据
                //if (connectedPlcs.Contains("PLC2"))
                //{
                //    try
                //    {
                //        var data1 = await _multiPlcService.ReadDBAsync<float>(15, 0, 2, "PLC2");
                //        var data2 = await _multiPlcService.ReadDBAsync<int>(15, 4, 1, "PLC2");
                //        var iBits = await _multiPlcService.ReadIBitsAsync(0, 0, 26, "PLC2");
                //        UpdatePLC2Data($"DB15.DBD0: {data1[0]:F2}", $"DB15.DBD4: {data2[0]}");
                //    }
                //    catch (InvalidOperationException ex) when (ex.Message.Contains("PLC连接已断开"))
                //    {
                //        // PLC连接已断开，更新状态但不停止定时器（等待自动重连）
                //        UpdatePLC2Data("连接断开", "等待重连...");
                //        // 不添加日志，避免日志过多
                //    }
                //    catch (Exception ex)
                //    {
                //        // 检查是否是连接相关异常
                //        var isConnectionError = ex.Message.Contains("连接") ||
                //                              ex.Message.Contains("远程主机") ||
                //                              ex.Message.Contains("PLC连接已断开") ||
                //                              ex.GetType().Name.Contains("PlcException") ||
                //                              ex.GetType().Name.Contains("SocketException");

                //        if (isConnectionError)
                //        {
                //            UpdatePLC2Data("连接断开", "等待重连...");
                //            // 不记录连接异常日志，避免日志过多
                //        }
                //        else
                //        {
                //            UpdatePLC2Data("读取失败", ex.Message.Length > 20 ? ex.Message.Substring(0, 20) + "..." : ex.Message);
                //            AddLog($"PLC2读取异常: {ex.Message}");
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                AddLog($"读取数据异常: {ex.Message}");
            }
        }

        private void UpdateStatus(string status)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(UpdateStatus), status);
                return;
            }
            lblStatus.Text = $"状态: {status}";
        }

        private void UpdatePlcStatus()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdatePlcStatus));
                return;
            }

            var statuses = _multiPlcService.GetAllConnectionStatuses();

            lblPLC1Status.Text = statuses.ContainsKey("PLC1") && statuses["PLC1"] ? "已连接" : "未连接";
            lblPLC1Status.ForeColor = statuses.ContainsKey("PLC1") && statuses["PLC1"] ? Color.Green : Color.Red;

            lblPLC2Status.Text = statuses.ContainsKey("PLC2") && statuses["PLC2"] ? "已连接" : "未连接";
            lblPLC2Status.ForeColor = statuses.ContainsKey("PLC2") && statuses["PLC2"] ? Color.Green : Color.Red;
        }

        private void UpdatePLC1Data(string data1, string data2)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string, string>(UpdatePLC1Data), data1, data2);
                return;
            }
            lblPLC1Data1.Text = data1;
            lblPLC1Data2.Text = data2;
        }

        private void UpdatePLC2Data(string data1, string data2)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string, string>(UpdatePLC2Data), data1, data2);
                return;
            }
            lblPLC2Data1.Text = data1;
            lblPLC2Data2.Text = data2;
        }

        private void AddLog(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(AddLog), message);
                return;
            }

            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var logMessage = $"[{timestamp}] {message}";

            lstLog.Items.Add(logMessage);

            //保持最新的50条日志
            while (lstLog.Items.Count > 50)
            {
                lstLog.Items.RemoveAt(0);
            }

            //滚动到最新
            lstLog.TopIndex = lstLog.Items.Count - 1;
        }

        private async Task UpdateRefreshRateDisplayAsync()
        {
            int count;
            lock (_lock)
            {
                count = _readTimestamps.Count;
            }

            // 刷新率 = 最近1秒内的读取次数（单位：Hz）
            var refreshRate = count; // 因为窗口是1秒，所以直接等于次数
            Console.WriteLine($"刷新率: {refreshRate} Hz");
            //// 在 UI 线程更新 Label
            //if (labelRefreshRate.InvokeRequired)
            //{
            //    await Task.Run(() =>
            //    {
            //        labelRefreshRate.Invoke(new Action(() =>
            //        {
            //            labelRefreshRate.Text = $"刷新率: {refreshRate} Hz";
            //        }));
            //    });
            //}
            //else
            //{
            //    labelRefreshRate.Text = $"刷新率: {refreshRate} Hz";
            //}
        }
        private void OnConnectionStatusChanged(object sender, (string PlcId, bool IsConnected) args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, (string, bool)>(OnConnectionStatusChanged), sender, args);
                return;
            }

            AddLog($"[事件] PLC {args.PlcId} 连接状态: {(args.IsConnected ? "已连接" : "已断开")}");
            UpdatePlcStatus();

            // 更新状态显示
            var connectedCount = _multiPlcService.GetAllConnectionStatuses().Values.Count(v => v);
            if (connectedCount == 0)
            {
                UpdateStatus("所有PLC连接已断开，等待自动重连...");
                // 不停止定时读取，让它继续运行以便检测重连
            }
            else
            {
                UpdateStatus($"已连接 {connectedCount} 个PLC");
                // 如果有PLC重连成功，确保定时读取正在运行
                if (args.IsConnected && _readTimer == null)
                {
                    StartPeriodicReading();
                }
            }
        }

        private void OnCommunicationError(object sender, (string PlcId, string Error) args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, (string, string)>(OnCommunicationError), sender, args);
                return;
            }

            // 只记录连接断开的错误，避免日志过多
            if (args.Error.Contains("连接已断开") || args.Error.Contains("连接失败"))
            {
                AddLog($"[错误] PLC {args.PlcId}: {args.Error}");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                // 取消正在进行的连接操作
                if (_connectionCancellationTokenSource != null)
                {
                    _connectionCancellationTokenSource.Cancel();
                    _connectionCancellationTokenSource.Dispose();
                    _connectionCancellationTokenSource = null;
                }

                StopPeriodicReading();

                // 取消事件订阅
                _multiPlcService.ConnectionStatusChanged -= OnConnectionStatusChanged;
                _multiPlcService.CommunicationError -= OnCommunicationError;

                // 断开所有PLC连接
                _multiPlcService.DisconnectAllPlcsAsync().Wait(2000);
            }
            catch (Exception ex)
            {
                AddLog($"关闭窗体时异常: {ex.Message}");
            }

            base.OnFormClosing(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // 初始化多PLC通讯服务
                _multiPlcService.Initialize();

                AddLog("多PLC服务初始化完成");
                AddLog($"默认PLC: {_multiPlcService.DefaultPlcId}");
                AddLog($"可用PLC: {string.Join(", ", _multiPlcService.GetAllPlcIds())}");

                UpdateStatus("服务已初始化，点击连接按钮连接PLC");
                UpdatePlcStatus();
            }
            catch (Exception ex)
            {
                AddLog($"初始化失败: {ex.Message}");
                UpdateStatus("初始化失败");
            }
        }

       
    }
}
