"""Make one-line "DJ Name" title variants from the existing two-line SVGs.

Keeps the original gold treatment (shadow, extrusion, outline, metal fill,
bottom ornament), drops the small "DJ" line and its rules, and fits
"DJ Name" on one line. Writes dj-<id>-line.svg plus transparent PNGs at
1248 x 540 and 1664 x 720 (master) using headless Chrome. Originals are
not modified.

Usage: python3 scripts/make_dj_titles_line.py
"""
import re
import subprocess
import tempfile
from pathlib import Path

from PIL import ImageFont

ROOT = Path(__file__).resolve().parent.parent
TITLES = ROOT / "outputs" / "dj-titles"
CHROME = "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome"
FONT = ImageFont.truetype("/System/Library/Fonts/Supplemental/Arial Bold.ttf", 100)
NAMES = {"edwin": "Edwin", "mumbai-rani": "Mumbai Rani", "dinu": "Dinu"}
FACE_WIDTH = 1000  # px at 1248 wide, before outline, extrusion and shadow
MAX_SIZE = 190
CAP_HEIGHT = 0.716  # Arial Bold cap height in em


def one_line(svg: str, name: str) -> str:
    label = f"DJ {name}"
    size = min(MAX_SIZE, round(100 * FACE_WIDTH / FONT.getlength(label)))
    baseline = round(270 + CAP_HEIGHT * size / 2 - 6)
    # Drop the small "DJ" line and the rules beside it.
    svg = re.sub(r'<g [^>]*><text [^>]*font-size="39"[^>]*>DJ</text></g>\n', "", svg)
    svg = re.sub(r'<path d="M452 150H554 M694 150H796"[^>]*/>\n', "", svg)
    # Resize and re-centre every copy of the name, then change its text.
    svg = re.sub(r'y="344"', f'y="{baseline}"', svg)
    svg = re.sub(r'font-size="\d+"', f'font-size="{size}"', svg)
    svg = svg.replace(f">{name}</text>", f">{label}</text>")
    # Keep the bottom ornament a quarter em (at least 44 px) below the baseline.
    shift = baseline + max(44, round(0.25 * size)) - 386
    svg = re.sub(r'(<path d="M612 386.*?/>\n<path d="M489 386.*?/>)',
                 rf'<g transform="translate(0,{shift})">\1</g>', svg, flags=re.S)
    svg = svg.replace(f'aria-label="DJ {name}"', f'aria-label="{label}"')
    return svg


def render(svg_path: Path, png_path: Path, width: int, height: int) -> None:
    with tempfile.TemporaryDirectory() as tmp:
        page = Path(tmp) / "page.html"
        page.write_text(
            '<!doctype html><html><body style="margin:0;background:transparent">'
            f'<img src="{svg_path.as_uri()}" width="{width}" height="{height}" style="display:block">'
            "</body></html>")
        subprocess.run([CHROME, "--headless=new", "--disable-gpu", "--hide-scrollbars",
                        "--force-device-scale-factor=1", "--default-background-color=00000000",
                        f"--window-size={width},{height}", f"--screenshot={png_path}",
                        "--allow-file-access-from-files", page.as_uri()],
                       check=True, capture_output=True)


for slug, name in NAMES.items():
    source = (TITLES / f"dj-{slug}.svg").read_text()
    svg_path = TITLES / f"dj-{slug}-line.svg"
    svg_path.write_text(one_line(source, name))
    render(svg_path, TITLES / f"dj-{slug}-line.png", 1248, 540)
    render(svg_path, TITLES / f"dj-{slug}-line-master.png", 1664, 720)
    print("wrote", svg_path.name)
