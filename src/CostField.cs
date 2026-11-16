using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace AlgoDesign
{
    /// <summary>
    /// V4：成本場。決定「走進每一格要花多少」，就是設計判斷：
    /// 吸引點附近變貴（例如噪音源、日曬、不想讓人經過的地方），或用雜訊做出起伏的地形。
    /// 成本只會往上加，最小仍是 1，A* 的猜測才不會高估。
    /// </summary>
    public static class CostField
    {
        /// <summary>
        /// 每個點的 radius 範圍內加成本：越靠近點加越多，中心加 strength，邊緣加 0。
        /// 回傳受影響的格數。
        /// </summary>
        public static int AddAttractors(Grid grid, List<Point3d> points, double radius, double strength)
        {
            if (points == null || radius <= 0 || strength <= 0) return 0;

            var affected = new bool[grid.Width, grid.Height];
            int affectedCount = 0;

            foreach (Point3d point in points)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    for (int y = 0; y < grid.Height; y++)
                    {
                        Point3d center = grid.CellCenter(new Cell(x, y));
                        // 只看水平距離（忽略 Z）
                        double dx = center.X - point.X;
                        double dy = center.Y - point.Y;
                        double distance = Math.Sqrt(dx * dx + dy * dy);
                        if (distance >= radius) continue;

                        grid.Cost[x, y] += strength * (1.0 - distance / radius);
                        if (!affected[x, y])
                        {
                            affected[x, y] = true;
                            affectedCount++;
                        }
                    }
                }
            }
            return affectedCount;
        }

        /// <summary>
        /// 加上平滑雜訊：每 scale 格一個隨機高度，中間平滑內插，值在 0 到 strength 之間。
        /// 同一個 seed 每次結果相同。
        /// </summary>
        public static void AddNoise(Grid grid, double scale, double strength, int seed)
        {
            if (scale <= 0 || strength <= 0) return;

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    grid.Cost[x, y] += strength * SmoothNoise(x / scale, y / scale, seed);
                }
            }
        }

        /// <summary>
        /// 值雜訊（value noise）：整數格點各有一個 0～1 的隨機值，格點之間用平滑曲線內插。
        /// </summary>
        private static double SmoothNoise(double u, double v, int seed)
        {
            int u0 = (int)Math.Floor(u);
            int v0 = (int)Math.Floor(v);
            double fu = Fade(u - u0);
            double fv = Fade(v - v0);

            double bottom = Lerp(LatticeValue(u0, v0, seed), LatticeValue(u0 + 1, v0, seed), fu);
            double top = Lerp(LatticeValue(u0, v0 + 1, seed), LatticeValue(u0 + 1, v0 + 1, seed), fu);
            return Lerp(bottom, top, fv);
        }

        /// <summary>整數格點 (i, j) 的固定隨機值，0～1：用整數雜湊，不用 Random，才能隨意查任何格點。</summary>
        private static double LatticeValue(int i, int j, int seed)
        {
            unchecked
            {
                uint hash = (uint)(i * 374761393 + j * 668265263 + seed * 1442695041);
                hash = (hash ^ (hash >> 13)) * 1274126177;
                hash ^= hash >> 16;
                return (hash & 0xFFFFFF) / (double)0xFFFFFF;
            }
        }

        /// <summary>平滑曲線 6t⁵ − 15t⁴ + 10t³：兩端斜率為 0，接縫不會有折角。</summary>
        private static double Fade(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }
    }
}
