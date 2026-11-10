// 貼到 GH C# Script Component 的範例（Rhino 8）。
// 輸入：name (string)；輸出：a (object)
// #load 路徑請先依 docs/loading.md 實測後調整。

#load "..\src\Greeter.cs"

using AlgoDesign;

private void RunScript(string name, ref object a)
{
    a = Greeter.Greet(name);
}
