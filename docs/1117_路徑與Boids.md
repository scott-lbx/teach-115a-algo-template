# 1117 搜尋與路徑規劃：Dijkstra × Boids

11/17 課堂用。Part 3 親手完成 `src/Dijkstra.cs` 裡標 `TODO` 的地方（3-1 到 3-4），不用 AI；Part 4、5 的完整版在 `1117-finish`。

## 分支

| 分支 | 內容 |
| --- | --- |
| `1117-path-starter` | 今天的起點：`Dijkstra.cs` 只有骨架與 TODO，其他都能用 |
| `1117-path-solution` | 參考解答：`Dijkstra.cs` 已完成（卡住時用） |
| `1117-finish` | 完整版：V1～V5（8 鄰居、優先佇列、A\*、成本場、距離場）、球照距離場走，以及課程最後的 Boids |

## 取得今天的 starter

```bash
git switch main
git pull
git switch 1117-path-starter
```

卡住想直接拿解答的 `Dijkstra.cs`（**會蓋掉你自己寫的這個檔**，先 commit 再做）：

```bash
git fetch
git checkout origin/1117-path-solution -- src/Dijkstra.cs
```

## 檔案

| 檔案 | 內容 | 今天要改嗎 |
| --- | --- | --- |
| `src/Cell.cs` | 一格的整數座標 (X, Y)，struct | 不用 |
| `src/Grid.cs` | 格子：`Cost`、`Blocked`、`Neighbors4()`、座標換算、用封閉曲線擋格子 | 不用，先讀懂 |
| `src/Dijkstra.cs` | Dijkstra 本體 | **要，TODO 3-1 到 3-4** |
| `src/FieldDisplay.cs` | 把距離場畫成上色網格 | 不用 |
| `src/Ball.cs` | 一顆球沿路徑走，把找到的路「走出來」 | 不用 |
| `src/Boid.cs`、`src/Flock.cs` | 沿用 11/03 的 Boids，多了規則 5「跟隨路徑」 | 不用（課程最後老師展示） |
| `src/ObstacleBuilder.cs` | 障礙曲線轉成實心 Brep，給 Boids 閃避 | 不用 |
| `gh_script/path_dijkstra.cs` | Part 3：格子 ＋ Dijkstra 找路 | 不用 |
| `gh_script/ball_path.cs` | Part 3：一顆球沿路徑走 | 不用 |
| `gh_script/path_boids.cs` | Part 5（課程最後）：Boids 沿路徑飛 | 不用 |

`1117-finish` 另外多了：

| 檔案 | 內容 |
| --- | --- |
| `src/CellHeap.cs` | V2 優先佇列（最小堆積）；自己寫，不依賴 .NET 版本 |
| `src/PathSearch.cs` | V1 8 鄰居、V2 優先佇列、V3 A\*；`SolveAll` 給距離場用 |
| `src/CostField.cs` | V4 成本場：吸引點、平滑雜訊 |
| `src/FlowField.cs` | V5 距離場：從終點反向擴散，每格一個往終點的方向 |
| `src/Grid.cs` | 多了 `Neighbors8`（不切牆角）、`StepCost`（斜向 ×√2）、`MinWalkableCost` |
| `src/Ball.cs` | 多了 `MoveInDirection`：照距離場的箭頭走 |
| `src/Boid.cs`、`src/Flock.cs` | 多了規則 6 `FollowField` |
| `gh_script/path_search.cs` | Part 4 V1～V4：同場景對照基礎版並印出驗證；`path` 接 `ball_path` 讓球跑 |
| `gh_script/ball_field.cs` | Part 4 V5：距離場 ＋ 一顆球從任何起點照箭頭走 |
| `gh_script/field_boids.cs` | Part 5（課程最後）方法 B：Boids 距離場導航 |

## 在 GH 組起來

1. 新的 .gh 檔**存在 `gh_script/` 資料夾裡**（`#load "..\src\..."` 的相對路徑以這個位置為準）。
2. 依課程進度放 C# Script 元件，依下表加輸入輸出，把對應的 `gh_script/*.cs` 全部貼進去。先做 `path_dijkstra` 與 `ball_path`，Boids 留到最後。
3. 在 VS Code 改 `src/` 的檔案、存檔，回 GH 讓元件重新運算。

每個 `gh_script/*.cs` 的開頭都和 GH 新建 C# 元件的範本相同：第一行 `// Grasshopper Script Instance`，接著是 `src/` 全部檔案的 `#load`，再來是 `#region Usings`（多一行 `using AlgoDesign;`）與 `Script_Instance` 類別。`src/` 新增檔案時，每個 script 的 `#load` 清單都要一起補上。

> 相對路徑不能用時，把 `#load` 改成絕對路徑，例如 `#load "C:\Users\你\Desktop\115a_algo\src\Grid.cs"`。結果記到 `docs/loading.md`。

### Part 3｜`path_dijkstra.cs`：找路

| 索引 | 名稱 | 型別 | 存取 | 建議值 |
| --- | --- | --- | --- | --- |
| 0 | origin | Point3d | Item | (0,0,0) |
| 1 | width | int | Item | 40 |
| 2 | height | int | Item | 40 |
| 3 | cellSize | double | Item | 1 |
| 4 | obstacles | Curve | List | Rhino 畫的封閉曲線（可不接） |
| 5 | start | Point3d | Item | (1,1,0) |
| 6 | goal | Point3d | Item | (38,38,0) |

輸出：`path`（Polyline）、`field`（上色 Mesh）。`out` 會印出總成本、路徑格數、定案格數、曼哈頓距離。

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
| 不接障礙、成本全 1 | 總成本 ＝ 曼哈頓距離；路徑格數 ＝ 總成本 ＋ 1 |
| 起點與終點對調 | 總成本不變 |
| 終點被障礙圍住 | 印出「找不到路」，沒有路徑 |
| `field` 網格 | 起點附近藍、越遠越紅、擋住的深灰、沒擴散到的白 |
| 球 | 從起點沿路走到終點停下，`out` 印出「到終點了」 |

## TODO 與課堂步驟對照

| TODO | 位置 | 對應的一句話 |
| --- | --- | --- |
| 3-1 | `Reset()`、`Solve()` 開頭 | 初始化：全部「還走不到」，起點成本 0 |
| 3-2 | `TryPickNearest()`、主迴圈前半 | 第一句：定案最便宜的 |
| 3-3 | 主迴圈後半 | 第二句：更新它的鄰居 |
| 3-4 | `BuildPath()` | 從終點沿 `Previous` 走回起點 |

## Part 4、5 完整版（`1117-finish`）

### Part 4｜`path_search.cs`：V1～V4

前 7 個輸入和 `path_dijkstra` 相同，後面接：

| 索引 | 名稱 | 型別 | 存取 | 建議值 |
| --- | --- | --- | --- | --- |
| 0–6 | origin … goal | 同 `path_dijkstra` | | |
| 7 | allowDiagonal | bool | Item | Toggle（V1） |
| 8 | useAStar | bool | Item | Toggle（V3） |
| 9 | attractors | Point3d | List | 可不接（V4） |
| 10 | attractorRadius | double | Item | 8 |
| 11 | attractorStrength | double | Item | 10 |
| 12 | noiseScale | double | Item | 8（每幾格一個起伏；0 關掉） |
| 13 | noiseStrength | double | Item | 0～5（0 關掉） |
| 14 | seed | int | Item | 0 |

輸出：`path`（進階版路徑）、`field`（距離場）、`costMap`（成本場）、`basicPath`（基礎版路徑，疊在一起比較）。`out` 會印出兩版的成本、定案格數、毫秒，以及 V2、V3 的驗證 ✔／✘。把 `path` 接到 `ball_path`，每一版都讓球重新跑一次。

### Part 4｜`ball_field.cs`：V5 距離場 ＋ 一顆球照箭頭走

| 索引 | 名稱 | 型別 | 存取 | 建議值 |
| --- | --- | --- | --- | --- |
| 0 | origin | Point3d | Item | (0,0,0) |
| 1 | width | int | Item | 40 |
| 2 | height | int | Item | 40 |
| 3 | cellSize | double | Item | 1 |
| 4 | obstacles | Curve | List | 封閉曲線 |
| 5 | start | Point3d | Item | 球的起點，任何位置都可以 |
| 6 | goal | Point3d | Item | 出口位置 |
| 7 | allowDiagonal | bool | Item | true |
| 8 | attractors | Point3d | List | 可不接 |
| 9 | attractorRadius | double | Item | 8 |
| 10 | attractorStrength | double | Item | 10 |
| 11 | speed | double | Item | 0.3 |
| 12 | radius | double | Item | 0.5 |
| 13 | reset | bool | Item | Button |
| 14 | run | bool | Item | Boolean Toggle ＋ Timer |

輸出：`ball`（Sphere）、`trail`（軌跡）、`arrows`（每格往終點的箭頭）、`field`（距離場上色）。不用事先找路，球放在哪裡都會照箭頭走到終點。**改了格子、障礙、終點、成本或 start，要按 `reset` 才會重算。**

### Part 5（課程最後）｜`field_boids.cs`：Boids 距離場導航（方法 B）

| 索引 | 名稱 | 型別 | 存取 | 建議值 |
| --- | --- | --- | --- | --- |
| 0 | origin | Point3d | Item | (0,0,0) |
| 1 | width | int | Item | 40 |
| 2 | height | int | Item | 40 |
| 3 | cellSize | double | Item | 1 |
| 4 | obstacles | Curve | List | 封閉曲線 |
| 5 | goal | Point3d | Item | 出口位置 |
| 6 | allowDiagonal | bool | Item | true |
| 7 | attractors | Point3d | List | 可不接 |
| 8 | attractorRadius | double | Item | 8 |
| 9 | attractorStrength | double | Item | 10 |
| 10 | boidCount | int | Item | 200 |
| 11 | seed | int | Item | 0 |
| 12 | maxSpeed | double | Item | 0.3 |
| 13 | maxForce | double | Item | 0.05 |
| 14 | viewRadius | double | Item | 3 |
| 15 | alignmentWeight | double | Item | 0.5 |
| 16 | cohesionWeight | double | Item | 0.3 |
| 17 | separationRadius | double | Item | 1 |
| 18 | separationWeight | double | Item | 1.5 |
| 19 | avoidRadius | double | Item | 2 |
| 20 | avoidWeight | double | Item | 3 |
| 21 | fieldWeight | double | Item | 1.5 |
| 22 | reset | bool | Item | Button |
| 23 | run | bool | Item | Boolean Toggle ＋ Timer |

輸出：`points`、`headings`、`arrows`（每格往終點的箭頭）、`field`（距離場上色）。鳥散在整個格子範圍出生，從四面八方匯流到出口。**改了格子、障礙、終點、成本的輸入，要按 `reset` 才會重算距離場。**

### 驗證（out 印出，或對照下表）

| 版 | 驗證方式 |
| --- | --- |
| V1 8 鄰居 | 無障礙時成本 ＝ 斜走步數 ×√2 ＋ 直走步數；不會從兩個障礙的牆角縫鑽過去 |
| V2 優先佇列 | 4 鄰居時成本和基礎版完全相同，速度快很多 |
| V3 A\* | 成本和同設定的 Dijkstra 相同，定案格數比較少 |
| V4 成本場 | 路徑繞開高成本區；成本調低時改成直接穿過 |
| V5 距離場 | 任何一格沿箭頭走都會到終點；球從任何起點放下都會停在終點格 |

參考速度（Rhino 外的測試，40×40／100×100）：基礎版 23／861 ms，優先佇列 1.2／6.8 ms，A\* 0.8／3.8 ms。
