# Durga visuals 2026

Live DJ and Dandiya visuals, built on the M1 Max Mac with Resolume and Wire. Direction after the organizer meeting: energetic abstract motion, beat-driven cuts, color changes, and DJ names that take over the wall. Daytime can use one approved event overview still. Gold is one palette, not a requirement for every look.

LED composition: **1248 x 540**, aspect **104:45**. Confirm the actual computer output signal with Sam before mapping the wall.

## Start on the Mac

1. Clone this repository, or pull the latest version in an existing checkout. Keep show media on the Mac's internal storage.
2. Follow the [Wire + GLSL tutorial](shaders/WIRE-TUTORIAL.md): first source, first effect, exposed controls, beat sync, title takeover, and code changes.
3. Use [the shader catalog](shaders/README.md) for the seven existing effects/sources. Two new [tutorial examples](shaders/examples/) are ready for their first native test.
4. Follow [MAC-HANDOFF.md](MAC-HANDOFF.md) to assemble the live layers and [SHOW-PLAN.md](SHOW-PLAN.md) for the venue rehearsal.

```sh
git clone https://github.com/drishans/durga-visuals.git
cd durga-visuals
```

The retired meeting demo has been removed from the current repository. Existing stills, loops, titles and generation records remain reusable. No Windows generation setup is needed for shader work on the Mac.

## Useful assets

- [DJ title PNGs and editable SVGs](outputs/dj-titles/): DJ Edwin, DJ Mumbai Rani, DJ Dinu. Use them with PrismaticTitle and the takeover tutorial, or animate their scale directly in Resolume.
- [Existing 39 stills and 11 loops](outputs/index.html), [wall PNGs](outputs/stills/wall/), and [loop files](outputs/video/loops/).
- [Ma image review](Ma/index.html), [prompts and workflows](Ma/README.md). Keep a chosen portrait stable when used behind a performance.
- [Prior-year material notes](MATERIAL-NOTES.md) and [wall calibration card](outputs/calibration/wall-check-1248x540.png).
- Generation archive: [setup](outputs/setup-status.md), [original prompts](outputs/golden-glamour-prompts.md), [later still prompts](outputs/meeting-prompts.md), and [motion prompts](outputs/golden-glamour-motion-prompts.json). Model weights remain outside Git.

Keep downloaded playlist footage, ads, and Wire patch bundles in a local `show-media/` directory or another folder outside Git. The repo is for editable shaders, guides, and our existing assets. Sanch's playlist is the style reference; select useful excerpts before building the show.

## What is tested

The seven original shaders passed browser rendering and loop checks previously. Native Wire compilation, alpha blending, beat triggers and show-Mac performance still need testing. The two new tutorial shaders have only static checks; they were not rendered because the PC GPU is reserved. No compiled Wire patches are included. Camera interaction remains a later phase.
