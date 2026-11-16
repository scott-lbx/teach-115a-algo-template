#load "..\src\Ball.cs"
// Grasshopper Script Instance
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
using AlgoDesign;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
#endregion

public class Script_Instance : GH_ScriptInstance
{
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
