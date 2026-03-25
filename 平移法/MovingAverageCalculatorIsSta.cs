using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace 平移法
{
    public class MovingAverageCalculatorIsSta
    {
        private readonly int sampleSize;
        private readonly float[] sampleData;
        private float sum; // 使用 float 来存储累加和
        private float sumOfSquares; // 用于存储平方和
        private int sampleIndex;
        private bool initialized;
        private float previousAverage; // 用于存储上一次的平均值
        private float stabilityThreshold; // 稳定性判定的阈值
        private readonly int StabilityCheckWindow ; // 用于稳定性检查的窗口大小
        private float[] recentAverages; // 用于存储最近几次的平均值
        private object MathF;

        /// <summary>
        /// 初始化一个新的 MovingAverageCalculator 实例。
        /// </summary>
        /// <param name="sampleSize">移动平均窗口的大小，默认为100。</param>
        /// <param name="stabilityThreshold">稳定性判定的阈值，默认为0.01。</param>
        public MovingAverageCalculatorIsSta(int sampleSize = 100, float stabilityThreshold = 0.01f, int StabilityCheckWindow = 5)
        {
            this.sampleSize = sampleSize;
            this.stabilityThreshold = stabilityThreshold;
            this.StabilityCheckWindow = StabilityCheckWindow;
            sampleData = new float[sampleSize];
            sum = 0;
            sampleIndex = 0;
            initialized = false;
            previousAverage = 0;
            recentAverages = new float[StabilityCheckWindow];
            //Array.Fill(recentAverages, 0);
            recentAverages = Enumerable.Repeat(0f, StabilityCheckWindow).ToArray();
        }

        /// <summary>
        /// 添加一个新的数据点并更新平均值。
        /// </summary>
        /// <param name="dataPoint">输入的数据点。</param>
        public void AddDataPoint(float dataPoint)
        {
            if (initialized)
            {
                // 减去旧的采集值，加入新的采集值
                sum -= sampleData[sampleIndex];
                sumOfSquares -= sampleData[sampleIndex] * sampleData[sampleIndex];
                sampleData[sampleIndex] = dataPoint;
                sum += dataPoint;

                // 更新平方和              
                sumOfSquares += dataPoint * dataPoint;
            }
            else
            {
                // 累加至指定的样本数量
                sampleData[sampleIndex] = dataPoint;
                sum += dataPoint;
                sumOfSquares += dataPoint * dataPoint;

                // 当采样达到指定数量时，标记为已初始化
                if (sampleIndex == sampleSize - 1)
                {
                    initialized = true;
                }
            }
            RealValue = dataPoint;

            // 计算平均值
            Average = initialized ? sum / sampleSize : sum / (sampleIndex + 1);
            // 计算标准差
            if (initialized /*&& sampleIndex >= sampleSize - 1*/)
            {
                float variance =Math.Abs( (sumOfSquares - sum * sum / sampleSize) / (sampleSize - 1));
                StandardDeviation = (float)Math.Sqrt(variance);
                StandardUncertainty = (float)(Math.Sqrt(StandardDeviation/(sampleSize-1)));
            }
            else
            {
                StandardDeviation = 0;
                StandardUncertainty = 0;
            }
            // 更新最近几次的平均值
            UpdateRecentAverages();

            // 更新采样索引，循环存储数据
            sampleIndex = (sampleIndex + 1) % sampleSize;
        }

        /// <summary>
        /// 更新最近几次的平均值。
        /// </summary>
        private void UpdateRecentAverages()
        {
            // 将当前平均值添加到 recentAverages 数组中
            for (int i = StabilityCheckWindow - 1; i > 0; i--)
            {
                recentAverages[i] = recentAverages[i - 1];
            }
            recentAverages[0] = RealValue;

            // 如果 recentAverages 数组已满，则可以进行稳定性检查
            if (initialized /*&& sampleIndex >= sampleSize - 1*/)
            {
                IsStable = CheckStability();
            }
        }

        /// <summary>
        /// 检查数据是否已经稳定。
        /// </summary>
        /// <returns>如果数据稳定，返回 true；否则返回 false。</returns>
        private bool CheckStability()
        {
            // 计算最近几次平均值的最大差异
            float maxDifference = recentAverages.Max() - recentAverages.Min();

            // 如果最大差异小于阈值，则认为数据已经稳定
            return maxDifference <= stabilityThreshold;
        }

        /// <summary>
        /// 获取当前的平均值。
        /// </summary>
        public float Average { get; private set; }

        /// <summary>
        /// 获取当前的平均值。
        /// </summary>
        public float RealValue { get; private set; }
        /// <summary>
        /// 获取当前数据是否已经稳定。
        /// </summary>
        public bool IsStable { get; private set; }

        /// <summary>
        /// 获取当前的样本标准差。
        /// </summary>
        public float StandardDeviation { get; private set; }

        /// <summary>
        /// 获取当前的标准不确定度。
        /// </summary>
        public float StandardUncertainty { get; private set; }

        /// <summary>
        /// 重置计算器状态，以便重新开始收集数据。
        /// </summary>
        public void Reset()
        {
            Array.Clear(sampleData, 0, sampleSize);
            sum = 0;
            sampleIndex = 0;
            initialized = false;
            previousAverage = 0;
            // Array.Fill(recentAverages, 0);
            recentAverages = Enumerable.Repeat(0f, StabilityCheckWindow).ToArray();
            IsStable = false;
            Average = 0;
        }

        /// <summary>
        /// 获取当前的样本窗口大小。
        /// </summary>
        public int SampleSize => sampleSize;

        /// <summary>
        /// 设置稳定性判定的阈值。
        /// </summary>
        /// <param name="threshold">稳定性判定的阈值。</param>
        public void SetStabilityThreshold(float threshold)
        {
            stabilityThreshold = threshold;
        }
    }

    public class MultiMovingAverageCalculatorIsSta
    {
        private readonly MovingAverageCalculatorIsSta[] calculators;

        /// <summary>
        /// 初始化一个新的 MultiMovingAverageCalculator 实例。
        /// </summary>
        /// <param name="numberOfCalculators">计算器的数量。</param>
        /// <param name="sampleSize">每个计算器的移动平均窗口大小，默认为100。</param>
        /// <param name="stabilityThreshold">稳定性判定的阈值，默认为0.01。</param>
        public MultiMovingAverageCalculatorIsSta(int numberOfCalculators, int sampleSize = 100, float stabilityThreshold = 0.01f, int StabilityCheckWindow=5)
        {
            calculators = Enumerable.Range(0, numberOfCalculators)
                                    .Select(_ => new MovingAverageCalculatorIsSta(sampleSize, stabilityThreshold, StabilityCheckWindow))
                                    .ToArray();
        }

        /// <summary>
        /// 并行添加一组新的数据点并更新每个数据点的平均值。
        /// </summary>
        /// <param name="dataInput">输入的数据点数组。</param>
        /// <returns>每个数据点的当前平均值数组。</returns>
        public float[] AddDataPointsParallel(float[] dataInput)
        {
            if (dataInput.Length != calculators.Length)
            {
                throw new ArgumentException("Input data length must match the number of calculators.");
            }

            // 使用 Parallel.For 进行并行处理
            Parallel.For(0, dataInput.Length, i =>
            {
                calculators[i].AddDataPoint(dataInput[i]);
            });

            // 获取所有计算器的当前平均值
            return calculators.Select(calculator => calculator.Average).ToArray();
        }

        /// <summary>
        /// 重置所有计算器的状态。
        /// </summary>
        public void ResetAll()
        {
            Parallel.ForEach(calculators, calculator => calculator.Reset());
        }

        /// <summary>
        /// 获取所有计算器的当前平均值。
        /// </summary>
        /// <returns>所有计算器的当前平均值数组。</returns>
        public float[] GetAllAverages()
        {
            return calculators.Select(calculator => calculator.Average).ToArray();
        }
        /// <summary>
        /// 获取所有计算器的样本标准差。
        /// </summary>
        /// <returns>所有计算器的样本标准差数组。</returns>
        public float[] GetAllStandardDeviations()
        {
            return calculators.Select(calculator => calculator.StandardDeviation).ToArray();
        }

        /// <summary>
        /// 获取所有计算器的标准不确定度。
        /// </summary>
        /// <returns>所有计算器的标准不确定度数组。</returns>
        public float[] GetAllStandardUncertainties()
        {
            return calculators.Select(calculator => calculator.StandardUncertainty).ToArray();
        }

        /// <summary>
        /// 获取所有计算器的样本窗口大小。
        /// </summary>
        /// <returns>所有计算器的样本窗口大小数组。</returns>
        public int[] GetSampleSizes()
        {
            return calculators.Select(calculator => calculator.SampleSize).ToArray();
        }

        /// <summary>
        /// 获取所有计算器的稳定性状态。
        /// </summary>
        /// <returns>所有计算器的稳定性状态数组。</returns>
        public bool[] GetStabilityStatus()
        {
            return calculators.Select(calculator => calculator.IsStable).ToArray();
        }

        /// <summary>
        /// 设置所有计算器的稳定性判定阈值。
        /// </summary>
        /// <param name="threshold">稳定性判定的阈值。</param>
        public void SetStabilityThreshold(float threshold)
        {
            Parallel.ForEach(calculators, calculator => calculator.SetStabilityThreshold(threshold));
        }
    }

}
