# gh_script

只放貼進 GH C# Script 元件的檔。

1. 在 repo 根目錄執行 `python tools\gen_gh_scripts.py`。
2. 它會由 `src/gh_templates/` 產生這裡的 `.cs`，`#load` 是本機絕對路徑，所以不進版控。
3. 把這裡的 `.cs` 整份貼進 GH 元件。`.gh` 檔也可以存在這個資料夾。
