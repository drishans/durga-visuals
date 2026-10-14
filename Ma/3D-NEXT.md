# Actual 3D, later

ComfyUI can generate meshes as well as images and video. It orchestrates whichever model and nodes you load. Tonight's sculpture looks are still 2D Qwen images; no 3D generation was attempted.

Two candidates worth a later test:

- **TRELLIS.2** generates shape and PBR materials from an image. Comfy now documents a [native image-to-model workflow](https://docs.comfy.org/tutorials/3d/trellis2), including preview and mesh export. The [original Microsoft implementation](https://github.com/microsoft/TRELLIS.2) specifies at least 24 GB VRAM and was tested on Linux. That is not a verified requirement or compatibility guarantee for this Windows Comfy integration. Your 32 GB 5090 is a plausible test machine; check the installed nodes before downloading anything.
- **Hunyuan3D 2.1** separates geometry and PBR texturing. Its [official repository](https://github.com/Tencent-Hunyuan/Hunyuan3D-2.1) reports 10 GB for shape, 21 GB for texture and 29 GB for both. It links a community Comfy integration. Comfy's separate [native Hunyuan3D-2 guide](https://docs.comfy.org/tutorials/3d/hunyuan3D-2) supports geometry and GLB export, but explicitly says its illustrated native path does not include texture generation. Do not confuse that workflow with the 2.1 community pipeline.

Suggested first experiment: choose one clean, isolated Durga bust image, generate a mesh, then inspect and repair it in Blender. Assign bronze, obsidian or pearl materials and animate an area light around the stationary sculpture over 8-12 seconds. A periodic lighting orbit gives motion while keeping the face legible. Render 1248 x 540, frames 0 through N-1 of one cycle, and bring the video into Resolume.

For a full ten-arm figure, plan separate modeled weapons and manual shoulder/hand cleanup. A single reference does not show the back or hidden connections. I would start with a bust to test the pipeline, then decide whether a sculptor-made full figure is the better foundation. This is a proposal, not tested output.
