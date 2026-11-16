# 1117 搜尋與路徑規劃：從一顆球到一群 Boids

本分支是 11/17 課堂的起點：**資料夾架構，加上 Dijkstra 找路元件的兩個檔**（`src/MeshDijkstra.cs` 與它的樣板）。`MeshDijkstra.cs` 留 TODO 3-1～3-4，親手寫，不用 AI；球與 Boids 在課堂上再加。

## 分支

| 分支 | 內容 |
| --- | --- |
| `1117-path-starter` | 今天的起點：`src/MeshDijkstra.cs`（留 TODO）、`path_dijkstra` 樣板、說明文件與產生器 |
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
| `src/*.cs` | 演算法類別（`namespace AlgoDesign`，一檔一類別） | 要 |
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

樣板只負責接線：建一個 `MeshDijkstra`、觸發一次走一步、把結果接到輸出。演算法都在 `src/MeshDijkstra.cs`。

```csharp
//@LOADS@
// Grasshopper Script Instance
using Rhino.Geometry;
using Grasshopper.Kernel;
using AlgoDesign;

public class Script_Instance : GH_ScriptInstance
{
    private MeshDijkstra _dijkstra;   // 跨次運算保留：記住走到哪一步

    private void RunScript(Mesh mesh, Point3d start, Point3d goal, bool reset, bool step,
        ref object path, ref object visited, ref object current)
    {
        if (mesh == null || mesh.TopologyVertices.Count == 0)
        {
            Print("沒接 Mesh");
            return;
        }

        if (reset || _dijkstra == null) _dijkstra = new MeshDijkstra(mesh, start, goal);
        else if (step) _dijkstra.Step();

        path = _dijkstra.Path();
        visited = _dijkstra.Visited();
        current = _dijkstra.Current >= 0 ? _dijkstra.PointOf(_dijkstra.Current) : start;
        Print(_dijkstra.Report());
    }
}
```

`_dijkstra` 是欄位，GH 每次重算都會留著，所以可以記住「走到哪一步」。

## 元件 `path_dijkstra` 的輸入輸出

| 輸入 | 型別 | 接什麼 |
| --- | --- | --- |
| mesh | Mesh | GH 的 Mesh Plane（例如 10×10 格）；挖掉的面就是障礙 |
| start | Point3d | 起點（會吸到最近的頂點） |
| goal | Point3d | 終點（會吸到最近的頂點） |
| reset | bool | Button：從頭開始。**換了 Mesh 或起終點要按一次** |
| step | bool | Button：按一下走一步；或 Boolean Toggle ＋ Timer：自己一直走 |

| 輸出 | 內容 |
| --- | --- |
| path | 找到才有的路徑（Polyline） |
| visited | 已經定案的頂點，看擴散到哪裡 |
| current | 剛定案的頂點 |

`out` 會印出頂點數、起終點編號、已定案幾個、狀態（進行中／找到路／找不到路）與直線距離。

## 檔案（只有兩個）

| 檔案 | 內容 | 今天要改嗎 |
| --- | --- | --- |
| `src/MeshDijkstra.cs` | 全部的演算法：三張表 `Distance`、`Previous`、`Done`；建構子初始化、`Step()` 走一步、`Path()` 回溯 | **要，TODO 3-1 到 3-4** |
| `src/gh_templates/path_dijkstra.cs` | GH 元件樣板（上面那段） | 不用 |

Mesh 的每個頂點用編號 0、1、2… 表示；鄰居直接問 Mesh：`_mesh.TopologyVertices.ConnectedTopologyVertices(v)`；成本是兩點的距離。不另外建「圖」的類別，也不管效率（每一步都掃過全部頂點）。

## 之後在課堂上加的

| 順序 | 加什麼 | 課堂段落 |
| --- | --- | --- |
| 1 | 一顆球沿 `path` 走 | Part 3 尾段 |
| 2 | 沿用 11/03 的 Boids，加一條「跟隨路徑」規則 | Part 5（課程最後） |

## Dijkstra 只記兩句話

1. **定案最便宜的**：從還沒定案的頂點裡，拿出 `Distance` 最小的，把它定案。
2. **更新它的鄰居**：對它的每個鄰居，如果「經過它再走過去」比較近，就改寫鄰居的 `Distance`，並記下 `Previous`（從哪來）。

`Step()` 一次＝兩句話各做一次＝定案一個頂點。在 GH 每按一次 step 就走一步，`visited` 的點會從起點一圈一圈往外長；接 Toggle ＋ Timer 就會自己一直走到找到終點。

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
