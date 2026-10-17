"""Local server for scripts/title_render.html (pre-rendering title shaders).

Serves the repository read-only and accepts raw RGBA frames:
  POST /frame?job=<name>&i=<index>&w=<width>&h=<height>
The body is width*height*4 bytes, bottom-up rows as returned by readPixels.
Frames are written into show-media/DJ/Titles/rendered/_frames/<name>.rgba
at their index, so ffmpeg can read the file as rawvideo (then vflip).

Usage: python3 scripts/title_render_server.py   (port 8766, localhost only)
"""
import re
from functools import partial
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
from urllib.parse import parse_qs, urlparse

ROOT = Path(__file__).resolve().parent.parent
FRAMES = ROOT / "show-media" / "DJ" / "Titles" / "rendered" / "_frames"


class Handler(SimpleHTTPRequestHandler):
    def do_POST(self):
        url = urlparse(self.path)
        query = {k: v[0] for k, v in parse_qs(url.query).items()}
        job = query.get("job", "")
        if url.path != "/frame" or not re.fullmatch(r"[a-z0-9-]+", job):
            self.send_error(400, "bad request")
            return
        size = int(query["w"]) * int(query["h"]) * 4
        body = self.rfile.read(int(self.headers["Content-Length"]))
        if len(body) != size:
            self.send_error(400, "wrong frame size")
            return
        FRAMES.mkdir(parents=True, exist_ok=True)
        path = FRAMES / f"{job}.rgba"
        with open(path, "r+b" if path.exists() else "wb") as f:
            f.seek(int(query["i"]) * size)
            f.write(body)
        self.send_response(204)
        self.end_headers()

    def log_message(self, *args):
        pass


if __name__ == "__main__":
    ThreadingHTTPServer(("127.0.0.1", 8766), partial(Handler, directory=str(ROOT))).serve_forever()
