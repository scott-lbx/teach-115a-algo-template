# #load 路徑實測

目的：確認 GH Script Component 的 `#load` 如何解析 `src/` 底下的 `.cs`。

**狀態：已確認，只能用絕對路徑。**

## 實測步驟（自己做一次最小測試）

1. 新增 `src/Greeter.cs`：

   ```csharp
   namespace AlgoDesign
   {
       public static class Greeter
       {
           public static string Greet(string name) => "Hello, " + name;
       }
   }
   ```

2. 新增樣板 `src/gh_templates/main.cs`：

   ```csharp
   //@LOADS@
   // Grasshopper Script Instance
   using Grasshopper.Kernel;
   using AlgoDesign;

   public class Script_Instance : GH_ScriptInstance
   {
       private void RunScript(string name, ref object a)
       {
           a = Greeter.Greet(name);
       }
   }
   ```

3. 在 repo 根目錄執行 `python tools\gen_gh_scripts.py`，產生 `gh_script\main.cs`。
4. 在 Rhino 8 開 Grasshopper，放一個 C# Script Component，新增輸入 `name`（string）、輸出 `a`。
5. 貼上 `gh_script\main.cs` 的全部內容，輸入 `name = "GH"`，按綠色執行鈕。
6. 預期輸出 `Hello, GH`。出現 `The type or namespace name 'AlgoDesign' could not be found` 代表 `#load` 沒載入：檢查 `gh_script\main.cs` 開頭的 `#load` 路徑是不是自己電腦上的位置。

## 結果

| 情況 | 結果 | 出處 |
| --- | --- | --- |
| 相對路徑 `..\src\...`（.gh 已存檔在 `gh_script/`） | 不行：什麼都沒載入，報 `The type or namespace name 'AlgoDesign' could not be found` | 11/16 在本 repo 的 GH 實測 |
| 絕對路徑 | 可以 | 11/03 boids_game 實測 |
| 萬用字元、巢狀載入（被載入的檔再 `#load`） | 不行 | 11/03 boids_game 實測 |
| 修改 `src/` 後，元件是否自動重新載入 | 不會，要打開元件編輯器按綠色執行鈕 | 11/03 boids_game 實測 |

## 結論

樣板放在 `src/gh_templates/`，第一行是 `//@LOADS@`。`tools/gen_gh_scripts.py` 把它換成 `src/` 每個 `.cs` 的絕對路徑 `#load`，輸出到 `gh_script/`（不進版控），把 `gh_script/` 的檔貼進 GH。
