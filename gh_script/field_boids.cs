//@LOADS@
// Grasshopper Script Instance
// Part 5（課程最後）方法 B：Boids 照距離場導航（改了輸入要按 reset；run 接 Toggle ＋ Timer）
// 本體在 src\FieldBoidsHost.cs；這裡只接輸入輸出
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper.Kernel;
using AlgoDesign;

public class Script_Instance : GH_ScriptInstance
{
    private readonly FieldBoidsHost _host = new FieldBoidsHost();

    private void RunScript(
        Point3d origin, int width, int height, double cellSize, List<Curve> obstacles,
        Point3d goal, bool allowDiagonal,
        List<Point3d> attractors, double attractorRadius, double attractorStrength,
        int boidCount, int seed, double maxSpeed, double maxForce, double viewRadius,
        double alignmentWeight, double cohesionWeight, double separationRadius, double separationWeight,
        double avoidRadius, double avoidWeight, double fieldWeight, bool reset, bool run,
        ref object points, ref object headings, ref object arrows, ref object field)
    {
        _host.Run(origin, width, height, cellSize, obstacles, goal, allowDiagonal,
            attractors, attractorRadius, attractorStrength, boidCount, seed, maxSpeed, maxForce,
            viewRadius, alignmentWeight, cohesionWeight, separationRadius, separationWeight,
            avoidRadius, avoidWeight, fieldWeight, reset, run);
        points = _host.Points;
        headings = _host.Headings;
        arrows = _host.Arrows;
        field = _host.Field;
        Print(_host.Report);
    }
}
