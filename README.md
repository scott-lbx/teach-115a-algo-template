# 115a_algo

演算法設計課專案：製作 Grasshopper C# Script Component。

## 目錄

| 路徑 | 用途 |
| --- | --- |
| `src/` | 所有 `.cs` 程式碼 |
| `src/` 底下的 `gh_templates/` | GH Script Component 的樣板（第一行 `//@LOADS@`） |
| `gh_script/` | 只放貼進 GH Script Component 的檔：由樣板產生，`#load` 是本機絕對路徑（不進版控） |
| `tools/` | `gen_gh_scripts.py`：由 `src/gh_templates/` 產生 `gh_script/*.cs`，`#load` 換成本機絕對路徑 |
| `docs/` | 討論用 `.md` |
| `.claude/` | 規劃原則（`CODE.md`、`GIT.md`）與 skill |
| `CLAUDE.md` | Claude 的專案說明與回覆規則 |

## 使用方式

1. 在 `src/` 撰寫演算法類別，在 `src/gh_templates/` 寫元件樣板。
2. 執行 `python tools\gen_gh_scripts.py`。
3. 把 `gh_script\` 內的檔案內容整份貼到 GH C# Script Component。
4. GH 的 `#load` 只吃絕對路徑，詳見 `docs/loading.md`。

## 課堂分支

| 分支 | 說明 |
| --- | --- |
| `1117-path-starter` | 11/17 課堂起點：只有資料夾架構，程式在課堂上一起寫；說明見 `docs/1117_路徑與Boids.md` |
| `1117-finish` | 同上的完整版與解答（下課後上傳） |
