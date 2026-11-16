# gh_templates

GH C# Script 元件的樣板，一個元件一個 `.cs`。

- 第一行寫 `//@LOADS@`，`tools/gen_gh_scripts.py` 會把它換成 `src/` 每個 `.cs` 的絕對路徑 `#load`，產生到 `gh_script/`。
- 樣板越精簡越好：`// Grasshopper Script Instance`、幾行 `using`、`Script_Instance` 裡一個 `RunScript`，把輸入交給 `src/` 的 `…Host`，再把結果接到輸出。
