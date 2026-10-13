# Durga visuals

- Create local visuals for a three-night Durga event. Theme: golden glamour / contemporary bling, gold chains, diamonds, liquid metal, dark contrast. Avoid generic palace decor and visual clutter.
- LED wall: 1248 x 540, aspect 104:45. Generate 1664 x 720 masters and resize to the wall without stretching.
- Generate on Windows RTX 5090 (32 GB); show playback is Resolume on an M1 Max Mac (64 GB). Keep the show independent of this PC.
- Current direction: prioritize editable ISF/GLSL shaders in Resolume Wire, which the user owns. Keep AI stills as textures and completed video loops as optional assets. Do not start more video batches unless requested.
- Start with `shaders/README.md` and `shaders/preview/`. Six shaders have browser validation; native Wire compilation and show-Mac performance still need testing. No compiled Wire patches exist yet. `SHOW-PLAN.md` covers show readiness and the X4 camera test.
- ComfyUI portable: `C:/Users/drishan/code/stablediff/ComfyUI_windows_portable`. Local server: `http://127.0.0.1:8188`.
- Models remain in `D:/AI/models`. Text encoder: `text_encoders/qwen_3_4b.safetensors`; full Z-Image: `diffusion/z_image_bf16.safetensors`; VAE: `vae/ae.safetensors`.
- Start with the native-node workflow and prompts under `outputs/`. Use full Z-Image settings, not Turbo settings. Preserve seeds and PNG workflow metadata.
- Keep setup and documentation concise. Prefer direct local files/API; MCP is optional. Do not add dependencies, automation frameworks, or move model weights without a concrete need.
- Preserve existing workflows and assets. Check the queue before restarting ComfyUI. Never interrupt someone else's render.
- Human time is flexible; overnight GPU work is acceptable. Test a sample before large batches, and distinguish tested results from proposals.
- Prior-year USB dGold was reviewed; the selected originals and silent video copies are in `Durga 2026/Previous years` on the USB, outside Git. Current ads also stay outside the public repository. `MAC-HANDOFF.md` explains the transfer and next session.
- The user operates three to five Resolume layers manually. Prioritize stable Durga imagery with barely visible fire/light/dust accents; keep face/hand distortion off. DJ names are DJ Edwin, DJ Mumbai Rani, and DJ Dinu; transparent titles are in `outputs/dj-titles/`. Camera silhouettes/trails remain a later experiment using X4 or Olympus E-M10 IV.
- Never use em dashes or add agent co-authors to commits. Do not manually edit generated/package files.
