# 開發備註

本檔收錄原 README 中與開發習慣有關的內容。

## 專案設定檔

| 路徑 | 用途 |
| --- | --- |
| `.claude/` | 規劃原則（`CODE.md`、`GIT.md`）與 skill |
| `CLAUDE.md` | Claude Code 的專案說明與回覆規則（繁體中文回答） |

## 工作規則（出自 CLAUDE.md）

- 新增、修改、刪除程式碼前，先讀 `.claude/CODE.md`。
- 執行 git 動作前，先讀 `.claude/GIT.md`。
- 所有規劃原則都放在 `.claude/`。

## 專案結構

- `src/`：所有程式碼檔案（`.cs`）
- `gh_script/`：要複製貼到 GH Script Component 的檔案，以 `#load` 讀取 `src/` 底下的 `.cs`
- `docs/`：討論用的 `.md` 檔案
