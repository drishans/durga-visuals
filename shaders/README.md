# GLSL into Resolume

These are editable **ISF 2.0 fragment shaders**: GLSL plus a JSON input declaration. They use no external textures except the source image for the two effects.

| File | Role | Good starting point |
| --- | --- | --- |
| [LiquidGold.fs](LiquidGold.fs) | Procedural source | Quiet gold motion behind performers; no image required |
| [GoldenFlow.fs](GoldenFlow.fs) | Texture effect | Make the generated stills flow; try 12-24 pixels of displacement |
| [DiamondKaleidoscope.fs](DiamondKaleidoscope.fs) | Texture effect | DJ visuals or a live camera; start with six facets and slow rotation |

![Live flow effect](../outputs/previews/shader-golden-flow.jpg)

## Free Mac route: ISF Editor + Syphon

1. Install the free [Desktop ISF Editor](https://isf.vidvox.net/desktop-editor/) on the show Mac and open one of the `.fs` files.
2. Set the output to **1248 x 540**. For GoldenFlow or DiamondKaleidoscope, choose a still, movie, or compatible camera as the input. LiquidGold generates its own image.
3. Publish the editor's output through Syphon. In Resolume, open **Sources**, locate the editor's Syphon stream, and drag it into a clip slot.
4. Keep both applications running on that Mac. Adjust shader parameters in ISF Editor; use Resolume for mixing and additional effects.

The editor supports shader editing, media/camera inputs, and Syphon output. Syphon passes textures between applications on the **same Mac**; it does not send the Windows PC's GPU output to the venue. See [ISF Editor features](https://isf.vidvox.net/desktop-editor/) and [Resolume Syphon instructions](https://www.resolume.com/support/syphonspout). This route still needs a test on the actual Mac and Resolume installation.

## Native effect route: Resolume Wire

For an effect, connect **Texture In -> ISF -> Texture Out**. In the ISF node's **Fragment Shader** attribute, browse to GoldenFlow or DiamondKaleidoscope. For LiquidGold, use **ISF -> Texture Out** and no Texture In. The `.fs` file is shader source, not a compiled Resolume plug-in. [ISF loading guide](https://resolume.com/support/en/isf)

Set the patch category to **Effect** or **Source**, respectively. Add Float In / Bool In nodes to the shader parameters you want exposed in Resolume. Save the patch; it then appears in the appropriate Resolume category. Follow the incoming texture's resolution for effects. [Wire integration](https://resolume.com/support/en/wire-resolume-integration)

For portable handoff, consolidate the resources and compile an editable `.wired` patch using a suitably licensed Wire installation. A compiled patch can run in compatible Avenue/Arena without the recipient owning Wire. Check the patch's minimum host version. This repository contains shader sources, not precompiled Wire patches. [Saving and compiling](https://resolume.com/support/en/wire-saving-consolidating) · [Compatibility](https://resolume.com/support/en/patch-compatibility)

## Timing, audio, and camera

- Auto animate uses `TIME / loopSeconds`. Temporal frequencies are integer multiples of one cycle; phase 0 and 1 match exactly for a static input. An independently moving camera/video will not itself become a seamless loop.
- For beat control, switch Auto animate off and drive Phase from 0 to 1 in the host. Keep the cycle duration fixed during a take to avoid jumping the clock's phase.
- For audio response, modulate Flow or Brightness gently. Wire's Spectrum input and Falloff can provide a smoothed envelope. Avoid mapping noisy FFT directly to Phase. [Wire FFT inputs](https://resolume.com/support/en/wire-resolume-integration)
- The texture effects can process a camera feed, but capture hardware, latency, silhouettes, and trails are not configured by these shaders. Neither effect uses a persistent feedback buffer.

## Preview and validation

From the repository root, run `python -m http.server 8970 --bind 127.0.0.1`, then open [the preview](http://127.0.0.1:8970/shaders/preview/). It uses the same `.fs` sources with a WebGL 2 wrapper and runs locally. If using ComfyUI portable Python, add `-s` before `-m`.

All three shaders compiled and rendered in the browser preview; each passed the **Check cycle** comparison with a maximum endpoint difference of **0/255** at default controls. This verifies shader math and WebGL rendering, not native Wire compilation or show-Mac performance. Wire/Syphon playback remains to be checked on the Mac. Refresh the page after editing a shader file.
