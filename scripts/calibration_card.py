"""Create the static LED framing card; use at native composition size."""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[1]
image = Image.new('RGB', (1248, 540), '#101014')
draw = ImageDraw.Draw(image)
font_path = Path('C:/Windows/Fonts/segoeui.ttf')


def font(size):
    return ImageFont.truetype(str(font_path), size) if font_path.exists() else ImageFont.load_default(size=size)


def label(position, text, size=20, fill='white', anchor='mm'):
    draw.text(position, text, font=font(size), fill=fill, anchor=anchor)


# Logical 13 x 5 panel grid from the supplied wall layout, not a processor map.
for x in range(0, 1248, 96):
    draw.line((x, 0, x, 539), fill='#34343b')
for y in range(0, 540, 108):
    draw.line((0, y, 1247, y), fill='#34343b')
draw.rectangle((1, 1, 1246, 538), outline='white', width=2)
draw.rectangle((12, 12, 1235, 527), outline='#d5b778', width=1)
label((26, 24), 'TOP LEFT', anchor='lt')
label((1222, 24), 'TOP RIGHT', anchor='rt')
label((26, 516), 'BOTTOM LEFT', anchor='lb')
label((1222, 516), 'BOTTOM RIGHT', anchor='rb')
label((624, 52), 'DURGA / WALL CHECK', 28, '#d5b778')
label((624, 87), '1248 x 540  |  104:45  |  13 columns x 5 rows', 19)

# A circle and square reveal nonuniform scaling. Crosshair marks their center.
draw.ellipse((534, 150, 714, 330), outline='white', width=2)
draw.rectangle((534, 150, 714, 330), outline='#777780', width=1)
draw.line((609, 240, 639, 240), fill='#d5b778', width=2)
draw.line((624, 225, 624, 255), fill='#d5b778', width=2)
label((624, 356), 'Circle stays round. All four borders remain visible.', 19)

# Reference patches only: this is not an instrument-based color calibration.
for i in range(11):
    value = round(i * 255 / 10)
    x = 184 + i * 80
    draw.rectangle((x, 394, x + 79, 441), fill=(value, value, value))
    label((x + 40, 457), str(value), 14, '#ccccd0')
label((624, 491), 'Grayscale steps 0-255 / framing and scaling reference', 16, '#b4b4bc')
output = ROOT / 'outputs' / 'calibration' / 'wall-check-1248x540.png'
output.parent.mkdir(parents=True, exist_ok=True)
image.save(output)
print(output)
