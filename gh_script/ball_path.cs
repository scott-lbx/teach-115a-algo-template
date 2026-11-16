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
// 1117 搜尋與路徑規劃｜Part 3：一顆球沿路徑走
// path 接 path_dijkstra（或 Part 4 的 path_search）輸出的路徑；按 reset 回到起點，run 接 Timer 讓球往前走
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
    private Ball _ball;

    private void RunScript(
		Curve path,
		double speed,
		double radius,
		bool reset,
		bool run,
		ref object ball,
		ref object trail)
    {
        if (path == null)
        {
            Print("沒接路徑（找不到路時 path 是空的）");
            return;
        }

        // 重來：球放回路徑起點
        if (reset || _ball == null)
        {
            _ball = new Ball(path.PointAtStart);
        }
        // 前進一步
        else if (run)
        {
            _ball.MoveAlong(path, speed);
        }

        // 輸出
        ball = new Sphere(_ball.Position, radius > 0 ? radius : 0.5);
        trail = _ball.Trail.Count >= 2 ? new Polyline(_ball.Trail) : null;
        Print("已走 {0:0.0} ／ 全長 {1:0.0}，{2}",
            _ball.Traveled, path.GetLength(), _ball.IsArrived ? "到終點了" : "前進中");
    }
}
