# 1117 搜尋與路徑規劃：從一顆球到一群 Boids

本分支是 11/17 課堂的起點：**只有資料夾架構，程式在課堂上一起寫**。Dijkstra 的部分親手寫，不用 AI。

## 分支

| 分支 | 內容 |
| --- | --- |
| `1117-path-starter` | 今天的起點：空的 `src/`、`src/gh_templates/`、`gh_script/`，加上說明文件與產生器 |
| `1117-finish` | 完整版：Dijkstra 解答、V1～V5、球照距離場走、課程最後的 Boids（下課後才上傳） |

## 取得今天的 starter

```bash
git switch main
git pull
git switch 1117-path-starter
```

卡住時跟著老師的畫面做；解答在 `1117-finish`，下課後上傳。

## 資料夾架構

| 位置 | 放什麼 | 進版控嗎 |
| --- | --- | --- |
| `src/*.cs` | 演算法類別，以及每個 GH 元件的本體 `…Host`（`namespace AlgoDesign`，一檔一類別） | 要 |
| `src/gh_templates/*.cs` | GH script 樣板，第一行 `//@LOADS@`，越精簡越好 | 要 |
| `gh_script/*.cs` | 產生器產生、整份貼進 GH 元件的檔（`#load` 是本機絕對路徑） | 不要 |
| `tools/gen_gh_scripts.py` | 由樣板產生 `gh_script/*.cs` | 要 |
| `docs/` | 說明文件 | 要 |

## 開發流程

1. 在 `src/` 寫類別（VS Code）。
2. 在 `src/gh_templates/` 寫這個元件的樣板。
3. 在 repo 根目錄執行 `python tools\gen_gh_scripts.py`，產生 `gh_script\*.cs`。
4. 把 `gh_script\` 裡對應的檔整份貼進 GH 的 C# Script 元件，依 `RunScript` 的參數加輸入輸出，按編輯器的綠色執行鈕。
5. 之後只改 `src/` 的內容時，回 GH 按綠色執行鈕即可（GH 不會自動重新編譯）。`src/` **增減檔案**、repo 搬位置、換電腦時，重跑步驟 3 並重新貼上。

GH 的 `#load` 只吃絕對路徑，相對路徑會什麼都沒載入（詳見 `docs/loading.md`），所以才需要產生器。

## 樣板的寫法

樣板只負責接線：把輸入交給 `src/` 的 `…Host`，再把結果接到輸出。邏輯都放 `src/`，GH 裡貼的東西就不用常常改。

```csharp
//@LOADS@
// Grasshopper Script Instance
using Rhino.Geometry;
using Grasshopper.Kernel;
using AlgoDesign;

public class Script_Instance : GH_ScriptInstance
{
    private readonly PathDijkstraHost _host = new PathDijkstraHost();

    private void RunScript(Mesh mesh, Point3d start, Point3d goal, bool reset, bool step, int steps,
        ref object path, ref object field, ref object current, ref object frontier)
    {
        _host.Run(mesh, start, goal, reset, step, steps);
        path = _host.Path;
        field = _host.Field;
        current = _host.Current;
        frontier = _host.Frontier;
        Print(_host.Report);
    }
}
```

`_host` 是欄位，GH 每次重算都會留著，所以可以記住「走到哪一步」。

## 今天要寫的東西（依序）

| 順序 | 檔案 | 內容 | 課堂段落 |
| --- | --- | --- | --- |
| 1 | `src/Edge.cs`、`src/Graph.cs` | 圖：節點用編號 0、1、2…，每個節點有位置與一串邊；邊有 `To`（通到哪）與 `Weight`（花多少） | Part 3 |
| 2 | `src/MeshGraph.cs` | GH Mesh → 圖：拓樸頂點是節點、Mesh 的邊是邊、權重是邊長 | Part 3 |
| 3 | `src/Dijkstra.cs` | 三張表 `Distance`、`Previous`、`Done`；`Begin`、`Step`、`Solve`、`BuildPath` | Part 3，親手寫 |
| 4 | `src/PathDijkstraHost.cs`、`src/gh_templates/path_dijkstra.cs` | 元件本體與樣板：起終點吸到最近的頂點、每觸發一次 `step` 走一步、依距離幫 Mesh 上色 | Part 3 |
| 5 | `src/Ball.cs`、`src/BallPathHost.cs`、`src/gh_templates/ball_path.cs` | 一顆球沿找到的路走 | Part 3 尾段 |
| 6 | `src/Boid.cs`、`src/Flock.cs` 與它的 Host、樣板 | 沿用 11/03 的 Boids，加一條「跟隨路徑」規則 | Part 5（課程最後） |

## Dijkstra 只記兩句話

1. **定案最便宜的**：從還沒定案的節點裡，拿出 `Distance` 最小的，把它定案。
2. **更新它的鄰居**：沿著它的每條邊，如果「經過它再走過去」比較近，就改寫鄰居的 `Distance`，並記下 `Previous`（從哪來）。

`Step()` 一次＝兩句話各做一次＝定案一個節點。`Solve()` 只是 `Begin()` 之後一直 `Step()` 到結束。在 GH 每按一次 step，就是迴圈跑一圈，可以看到顏色從起點一步一步往外長。

## 怎麼確認寫對了

| 情況 | 應該看到 |
| --- | --- |
| Mesh Plane（四邊形面、沒有挖洞），間距 s | 總長 ＝ （橫向格數＋縱向格數）× s，也就是曼哈頓距離 × s |
| 同一個 Mesh 改成三角面（Triangulate） | 總長變短：可以走對角線 |
| 起點與終點對調 | 總長不變 |
| 任何情況 | 總長 ≥ 起終點的直線距離 |
| 終點被挖洞的面圍成孤島 | 找不到路，沒有路徑 |
| 每一步定案的距離 | 不會比上一步小 |
| 球 | 從起點沿路走到終點停下 |

## 之後的變化（Part 4，用 AI，先開 branch）

V1 8 鄰居 → V2 優先佇列加速 → V3 A\* → V4 成本場 → V5 距離場。每一版都讓球重新跑一次，並用上表的基準驗證。完整程式在 `1117-finish`。Boids 放在課程最後。
