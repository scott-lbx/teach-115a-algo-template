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
// 貼到 GH C# Script Component 的範例（Rhino 8）。
// 輸入：name (string)；輸出：a (object)
// #load 路徑請先依 docs/loading.md 實測後調整。
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

    private void RunScript(string name, ref object a)
    {
        a = Greeter.Greet(name);
    }
}
