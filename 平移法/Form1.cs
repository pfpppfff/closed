using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Timer = System.Windows.Forms.Timer;

namespace 平移法
{
    public partial class Form1 : Form
    {
        private int _timesCollect;
        private float[] _dataList;

        private TimeSeriesChart chart;
        private Timer timer;
        private int counter = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MovedgvIsStale();
            timeinit();
        }

        CancellationTokenSource cts = new CancellationTokenSource();

        private async void Movedgv()
        {
            //平移法取均值
            int numberOfCalculators = 5; // 假设有5个独立的平均值计算器
            int numberOfCalculators2 = 3; // 假设有5个独立的平均值计算器
            MultiMovingAverageCalculator multiCalculator = new MultiMovingAverageCalculator(numberOfCalculators, 50);
            MultiMovingAverageCalculator multiCalculator1 = new MultiMovingAverageCalculator(numberOfCalculators2, 10);
            await Task.Run(async () =>
            {
                while (!cts.IsCancellationRequested)
                {
                    try
                    {


                        Random random = new Random();
                        float d1 = random.Next(20, 25); // 假设有5个数据点
                        float d2 = random.Next(1, 3);
                        float d3 = random.Next(5, 8);
                        float d4 = random.Next(14, 17);
                        float d5 = random.Next(30, 33);
                        float[] dataPoints = { d1, d2, d3, d4, d5 }; // 假设有5个数据点
                        // 将浮点数数组添加到计算器中，并获取每个数据点的平均值
                        float[] averages = multiCalculator.AddDataPointsParallel(dataPoints);
                        float[] averages2 = multiCalculator1.AddDataPointsParallel(new float[] { 1f, 2f, 3f });
                        // 输出每个数据点的当前平均值

                        for (int i = 0; i < averages.Length; i++)
                        {
                            Console.WriteLine($"  Average for data point {i}: {averages[i]:F3}");

                        }
                        for (int i = 0; i < averages2.Length; i++)
                        {

                            Console.WriteLine($"  Average for data point {i}: {averages2[i]:F3}");
                        }
                        Console.WriteLine(new string('-', 40));


                        // 重置所有计算器的状态
                        // multiCalculator.ResetAll();
                    }
                    catch (OperationCanceledException)
                    {
                        // 处理取消的情况
                        break;
                    }
                    catch (Exception ex)
                    {
                        // 记录异常
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        // 可能还需要额外的日志记录或其他错误处理机制
                    }

                    // 使用 Task.Delay 代替 Thread.Sleep，并传递取消令牌
                    try
                    {
                        await Task.Delay(1000, cts.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {

                        break;
                    }
                }
            }, cts.Token);

        }

        //平均法判断稳定性

        float[] averages;
        bool[] stabilityStatus;
        float[] standardDeviations ;
        float[] standardUncertainties ;
        float d1 = 0; // 假设有5个数据点
        float d2 = 0;
        float d3 = 0;
        float d4 = 0;
        float d5 = 0;
        float[] datas ;
        private async void MovedgvIsStale()
        {
           
            //平移法取均值
            int numberOfCalculators = 5; // 假设有5个独立的平均值计算器
            int sampleSize = 5; // 每个计算器的移动平均窗口大小为50
            float stabilityThreshold = 0.1f; // 稳定性判定的阈值
            int StabilityCheckWindow = 5;
            averages = Enumerable.Repeat(0f, numberOfCalculators).ToArray();
            stabilityStatus = Enumerable.Repeat(false, numberOfCalculators).ToArray();
            standardDeviations = Enumerable.Repeat(0f, numberOfCalculators).ToArray();
            standardUncertainties = Enumerable.Repeat(0f, numberOfCalculators).ToArray();
            datas = Enumerable.Repeat(0f, numberOfCalculators).ToArray();
            MultiMovingAverageCalculatorIsSta multiCalculator = new MultiMovingAverageCalculatorIsSta(numberOfCalculators, sampleSize, stabilityThreshold, StabilityCheckWindow);
            await Task.Run(async () =>
            {
                while (!cts.IsCancellationRequested)
                {
                    try
                    {


                        Random random = new Random();
                        d1 =Convert.ToSingle(3.2+0.1* random.NextDouble()); // 假设有5个数据点
                         d2 = Convert.ToSingle(9 + 1 * random.NextDouble());
                         d3 = Convert.ToSingle(20 + 5 * random.NextDouble());
                         d4 = Convert.ToSingle(60 + 2 * random.NextDouble());
                         d5 = Convert.ToSingle( random.Next(10,12));

                         datas =new float[5] { d1, d2, d3, d4, d5 };
                        

                        averages = multiCalculator.AddDataPointsParallel(datas);
                         stabilityStatus = multiCalculator.GetStabilityStatus();
                         standardDeviations = multiCalculator.GetAllStandardDeviations();
                         standardUncertainties = multiCalculator.GetAllStandardUncertainties();

                        // 输出每个数据点的当前平均值和稳定性状态

                        for (int i = 0; i < datas.Length; i++)
                        {
                            Console.WriteLine($"   {i}: {datas[i]:F3} ,Average  {averages[i]:F3} ,Dev {standardDeviations[i]:F3},Unc {standardUncertainties[i]:F3} ,Is Stable: {stabilityStatus[i]}");
                        }

                        Console.WriteLine(new string('-', 40));


                        // 重置所有计算器的状态
                        // multiCalculator.ResetAll();
                    }
                    catch (OperationCanceledException)
                    {
                        // 处理取消的情况
                        break;
                    }
                    catch (Exception ex)
                    {
                        // 记录异常
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        // 可能还需要额外的日志记录或其他错误处理机制
                    }

                    // 使用 Task.Delay 代替 Thread.Sleep，并传递取消令牌
                    try
                    {
                        await Task.Delay(300, cts.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {

                        break;
                    }
                }
            }, cts.Token);

        }

        public void MoveIssta()
        {
            int numberOfCalculators = 5; // 假设有5个独立的平均值计算器
            int sampleSize = 50; // 每个计算器的移动平均窗口大小为50
            float stabilityThreshold = 0.01f; // 稳定性判定的阈值
            MultiMovingAverageCalculatorIsSta multiCalculator = new MultiMovingAverageCalculatorIsSta(numberOfCalculators, sampleSize, stabilityThreshold);
            float[] dataPoints = { 1.2f, 2.3f, 3.4f, 4.5f, 5.6f }; // 假设有5个数据点

            // 模拟多次添加相同的数据点
            for (int iteration = 0; iteration < 55; iteration++) // 55次以确保超过50个样本
            {
                float[] averages = multiCalculator.AddDataPointsParallel(dataPoints);
                bool[] stabilityStatus = multiCalculator.GetStabilityStatus();

                // 输出每个数据点的当前平均值和稳定性状态
                Console.WriteLine($"Iteration {iteration}:");
                for (int i = 0; i < averages.Length; i++)
                {
                    Console.WriteLine($"  Average  {i}:{averages[i]:F3} {averages[i]:F3}, Is Stable: {stabilityStatus[i]}");
                }

                Console.WriteLine(new string('-', 40));
            }

            // 重置所有计算器的状态
            multiCalculator.ResetAll();

            // 打印每个计算器的样本窗口大小
            int[] sampleSizes = multiCalculator.GetSampleSizes();
            for (int i = 0; i < sampleSizes.Length; i++)
            {
                Console.WriteLine($"  Sample size for calculator {i}: {sampleSizes[i]}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CAL();
        }

        public  void CAL()
        {
            int numberOfCalculators = 5; // 假设有5个独立的平均值计算器
            int sampleSize = 50; // 每个计算器的移动平均窗口大小为50
            float stabilityThreshold = 0.01f; // 稳定性判定的阈值
            MultiMovingAverageCalculatorIsSta multiCalculator = new MultiMovingAverageCalculatorIsSta(numberOfCalculators, sampleSize, stabilityThreshold);
            float[] dataPoints = { 1.2f, 2.3f, 3.4f, 4.5f, 5.6f }; // 假设有5个数据点

            // 模拟多次添加相同的数据点
            for (int iteration = 0; iteration < 55; iteration++) // 55次以确保超过50个样本
            {
                float[] averages = multiCalculator.AddDataPointsParallel(dataPoints);
                bool[] stabilityStatus = multiCalculator.GetStabilityStatus();
                float[] standardDeviations = multiCalculator.GetAllStandardDeviations();
                float[] standardUncertainties = multiCalculator.GetAllStandardUncertainties();

                // 输出每个数据点的当前平均值、标准差、标准不确定度和稳定性状态
                Console.WriteLine($"Iteration {iteration}:");
                for (int i = 0; i < averages.Length; i++)
                {
                    Console.WriteLine($"  Average for data point {i}: {averages[i]:F3}, Standard Deviation: {standardDeviations[i]:F3}, Standard Uncertainty: {standardUncertainties[i]:F3}, Is Stable: {stabilityStatus[i]}");
                }

                Console.WriteLine(new string('-', 40));
            }

            // 重置所有计算器的状态
            multiCalculator.ResetAll();

            // 打印每个计算器的样本窗口大小
            int[] sampleSizes = multiCalculator.GetSampleSizes();
            for (int i = 0; i < sampleSizes.Length; i++)
            {
                Console.WriteLine($"  Sample size for calculator {i}: {sampleSizes[i]}");
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void timeinit()
        {
            // Initialize the custom chart control
            chart = new TimeSeriesChart();
            this.Controls.Add(chart);

            // Add two series to the chart
            chart.AddSeries("Series1", SeriesChartType.Spline);
            chart.AddSeries("Series2", SeriesChartType.Spline);

            // Set the maximum number of points to display
            chart.SetMaxPoints(20);

            // Set up a timer to add data points periodically
            timer = new Timer { Interval = 500 }; // 1 second interval
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Generate some random data for demonstration purposes
            var rand = new Random();
            var now = DateTime.Now;

            // Add the new point to both series
            chart.AddDataPoint("Series1", now, datas[0]);
            chart.AddDataPoint("Series2", now, averages[0]);

            counter++;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.TopMost = true;
            form2.Show();
        }
    }
}
