# Mac handoff

Copy the entire **Durga 2026** folder from dGold to the Mac's internal storage before building the show. Open the copied **Project** folder in the next chat. The USB holds a complete project snapshot, the meeting ZIP, selected prior-year media, and a place for current ads. Git is optional for playback; clone the public repository separately if you want to edit and sync shader sources. Keep ads and prior-year media outside the public repository.

## Start here

1. Read `START-HERE.txt` at the top of the copied USB folder and `Previous years/REUSE-NOTES.md` for the actual media shortlist and caveats.
2. Open `Project/outputs/demo.html` for the meeting. The 90-second overview is beside it. `Project/outputs/dj-titles/` contains transparent gold title PNGs for DJ Edwin, DJ Mumbai Rani, and DJ Dinu, plus editable SVGs and 1664 x 720 masters.
3. In Resolume, create a 1248 x 540 composition. Confirm the installed edition and version before configuring output. The processor's input raster and refresh rate still need Sam's confirmation.
4. Review `Project/Ma/index.html` for the new Durga studies, exact prompts, and close-review notes before choosing a still. The earlier USB image shortlist is superseded by the user's review: 03 is rejected, 09/12 have defects, 14 is the best old reference, 16 needs eye correction, and 20 is secondary. Start with the chosen stable still and one subtle accent. Then try the seven `.fs` shaders using [the Wire walkthrough](shaders/README.md). They have browser validation only; no compiled Wire patches exist yet.
5. Try `shaders/PrismaticTitle.fs` on the DJ PNGs for colorful animated chrome and apparent depth. Its Wire setup is in `shaders/README.md`. Read `MATERIAL-NOTES.md` for more existing USB footage. The latest Ma images and shader are in Project, separate from the older meeting ZIP.
6. Rehearse three to five layers on the Mac, save the composition beside the copied media, and reopen it with the USB removed. Do the camera test after this works.

## Suggested layer order, bottom to top

| Layer | Performance | DJ |
| --- | --- | --- |
| 1 | Quiet gold backing or fallback | Gold loop or Wire source |
| 2 | Stable Durga image chosen after close review | Alternate texture or camera, initially off |
| 3 | Very faint fire/light near the bottom, if available | Gold dust or geometric accent |
| 4 | Sparse gold dust at the edges | Transparent DJ title PNG |
| 5 | Announcement/ad override, normally off | Announcement/ad override, normally off |

Start accents around 5-12% opacity and adjust by eye on the actual wall. Use Screen/Add for bright-on-black accent footage; use normal alpha blending for the transparent title PNGs. Keep deity faces and hands stable, and restrict displacement to background/accent layers. Ads should be shown cleanly, with decorative overlays cleared. Audio and camera input remain separate from the visuals until tested.

The prior-year 1152 x 544 Durga stills need proportional fitting. At full wall height they occupy about 1144 x 540, leaving roughly 52 pixels per side for the backing. Avoid stretching or cropping crowns to fill the width. Most old loops are 16:9; choose their crop deliberately.

## Context for the next chat

The user knows GLSL and normally operates three to five Resolume layers by hand. Last year's successful performance look was a stable Durga still with barely visible fire/effects. Keep that approach. The Mac is M1 Max / 64 GB and has Wire; it is the only show computer. Windows RTX 5090 / 32 GB is for generation. No new framework or video-model batch is needed. X4 camera input, trail effects, native Wire compilation, the exact laptop-to-wall connection, and performance under the intended layer count remain untested. See [SHOW-PLAN.md](SHOW-PLAN.md).

Sanch handles the screen/event coordination, Sam supplies the panels, and Venu is the photographer. The meeting moved a day later; do not assume a calendar date. The user is downloading current ads into the USB folder. No one has been contacted on their behalf.
