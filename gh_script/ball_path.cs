//@LOADS@
// Grasshopper Script Instance
// Part 3：一顆球沿路徑走（path 接 path_dijkstra 的 path；run 接 Toggle ＋ Timer）
// 本體在 src\BallPathHost.cs；這裡只接輸入輸出
using Rhino.Geometry;
using Grasshopper.Kernel;
using AlgoDesign;

public class Script_Instance : GH_ScriptInstance
{
    private readonly BallPathHost _host = new BallPathHost();

    private void RunScript(Curve path, double speed, double radius, bool reset, bool run,
        ref object ball, ref object trail)
    {
        _host.Run(path, speed, radius, reset, run);
        ball = _host.BallShape;
        trail = _host.Trail;
        Print(_host.Report);
    }
}
