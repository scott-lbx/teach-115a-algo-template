---
name: git
description: 依照專案 .claude/GIT.md 的原則建立 git commit（conventional commit、繁體中文、不附 AI 署名）。使用者輸入 /git 或要求依規則 commit 時使用。
---

# git：依 GIT.md 規則 commit

## 步驟

1. **讀規則**：先讀取 `.claude/GIT.md`（專案根目錄下），所有 commit 訊息以該檔為準。檔案不存在就停止並告知使用者。
2. **看變更**：執行 `git status --short` 與 `git diff`（含已 staged 的內容）。沒有任何變更就告知使用者，不要建立空 commit。
3. **選檔案**：只 stage 這次要提交的檔案，逐一列名加入 `git add <檔案>`，不用 `git add -A` 或 `git add .`。若變更混雜不相關的檔案，先列出並詢問使用者要包含哪些。
4. **寫訊息**：依 GIT.md 規範撰寫 conventional commit（`type(scope): 摘要`），摘要與內文用繁體中文，簡潔說明「改了什麼、為什麼」。不加入 Co-Authored-By 或任何 AI 署名。
5. **commit**：用 heredoc 傳入訊息，例如：

   ```bash
   git commit -F - <<'EOF'
   type: 摘要

   - 內文
   EOF
   ```

6. **回報**：執行 `git log --oneline -1` 與 `git status --short`，告知 commit hash、訊息與剩餘未提交的檔案。

## 注意

- 不 push、不 amend、不 reset，除非使用者明確要求。
- 不 commit 疑似機密檔案（`.env`、金鑰、token）；發現時停止並詢問。
- 若 GIT.md 規則與上述步驟衝突，以 GIT.md 為準。
