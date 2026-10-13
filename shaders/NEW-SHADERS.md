# Additional procedural sources

Status: **browser validated on October 13, 2026.** All three compile and render at 1248 x 540 in the preview. Native Wire and Mac performance remain untested. Cost notes are estimates from the code, not measurements. These are ISF 2.0 **Generator** sources with no image input, using only float and bool inputs and the shared `phase`, `loopSeconds`, `animate` timing.

Motion uses whole-number harmonics of the cycle or a whole-period shift of a wrapping field. Loop closure assumes fixed parameters. At defaults, all three have identical phase 0/1 frames. The preview also samples the last-to-first transition at 30 fps and four ordinary adjacent-frame pairs; these are useful diagnostics, not exhaustive proof of artifact-free motion.

| Shader | Wrap mean RGB change / 255 | Sampled ordinary changes / 255 |
| --- | --- | --- |
| GoldDust | 0.03 | 0.03-0.03 |
| GoldRibbons | 0.35 | 0.36-0.39 |
| PrismField | 1.85 | 1.14-1.85 |

## GoldDust.fs

**Concept.** Sparse gold glitter falling slowly in three depth layers: fine far specks, mid flecks with brief four-point star glints, and a few large, soft near bokeh discs. An elliptical open center keeps the middle dark for performers, with a faint warm haze outside it.

| Look | density | sparkle | bokeh | clearCenter | exposure | loopSeconds |
| --- | --- | --- | --- | --- | --- | --- |
| Default backdrop | 0.18 | 0.7 | 0.5 | 0.4 | 1.0 | 40 |
| Very calm, wide opening | 0.12 | 0.5 | 0.3 | 0.6 | 0.9 | 60 |
| Busier DJ glitter | 0.35 | 1.2 | 0.8 | 0.1 | 1.1 | 20 |

**Cost.** Low. Three layers times a 2x2 cell search gives at most 12 fleck evaluations per pixel; empty cells exit early. Bokeh 0 skips the near layer.

**Limitations.**
- Fall speed is tied to the cycle: each layer moves about 1.1 to 1.4 wall heights per cycle. Lengthen `loopSeconds` to slow it. A free speed control would break the loop.
- Direction is fixed (downward, with a gentle sideways breeze).
- Fleck sizes are authored for 540 px height and scale with resolution; the minimum width is about one output pixel.
- Glints are small and staggered, not full-frame flashes, but high `sparkle` with short cycles twinkles faster.

## GoldRibbons.fs

**Concept.** Up to six discrete polished gold ribbons that twist along their length, showing a mirror front and a satin back on alternate half-turns, with edge-on pinches reading as fine bright lines. At the wall's sides they flow near the midline; toward the middle they lift up and down, framing an adjustable open center. Drop shadows and a faint glow separate the layers. Distinct from LiquidGold, which is a continuous folded height field.

| Look | ribbons | ribbonWidth | twist | flow | clearCenter | exposure | loopSeconds |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Default frame | 4 | 1.0 | 1.0 | 1.0 | 0.7 | 1.0 | 20 |
| Wide open, delicate | 6 | 0.7 | 0.8 | 0.6 | 1.0 | 0.9 | 30 |
| Bold DJ | 3 | 1.8 | 2.0 | 1.5 | 0.2 | 1.1 | 12 |

**Cost.** Moderate and linear in `ribbons`: roughly 25 transcendental calls per ribbon (three centerline samples for the slope, twist, width, shading). Six ribbons is the most expensive setting.

**Limitations.**
- A 2D shading model, not geometry: fixed back-to-front order, so ribbons overlap but never weave or intersect.
- Thickness uses a slope-based perpendicular-distance approximation, so very high `flow` combined with a strong open-center bend may thicken slightly at steep sections.
- With `twist` 0, each ribbon still rolls as a whole once per cycle (the roll is what keeps the loop exact).
- Changing `ribbons` adds or removes a ribbon abruptly.

## PrismField.fs

**Concept.** A slow field of large flat crystal shards (animated Voronoi cells). Each facet tilts and precesses, reflecting a dark jewelry light box with two strip lights, so highlights travel across the wall. Dark grooves and fine bevel lines define the edges. Gold tint blends from icy diamond to gold. Spectral fire adds faint red/blue fringes at highlight edges and a soft hue on about one facet in ten. An optional light sweep moves a soft vertical band across the wall. Intended as DJ material; there is no open center.

| Look | facets | goldTint | spectral | sweep | exposure | loopSeconds |
| --- | --- | --- | --- | --- | --- | --- |
| Default gold crystal | 5 | 0.65 | 0.3 | 0.5 | 1.0 | 24 |
| Ice diamond | 5 | 0.1 | 0.5 | 0.3 | 1.0 | 24 |
| Fine pave | 11 | 0.8 | 0.2 | 0.7 | 1.0 | 16 |
| Slow, large shards | 3.5 | 0.65 | 0.2 | 0.0 | 0.9 | 48 |

**Cost.** Low to moderate. Two 3x3 Voronoi passes (18 seed evaluations with a hash, sin, and cos each) plus three light box lookups. Cost is independent of `facets`.

**Limitations.**
- The border pass uses 3x3 instead of 5x5; rare small edge errors are possible.
- Facets are flat-shaded reflections, not refraction; "fire" is a channel offset, not physical dispersion.
- At high `exposure`, large lit facets can make big bright areas. Start at 1.0 or below on the LED wall.
- Changing `facets` live rescales the whole field.

## Notes for integration

- All three are available in the preview selector and use the same Source patch structure as LiquidGold.
- **Check cycle** now compares near-wrap frames as well as endpoints. Watch playback across the wrap too, especially after changing parameters.
- Wire compilation, Mac GPU cost at 1248 x 540, and appearance on the LED wall remain to be checked.
