"""Make a smooth forward/reverse ambient loop from a Wan clip."""
import argparse
import json
import subprocess
from pathlib import Path
import numpy as np


def run(*args):
    subprocess.run(['ffmpeg', '-hide_banner', '-loglevel', 'error', '-y', *map(str, args)], check=True)


parser = argparse.ArgumentParser()
parser.add_argument('source', type=Path)
parser.add_argument('--duration', type=float, default=8)
args = parser.parse_args()
if args.duration <= 0:
    raise ValueError('Duration must be positive.')
root = Path(__file__).resolve().parents[1]
name = args.source.stem
loops = root / 'outputs/video/loops'
masters = root / 'outputs/video/prores'
catalog = root / 'outputs/video/catalog'
for folder in (loops, masters, catalog): folder.mkdir(parents=True, exist_ok=True)
loop, master = loops / (name + '.mp4'), masters / (name + '.mov')
width, height, fps = 1248, 540, 30
count = round(args.duration * fps)
# Optical flow supplies closer temporal samples before the periodic time remap.
decoded = subprocess.check_output([
    'ffmpeg', '-hide_banner', '-loglevel', 'error', '-i', str(args.source),
    '-vf', 'crop=1248:540:0:2,minterpolate=fps=60:mi_mode=mci:mc_mode=aobmc:me_mode=bidir:vsbmc=1',
    '-f', 'rawvideo', '-pix_fmt', 'rgb24', '-an', '-'])
frames = np.frombuffer(decoded, np.uint8).reshape(-1, height, width, 3)
command = ['ffmpeg', '-hide_banner', '-loglevel', 'error', '-y', '-f', 'rawvideo', '-pix_fmt', 'rgb24',
           '-s', f'{width}x{height}', '-r', str(fps), '-i', '-', '-an', '-c:v', 'prores_ks',
           '-profile:v', '1', '-pix_fmt', 'yuv422p10le', '-vendor', 'apl0', str(master)]
encoder = subprocess.Popen(command, stdin=subprocess.PIPE)
try:
    for index in range(count):
        phase = 2 * np.pi * index / count
        position = (1 - np.cos(phase)) * 0.5 * (len(frames) - 1)
        lower = int(position)
        upper = min(lower + 1, len(frames) - 1)
        fraction = position - lower
        frame = np.rint(frames[lower].astype(np.float32) * (1-fraction) + frames[upper] * fraction).astype(np.uint8)
        encoder.stdin.write(frame.tobytes())
finally:
    encoder.stdin.close()
    result = encoder.wait()
if result:
    raise RuntimeError('ProRes encoding failed.')
run('-i', master, '-an', '-c:v', 'libx264', '-preset', 'slow', '-crf', 17, '-g', 1, '-pix_fmt', 'yuv420p', '-movflags', '+faststart', '-map_metadata', '-1', loop)
probe = json.loads(subprocess.check_output(['ffprobe', '-v', 'error', '-select_streams', 'v:0', '-show_streams', '-of', 'json', str(loop)]))['streams'][0]
if (probe['width'], probe['height'], int(probe['nb_frames'])) != (width, height, count):
    raise RuntimeError('Unexpected delivery dimensions or frame count.')
run('-i', loop, '-f', 'null', '-')
source = json.loads(args.source.with_suffix('.json').read_text())
record = {key: source[key] for key in ('name', 'seed', 'motion_prompt')}
record.update(source='outputs/stills/wall/' + Path(source['source']).name,
              loop=loop.relative_to(root).as_posix(), prores=master.relative_to(root).as_posix(),
              dimensions=[width, height], fps=fps, frames=count, duration_seconds=count/fps,
              processing=f'Wan 2.2 first/last frame, 81 frames at 16 fps; crop 2 px top/bottom; optical-flow interpolation to 60 fps; cosine time remap forward then reverse over one {count/fps:g}-second cycle, delivered at 30 fps. Eased reversals avoid a hard cut or a tail-to-head dissolve. ProRes LT master and all-intra H.264 delivery.')
(catalog / (name + '.json')).write_text(json.dumps(record, indent=2) + '\n', encoding='utf-8')
print('Verified loop:', loop, flush=True)
