# Golden glamour prompt set

Paste one complete prompt into **Positive prompt**. The workflow uses full Z-Image, 1664 x 720, 35 steps, CFG 4. Start with four different seeds per concept, then refine favorites with a fixed seed. Update both Save Image prefixes from P01 to the prompt ID. Evaluate the 1248 x 540 output too: tiny sparkle and intricate chains can disappear on the wall.

## Shared negative prompt

```text
text, lettering, logos, watermark, people, faces, hands, cluttered composition, muddy brown metal, plastic gold, cheap glitter, excessive lens flare, blown-out white highlights, blurry details, compression artifacts
```

## Performance backgrounds - controlled contrast, room for performers

### P01 - Liquid gold, dark center (starter)

```text
An ultra-wide stage background, sculptural folds of polished champagne gold flowing in from the lower left and upper right edges, broad fluid metal surfaces with beautifully controlled reflections and fine brushed detail, deep warm charcoal negative space across the middle half of the image, one elegant diagonal flow, black studio environment, contemporary luxury jewelry campaign lighting, restrained bright highlights, rich dimensional shadows, minimal and confident, photorealistic material study, edge-to-edge panoramic composition, the gold shapes stay primarily at the outer edges, no typography.
```

### P02 - Diamond fringe

```text
An ultra-wide black stage backdrop with a sparse fringe of large brilliant-cut diamonds suspended along the very top edge, each stone individually defined with realistic internal refraction and subtle champagne reflections, a few small gold mounting details, almost all of the central and lower canvas remains uninterrupted deep black, elegant high jewelry editorial photography, precise softbox lighting, controlled white points of light, extremely clean panoramic composition, subtle luxury, no lettering.
```

### P03 - Cuban chain corners

```text
A panoramic luxury hip-hop jewelry still life, oversized solid 18-karat yellow gold Cuban link chains entering from the far left and far right corners, beautifully interlocked rounded links with real weight and thickness, broad polished surfaces with subtle wear, chains curve out of frame before reaching the center, expansive matte black negative space in the middle, warm controlled studio reflections, minimal contemporary composition, confident and understated, no pendants, no letters, no people.
```

### P04 - Satin and metal

```text
An ultra-wide abstract stage background of black satin drapery and two broad champagne-gold metallic ribbons, gentle sweeping horizontal folds along the bottom third, soft side lighting, deep shadowed center and upper half, refined material contrast between soft black fabric and hard reflective gold, elegant modern fashion campaign aesthetic, large simple shapes, subtle depth, quiet visual rhythm, no figures, no typography, no ornate architecture.
```

### P05 - Vermilion and gold

```text
A contemporary ultra-wide abstract composition, deep vermilion silk-like waves gently framing the outer edges of a dark oxblood center, a thin sculptural ribbon of luminous brushed gold tracing one wave across the lower edge, warm soft light, rich tactile surfaces, generous open middle area, restrained celebratory energy, sophisticated red and gold palette, clean large forms visible from a distance, no text, no people, no religious figures, no ornate palace decoration.
```

## DJ backgrounds - stronger forms and depth

### D01 - Chain portal

```text
An ultra-wide surreal jewelry installation, a massive oval ring made from thick perfectly interlocking polished gold Cuban chain links floating in a black void, the opening is wide and dark, the chain ring fills the outer edges of the panoramic frame, several smaller rings recede into deep perspective through the opening, physically convincing heavy gold, dramatic amber rim light and crisp white reflections, modern hip-hop luxury, bold readable geometry, photorealistic 3D render, no text, no people.
```

### D02 - Diamond prism field

```text
An ultra-wide futuristic field of huge clear diamond prisms emerging from an obsidian floor, strongly separated geometric forms concentrated at the left and right, deep black space through the center, brilliant refraction with subtle violet and electric blue accents in the diamonds, champagne gold reflected light, dramatic low camera angle, highly polished surfaces, high-end jewelry campaign meets nightclub stage design, sharp clean geometry, no text, no figures.
```

### D03 - Gold frequency

```text
An ultra-wide abstract sculpture of a single thick liquid-gold ribbon oscillating across a black void like a bass waveform, broad rounded crests and valleys, a continuous smooth surface, strong near-to-far depth, molten mirror finish with warm yellow gold and dark bronze reflections, dramatic studio lighting, clean dimensional silhouette, energetic contemporary club aesthetic, generous black separation around the ribbon, no fine tangled filaments, no lettering.
```

### D04 - Faceted gold cavern

```text
An ultra-wide geometric tunnel constructed from enormous irregular planes of polished gold and black onyx, central vanishing point, strong perspective, bold angular facets and crisp edges, alternating dark panels and warm gold reflections, only a few concentrated white highlights, sophisticated contemporary stage scenography, immersive depth, dense geometry at the edges and a dark open distance in the middle, premium photorealistic architectural rendering, no lettering, no people.
```

### D05 - Diamond disco monoliths

```text
A panoramic arrangement of three oversized abstract jewelry monoliths floating above a black reflective surface, black chrome forms with wide bands of diamond pave and solid yellow gold, sparse balanced composition, tiny crisp diamond reflections contained within the broad bands, dramatic sculptural silhouettes, dark nightclub atmosphere, a subtle magenta accent light on one side, contemporary luxury music video aesthetic, physically convincing materials, no logos, no text, no people.
```

## Overlays - black-background plates for Screen/Add or luma key

These generate RGB images on black, not transparent PNGs. Screen/Add blending in Resolume makes dark areas unobtrusive; adjust levels or use a luma key if the model leaves gray haze. Additive blending also changes the appearance of dark metal, so use these primarily for luminous accents.

### O01 - Sparse diamond glints

```text
An ultra-wide black compositing plate with twelve isolated small diamond-like glints distributed asymmetrically around the outer thirds, a few clean four-point white star reflections with warm gold cores, substantial separation between every glint, most of the image is perfectly solid black, restrained high jewelry sparkle, crisp bright shapes with a tiny soft falloff, no haze, no environment, no surface, no text.
```

### O02 - Gold dust sweep

```text
An ultra-wide compositing plate, a sparse sweeping arc of glowing gold particles rising gently from the lower left toward the far right edge, a mix of a few larger soft amber points and small sharply defined gold sparks, particles occupy only a narrow flowing band, central upper area completely empty, solid pure black background, elegant slow celebratory sparkle, no smoke, no gray fog, no scenery, no lettering.
```

### O03 - Luminous corner ribbons

```text
An ultra-wide black compositing plate with two isolated curling ribbons of luminous champagne-gold light, one confined to the upper left corner and one to the lower right corner, smooth thick graceful curves with bright gold edges and subtle internal reflections, the middle two thirds entirely black, clean separation from a perfectly black background, premium minimal celebration graphic, no haze, no objects in the center, no text.
```

## Daytime / intermission stills

### S01 - Warm gold welcome background

```text
An ultra-wide minimal festive background, a warm ivory field with a subtle handmade paper texture, sweeping brushed-gold foil curves confined to the lower left and upper right corners, a few small faceted diamond accents nestled into the gold curves, generous clean center for separately typeset event information, soft natural light, polished contemporary luxury, simple large shapes, no generated text, no people, no religious figures.
```

### S02 - Sindoor-inspired abstract frame

```text
A refined panoramic abstract frame, rich vermilion pigment textures and crimson silk folds gathered along the far left and right edges, delicate irregular gold leaf highlights, calm warm cream negative space across the central half, contemporary editorial composition, tactile and celebratory, suitable as a background around separately placed existing festival artwork, no people, no deity imagery, no text, no ornate architecture.
```

Use last year's actual Durga and Sindoor Khela imagery for the religious/daytime material. These two prompts provide supporting backgrounds; they do not replace those photographs.

## Fast art-direction changes

- More restrained: add `fewer objects, larger forms, deeper black, softer light, quieter center`.
- More bling: add `thicker gold, larger diamond facets, sharper jewelry lighting`; avoid simply adding tiny glitter everywhere.
- Cooler contrast: replace a minor amber accent with cobalt, violet, or black chrome while retaining gold as the main material.
- Preserve usable center space for performance looks. Check highlights at reduced brightness on the actual wall.
- Still images are not motion loops. Start with Resolume transforms/effects; create video loops as a separate step after selecting the art direction.
