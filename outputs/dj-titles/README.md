# DJ title layers

**One-line versions:** `dj-edwin-line.png`, `dj-mumbai-rani-line.png` and `dj-dinu-line.png` put "DJ" next to the name on one line (same gold treatment; 1248 x 540 plus `-line-master.png` at 1664 x 720 and editable `-line.svg`). Made from the two-line SVGs by `scripts/make_dj_titles_line.py` with headless Chrome. These are the show versions; the two-line files below are kept.

Use `dj-edwin.png`, `dj-mumbai-rani.png`, or `dj-dinu.png` on a layer above the moving visuals. Each is 1248 x 540 RGBA with a transparent background. Use normal alpha blending; Screen/Add would remove the dark letter outlines that help readability.

The `-master.png` versions are 1664 x 720. SVGs keep the names and gold treatment editable; they reference Arial/Helvetica. The PNGs need no fonts installed. These are designed typography assets, not AI-generated lettering. The preview JPG is flattened for review, not the transparent show layer.

Keep the title stable while the background moves. Start with occasional opacity fades or a subtle scale change in Resolume. Show files are static overlays, not animated clips or compiled Wire patches.

## Colorful animated option

[PrismaticTitle](../../shaders/PrismaticTitle.fs) recolors these transparent titles and adds animated apparent extrusion. See the [Wire setup](../../shaders/README.md#colorful-dj-titles). The effect uses the PNG alpha, so the gold RGB does not constrain its palette.
