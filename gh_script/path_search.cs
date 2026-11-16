//@LOADS@
// Grasshopper Script Instance
// Part 4 V1～V4：格子上對照基礎版 Dijkstra 與 PathSearch（8 鄰居、優先佇列、A*、成本場）
// 本體在 src\PathSearchHost.cs；這裡只接輸入輸出。path 可接 ball_path 讓球跑
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper.Kernel;
using AlgoDesign;

public class Script_Instance : GH_ScriptInstance
{
    private readonly PathSearchHost _host = new PathSearchHost();

    private void RunScript(
        Point3d origin, int width, int height, double cellSize, List<Curve> obstacles,
        Point3d start, Point3d goal, bool allowDiagonal, bool useAStar,
        List<Point3d> attractors, double attractorRadius, double attractorStrength,
        double noiseScale, double noiseStrength, int seed,
        ref object path, ref object field, ref object costMap, ref object basicPath)
    {
        _host.Run(origin, width, height, cellSize, obstacles, start, goal, allowDiagonal, useAStar,
            attractors, attractorRadius, attractorStrength, noiseScale, noiseStrength, seed);
        path = _host.Path;
        field = _host.Field;
        costMap = _host.CostMap;
        basicPath = _host.BasicPath;
        Print(_host.Report);
    }
}
