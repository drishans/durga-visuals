/*{
  "ISFVSN": "2.0",
  "DESCRIPTION": "Rotating mirrored facets for diamond images, video, or camera textures. Preserves source alpha.",
  "CATEGORIES": ["Distortion"],
  "INPUTS": [
    {"NAME":"inputImage","TYPE":"image"},
    {"NAME":"segments","LABEL":"Facets","TYPE":"float","DEFAULT":6.0,"MIN":2.0,"MAX":16.0},
    {"NAME":"zoom","LABEL":"Zoom","TYPE":"float","DEFAULT":1.15,"MIN":0.4,"MAX":3.0},
    {"NAME":"breath","LABEL":"Breathing","TYPE":"float","DEFAULT":0.06,"MIN":0.0,"MAX":0.25},
    {"NAME":"loopSeconds","LABEL":"Cycle (seconds)","TYPE":"float","DEFAULT":16.0,"MIN":4.0,"MAX":60.0},
    {"NAME":"phase","LABEL":"Phase","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"animate","LABEL":"Auto animate","TYPE":"bool","DEFAULT":true}
  ]
}*/

const float TAU = 6.28318530718;
vec2 mirrorUV(vec2 p) { return 1.0 - abs(mod(p, 2.0) - 1.0); }

void main() {
    vec2 aspect = vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    vec2 p = (isf_FragNormCoord - 0.5) * aspect;
    float t = TAU * fract(phase + (animate ? TIME / max(loopSeconds, 0.01) : 0.0));
    float sector = TAU / floor(segments + 0.5);
    // Avoid atan(0,0), which is undefined on some GPU drivers.
    float angle = length(p) < 0.000001 ? 0.0 : atan(p.y,p.x);
    angle = abs(mod(angle + t + sector*0.5, sector) - sector*0.5);
    float radius = length(p) / (zoom * (1.0 + breath*sin(t)));
    vec2 sampleUV = vec2(cos(angle),sin(angle))*radius/aspect + 0.5;
    gl_FragColor = IMG_NORM_PIXEL(inputImage, mirrorUV(sampleUV));
}
