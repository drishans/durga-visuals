"""Render periodic 2D displacement loops from selected stills, using the CPU."""
import json
import subprocess
from pathlib import Path
import numpy as np
from PIL import Image
from scipy.ndimage import map_coordinates

root = Path(__file__).resolve().parents[1]
width, height, fps, count = 1248, 540, 30, 240
jobs = [('FX_D03_gold_wave', 'D03_s2026101207.png', 12, 20),
        ('FX_P05_vermilion_flow', 'P05_s2026101204.png', 14, 16),
        ('FX_O03_gold_ribbons', 'O03_s2026101212.png', 10, 18)]
y, x = np.mgrid[:height, :width].astype(np.float32)
edge_x, edge_y = np.sin(np.pi*x/(width-1))**2, np.sin(np.pi*y/(height-1))**2
for name, source_name, amplitude_x, amplitude_y in jobs:
    source = root / 'outputs/stills/wall' / source_name
    pixels = np.asarray(Image.open(source).convert('RGB'), dtype=np.float32)
    master = root / 'outputs/video/prores' / (name + '.mov')
    loop = root / 'outputs/video/loops' / (name + '.mp4')
    command = ['ffmpeg', '-hide_banner', '-loglevel', 'error', '-y', '-f', 'rawvideo', '-pix_fmt', 'rgb24',
               '-s', f'{width}x{height}', '-r', str(fps), '-i', '-', '-an', '-c:v', 'prores_ks',
               '-profile:v', '1', '-pix_fmt', 'yuv422p10le', '-vendor', 'apl0', str(master)]
    encoder = subprocess.Popen(command, stdin=subprocess.PIPE)
    try:
        for index in range(count):
            phase = 2*np.pi*index/count
            map_x = x + amplitude_x*np.sin(2*np.pi*y/height+phase)*edge_x
            map_y = y + amplitude_y*np.sin(4*np.pi*x/width-phase)*edge_y
            frame = np.stack([map_coordinates(pixels[:,:,c], [map_y, map_x], order=1, mode='reflect') for c in range(3)], axis=-1)
            encoder.stdin.write(np.clip(np.rint(frame), 0, 255).astype(np.uint8).tobytes())
    finally:
        encoder.stdin.close()
        result = encoder.wait()
    if result: raise RuntimeError('Encoding failed: ' + name)
    subprocess.run(['ffmpeg', '-hide_banner', '-loglevel', 'error', '-y', '-i', str(master), '-an',
                    '-c:v', 'libx264', '-preset', 'slow', '-crf', '17', '-g', '1', '-pix_fmt', 'yuv420p',
                    '-movflags', '+faststart', '-map_metadata', '-1', str(loop)], check=True)
    record = dict(name=name, source=source.relative_to(root).as_posix(), loop=loop.relative_to(root).as_posix(),
                  prores=master.relative_to(root).as_posix(), dimensions=[width,height], fps=fps, frames=count,
                  duration_seconds=count/fps, amplitude_pixels=[amplitude_x,amplitude_y],
                  processing='Z-Image still animated with periodic sinusoidal 2D displacement. CPU-rendered texture motion, not AI video. Full-period sampling omits the duplicate endpoint; no crossfade or reversal. ProRes LT master and all-intra H.264 delivery.')
    (root / 'outputs/video/catalog' / (name+'.json')).write_text(json.dumps(record,indent=2)+'\n',encoding='utf-8')
    print('Delivered', name, flush=True)
