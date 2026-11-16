using System.Collections.Generic;

namespace AlgoDesign
{
    /// <summary>
    /// 最基礎的 Dijkstra：4 鄰居，每次掃過全部格子找最近的那一格（不用優先佇列）。
    /// 只記兩句話：定案最便宜的，更新它的鄰居。
    /// 可以一次解完（Solve），也可以一步一步走（Begin 之後每呼叫一次 Step 定案一格）。
    /// 今天要親手完成標 TODO 的地方（3-1 到 3-4），不用 AI。
    /// </summary>
    public class Dijkstra
    {
        private readonly Grid _grid;
        private Cell _goal = Cell.None;

        /// <summary>從起點走到每一格的最小成本；還走不到的是 double.MaxValue。</summary>
        public double[,] Distance { get; }

        /// <summary>每一格是從哪一格走過來的；起點與走不到的格子是 Cell.None。</summary>
        public Cell[,] Previous { get; }

        /// <summary>已定案的格子：最小成本已確定，之後不會再變。</summary>
        public bool[,] Done { get; }

        /// <summary>總共定案了幾格（之後拿來和 A* 比較誰找得比較少）。</summary>
        public int DoneCount { get; private set; }

        /// <summary>最近一次 Step 定案的格子；還沒走過是 Cell.None。</summary>
        public Cell Current { get; private set; } = Cell.None;

        /// <summary>結束了沒：終點已定案，或剩下的格子都走不到。</summary>
        public bool IsFinished { get; private set; }

        /// <summary>找到路了沒（終點已定案）。</summary>
        public bool IsFound { get; private set; }

        public Dijkstra(Grid grid)
        {
            _grid = grid;
            Distance = new double[grid.Width, grid.Height];
            Previous = new Cell[grid.Width, grid.Height];
            Done = new bool[grid.Width, grid.Height];
        }

        /// <summary>一次解完：Begin 之後一直 Step 到結束。走得到回傳 true。（不用改）</summary>
        public bool Solve(Cell start, Cell goal)
        {
            Begin(start, goal);
            while (Step())
            {
            }
            return IsFound;
        }

        /// <summary>準備開始：清空三張表，起點的成本設 0。之後每呼叫一次 Step 走一步。</summary>
        public void Begin(Cell start, Cell goal)
        {
            Reset();
            _goal = goal;
            if (!_grid.IsWalkable(start) || !_grid.IsWalkable(goal))
            {
                IsFinished = true;
                return;
            }

            // TODO 3-1：起點的 Distance 設成 0
        }

        /// <summary>
        /// 走一步＝兩句話各做一次：定案最便宜的一格，再更新它的鄰居。
        /// 還能繼續回傳 true；找到終點或沒有格子可以定案了，回傳 false。
        /// </summary>
        public bool Step()
        {
            if (IsFinished) return false;

            // TODO 3-2 第一句：定案最便宜的（把下面的註解改成程式）
            // TryPickNearest(out Cell current) 找不到 → IsFinished = true，return false
            // 定案 current → Done 設 true、DoneCount 加 1、Current = current
            // current 就是 _goal → IsFound = true、IsFinished = true，return false

            // TODO 3-3 第二句：更新它的鄰居
            // 對 _grid.Neighbors4(current) 的每一格 next：
            //     next 已定案就跳過
            //     newDistance = current 的 Distance ＋ next 的 Cost
            //     newDistance 比 next 現在的 Distance 小 →
            //         改寫 next 的 Distance，並記下 next 的 Previous = current

            // TODO 寫完 3-2、3-3 後，把下面兩行換成 return true;
            IsFinished = true;
            return false;
        }

        /// <summary>每次 Begin 前清空：Distance 全設 double.MaxValue、Previous 全設 Cell.None、Done 全設 false。</summary>
        private void Reset()
        {
            DoneCount = 0;
            Current = Cell.None;
            IsFinished = false;
            IsFound = false;

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
