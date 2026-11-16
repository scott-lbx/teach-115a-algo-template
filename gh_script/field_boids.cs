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
// 1117 搜尋與路徑規劃｜Part 5（課程最後）方法 B：Boids 距離場導航（V5 ＋ Boids 規則 6）
// 按 reset 時建格子與距離場並重建鳥（散在整個格子範圍）；之後每次運算只走一步，連續跑要外接 Timer
// 改了格子、障礙、終點、成本等輸入，要按 reset 才會重算距離場
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

    // 跨次運算保留：鳥、距離場、障礙、顯示用網格與箭頭、鳥飛行的範圍
    private Flock _flock = new Flock();
    private FlowField _field;
    private List<Brep> _obstacleBreps = new List<Brep>();
    private Mesh _fieldMesh;
    private List<Line> _arrows = new List<Line>();
    private Point3d _boxMin;
    private Point3d _boxMax;

    private void RunScript(
		Point3d origin,
		int width,
		int height,
		double cellSize,
		List<Curve> obstacles,
		Point3d goal,
		bool allowDiagonal,
		List<Point3d> attractors,
		double attractorRadius,
		double attractorStrength,
		int boidCount,
		int seed,
		double maxSpeed,
		double maxForce,
		double viewRadius,
		double alignmentWeight,
		double cohesionWeight,
		double separationRadius,
		double separationWeight,
		double avoidRadius,
		double avoidWeight,
		double fieldWeight,
		bool reset,
		bool run,
		ref object points,
		ref object headings,
		ref object arrows,
		ref object field)
    {
        // 重來：建距離場，鳥散在整個格子範圍
        if (reset || _field == null)
        {
            var grid = new Grid(origin, width, height, cellSize);
            grid.BlockInside(obstacles);
            CostField.AddAttractors(grid, attractors, attractorRadius, attractorStrength);

            _field = new FlowField(grid, grid.CellAt(goal), allowDiagonal);
            _obstacleBreps = ObstacleBuilder.FromCurves(obstacles);
            _fieldMesh = FieldDisplay.BuildMesh(grid, _field.Distance);
            _arrows = _field.Arrows(grid.CellSize * 0.8);

            _boxMin = grid.Origin;
            _boxMax = new Point3d(
                grid.Origin.X + grid.Width * grid.CellSize,
                grid.Origin.Y + grid.Height * grid.CellSize,
                grid.Origin.Z);
            _flock.Reset(boidCount, _boxMin, _boxMax, maxSpeed, maxForce, seed, _obstacleBreps);
        }
        // 前進一步
        else if (run)
        {
            _flock.Step(_boxMin, _boxMax, maxSpeed, maxForce, viewRadius, alignmentWeight,
                cohesionWeight, separationRadius, separationWeight,
                _obstacleBreps, avoidRadius, avoidWeight,
                field: _field, fieldWeight: fieldWeight);
        }

        // 輸出
        var pointList = new List<Point3d>();
        var headingList = new List<Line>();
        foreach (Boid boid in _flock.Boids)
        {
            pointList.Add(boid.Position);
            headingList.Add(new Line(boid.Position, boid.Position + boid.Velocity * 3));
        }
        Print("鳥 {0} 隻，障礙 {1} 個，第 {2} 步，終點格 {3}",
            _flock.Boids.Count, _obstacleBreps.Count, _flock.StepCount, _field.Goal);
        points = pointList;
        headings = headingList;
        arrows = _arrows;
        field = _fieldMesh;
    }
}
