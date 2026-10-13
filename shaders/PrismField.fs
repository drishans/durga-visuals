/*{
  "ISFVSN": "2.0",
  "DESCRIPTION": "Slow faceted crystal shard field with gold reflections and restrained spectral fire. No image or video input required.",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    {"NAME":"facets","LABEL":"Facet scale","TYPE":"float","DEFAULT":5.0,"MIN":2.5,"MAX":14.0},
    {"NAME":"goldTint","LABEL":"Gold tint","TYPE":"float","DEFAULT":0.65,"MIN":0.0,"MAX":1.0},
    {"NAME":"spectral","LABEL":"Spectral fire","TYPE":"float","DEFAULT":0.3,"MIN":0.0,"MAX":1.0},
    {"NAME":"sweep","LABEL":"Light sweep","TYPE":"float","DEFAULT":0.5,"MIN":0.0,"MAX":1.0},
    {"NAME":"exposure","LABEL":"Brightness","TYPE":"float","DEFAULT":1.0,"MIN":0.25,"MAX":1.8},
    {"NAME":"loopSeconds","LABEL":"Cycle (seconds)","TYPE":"float","DEFAULT":24.0,"MIN":4.0,"MAX":90.0},
    {"NAME":"phase","LABEL":"Phase","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"animate","LABEL":"Auto animate","TYPE":"bool","DEFAULT":true}
  ]
}*/

// Browser compiled/rendered and cycle sampled on 2026-10-13. Wire/Mac untested.
// Third-party helper notices: see THIRD_PARTY_NOTICES.md.

const float TAU = 6.28318530718;

// Four pseudo-random values in [0,1) per 2D cell id ("Hash without Sine",
// Dave Hoskins). Avoids trigonometric hashing; floating-point results may still
// differ between GPU drivers.
vec4 hash42(vec2 p) {
    vec4 p4 = fract(p.xyxy * vec4(0.1031, 0.1030, 0.0973, 0.1099));
    p4 += dot(p4, p4.wzxy + 33.33);
    return fract((p4.xxyz + p4.yzzw) * p4.zywx);
}

// Seed point of a shard cell, in local cell coordinates: a fixed jitter plus a
// small closed orbit (one turn per cycle, either direction). Points stay in
// 0.15..0.85 of their cell, so 3x3 neighborhoods suffice below.
vec2 shardPoint(vec2 cell, float t) {
    vec4 h = hash42(cell);
    float turn = h.z < 0.5 ? -1.0 : 1.0;
    vec2 jitter = 0.3 * (h.xy - 0.5);
    float a = turn * t + TAU * h.w;
    return 0.5 + jitter + 0.2 * vec2(cos(a), sin(a));
}

// Nearest shard and distance to its border (Inigo Quilez's two-pass method).
// Returns xy = shard cell id, z = distance to the nearest border in cell units.
// The second pass uses 3x3 rather than 5x5; with the limited jitter above this
// is very close to exact and roughly a third cheaper.
vec3 shards(vec2 q, float t) {
    vec2 ip = floor(q);
    vec2 fp = q - ip;
    vec2 nearCell = vec2(0.0);
    vec2 nearVec = vec2(0.0);
    float best = 8.0;
    for (int j = -1; j <= 1; j++) {
        for (int i = -1; i <= 1; i++) {
            vec2 g = vec2(float(i), float(j));
            vec2 r = g + shardPoint(ip + g, t) - fp;
            float d = dot(r, r);
            if (d < best) { best = d; nearVec = r; nearCell = g; }
        }
    }
    float border = 8.0;
    for (int j = -1; j <= 1; j++) {
        for (int i = -1; i <= 1; i++) {
            vec2 g = nearCell + vec2(float(i), float(j));
            vec2 r = g + shardPoint(ip + g, t) - fp;
            vec2 diff = r - nearVec;
            float len2 = dot(diff, diff);
            // Skip the nearest point itself; normalizing a zero vector is undefined.
            if (len2 > 0.000001) {
                border = min(border, dot(0.5 * (nearVec + r), diff * inversesqrt(len2)));
            }
        }
    }
    return vec3(ip + nearCell, border);
}

// Each shard is a flat facet tilted slightly off the view axis. The tilt
// direction precesses by one full turn per cycle, so highlights travel slowly.
vec3 shardNormal(vec2 id, float t) {
    vec4 h = hash42(id + 17.31);
    float dir = h.x < 0.5 ? -1.0 : 1.0;
    float ang = TAU * h.y + dir * t;
    float tilt = mix(0.15, 0.55, h.z) * (1.0 + 0.15 * sin(t + TAU * h.w));
    return normalize(vec3(tilt * cos(ang), tilt * sin(ang), 1.0));
}

// Dark jewelry light box with two soft strip lights, seen in reflection.
// r is the xy part of the reflected ray; stripA is a unit direction.
float lightBox(vec2 r, vec2 stripA) {
    vec2 stripB = vec2(-stripA.y, stripA.x);
    float a = (dot(r, stripA) - 0.35) * 7.0;
    float b = (dot(r, stripB) + 0.45) * 5.0;
    return 0.015 + 0.85 * exp(-a * a) + 0.4 * exp(-b * b);
}

// Saturated hue ramp; x runs once around the color wheel per unit.
vec3 hue(float x) {
    return clamp(abs(fract(x + vec3(0.0, 2.0 / 3.0, 1.0 / 3.0)) * 6.0 - 3.0) - 1.0, 0.0, 1.0);
}

void main() {
    vec2 uv = isf_FragNormCoord;
    float height = max(RENDERSIZE.y, 1.0);
    vec2 p = (uv - 0.5) * vec2(RENDERSIZE.x / height, 1.0);
    float t = TAU * fract(phase + (animate ? TIME / max(loopSeconds, 0.01) : 0.0));
    float scale = clamp(facets, 1.5, 24.0);
    float fire = clamp(spectral, 0.0, 1.0);

    // Whole field drifts on a slow Lissajous path (cell units).
    vec2 q = p * scale + vec2(0.5 * sin(t), 0.3 * sin(2.0 * t + 0.7));
    vec3 v = shards(q, t);
    vec4 h = hash42(v.xy + 5.17);
    vec3 n = shardNormal(v.xy, t);

    // Mild perspective, so a highlight can slice across a facet instead of
    // lighting the whole facet at once.
    vec3 ray = normalize(vec3(0.35 * p, -1.0));
    vec2 r = reflect(ray, n).xy;

    // Strip lights lean slowly back and forth.
    float lean = 0.7 + 0.3 * sin(t);
    vec2 stripA = vec2(cos(lean), sin(lean));
    // Dispersion: red and blue see the light box shifted slightly across strip A.
    vec2 split = stripA * (0.05 * fire);
    float lit = lightBox(r, stripA);
    vec3 litRGB = vec3(lightBox(r + split, stripA), lit, lightBox(r - split, stripA));

    // Reflectance from icy diamond to gold. About one facet in ten carries a
    // soft hue, scaled by Spectral fire.
    vec3 tint = mix(vec3(0.88, 0.93, 1.0), vec3(1.0, 0.72, 0.33), clamp(goldTint, 0.0, 1.0));
    vec3 fireTint = mix(tint, hue(h.y + 0.08 * sin(t + TAU * h.z)), 0.55);
    tint = mix(tint, fireTint, step(0.9, h.x) * fire);

    // Light sweep: a soft vertical band crossing the wall once each way per cycle.
    float band = (p.x - 1.3 * sin(t)) * 1.4;
    float sweepLight = mix(1.0, 0.2 + 0.8 * exp(-band * band), clamp(sweep, 0.0, 1.0));

    // Dark smoky glass, varied per facet, plus reflections and spectral fringes.
    // The fringe term is exactly zero when Spectral fire is 0.
    vec3 color = vec3(0.010, 0.009, 0.008) * (1.0 + 1.5 * h.z);
    color += tint * lit * sweepLight;
    color = max(color + (litRGB - lit) * 0.9 * sweepLight, 0.0);

    // Facet edges: a dark groove with a fine bright bevel, sized for 540 px and
    // never thinner than about one output pixel.
    float px540 = height / 540.0;
    float edgePx = v.z * height / scale;
    float lineW = max(0.6 * px540, 0.6);
    color *= mix(0.5, 1.0, smoothstep(lineW, lineW + 3.0 * px540, edgePx));
    float line = 1.0 - smoothstep(lineW, lineW + 1.0, edgePx);
    color += tint * line * (0.08 + 0.5 * lit) * sweepLight;

    // Gentle vignette so the wall edges do not feel cut off.
    float vig = smoothstep(0.7, 1.5, length(p * vec2(0.75, 1.6)));
    color *= 1.0 - 0.3 * vig;
    gl_FragColor = vec4(clamp(color * clamp(exposure, 0.0, 4.0), 0.0, 1.0), 1.0);
}
