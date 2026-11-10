# #load 路徑實測

目的：確認 GH Script Component 的 `#load` 如何解析 `src/` 底下的 `.cs`。

**狀態：尚未實測**（範例檔已備好，需在 Rhino 內驗證）。

## 實測步驟

1. 在 Rhino 8 開 Grasshopper，放一個 C# Script Component。
2. 新增輸入 `name`（string）、輸出 `a`。
3. 貼上 `gh_script/main.cs` 的內容，輸入 `name = "GH"`。
4. 預期輸出 `Hello, GH`。

## 要記錄的結果

| 情況 | 結果 |
| --- | --- |
| 相對路徑 `..\src\Greeter.cs`（.gh 已存檔於專案內） | 待測 |
| 相對路徑（.gh 未存檔） | 待測 |
| 絕對路徑 | 待測 |
| 修改 `src/` 後，元件是否自動重新載入 | 待測 |

## 結論

待實測後填寫。
