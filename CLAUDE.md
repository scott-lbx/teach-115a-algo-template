## 專案介紹

演算法設計課專案，目標是製作 Grasshopper（GH）的 C# Script Component。

所有規劃原則都放在 `.claude/`。

## 目錄結構

- `src/`：所有程式碼檔案（`.cs`）都放這裡
- `gh_script/`：只放要複製貼到 GH Script Component 的檔案，由 `tools/gen_gh_scripts.py` 從 `src/gh_templates/` 產生（`#load` 是本機絕對路徑，不進版控）
- `docs/`：討論用的 `.md` 檔案
- `.claude/`：規劃原則（`CODE.md`、`GIT.md`）與 skill

## 回覆規則

每次回答結束要說 "是的船長"

請用繁體中文回答我

## 讀取檔案 .claude\

- 要新增，修改，刪除任何程式碼 請先讀取CODE.md
- 要git 動作，請先讀取GIT.md
