using System;
using System.Linq;
using System.Threading.Tasks;

namespace 平移法
{
    public class MovingAverageCalculator
    {
        private readonly int sampleSize;
        private readonly float[] sampleData;
        private float sum; // 使用 float 来存储累加和
        private int sampleIndex;
        private bool initialized;

        /// <summary>
        /// 初始化一个新的 MovingAverageCalculator 实例。
        /// </summary>
        /// <param name="sampleSize">移动平均窗口的大小，默认为100。</param>
        public MovingAverageCalculator(int sampleSize = 100)
        {
            this.sampleSize = sampleSize;
            sampleData = new float[sampleSize];
            sum = 0;
            sampleIndex = 0;
            initialized = false;
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
                sampleData[sampleIndex] = dataPoint;
                sum += dataPoint;
            }
            else
            {
                // 累加至指定的样本数量
                sampleData[sampleIndex] = dataPoint;
                sum += dataPoint;

                // 当采样达到指定数量时，标记为已初始化
                if (sampleIndex == sampleSize - 1)
                {
                    initialized = true;
                }
            }

            // 计算平均值
            Average = initialized ? sum / sampleSize : sum / (sampleIndex + 1);

            // 更新采样索引，循环存储数据
            sampleIndex = (sampleIndex + 1) % sampleSize;
        }

        /// <summary>
        /// 获取当前的平均值。
        /// </summary>
        public float Average { get; private set; }

        /// <summary>
        /// 重置计算器状态，以便重新开始收集数据。
        /// </summary>
        public void Reset()
        {
            Array.Clear(sampleData, 0, sampleSize);
            sum = 0;
            sampleIndex = 0;
            initialized = false;
            Average = 0;
        }

        /// <summary>
        /// 获取当前的样本窗口大小。
        /// </summary>
        public int SampleSize => sampleSize;
    }

    public class MultiMovingAverageCalculator
    {
        private readonly MovingAverageCalculator[] calculators;

        /// <summary>
        /// 初始化一个新的 MultiMovingAverageCalculator 实例。
        /// </summary>
        /// <param name="numberOfCalculators">计算器的数量。</param>
        /// <param name="sampleSize">每个计算器的移动平均窗口大小，默认为100。</param>
        public MultiMovingAverageCalculator(int numberOfCalculators, int sampleSize = 100)
        {
            calculators = Enumerable.Range(0, numberOfCalculators)
                                    .Select(_ => new MovingAverageCalculator(sampleSize))
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
        /// 获取所有计算器的样本窗口大小。
        /// </summary>
        /// <returns>所有计算器的样本窗口大小数组。</returns>
        public int[] GetSampleSizes()
        {
            return calculators.Select(calculator => calculator.SampleSize).ToArray();
        }
    }
}
