"""Build an offline review page from the completed asset catalogs."""
import html
import json
from pathlib import Path

root = Path(__file__).resolve().parents[1]
out = root / 'outputs'
stills = [json.loads(p.read_text()) for p in sorted((out/'catalog').glob('*.json'))]
videos = [json.loads(p.read_text()) for p in sorted((out/'video/catalog').glob('*.json'))]
escape = html.escape
cards = []
for item in videos:
    name = item['name']
    title = name.replace('FX_', '').replace('_', ' ')
    kind = 'Procedural flow' if name.startswith('FX_') else 'Wan animation'
    source = item['source'].removeprefix('outputs/')
    clip = item['loop'].removeprefix('outputs/')
    cards.append(f'<article><video id="{escape(name)}" loop playsinline controls preload="none" poster="{escape(source)}"><source src="{escape(clip)}" type="video/mp4"></video><div class="card-info"><h3>{escape(title)}</h3><p>{kind} · 8 seconds · 30 fps</p><div class="actions"><button data-play="{escape(name)}">Play</button><a href="{escape(clip)}" download>MP4</a></div></div></article>')
still_cards = []
for item in stills:
    wall = item['files']['wall'].removeprefix('outputs/')
    master = item['files']['master'].removeprefix('outputs/')
    title = item['id'] + ' / ' + item['title'].replace(' (starter)', '')
    still_cards.append(f'<article><a href="{escape(wall)}"><img loading="lazy" src="{escape(wall)}" alt="{escape(title)}"></a><div class="card-info"><h3>{escape(title)}</h3><p>1248 × 540 wall · 1664 × 720 master</p><div class="actions"><a href="{escape(wall)}" download>Wall PNG</a><a href="{escape(master)}" download>Master PNG</a></div></div></article>')
page = '''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Golden Glamour - Collection</title>
<style>*{box-sizing:border-box}body{margin:0;background:#111113;color:#eeeae2;font:15px system-ui,sans-serif}header,main{max-width:1500px;margin:auto;padding:30px}header{border-bottom:1px solid #343238;display:flex;justify-content:space-between;gap:24px;align-items:center}small{color:#c8ac70;font-size:10px;letter-spacing:2px}h1{font-size:32px;letter-spacing:-1px;font-weight:550;margin:7px 0}header p,.intro{color:#aaa39a;line-height:1.6;font-size:13px;margin:0}header nav{display:flex;gap:16px;flex-wrap:wrap}a{color:#dfc389;text-decoration:none}a:hover{text-decoration:underline}h2{font-weight:500;font-size:21px;margin:28px 0 15px}h2:first-child{margin-top:0}.grid{display:grid;grid-template-columns:repeat(3,minmax(0,1fr));gap:22px}article{border:1px solid #373338;border-radius:9px;overflow:hidden;background:#1b1a1e}img,video{display:block;width:100%;aspect-ratio:104/45;object-fit:cover;background:#030305}.card-info{padding:15px}h3{font-size:14px;font-weight:500;margin:0 0 6px}.card-info p{font-size:11px;color:#a69e92;margin:0}.actions{display:flex;align-items:center;gap:17px;margin-top:14px;font-size:12px}button{font:inherit;padding:6px 15px;border:1px solid #b49b66;border-radius:5px;background:#d2b476;color:#151313;cursor:pointer}.intro{margin-bottom:20px}footer{color:#888279;font-size:12px;padding:30px 0;line-height:1.6}@media(max-width:1000px){.grid{grid-template-columns:repeat(2,minmax(0,1fr))}}@media(max-width:650px){.grid{grid-template-columns:1fr}header{display:block}header nav{margin-top:20px}header,main{padding:22px}}</style>
<header><div><small>DURGA / VISUAL COLLECTION</small><h1>Golden Glamour</h1><p>Gold, diamonds, and flowing light. Ready to explore.</p></div><nav><a href="#motion">Motion</a><a href="#stills">Stills</a><a href="../shaders/preview/">Live shaders</a><a href="../README.md">Setup</a></nav></header><main>
'''
page += f'<h2 id="motion">Motion / {len(videos)} loops</h2><p class="intro">1248 × 540. Play one to preview; it repeats automatically. Wan clips use eased forward/reverse motion. Procedural clips use continuous periodic displacement. ProRes LT copies are in the local video/prores folder.</p><div class="grid">' + ''.join(cards) + '</div>'
page += f'<h2 id="stills">Stills / {len(stills)} renders</h2><div class="grid">' + ''.join(still_cards) + '</div>'
page += '''<footer>Use these as source material for Resolume. The live shader preview needs the local HTTP server described in the shader guide. This collection page also opens directly from disk.</footer></main>
<script>for(const button of document.querySelectorAll('[data-play]')){const video=document.getElementById(button.dataset.play);button.addEventListener('click',()=>{if(video.paused){for(const other of document.querySelectorAll('video'))if(other!==video)other.pause();video.play().catch(()=>{button.textContent='Use video controls';});}else video.pause();});video.addEventListener('play',()=>button.textContent='Pause');video.addEventListener('pause',()=>button.textContent='Play');}</script></html>'''
(out/'index.html').write_text(page,encoding='utf-8')
print(f'Gallery: {len(videos)} loops and {len(stills)} stills.')
