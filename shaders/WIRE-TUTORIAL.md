# Mac workshop: GLSL to a live Resolume show

Build one source and one title effect, then combine them with the playlist footage. Aim for reusable motion you can cue live: radial bursts, kaleidoscopes, color changes and huge DJ-name entrances. Basic scale and opacity animation can stay in Resolume. Use Wire for a custom visual or one control coordinating several changes.

**Status:** the two `examples/` files have static checks only. GoldTitleTakeover and the older seven shaders have browser validation, not native Wire validation. Compile and test on the M1 Max.

## 1. The pieces

| Piece | Job |
| --- | --- |
| `.fs` file | GLSL fragment shader plus an ISF JSON input declaration |
| Wire patch | Connects textures, shader nodes and exposed controls |
| Resolume clip | Runs a saved patch or footage and controls timing |
| Resolume layer | Blends a clip over other parts of the show |

ISF supplies `TIME`, `RENDERSIZE`, `isf_FragNormCoord` and image-sampling helpers. You do not need to build an OpenGL application. [ISF basics](https://docs.isf.video/quickstart.html)

## 2. First source: radial rave

1. Create a Wire patch. Set category **Source** in the Patch panel and use a 1248 x 540 working resolution.
2. Add an **ISF** node. Under Attributes > Fragment Shader, browse to [examples/RadialRave.fs](examples/RadialRave.fs).
3. Connect ISF to **Texture Out**. Open **View > Layout > Shader Editing** for the code editor. [Loading shaders](https://resolume.com/support/en/isf)
4. Add **Float In** nodes for `phase`, `energy`, `hue`, `spokes`, `hit` and `brightness`. Connect each to its matching ISF inlet and copy defaults/ranges from the header. Keep White hit at zero.
5. Scrub Phase from 0 to 1. The source has no automatic clock. Save as `Durga Radial Rave`; find the saved patch under Resolume Sources and put it in a clip.

```text
Float In: Phase ------> ISF: RadialRave ------> Texture Out
Float In: Energy ----->       ^
Float In: Color --------------+
```

The JSON header creates shader inlets. Wire **Input nodes** expose controls to Resolume. Arrange them in the Dashboard. Set texture nodes to follow Patch resolution in the host. Save with the correct Source or Effect category. [Host integration](https://resolume.com/support/en/wire-resolume-integration)

## 3. Beat control and deliberate hits

In Resolume, use the animation menu beside Phase, select **BPM Sync**, set the range to 0-1 and start with an eight-beat loop. Tap the musical tempo. Try four beats for faster motion or sixteen for a longer phrase. Change patterns or trigger a title on a drop.

For one-shot entrances, use Timeline or BPM Sync in one-shot mode and restart on clip trigger. Disable BPM Phase Lock in the start settings if you want a triggered entrance to start at zero instead of joining the current master phase. [Parameter animation](https://resolume.com/support/parameter-animation)

Keep White hit on a separate manual control, normally zero; briefly raising and returning it gives a deliberate bright accent. Energy changes the pattern without changing its clock. Later, use Spectrum input -> Falloff/smoothing -> a bounded Energy range for audio response. This needs a tested audio input. Keep noisy audio away from Phase.

## 4. DJ name takeover: GoldTitleTakeover

[GoldTitleTakeover.fs](GoldTitleTakeover.fs) merges PrismaticTitle and TitleTakeover into one Bling / Golden Glamour effect for the transparent title PNGs in `show-media/DJ/Titles/` (`dj-edwin.png`, `dj-mumbai-rani.png`, `dj-dinu.png`). The PNG's bright face becomes animated metallic gold with a light sweep, edge glints and a soft bloom. The PNG's dark outline, extrusion and shadow are kept, so the name reads over busy tunnels.

**Status:** browser validated on October 16, 2026 (compiles; all three names render; Intro, Pulse and size checked by pixel measurement over black and busy stills). Native Wire compilation, TIME behavior in Wire and Mac performance still need testing. About 75 texture reads per pixel.

### Build it in Wire 7.28.1

1. New patch. In the Patch panel set category **Effect** and working resolution 1248 x 540.
2. Add **Texture In**, an **ISF** node and **Texture Out**. In the ISF node's Attributes > Fragment Shader, browse to `shaders/GoldTitleTakeover.fs`.
3. Connect `Texture In -> ISF inputImage`, then `ISF -> Texture Out`.
4. Add one **Float In** per control and connect it to the matching ISF inlet. Copy names, defaults and ranges from the table. Arrange them in the Dashboard.
5. Save as `Durga Gold Title`. In Avenue, put a title PNG in a clip on the Titles layer (normal blend), then drag `Durga Gold Title` from Effects onto that clip.

```text
Texture In -> ISF: GoldTitleTakeover -> Texture Out
                 ^ Intro, Pulse, Pulse strength, Speed, Glow, Sparkle, Readable size, Opacity (Float In)
```

| Control | Inlet | Default | Range | Avenue mapping |
| --- | --- | --- | --- | --- |
| Intro | `intro` | 1.0 | 0-1 | BPM Sync **one-shot**, 4 beats, restart on clip trigger. 0 hidden, ~0.1 slams in oversized with glitch, settles by 0.55, clean hold from 0.7 |
| Pulse (beat phase) | `pulse` | 1.0 | 0-1 | BPM Sync **loop**, 1 beat. The hit is at 0 and decays; 1 means no hit |
| Pulse strength | `punch` | 0.6 | 0-1 | Manual; raise on drops |
| Speed | `speed` | 1.0 | 0-3 | Manual, or BPM Sync between 0.5 and 2 over 8 beats. Drives sweep, shimmer and glints |
| Glow | `glow` | 0.8 | 0-2 | Manual, or BPM Sync 0.6-1.4 over 2 beats |
| Sparkle | `sparkle` | 0.6 | 0-1 | Manual |
| Readable size | `size` | 1.0 | 0.5-2.5 | Per DJ, see below |
| Opacity | `opacity` | 1.0 | 0-1 | Fade out, or use the layer opacity |

Running Intro from 1 back to 0 plays the entrance in reverse as a glitch-out.

**Readable size per DJ** (measured at 1248 x 540, including glow): Edwin 1.85 (84% of wall width), Mumbai Rani 1.02 (85%), Dinu 2.0 (73%; above about 2.1 the small "DJ" line above the name crops at the top). Make one clip per DJ with its size saved in the clip.

### Check in Wire before the show

- The sweep and glints move by themselves. If they freeze in Wire, TIME is not reaching the ISF node; set Speed to 0 and animate Glow instead, then report it so the shader can take a phase input.
- Scrub Intro and Pulse manually first, then turn on BPM Sync. Disable BPM Phase Lock for Intro so every trigger starts at 0.
- Check all three names over dark footage and over the busiest tunnel. Watch for frame drops with three to five layers running.

The older two-node chain (`PrismaticTitle -> TitleTakeover`) still works for a colorful chrome look; both files are unchanged.

## 5. Things to change in the code

These excerpts come from the complete examples. No extra nodes are needed.

**Coordinates:** aspect correction makes radial geometry round on the wide wall.

```glsl
vec2 p = (isf_FragNormCoord - 0.5)
       * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
float radius = length(p);
```

**Motion:** `24.0` controls band density; `2.0` controls cycles of travel. Integer time multipliers preserve loop closure. Spokes controls ray count.

```glsl
float t = 6.28318530718 * fract(phase);
float rings = 0.5 + 0.5 * cos(24.0 * radius - 2.0 * t);
```

**Palette:** the three offsets create RGB phase shifts. The exposed hue moves the palette. For Dandiya, try replacing the spectrum with a coordinated pair of colors.

```glsl
vec3 tint = 0.5 + 0.5 * cos(6.28318530718 *
    (hue + 0.12 * radius + vec3(0.0, 0.33, 0.67)));
// Alternative palette: gold to magenta.
tint = mix(vec3(1.0, 0.55, 0.06), vec3(0.7, 0.04, 0.45), rings);
```

**Title timing:** `0.18` ends the entrance; `0.72` starts the exit. `smoothstep` eases the movement. Dividing UVs by scale enlarges the sampled image.

```glsl
float enter = smoothstep(0.0, 0.18, u);
float leave = smoothstep(0.72, 1.0, u);
float scale = mix(0.45, holdScale, enter) * mix(1.0, exitScale, leave);
vec2 uv = (isf_FragNormCoord - 0.5) / max(scale, 0.001) + 0.5;
```

Preserve the transparent-border check in the full file so zoom-out does not smear edge pixels. These titles use straight alpha: fade alpha without multiplying RGB by alpha again. Use `fract(phase)` for a repeating source and `clamp(progress,0.0,1.0)` for a one-shot.

## 6. Assemble a small live set

| Layer, bottom to top | Use |
| --- | --- |
| 1 | Selected playlist footage or RadialRave |
| 2 | Alternate pattern, kaleidoscope or short accent |
| 3 | DJ title takeover, normally hidden |
| 4 | Optional crowd feed after capture is tested |
| 5 | Clean event overview/ad override |

For Dandiya, try rhythmic radial/mirrored patterns and coordinated warm colors. For DJ sets, use larger zooms, sharper cuts and color changes. Keep a calm fallback and quick blackout control. Mute imported video audio unless intentionally needed. Map next background, each DJ name, Energy and return to overview.

Keep footage, patches and the composition in local `show-media/` or another media folder outside Git. Keep `.fs` sources in the repo. After editing in Wire, save the same shader revision back to the repo. Do not assume external file edits automatically refresh a loaded Wire resource; reload and confirm.

## 7. Save and rehearse

Wire normally saves patches in `~/Documents/Resolume Wire/Patches/`. Consolidate a finished patch with its resources, optionally compile an editable `.wired` bundle, and export a video backup through **Patch > Export as Video**. Keep show media on the internal SSD. [Saving and exporting](https://resolume.com/support/en/wire-saving-consolidating)

| Symptom | First check |
| --- | --- |
| Patch missing in Resolume | Saved patch and Source/Effect category |
| Slider missing | Float In/Bool In connection and Dashboard |
| Static pattern | Phase animation; this source has no TIME clock |
| Invisible title | Progress 0.4, opacity 1, real-alpha PNG, connected Texture In |
| Entrance starts halfway through | Clip-trigger restart and BPM Phase Lock |
| Jagged or slow output | Reduce pattern density, then isolate costly effects/layers |

On the Mac, check compilation, all three names, alpha over dark and bright footage, loop joins and retriggers. Run the intended layers on an external display for 30 minutes. Confirm the computer signal and wall mapping with Sam separately from the 1248 x 540 composition. Camera trails come after this playback path works.
