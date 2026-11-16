//@LOADS@
// Grasshopper Script Instance
// Part 3：Mesh 轉成圖，Dijkstra 一步一步找路（每觸發一次 step 走 steps 步）
// 本體在 src\PathDijkstraHost.cs；這裡只接輸入輸出
using Rhino.Geometry;
using Grasshopper.Kernel;
using AlgoDesign;

public class Script_Instance : GH_ScriptInstance
{
    private readonly PathDijkstraHost _host = new PathDijkstraHost();

    private void RunScript(Mesh mesh, Point3d start, Point3d goal, bool reset, bool step, int steps,
        ref object path, ref object field, ref object current, ref object frontier)
    {
        _host.Run(mesh, start, goal, reset, step, steps);
        path = _host.Path;
        field = _host.Field;
        current = _host.Current;
        frontier = _host.Frontier;
        Print(_host.Report);
    }
}
