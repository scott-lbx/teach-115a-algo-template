r"""產生 gh_script\ready\*.cs：樣板的 //@LOADS@ 換成 src 每個 .cs 的 #load 絕對路徑。

GH 的 C# Script 元件，#load 只吃寫死的絕對路徑
（相對路徑、萬用字元都不行，11/03 boids_game 已在 GH 實測）。
clone 到新位置、換電腦、src 增減檔案之後，都要重跑一次，
再把 ready 裡的檔整份貼進 GH。

用法
  python tools\gen_gh_scripts.py
"""

import sys
from pathlib import Path

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8")

ROOT = Path(__file__).resolve().parent.parent
SOURCE = ROOT / "src"
TEMPLATES = ROOT / "gh_script"
OUT = TEMPLATES / "ready"
PLACEHOLDER = "//@LOADS@"

# 有相依的放前面；清單外的檔依檔名排在最後
ORDER = [
    "Cell", "Grid", "Dijkstra",
    "CellHeap", "PathSearch", "CostField",
    "FieldDisplay", "FlowField", "Ball",
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


def main() -> None:
    """每個含 //@LOADS@ 的樣板，各產生一份貼進 GH 用的檔到 ready。"""
    loads = load_lines()
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
    print(f"#load 共 {loads.count(chr(10)) + 1} 行，來源：{SOURCE}")


if __name__ == "__main__":
    main()
