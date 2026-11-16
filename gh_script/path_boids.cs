// Grasshopper Script Instance
#load "..\src\Cell.cs"
#load "..\src\Grid.cs"
#load "..\src\Dijkstra.cs"
#load "..\src\CellHeap.cs"
#load "..\src\PathSearch.cs"
#load "..\src\CostField.cs"
#load "..\src\FieldDisplay.cs"
#load "..\src\FlowField.cs"
#load "..\src\Ball.cs"
#load "..\src\Boid.cs"
#load "..\src\Flock.cs"
#load "..\src\ObstacleBuilder.cs"
#load "..\src\Greeter.cs"
// 1117 搜尋與路徑規劃｜Part 5（課程最後）：Boids 沿路徑飛（Boid、Flock 沿用 11/03，多了規則 5 跟隨路徑）
// 每次運算只走一步，鳥存在 _flock 裡跨次運算保留；連續跑要外接 Timer
// 輸入輸出的名稱、型別、順序見 docs/1117_路徑與Boids.md
#region Usings
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using AlgoDesign;
#endregion

public class Script_Instance : GH_ScriptInstance
{
    #region Notes
    /*
      Members:
        RhinoDoc RhinoDocument
        GH_Document GrasshopperDocument
        IGH_Component Component
        int Iteration

      Methods (Virtual & overridable):
        Print(string text)
        Print(string format, params object[] args)
        Reflect(object obj)
        Reflect(object obj, string method_name)
    */
    #endregion

    // 跨次運算保留
    private Flock _flock = new Flock();

    private void RunScript(
		Curve path,
		List<Curve> obstacles,
		Point3d min,
		Point3d max,
		int boidCount,
		int seed,
		double spawnSize,
		double maxSpeed,
		double maxForce,
		double viewRadius,
		double alignmentWeight,
		double cohesionWeight,
		double separationRadius,
		double separationWeight,
		double avoidRadius,
		double avoidWeight,
		double followWeight,
		double lookAhead,
		double pathRadius,
		bool reset,
		bool run,
		ref object points,
		ref object headings)
    {
        // 和 path_dijkstra 同一組障礙曲線，轉成實心 Brep 給 Boids 閃避
        List<Brep> obstacleBreps = ObstacleBuilder.FromCurves(obstacles);

        // 重來：在路徑起點附近 spawnSize 見方的範圍內重建所有鳥
        if (reset || _flock.Boids.Count == 0)
        {
            Point3d center = path != null ? path.PointAtStart : (min + max) / 2;
            var spawnMin = new Point3d(center.X - spawnSize / 2, center.Y - spawnSize / 2, min.Z);
            var spawnMax = new Point3d(center.X + spawnSize / 2, center.Y + spawnSize / 2, max.Z);
            _flock.Reset(boidCount, spawnMin, spawnMax, maxSpeed, maxForce, seed, obstacleBreps);
        }
        // 前進一步
        else if (run)
        {
            _flock.Step(min, max, maxSpeed, maxForce, viewRadius, alignmentWeight,
                cohesionWeight, separationRadius, separationWeight,
                obstacleBreps, avoidRadius, avoidWeight,
                path, followWeight, lookAhead, pathRadius);
        }

        // 輸出
        var pointList = new List<Point3d>();
        var headingList = new List<Line>();
        foreach (Boid boid in _flock.Boids)
        {
            pointList.Add(boid.Position);
            headingList.Add(new Line(boid.Position, boid.Position + boid.Velocity * 3));
        }
        Print("鳥 {0} 隻，障礙 {1} 個，第 {2} 步，{3}",
            _flock.Boids.Count, obstacleBreps.Count, _flock.StepCount, path != null ? "有路徑" : "沒接路徑");
        points = pointList;
        headings = headingList;
    }
}
