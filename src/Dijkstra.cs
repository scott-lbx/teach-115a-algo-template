using System.Collections.Generic;

namespace AlgoDesign
{
    /// <summary>
    /// 最基礎的 Dijkstra：4 鄰居，每次掃過全部格子找最近的那一格（不用優先佇列）。
    /// 只記兩句話：定案最便宜的，更新它的鄰居。
    /// 參考解答（1117-path-solution 分支）：註解的 3-1 到 3-4 對應 starter 的 TODO。
    /// </summary>
    public class Dijkstra
    {
        private readonly Grid _grid;

        /// <summary>從起點走到每一格的最小成本；還走不到的是 double.MaxValue。</summary>
        public double[,] Distance { get; }

        /// <summary>每一格是從哪一格走過來的；起點與走不到的格子是 Cell.None。</summary>
        public Cell[,] Previous { get; }

        /// <summary>已定案的格子：最小成本已確定，之後不會再變。</summary>
        public bool[,] Done { get; }

        /// <summary>總共定案了幾格（之後拿來和 A* 比較誰找得比較少）。</summary>
        public int DoneCount { get; private set; }

        public Dijkstra(Grid grid)
        {
            _grid = grid;
            Distance = new double[grid.Width, grid.Height];
            Previous = new Cell[grid.Width, grid.Height];
            Done = new bool[grid.Width, grid.Height];
        }

        /// <summary>從 start 開始擴散，直到 goal 定案。走得到回傳 true。</summary>
        public bool Solve(Cell start, Cell goal)
        {
            Reset();
            if (!_grid.IsWalkable(start) || !_grid.IsWalkable(goal)) return false;

            // 3-1：起點的成本是 0
            Distance[start.X, start.Y] = 0;

            while (TryPickNearest(out Cell current))
            {
                // 3-2 第一句：定案最便宜的
                Done[current.X, current.Y] = true;
                DoneCount++;
                if (current.IsSame(goal)) return true;

                // 3-3 第二句：更新它的鄰居
                foreach (Cell next in _grid.Neighbors4(current))
                {
                    if (Done[next.X, next.Y]) continue;

                    double newDistance = Distance[current.X, current.Y] + _grid.Cost[next.X, next.Y];
                    if (newDistance < Distance[next.X, next.Y])
                    {
                        Distance[next.X, next.Y] = newDistance;
                        Previous[next.X, next.Y] = current;
                    }
                }
            }

            return false;
        }

        /// <summary>每次 Solve 前清空：Distance 全設 double.MaxValue、Previous 全設 Cell.None、Done 全設 false。</summary>
        private void Reset()
        {
            DoneCount = 0;

            // 3-1：兩層 for 掃過全部格子，設定三個陣列的初始值
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

        /// <summary>
        /// 第一句「定案最便宜的」：從還沒定案、而且已經走得到的格子裡，找 Distance 最小的那格。
        /// 找到回傳 true；全部定案或剩下的都走不到，回傳 false。
        /// </summary>
        private bool TryPickNearest(out Cell nearest)
        {
            nearest = Cell.None;
            double smallest = double.MaxValue;

            // 3-2：兩層 for 掃過全部格子，跳過已定案的；還走不到的是 double.MaxValue，自然不會被選到
            for (int x = 0; x < _grid.Width; x++)
            {
                for (int y = 0; y < _grid.Height; y++)
                {
                    if (Done[x, y]) continue;
                    if (Distance[x, y] < smallest)
                    {
                        smallest = Distance[x, y];
                        nearest = new Cell(x, y);
                    }
                }
            }

            return !nearest.IsNone;
        }

        /// <summary>
        /// 從 goal 沿 Previous 一路走回 start，反轉後回傳（含起點與終點）。
        /// goal 沒定案（走不到）回傳空清單。
        /// </summary>
        public List<Cell> BuildPath(Cell start, Cell goal)
        {
            var path = new List<Cell>();
            if (!_grid.IsInside(goal) || !Done[goal.X, goal.Y]) return path;

            // 3-4：從 goal 沿 Previous 往回走，起點的 Previous 是 Cell.None，走到它就停
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
    }
}
