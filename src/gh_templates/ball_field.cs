//@LOADS@
// Grasshopper Script Instance
// Part 4 V5：距離場 ＋ 一顆球從任何起點照箭頭走（改了輸入要按 reset；run 接 Toggle ＋ Timer）
// 本體在 src\BallFieldHost.cs；這裡只接輸入輸出
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper.Kernel;
using AlgoDesign;

public class Script_Instance : GH_ScriptInstance
{
    private readonly BallFieldHost _host = new BallFieldHost();

    private void RunScript(
        Point3d origin, int width, int height, double cellSize, List<Curve> obstacles,
        Point3d start, Point3d goal, bool allowDiagonal,
        List<Point3d> attractors, double attractorRadius, double attractorStrength,
        double speed, double radius, bool reset, bool run,
        ref object ball, ref object trail, ref object arrows, ref object field)
    {
        _host.Run(origin, width, height, cellSize, obstacles, start, goal, allowDiagonal,
            attractors, attractorRadius, attractorStrength, speed, radius, reset, run);
        ball = _host.BallShape;
        trail = _host.Trail;
        arrows = _host.Arrows;
        field = _host.Field;
        Print(_host.Report);
    }
}
