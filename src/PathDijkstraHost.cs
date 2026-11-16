using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;

namespace AlgoDesign
{
    /// <summary>
    /// GH 元件 path_dijkstra 的本體：Mesh 轉成圖，Dijkstra 一步一步找路。
    /// 跨次運算保留走到哪一步；Mesh 或起終點改了就自動從頭開始。GH script 只把輸入輸出接過來。
    /// </summary>
    public class PathDijkstraHost
    {
        private Graph _graph;
        private Dijkstra _solver;
        private Mesh _mesh;
        private int _start = -1;
        private int _goal = -1;
        private string _sceneKey = "";

        /// <summary>找到才有的路徑；還沒找到是 null。</summary>
        public Polyline Path { get; private set; }

        /// <summary>依距離上色的 Mesh：藍近、紅遠、白色還沒走到。</summary>
        public Mesh Field { get; private set; }

        /// <summary>剛定案的節點位置；還沒走過是 null。</summary>
        public Point3d? Current { get; private set; }

        /// <summary>邊界：已經有距離、還沒定案的節點位置。</summary>
        public List<Point3d> Frontier { get; private set; } = new List<Point3d>();

        /// <summary>給 GH 的 out 看的狀態文字。</summary>
        public string Report { get; private set; } = "";

        /// <summary>每次 GH 運算呼叫一次：需要就重來，step 為 true 時走 steps 步，最後更新輸出。</summary>
        public void Run(Mesh mesh, Point3d start, Point3d goal, bool reset, bool step, int steps)
        {
            if (mesh == null || !mesh.IsValid)
            {
                Clear("沒接 Mesh（或 Mesh 不正確）");
                return;
            }

            // 重來：按 reset、第一次執行、或 Mesh／起終點改了
            string sceneKey = SceneKey(mesh, start, goal);
            if (reset || _solver == null || sceneKey != _sceneKey)
            {
                Restart(mesh, start, goal, sceneKey);
            }
            // 前進：這次觸發走 steps 步（沒接或 ≤ 0 時走 1 步）
            else if (step)
            {
                int count = steps > 0 ? steps : 1;
                for (int i = 0; i < count; i++)
                {
                    if (!_solver.Step()) break;
                }
            }

            UpdateOutputs();
        }

        /// <summary>Mesh → 圖，起終點取最近的頂點，Dijkstra 從頭準備。</summary>
        private void Restart(Mesh mesh, Point3d start, Point3d goal, string sceneKey)
        {
            _mesh = mesh;
            _graph = MeshGraph.FromMesh(mesh);
            _start = _graph.ClosestNode(start);
            _goal = _graph.ClosestNode(goal);
            _solver = new Dijkstra(_graph);
            _solver.Begin(_start, _goal);
            _sceneKey = sceneKey;
        }

        /// <summary>路徑、上色 Mesh、剛定案的點、邊界，以及狀態文字。</summary>
        private void UpdateOutputs()
        {
            List<int> nodes = _solver.BuildPath(_goal);
            Path = nodes.Count >= 2 ? new Polyline(nodes.Select(node => _graph.Positions[node])) : null;
            Field = GraphDisplay.ColorMesh(_mesh, _solver.Distance);
            Current = _solver.Current >= 0 ? _graph.Positions[_solver.Current] : (Point3d?)null;
            Frontier = FrontierPoints();
            Report = BuildReport(nodes.Count);
        }

        // 已經有距離、還沒定案的節點：下一步會從這些節點裡挑最便宜的
        private List<Point3d> FrontierPoints()
        {
            var points = new List<Point3d>();
            for (int node = 0; node < _graph.NodeCount; node++)
            {
                if (_solver.Done[node] || _solver.Distance[node] == double.MaxValue) continue;
                points.Add(_graph.Positions[node]);
            }
            return points;
        }

        // 狀態與驗證基準：路徑長不可能比起終點的直線距離短
        private string BuildReport(int pathNodeCount)
        {
            var text = new StringBuilder();
            text.AppendLine($"圖：{_graph.NodeCount} 個節點、{_graph.EdgeCount / 2} 條邊；起點 {_start} → 終點 {_goal}");
            text.AppendLine($"已定案 {_solver.DoneCount} 個，邊界 {Frontier.Count} 個");
            if (_solver.Current >= 0)
            {
                text.AppendLine($"剛定案 {_solver.Current}，距離 {_solver.Distance[_solver.Current]:0.###}");
            }

            if (_solver.IsFound)
            {
                text.AppendLine($"找到路：總長 {_solver.TotalCost(_goal):0.###}，經過 {pathNodeCount} 個節點");
            }
            else if (_solver.IsFinished)
            {
                text.AppendLine("找不到路");
            }
            else
            {
                text.AppendLine("進行中：觸發 step 繼續走");
            }

            if (_graph.Contains(_start) && _graph.Contains(_goal))
            {
                double straight = _graph.Positions[_start].DistanceTo(_graph.Positions[_goal]);
                text.Append($"直線距離 {straight:0.###}（路徑總長一定 ≥ 這個數）");
            }
            return text.ToString();
        }

        // 沒有 Mesh 時清空輸出
        private void Clear(string message)
        {
            _solver = null;
            Path = null;
            Field = null;
            Current = null;
            Frontier = new List<Point3d>();
            Report = message;
        }

        // 場景的指紋：Mesh 的頂點數、面數、外框，加上起終點；任何一個改了，指紋就不同
        private static string SceneKey(Mesh mesh, Point3d start, Point3d goal)
        {
            BoundingBox box = mesh.GetBoundingBox(true);
            return string.Join("|", mesh.Vertices.Count, mesh.Faces.Count, box.Min, box.Max, start, goal);
        }
    }
}
