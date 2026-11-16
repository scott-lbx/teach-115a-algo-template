using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Rhino.Geometry;

namespace AlgoDesign
{
    /// <summary>
    /// GH 元件 path_search 的本體（Part 4 V1～V4）：同一個格子場景，
    /// 同時跑「基礎版 Dijkstra（圖版、4 鄰居、線性掃描）」與「進階版 PathSearch」，回報對照與驗證。
    /// </summary>
    public class PathSearchHost
    {
        /// <summary>進階版找到的路徑。</summary>
        public Polyline Path { get; private set; }

        /// <summary>基礎版找到的路徑（疊在一起比較）。</summary>
        public Polyline BasicPath { get; private set; }

        /// <summary>進階版的距離場上色網格。</summary>
        public Mesh Field { get; private set; }

        /// <summary>成本場上色網格（V4）。</summary>
        public Mesh CostMap { get; private set; }

        /// <summary>給 GH 的 out 看的對照與驗證文字。</summary>
        public string Report { get; private set; } = "";

        /// <summary>每次 GH 運算呼叫一次：重建場景，兩版各解一次。</summary>
        public void Run(Point3d origin, int width, int height, double cellSize, List<Curve> obstacles,
            Point3d start, Point3d goal, bool allowDiagonal, bool useAStar,
            List<Point3d> attractors, double attractorRadius, double attractorStrength,
            double noiseScale, double noiseStrength, int seed)
        {
            // 場景：格子、障礙、V4 成本場
            var grid = new Grid(origin, width, height, cellSize);
            int blockedCount = grid.BlockInside(obstacles);
            int attractedCount = CostField.AddAttractors(grid, attractors, attractorRadius, attractorStrength);
            CostField.AddNoise(grid, noiseScale, noiseStrength, seed);
            Cell startCell = grid.CellAt(start);
            Cell goalCell = grid.CellAt(goal);

            // 基礎版：格子轉成圖，4 鄰居、線性掃描（課堂 Part 3 手寫的那份）
            var timer = Stopwatch.StartNew();
            Graph graph = GridGraph.FromGrid(grid);
            var basic = new Dijkstra(graph);
            int startNode = GridGraph.NodeOf(grid, startCell);
            int goalNode = GridGraph.NodeOf(grid, goalCell);
            bool isBasicFound = grid.IsWalkable(startCell) && basic.Solve(startNode, goalNode);
            double basicMs = timer.Elapsed.TotalMilliseconds;

            // 進階版：依輸入切換 8 鄰居（V1）與 A*（V3），一律用優先佇列（V2）
            timer.Restart();
            var advanced = new PathSearch(grid) { AllowDiagonal = allowDiagonal, UseAStar = useAStar };
            bool isFound = advanced.Solve(startCell, goalCell);
            double advancedMs = timer.Elapsed.TotalMilliseconds;

            // 驗證用：同樣鄰居設定、不用 A* 的 Dijkstra
            var reference = new PathSearch(grid) { AllowDiagonal = allowDiagonal, UseAStar = false };
            reference.Solve(startCell, goalCell);

            // 輸出
            double basicCost = isBasicFound ? basic.TotalCost(goalNode) : double.MaxValue;
            Path = ToPolyline(advanced.BuildPath(startCell, goalCell).Select(grid.CellCenter));
            BasicPath = ToPolyline(basic.BuildPath(goalNode).Select(node => graph.Positions[node]));
            Field = FieldDisplay.BuildMesh(grid, advanced.Distance);
            CostMap = FieldDisplay.BuildMesh(grid, grid.Cost);

            var text = new StringBuilder();
            text.AppendLine($"格子 {grid.Width}×{grid.Height}，擋住 {blockedCount} 格，吸引點影響 {attractedCount} 格");
            text.AppendLine($"基礎版（4 鄰居、線性掃描）：{(isBasicFound ? "找到" : "找不到")}，"
                + $"成本 {basicCost:0.###}，定案 {basic.DoneCount} 格，{basicMs:0.0} ms");
            text.AppendLine($"進階版（{(allowDiagonal ? "8 鄰居" : "4 鄰居")}、{(useAStar ? "A*" : "Dijkstra")}、優先佇列）："
                + $"{(isFound ? "找到" : "找不到")}，成本 {advanced.TotalCost(goalCell):0.###}，"
                + $"定案 {advanced.DoneCount} 格，{advancedMs:0.0} ms");
            if (!allowDiagonal)
            {
                text.AppendLine("V2 驗證：4 鄰居時，優先佇列版成本要和基礎版相同 → "
                    + (IsSameCost(advanced.TotalCost(goalCell), basicCost) ? "✔" : "✘"));
            }
            if (useAStar)
            {
                text.AppendLine("V3 驗證：A* 成本要和 Dijkstra 相同 → "
                    + (IsSameCost(advanced.TotalCost(goalCell), reference.TotalCost(goalCell)) ? "✔" : "✘")
                    + $"；定案格數 A* {advanced.DoneCount} vs Dijkstra {reference.DoneCount}");
            }
            Report = text.ToString().TrimEnd();
        }

        // 點序列 → 折線；不到兩點回傳 null
        private static Polyline ToPolyline(IEnumerable<Point3d> points)
        {
            var list = points.ToList();
            return list.Count >= 2 ? new Polyline(list) : null;
        }

        // 兩個成本是否相同（斜向的 √2 加總會有極小的浮點誤差）
        private static bool IsSameCost(double a, double b)
        {
            if (a == double.MaxValue || b == double.MaxValue) return a == b;
            return Math.Abs(a - b) < 1e-6;
        }
    }
}
