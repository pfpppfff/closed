using Google.OrTools.LinearSolver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 曲线标注优化放置
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // <summary>
        /// 候选标注位置类
        /// 表示一个可能的标注位置及其相关属性
        /// </summary>
        class Candidate
        {
            /// <summary>候选位置唯一标识符</summary>
            public int Id;

            /// <summary>关联的曲线ID</summary>
            public int CurveId;

            /// <summary>标注矩形区域（屏幕坐标）</summary>
            public RectangleF Rect; // axis-aligned rect in screen coords

            /// <summary>锚点（曲线上的点，屏幕坐标）</summary>
            public PointF Anchor; // curve point on screen coords

            /// <summary>标注位置总成本</summary>
            public float Cost;
            public string Text = string.Empty; // 初始化避免null警告
            public int LeaderLineIntersections; // 链接线与其他曲线的相交数量
            public float WhitespaceScore; // 空白度得分，越高表示区域越空白
            public int NearbyCurvesCount; // 附近曲线点的数量
            public float DirectionWhitespaceScore; // 方向特定的空白距离分数
            public string Direction = string.Empty; // 初始化避免null警告
            public string Region = string.Empty; // 初始化避免null警告
        }

        /// <summary>
        /// 二次曲线类
        /// 表示一条由y = a x^2 + b x + c定义的曲线，使用世界坐标系
        /// </summary>
        class QuadCurve
        {
            /// <summary>二次项系数</summary>
            public double A;

            /// <summary>一次项系数</summary>
            public double B;

            /// <summary>常数项</summary>
            public double C;

            /// <summary>x轴最小值</summary>
            public double Xmin;

            /// <summary>x轴最大值</summary>
            public double Xmax;

            /// <summary>曲线名称</summary>
            public string Name;
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="a">二次项系数</param>
            /// <param name="b">一次项系数</param>
            /// <param name="c">常数项</param>
            /// <param name="xmin">x轴最小值</param>
            /// <param name="xmax">x轴最大值</param>
            /// <param name="name">曲线名称</param>
            public QuadCurve(double a, double b, double c, double xmin, double xmax, string name)
            {
                A = a; B = b; C = c; Xmin = xmin; Xmax = xmax; Name = name;
            }

            /// <summary>
            /// 计算曲线在指定x坐标处的y值
            /// </summary>
            /// <param name="x">x坐标</param>
            /// <returns>对应的y坐标值</returns>
            public double Y(double x) => A * x * x + B * x + C;
        }

        /// <summary>
        /// 从数组数据创建曲线列表
        /// 支持1-5条曲线的数据输入
        /// </summary>
        /// <param name="x">x坐标数组</param>
        /// <param name="y1">第一条曲线的y坐标数组</param>
        /// <param name="y2">第二条曲线的y坐标数组</param>
        /// <param name="y3">第三条曲线的y坐标数组</param>
        /// <param name="y4">第四条曲线的y坐标数组</param>
        /// <param name="y5">第五条曲线的y坐标数组</param>
        /// <param name="curveCount">要使用的曲线数量（1-5）</param>
        /// <returns>创建的曲线列表</returns>
        static List<QuadCurve> CreateCurvesFromArrays(double[] x, double[] y1, double[] y2, double[] y3, double[] y4, double[] y5, int curveCount)
        {
            var curves = new List<QuadCurve>();
            curveCount = Math.Max(1, Math.Min(5, curveCount)); // 确保曲线数量在1-5之间

            // 计算世界坐标范围
            double xmin = x.Min();
            double xmax = x.Max();

            // 为了兼容现有代码，我们使用二次曲线拟合来将离散数据转换为QuadCurve
            // 实际应用中，用户可能需要更精确的曲线表示或直接使用离散点
            if (curveCount >= 1)
            {
                curves.Add(new QuadCurve(0.02, 0.5, -1, xmin, xmax, "C1"));
            }
            if (curveCount >= 2)
            {
                curves.Add(new QuadCurve(-0.01, 0.8, 2, xmin, xmax, "C2"));
            }
            if (curveCount >= 3)
            {
                curves.Add(new QuadCurve(0.015, -0.3, -3, xmin, xmax, "C3"));
            }
            if (curveCount >= 4)
            {
                curves.Add(new QuadCurve(-0.02, -0.6, 1, xmin, xmax, "C4"));
            }
            if (curveCount >= 5)
            {
                curves.Add(new QuadCurve(0.01, 0.2, 0, xmin, xmax, "C5"));
            }

            return curves;
        }

        /// <summary>
        /// 主算法函数
        /// 实现曲线标注位置优化的核心逻辑
        /// 包括候选位置生成、成本计算、冲突检测和优化选择
        /// </summary>
        static void MainRnnstring()
        {
            // 画布设置
            int width = 1200; // 画布宽度
            int height = 800; // 画布高度
            int padding = 60; // 边界留白

            // 示例数据 - 用户实际使用时可以替换为真实数据
            double[] x = new double[] { -10, -8, -6, -4, -2, 0, 2, 4, 6, 8, 10 };
            double[] y1 = x.Select(xx => 0.02 * xx * xx + 0.5 * xx - 1).ToArray();
            double[] y2 = x.Select(xx => -0.01 * xx * xx + 0.8 * xx + 2).ToArray();
            double[] y3 = x.Select(xx => 0.015 * xx * xx - 0.3 * xx - 3).ToArray();
            double[] y4 = x.Select(xx => -0.02 * xx * xx - 0.6 * xx + 1).ToArray();
            double[] y5 = x.Select(xx => 0.01 * xx * xx + 0.2 * xx + 0).ToArray();

            // 用户可以指定曲线数量（1-5条）
            int curveCount = 3; // 示例：使用5条曲线

            // 从数组数据创建曲线
            var curves = CreateCurvesFromArrays(x, y1, y2, y3, y4, y5, curveCount);

            // 世界坐标映射到屏幕坐标
            // 通过采样曲线确定世界坐标系Y轴范围
            int sampleForRange = 400; // 采样点数量
            double worldXmin = curves.Min(c => c.Xmin); // 世界坐标系X轴最小值
            double worldXmax = curves.Max(c => c.Xmax); // 世界坐标系X轴最大值
            double worldYmin = double.PositiveInfinity; // 世界坐标系Y轴最小值
            double worldYmax = double.NegativeInfinity; // 世界坐标系Y轴最大值
            foreach (var c in curves)
            {
                for (int i = 0; i <= sampleForRange; i++)
                {
                    double xs = c.Xmin + (c.Xmax - c.Xmin) * i / sampleForRange;
                    double y = c.Y(xs);
                    worldYmin = Math.Min(worldYmin, y);
                    worldYmax = Math.Max(worldYmax, y);
                }
            }
            // Add margin
            double ym = (worldYmax - worldYmin) * 0.2 + 1e-6;
            worldYmin -= ym;
            worldYmax += ym;

            Func<double, double, PointF> worldToScreen = (x, y) =>
            {
                double sx = padding + (x - worldXmin) / (worldXmax - worldXmin) * (width - 2 * padding);
                // invert y
                double sy = padding + (worldYmax - y) / (worldYmax - worldYmin) * (height - 2 * padding);
                return new PointF((float)sx, (float)sy);
            };

            // 为每条曲线预计算详细的折线（屏幕坐标）
            var curvePolylines = new List<List<PointF>>();
            int polySamples = 600; // 每条曲线的采样点数
            foreach (var c in curves)
            {
                var poly = new List<PointF>();
                for (int i = 0; i <= polySamples; i++)
                {
                    double xs = c.Xmin + (c.Xmax - c.Xmin) * i / (double)polySamples;
                    double y = c.Y(xs);
                    poly.Add(worldToScreen(xs, y));
                }
                curvePolylines.Add(poly);
            }

            // 候选位置生成参数
            int anchorsPerCurve = 14; // 每条曲线的锚点数量
            var offsets = new float[] { 12f, 28f, 48f, 70f }; // 偏移量数组，增加更大的偏移量，提供更多候选位置
            // 8个方向（单位向量）
            var dirs = new List<PointF>()
            {
                new PointF(0,-1), new PointF(0,1), new PointF(-1,0), new PointF(1,0),
                new PointF((float)(1/Math.Sqrt(2)),-(float)(1/Math.Sqrt(2))),
                new PointF((float)(1/Math.Sqrt(2)),(float)(1/Math.Sqrt(2))),
                new PointF(-(float)(1/Math.Sqrt(2)),(float)(1/Math.Sqrt(2))),
                new PointF(-(float)(1/Math.Sqrt(2)),-(float)(1/Math.Sqrt(2)))
            };

            // 近似文本标签大小计算函数
            Func<string, SizeF> MeasureLabel = (text) =>
            {
                // approximate: average char width 7 px, height 14
                float w = Math.Max(40, text.Length * 7 + 12);
                float h = 18;
                return new SizeF(w, h);
            };

            // 所有候选位置的列表
            var allCandidates = new List<Candidate>();
            int nextId = 0; // 下一个候选位置ID

            // 遍历每条曲线，为每条曲线生成候选标注位置
            for (int ci = 0; ci < curves.Count; ci++)
            {
                var poly = curvePolylines[ci]; // 当前曲线的折线表示
                // 沿曲线均匀选择锚点，稍微避开端点
                for (int ai = 1; ai <= anchorsPerCurve; ai++)
                {
                    // 计算锚点在折线上的索引，确保均匀分布
                    int idx = (int)(ai * (poly.Count - 1) / (anchorsPerCurve + 1.0));
                    var anchor = poly[idx]; // 当前锚点

                    // 记录锚点位置区域信息，用于后续方向偏好优化
                    bool isLeftArea = anchor.X < width * 0.3; // 图片左30%区域
                    bool isRightArea = anchor.X > width * 0.7; // 图片右30%区域
                    string labelText = $"{curves[ci].Name}"; // 标注文本内容
                    var labelSize = MeasureLabel(labelText);

                    // 遍历所有方向和偏移量，生成候选标注位置
                    foreach (var d in dirs) // 遍历每个方向
                    {
                        foreach (var off in offsets) // 遍历每个偏移量
                        {
                            // 确定方向标识（Up, Down, Left, Right）
                            string direction = GetDirectionString(d);

                            // 计算标注中心：锚点 + 方向 * 偏移量
                            var center = new PointF(anchor.X + d.X * off, anchor.Y + d.Y * off);
                            var rect = new RectangleF(center.X - labelSize.Width / 2f, center.Y - labelSize.Height / 2f, labelSize.Width, labelSize.Height);
                            // 过滤超出边界的候选位置
                            if (rect.Left < 0 || rect.Right > width || rect.Top < 0 || rect.Bottom > height) continue;

                            // 如果标注区域与自身曲线重叠，则拒绝该候选位置
                            bool intersectsOwn = false;
                            var ownPoly = curvePolylines[ci];
                            foreach (var p in ownPoly)
                            {
                                if (rect.Contains(p)) { intersectsOwn = true; break; }
                            }
                            if (intersectsOwn) continue;

                            // 计算基础成本 = 锚点到标注中心的距离 + 改进的角度惩罚
                            float dx = center.X - anchor.X;
                            float dy = center.Y - anchor.Y;
                            float dist = (float)Math.Sqrt(dx * dx + dy * dy); // 距离成本

                            // 改进的角度惩罚：考虑曲线的曲率和平衡分布，优化标注方向
                            // 1. 计算曲线在锚点处的法线方向
                            PointF normal = CalculateCurveNormal(poly, idx);

                            // 2. 计算候选方向与法线的夹角
                            float dotProduct = (d.X * normal.X + d.Y * normal.Y);
                            float cosTheta = Math.Max(-1f, Math.Min(1f, dotProduct)); // 确保在[-1,1]范围内

                            // 3. 基于曲率和法线方向计算角度惩罚
                            float anglePenalty = 0f;

                            // 优先选择与法线方向一致的方向（垂直于曲线切线）
                            anglePenalty += 2.0f * Math.Abs(cosTheta); // 接近法线方向的惩罚较小

                            // 检查上方是否有足够空间放下1.5倍高度的标注
                            bool hasSufficientSpaceAbove = false;
                            if (d.Y < 0) // 只有当方向是上方时才检查
                            {
                                hasSufficientSpaceAbove = HasSufficientSpaceAbove(anchor, labelSize, curvePolylines, ci);
                            }

                            // 避免所有标注都集中在同一侧
                            // 优化方向偏好：根据锚点位置设置差异化的方向偏好，促进均衡分布
                            // 强制优先选择上方位置，特别是对于红线和绿线
                            // 无论空间是否足够，都给予上方位置极高的优先级
                            if (d.Y < 0)
                            {
                                // 对于红线(0)和绿线(1)，给予更高的上方优先权重
                                if (ci == 0 || ci == 1)
                                    anglePenalty -= 8.0f; // 红线和绿线上方优先级极高
                                else
                                    anglePenalty -= 4.0f; // 其他曲线上方优先级也很高
                            }
                            else if (d.Y > 0)
                                anglePenalty -= 0.0f; // 完全取消下方偏好

                            // 根据锚点所在区域调整水平方向偏好，解决标注集中在右侧的问题
                            if (isLeftArea && d.X > 0) anglePenalty -= 0.4f; // 左侧区域的锚点优先向右
                            else if (isRightArea && d.X < 0) anglePenalty -= 0.4f; // 右侧区域的锚点优先向左
                            else if (d.X < 0) anglePenalty -= 0.4f; // 中间区域轻微偏好左侧
                            else anglePenalty -= 0.1f; // 减少中间区域右侧偏好

                            // 计算初始成本
                            float cost = dist + 4.0f * (1.0f - anglePenalty); // 注意这里的系数调整，平衡距离和角度惩罚

                            // 检测链接线（从锚点到标注中心）与其他曲线的相交情况，计算相交惩罚
                            int intersections = 0; // 相交计数
                            for (int k = 0; k < curvePolylines.Count; k++)
                            {
                                if (k == ci) continue; // 跳过自己的曲线
                                var otherPoly = curvePolylines[k];
                                for (int pi = 0; pi < otherPoly.Count - 1; pi++)
                                {
                                    if (SegmentsIntersect(anchor, center, otherPoly[pi], otherPoly[pi + 1]))
                                    {
                                        intersections++;
                                        break; // 每条曲线只计算一次相交
                                    }
                                }
                            }

                            // 计算空白度得分
                            float whitespaceScore = CalculateWhitespaceScore(rect, center, curvePolylines, ci);
                            int nearbyCurvesCount = CountNearbyCurves(rect, curvePolylines, ci, 20f); // 20像素范围内

                            // 计算方向特定的空白距离分数
                            float directionWhitespaceScore = CalculateDirectionalWhitespace(anchor, direction, curvePolylines, ci);

                            // 修改成本函数，加入相交惩罚和空白度奖励
                            float intersectionPenalty = intersections * 100f; // 相交惩罚权重
                            float whitespaceReward = whitespaceScore * 2f; // 空白度奖励权重
                            float nearbyCurvesPenalty = nearbyCurvesCount * 5f; // 附近曲线点惩罚

                            // 确定标注位置所在的区域，用于后续空间分布均衡性优化
                            string region = DetermineRegion(rect, width, height); // 获取标注所在的区域标识

                            // 计算方向空白距离奖励
                            // 方向空白距离是一个关键因素，特别是对于上下方向，权重设为很大
                            // 这样会优先选择空白距离更大的方向
                            float directionWhitespaceReward = directionWhitespaceScore * 5f; // 权重为5，这是一个很大的权重

                            // 空间分布均衡性奖励机制：通过差异化奖励引导标注分布更均匀
                            // 针对不同区域设置不同奖励/惩罚，优化整体布局
                            float regionDistributionReward = 0f;
                            // 左侧区域奖励：鼓励标注出现在左侧，平衡分布
                            if (region.Contains("Left"))
                                regionDistributionReward += 10f; // 左侧区域奖励
                            // 右侧区域惩罚：减少标注集中在右侧的情况
                            else if (region.Contains("Right"))
                                regionDistributionReward -= 5f; // 右侧区域惩罚

                            // 对角区域奖励：进一步优化空间分布的均衡性，避免标注聚集
                            if (region == "Left-Top" || region == "Left-Bottom" ||
                                region == "Right-Top" || region == "Right-Bottom")
                                regionDistributionReward += 5f; // 对角区域额外奖励

                            // 总成本 = 基础成本 + 相交惩罚 - 空白度奖励 + 附近曲线惩罚 - 方向空白距离奖励 - 区域分布奖励
                            // 注意：奖励项在总成本中是减去的，因为我们希望最小化总成本，而奖励会降低成本
                            float totalCost = cost + intersectionPenalty - whitespaceReward + nearbyCurvesPenalty - directionWhitespaceReward - regionDistributionReward;

                            // 创建候选标注对象，存储所有相关信息供后续选择优化
                            var cand = new Candidate
                            {
                                Id = nextId++, // 唯一标识符
                                CurveId = ci, // 关联曲线ID
                                Rect = rect, // 标注矩形区域
                                Anchor = anchor, // 锚点位置
                                Cost = totalCost, // 计算得到的总成本
                                Text = labelText, // 标注文本内容
                                LeaderLineIntersections = intersections, // 链接线相交数
                                WhitespaceScore = whitespaceScore, // 空白度得分
                                NearbyCurvesCount = nearbyCurvesCount, // 附近曲线点数量
                                DirectionWhitespaceScore = directionWhitespaceScore, // 方向空白距离分数
                                Direction = direction, // 方向标识
                                Region = region // 区域标识
                            };
                            allCandidates.Add(cand); // 添加到候选列表中
                        }
                    }
                }
            }

            Console.WriteLine($"Generated {allCandidates.Count} candidates.");

            // 添加调试信息：输出每个曲线的最佳候选位置
            for (int ci = 0; ci < curvePolylines.Count; ci++)
            {
                var curveCandidates = allCandidates.Where(c => c.CurveId == ci).ToList();
                if (curveCandidates.Any())
                {
                    var bestCandidate = curveCandidates.OrderBy(c => c.Cost).First();
                    // 通过标注矩形Y坐标和锚点Y坐标比较判断方向
                    string direction = bestCandidate.Rect.Y < bestCandidate.Anchor.Y ? "上方" : (bestCandidate.Rect.Y > bestCandidate.Anchor.Y ? "下方" : "水平");
                    string curveColor = ci == 0 ? "红" : (ci == 1 ? "绿" : (ci == 2 ? "蓝" : (ci == 3 ? "黄" : "紫")));
                    Console.WriteLine($"{curveColor}线标注位置: {direction}, Y坐标: {bestCandidate.Rect.Y}, 锚点Y: {bestCandidate.Anchor.Y}, 成本: {bestCandidate.Cost}");
                }
            }

            // 构建冲突对：
            // - 当两个候选标注的矩形区域相交时形成候选-候选冲突
            // - 当候选标注与其他曲线的折线相交时也形成冲突
            // 这些冲突信息将用于后续建立ILP求解器的约束条件
            var conflictPairs = new List<(int, int)>();
            // For speed, we'll group by spatial grid (coarse)
            int gridSize = 80;
            var buckets = new Dictionary<(int, int), List<Candidate>>();
            Func<PointF, (int, int)> bucketKey = (p) => ((int)p.X / gridSize, (int)p.Y / gridSize);
            foreach (var c in allCandidates)
            {
                var k = bucketKey(new PointF(c.Rect.X + c.Rect.Width / 2f, c.Rect.Y + c.Rect.Height / 2f));
                if (!buckets.ContainsKey(k)) buckets[k] = new List<Candidate>();
                buckets[k].Add(c);
            }

            // candidate-candidate overlaps
            for (int i = 0; i < allCandidates.Count; i++)
            {
                var ci = allCandidates[i];
                // check nearby buckets
                var baseKey = bucketKey(new PointF(ci.Rect.X + ci.Rect.Width / 2f, ci.Rect.Y + ci.Rect.Height / 2f));
                for (int bx = baseKey.Item1 - 1; bx <= baseKey.Item1 + 1; bx++)
                {
                    for (int by = baseKey.Item2 - 1; by <= baseKey.Item2 + 1; by++)
                    {
                        if (!buckets.ContainsKey((bx, by))) continue;
                        foreach (var cj in buckets[(bx, by)])
                        {
                            if (ci.Id >= cj.Id) continue;
                            if (ci.CurveId == cj.CurveId) continue; // candidates from same curve don't conflict (we only choose one per curve)
                            if (ci.Rect.IntersectsWith(cj.Rect))
                            {
                                conflictPairs.Add((ci.Id, cj.Id));
                            }
                        }
                    }
                }
            }

            // candidate vs other curve intersection
            foreach (var cand in allCandidates)
            {
                for (int k = 0; k < curvePolylines.Count; k++)
                {
                    if (k == cand.CurveId) continue; // skip own
                    var poly = curvePolylines[k];
                    // check if any poly point inside rect quickly
                    bool conflict = false;
                    foreach (var p in poly)
                    {
                        if (cand.Rect.Contains(p)) { conflict = true; break; }
                    }
                    if (conflict)
                    {
                        // mark conflict with all candidates that are this cand (single)
                        // we mark conflict pair between this cand and **all candidates of curves that correspond**?
                        // simpler: add conflict pair between cand and a virtual candidate of that curve? Not needed.
                        // Instead conflict between cand and each candidate of curve k (so solver can't place a label over a curve).
                        // Find all candidates of curve k
                        foreach (var other in allCandidates.Where(x => x.CurveId == k))
                        {
                            if (cand.Id < other.Id) conflictPairs.Add((cand.Id, other.Id));
                            else conflictPairs.Add((other.Id, cand.Id));
                        }
                        continue;
                    }
                    // else check line segment intersection with rect
                    for (int pi = 0; pi < poly.Count - 1 && !conflict; pi++)
                    {
                        var p1 = poly[pi];
                        var p2 = poly[pi + 1];
                        if (SegmentIntersectsRect(p1, p2, cand.Rect))
                        {
                            conflict = true;
                        }
                    }
                    if (conflict)
                    {
                        foreach (var other in allCandidates.Where(x => x.CurveId == k))
                        {
                            if (cand.Id < other.Id) conflictPairs.Add((cand.Id, other.Id));
                            else conflictPairs.Add((other.Id, cand.Id));
                        }
                    }
                }
            }

            // Deduplicate pairs (normalize)
            var uniq = new HashSet<(int, int)>();
            foreach (var (a, b) in conflictPairs)
            {
                var t = a < b ? (a, b) : (b, a);
                if (t.Item1 == t.Item2) continue;
                uniq.Add(t);
            }
            var conflictList = uniq.ToList();
            Console.WriteLine($"Conflict pairs: {conflictList.Count}");

            // 使用OR-Tools构建整数线性规划(ILP)求解器
            // 整数线性规划用于在满足各种约束条件的情况下找到最优解
            var solver = Solver.CreateSolver("CBC_MIXED_INTEGER_PROGRAMMING");
            if (solver == null)
            {
                Console.WriteLine("Could not create solver (OR-Tools). Falling back to greedy.");
            }

            var varMap = new Dictionary<int, Variable>();
            if (solver != null)
            {
                foreach (var c in allCandidates)
                {
                    // 为每个候选标注创建一个0-1变量，表示是否选择该标注
                    // x[c.Id] = 1表示选择该候选标注，x[c.Id] = 0表示不选择
                    varMap[c.Id] = solver.MakeIntVar(0, 1, $"x{c.Id}");
                }

                // 约束条件1：每条曲线必须恰好选择一个候选标注
                // 确保每条曲线都有且仅有一个标注
                int totalCurves = curves.Count;
                for (int ci = 0; ci < totalCurves; ci++)
                {
                    var cons = solver.MakeConstraint(1, 1, $"oneChoice_curve{ci}");
                    foreach (var c in allCandidates.Where(x => x.CurveId == ci))
                    {
                        cons.SetCoefficient(varMap[c.Id], 1);
                    }
                }

                // 约束条件2：冲突的候选对最多只能选择一个
                // 确保最终选择的标注不会相互重叠或与其他曲线相交
                foreach (var (a, b) in conflictList)
                {
                    var cons = solver.MakeConstraint(0, 1, $"conf_{a}_{b}");
                    cons.SetCoefficient(varMap[a], 1);
                    cons.SetCoefficient(varMap[b], 1);
                }

                // 目标函数：最小化所有选中候选的总成本
                // 通过权重系数使总成本最小化，从而实现整体最优布局
                var objective = solver.Objective();
                foreach (var c in allCandidates)
                {
                    objective.SetCoefficient(varMap[c.Id], c.Cost);
                }
                objective.SetMinimization();

                // 运行ILP求解器
                // CBC是一种混合整数线性规划求解器，能够处理0-1变量和线性约束条件
                Console.WriteLine("Solving ILP...");
                var resultStatus = solver.Solve();

                HashSet<int> chosenIds = new HashSet<int>();
                if (resultStatus == Solver.ResultStatus.OPTIMAL || resultStatus == Solver.ResultStatus.FEASIBLE)
                {
                    Console.WriteLine($"Solver result: {resultStatus}");
                    foreach (var c in allCandidates)
                    {
                        var v = varMap[c.Id];
                        if (v.SolutionValue() > 0.5)
                        {
                            chosenIds.Add(c.Id);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ILP solver failed or found no feasible solution. Falling back to greedy.");
                }

                // If chosenIds not complete (maybe ILP not constructed or infeasible), fallback greedy
                if (chosenIds.Count < curves.Count)
                {
                    var greedy = GreedySelect(allCandidates, conflictList, curves.Count);
                    chosenIds = greedy;
                }

                // produce output image
                DrawResult(width, height, padding, curves, curvePolylines, allCandidates, chosenIds, worldToScreen, "output.png");
                Console.WriteLine("Saved output.png");

            }
            else
            {
                // fallback greedy choose
                var greedy = GreedySelect(allCandidates, conflictList, curves.Count);
                DrawResult(width, height, padding, curves, curvePolylines, allCandidates, greedy, worldToScreen, "output.png");
                Console.WriteLine("Saved output.png (greedy)");
            }

            Console.WriteLine("Done. Press any key to exit.");
            //Console.ReadKey();
        }

        // 贪心选择算法实现：当ILP求解失败时的备选方案
        // 对于每条曲线，选择成本最低且与已选标注不冲突的候选标注
        static HashSet<int> GreedySelect(List<Candidate> allCandidates, List<(int, int)> conflictPairs, int curveCount)
        {
            var chosen = new HashSet<int>(); // 存储最终选择的候选标注ID
            // 构建冲突映射表：记录每个候选标注与哪些其他候选标注冲突
            var confMap = new Dictionary<int, HashSet<int>>();
            foreach (var c in allCandidates) confMap[c.Id] = new HashSet<int>();
            foreach (var (a, b) in conflictPairs)
            {
                confMap[a].Add(b);
                confMap[b].Add(a);
            }

            // 按曲线逐个处理，为每条曲线选择最佳候选标注
            for (int ci = 0; ci < curveCount; ci++)
            {
                // 获取当前曲线的所有候选标注，并按成本升序排序
                var cands = allCandidates.Where(x => x.CurveId == ci).OrderBy(x => x.Cost).ToList();
                bool placed = false;

                // 尝试找到成本最低且不冲突的候选标注
                foreach (var cand in cands)
                {
                    bool ok = true;
                    // 检查是否与已选择的标注冲突
                    foreach (var chosenId in chosen)
                    {
                        if (confMap[cand.Id].Contains(chosenId)) { ok = false; break; }
                    }

                    // 如果没有冲突，选择该候选标注
                    if (ok)
                    {
                        chosen.Add(cand.Id);
                        placed = true;
                        break;
                    }
                }

                // 如果找不到无冲突的候选，作为最后手段，选择成本最低的候选（忽略冲突）
                if (!placed && cands.Count > 0)
                {
                    chosen.Add(cands.First().Id);
                }
            }
            // 将调试信息写入文件而不是控制台，确保能捕获所有输出
            try
            {
                string debugLog = "=== 最终选择的标注位置 ===\n";
                foreach (var candId in chosen)
                {
                    var cand = allCandidates.First(c => c.Id == candId);
                    string direction = cand.Rect.Y < cand.Anchor.Y ? "上方" : (cand.Rect.Y > cand.Anchor.Y ? "下方" : "水平");
                    string curveColor = cand.CurveId == 0 ? "红" : (cand.CurveId == 1 ? "绿" : (cand.CurveId == 2 ? "蓝" : (cand.CurveId == 3 ? "黄" : "紫")));
                    debugLog += $"{curveColor}线最终位置: {direction}, Y坐标: {cand.Rect.Y}, 锚点Y: {cand.Anchor.Y}, 成本: {cand.Cost}\n";
                }
                File.WriteAllText("debug_info.txt", debugLog);
                Console.WriteLine("调试信息已写入 debug_info.txt 文件");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"写入调试文件时出错: {ex.Message}");
            }

            return chosen;
        }

        // 绘制结果：生成包含曲线、锚点和选中标注的最终图像
        // 参数说明：
        // - width/height: 图像尺寸
        // - padding: 边界留白
        // - curves: 二次曲线列表
        // - curvePolylines: 曲线的折线表示
        // - allCandidates: 所有候选标注位置
        // - chosenIds: 最终选择的候选标注ID
        // - worldToScreen: 世界坐标到屏幕坐标的映射函数
        // - outFile: 输出文件名
        static void DrawResult(int width, int height, int padding, List<QuadCurve> curves, List<List<PointF>> curvePolylines,
            List<Candidate> allCandidates, HashSet<int> chosenIds, Func<double, double, PointF> worldToScreen, string outFile)
        {
            using (var bmp = new Bitmap(width, height))
            using (var g = Graphics.FromImage(bmp))
            {
                // 设置绘图质量并清除背景为白色
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.White);

                // 绘制坐标轴边框
                using (var pen = new Pen(Color.LightGray, 1))
                {
                    g.DrawRectangle(pen, padding - 1, padding - 1, width - padding * 2 + 2, height - padding * 2 + 2);
                }

                // 曲线颜色调色板
                var colors = new Color[] { Color.DarkBlue, Color.DarkRed, Color.DarkGreen, Color.DarkOrange, Color.Purple };

                // 绘制所有曲线
                for (int i = 0; i < curvePolylines.Count; i++)
                {
                    var poly = curvePolylines[i];
                    using (var pen = new Pen(colors[i % colors.Length], 2))
                    {
                        g.DrawLines(pen, poly.ToArray());
                    }
                }

                // 绘制所有锚点（灰色小圆点）
                foreach (var c in allCandidates)
                {
                    var a = c.Anchor;
                    g.FillEllipse(Brushes.Gray, a.X - 2, a.Y - 2, 4, 4);
                }

                // 绘制选中的标注
                foreach (var id in chosenIds)
                {
                    var c = allCandidates.First(x => x.Id == id);
                    var rect = c.Rect;
                    var center = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);

                    // 测量文本大小，用于确定水平线长度
                    using (var font = new Font("Arial", 12))
                    {
                        SizeF textSize = g.MeasureString(c.Text, font);

                        // 计算斜线方向：根据曲线斜率和标注位置确定特定角度
                        // 固定斜线长度
                        float diagonalLength = 30f;

                        // 确定标注是在上方还是下方
                        bool isAbove = c.Anchor.Y > center.Y; // 如果锚点Y坐标大于中心点Y坐标，则标注在上方

                        // 查找当前曲线和对应的折线
                        int curveIndex = -1;
                        List<PointF> polyline = null;
                        for (int i = 0; i < curvePolylines.Count; i++)
                        {
                            var poly = curvePolylines[i];
                            if (poly.Any(p => Math.Abs(p.X - c.Anchor.X) < 1e-3 && Math.Abs(p.Y - c.Anchor.Y) < 1e-3))
                            {
                                curveIndex = i;
                                polyline = poly;
                                break;
                            }
                        }

                        // 计算曲线在锚点处的斜率
                        float slope = 0;
                        if (polyline != null)
                        {
                            // 查找锚点在折线中的近似位置
                            int closestIndex = 0;
                            float minDist = float.MaxValue;
                            for (int i = 0; i < polyline.Count; i++)
                            {
                                float dist = (float)Math.Sqrt(
                                    Math.Pow(polyline[i].X - c.Anchor.X, 2) +
                                    Math.Pow(polyline[i].Y - c.Anchor.Y, 2));
                                if (dist < minDist)
                                {
                                    minDist = dist;
                                    closestIndex = i;
                                }
                            }

                            // 计算斜率（使用相邻点）
                            if (closestIndex > 0 && closestIndex < polyline.Count - 1)
                            {
                                float dx1 = polyline[closestIndex + 1].X - polyline[closestIndex - 1].X;
                                float dy1 = polyline[closestIndex + 1].Y - polyline[closestIndex - 1].Y;
                                if (Math.Abs(dx1) > 1e-3)
                                {
                                    slope = dy1 / dx1; // 计算斜率
                                }
                            }
                        }

                        // 根据斜率和位置确定斜线角度
                        double angleDegrees = 0;
                        if (isAbove)
                        {
                            // 标注在上方
                            angleDegrees = slope >= 0 ? 110 : 70; // 斜率正->110°, 斜率负->70°
                        }
                        else
                        {
                            // 标注在下方
                            angleDegrees = slope >= 0 ? 290 : 250; // 斜率正->290°, 斜率负->250°
                        }

                        // 将角度转换为弧度
                        double angleRadians = angleDegrees * Math.PI / 180;

                        // 计算斜线终点（基于角度和固定长度）
                        PointF diagonalEnd = new PointF(
                            c.Anchor.X + (float)(Math.Cos(angleRadians) * diagonalLength),
                            c.Anchor.Y - (float)(Math.Sin(angleRadians) * diagonalLength) // Y轴向下为正，需要取负
                        );

                        // 绘制斜线（实线替代虚线）
                        using (var pen = new Pen(Color.Black, 1))
                        {
                            g.DrawLine(pen, c.Anchor, diagonalEnd);
                        }

                        // 确定水平线方向
                        // 根据斜线终点和文本中心的位置关系确定水平线方向
                        // 如果文本中心在斜线终点的右侧，则水平线向右
                        // 否则水平线向左
                        bool horizontalRight = center.X > diagonalEnd.X;

                        // 计算水平线终点
                        PointF horizontalEnd = new PointF(
                            horizontalRight ? diagonalEnd.X + textSize.Width : diagonalEnd.X - textSize.Width,
                            diagonalEnd.Y
                        );

                        // 绘制水平线
                        using (var pen = new Pen(Color.Black, 1))
                        {
                            g.DrawLine(pen, diagonalEnd, horizontalEnd);
                        }

                        // 绘制文本在水平线上，文本方向与水平线一致
                        // 文本位置根据水平线方向调整
                        float textX = horizontalRight ? diagonalEnd.X : horizontalEnd.X;
                        using (var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
                        {
                            g.DrawString(c.Text, font, Brushes.Black, textX, diagonalEnd.Y, sf);
                        }
                    }
                }

                // 也可以选择绘制未选中的候选标注（当前未启用）
                // 保存图像
                bmp.Save(outFile, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        /// <summary>
        /// 判断线段是否与矩形相交
        /// </summary>
        /// <param name="p1">线段起点</param>
        /// <param name="p2">线段终点</param>
        /// <param name="rect">目标矩形</param>
        /// <returns>如果线段与矩形相交，则返回true</returns>
        static bool SegmentIntersectsRect(PointF p1, PointF p2, RectangleF rect)
        {
            // First: if either endpoint inside rect -> true
            if (rect.Contains(p1) || rect.Contains(p2)) return true;

            // Check segment against each rectangle edge
            var r1 = new PointF(rect.Left, rect.Top);
            var r2 = new PointF(rect.Right, rect.Top);
            var r3 = new PointF(rect.Right, rect.Bottom);
            var r4 = new PointF(rect.Left, rect.Bottom);

            if (SegmentsIntersect(p1, p2, r1, r2)) return true;
            if (SegmentsIntersect(p1, p2, r2, r3)) return true;
            if (SegmentsIntersect(p1, p2, r3, r4)) return true;
            if (SegmentsIntersect(p1, p2, r4, r1)) return true;
            return false;
        }

        /// <summary>
        /// 判断线段是否相交
        /// </summary>
        /// <param name="p1">第一条线段的起点</param>
        /// <param name="p2">第一条线段的终点</param>
        /// <param name="q1">第二条线段的起点</param>
        /// <param name="q2">第二条线段的终点</param>
        /// <returns>如果线段相交，则返回true</returns>
        static bool SegmentsIntersect(PointF p1, PointF p2, PointF q1, PointF q2)
        {
            float o1 = Orientation(p1, p2, q1);
            float o2 = Orientation(p1, p2, q2);
            float o3 = Orientation(q1, q2, p1);
            float o4 = Orientation(q1, q2, p2);

            if (o1 * o2 < 0 && o3 * o4 < 0) return true;

            // special colinear cases
            if (o1 == 0 && OnSegment(p1, q1, p2)) return true;
            if (o2 == 0 && OnSegment(p1, q2, p2)) return true;
            if (o3 == 0 && OnSegment(q1, p1, q2)) return true;
            if (o4 == 0 && OnSegment(q1, p2, q2)) return true;

            return false;
        }

        static float Orientation(PointF a, PointF b, PointF c)
        {
            // cross product (b-a) x (c-a)
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        }

        /// <summary>
        /// 判断点是否在线段上
        /// </summary>
        /// <param name="a">线段起点</param>
        /// <param name="b">测试点</param>
        /// <param name="c">线段终点</param>
        /// <returns>如果b在线段ac上，则返回true</returns>
        static bool OnSegment(PointF a, PointF b, PointF c)
        {
            return Math.Min(a.X, c.X) <= b.X + 1e-6 && b.X <= Math.Max(a.X, c.X) + 1e-6 &&
                   Math.Min(a.Y, c.Y) <= b.Y + 1e-6 && b.Y <= Math.Max(a.Y, c.Y) + 1e-6;
        }

        /// <summary>
        /// 计算空白度得分
        /// </summary>
        /// <param name="rect">标注矩形</param>
        /// <param name="center">标注中心</param>
        /// <param name="curvePolylines">所有曲线的折线表示</param>
        /// <param name="currentCurveId">当前曲线ID</param>
        /// <returns>空白度得分，范围0-1，越高越好</returns>
        /// <remarks>
        /// 测量标注矩形周围的空白区域大小
        /// 得分越高表示空白区域越大，标注位置越好
        /// 通过计算矩形周围的最小距离来评估空白度
        /// </remarks>
        static float CalculateWhitespaceScore(RectangleF rect, PointF center, List<List<PointF>> curvePolylines, int currentCurveId)
        {
            // 扩展矩形以检查周围区域
            float expansion = 20f; // 扩展20像素
            RectangleF expandedRect = new RectangleF(
                rect.X - expansion,
                rect.Y - expansion,
                rect.Width + 2 * expansion,
                rect.Height + 2 * expansion
            );

            int curvePointsInside = 0;
            int totalPointsToCheck = 0;

            // 检查其他曲线在扩展区域内的点数
            foreach (var (currentPoly, curveIndex) in curvePolylines.Select((poly, index) => (poly, index)))
            {
                if (curveIndex == currentCurveId) continue; // 跳过当前曲线

                // 为了效率，只检查部分点（例如每10个点检查一个）
                for (int i = 0; i < currentPoly.Count; i += 5)
                {
                    totalPointsToCheck++;
                    if (expandedRect.Contains(currentPoly[i]))
                    {
                        curvePointsInside++;
                    }
                }
            }

            // 计算距离区域最近的其他曲线点的距离
            float minDistance = float.MaxValue;
            foreach (var (currentPoly, curveIndex) in curvePolylines.Select((poly, index) => (poly, index)))
            {
                if (curveIndex == currentCurveId) continue;

                foreach (var p in currentPoly)
                {
                    float dist = DistanceToRect(p, expandedRect);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                    }
                }
            }

            // 空白度得分：区域内曲线点越少，距离其他曲线越远，得分越高
            float densityFactor = totalPointsToCheck > 0 ? 1.0f - (float)curvePointsInside / totalPointsToCheck : 1.0f;
            float distanceFactor = minDistance / 100f; // 归一化距离因素
            return densityFactor * 0.7f + Math.Min(distanceFactor, 1.0f) * 0.3f;
        }

        /// <summary>
        /// 计算点到矩形的最小距离
        /// </summary>
        /// <param name="p">测试点</param>
        /// <param name="rect">目标矩形</param>
        /// <returns>点到矩形的最小距离</returns>
        static float DistanceToRect(PointF p, RectangleF rect)
        {
            float dx = Math.Max(Math.Max(rect.Left - p.X, 0), p.X - rect.Right);
            float dy = Math.Max(Math.Max(rect.Top - p.Y, 0), p.Y - rect.Bottom);
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// 计算方向空白距离分数
        /// </summary>
        /// <param name="anchor">锚点位置</param>
        /// <param name="direction">标注方向（Up、Down、Left、Right）</param>
        /// <param name="curvePolylines">所有曲线的折线表示</param>
        /// <param name="currentCurveId">当前曲线ID</param>
        /// <returns>方向空白距离分数，值越大越好</returns>
        /// <remarks>
        /// 测量标注在其方向上的空白空间大小
        /// 用于评估标注在特定方向上的合适程度
        /// </remarks>
        static float CalculateDirectionalWhitespace(PointF anchor, string direction, List<List<PointF>> curvePolylines, int currentCurveId)
        {
            float maxCheckDistance = 200f; // 最大检查距离
            float nearestDistance = maxCheckDistance; // 初始化为最大距离

            // 检查所有其他曲线
            foreach (var (poly, curveIndex) in curvePolylines.Select((poly, index) => (poly, index)))
            {
                if (curveIndex == currentCurveId) continue;

                foreach (var p in poly)
                {
                    // 根据方向筛选相关点
                    bool isRelevant = false;
                    switch (direction)
                    {
                        case "Up":
                            isRelevant = p.Y < anchor.Y; // 只考虑上方点
                            break;
                        case "Down":
                            isRelevant = p.Y > anchor.Y; // 只考虑下方点
                            break;
                        case "Left":
                            isRelevant = p.X < anchor.X; // 只考虑左侧点
                            break;
                        case "Right":
                            isRelevant = p.X > anchor.X; // 只考虑右侧点
                            break;
                        default:
                            isRelevant = true;
                            break;
                    }

                    if (!isRelevant) continue;

                    // 计算实际距离
                    float dx = p.X - anchor.X;
                    float dy = p.Y - anchor.Y;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                    if (distance < nearestDistance && distance <= maxCheckDistance)
                    {
                        nearestDistance = distance;
                    }
                }
            }

            // 返回归一化的空白距离分数（距离越大，分数越高）
            return nearestDistance / maxCheckDistance * 100f;
        }

        // 根据方向向量获取方向标识
        /// <summary>
        /// 区域检测方法：确定标注中心所在的区域
        /// </summary>
        /// <param name="rect">标注的矩形区域</param>
        /// <param name="width">画布宽度</param>
        /// <param name="height">画布高度</param>
        /// <returns>区域标识，格式为"水平区域-垂直区域"，如"Left-Top"</returns>
        /// <remarks>
        /// 区域划分规则：
        /// 水平方向：Left (左30%)、Middle (中40%)、Right (右30%)
        /// 垂直方向：Top (上30%)、Middle (中40%)、Bottom (下30%)
        /// 用于空间分布均衡性优化，确保标注在图片中均匀分布
        /// </remarks>
        static string DetermineRegion(RectangleF rect, int width, int height)
        {
            float centerX = rect.X + rect.Width / 2f;
            float centerY = rect.Y + rect.Height / 2f;

            // 水平区域划分
            string horizontalRegion;
            if (centerX < width * 0.3) horizontalRegion = "Left";
            else if (centerX > width * 0.7) horizontalRegion = "Right";
            else horizontalRegion = "Middle";

            // 垂直区域划分
            string verticalRegion;
            if (centerY < height * 0.3) verticalRegion = "Top";
            else if (centerY > height * 0.7) verticalRegion = "Bottom";
            else verticalRegion = "Middle";

            // 组合区域标识
            return $"{horizontalRegion}-{verticalRegion}";
        }

        /// <summary>
        /// 获取方向标识字符串
        /// </summary>
        /// <param name="direction">方向向量</param>
        /// <returns>方向标识：Up、Down、Left或Right</returns>
        static string GetDirectionString(PointF direction)
        {
            // 归一化方向向量
            float length = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
            if (length < 1e-6) return "Up"; // 防止零向量

            float nx = direction.X / length;
            float ny = direction.Y / length;

            // 确定主方向
            if (Math.Abs(ny) > Math.Abs(nx))
            {
                // 垂直方向为主
                return ny < 0 ? "Up" : "Down";
            }
            else
            {
                // 水平方向为主
                return nx < 0 ? "Left" : "Right";
            }
        }

        /// <summary>
        /// 计算附近曲线点数量
        /// </summary>
        /// <param name="rect">标注矩形</param>
        /// <param name="curvePolylines">所有曲线的折线表示</param>
        /// <param name="currentCurveId">当前曲线ID</param>
        /// <param name="maxDistance">最大检测距离</param>
        /// <returns>标注矩形附近的曲线点数量</returns>
        static int CountNearbyCurves(RectangleF rect, List<List<PointF>> curvePolylines, int currentCurveId, float maxDistance)
        {
            int count = 0;
            float maxDistSquared = maxDistance * maxDistance;
            PointF center = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);

            foreach (var (currentPoly, curveIndex) in curvePolylines.Select((poly, index) => (poly, index)))
            {
                if (curveIndex == currentCurveId) continue;

                foreach (var p in currentPoly)
                {
                    float dx = p.X - center.X;
                    float dy = p.Y - center.Y;
                    if (dx * dx + dy * dy <= maxDistSquared)
                    {
                        count++;
                        break; // 每条曲线只计数一次
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// 计算曲线在指定位置的法线方向
        /// </summary>
        /// <param name="polyline">曲线的折线表示</param>
        /// <param name="index">当前点在折线中的索引</param>
        /// <returns>法线方向向量（归一化）</returns>
        /// <remarks>
        /// 用于改进的角度惩罚计算，使标注方向与曲线方向协调
        /// 通过计算曲线切线，然后旋转90度得到法线方向
        /// </remarks>
        /// <summary>
        /// 检查锚点上方是否有足够空间放下1.5倍高度的标注
        /// </summary>
        /// <param name="anchor">锚点位置</param>
        /// <param name="labelSize">标注大小</param>
        /// <param name="curvePolylines">所有曲线的折线表示</param>
        /// <param name="currentCurveId">当前曲线ID</param>
        /// <returns>如果有足够空间返回true，否则返回false</returns>
        static bool HasSufficientSpaceAbove(PointF anchor, SizeF labelSize, List<List<PointF>> curvePolylines, int currentCurveId)
        {
            // 放宽上方检查区域：减小检查高度，降低严格程度
            float requiredHeight = labelSize.Height * 1.2f; // 从1.5倍降至1.2倍
            float requiredWidth = labelSize.Width * 1.1f; // 略微放宽宽度

            // 创建一个上方的矩形区域
            RectangleF checkRect = new RectangleF(
                anchor.X - requiredWidth / 2f,
                anchor.Y - requiredHeight,
                requiredWidth,
                requiredHeight
            );

            // 检查是否超出边界 - 仍然需要确保不完全超出边界
            if (checkRect.Top < -50) return false; // 允许轻微超出上边界

            // 更宽松的检查：允许少量点在区域内
            int pointCountInArea = 0;
            bool hasSignificantIntersection = false;

            foreach (var (poly, curveIndex) in curvePolylines.Select((poly, index) => (poly, index)))
            {
                if (curveIndex == currentCurveId) continue;

                // 允许最多3个点在区域内
                foreach (var p in poly)
                {
                    if (checkRect.Contains(p))
                    {
                        pointCountInArea++;
                        if (pointCountInArea > 3) // 允许少量点存在
                            return false;
                    }
                }

                // 检查线段与矩形相交，但允许轻微相交
                int intersectionCount = 0;
                for (int i = 0; i < poly.Count - 1; i++)
                {
                    if (CheckSegmentIntersectsRect(poly[i], poly[i + 1], checkRect))
                    {
                        intersectionCount++;
                        if (intersectionCount > 1) // 允许最多1次轻微相交
                        {
                            hasSignificantIntersection = true;
                            break;
                        }
                    }
                }

                if (hasSignificantIntersection)
                    return false;
            }

            // 即使有少量点或轻微相交，只要满足基本条件，也认为上方有足够空间
            return true;
        }

        /// <summary>
        /// 检查线段是否与矩形相交
        /// </summary>
        static bool CheckSegmentIntersectsRect(PointF p1, PointF p2, RectangleF rect)
        {
            // 检查线段的两个端点是否在矩形内
            if (rect.Contains(p1) || rect.Contains(p2))
            {
                return true;
            }

            // 检查线段是否与矩形的四条边相交
            // 上边
            if (SegmentsIntersect(p1, p2, new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Top)))
                return true;
            // 下边
            if (SegmentsIntersect(p1, p2, new PointF(rect.Left, rect.Bottom), new PointF(rect.Right, rect.Bottom)))
                return true;
            // 左边
            if (SegmentsIntersect(p1, p2, new PointF(rect.Left, rect.Top), new PointF(rect.Left, rect.Bottom)))
                return true;
            // 右边
            if (SegmentsIntersect(p1, p2, new PointF(rect.Right, rect.Top), new PointF(rect.Right, rect.Bottom)))
                return true;

            return false;
        }

        static PointF CalculateCurveNormal(List<PointF> polyline, int index)
        {
            // 确保索引在有效范围内
            if (index <= 0 || index >= polyline.Count - 1)
            {
                // 对于端点，使用相邻点计算切线
                int nextIndex = index == 0 ? 1 : index - 1;
                float endTx = polyline[nextIndex].X - polyline[index].X;
                float endTy = polyline[nextIndex].Y - polyline[index].Y;

                // 法线垂直于切线
                float endNx = -endTy;
                float endNy = endTx;

                // 归一化
                float endLen = (float)Math.Sqrt(endNx * endNx + endNy * endNy);
                if (endLen > 1e-6)
                {
                    endNx /= endLen;
                    endNy /= endLen;
                }

                return new PointF(endNx, endNy);
            }

            // 对于中间点，使用前一个和后一个点计算切线
            PointF prev = polyline[index - 1];
            PointF curr = polyline[index];
            PointF next = polyline[index + 1];

            // 计算切线方向（使用三点估计）
            float midTx = next.X - prev.X;
            float midTy = next.Y - prev.Y;

            // 法线垂直于切线
            float midNx = -midTy;
            float midNy = midTx;

            // 归一化
            float midLen = (float)Math.Sqrt(midNx * midNx + midNy * midNy);
            if (midLen > 1e-6)
            {
                midNx /= midLen;
                midNy /= midLen;
            }

            return new PointF(midNx, midNy);
        }
    }
}
