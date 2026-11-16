# 1117 搜尋與路徑規劃：Dijkstra × Boids

本分支是 11/17 課堂的 starter。今天親手完成 `src/Dijkstra.cs` 裡標 `TODO` 的地方（3-1 到 3-4），不用 AI。

## 分支

| 分支 | 內容 |
| --- | --- |
| `1117-path-starter` | 今天的起點：`Dijkstra.cs` 只有骨架與 TODO，其他都能用 |
| `1117-finish` | 完整版：Dijkstra 解答、V1～V5、課程最後的 Boids（下課後才上傳） |

## 取得今天的 starter

```bash
git switch main
git pull
git switch 1117-path-starter
python tools\gen_gh_scripts.py
```

卡住時跟著老師的畫面做；解答在 `1117-finish`，下課後上傳。

## 檔案

| 檔案 | 內容 | 今天要改嗎 |
| --- | --- | --- |
| `src/Edge.cs` | 圖上的一條邊：通到哪個節點、花多少（struct） | 不用 |
| `src/Graph.cs` | 圖：節點（編號＋位置）、每個節點的鄰接清單、找最近的節點 | 不用，先讀懂 |
| `src/MeshGraph.cs` | GH Mesh → 圖：頂點是節點、Mesh 的邊是邊、權重是邊長 | 不用 |
| `src/Dijkstra.cs` | Dijkstra 本體（在圖上找路） | **要，TODO 3-1 到 3-4** |
| `src/ValueColor.cs`、`src/GraphDisplay.cs` | 依距離幫 Mesh 上色 | 不用 |
| `src/Ball.cs` | 一顆球沿路徑走，把找到的路「走出來」 | 不用 |
| `src/Boid.cs`、`src/Flock.cs` | 沿用 11/03 的 Boids，多了規則 5「跟隨路徑」 | 不用（課程最後老師展示） |
| `src/ObstacleBuilder.cs` | 障礙曲線轉成實心 Brep，給 Boids 閃避 | 不用 |
| `src/*Host.cs` | 每個 GH 元件的本體（狀態、輸出、`out` 文字），GH script 只把輸入輸出接過來 | 不用 |
| `gh_script/path_dijkstra.cs` | Part 3：Mesh → 圖 → Dijkstra 找路 | 不用 |
| `gh_script/ball_path.cs` | Part 3：一顆球沿路徑走 | 不用 |
| `gh_script/path_boids.cs` | Part 5（課程最後）：Boids 沿路徑飛 | 不用 |
| `tools/gen_gh_scripts.py` | 產生貼進 GH 用的檔到 `gh_script/ready/` | 不用，執行它 |

## 在 GH 組起來

GH 的 `#load` **只吃絕對路徑**（相對路徑會什麼都沒載入，出現 `The type or namespace name 'AlgoDesign' could not be found`）。所以 `gh_script/*.cs` 是樣板，第一行是 `//@LOADS@`，要先產生自己電腦的版本：

1. 在 repo 根目錄執行 `python tools\gen_gh_scripts.py`，產生 `gh_script\ready\*.cs`：`//@LOADS@` 換成 `src\` 每個 `.cs` 的絕對路徑 `#load`。`ready\` 不進版控。
2. 依課程進度放 C# Script 元件，把 **`gh_script\ready\`** 裡對應的檔整份貼進去（不是貼 `gh_script\` 的樣板），依下表加輸入輸出。先做 `path_dijkstra` 與 `ball_path`，Boids 留到最後。
3. 在 VS Code 改 `src/` 的檔案、存檔後，回 GH 打開元件編輯器按綠色執行鈕。GH 不會自動重新編譯。
4. repo 搬位置、換電腦、`src/` 增減檔案之後，重跑步驟 1，再重新貼一次。

樣板越精簡越好：`//@LOADS@`、`// Grasshopper Script Instance`、幾行 `using`，以及 `Script_Instance` 裡一個 `RunScript`：把輸入交給 `src/` 的 `…Host`，再把它的結果接到輸出。邏輯都在 `src/`，改程式只改 `src/`，GH 裡貼的東西不用動。

### Part 3｜`path_dijkstra.cs`：Mesh → 圖 → 找路

| 索引 | 名稱 | 型別 | 存取 | 建議值 |
| --- | --- | --- | --- | --- |
| 0 | mesh | Mesh | Item | GH 的 Mesh Plane（例如 20×20 格）、地形 Mesh；挖掉的面就是障礙 |
| 1 | start | Point3d | Item | 起點（會吸到最近的頂點） |
| 2 | goal | Point3d | Item | 終點（會吸到最近的頂點） |
| 3 | reset | bool | Item | Button（從頭開始） |
| 4 | step | bool | Item | Button：按一下走一次；或 Boolean Toggle ＋ Timer：自己一直走 |
| 5 | steps | int | Item | 1（每觸發一次定案幾個節點；Mesh 大時調大） |

輸出：`path`（找到才有的 Polyline）、`field`（依距離上色的 Mesh）、`current`（剛定案的節點）、`frontier`（邊界：已經有距離、還沒定案的節點）。`out` 會印出節點數、邊數、已定案與邊界的個數、剛定案的節點與距離、狀態（進行中／找到路／找不到路）、起終點的直線距離。Mesh 或起終點改了會自動從頭開始。

### Part 3｜`ball_path.cs`：一顆球沿路徑走

| 索引 | 名稱 | 型別 | 存取 | 建議值 |
| --- | --- | --- | --- | --- |
| 0 | path | Curve | Item | 接 `path_dijkstra` 的 `path` |
| 1 | speed | double | Item | 0.3（每步沿路走多長） |
| 2 | radius | double | Item | 0.5 |
| 3 | reset | bool | Item | Button（球回到起點） |
| 4 | run | bool | Item | Boolean Toggle ＋ Timer |

輸出：`ball`（Sphere）、`trail`（走過的軌跡 Polyline）。改了障礙或起終點，路徑會變，按 `reset` 讓球從新起點出發。

### Part 5（課程最後）｜`path_boids.cs`：Boids 沿路徑飛

| 索引 | 名稱 | 型別 | 存取 | 建議值 |
| --- | --- | --- | --- | --- |
| 0 | path | Curve | Item | 接 `path_dijkstra` 的 `path` |
| 1 | obstacles | Curve | List | 和 `path_dijkstra` 同一組曲線 |
| 2 | min | Point3d | Item | (0,0,0) |
| 3 | max | Point3d | Item | (40,40,0)（Z 和 min 相同 → 只在平面上飛） |
| 4 | boidCount | int | Item | 40 |
| 5 | seed | int | Item | 0 |
| 6 | spawnSize | double | Item | 4（在路徑起點附近這麼大的範圍出生） |
| 7 | maxSpeed | double | Item | 0.3 |
| 8 | maxForce | double | Item | 0.05 |
| 9 | viewRadius | double | Item | 4 |
| 10 | alignmentWeight | double | Item | 1 |
| 11 | cohesionWeight | double | Item | 1 |
| 12 | separationRadius | double | Item | 1.5 |
| 13 | separationWeight | double | Item | 1.5 |
| 14 | avoidRadius | double | Item | 2 |
| 15 | avoidWeight | double | Item | 3 |
| 16 | followWeight | double | Item | 1.5 |
| 17 | lookAhead | double | Item | 3 |
| 18 | pathRadius | double | Item | 1.5 |
| 19 | reset | bool | Item | Button |
| 20 | run | bool | Item | Boolean Toggle ＋ Timer |

輸出：`points`（Point3d 清單）、`headings`（Line 清單）。

## 怎麼確認你寫對了

| 情況 | 應該看到 |
| --- | --- |
| Mesh Plane（四邊形面、沒有挖洞），間距 s | 總長 ＝ （橫向格數＋縱向格數）× s，也就是曼哈頓距離 × s |
| 同一個 Mesh 改成三角面（Triangulate） | 總長變短：可以走對角線 |
| 起點與終點對調 | 總長不變 |
| 任何情況 | 總長 ≥ `out` 印的直線距離 |
| 終點被挖洞的面圍成孤島 | 印出「找不到路」，沒有路徑 |
| `field` Mesh | 起點附近藍、越遠越紅、沒擴散到的白 |
| 球 | 從起點沿路走到終點停下，`out` 印出「到終點了」 |

## TODO 與課堂步驟對照

| TODO | 位置 | 對應的一句話 |
| --- | --- | --- |
| 3-1 | `Reset()`、`Begin()` | 初始化：全部「還走不到」，起點成本 0 |
| 3-2 | `TryPickNearest()`、`Step()` 前半 | 第一句：定案最便宜的 |
| 3-3 | `Step()` 後半，最後改成 `return true;` | 第二句：更新它的鄰居 |
| 3-4 | `BuildPath()` | 從終點沿 `Previous` 走回起點 |

`Step()` 一次＝兩句話各做一次＝定案一個節點。`Solve()` 只是 `Begin()` 之後一直 `Step()` 到結束，所以在 GH 按一次 step 看到的，就是迴圈跑一圈。寫完一段就按 step 看效果：3-1 寫完，`field` 只剩起點有顏色；3-2、3-3 寫完，每按一次多定案一個節點。

節點用編號 0、1、2… 表示，三張表都是一維陣列：`Distance[node]`、`Previous[node]`、`Done[node]`；鄰居用 `_graph.Neighbors(node)` 拿，每條邊有 `To`（通到哪）和 `Weight`（花多少）。

## 之後的變化（Part 4，用 AI，先開 branch）

V1 8 鄰居 → V2 優先佇列加速（100×100 線性掃描約 1 秒，可以計時比較）→ V3 A\* → V4 成本場 → V5 距離場。每一版都讓球重新跑一次，並用上表的基準驗證。Boids 放在課程最後。
