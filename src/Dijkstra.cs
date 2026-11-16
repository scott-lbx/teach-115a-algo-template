using System.Collections.Generic;

namespace AlgoDesign
{
    /// <summary>
    /// 最基礎的 Dijkstra，在一張圖（Graph）上找最短路：每次掃過全部節點找最近的那一個（不用優先佇列）。
    /// 只記兩句話：定案最便宜的，更新它的鄰居。
    /// 可以一次解完（Solve），也可以一步一步走（Begin 之後每呼叫一次 Step 定案一個節點）。
    /// 參考解答（1117-finish 分支）：註解的 3-1 到 3-4 對應 starter 的 TODO。
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

        /// <summary>一次解完：Begin 之後一直 Step 到結束。走得到回傳 true。</summary>
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

            // 3-1：起點的成本是 0
            Distance[start] = 0;
        }

        /// <summary>
        /// 走一步＝兩句話各做一次：定案最便宜的一個節點，再更新它的鄰居。
        /// 還能繼續回傳 true；找到終點或沒有節點可以定案了，回傳 false。
        /// </summary>
        public bool Step()
        {
            if (IsFinished) return false;

            // 3-2 第一句：定案最便宜的
            if (!TryPickNearest(out int current))
            {
                IsFinished = true;
                return false;
            }
            Done[current] = true;
            DoneCount++;
            Current = current;
            if (current == _goal)
            {
                IsFound = true;
                IsFinished = true;
                return false;
            }

            // 3-3 第二句：更新它的鄰居
            foreach (Edge edge in _graph.Neighbors(current))
            {
                int next = edge.To;
                if (Done[next]) continue;

                double newDistance = Distance[current] + edge.Weight;
                if (newDistance < Distance[next])
                {
                    Distance[next] = newDistance;
                    Previous[next] = current;
                }
            }
            return true;
        }

        /// <summary>每次 Begin 前清空：Distance 全設 double.MaxValue、Previous 全設 -1、Done 全設 false。</summary>
        private void Reset()
        {
            DoneCount = 0;
            Current = -1;
            IsFinished = false;
            IsFound = false;

            // 3-1：一個 for 掃過全部節點，設定三張表的初始值
            for (int node = 0; node < _graph.NodeCount; node++)
            {
                Distance[node] = double.MaxValue;
                Previous[node] = -1;
                Done[node] = false;
            }
        }

        /// <summary>
        /// 第一句「定案最便宜的」：從還沒定案、而且已經走得到的節點裡，找 Distance 最小的那個。
        /// 找到回傳 true；全部定案或剩下的都走不到，回傳 false。
        /// </summary>
        private bool TryPickNearest(out int nearest)
        {
            nearest = -1;
            double smallest = double.MaxValue;

            // 3-2：掃過全部節點，跳過已定案的；還走不到的是 double.MaxValue，自然不會被選到
            for (int node = 0; node < _graph.NodeCount; node++)
            {
                if (Done[node]) continue;
                if (Distance[node] < smallest)
                {
                    smallest = Distance[node];
                    nearest = node;
                }
            }

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

            // 3-4：從 goal 沿 Previous 往回走，起點的 Previous 是 -1，走到它就停
            int current = goal;
            while (current >= 0)
            {
                path.Add(current);
                current = Previous[current];
            }
            path.Reverse();

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
