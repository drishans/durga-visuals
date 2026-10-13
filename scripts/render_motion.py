"""Animate one selected still through the native Wan workflow."""
import argparse
import json
import shutil
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from comfy_local import render

parser = argparse.ArgumentParser()
parser.add_argument('--comfy-root', required=True, type=Path)
parser.add_argument('--source', required=True, type=Path)
parser.add_argument('--name', required=True)
parser.add_argument('--prompt', required=True)
parser.add_argument('--seed', type=int, default=2026101250)
args = parser.parse_args()
root = Path(__file__).resolve().parents[1]
out = root / 'outputs'
graph = json.loads((out / 'golden-glamour-wan-motion-api.json').read_text())
workflow = json.loads((out / 'golden-glamour-wan-motion.json').read_text())
reference = args.comfy_root / 'input' / ('durga_2026_' + args.source.name)
reference.parent.mkdir(parents=True, exist_ok=True)
shutil.copy2(args.source, reference)
image = reference.name
prefix = 'durga_2026/video/' + args.name
graph['80']['inputs']['image'] = image
graph['90']['inputs']['text'] = args.prompt
graph['84']['inputs']['noise_seed'] = args.seed
graph['83']['inputs']['filename_prefix'] = prefix
for node in workflow['nodes']:
    if node['id'] == 80: node['widgets_values'] = [image, 'image']
    elif node['id'] == 90: node['widgets_values'] = [args.prompt]
    elif node['id'] == 84: node['widgets_values'][1] = args.seed
    elif node['id'] == 83: node['widgets_values'][0] = prefix
print('Rendering', args.name, flush=True)
result, seconds = render(graph, workflow)
raw_dir = out / 'video' / 'raw'
raw_dir.mkdir(parents=True, exist_ok=True)
info = result['outputs']['83']
print('Video output:', json.dumps(info), flush=True)
entries = info.get('images') or info.get('videos') or info.get('gifs')
if not entries:
    raise RuntimeError('Video completed but output paths were not returned: ' + json.dumps(info))
entry = entries[0]
source = args.comfy_root / 'output' / entry['subfolder'] / entry['filename']
target = raw_dir / (args.name + '.mp4')
shutil.copy2(source, target)
catalog = {'name': args.name, 'source': args.source.as_posix(), 'seed': args.seed, 'motion_prompt': args.prompt, 'seconds': seconds, 'raw': target.relative_to(root).as_posix(), 'workflow': workflow, 'api': graph}
(raw_dir / (args.name + '.json')).write_text(json.dumps(catalog, indent=2) + '\n', encoding='utf-8')
print('Completed', args.name, seconds, flush=True)
