//@LOADS@
// Grasshopper Script Instance
// 1117 搜尋與路徑規劃｜Part 4 V5：距離場 ＋ 一顆球照箭頭走
// 不用事先找路：球放在任何起點，每步查所在格子的箭頭往前走，最後都會到終點
// 按 reset 時建格子與距離場、球回到 start；改了格子、障礙、終點、成本的輸入要按 reset 才會重算
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

    // 跨次運算保留：距離場、球、顯示用網格與箭頭
    private FlowField _field;
    private Ball _ball;
    private Mesh _fieldMesh;
    private List<Line> _arrows = new List<Line>();
    private bool _isStartReachable;

    private void RunScript(
		Point3d origin,
		int width,
		int height,
		double cellSize,
		List<Curve> obstacles,
		Point3d start,
		Point3d goal,
		bool allowDiagonal,
		List<Point3d> attractors,
		double attractorRadius,
		double attractorStrength,
		double speed,
		double radius,
		bool reset,
		bool run,
		ref object ball,
		ref object trail,
		ref object arrows,
		ref object field)
    {
        // 重來：建距離場，球放回 start
        if (reset || _field == null)
        {
            var grid = new Grid(origin, width, height, cellSize);
            grid.BlockInside(obstacles);
            CostField.AddAttractors(grid, attractors, attractorRadius, attractorStrength);

            _field = new FlowField(grid, grid.CellAt(goal), allowDiagonal);
            _fieldMesh = FieldDisplay.BuildMesh(grid, _field.Distance);
            _arrows = _field.Arrows(grid.CellSize * 0.8);

            Cell startCell = grid.CellAt(start);
            _isStartReachable = _field.Distance[startCell.X, startCell.Y] != double.MaxValue;
            _ball = new Ball(start);
        }
        // 前進一步：查所在格子的箭頭
        else if (run)
        {
            _ball.MoveInDirection(_field.DirectionAt(_ball.Position), speed);
        }

        // 輸出
        ball = new Sphere(_ball.Position, radius > 0 ? radius : 0.5);
        trail = _ball.Trail.Count >= 2 ? new Polyline(_ball.Trail) : null;
        arrows = _arrows;
        field = _fieldMesh;

        string state = !_isStartReachable ? "起點走不到終點"
            : _ball.IsArrived ? "到終點了"
            : "前進中";
        Print("終點格 {0}，已走 {1:0.0}，{2}", _field.Goal, _ball.Traveled, state);
    }
}
