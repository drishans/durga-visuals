"""Render Ma prompt studies locally; candidates are not automatically show-approved."""
import argparse
import copy
import json
import shutil
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from comfy_local import render


def main():
    root = Path(__file__).resolve().parents[1]
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--comfy-root', type=Path, required=True)
    parser.add_argument('--prompts', type=Path, default=root / 'Ma/prompts.json')
    parser.add_argument('--ids', nargs='+', help='Only these prompt IDs')
    parser.add_argument('--model', choices=['qwen-image', 'z-image'], default='qwen-image')
    parser.add_argument('--variants', type=int, default=1)
    parser.add_argument('--seed', type=int, default=2026101300)
    parser.add_argument('--steps', type=int, help='Override workflow step count')
    parser.add_argument('--output', type=Path, default=root / 'work/ma-candidates')
    args = parser.parse_args()
    if args.variants < 1 or (args.steps is not None and args.steps < 1):
        parser.error('variants and steps must be positive')
    prompts = json.loads(args.prompts.read_text(encoding='utf-8-sig'))['prompts']
    if args.ids and set(args.ids) - {p['id'] for p in prompts}:
        parser.error('Unknown prompt ID')
    base = json.loads((root / f'Ma/{args.model}-api.json').read_text())
    ui = json.loads((root / f'Ma/{args.model}.json').read_text())
    steps = args.steps or base['8']['inputs']['steps']
    for variant in range(args.variants):
        for index, prompt in enumerate(prompts):
            if args.ids and prompt['id'] not in args.ids:
                continue
            seed = args.seed + variant * 100 + index
            name = f"{args.model}_{prompt['id']}_s{seed}_{steps}steps"
            folder = args.output / name
            if (folder / 'render.json').exists():
                continue
            graph, workflow = copy.deepcopy(base), copy.deepcopy(ui)
            graph['5']['inputs']['text'] = prompt['positive']
            graph['6']['inputs']['text'] = prompt['negative']
            graph['8']['inputs'].update(seed=seed, steps=steps)
            for node_id, kind in [('10', 'master'), ('12', 'wall')]:
                graph[node_id]['inputs']['filename_prefix'] = f'durga_2026/Ma/{name}/{kind}'
            for node in workflow['nodes']:
                key = str(node['id'])
                if key not in graph:
                    continue
                inputs = graph[key]['inputs']
                named = node.get('widgets_values_named', {})
                for widget in named:
                    if widget in inputs:
                        named[widget] = inputs[widget]
                if key in ('5', '6'):
                    node['widgets_values'] = [inputs['text']]
                elif key == '8':
                    node['widgets_values'][0] = seed
                    node['widgets_values'][2] = steps
                elif key in ('10', '12'):
                    node['widgets_values'] = [inputs['filename_prefix']]
            print('Rendering', name, prompt['title'], flush=True)
            result, seconds = render(graph, workflow)
            folder.mkdir(parents=True, exist_ok=True)
            for node_id, kind in [('10', 'master'), ('12', 'wall')]:
                info = result['outputs'][node_id]['images'][0]
                source = args.comfy_root / 'output' / info['subfolder'] / info['filename']
                shutil.copy2(source, folder / f'{kind}.png')
            metadata = dict(prompt, seed=seed, steps=steps, model=args.model, seconds=seconds,
                            status='unreviewed', api=graph)
            (folder / 'render.json').write_text(json.dumps(metadata, indent=2) + '\n', encoding='utf-8')
            print('Completed', name, seconds, 'seconds', flush=True)


if __name__ == '__main__':
    main()
