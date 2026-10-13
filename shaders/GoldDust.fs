/*{
  "ISFVSN": "2.0",
  "DESCRIPTION": "Sparse drifting gold glitter in three depth layers with a calm open center. No image or video input required.",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    {"NAME":"density","LABEL":"Density","TYPE":"float","DEFAULT":0.18,"MIN":0.02,"MAX":0.6},
    {"NAME":"sparkle","LABEL":"Sparkle","TYPE":"float","DEFAULT":0.7,"MIN":0.0,"MAX":1.5},
    {"NAME":"bokeh","LABEL":"Near bokeh","TYPE":"float","DEFAULT":0.5,"MIN":0.0,"MAX":1.0},
    {"NAME":"clearCenter","LABEL":"Open center","TYPE":"float","DEFAULT":0.4,"MIN":0.0,"MAX":0.9},
    {"NAME":"exposure","LABEL":"Brightness","TYPE":"float","DEFAULT":1.0,"MIN":0.25,"MAX":1.8},
    {"NAME":"loopSeconds","LABEL":"Cycle (seconds)","TYPE":"float","DEFAULT":40.0,"MIN":8.0,"MAX":120.0},
    {"NAME":"phase","LABEL":"Phase","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"animate","LABEL":"Auto animate","TYPE":"bool","DEFAULT":true}
  ]
}*/

// Browser compiled/rendered and cycle sampled on 2026-10-13. Wire/Mac untested.
// Third-party helper notice: see THIRD_PARTY_NOTICES.md.

const float TAU = 6.28318530718;

// Four pseudo-random values in [0,1) per 2D cell id ("Hash without Sine",
// Dave Hoskins). Avoids trigonometric hashing; floating-point results may still
// differ between GPU drivers.
vec4 hash42(vec2 p) {
    vec4 p4 = fract(p.xyxy * vec4(0.1031, 0.1030, 0.0973, 0.1099));
    p4 += dot(p4, p4.wzxy + 33.33);
    return fract((p4.xxyz + p4.yzzw) * p4.zywx);
}

// Wraps an integer-valued cell index into [0, period). The +0.5 keeps floor()
// away from exact multiples, where approximate GPU division can round down.
float wrapCell(float c, float period) {
    return c - period * floor((c + 0.5) / period);
}

// 0 inside the open center, 1 outside, using LiquidGold's ellipse proportions.
// Edge width scales with the opening; +0.001 keeps the edges ordered at 0.
float openMask(vec2 p) {
    float c = max(clearCenter, 0.0);
    float radius = length(vec2(p.x * 0.70, p.y * 1.75));
    return smoothstep(0.6 * c, 1.4 * c + 0.001, radius);
}

// One depth layer of glitter on a grid of `scale` cells per wall height.
// Motion loop: the layer falls exactly `travel` wrap periods per cycle, and
// cell ids wrap vertically with that period, so phase 0 and 1 are identical.
// The period is taller than the visible span, so no repeat is ever on screen.
// Each cell holds at most one fleck, kept within 0.65 cell of its own cell, so
// the 2x2 nearest cells contain every fleck that can reach this pixel.
vec3 dustLayer(vec2 p, float t, float scale, float travel, float seed,
               float fill, float sizeRef, float gain, float starGain, bool defocus) {
    float height = max(RENDERSIZE.y, 1.0);
    float pxPerCell = height / scale;
    float sizePx = sizeRef * height / 540.0;    // sizes are authored for 540 px
    float period = floor(scale) + 3.0;
    float threshold = clamp(density, 0.0, 1.0) * fill;
    float glow = clamp(sparkle, 0.0, 2.0);

    vec2 q = p * scale;
    q.x += 0.35 * sin(t + 2.1 * seed);          // slow lateral breeze
    q.y += period * travel * (t / TAU);         // whole-period fall

    vec2 base = floor(q - 0.5);
    vec3 light = vec3(0.0);
    for (int j = 0; j < 2; j++) {
        for (int i = 0; i < 2; i++) {
            vec2 cell = base + vec2(float(i), float(j));
            vec2 id = vec2(cell.x, wrapCell(cell.y, period)) + seed * vec2(57.0, 113.0);
            vec4 a = hash42(id);                // presence, position, size
            if (a.x >= threshold) continue;
            vec4 b = hash42(id + 71.37);        // orbit, glint phase, glint rate, tint
            // Fade flecks near the threshold so the Density slider is smooth.
            float presence = clamp((threshold - a.x) * 40.0, 0.0, 1.0);

            // Rest position plus a small figure-eight orbit (1 and 2 turns per cycle).
            vec2 orbit = 0.1 * vec2(sin(t + TAU * b.x), cos(2.0 * t + TAU * b.x));
            vec2 center = cell + 0.25 + 0.5 * a.yz + orbit;

            vec2 d = (q - center) * pxPerCell;  // offset in output pixels
            float r = length(d);
            float reach = 0.65 * pxPerCell;
            if (r >= reach) continue;
            float window = 1.0 - smoothstep(0.6 * reach, reach, r);

            // Mask by the fleck's own position, so it dims whole, not cut in half.
            float open = openMask(p + (center - q) / scale);

            // One slow fade per cycle, plus 2 to 6 brief mirror glints per cycle.
            float life = 0.5 + 0.5 * sin(t + TAU * fract(17.0 * a.w));
            float rate = 2.0 + floor(b.z * 5.0);
            float glint = pow(max(0.5 + 0.5 * sin(rate * t + TAU * b.y), 0.0), 20.0);
            float level = gain * presence * open * life * (0.35 + 1.3 * glow * glint);
            vec3 tint = mix(vec3(1.0, 0.62, 0.30), vec3(1.0, 0.85, 0.58), b.w);
            tint = mix(tint, vec3(1.0, 0.96, 0.88), 0.6 * glint);

            float size = sizePx * mix(0.6, 1.4, a.w);
            float shape;
            if (defocus) {
                // Out-of-focus fleck: soft disc with a slightly brighter rim.
                float radius = max(size, 2.0);
                float disc = 1.0 - smoothstep(radius - 1.0, radius + 1.0, r);
                float rim = (r - radius) / 1.2;
                shape = 0.45 * disc + 0.4 * exp(-0.5 * rim * rim);
            } else {
                // Gaussian core, never narrower than about one pixel, plus halo.
                float sigma = max(size, 0.7);
                float core = exp(-0.5 * r * r / (sigma * sigma));
                float halo = 0.12 * exp(-r / (3.0 * sigma));
                // Four-point star filter, visible only while glinting.
                float spikeLen = 7.0 * sigma;
                float spikeW = max(0.75 * height / 540.0, 0.75);
                float sx = d.x / spikeW;
                float sy = d.y / spikeW;
                float star = exp(-0.5 * sy * sy - abs(d.x) / spikeLen)
                           + exp(-0.5 * sx * sx - abs(d.y) / spikeLen);
                shape = core + halo + 0.5 * starGain * glow * glint * star;
            }
            light += tint * (level * shape * window);
        }
    }
    return light;
}

void main() {
    vec2 uv = isf_FragNormCoord;
    vec2 p = (uv - 0.5) * vec2(RENDERSIZE.x / max(RENDERSIZE.y, 1.0), 1.0);
    float t = TAU * fract(phase + (animate ? TIME / max(loopSeconds, 0.01) : 0.0));

    // Warm black with a very faint haze outside the open center, lighter at the top.
    vec3 color = vec3(0.006, 0.005, 0.006)
               + vec3(0.016, 0.009, 0.003) * openMask(p) * (0.5 + 0.5 * uv.y);

    // Far specks and mid flecks. Each layer falls about 1.1 to 1.4 wall heights
    // per cycle; nearer layers move slightly faster for parallax.
    vec3 dust = dustLayer(p, t, 30.0, 1.0, 0.0, 0.8, 0.55, 0.45, 0.0, false)
              + dustLayer(p, t, 16.0, 1.0, 1.0, 1.0, 0.95, 0.85, 1.0, false);
    // Near layer: a few large defocused discs.
    float nearGain = clamp(bokeh, 0.0, 1.0);
    if (nearGain > 0.001) {
        dust += dustLayer(p, t, 7.0, 1.0, 2.0, 0.35, 9.0, 0.22 * nearGain, 0.0, true);
    }
    color += dust * clamp(exposure, 0.0, 4.0);
    gl_FragColor = vec4(clamp(color, 0.0, 1.0), 1.0);
}
