//@LOADS@
// Grasshopper Script Instance
// 驗證 #load 的最小範例：輸入 name (string)，輸出 a；name = "GH" 時 a 應是 Hello, GH
using Grasshopper.Kernel;
using AlgoDesign;

public class Script_Instance : GH_ScriptInstance
{
    private void RunScript(string name, ref object a)
    {
        a = Greeter.Greet(name);
    }
}
