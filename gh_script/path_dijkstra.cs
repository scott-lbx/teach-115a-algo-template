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
// 1117 搜尋與路徑規劃｜Part 3：格子 ＋ Dijkstra 找路
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

    private void RunScript(
		Point3d origin,
		int width,
		int height,
		double cellSize,
		List<Curve> obstacles,
		Point3d start,
		Point3d goal,
		ref object path,
		ref object field)
    {
        // 建格子、擋住障礙曲線圍住的格子
        var grid = new Grid(origin, width, height, cellSize);
        int blockedCount = grid.BlockInside(obstacles);

        // 找路
        Cell startCell = grid.CellAt(start);
        Cell goalCell = grid.CellAt(goal);
        var solver = new Dijkstra(grid);
        bool isFound = solver.Solve(startCell, goalCell);
        List<Cell> cells = solver.BuildPath(startCell, goalCell);

        // 輸出：路徑折線、距離場上色網格
        var points = new List<Point3d>();
        foreach (Cell cell in cells)
        {
            points.Add(grid.CellCenter(cell));
        }
        path = points.Count >= 2 ? new Polyline(points) : null;
        field = FieldDisplay.BuildMesh(grid, solver.Distance);

        // 驗證基準：無障礙、成本全 1 時，總成本要等於曼哈頓距離
        int manhattan = Math.Abs(goalCell.X - startCell.X) + Math.Abs(goalCell.Y - startCell.Y);
        Print("格子 {0}×{1}，擋住 {2} 格", grid.Width, grid.Height, blockedCount);
        Print("起點 {0} → 終點 {1}：{2}", startCell, goalCell, isFound ? "找到路" : "找不到路");
        if (isFound)
        {
            Print("總成本 {0}，路徑 {1} 格，定案 {2} 格", solver.TotalCost(goalCell), cells.Count, solver.DoneCount);
        }
        Print("曼哈頓距離 {0}（無障礙、成本全 1 時要和總成本相等）", manhattan);
    }
}
