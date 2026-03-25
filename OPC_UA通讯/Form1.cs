using System;
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

namespace OPC_UA通讯
{
    public partial class Form1 : Form
    {
        // OPC客户端
        private OpcClient _opcClient;

        // OPC分组
        private string _groupName = "MainGroup";

        // 数据存储
        private DataTable _dataTable;

        // 数据缓存
        private Dictionary<string, object> _dataCache = new Dictionary<string, object>();

        // 定义OPC项目标签
        private string[] _intTags = new string[5];
        private string[] _boolTags = new string[5];
        private string[] _floatTags = new string[10];
        private string[] _allTags = new string[20];

        // UI更新计时器
        private System.Windows.Forms.Timer _uiUpdateTimer;

        // 用于停止读取循环的取消标记
        private CancellationTokenSource _cancellationTokenSource;

        // 用于线程同步的对象
        private readonly object _lockObject = new object();

        // 连接状态标志
        private bool _isConnected = false;

        // TextBox数组，用于显示int值
        private TextBox[] _intTextBoxes;

        // CheckBox数组，用于显示bool值
        private CheckBox[] _boolCheckBoxes;
        public Form1()
        {
            InitializeComponent();
            InitializeComponent1();
            // 初始化控件数组
            _intTextBoxes = new TextBox[] { txtInt1, txtInt2, txtInt3, txtInt4, txtInt5 };
            _boolCheckBoxes = new CheckBox[] { chkBool1, chkBool2, chkBool3, chkBool4, chkBool5 };

            // 初始化数据表
            InitializeDataTable();

            // 绑定数据源
            dataGridViewValues.DataSource = _dataTable;
            dataGridViewValues.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 设置UI更新计时器
            _uiUpdateTimer = new System.Windows.Forms.Timer();
            _uiUpdateTimer.Interval = 300; // 500毫秒更新一次UI
            _uiUpdateTimer.Tick += UiUpdateTimer_Tick;

            // 初始化标签数组
            InitializeTags();

            // 设置异常处理
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;


        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }
        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            LogMessage($"UI线程异常: {e.Exception.Message}");
            MessageBox.Show($"应用程序发生错误: {e.Exception.Message}\n\n程序将继续运行，但可能需要重启。",
                "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // 全局非UI线程异常处理
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            string errorMsg = ex != null ? ex.Message : "未知错误";

            LogMessage($"应用程序致命错误: {errorMsg}");
            MessageBox.Show($"应用程序发生严重错误: {errorMsg}\n\n程序需要关闭。",
                "严重错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void InitializeTags()
        {
            // 初始化标签名称，根据实际Kepserver配置修改
            for (int i = 0; i < 5; i++)
            {
                _intTags[i] = $"1200.PLC1.Int{i + 1}";
                _boolTags[i] = $"1200.PLC1.Bool{i + 1}";
            }

            for (int i = 0; i < 10; i++)
            {
                _floatTags[i] = $"1200.PLC1.Float{i + 1}";
            }

            // 合并所有标签到一个数组中，便于批量读取
            int index = 0;
            foreach (var tag in _intTags)
            {
                _allTags[index++] = tag;
            }

            foreach (var tag in _boolTags)
            {
                _allTags[index++] = tag;
            }

            foreach (var tag in _floatTags)
            {
                _allTags[index++] = tag;
            }
        }

        private void InitializeDataTable()
        {
            _dataTable = new DataTable();
            _dataTable.Columns.Add("Timestamp", typeof(DateTime));

            // 添加Float数据列
            for (int i = 0; i < 10; i++)
            {
                _dataTable.Columns.Add($"Float{i + 1}", typeof(float));
            }

            // 添加一行初始数据
            AddNewRow();
        }

        private void AddNewRow()
        {
            DataRow newRow = _dataTable.NewRow();
            newRow["Timestamp"] = DateTime.Now;

            // 初始化所有Float值为0
            for (int i = 0; i < 10; i++)
            {
                newRow[$"Float{i + 1}"] = 0.0f;
            }

            _dataTable.Rows.Add(newRow);

            // 保持最多显示100行，防止内存占用过大
            if (_dataTable.Rows.Count > 100)
            {
                _dataTable.Rows.RemoveAt(0);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_isConnected)
                {
                    // 获取用户输入的OPC服务器地址
                    string serverUrl = txtServerUrl.Text.Trim();
                    if (string.IsNullOrEmpty(serverUrl))
                    {
                        serverUrl = "opc.tcp://127.0.0.1:49320";
                    }

                    // 创建OPC客户端并连接
                     _opcClient = new OpcClient(new Uri(serverUrl));

                    if (_opcClient.Connect == OpcStatus.Connected)
                    {
                        MessageBox.Show("连接成功！");

                    }
                    else
                    {

                        MessageBox.Show("连接失败！");

                    }

                    // 初始化缓存
                    foreach (string tag in _allTags)
                    {
                        if (tag.Contains("Int") || tag.Contains("Float"))
                        {
                            _dataCache[tag] = 0;
                        }
                        else if (tag.Contains("Bool"))
                        {
                            _dataCache[tag] = false;
                        }
                    }

                    //// 添加OPC项目到分组
                    //string errorMsg;
                    //foreach (string tag in _allTags)
                    //{
                    //    _opcClient.AddItems(_groupName, tag, out errorMsg);
                    //    if (!string.IsNullOrEmpty(errorMsg))
                    //    {
                    //        LogMessage($"添加项目 {tag} 失败: {errorMsg}");
                    //    }
                    //}

                    // 创建取消标记
                    _cancellationTokenSource = new CancellationTokenSource();

                    // 启动后台读取任务
                    StartOpcReading(_cancellationTokenSource.Token);

                    // 启动UI更新计时器
                    _uiUpdateTimer.Start();

                    // 更新状态
                    _isConnected = true;
                    btnConnect.Text = "断开连接";
                    lblStatus.Text = "已连接";
                    lblStatus.ForeColor = Color.Green;

                    // 记录日志
                    LogMessage($"已连接到OPC服务器: {serverUrl}");
                }
                else
                {
                    // 断开连接
                    DisconnectOpc();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"连接OPC服务器失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogMessage($"连接失败: {ex.Message}");
            }
        }

        private void DisconnectOpc()
        {
            try
            {
                // 停止数据读取
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }

                _uiUpdateTimer.Stop();

                // 断开OPC连接
                if (_opcClient != null)
                {
                    //_opcClient.Disconnect();
                    _opcClient = null;
                }

                // 更新状态
                _isConnected = false;
                btnConnect.Text = "连接";
                lblStatus.Text = "未连接";
                lblStatus.ForeColor = Color.Red;

                // 记录日志
                LogMessage("已断开OPC服务器连接");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"断开连接时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogMessage($"断开连接错误: {ex.Message}");
            }
        }

        private async void StartOpcReading(CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                try
                {
                    // 循环读取数据，直到取消
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            // 批量读取所有数据点
                            List<OpcItemValue> values = _opcClient.Read(_allTags);

                            // 处理读取结果
                            lock (_lockObject)
                            {
                                foreach (var val in values)
                                {
                                    try
                                    {
                                        // 使用反射获取ItemName属性，如果存在的话
                                        var itemNameProp = val.GetType().GetProperty("ItemName");
                                        string itemName = "Unknown";

                                        if (itemNameProp != null)
                                        {
                                            itemName = itemNameProp.GetValue(val)?.ToString();
                                        }
                                        else
                                        {
                                            // 如果ItemName不直接可用，尝试根据索引对应标签名
                                            int index = values.IndexOf(val);
                                            if (index >= 0 && index < _allTags.Length)
                                            {
                                                itemName = _allTags[index];
                                            }
                                        }

                                        // 检查数据有效性
                                        if (val.Value != null && val.Quality == "Good")
                                        {
                                            // 根据数据类型处理
                                            if (itemName.Contains("Int"))
                                            {
                                                _dataCache[itemName] = Convert.ToInt32(val.Value);
                                            }
                                            else if (itemName.Contains("Bool"))
                                            {
                                                _dataCache[itemName] = Convert.ToBoolean(val.Value);
                                            }
                                            else if (itemName.Contains("Float"))
                                            {
                                                _dataCache[itemName] = Convert.ToSingle(val.Value);
                                            }
                                        }
                                        else
                                        {
                                            // 数据读取异常，设置为默认值0
                                            if (itemName.Contains("Int") || itemName.Contains("Float"))
                                            {
                                                _dataCache[itemName] = 0;
                                            }
                                            else if (itemName.Contains("Bool"))
                                            {
                                                _dataCache[itemName] = false;
                                            }

                                            // 记录无效的数据质量
                                            if (val.Quality != "Good")
                                            {
                                                LogMessage($"数据点 {itemName} 质量不佳: {val.Quality}");
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LogMessage($"处理数据点失败: {ex.Message}");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // 全局读取异常处理
                            LogMessage($"批量读取OPC数据失败: {ex.Message}");

                            // 所有数据置零
                            lock (_lockObject)
                            {
                                foreach (string tag in _allTags)
                                {
                                    if (tag.Contains("Int") || tag.Contains("Float"))
                                    {
                                        _dataCache[tag] = 0;
                                    }
                                    else if (tag.Contains("Bool"))
                                    {
                                        _dataCache[tag] = false;
                                    }
                                }
                            }

                            // 如果连续失败，考虑重新连接
                            if (_isConnected)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    // 尝试重新连接逻辑放在主线程执行
                                    LogMessage("尝试重新连接OPC服务器...");

                                    // 这里可以添加重连逻辑，但现在仅提示用户
                                    MessageBox.Show("OPC连接异常，请检查服务器状态后重试", "连接错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    DisconnectOpc();
                                }));

                                // 退出循环
                                break;
                            }
                        }

                        // 等待100毫秒后再次读取
                        await Task.Delay(100, cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    // 操作被取消，正常退出
                    LogMessage("OPC数据读取任务已取消");
                }
                catch (Exception ex)
                {
                    // 其他未预期的异常
                    LogMessage($"OPC读取任务异常: {ex.Message}");
                }
            }, cancellationToken);
        }

        private void UiUpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                lock (_lockObject)
                {
                    // 更新DataGridView的最后一行，显示Float数据
                    if (_dataTable.Rows.Count > 0)
                    {
                        DataRow lastRow = _dataTable.Rows[_dataTable.Rows.Count - 1];
                        lastRow["Timestamp"] = DateTime.Now;

                        for (int i = 0; i < _floatTags.Length; i++)
                        {
                            if (_dataCache.ContainsKey(_floatTags[i]))
                            {
                                lastRow[$"Float{i + 1}"] = _dataCache[_floatTags[i]];
                            }
                        }
                    }

                    // 保持滚动到最后一行
                    if (dataGridViewValues.Rows.Count > 0)
                    {
                        dataGridViewValues.FirstDisplayedScrollingRowIndex = dataGridViewValues.Rows.Count - 1;
                    }

                    // 更新TextBox显示Int数据
                    for (int i = 0; i < _intTags.Length; i++)
                    {
                        if (_dataCache.ContainsKey(_intTags[i]))
                        {
                            _intTextBoxes[i].Text = _dataCache[_intTags[i]].ToString();
                        }
                    }

                    // 更新CheckBox显示Bool数据
                    for (int i = 0; i < _boolTags.Length; i++)
                    {
                        if (_dataCache.ContainsKey(_boolTags[i]))
                        {
                            _boolCheckBoxes[i].Checked = (bool)_dataCache[_boolTags[i]];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"UI更新错误: {ex.Message}");
            }
        }

        private void LogMessage(string message)
        {
            try
            {
                // 在UI线程上更新日志
                if (InvokeRequired)
                {
                    BeginInvoke(new Action<string>(LogMessage), message);
                    return;
                }

                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
                listBoxLog.Items.Add(logEntry);

                // 保持滚动到最新日志
                listBoxLog.TopIndex = listBoxLog.Items.Count - 1;

                // 限制日志条数防止内存占用过大
                if (listBoxLog.Items.Count > 1000)
                {
                    listBoxLog.Items.RemoveAt(0);
                }

                // 同时写入文件日志
                System.IO.File.AppendAllText("OpcClient.log", logEntry + Environment.NewLine);
            }
            catch
            {
                // 日志记录异常不处理，防止递归
            }
        }

        private void btnNewRow_Click(object sender, EventArgs e)
        {
            // 添加新行记录
            AddNewRow();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            // 清空日志
            listBoxLog.Items.Clear();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 关闭窗口时断开连接
            DisconnectOpc();
        }
        // 下面是初始化组件方法，包含界面控件创建代码
        private void InitializeComponent1()
        {
            this.panelHeader = new System.Windows.Forms.Panel();
            this.txtServerUrl = new System.Windows.Forms.TextBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBoxFloat = new System.Windows.Forms.GroupBox();
            this.dataGridViewValues = new System.Windows.Forms.DataGridView();
            this.btnNewRow = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBoxInt = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelInt = new System.Windows.Forms.TableLayoutPanel();
            this.lblInt1 = new System.Windows.Forms.Label();
            this.lblInt2 = new System.Windows.Forms.Label();
            this.lblInt3 = new System.Windows.Forms.Label();
            this.lblInt4 = new System.Windows.Forms.Label();
            this.lblInt5 = new System.Windows.Forms.Label();
            this.txtInt1 = new System.Windows.Forms.TextBox();
            this.txtInt2 = new System.Windows.Forms.TextBox();
            this.txtInt3 = new System.Windows.Forms.TextBox();
            this.txtInt4 = new System.Windows.Forms.TextBox();
            this.txtInt5 = new System.Windows.Forms.TextBox();
            this.groupBoxBool = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelBool = new System.Windows.Forms.TableLayoutPanel();
            this.lblBool1 = new System.Windows.Forms.Label();
            this.lblBool2 = new System.Windows.Forms.Label();
            this.lblBool3 = new System.Windows.Forms.Label();
            this.lblBool4 = new System.Windows.Forms.Label();
            this.lblBool5 = new System.Windows.Forms.Label();
            this.chkBool1 = new System.Windows.Forms.CheckBox();
            this.chkBool2 = new System.Windows.Forms.CheckBox();
            this.chkBool3 = new System.Windows.Forms.CheckBox();
            this.chkBool4 = new System.Windows.Forms.CheckBox();
            this.chkBool5 = new System.Windows.Forms.CheckBox();
            this.groupBoxLog = new System.Windows.Forms.GroupBox();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.btnClearLog = new System.Windows.Forms.Button();

            // 省略其他组件设置代码...

            // 示例部分组件配置 
            this.Text = "Kepserver数据监视器";
            this.Size = new System.Drawing.Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);

            // 添加其他控件配置...
            // 面板配置
            this.panelHeader.Dock = DockStyle.Top;
            this.panelHeader.Height = 60;

            // 服务器URL输入框
            this.txtServerUrl.Location = new Point(70, 15);
            this.txtServerUrl.Size = new Size(350, 23);
            this.txtServerUrl.Text = "opc.tcp://127.0.0.1:49320";

            // 服务器标签
            this.lblServer.Location = new Point(10, 18);
            this.lblServer.Size = new Size(60, 20);
            this.lblServer.Text = "服务器:";

            // 状态标签
            this.lblStatus.Location = new Point(550, 18);
            this.lblStatus.Size = new Size(100, 20);
            this.lblStatus.Text = "未连接";
            this.lblStatus.ForeColor = Color.Red;

            // 连接按钮
            this.btnConnect.Location = new Point(450, 14);
            this.btnConnect.Size = new Size(80, 25);
            this.btnConnect.Text = "连接";
            this.btnConnect.Click += new EventHandler(this.btnConnect_Click);

          

            // 添加控件到面板
            this.panelHeader.Controls.Add(this.txtServerUrl);
            this.panelHeader.Controls.Add(this.lblServer);
            this.panelHeader.Controls.Add(this.lblStatus);
            this.panelHeader.Controls.Add(this.btnConnect);

            // 主分隔容器
            this.splitContainer1.Dock = DockStyle.Fill;
            this.splitContainer1.Orientation = Orientation.Vertical;
            this.splitContainer1.SplitterDistance = 350;

            // Float数据GroupBox和DataGridView
            this.groupBoxFloat.Text = "Float数据实时监控";
            this.groupBoxFloat.Dock = DockStyle.Fill;

            this.dataGridViewValues.Dock = DockStyle.Fill;
            this.dataGridViewValues.AllowUserToAddRows = false;
            this.dataGridViewValues.ReadOnly = true;

            this.btnNewRow.Text = "添加新行";
            this.btnNewRow.Dock = DockStyle.Bottom;
            this.btnNewRow.Height = 30;
            this.btnNewRow.Click += new EventHandler(this.btnNewRow_Click);

            this.groupBoxFloat.Controls.Add(this.dataGridViewValues);
            this.groupBoxFloat.Controls.Add(this.btnNewRow);

            // 下部分隔容器
            this.splitContainer2.Dock = DockStyle.Fill;
            this.splitContainer2.SplitterDistance = 350;

            // Int数据GroupBox
            this.groupBoxInt.Text = "Int数据显示";
            this.groupBoxInt.Dock = DockStyle.Fill;

            // Int数据表格布局
            this.tableLayoutPanelInt.Dock = DockStyle.Fill;
            this.tableLayoutPanelInt.ColumnCount = 2;
            this.tableLayoutPanelInt.RowCount = 5;

            // 添加Int标签和文本框
            this.lblInt1.Text = "Int1:";
            this.lblInt2.Text = "Int2:";
            this.lblInt3.Text = "Int3:";
            this.lblInt4.Text = "Int4:";
            this.lblInt5.Text = "Int5:";

            this.txtInt1.ReadOnly = true;
            this.txtInt2.ReadOnly = true;
            this.txtInt3.ReadOnly = true;
            this.txtInt4.ReadOnly = true;
            this.txtInt5.ReadOnly = true;

            this.tableLayoutPanelInt.Controls.Add(this.lblInt1, 0, 0);
            this.tableLayoutPanelInt.Controls.Add(this.txtInt1, 1, 0);
            this.tableLayoutPanelInt.Controls.Add(this.lblInt2, 0, 1);
            this.tableLayoutPanelInt.Controls.Add(this.txtInt2, 1, 1);
            this.tableLayoutPanelInt.Controls.Add(this.lblInt3, 0, 2);
            this.tableLayoutPanelInt.Controls.Add(this.txtInt3, 1, 2);
            this.tableLayoutPanelInt.Controls.Add(this.lblInt4, 0, 3);
            this.tableLayoutPanelInt.Controls.Add(this.txtInt4, 1, 3);
            this.tableLayoutPanelInt.Controls.Add(this.lblInt5, 0, 4);
            this.tableLayoutPanelInt.Controls.Add(this.txtInt5, 1, 4);

            this.groupBoxInt.Controls.Add(this.tableLayoutPanelInt);

            // Bool数据GroupBox
            this.groupBoxBool.Text = "Bool数据显示";
            this.groupBoxBool.Dock = DockStyle.Fill;

            // Bool数据表格布局
            this.tableLayoutPanelBool.Dock = DockStyle.Fill;
            this.tableLayoutPanelBool.ColumnCount = 2;
            this.tableLayoutPanelBool.RowCount = 5;

            // 添加Bool标签和复选框
            this.lblBool1.Text = "Bool1:";
            this.lblBool2.Text = "Bool2:";
            this.lblBool3.Text = "Bool3:";
            this.lblBool4.Text = "Bool4:";
            this.lblBool5.Text = "Bool5:";

            this.chkBool1.Enabled = false;
            this.chkBool2.Enabled = false;
            this.chkBool3.Enabled = false;
            this.chkBool4.Enabled = false;
            this.chkBool5.Enabled = false;

            this.tableLayoutPanelBool.Controls.Add(this.lblBool1, 0, 0);
            this.tableLayoutPanelBool.Controls.Add(this.chkBool1, 1, 0);
            this.tableLayoutPanelBool.Controls.Add(this.lblBool2, 0, 1);
            this.tableLayoutPanelBool.Controls.Add(this.chkBool2, 1, 1);
            this.tableLayoutPanelBool.Controls.Add(this.lblBool3, 0, 2);
            this.tableLayoutPanelBool.Controls.Add(this.chkBool3, 1, 2);
            this.tableLayoutPanelBool.Controls.Add(this.lblBool4, 0, 3);
            this.tableLayoutPanelBool.Controls.Add(this.chkBool4, 1, 3);
            this.tableLayoutPanelBool.Controls.Add(this.lblBool5, 0, 4);
            this.tableLayoutPanelBool.Controls.Add(this.chkBool5, 1, 4);

            this.groupBoxBool.Controls.Add(this.tableLayoutPanelBool);

            // 日志GroupBox
            this.groupBoxLog.Text = "运行日志";
            this.groupBoxLog.Dock = DockStyle.Bottom;
            this.groupBoxLog.Height = 150;

            this.listBoxLog.Dock = DockStyle.Fill;

            this.btnClearLog.Text = "清空日志";
            this.btnClearLog.Dock = DockStyle.Bottom;
            this.btnClearLog.Height = 25;
            this.btnClearLog.Click += new EventHandler(this.btnClearLog_Click);

            this.groupBoxLog.Controls.Add(this.listBoxLog);
            this.groupBoxLog.Controls.Add(this.btnClearLog);

            // 添加控件到分隔容器
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxFloat);
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2.Controls.Add(this.groupBoxLog);

            this.splitContainer2.Panel1.Controls.Add(this.groupBoxInt);
            this.splitContainer2.Panel2.Controls.Add(this.groupBoxBool);

            // 添加控件到主窗体
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panelHeader);
        }
        // 主窗体控件
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.TextBox txtServerUrl;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBoxFloat;
        private System.Windows.Forms.DataGridView dataGridViewValues;
        private System.Windows.Forms.Button btnNewRow;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBoxInt;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelInt;
        private System.Windows.Forms.Label lblInt1;
        private System.Windows.Forms.Label lblInt2;
        private System.Windows.Forms.Label lblInt3;
        private System.Windows.Forms.Label lblInt4;
        private System.Windows.Forms.Label lblInt5;
        private System.Windows.Forms.TextBox txtInt1;
        private System.Windows.Forms.TextBox txtInt2;
        private System.Windows.Forms.TextBox txtInt3;
        private System.Windows.Forms.TextBox txtInt4;
        private System.Windows.Forms.TextBox txtInt5;
        private System.Windows.Forms.GroupBox groupBoxBool;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBool;
        private System.Windows.Forms.Label lblBool1;
        private System.Windows.Forms.Label lblBool2;
        private System.Windows.Forms.Label lblBool3;
        private System.Windows.Forms.Label lblBool4;
        private System.Windows.Forms.Label lblBool5;
        private System.Windows.Forms.CheckBox chkBool1;
        private System.Windows.Forms.CheckBox chkBool2;
        private System.Windows.Forms.CheckBox chkBool3;
        private System.Windows.Forms.CheckBox chkBool4;
        private System.Windows.Forms.CheckBox chkBool5;
        private System.Windows.Forms.GroupBox groupBoxLog;
        private System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.Button btnClearLog;

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}
