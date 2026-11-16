using System.Collections.Generic;
using Rhino.Geometry;

namespace AlgoDesign
{
    /// <summary>
    /// 一張圖：節點＋邊＋權重。節點用編號 0、1、2… 表示，每個節點有一個位置。
    /// 格子、Mesh、街道網都可以轉成這張圖，Dijkstra 只認這張圖，不管它原本是什麼。
    /// </summary>
    public class Graph
    {
        // 每個節點的「鄰接清單」：從這個節點出發的所有邊
        private readonly List<List<Edge>> _adjacent = new List<List<Edge>>();

        /// <summary>每個節點的位置（索引就是節點編號）。</summary>
        public List<Point3d> Positions { get; } = new List<Point3d>();

        /// <summary>節點數。</summary>
        public int NodeCount => Positions.Count;

        /// <summary>邊數（單向邊各算一條）。</summary>
        public int EdgeCount { get; private set; }

        /// <summary>加一個節點，回傳它的編號。</summary>
        public int AddNode(Point3d position)
        {
            Positions.Add(position);
            _adjacent.Add(new List<Edge>());
            return Positions.Count - 1;
        }

        /// <summary>加一條單向邊：from → to，花 weight。</summary>
        public void AddEdge(int from, int to, double weight)
        {
            _adjacent[from].Add(new Edge(to, weight));
            EdgeCount++;
        }

        /// <summary>加一條雙向邊：a ↔ b，兩個方向都花 weight。</summary>
        public void AddUndirectedEdge(int a, int b, double weight)
        {
            AddEdge(a, b, weight);
            AddEdge(b, a, weight);
        }

        /// <summary>從 node 出發的所有邊。</summary>
        public List<Edge> Neighbors(int node)
        {
            return _adjacent[node];
        }

        /// <summary>這個編號是不是圖上的節點。</summary>
        public bool Contains(int node)
        {
            return node >= 0 && node < NodeCount;
        }

        /// <summary>離 point 最近的節點編號；圖是空的回傳 -1。</summary>
        public int ClosestNode(Point3d point)
        {
            int closest = -1;
            double smallest = double.MaxValue;
            for (int node = 0; node < NodeCount; node++)
            {
                double distance = Positions[node].DistanceTo(point);
                if (distance < smallest)
                {
                    smallest = distance;
                    closest = node;
                }
            }
            return closest;
        }
    }
}
