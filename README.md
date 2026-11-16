# 115a_algo

演算法設計課專案：製作 Grasshopper C# Script Component。

## 目錄

| 路徑 | 用途 |
| --- | --- |
| `src/` | 所有 `.cs` 程式碼 |
| `gh_script/` | 貼到 GH Script Component 的檔案，用 `#load` 讀取 `src/` |
| `docs/` | 討論用 `.md` |
| `.claude/` | 規劃原則（`CODE.md`、`GIT.md`）與 skill |
| `CLAUDE.md` | Claude 的專案說明與回覆規則 |

## 使用方式

1. 在 `src/` 撰寫演算法類別。
2. 把 `gh_script/` 內的檔案內容貼到 GH C# Script Component。
3. `#load` 路徑行為見 `docs/loading.md`（尚待實測）。

## 課堂分支

| 分支 | 說明 |
| --- | --- |
| `1117-path-starter` | 11/17 Dijkstra × Boids starter，說明見 `docs/1117_路徑與Boids.md` |
| `1117-path-solution` | 同上的參考解答 |
| `1117-finish` | 完整版：V1～V5 與 Boids 距離場導航 |
