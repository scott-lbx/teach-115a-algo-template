using System.Collections.Generic;

namespace AlgoDesign
{
    /// <summary>
    /// 最基礎的 Dijkstra，在一張圖（Graph）上找最短路：每次掃過全部節點找最近的那一個（不用優先佇列）。
    /// 只記兩句話：定案最便宜的，更新它的鄰居。
    /// 可以一次解完（Solve），也可以一步一步走（Begin 之後每呼叫一次 Step 定案一個節點）。
    /// 今天要親手完成標 TODO 的地方（3-1 到 3-4），不用 AI。
    /// 節點用編號 0、1、2… 表示；三張表都是一維陣列，索引就是節點編號。
    /// </summary>
    public class Dijkstra
    {
        private readonly Graph _graph;
        private int _goal = -1;

        /// <summary>從起點走到每個節點的最小成本；還走不到的是 double.MaxValue。</summary>
        public double[] Distance { get; }

        /// <summary>每個節點是從哪個節點走過來的；起點與走不到的節點是 -1。</summary>
        public int[] Previous { get; }

        /// <summary>已定案的節點：最小成本已確定，之後不會再變。</summary>
        public bool[] Done { get; }

        /// <summary>總共定案了幾個節點（之後拿來和 A* 比較誰找得比較少）。</summary>
        public int DoneCount { get; private set; }

        /// <summary>最近一次 Step 定案的節點；還沒走過是 -1。</summary>
        public int Current { get; private set; } = -1;

        /// <summary>結束了沒：終點已定案，或剩下的節點都走不到。</summary>
        public bool IsFinished { get; private set; }

        /// <summary>找到路了沒（終點已定案）。</summary>
        public bool IsFound { get; private set; }

        public Dijkstra(Graph graph)
        {
            _graph = graph;
            Distance = new double[graph.NodeCount];
            Previous = new int[graph.NodeCount];
            Done = new bool[graph.NodeCount];
        }

        /// <summary>一次解完：Begin 之後一直 Step 到結束。走得到回傳 true。（不用改）</summary>
        public bool Solve(int start, int goal)
        {
            Begin(start, goal);
            while (Step())
            {
            }
            return IsFound;
        }

        /// <summary>準備開始：清空三張表，起點的成本設 0。之後每呼叫一次 Step 走一步。</summary>
        public void Begin(int start, int goal)
        {
            Reset();
            _goal = goal;
            if (!_graph.Contains(start) || !_graph.Contains(goal))
            {
                IsFinished = true;
                return;
            }

            // TODO 3-1：起點的 Distance 設成 0
        }

        /// <summary>
        /// 走一步＝兩句話各做一次：定案最便宜的一個節點，再更新它的鄰居。
        /// 還能繼續回傳 true；找到終點或沒有節點可以定案了，回傳 false。
        /// </summary>
        public bool Step()
        {
            if (IsFinished) return false;

            // TODO 3-2 第一句：定案最便宜的（把下面的註解改成程式）
            // TryPickNearest(out int current) 找不到 → IsFinished = true，return false
            // 定案 current → Done[current] 設 true、DoneCount 加 1、Current = current
            // current 就是 _goal → IsFound = true、IsFinished = true，return false

            // TODO 3-3 第二句：更新它的鄰居
            // foreach (Edge edge in _graph.Neighbors(current))：
            //     next = edge.To；next 已定案就跳過
            //     newDistance = Distance[current] ＋ edge.Weight
            //     newDistance 比 Distance[next] 小 →
            //         改寫 Distance[next]，並記下 Previous[next] = current

            // TODO 寫完 3-2、3-3 後，把下面兩行換成 return true;
            IsFinished = true;
            return false;
        }

        /// <summary>每次 Begin 前清空：Distance 全設 double.MaxValue、Previous 全設 -1、Done 全設 false。</summary>
        private void Reset()
        {
            DoneCount = 0;
            Current = -1;
            IsFinished = false;
            IsFound = false;

            // TODO 3-1：一個 for 掃過全部節點（node 從 0 到 _graph.NodeCount - 1），設定三張表的初始值
        }

        /// <summary>
        /// 第一句「定案最便宜的」：從還沒定案、而且已經走得到的節點裡，找 Distance 最小的那個。
        /// 找到回傳 true；全部定案或剩下的都走不到，回傳 false。
        /// </summary>
        private bool TryPickNearest(out int nearest)
        {
            nearest = -1;
            double smallest = double.MaxValue;

            // TODO 3-2：一個 for 掃過全部節點
            //           跳過 Done 的節點
            //           Distance 比 smallest 小 → 更新 smallest 與 nearest

            return nearest >= 0;
        }

        /// <summary>
        /// 從 goal 沿 Previous 一路走回起點，反轉後回傳節點編號（含起點與終點）。
        /// goal 沒定案（走不到）回傳空清單。
        /// </summary>
        public List<int> BuildPath(int goal)
        {
            var path = new List<int>();
            if (!_graph.Contains(goal) || !Done[goal]) return path;

            // TODO 3-4：從 goal 開始，加進 path，再換成它的 Previous，一直到 -1 為止
            //           最後 path.Reverse()，讓第一個是起點

            return path;
        }

        /// <summary>走到 goal 的總成本；走不到回傳 double.MaxValue。</summary>
        public double TotalCost(int goal)
        {
            if (!_graph.Contains(goal) || !Done[goal]) return double.MaxValue;
            return Distance[goal];
        }
    }
}
