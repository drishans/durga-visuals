"""Small local ComfyUI client shared by this project's render scripts."""
import json
import time
import urllib.error
import urllib.request


def request(route, data=None):
    body = None if data is None else json.dumps(data).encode('utf-8')
    req = urllib.request.Request('http://127.0.0.1:8188' + route, data=body,
                                 headers={'Content-Type': 'application/json'})
    try:
        with urllib.request.urlopen(req, timeout=60) as response:
            return json.load(response)
    except urllib.error.HTTPError as error:
        raise RuntimeError(error.read().decode('utf-8')) from error


def render(graph, workflow, timeout=3600):
    queue = request('/queue')
    if queue['queue_running'] or queue['queue_pending']:
        raise RuntimeError('ComfyUI queue is occupied. Wait before starting this batch.')
    started = time.monotonic()
    result = request('/prompt', {'prompt': graph, 'extra_data': {'extra_pnginfo': {'workflow': workflow}}})
    if result.get('node_errors'):
        raise RuntimeError(str(result['node_errors']))
    prompt_id = result['prompt_id']
    print('queued', prompt_id, flush=True)
    while time.monotonic() - started < timeout:
        history = request('/history/' + prompt_id)
        if prompt_id in history:
            item = history[prompt_id]
            if item['status']['status_str'] != 'success':
                raise RuntimeError(json.dumps(item['status']))
            return item, round(time.monotonic() - started, 2)
        time.sleep(3)
    raise TimeoutError(f'Render {prompt_id} has not completed; inspect the queue before retrying.')
