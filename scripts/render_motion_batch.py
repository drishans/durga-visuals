"""Run selected motion prompts, retaining raw renders and delivering loops."""
import argparse
import json
import subprocess
import sys
from pathlib import Path

parser = argparse.ArgumentParser()
parser.add_argument('--comfy-root', required=True)
parser.add_argument('--start', type=int, default=0)
parser.add_argument('--count', type=int, default=1)
args = parser.parse_args()
root = Path(__file__).resolve().parents[1]
jobs = json.loads((root / 'outputs/golden-glamour-motion-prompts.json').read_text())
for job in jobs[args.start:args.start+args.count]:
    raw = root / 'outputs/video/raw' / (job['name'] + '.mp4')
    if not raw.exists():
        subprocess.run([sys.executable, '-s', '-u', str(root / 'scripts/render_motion.py'),
                        '--comfy-root', args.comfy_root, '--source', str(root / 'outputs/stills/wall' / job['source']),
                        '--name', job['name'], '--seed', str(job['seed']), '--prompt', job['prompt']], check=True)
    marker = root / 'outputs/video/catalog' / (job['name'] + '.json')
    if not marker.exists():
        subprocess.run([sys.executable, '-s', '-u', str(root / 'scripts/make_loop.py'), str(raw)], check=True)
    print('Delivered', job['name'], flush=True)
