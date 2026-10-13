# Setup and verification

- Verified October 12-13, 2026 on RTX 5090: ComfyUI 0.39.0, PyTorch 2.14.1+cu130. Both Z-Image and Wan model loaders see the external D-drive weights.
- 15 new Z-Image concepts rendered, with 1664 x 720 masters and 1248 x 540 wall copies. PNGs preserve prompts and workflow metadata. The earlier P01 test remains in `previews/`.
- Completed motion set: eight Wan clips and three `FX_` displacement loops. All 11 decode correctly at 1248 x 540, 30 fps, 240 frames / 8 seconds. Frame-change measurements are in `video/verification.json`. These checks do not certify every interpolated frame as artifact-free.
- The October 13 batch finished. No further video batches are planned; the user prefers editable GLSL in Wire for the main motion workflow.
- The first Wan generation took about 12 minutes 34 seconds. An export-parameter issue was fixed and its cached frames recovered. This is one measured run, not a general performance claim.
- The initial dissolve loop showed ghosting and was replaced with eased forward/reverse playback. Optical-flow interpolation and temporal reversal can still produce artifacts; review the finished loops at show size.
- All three ISF shaders compiled, rendered, and matched phase 0/1 exactly in the WebGL preview. Native Wire compilation, Syphon input, and show-Mac performance are not yet verified.
- UI and API workflow files are included. If ComfyUI does not list a newly copied input, use **Edit > Refresh Node Definitions**. Use the node's upload picker if a stale missing-media warning remains.
- No custom nodes, MCP server, model weights, raw videos, or ProRes masters are committed to Git. ProRes masters remain local under `video/prores/`.
