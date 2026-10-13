/*{
  "ISFVSN": "2.0",
  "DESCRIPTION": "Periodic liquid displacement for stills, video, or a live camera. Preserves source alpha.",
  "CATEGORIES": ["Distortion"],
  "INPUTS": [
    {"NAME":"inputImage","TYPE":"image"},
    {"NAME":"amount","LABEL":"Flow (pixels)","TYPE":"float","DEFAULT":18.0,"MIN":0.0,"MAX":80.0},
    {"NAME":"detail","LABEL":"Wave scale","TYPE":"float","DEFAULT":1.0,"MIN":0.3,"MAX":3.0},
    {"NAME":"loopSeconds","LABEL":"Cycle (seconds)","TYPE":"float","DEFAULT":8.0,"MIN":2.0,"MAX":60.0},
    {"NAME":"phase","LABEL":"Phase","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"animate","LABEL":"Auto animate","TYPE":"bool","DEFAULT":true}
  ]
}*/

const float TAU = 6.28318530718;
vec2 mirrorUV(vec2 p) { return 1.0 - abs(mod(p, 2.0) - 1.0); }

void main() {
    vec2 uv = isf_FragNormCoord;
    // All temporal frequencies are integer multiples, so phase 0 and 1 match.
    float t = TAU * fract(phase + (animate ? TIME / max(loopSeconds, 0.01) : 0.0));
    vec2 envelope = pow(sin(3.14159265359 * uv), vec2(2.0));
    vec2 wave = vec2(
        sin(TAU * uv.y * detail + t) + 0.3 * sin(TAU * uv.x * detail - 2.0*t),
        sin(2.0 * TAU * uv.x * detail - t) + 0.3 * cos(TAU * uv.y * detail + 2.0*t)
    );
    // Displacement is in output pixels, independent of the wall's aspect ratio.
    vec2 warped = uv + amount * wave * envelope / RENDERSIZE;
    gl_FragColor = IMG_NORM_PIXEL(inputImage, mirrorUV(warped));
}

