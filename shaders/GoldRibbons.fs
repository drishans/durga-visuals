/*{
  "ISFVSN": "2.0",
  "DESCRIPTION": "Sculptural twisting gold ribbons that sweep around an adjustable open center. No image or video input required.",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    {"NAME":"ribbons","LABEL":"Ribbons","TYPE":"float","DEFAULT":4.0,"MIN":1.0,"MAX":6.0},
    {"NAME":"ribbonWidth","LABEL":"Width","TYPE":"float","DEFAULT":1.0,"MIN":0.4,"MAX":2.0},
    {"NAME":"twist","LABEL":"Twist","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":3.0},
    {"NAME":"flow","LABEL":"Flow","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0},
    {"NAME":"clearCenter","LABEL":"Open center","TYPE":"float","DEFAULT":0.7,"MIN":0.0,"MAX":1.0},
    {"NAME":"exposure","LABEL":"Brightness","TYPE":"float","DEFAULT":1.0,"MIN":0.25,"MAX":1.8},
    {"NAME":"loopSeconds","LABEL":"Cycle (seconds)","TYPE":"float","DEFAULT":20.0,"MIN":4.0,"MAX":90.0},
    {"NAME":"phase","LABEL":"Phase","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"animate","LABEL":"Auto animate","TYPE":"bool","DEFAULT":true}
  ]
}*/

// Browser compiled/rendered and cycle sampled on 2026-10-13. Wire/Mac untested.

const float TAU = 6.28318530718;

// +1 for even ribbons (above the midline), -1 for odd ribbons (below).
float ribbonSide(float fi) {
    return fract(fi * 0.5) < 0.25 ? 1.0 : -1.0;
}

// Centerline height of ribbon fi at horizontal position x. At the wall's sides
// each ribbon keeps its own lane near the midline; toward the middle a bell
// curve lifts it toward the top or bottom edge, opening the center.
float ribbonY(float x, float t, float fi) {
    float side = ribbonSide(fi);
    float lane = 0.04 + 0.055 * fi;
    float open = clamp(clearCenter, 0.0, 1.0) * exp(-1.8 * x * x);
    float y = mix(lane, 0.28 + 0.5 * lane, open);
    // Travelling waves. Temporal frequencies are whole multiples of the cycle.
    float ph = 2.39996 * fi;                    // golden-angle spread
    float amp = clamp(flow, 0.0, 3.0) * (1.0 - 0.6 * open);
    y += amp * (0.045 * sin(1.7 * x + side * t + ph)
              + 0.020 * sin(3.1 * x - 2.0 * side * t + 1.3 * ph));
    return side * y;
}

// Twist angle along ribbon fi. The x term sets how many half-turns are visible;
// side * t rolls the twist along the ribbon by one full turn per cycle.
float ribbonTwist(float x, float t, float fi) {
    return clamp(twist, 0.0, 4.0) * 2.4 * x + ribbonSide(fi) * t
         + 0.5 * sin(1.3 * x - t + fi) + 1.3 * fi;
}

// Face-on half width, with a gentle taper that drifts along the ribbon.
float ribbonHalfWidth(float x, float t, float fi) {
    float w = 0.03 * clamp(ribbonWidth, 0.2, 3.0) * (1.0 + 0.3 * sin(2.1 * fi + 0.5));
    return w * (0.85 + 0.15 * sin(0.9 * x + t + fi));
}

// Gold in a dark studio: a diagonal key strip, a low warm bounce, and a
// Fresnel-like rim. polished is 1 for the mirror front, 0 for the satin back.
vec3 goldShade(vec3 n, vec3 ray, float polished) {
    vec3 r = reflect(ray, n);
    float sharp = mix(3.0, 12.0, polished);
    float k = (dot(r.xy, vec2(-0.53, 0.85)) - 0.42) * sharp;
    float b = (r.y + 0.6) * 2.5;
    float rim = pow(clamp(1.0 - n.z, 0.0, 1.0), 3.0);
    vec3 body = mix(vec3(0.55, 0.33, 0.10), vec3(0.95, 0.67, 0.27), polished);
    vec3 spec = mix(body, vec3(1.0, 0.92, 0.72), 0.55);
    return body * (0.06 + 0.32 * exp(-b * b))
         + spec * exp(-k * k) * mix(0.7, 1.15, polished)
         + vec3(0.9, 0.6, 0.24) * rim * 0.5;
}

void main() {
    vec2 uv = isf_FragNormCoord;
    float height = max(RENDERSIZE.y, 1.0);
    vec2 p = (uv - 0.5) * vec2(RENDERSIZE.x / height, 1.0);
    float t = TAU * fract(phase + (animate ? TIME / max(loopSeconds, 0.01) : 0.0));
    float aa = 1.0 / height;                    // one output pixel in p units
    vec3 ray = normalize(vec3(0.3 * p, -1.0));  // mild perspective for reflections

    vec3 color = vec3(0.006, 0.006, 0.008);
    float count = floor(clamp(ribbons, 1.0, 6.0) + 0.5);
    float gain = clamp(exposure, 0.0, 4.0);

    // Back to front: ribbon 0 is farthest, later ribbons overlap earlier ones.
    for (int i = 0; i < 6; i++) {
        float fi = float(i);
        if (fi >= count) break;

        // Perpendicular distance to the centerline from its local slope.
        float e = 0.002;
        float y = ribbonY(p.x, t, fi);
        float slope = (ribbonY(p.x + e, t, fi) - ribbonY(p.x - e, t, fi)) / (2.0 * e);
        float inv = inversesqrt(1.0 + slope * slope);
        float dist = (p.y - y) * inv;
        vec2 across = vec2(-slope, 1.0) * inv;  // unit vector across the ribbon

        // A twisting flat strip: apparent width follows |cos(theta)|, with a
        // minimum so the edge-on pinch still reads as a fine bright line.
        float theta = ribbonTwist(p.x, t, fi);
        float c = cos(theta);
        float s = sin(theta);
        float hw = ribbonHalfWidth(p.x, t, fi) * (0.1 + 0.9 * abs(c));
        float u = clamp(dist / max(hw, 0.00001), -1.0, 1.0);

        // Soft drop shadow (light from above) and faint glow on what lies behind.
        float depth = mix(0.6, 1.0, fi / max(count - 1.0, 1.0));
        float shadow = 1.0 - smoothstep(0.8 * hw, hw + 0.05, abs(dist + 0.015));
        color *= 1.0 - 0.6 * shadow;
        color += vec3(0.045, 0.026, 0.008) * depth * gain * exp(-max(abs(dist) - hw, 0.0) / 0.03);

        // Visible normal. Face-on: the strip normal plus a slight cross curl.
        // Edge-on: a rounded edge, so the front/back flip at cos(theta) = 0
        // happens where both terms agree and stays continuous.
        float faceOn = smoothstep(0.0, 0.3, abs(c));
        float facing = c >= 0.0 ? 1.0 : -1.0;
        float nAcross = mix(0.9 * u, 0.45 * u - s * facing, faceOn);
        float nZ = mix(sqrt(max(1.0 - 0.81 * u * u, 0.0)), abs(c), faceOn);
        vec3 n = normalize(vec3(across * nAcross, max(nZ, 0.05)));

        // Alternate half-turns show the polished front or the satin back.
        float polished = mix(0.5, step(0.0, c), faceOn);
        vec3 metal = goldShade(n, ray, polished);
        metal *= mix(1.0, 0.5, smoothstep(0.75, 1.0, abs(u)));  // darker beveled edges

        float cover = 1.0 - smoothstep(hw - aa, hw + aa, abs(dist));
        color = mix(color, metal * depth * gain, cover);
    }
    gl_FragColor = vec4(clamp(color, 0.0, 1.0), 1.0);
}
