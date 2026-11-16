r"""產生 gh_script\ready\*.cs：樣板的 //@LOADS@ 換成 src 每個 .cs 的 #load 絕對路徑。

GH 的 C# Script 元件，#load 只吃寫死的絕對路徑
（相對路徑、萬用字元都不行，11/03 boids_game 已在 GH 實測）。
clone 到新位置、換電腦、src 增減檔案之後，都要重跑一次，
再把 ready 裡的檔整份貼進 GH。

用法
  python tools\gen_gh_scripts.py
"""

import subprocess
import sys
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8")

ROOT = Path(__file__).resolve().parent.parent
SOURCE = ROOT / "src"
TEMPLATES = ROOT / "gh_script"
OUT = TEMPLATES / "ready"
PLACEHOLDER = "//@LOADS@"

# 有相依的放前面；清單外的檔（各元件的 *Host）依檔名排在最後
ORDER = [
    "Cell", "Grid", "Edge",
    "Graph", "MeshGraph", "GridGraph",
    "Dijkstra", "CellHeap", "PathSearch",
    "CostField", "ValueColor", "FieldDisplay",
    "GraphDisplay", "FlowField", "Ball",
    "Boid", "Flock",
    "ObstacleBuilder", "Greeter",
]


def load_lines() -> str:
    """src 底下每個 .cs 一行 #load，用絕對路徑。"""
    files = {path.stem: path for path in SOURCE.glob("*.cs")}
    names = [n for n in ORDER if n in files] + sorted(
        n for n in files if n not in ORDER
    )
    return "\n".join(f'#load "{files[name]}"' for name in names)


def current_branch() -> str:
    """目前的 git 分支名稱；拿不到時回傳「未知」。"""
    try:
        result = subprocess.run(
            ["git", "-C", str(ROOT), "branch", "--show-current"],
            capture_output=True,
            text=True,
            check=True,
        )
        return result.stdout.strip() or "未知"
    except (OSError, subprocess.CalledProcessError):
        return "未知"


def main() -> None:
    """每個含 //@LOADS@ 的樣板，各產生一份貼進 GH 用的檔到 ready。"""
    branch = current_branch()
    count = len(list(SOURCE.glob("*.cs")))

    # #load 一定放最前面；分支資訊接在後面，切換分支後一眼看出貼的是不是舊版
    stamp = (
        f"// 由 tools\\gen_gh_scripts.py 產生：分支 {branch}，"
        f"src 共 {count} 個檔。切換分支後要重跑並重新貼上"
    )
    loads = load_lines() + "\n" + stamp
    OUT.mkdir(exist_ok=True)

    # 先清掉舊檔：ready 不進版控，切換分支後會留著別的分支產生的檔
    for old in OUT.glob("*.cs"):
        old.unlink()

    for template in sorted(TEMPLATES.glob("*.cs")):
        text = template.read_text(encoding="utf-8").replace("\r\n", "\n")
        if PLACEHOLDER not in text:
            print(f"略過（沒有 {PLACEHOLDER}）：{template.name}")
            continue
        target = OUT / template.name
        target.write_text(
            text.replace(PLACEHOLDER, loads), encoding="utf-8", newline="\r\n"
        )
        print(f"產生 {target.relative_to(ROOT)}")
    print(f"分支 {branch}：#load 共 {count} 行，來源：{SOURCE}")


if __name__ == "__main__":
    main()
