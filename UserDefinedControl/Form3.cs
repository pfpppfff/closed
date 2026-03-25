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
using UserDefinedControl.OPCUA;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Timer = System.Windows.Forms.Timer;

namespace UserDefinedControl
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
       
            //InitializeControls();
        
            // 初始化取消令牌源
            _cancellationTokenSource = new CancellationTokenSource();
        }
        private Timer simulationTimer;
        private Timer signalBUpdateTimer; // 新增：更新SignalB状态的定时器
        private IOpcUaService _opcService;
        private CancellationTokenSource _cancellationTokenSource;
        private bool plcStaus = false;

        // 模拟的地址存储器（用于演示，实际使用时可以移除）
        private Dictionary<string, bool> addressStorage = new Dictionary<string, bool>();
     
        private void Form3_Load(object sender, EventArgs e)
        {
            _opcService = OpcUaServiceManager.Current;
            var  plc1Status = _opcService.GetPlcStatus("PLC1");
        }

        #region 控件初始化 刷新
        private void InitializeControls()
        {
            // 创建信号更新定时器（同时更新SignalA和SignalB）
            signalBUpdateTimer = new Timer();
            signalBUpdateTimer.Interval = 500; // 500毫秒更新一次
            signalBUpdateTimer.Tick += SignalUpdateTimer_Tick;
            signalBUpdateTimer.Start();

            // 设置窗体属性
            this.Text = "自定义指示灯按钮控件演示 - PLC地址控制版";
            this.StartPosition = FormStartPosition.CenterScreen;
         
            SetupButtonControl(buttonControl2, "1214.PLC1.PowerCtrl.Motor.item1.PC_PowerSel");
            SetupButtonControl(buttonControl4, "1214.PLC1.Val.Switch.item1.SwitchVal_Open");
          

          
            controls = GetAllUpdatableControls(this.Controls);
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
            //this.BeginInvoke(new Action(() => {
            //    labelStatus.Text = $"控件 {control.Name} 的 SignalC 状态: {value}";
            //}));
        }

        private void IndicatorButton_ButtonClick(object sender, EventArgs e)
        {
            IndicatorButtonControl control = sender as IndicatorButtonControl;
            Console.WriteLine($"控件 {control.Name} 按钮被点击");
        }

        #endregion


        PlcStatusModel plc1Status;
        PlcStatusModel plc2Status;


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
        private void SetupNumericInputControl(NumericInputControl control, string setValueAddress, string feedbackAddress)
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
                if (!_opcService.CanSafelyReadPlcData("PLC1")) return;
                await _opcService.WriteBoolAsync(adrName, value);

                // 记录日志
                Console.WriteLine($"MyS7Connect BoolWrite: {adrName} = {value}");
            }
            catch (Exception ex)
            {
                // 错误处理
                Console.WriteLine($"MyS7Connect BoolWrite 错误: {ex.Message}");
                //this.BeginInvoke(new Action(() => {
                //    labelStatus.Text = $"写入失败: {adrName} - {ex.Message}";
                //}));
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
                result = await _opcService.ReadBoolAsync(adrName);

                // 记录日志
                Console.WriteLine($"MyS7Connect ReadBool: {adrName} = {result}");

                return result;
            }
            catch (Exception ex)
            {
                // 错误处理
                Console.WriteLine($"MyS7Connect ReadBool 错误: {ex.Message}");
                //this.BeginInvoke(new Action(() => {
                //    labelStatus.Text = $"读取失败: {adrName} - {ex.Message}";
                //}));
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
                await _opcService.WriteInt16Async(adrName, value);

                // 记录日志
                Console.WriteLine($"MyS7Connect BoolWrite: {adrName} = {value}");
            }
            catch (Exception ex)
            {
                // 错误处理
                Console.WriteLine($"MyS7Connect BoolWrite 错误: {ex.Message}");
                //this.BeginInvoke(new Action(() => {
                //    labelStatus.Text = $"写入失败: {adrName} - {ex.Message}";
                //}));
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

                Int16 result = 0;
                if (!_opcService.CanSafelyReadPlcData("PLC1")) return result;
                result = await _opcService.ReadInt16Async(adrName);

                // 记录日志
                Console.WriteLine($"MyS7Connect ReadBool: {adrName} = {result}");

                return result;
            }
            catch (Exception ex)
            {
                // 错误处理
                Console.WriteLine($"MyS7Connect ReadBool 错误: {ex.Message}");
                //this.BeginInvoke(new Action(() => {
                //    labelStatus.Text = $"读取失败: {adrName} - {ex.Message}";
                //}));
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
                if (!_opcService.CanSafelyReadPlcData("PLC1")) return;
                await _opcService.WriteFloatAsync(adrName, (float)value);

                // 记录日志
                Console.WriteLine($"MyS7Connect FloatWrite: {adrName} = {value}");
            }
            catch (Exception ex)
            {
                // 错误处理
                Console.WriteLine($"MyS7Connect FloatWrite 错误: {ex.Message}");
                //this.BeginInvoke(new Action(() => {
                //    labelStatus.Text = $"浮点数写入失败: {adrName} - {ex.Message}";
                //}));
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
                float result = await _opcService.ReadFloatAsync(adrName);
                // 记录日志
                Console.WriteLine($"MyS7Connect ReadFloat: {adrName} = {result}");
                return (double)result;
            }
            catch (Exception ex)
            {
                // 错误处理
                Console.WriteLine($"MyS7Connect ReadFloat 错误: {ex.Message}");
                //this.BeginInvoke(new Action(() => {
                //    labelStatus.Text = $"浮点数读取失败: {adrName} - {ex.Message}";
                //}));
                return 0.0; // 默认返回值
            }
        }
        #endregion


        /// <summary>
        /// 更新所有控件的信号状态（SignalA和SignalB）（异步）
        /// </summary>
        private async Task UpdateAllSignalStatesAsync()
        {
            // 检查PLC连接状态，如果无法安全读取则直接更新控件为断开状态
            if (!_opcService.CanSafelyReadPlcData("PLC1"))
            {
                // 直接在UI线程上更新所有控件状态为断开状态，避免异步操作
                this.Invoke(new Action(() =>
                {
                    foreach (var ctrl in controls)
                    {
                        if (ctrl is IndicatorButtonControl indicatorButton)
                        {
                            indicatorButton.SignalA = false;
                            indicatorButton.SignalB = false;
                        }
                        else if (ctrl is ButtonControl buttonControl)
                        {
                            buttonControl.SignalB = false;
                        }
                    }
                }));
                _cancellationTokenSource.Cancel();
                return;
            }

            // 检查取消令牌是否已请求取消
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            // 使用并发控制来限制同时执行的任务数量
            var semaphore = new SemaphoreSlim(10); // 最多同时执行10个任务
            var tasks = new List<Task>();

            foreach (var ctrl in controls)
            {
                // 检查取消令牌是否已请求取消
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }

                await semaphore.WaitAsync();
                var task = Task.Run(async () =>
                {
                    try
                    {
                        // 检查取消令牌是否已请求取消
                        if (_cancellationTokenSource.IsCancellationRequested)
                        {
                            return;
                        }

                        if (ctrl is IndicatorButtonControl indicatorButton)
                        {
                            await indicatorButton.UpdateAllSignalsFromAddressAsync();
                        }
                        else if (ctrl is ButtonControl buttonControl)
                        {
                            await buttonControl.UpdateAllSignalsFromAddressAsync();
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, _cancellationTokenSource.Token);
                tasks.Add(task);
            }

            // 等待所有任务完成，但支持取消
            try
            {
                // 使用Task.WhenAny实现带取消的等待
                var allTasks = Task.WhenAll(tasks);
                var cancelTask = Task.Delay(-1, _cancellationTokenSource.Token); // 永远不会完成的任务，除非被取消
                var completedTask = await Task.WhenAny(allTasks, cancelTask);

                // 如果完成的是取消任务，说明操作被取消了
                if (completedTask == cancelTask)
                {
                    System.Diagnostics.Debug.WriteLine("更新任务被取消");
                    // 取消所有任务
                    foreach (var task in tasks)
                    {
                        if (!task.IsCompleted)
                        {
                            task.Wait(1); // 短暂等待，让任务有机会完成
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 其他异常
                System.Diagnostics.Debug.WriteLine($"更新任务异常: {ex.Message}");
            }
        }

        //protected override void Dispose(bool disposing)
        //{
        //    //if (disposing)
        //    //{
        //    //    simulationTimer?.Dispose();
        //    //    signalBUpdateTimer?.Dispose();
        //    //    _cancellationTokenSource?.Cancel();
        //    //    _cancellationTokenSource?.Dispose();
        //    //}
        //    //base.Dispose(disposing);
        //}

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.ShowDialog();
            //if (OpcUa.objUa.Connect == OpcStatus.Connected)
            //{
            //    MessageBox.Show("连接成功！");

            //}
            //else
            //{

            //    MessageBox.Show("连接失败！");

            //}
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
