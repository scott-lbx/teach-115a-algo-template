//@LOADS@
// Grasshopper Script Instance
// Part 3：在 Mesh 上用 Dijkstra 找路。按 reset 從頭開始，每觸發一次 step 走一步（step 接 Toggle ＋ Timer 會自己一直走）
using Rhino.Geometry;
using Grasshopper.Kernel;
using AlgoDesign;

public class Script_Instance : GH_ScriptInstance
{
    private MeshDijkstra _dijkstra;   // 跨次運算保留：記住走到哪一步

    private void RunScript(Mesh mesh, Point3d start, Point3d goal, bool reset, bool step,
        ref object path, ref object visited, ref object current)
    {
        if (mesh == null || mesh.TopologyVertices.Count == 0)
        {
            Print("沒接 Mesh");
            return;
        }

        if (reset || _dijkstra == null) _dijkstra = new MeshDijkstra(mesh, start, goal);
        else if (step) _dijkstra.Step();

        path = _dijkstra.Path();
        visited = _dijkstra.Visited();
        current = _dijkstra.Current >= 0 ? _dijkstra.PointOf(_dijkstra.Current) : start;
        Print(_dijkstra.Report());
    }
}
