/*{
  "ISFVSN":"2.0",
  "DESCRIPTION":"Colorful chrome and moving extrusion from a transparent title PNG. RGB is recolored; input alpha defines the letters. Straight-alpha output, no mesh required.",
  "CATEGORIES":["Stylize"],
  "INPUTS":[
    {"NAME":"inputImage","TYPE":"image"},
    {"NAME":"depth","LABEL":"Extrusion (pixels)","TYPE":"float","DEFAULT":22.0,"MIN":0.0,"MAX":40.0},
    {"NAME":"bevel","LABEL":"Edge light (pixels)","TYPE":"float","DEFAULT":2.0,"MIN":0.5,"MAX":4.0},
    {"NAME":"hue","LABEL":"Color offset","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"shine","LABEL":"Chrome shine","TYPE":"float","DEFAULT":0.7,"MIN":0.0,"MAX":1.0},
    {"NAME":"loopSeconds","LABEL":"Cycle (seconds)","TYPE":"float","DEFAULT":12.0,"MIN":4.0,"MAX":60.0},
    {"NAME":"phase","LABEL":"Phase","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"animate","LABEL":"Auto animate","TYPE":"bool","DEFAULT":true}
  ]
}*/

const float TAU = 6.28318530718;
float titleMask(vec2 p) {
    // Transparent beyond the texture, including at the edge of a cropped title.
    if (p.x < 0.0 || p.y < 0.0 || p.x > 1.0 || p.y > 1.0) return 0.0;
    return IMG_NORM_PIXEL(inputImage,p).a;
}
vec3 spectrum(float p) {
    return 0.5 + 0.5*cos(TAU*(p+vec3(0.0,0.33,0.67)));
}
void main() {
    vec2 uv = isf_FragNormCoord;
    vec2 px = 1.0/RENDERSIZE;
    float t = TAU*fract(phase+(animate ? TIME/max(loopSeconds,0.01) : 0.0));
    // Orbit the apparent depth while the front face remains legible and stable.
    vec2 direction = vec2(cos(t),0.65*sin(t));
    float front = titleMask(uv);
    float volume = front;
    for (int i=1; i<=40; ++i) {
        float a = titleMask(uv-direction*px*depth*(float(i)/40.0));
        volume = max(volume,a);
    }
    float dx = titleMask(uv-vec2(bevel*px.x,0.0))-titleMask(uv+vec2(bevel*px.x,0.0));
    float dy = titleMask(uv-vec2(0.0,bevel*px.y))-titleMask(uv+vec2(0.0,bevel*px.y));
    vec3 normal = normalize(vec3(dx,dy,0.6));
    vec3 light = normalize(vec3(cos(t+0.6),sin(t+0.6),0.8));
    float rim = max(0.0,dot(normal,light));
    float band = 0.5+0.5*sin(uv.y*19.0+2.0*sin(t)+uv.x*2.0);
    vec3 tint = spectrum(hue+uv.x*0.32+0.10*sin(t));
    vec3 face = mix(vec3(0.22),tint,0.78)*(0.65+0.35*band);
    face = mix(face,vec3(0.97,0.98,1.0),shine*pow(band,12.0));
    face += vec3(0.55,0.65,0.8)*rim*length(vec2(dx,dy))*0.55;
    vec3 side = spectrum(hue+0.25+uv.x*0.32+0.10*sin(t))*(0.25+0.08*cos(t+uv.y*12.0));
    // Emit straight alpha. Preview and host must composite it over a background.
    vec3 rgb = mix(side,face,front/max(volume,0.0001));
    gl_FragColor = vec4(clamp(rgb,0.0,1.0),volume);
}
