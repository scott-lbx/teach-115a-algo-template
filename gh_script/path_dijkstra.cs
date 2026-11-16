//@LOADS@
// Grasshopper Script Instance
// 1117 搜尋與路徑規劃｜Part 3：格子 ＋ Dijkstra 找路（一步一步看）
// step 每觸發一次走 steps 步（定案 steps 格）：接 Button 按一下走一次；接 Toggle ＋ Timer 會自己一直走
// 格子、障礙、起終點改了會自動重來；reset 也可以手動重來
// 只做接線：建格子、擋障礙、呼叫 Dijkstra、輸出；演算法本體在 src/Dijkstra.cs
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

    // 跨次運算保留：格子、Dijkstra 走到哪一步、目前場景的指紋（輸入一變就重來）
    private Grid _grid;
    private Dijkstra _solver;
    private Cell _startCell;
    private Cell _goalCell;
    private int _blockedCount;
    private string _sceneKey = "";

    private void RunScript(
		Point3d origin,
		int width,
		int height,
		double cellSize,
		List<Curve> obstacles,
		Point3d start,
		Point3d goal,
		bool reset,
		bool step,
		int steps,
		ref object path,
		ref object field,
		ref object current,
		ref object frontier)
    {
        // 重來：按 reset、第一次執行、或格子／障礙／起終點改了
        string sceneKey = SceneKey(origin, width, height, cellSize, obstacles, start, goal);
        if (reset || _solver == null || sceneKey != _sceneKey)
        {
            _grid = new Grid(origin, width, height, cellSize);
            _blockedCount = _grid.BlockInside(obstacles);
            _startCell = _grid.CellAt(start);
            _goalCell = _grid.CellAt(goal);
            _solver = new Dijkstra(_grid);
            _solver.Begin(_startCell, _goalCell);
            _sceneKey = sceneKey;
        }
        // 前進：這次觸發走 steps 步（沒接或 ≤ 0 時走 1 步）
        else if (step)
        {
            int count = steps > 0 ? steps : 1;
            for (int i = 0; i < count; i++)
            {
                if (!_solver.Step()) break;
            }
        }

        // 輸出：找到才有路徑；距離場；剛定案的格子；邊界（已經有距離、還沒定案的格子）
        List<Cell> cells = _solver.BuildPath(_startCell, _goalCell);
        path = cells.Count >= 2 ? new Polyline(cells.Select(cell => _grid.CellCenter(cell))) : null;
        field = FieldDisplay.BuildMesh(_grid, _solver.Distance);
        current = _solver.Current.IsNone ? null : (object)_grid.CellCenter(_solver.Current);
        List<Point3d> frontierPoints = FrontierPoints();
        frontier = frontierPoints;

        // 狀態與驗證基準：無障礙、成本全 1 時，總成本要等於曼哈頓距離
        int manhattan = Math.Abs(_goalCell.X - _startCell.X) + Math.Abs(_goalCell.Y - _startCell.Y);
        Print("格子 {0}×{1}，擋住 {2} 格；起點 {3} → 終點 {4}",
            _grid.Width, _grid.Height, _blockedCount, _startCell, _goalCell);
        Print("已定案 {0} 格，邊界 {1} 格", _solver.DoneCount, frontierPoints.Count);
        if (!_solver.Current.IsNone)
        {
            Print("剛定案 {0}，距離 {1}", _solver.Current,
                _solver.Distance[_solver.Current.X, _solver.Current.Y]);
        }
        if (_solver.IsFound)
        {
            Print("找到路：總成本 {0}，路徑 {1} 格", _solver.TotalCost(_goalCell), cells.Count);
        }
        else if (_solver.IsFinished)
        {
            Print("找不到路");
        }
        else
        {
            Print("進行中：觸發 step 繼續走");
        }
        Print("曼哈頓距離 {0}（無障礙、成本全 1 時要和總成本相等）", manhattan);
    }

    // 已經有距離、還沒定案的格子：下一步會從這些格子裡挑最便宜的
    private List<Point3d> FrontierPoints()
    {
        var points = new List<Point3d>();
        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                if (_solver.Done[x, y] || _solver.Distance[x, y] == double.MaxValue) continue;
                points.Add(_grid.CellCenter(new Cell(x, y)));
            }
        }
        return points;
    }

    // 場景的指紋：格子、起終點、每條障礙曲線的外框；任何一個改了，指紋就不同
    private string SceneKey(Point3d origin, int width, int height, double cellSize,
        List<Curve> obstacles, Point3d start, Point3d goal)
    {
        var parts = new List<string> { origin.ToString(), width.ToString(), height.ToString(),
            cellSize.ToString("R"), start.ToString(), goal.ToString() };
        if (obstacles != null)
        {
            foreach (Curve curve in obstacles)
            {
                if (curve == null) continue;
                BoundingBox box = curve.GetBoundingBox(true);
                parts.Add(box.Min.ToString() + box.Max.ToString() + curve.GetLength().ToString("R"));
            }
        }
        return string.Join("|", parts);
    }
}
