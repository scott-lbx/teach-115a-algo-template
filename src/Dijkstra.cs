using System.Collections.Generic;

namespace AlgoDesign
{
    /// <summary>
    /// 最基礎的 Dijkstra：4 鄰居，每次掃過全部格子找最近的那一格（不用優先佇列）。
    /// 只記兩句話：定案最便宜的，更新它的鄰居。
    /// 今天要親手完成標 TODO 的地方（3-1 到 3-4），不用 AI。
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

            // TODO 3-1：起點的 Distance 設成 0

            // TODO 3-2、3-3：主迴圈（把下面的註解改成程式）
            // while (TryPickNearest(out Cell current))
            // {
            //     第一句：定案 current → Done 設 true、DoneCount 加 1
            //     current 就是 goal → return true
            //
            //     第二句：更新鄰居 → 對 _grid.Neighbors4(current) 的每一格 next：
            //         next 已定案就跳過
            //         newDistance = current 的 Distance ＋ next 的 Cost
            //         newDistance 比 next 現在的 Distance 小 →
            //             改寫 next 的 Distance，並記下 next 的 Previous = current
            // }

            return false;
        }

        /// <summary>每次 Solve 前清空：Distance 全設 double.MaxValue、Previous 全設 Cell.None、Done 全設 false。</summary>
        private void Reset()
        {
            DoneCount = 0;

            // TODO 3-1：兩層 for 掃過全部格子（和 W4 反應擴散一樣），設定三個陣列的初始值
        }

        /// <summary>
        /// 第一句「定案最便宜的」：從還沒定案、而且已經走得到的格子裡，找 Distance 最小的那格。
        /// 找到回傳 true；全部定案或剩下的都走不到，回傳 false。
        /// </summary>
        private bool TryPickNearest(out Cell nearest)
        {
            nearest = Cell.None;
            double smallest = double.MaxValue;

            // TODO 3-2：兩層 for 掃過全部格子
            //           跳過 Done 的格子
            //           Distance 比 smallest 小 → 更新 smallest 與 nearest

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

            // TODO 3-4：從 goal 開始，加進 path，再換成它的 Previous，一直到 Cell.None 為止
            //           最後 path.Reverse()，讓第一格是 start

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
