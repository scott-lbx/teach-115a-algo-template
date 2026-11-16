using System.Collections.Generic;
using Rhino.Geometry;

namespace AlgoDesign
{
    /// <summary>
    /// 在 Mesh 上用 Dijkstra 找最短路（給初學者的最簡版，不管效率）。
    /// Mesh 的每個頂點用編號 0、1、2… 表示；有 Mesh 邊相連的兩個頂點是鄰居，成本是兩點的距離。
    /// 只記兩句話：定案最便宜的，更新它的鄰居。每呼叫一次 Step() 做一次。
    /// 參考解答（1117-finish 分支）：註解的 3-1 到 3-4 對應 starter 的 TODO。
    /// </summary>
    public class MeshDijkstra
    {
        private readonly Mesh _mesh;

        // 三張表，索引就是頂點編號
        public double[] Distance;   // 從起點走到這個頂點的最短距離；還走不到是 double.MaxValue
        public int[] Previous;      // 從哪個頂點走過來；起點與還走不到的是 -1
        public bool[] Done;         // 定案了沒（定案＝最短距離已確定，不會再變）

        public int Start;           // 起點的頂點編號
        public int Goal;            // 終點的頂點編號
        public int Current = -1;    // 最近一次定案的頂點
        public int DoneCount;       // 定案了幾個
        public bool IsFinished;     // 結束了沒
        public bool IsFound;        // 找到路了沒

        /// <summary>準備開始：起終點吸到最近的頂點，三張表設初始值。</summary>
        public MeshDijkstra(Mesh mesh, Point3d start, Point3d goal)
        {
            _mesh = mesh;
            int count = mesh.TopologyVertices.Count;
            Distance = new double[count];
            Previous = new int[count];
            Done = new bool[count];
            Start = ClosestVertex(start);
            Goal = ClosestVertex(goal);

            // 3-1：全部還走不到、沒有來處、沒定案；起點的距離是 0
            for (int v = 0; v < count; v++)
            {
                Distance[v] = double.MaxValue;
                Previous[v] = -1;
                Done[v] = false;
            }
            Distance[Start] = 0;
        }

        /// <summary>走一步：兩句話各做一次。</summary>
        public void Step()
        {
            if (IsFinished) return;

            // 3-2 第一句：定案最便宜的
            int current = PickNearest();
            if (current == -1)
            {
                IsFinished = true;   // 剩下的都走不到
                return;
            }
            Done[current] = true;
            DoneCount++;
            Current = current;
            if (current == Goal)
            {
                IsFound = true;
                IsFinished = true;
                return;
            }

            // 3-3 第二句：更新它的鄰居
            foreach (int next in _mesh.TopologyVertices.ConnectedTopologyVertices(current))
            {
                if (Done[next]) continue;
                double newDistance = Distance[current] + PointOf(current).DistanceTo(PointOf(next));
                if (newDistance < Distance[next])
                {
                    Distance[next] = newDistance;
                    Previous[next] = current;
                }
            }
        }

        /// <summary>還沒定案的頂點裡，距離最小的那個；都走不到時回傳 -1。</summary>
        private int PickNearest()
        {
            // 3-2：掃過全部頂點
            int nearest = -1;
            for (int v = 0; v < Distance.Length; v++)
            {
                if (Done[v]) continue;
                if (Distance[v] == double.MaxValue) continue;
                if (nearest == -1 || Distance[v] < Distance[nearest]) nearest = v;
            }
            return nearest;
        }

        /// <summary>找到的路：從終點沿 Previous 走回起點，再反過來；還沒找到回傳 null。</summary>
        public Polyline Path()
        {
            if (!IsFound) return null;

            // 3-4：從終點一路往回走，走到 -1（起點的來處）為止
            var points = new List<Point3d>();
            for (int v = Goal; v != -1; v = Previous[v])
            {
                points.Add(PointOf(v));
            }
            points.Reverse();
            return points.Count >= 2 ? new Polyline(points) : null;
        }

        /// <summary>已經定案的頂點位置（看擴散到哪裡）。</summary>
        public List<Point3d> Visited()
        {
            var points = new List<Point3d>();
            for (int v = 0; v < Done.Length; v++)
            {
                if (Done[v]) points.Add(PointOf(v));
            }
            return points;
        }

        /// <summary>頂點 v 的位置。</summary>
        public Point3d PointOf(int v)
        {
            return new Point3d(_mesh.TopologyVertices[v]);
        }

        /// <summary>給 GH 的 out 看的狀態文字。</summary>
        public string Report()
        {
            string state = IsFound ? "找到路：總長 " + Distance[Goal].ToString("0.###")
                : IsFinished ? "找不到路"
                : "進行中：觸發 step 繼續走";
            return "頂點 " + Distance.Length + " 個；起點 " + Start + " → 終點 " + Goal
                + "\n已定案 " + DoneCount + " 個，剛定案 " + Current
                + "\n" + state
                + "\n直線距離 " + PointOf(Start).DistanceTo(PointOf(Goal)).ToString("0.###") + "（總長一定 ≥ 這個數）";
        }

        // 離 p 最近的頂點編號
        private int ClosestVertex(Point3d p)
        {
            int closest = 0;
            for (int v = 1; v < _mesh.TopologyVertices.Count; v++)
            {
                if (PointOf(v).DistanceTo(p) < PointOf(closest).DistanceTo(p)) closest = v;
            }
            return closest;
        }
    }
}
