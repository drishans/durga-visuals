"""Make a labeled review sheet from the still-image catalog."""
import json
import math
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

root = Path(__file__).resolve().parents[1]
records = [json.loads(p.read_text()) for p in sorted((root / 'outputs/catalog').glob('*.json'))]
width, height, gap = 480, 245, 18
columns = 3
rows = math.ceil(len(records) / columns)
sheet = Image.new('RGB', (columns * (width+gap)+gap, rows * (height+gap)+70), '#111113')
draw = ImageDraw.Draw(sheet)
try:
    font = ImageFont.truetype('C:/Windows/Fonts/segoeui.ttf', 18)
    heading = ImageFont.truetype('C:/Windows/Fonts/segoeuib.ttf', 26)
except OSError:
    font = heading = ImageFont.load_default()
draw.text((gap, 16), 'GOLDEN GLAMOUR / STILL COLLECTION', font=heading, fill='#d6b572')
for i, record in enumerate(records):
    x, y = gap+(i%columns)*(width+gap), 62+(i//columns)*(height+gap)
    with Image.open(root / record['files']['wall']) as frame:
        frame = frame.convert('RGB').resize((width, 208), Image.Resampling.LANCZOS)
        sheet.paste(frame, (x, y))
    draw.text((x, y+214), record['id']+' / '+record['title'].replace(' (starter)', ''), font=font, fill='#ede8df')
target = root / 'outputs/previews/still-collection.jpg'
sheet.save(target, quality=92)
print(target)
