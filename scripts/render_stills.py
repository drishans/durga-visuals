"""Render the curated prompt file through a running local ComfyUI."""
import argparse
import copy
import json
import re
import shutil
import sys
from pathlib import Path
# ComfyUI's embedded Python does not add the script folder to sys.path.
sys.path.insert(0, str(Path(__file__).resolve().parent))
from comfy_local import render

parser = argparse.ArgumentParser()
parser.add_argument('--comfy-root', required=True, type=Path)
parser.add_argument('--variants', type=int, default=1)
parser.add_argument('--seed', type=int, default=2026101200)
parser.add_argument('--prompts', type=Path, help='Prompt Markdown; defaults to the original collection')
parser.add_argument('--limit', type=int, help='Render only the first N concepts, useful for a sample')
args = parser.parse_args()
root = Path(__file__).resolve().parents[1]
out = root / 'outputs'
graph_template = json.loads((out / 'golden-glamour-z-image-api.json').read_text())
ui_template = json.loads((out / 'golden-glamour-z-image.json').read_text())
prompt_file = args.prompts or out / 'golden-glamour-prompts.md'
prompts = re.findall(r'### ([PDOS]\d+) - ([^\n]+)\s+```text\n(.*?)\n```', prompt_file.read_text(encoding='utf-8'), re.S)
if not prompts:
    parser.error('No prompts found in ' + str(prompt_file))
if args.variants < 1 or (args.limit is not None and args.limit < 1):
    parser.error('variants and limit must be positive')
if args.limit is not None:
    prompts = prompts[:args.limit]
catalog = out / 'catalog'
catalog.mkdir(exist_ok=True)
for variant in range(args.variants):
    for index, (prompt_id, title, positive) in enumerate(prompts):
        seed = args.seed + variant * 100 + index
        name = f'{prompt_id}_s{seed}'
        marker = catalog / (name + '.json')
        if marker.exists():
            continue
        graph, workflow = copy.deepcopy(graph_template), copy.deepcopy(ui_template)
        graph['5']['inputs']['text'] = positive
        graph['8']['inputs']['seed'] = seed
        for node in workflow['nodes']:
            if node['id'] == 5:
                node['widgets_values'] = [positive]
                node['title'] = f'Positive prompt - {prompt_id} {title}'
            elif node['id'] == 8:
                node['widgets_values'][0] = seed
        for node_id, kind in [('10', 'master'), ('12', 'wall')]:
            prefix = f'durga_2026/stills/{kind}/{name}'
            graph[node_id]['inputs']['filename_prefix'] = prefix
            next(n for n in workflow['nodes'] if n['id'] == int(node_id))['widgets_values'] = [prefix]
        print('rendering', name, title, flush=True)
        result, seconds = render(graph, workflow)
        files = {}
        for node_id, kind in [('10', 'master'), ('12', 'wall')]:
            info = result['outputs'][node_id]['images'][0]
            source = args.comfy_root / 'output' / info['subfolder'] / info['filename']
            target = out / 'stills' / kind / (name + '.png')
            target.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(source, target)
            files[kind] = target.relative_to(root).as_posix()
        marker.write_text(json.dumps({'id': prompt_id, 'title': title, 'seed': seed, 'prompt': positive, 'seconds': seconds, 'files': files}, indent=2) + '\n', encoding='utf-8')
        print('completed', name, seconds, flush=True)
