using System;
using System.Collections.Generic;

namespace AlgoDesign
{
    /// <summary>
    /// Dijkstra 的進階版，課堂 Part 4 的 V1～V3 都在這裡：
    /// V1 可選 8 鄰居（斜向成本 ×√2）、V2 用優先佇列取代線性掃描、V3 可選 A*（加上「離終點還有多遠」的猜測）。
    /// 基礎版 Dijkstra.cs 保持原樣，拿來和這裡對照。
    /// </summary>
    public class PathSearch
    {
        private readonly Grid _grid;

        /// <summary>V1：true 用 8 鄰居，false 用 4 鄰居。</summary>
        public bool AllowDiagonal { get; set; }

        /// <summary>V3：true 用 A*，false 是一般 Dijkstra。</summary>
        public bool UseAStar { get; set; }

        /// <summary>從起點到每一格的最小成本；還走不到的是 double.MaxValue。</summary>
        public double[,] Distance { get; }

        /// <summary>每一格是從哪一格走過來的；起點與走不到的格子是 Cell.None。</summary>
        public Cell[,] Previous { get; }

        /// <summary>已定案的格子。</summary>
        public bool[,] Done { get; }

        /// <summary>定案了幾格；A* 應該比 Dijkstra 少。</summary>
        public int DoneCount { get; private set; }

        public PathSearch(Grid grid)
        {
            _grid = grid;
            Distance = new double[grid.Width, grid.Height];
            Previous = new Cell[grid.Width, grid.Height];
            Done = new bool[grid.Width, grid.Height];
        }

        /// <summary>從 start 找到 goal 定案就停。走得到回傳 true。</summary>
        public bool Solve(Cell start, Cell goal)
        {
            if (!_grid.IsWalkable(goal))
            {
                Reset();
                return false;
            }
            return Run(start, goal, false);
        }

        /// <summary>
        /// V5 距離場用：從 root 擴散到整張圖（不用 A*）。
        /// isTowardRoot 為 true 時，Distance 是「從這一格走到 root」的成本，Previous 指向往 root 的下一格。
        /// </summary>
        public void SolveAll(Cell root, bool isTowardRoot)
        {
            bool useAStar = UseAStar;
            UseAStar = false;
            Run(root, Cell.None, isTowardRoot);
            UseAStar = useAStar;
        }

        /// <summary>從 goal 沿 Previous 走回 start，反轉後回傳；goal 沒定案回傳空清單。</summary>
        public List<Cell> BuildPath(Cell start, Cell goal)
        {
            var path = new List<Cell>();
            if (!_grid.IsInside(goal) || !Done[goal.X, goal.Y]) return path;

            Cell current = goal;
            while (!current.IsNone)
            {
                path.Add(current);
                current = Previous[current.X, current.Y];
            }
            path.Reverse();
            return path;
        }

        /// <summary>走到 goal 的總成本；走不到回傳 double.MaxValue。</summary>
        public double TotalCost(Cell goal)
        {
            if (!_grid.IsInside(goal) || !Done[goal.X, goal.Y]) return double.MaxValue;
            return Distance[goal.X, goal.Y];
        }

        /// <summary>
        /// 主迴圈：和基礎版同樣兩句話，只是「找最便宜的」改成從堆積拿。
        /// goal 是 Cell.None 時擴散到整張圖。
        /// </summary>
        private bool Run(Cell start, Cell goal, bool isTowardRoot)
        {
            Reset();
            if (!_grid.IsWalkable(start)) return false;

            bool hasGoal = !goal.IsNone;
            double minCost = _grid.MinWalkableCost();
            var heap = new CellHeap();
            Distance[start.X, start.Y] = 0;
            heap.Push(start, Estimate(start, goal, hasGoal, minCost));

            while (heap.TryPop(out Cell current, out double _))
            {
                // 同一格可能被放進好幾次，已定案的舊紀錄直接跳過
                if (Done[current.X, current.Y]) continue;

                // 第一句：定案最便宜的
                Done[current.X, current.Y] = true;
                DoneCount++;
                if (hasGoal && current.IsSame(goal)) return true;

                // 第二句：更新它的鄰居
                List<Cell> neighbors = AllowDiagonal ? _grid.Neighbors8(current) : _grid.Neighbors4(current);
                foreach (Cell next in neighbors)
                {
                    if (Done[next.X, next.Y]) continue;

                    // 往 root 走時，實際行走方向是 next → current，成本要算走進 current 的
                    double stepCost = isTowardRoot ? _grid.StepCost(next, current) : _grid.StepCost(current, next);
                    double newDistance = Distance[current.X, current.Y] + stepCost;
                    if (newDistance < Distance[next.X, next.Y])
                    {
                        Distance[next.X, next.Y] = newDistance;
                        Previous[next.X, next.Y] = current;
                        heap.Push(next, newDistance + Estimate(next, goal, hasGoal, minCost));
                    }
                }
            }
            return !hasGoal;
        }

        /// <summary>
        /// V3：A* 的猜測，「最少還要花多少才到得了終點」。
        /// 4 鄰居用曼哈頓距離、8 鄰居用八方向距離，再乘上最小成本；只能少估、不能多估，結果才會和 Dijkstra 一樣短。
        /// 不用 A* 時回傳 0，就退回一般 Dijkstra。
        /// </summary>
        private double Estimate(Cell cell, Cell goal, bool hasGoal, double minCost)
        {
            if (!UseAStar || !hasGoal) return 0.0;

            int dx = Math.Abs(goal.X - cell.X);
            int dy = Math.Abs(goal.Y - cell.Y);
            double steps = AllowDiagonal
                ? Math.Max(dx, dy) + (Math.Sqrt(2.0) - 1.0) * Math.Min(dx, dy)
                : dx + dy;
            return steps * minCost;
        }

        /// <summary>清空三個陣列與計數。</summary>
        private void Reset()
        {
            DoneCount = 0;
            for (int x = 0; x < _grid.Width; x++)
            {
                for (int y = 0; y < _grid.Height; y++)
                {
                    Distance[x, y] = double.MaxValue;
                    Previous[x, y] = Cell.None;
                    Done[x, y] = false;
                }
            }
        }
    }
}
