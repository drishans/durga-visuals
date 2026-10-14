# Ma Durga studies

Open `index.html` to compare the images, read the review notes, and open their exact prompts. The shortlist is a recommendation for discussion, not approval for the event. Failed studies remain available for comparison. Every PNG includes its ComfyUI workflow.

## Creative batch

M08-M15 explore obsidian, prismatic glass, liquid chrome, woodcut, architectural relief, porcelain, cosmic ink and monumental bronze. These are Qwen-generated **2D images**, including several that look like 3D renders. They intentionally use busts or portraits; they do not depict a complete ten-arm scene. The review notes distinguish good visual experiments from traditional depictions. Keep face distortion off in Resolume.

My first creative picks are M10 (chrome), M11 (woodcut), M13 (porcelain) and M14 (cosmic ink). Open `qwen-creative.json` for the exact M14 workflow, or drag any master PNG into ComfyUI. These artistic studies often render a symbolic forehead motif rather than a distinct eye; that is called out in the reviews.

E04 is the latest traditional full-figure candidate, after three small masked corrections. Every pixel outside each edit mask was checked against its source and is unchanged in the master. It remains an artistic interpretation for human review.

## Adjust a prompt

Drag an image or `qwen-image.json` into ComfyUI. Edit the positive prompt, keep the seed fixed, and run once. Then change the seed to explore another composition. `prompts.json` keeps the reusable concepts; each image's `render.json` preserves what actually ran, including its negative prompt and settings. Do not overwrite an existing candidate when comparing edits; choose another output name or seed.

- Qwen-Image-2512: FP8 model, its Qwen 2.5 VL encoder and Qwen VAE; 50 steps, CFG 4, Euler/simple, shift 3.1. Native nodes, no acceleration LoRA.
- Z-Image comparison: full BF16 model, Qwen 3 encoder, `ae.safetensors`; 40 steps, CFG 4, res_multistep/simple, shift 3.
- Targeted correction: `qwen-edit.json`, Qwen-Image-Edit-2511 FP8 mixed, the same Qwen 2.5 VL encoder/VAE, 40 steps, CFG 4, Euler/simple, shift 3.1. Choose the source master in Load Image and edit the instruction. Each edit's `render.json` identifies its source. Changes are generated and must be checked for unintended differences.
- Both: 1664 x 720 master and 1248 x 540 wall copy. No stretching or crop. Models stay on the Windows D drive; the Mac only needs the images.

The graph is model + text encoder + VAE -> positive/negative conditioning -> sampler with an empty latent -> VAE decode -> master save and wall resize/save. The two model families need their matching components, not just a model-name swap.

For small repairs, prefer `qwen-detail-eyes.json` , `qwen-detail-hands.json` or `qwen-detail-bell.json`: crop context -> enlarge -> Qwen edit -> shrink -> composite through a feathered mask over the original. These crop and mask coordinates are for the included M06 reference, not arbitrary images. Choose the source recorded with the edit; adjust both the crop and mask when changing composition. The master preserves pixels outside the mask exactly; the wall copy is then resized. The broader `qwen-edit.json` experiments changed too much and are retained as studies.

## Depiction and review

These are peaceful devotional portraits with a lion, not complete Mahishasuramardini battle scenes. The ten-arm brief follows the familiar Bengali Puja form described by [Belur Math](https://belurmath.org/durga-puja-at-belur-math/). Other legitimate forms exist, including the [Met's eight-armed Durga](https://www.metmuseum.org/art/collection/search/719420).

The main brief's selected objects are trident, sword, discus, conch, bow, arrow, bell, axe, shield and noose. The M06 alternate brief uses eight hands with a lotus instead of some of those objects; the E02-E04 masked corrections preserve ten and add a distinct forehead eye, shield, noose and clearer bell grip. The traditional gifts, including the lotus, appear in [Devi Mahatmya, chapter 2, verses 20-30](https://devimahatmya.com/book/chapter-2-the-slaughter-of-mahisasuras-armies/). Their arrangement here is an artistic choice, not a claim of one universal hand assignment. A prompt naming them does not prove an image depicts them correctly. M07 is deliberately a close portrait rather than a full-body illustration.

Review the two natural eyes together, the separate forehead eye, every shoulder-to-hand connection, the arm count, recognizable held objects, and the lion's eyes and paws. Also check the crown margin and the face at actual wall size. Keep Resolume motion around the base and edges, with the figure stable. Sanch can choose the preferred look and help confirm the depiction for this event.

The old PNG metadata identified SDXL base 1.0 and, for 14/16, a ControlNet reference. The user's review supersedes the earlier USB shortlist: 03 rejected; 09 and 12 have anatomy/eye defects; 14 is the strongest old reference; 16 needs matching eyes; 20 is secondary. Originals remain untouched.

Initial prompt drafts were delegated to Claude Opus 5.5. Local GPU generation and visual selection are separate from that prompt assistance. See the [official Qwen workflow](https://docs.comfy.org/tutorials/image/qwen/qwen-image-2512) for model downloads and baseline settings.

For actual mesh generation later, see [the short 3D plan](3D-NEXT.md). No 3D models or tools were installed for this batch.
