# Show plan and camera experiment

Current direction: manually operate three to five layers, keeping prior-year Durga images stable with barely visible fire/light/dust accents. Use editable Wire shaders for motion, AI stills as textures, and the 11 completed loops as optional playback material. [MAC-HANDOFF.md](MAC-HANDOFF.md) gives the starting layer order and USB transfer instructions. The show runs entirely on the M1 Max Mac. The PC is for preparation.

## Before depending on live effects

| Priority | Missing decision or test | Done when |
| --- | --- | --- |
| 1 | Build one real Wire Source and one Effect on the Mac | LiquidGold and GoldenFlow run inside the installed Resolume, with exposed controls and no missing resources |
| 1 | Confirm LED processor input with the wall operator | Accepted HDMI resolution/refresh rate, active image rectangle, scaling, and connection path are written down; the 1248 x 540 composition reaches the wall without stretching or clipping |
| 1 | Make a minimal show composition | Daytime, performance, dance/DJ, camera, and fallback looks are immediately accessible |
| 1 | Rehearse the Mac output and recovery | A still, a loop, and a shader play on an external screen; camera can be removed without disrupting the fallback; composition reopens with all media offline |
| 2 | Confirm music control | Manual tempo is usable; any audio-reactive controls have an actual tested audio input and restrained ranges |
| 2 | Choose from the reviewed USB shortlist and confirm running order | Existing Durga/Sindoor imagery is available on the Mac, and the performer/DJ handoff is agreed |

The wall's pixel count does not establish the processor's accepted computer signal. It might accept a larger standard raster and use a mapped rectangle. Ask the operator rather than assuming the Mac should output a custom 1248 x 540 display mode. Save the tested output preset separately from the composition. [Resolume output mapping](https://resolume.com/support/en/advanced-output)

Use the [1248 x 540 wall-check card](outputs/calibration/wall-check-1248x540.png) to check all four borders, a round circle, and grayscale steps. Its 13 x 5 grid follows the supplied layout; it is not a processor configuration or measured color calibration.

## Minimal show layout

- Base layer: quiet still, slow loop, or procedural source. Keep a familiar fallback clip ready.
- Accent layer: gold dust, fringe, or ribbons, with deliberate blend mode and modest opacity.
- Camera layer: disabled by default, brought in for suitable moments. Its failure should leave the base look usable.
- Separate looks for welcome/daytime, performances, dance/DJ, and camera play. Friday/Saturday/Sunday assignments remain provisional until the running order is known.
- Map a small set of controls: next look, camera opacity, effect intensity, tempo/phase, clear trails, and return to fallback. Avoid adjusting shader code during a performance.
- Keep a copy of the project and selected media on the Mac's internal storage, plus a USB backup. Export favorite shader looks as video after native testing.

Before camera work, test the composition for 30 minutes with the intended layer count and external output. Check frame rate, reconnect behavior, brightness, text legibility, and media paths. Shader browser previews do not certify Mac show performance.

## First camera test: X4 + Mac

The X4 and Mac are available, not yet connected. The user knows OBS. A cameraman has used a capture adapter for crowd feeds in previous years; its model and availability are unknown. No purchase is needed for the first test.

1. Use a USB data cable to connect the powered X4 to the Mac. Select **Webcam Mode**. If absent, check firmware against Insta360's current instructions before proceeding.
2. In OBS, add **Video Capture Device > Insta360 X4** and confirm a stable picture. Start at 30 fps. Choose a normal framed view if the device exposes one; 360 output is an optional later choice. Insta360 documents 2880 x 1440 at 30 fps for a 2:1 panorama, which needs appropriate framing/reprojection rather than stretching across the LED wall.
3. Close or deactivate OBS's physical camera capture, then check for the X4 under Resolume **Sources**. Drag the input into a clip and verify it. Direct support is a test, not a promise: Insta360 explicitly states that other software beyond OBS has not been tested.
4. If direct capture fails but OBS works, let OBS own the X4, start **OBS Virtual Camera**, and test that virtual input in Resolume. Use a fixed OBS scene/source output so operator scene changes do not accidentally change the feed. This is an unverified fallback until tested on this Mac; it can add latency.
5. Frame one person with enough separation from the background. Apply the existing DiamondKaleidoscope effect first. Prove that movement changes the output before building masks and trails.
6. After the picture is stable, test a 30-minute run and a disconnect/reconnect. Keep the camera source's audio out of the visual experiment.

Sources: [X4 webcam tutorial](https://onlinemanual.insta360.com/x4/en-us/camera/appuse/obs), [X4 firmware/live-stream FAQ](https://onlinemanual.insta360.com/x4/en-us/faq/feature/livestream), [Resolume capture inputs and exclusive device access](https://resolume.com/support/en/live-inputs), [OBS Virtual Camera](https://obsproject.com/kb/virtual-camera-guide).

## Camera looks, in order

| Stage | Visual | What it proves |
| --- | --- | --- |
| A | Live camera through kaleidoscope, restrained gold tint | Capture, framing, routing, and latency |
| B | Contrast/edge extraction, then color mapping | Recognizable people and movement under venue-like lighting |
| C | Add controlled previous-frame feedback with decay and a clear/reset control | Trails remain stable, can be cleared, and do not build to white |
| D | Optional cleaner person mask | Only add segmentation if a simple lighting/contrast treatment is inadequate |

Conceptual trail pipeline: camera -> frame/crop -> contrast or mask -> color -> blend with decaying previous frame -> output. The current shaders are single-frame effects and do not yet implement trails or person segmentation. Use frame-rate-aware decay, for example retention = exp(-deltaTime / lifetime), when implementing feedback.

A stage-mounted wide crowd view should work well as an abstract moving texture. Large individual silhouettes are easier with a framed participation area near the camera. A brightness threshold does not distinguish people from lights; camera motion and changing stage lights can also overwhelm simple frame differencing. Avoid pointing the camera at the LED wall during the first test, where unintended optical feedback would obscure the result.

## Venue details to resolve

- Camera-to-Mac distance, mounting point, lighting, power, and cable route.
- Whether the cameraman can supply a spare clean feed while keeping his existing duties; exact adapter, output format, and Apple Silicon compatibility.
- Exact Resolume edition (Avenue or Arena), Resolume/Wire versions, and the laptop-to-processor adapter/cable chain.
- Whether camera interaction is a short DJ feature or a separate participation station, and who operates it.

The Olympus remains a later alternative. Do not assume its HDMI connector provides a clean continuous live feed, or that it supports USB webcam output. Verify those behaviors and power/sleep behavior on the exact E-M10 Mark IV before choosing or buying capture hardware.
