# 程式碼的撰寫原則

- 程式碼依照 SOLID DRY OOP 習慣原則

## C# 規範

- 所有類別放在 `AlgoDesign` namespace 下，一個檔案一個主要類別，檔名同類別名
- 類別、方法、屬性用 PascalCase；區域變數、參數用 camelCase；私有欄位加 `_` 前綴
- 演算法邏輯寫在 `src/`，不依賴 GH 的輸入輸出，以便單獨測試與重用
- GH script 的樣板放 `src/gh_templates/`，只做輸入輸出接線，呼叫 `src/` 的類別（各元件的 `*Host`），不放演算法本體；`gh_script/` 只放產生出來、貼進 GH 的檔
- 註解與 docstring 用繁體中文
