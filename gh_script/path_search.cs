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
// 1117 搜尋與路徑規劃｜Part 4 V1～V4：8 鄰居、優先佇列、A*、成本場（輸出的 path 可接 ball_path 讓球跑）
// 同一個場景同時跑「基礎版 Dijkstra」與「進階版 PathSearch」，在 out 印出對照與驗證結果
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

using System.Diagnostics;
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
		bool allowDiagonal,
		bool useAStar,
		List<Point3d> attractors,
		double attractorRadius,
		double attractorStrength,
		double noiseScale,
		double noiseStrength,
		int seed,
		ref object path,
		ref object field,
		ref object costMap,
		ref object basicPath)
    {
        // 場景：格子、障礙、V4 成本場
        var grid = new Grid(origin, width, height, cellSize);
        int blockedCount = grid.BlockInside(obstacles);
        int attractedCount = CostField.AddAttractors(grid, attractors, attractorRadius, attractorStrength);
        CostField.AddNoise(grid, noiseScale, noiseStrength, seed);
        Cell startCell = grid.CellAt(start);
        Cell goalCell = grid.CellAt(goal);

        // 基礎版：4 鄰居、線性掃描（課堂 Part 3 手寫的那份）
        var timer = Stopwatch.StartNew();
        var basic = new Dijkstra(grid);
        bool isBasicFound = basic.Solve(startCell, goalCell);
        double basicMs = timer.Elapsed.TotalMilliseconds;

        // 進階版：依輸入切換 8 鄰居（V1）與 A*（V3），一律用優先佇列（V2）
        timer.Restart();
        var advanced = new PathSearch(grid) { AllowDiagonal = allowDiagonal, UseAStar = useAStar };
        bool isFound = advanced.Solve(startCell, goalCell);
        double advancedMs = timer.Elapsed.TotalMilliseconds;

        // 驗證用：同樣鄰居設定、不用 A* 的 Dijkstra
        var reference = new PathSearch(grid) { AllowDiagonal = allowDiagonal, UseAStar = false };
        reference.Solve(startCell, goalCell);

        // 輸出
        path = ToPolyline(grid, advanced.BuildPath(startCell, goalCell));
        basicPath = ToPolyline(grid, basic.BuildPath(startCell, goalCell));
        field = FieldDisplay.BuildMesh(grid, advanced.Distance);
        costMap = FieldDisplay.BuildMesh(grid, grid.Cost);

        Print("格子 {0}×{1}，擋住 {2} 格，吸引點影響 {3} 格", grid.Width, grid.Height, blockedCount, attractedCount);
        Print("基礎版（4 鄰居、線性掃描）：{0}，成本 {1:0.###}，定案 {2} 格，{3:0.0} ms",
            isBasicFound ? "找到" : "找不到", basic.TotalCost(goalCell), basic.DoneCount, basicMs);
        Print("進階版（{0}、{1}、優先佇列）：{2}，成本 {3:0.###}，定案 {4} 格，{5:0.0} ms",
            allowDiagonal ? "8 鄰居" : "4 鄰居", useAStar ? "A*" : "Dijkstra",
            isFound ? "找到" : "找不到", advanced.TotalCost(goalCell), advanced.DoneCount, advancedMs);

        // 驗證基準
        if (!allowDiagonal)
        {
            Print("V2 驗證：4 鄰居時，優先佇列版成本要和基礎版相同 → {0}",
                IsSameCost(advanced.TotalCost(goalCell), basic.TotalCost(goalCell)) ? "✔" : "✘");
        }
        if (useAStar)
        {
            Print("V3 驗證：A* 成本要和 Dijkstra 相同 → {0}；定案格數 A* {1} vs Dijkstra {2}",
                IsSameCost(advanced.TotalCost(goalCell), reference.TotalCost(goalCell)) ? "✔" : "✘",
                advanced.DoneCount, reference.DoneCount);
        }
    }

    // 格子序列 → 格心折線；不到兩點回傳 null
    private Polyline ToPolyline(Grid grid, List<Cell> cells)
    {
        if (cells.Count < 2) return null;
        return new Polyline(cells.Select(cell => grid.CellCenter(cell)));
    }

    // 兩個成本是否相同（斜向的 √2 加總會有極小的浮點誤差）
    private bool IsSameCost(double a, double b)
    {
        if (a == double.MaxValue || b == double.MaxValue) return a == b;
        return Math.Abs(a - b) < 1e-6;
    }
}
