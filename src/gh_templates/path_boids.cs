//@LOADS@
// Grasshopper Script Instance
// Part 5（課程最後）：Boids 沿路徑飛（規則 5 跟隨路徑；run 接 Toggle ＋ Timer）
// 本體在 src\PathBoidsHost.cs；這裡只接輸入輸出
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper.Kernel;
using AlgoDesign;

public class Script_Instance : GH_ScriptInstance
{
    private readonly PathBoidsHost _host = new PathBoidsHost();

    private void RunScript(
        Curve path, List<Curve> obstacles, Point3d min, Point3d max,
        int boidCount, int seed, double spawnSize, double maxSpeed, double maxForce,
        double viewRadius, double alignmentWeight, double cohesionWeight,
        double separationRadius, double separationWeight, double avoidRadius, double avoidWeight,
        double followWeight, double lookAhead, double pathRadius, bool reset, bool run,
        ref object points, ref object headings)
    {
        _host.Run(path, obstacles, min, max, boidCount, seed, spawnSize, maxSpeed, maxForce,
            viewRadius, alignmentWeight, cohesionWeight, separationRadius, separationWeight,
            avoidRadius, avoidWeight, followWeight, lookAhead, pathRadius, reset, run);
        points = _host.Points;
        headings = _host.Headings;
        Print(_host.Report);
    }
}
