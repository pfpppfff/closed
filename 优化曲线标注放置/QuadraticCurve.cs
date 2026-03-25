using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 优化曲线标注放置
{
    public class QuadraticCurve
    {
        /// <summary>
        /// 曲线系数 a*x^2 + b*x + c
        /// </summary>
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

        /// <summary>
        /// 曲线ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 曲线颜色
        /// </summary>
        public Color Color { get; set; }

        public QuadraticCurve(int id, double a, double b, double c, Color color)
        {
            Id = id;
            A = a;
            B = b;
            C = c;
            Color = color;
        }

        /// <summary>
        /// 计算给定X值对应的Y值
        /// </summary>
        public double CalculateY(double x)
        {
            return A * x * x + B * x + C;
        }

        /// <summary>
        /// 获取曲线上的点
        /// </summary>
        public PointF GetPoint(double x)
        {
            return new PointF((float)x, (float)CalculateY(x));
        }
    }

    /// <summary>
    /// 表示一个标注
    /// </summary>
    public class LabelAnnotation
    {
        /// <summary>
        /// 标注关联的曲线
        /// </summary>
        public QuadraticCurve Curve { get; set; }

        /// <summary>
        /// 标注在曲线上的锚点
        /// </summary>
        public PointF AnchorPoint { get; set; }

        /// <summary>
        /// 标注的中心点
        /// </summary>
        public PointF Center { get; set; }

        /// <summary>
        /// 标注的大小（正方形边长）
        /// </summary>
        public float Size { get; set; }

        /// <summary>
        /// 标注的角度（度）
        /// </summary>
        public int Angle { get; set; }

        /// <summary>
        /// 标注的四个角点
        /// </summary>
        public PointF[] Corners { get; set; }

        public LabelAnnotation(QuadraticCurve curve, PointF anchorPoint, float size)
        {
            Curve = curve;
            AnchorPoint = anchorPoint;
            Size = size;
            Corners = new PointF[4];
        }

        /// <summary>
        /// 计算标注的四个角点
        /// </summary>
        public void CalculateCorners()
        {
            // 角点是锚点，我们需要计算其他三个点
            // 基于角度和大小计算正方形的四个角点
            double radian = Angle * Math.PI / 180.0;
            double cos = Math.Cos(radian);
            double sin = Math.Sin(radian);

            // 计算相对于锚点的偏移
            // 锚点是四个角中的一个，我们假设是左下角
            float halfSize = Size / 2;

            // 四个角点的相对位置
            PointF[] relativePoints = new PointF[]
            {
                new PointF(-halfSize, -halfSize), // 左下角
                new PointF(halfSize, -halfSize),  // 右下角
                new PointF(halfSize, halfSize),   // 右上角
                new PointF(-halfSize, halfSize)   // 左上角
            };

            // 旋转并平移到锚点位置
            for (int i = 0; i < 4; i++)
            {
                float rotatedX = relativePoints[i].X * (float)cos - relativePoints[i].Y * (float)sin;
                float rotatedY = relativePoints[i].X * (float)sin + relativePoints[i].Y * (float)cos;
                Corners[i] = new PointF(AnchorPoint.X + rotatedX, AnchorPoint.Y + rotatedY);
            }

            // 计算中心点
            Center = new PointF(
                (Corners[0].X + Corners[1].X + Corners[2].X + Corners[3].X) / 4,
                (Corners[0].Y + Corners[1].Y + Corners[2].Y + Corners[3].Y) / 4
            );
        }
    }

    /// <summary>
    /// 曲线标注优化器
    /// </summary>
    public class CurveLabelOptimizer
    {
        /// <summary>
        /// 曲线列表
        /// </summary>
        public List<QuadraticCurve> Curves { get; set; }

        /// <summary>
        /// 标注列表
        /// </summary>
        public List<LabelAnnotation> Labels { get; set; }

        /// <summary>
        /// 标注大小
        /// </summary>
        public float LabelSize { get; set; }

        /// <summary>
        /// 图像边界
        /// </summary>
        public RectangleF ImageBounds { get; set; }

        public CurveLabelOptimizer(RectangleF imageBounds, float labelSize = 20)
        {
            Curves = new List<QuadraticCurve>();
            Labels = new List<LabelAnnotation>();
            ImageBounds = imageBounds;
            LabelSize = labelSize;
        }

        /// <summary>
        /// 添加曲线
        /// </summary>
        public void AddCurve(QuadraticCurve curve)
        {
            Curves.Add(curve);
        }

        /// <summary>
        /// 优化标注放置
        /// </summary>
        public bool OptimizeLabels()
        {
            Labels.Clear();

            // 可选角度
            int[] angles = { 0, 45, 90, 135, 180, 225, 270, 315 };

            // 为每条曲线找到最佳标注位置
            foreach (var curve in Curves)
            {
                LabelAnnotation bestLabel = null;
                double bestScore = double.MinValue;

                // 在曲线上采样多个点
                for (double x = ImageBounds.Left; x <= ImageBounds.Right; x += 5)
                {
                    // 检查点是否在图像范围内
                    double y = curve.CalculateY(x);
                    if (y < ImageBounds.Top || y > ImageBounds.Bottom)
                        continue;

                    PointF anchorPoint = new PointF((float)x, (float)y);

                    // 尝试每个角度
                    foreach (int angle in angles)
                    {
                        LabelAnnotation label = new LabelAnnotation(curve, anchorPoint, LabelSize);
                        label.Angle = angle;
                        label.CalculateCorners();

                        // 检查标注是否在图像边界内
                        if (!IsLabelWithinBounds(label))
                            continue;

                        // 检查标注是否与其他曲线相交
                        if (IsLabelIntersectingWithCurves(label))
                            continue;

                        // 检查标注是否与其他已放置的标注重叠
                        if (IsLabelOverlappingWithExistingLabels(label))
                            continue;

                        // 计算评分（可以根据美学要求调整）
                        double score = CalculateLabelScore(label);
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestLabel = label;
                        }
                    }
                }

                // 如果找到了合适的标注位置，则添加到列表中
                if (bestLabel != null)
                {
                    Labels.Add(bestLabel);
                }
                else
                {
                    // 如果没有找到合适的位置，返回失败
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 检查标注是否在图像边界内
        /// </summary>
        private bool IsLabelWithinBounds(LabelAnnotation label)
        {
            foreach (var corner in label.Corners)
            {
                if (corner.X < ImageBounds.Left || corner.X > ImageBounds.Right ||
                    corner.Y < ImageBounds.Top || corner.Y > ImageBounds.Bottom)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检查标注是否与其他曲线相交
        /// </summary>
        private bool IsLabelIntersectingWithCurves(LabelAnnotation label)
        {
            // 检查标注的四条边是否与任何曲线相交
            for (int i = 0; i < 4; i++)
            {
                PointF p1 = label.Corners[i];
                PointF p2 = label.Corners[(i + 1) % 4];

                // 检查这条边是否与任何曲线相交
                foreach (var curve in Curves)
                {
                    // 跳过标注所属的曲线
                    if (curve.Id == label.Curve.Id)
                        continue;

                    if (IsLineIntersectingCurve(p1, p2, curve))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 检查线段是否与曲线相交
        /// </summary>
        private bool IsLineIntersectingCurve(PointF p1, PointF p2, QuadraticCurve curve)
        {
            // 简化检查：采样线段上的多个点，检查这些点是否非常接近曲线
            const int sampleCount = 20;
            const float threshold = 2.0f; // 距离阈值

            for (int i = 0; i <= sampleCount; i++)
            {
                float t = (float)i / sampleCount;
                PointF pointOnLine = new PointF(
                    p1.X + t * (p2.X - p1.X),
                    p1.Y + t * (p2.Y - p1.Y)
                );

                // 计算该点到曲线的垂直距离
                double distance = DistancePointToCurve(pointOnLine, curve);
                if (distance < threshold)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 计算点到曲线的近似距离
        /// </summary>
        private double DistancePointToCurve(PointF point, QuadraticCurve curve)
        {
            // 找到曲线上最接近给定点的点
            // 这是一个简化实现，实际应该求解最小距离

            // 在曲线定义域内采样寻找最近点
            double minDistance = double.MaxValue;
            const int sampleCount = 50;

            double xStep = (ImageBounds.Right - ImageBounds.Left) / sampleCount;

            for (double x = ImageBounds.Left; x <= ImageBounds.Right; x += xStep)
            {
                double y = curve.CalculateY(x);
                double distance = Math.Sqrt(Math.Pow(point.X - x, 2) + Math.Pow(point.Y - y, 2));

                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }

            return minDistance;
        }

        /// <summary>
        /// 检查标注是否与其他已放置的标注重叠
        /// </summary>
        private bool IsLabelOverlappingWithExistingLabels(LabelAnnotation label)
        {
            foreach (var existingLabel in Labels)
            {
                if (IsLabelOverlapping(label, existingLabel))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查两个标注是否重叠
        /// </summary>
        private bool IsLabelOverlapping(LabelAnnotation label1, LabelAnnotation label2)
        {
            // 简化的边界框检查
            RectangleF bounds1 = GetLabelBounds(label1);
            RectangleF bounds2 = GetLabelBounds(label2);

            return bounds1.IntersectsWith(bounds2);
        }

        /// <summary>
        /// 获取标注的边界框
        /// </summary>
        private RectangleF GetLabelBounds(LabelAnnotation label)
        {
            float minX = label.Corners[0].X;
            float maxX = label.Corners[0].X;
            float minY = label.Corners[0].Y;
            float maxY = label.Corners[0].Y;

            for (int i = 1; i < 4; i++)
            {
                minX = Math.Min(minX, label.Corners[i].X);
                maxX = Math.Max(maxX, label.Corners[i].X);
                minY = Math.Min(minY, label.Corners[i].Y);
                maxY = Math.Max(maxY, label.Corners[i].Y);
            }

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// 计算标注评分（美学评估）
        /// </summary>
        private double CalculateLabelScore(LabelAnnotation label)
        {
            double score = 0;

            // 1. 角度偏好：水平或垂直更美观
            int[] preferredAngles = { 0, 90, 180, 270 };
            if (preferredAngles.Contains(label.Angle))
            {
                score += 10;
            }

            // 2. 位置偏好：避免边缘
            float centerX = (ImageBounds.Left + ImageBounds.Right) / 2;
            float centerY = (ImageBounds.Top + ImageBounds.Bottom) / 2;
            float distanceToCenter = (float)Math.Sqrt(
                Math.Pow(label.Center.X - centerX, 2) +
                Math.Pow(label.Center.Y - centerY, 2));

            // 距离中心越近得分越高
            float maxDistance = (float)Math.Sqrt(
                Math.Pow(ImageBounds.Width / 2, 2) +
                Math.Pow(ImageBounds.Height / 2, 2));

            score += (1 - distanceToCenter / maxDistance) * 5;

            // 3. 避免与其他标注过于接近
            foreach (var otherLabel in Labels)
            {
                float distance = (float)Math.Sqrt(
                    Math.Pow(label.Center.X - otherLabel.Center.X, 2) +
                    Math.Pow(label.Center.Y - otherLabel.Center.Y, 2));

                // 距离越远得分越高
                score += Math.Min(distance / 50, 2);
            }

            return score;
        }
    }
}
