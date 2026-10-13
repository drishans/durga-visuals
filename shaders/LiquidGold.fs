/*{
  "ISFVSN": "2.0",
  "DESCRIPTION": "A procedural metallic silk source with an open center. No image or video input required.",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    {"NAME":"folds","LABEL":"Fold scale","TYPE":"float","DEFAULT":1.2,"MIN":0.5,"MAX":3.0},
    {"NAME":"clearCenter","LABEL":"Open center","TYPE":"float","DEFAULT":0.4,"MIN":0.0,"MAX":0.8},
    {"NAME":"polish","LABEL":"Polish","TYPE":"float","DEFAULT":0.65,"MIN":0.0,"MAX":1.0},
    {"NAME":"exposure","LABEL":"Brightness","TYPE":"float","DEFAULT":1.0,"MIN":0.25,"MAX":1.8},
    {"NAME":"loopSeconds","LABEL":"Cycle (seconds)","TYPE":"float","DEFAULT":12.0,"MIN":2.0,"MAX":60.0},
    {"NAME":"phase","LABEL":"Phase","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"animate","LABEL":"Auto animate","TYPE":"bool","DEFAULT":true}
  ]
}*/

const float TAU = 6.28318530718;

float heightField(vec2 p, float t) {
    p *= folds;
    float drift = 0.18*sin(t) + 0.11*cos(2.0*t + p.x);
    float bend = p.y + 0.32*sin(p.x*1.8 - t) + drift;
    return 0.28*sin(bend*7.0 + p.x*0.8)
         + 0.09*sin(bend*13.0 - p.x*1.5 + cos(t))
         + 0.035*cos(p.x*4.0 + bend*19.0 + 2.0*t);
}

void main() {
    vec2 uv = isf_FragNormCoord;
    vec2 p = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    float t = TAU * fract(phase + (animate ? TIME / max(loopSeconds, 0.01) : 0.0));
    float e = 1.0 / RENDERSIZE.y;
    float h = heightField(p, t);
    vec2 gradient = vec2(heightField(p+vec2(e,0.0),t)-h,
                         heightField(p+vec2(0.0,e),t)-h) / e;
    vec3 normal = normalize(vec3(-gradient, 1.0));
    vec3 reflection = reflect(vec3(0.0,0.0,-1.0), normal);
    // Softbox-like bands in a dark studio environment give a metal reflection.
    float broad = exp(-pow((reflection.y-0.4)*2.6, 2.0));
    float edge = exp(-pow((reflection.y+0.32)*mix(7.0,22.0,polish), 2.0));
    float side = pow(max(dot(normal,normalize(vec3(-0.6,0.8,1.0))),0.0), 2.0);
    vec3 gold = vec3(0.17,0.073,0.018)
              + vec3(0.66,0.41,0.12)*broad
              + vec3(0.90,0.79,0.48)*edge*0.72
              + vec3(0.20,0.13,0.045)*side;
    // Large open middle, flowing metal at the outer edges.
    float radius = length(vec2(p.x*0.70, p.y*1.75));
    float boundary = clearCenter + 0.075*sin(p.x*2.7+t);
    float metal = smoothstep(boundary, boundary+0.15, radius);
    vec3 background = vec3(0.007,0.009,0.012);
    vec3 color = mix(background, gold*exposure, metal);
    gl_FragColor = vec4(clamp(color,0.0,1.0), 1.0);
}
