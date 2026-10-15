/*{
  "ISFVSN":"2.0",
  "DESCRIPTION":"Tutorial effect: title enters, holds, then zooms through. Straight-alpha input/output. Not yet rendered or tested in Wire.",
  "CATEGORIES":["Transform"],
  "INPUTS":[
    {"NAME":"inputImage","TYPE":"image"},
    {"NAME":"progress","LABEL":"Takeover progress","TYPE":"float","DEFAULT":0.4,"MIN":0.0,"MAX":1.0},
    {"NAME":"holdScale","LABEL":"Readable size","TYPE":"float","DEFAULT":1.0,"MIN":0.5,"MAX":2.5},
    {"NAME":"exitScale","LABEL":"Exit zoom multiplier","TYPE":"float","DEFAULT":3.0,"MIN":1.0,"MAX":6.0},
    {"NAME":"opacity","LABEL":"Opacity","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":1.0}
  ]
}*/

void main() {
    float u=clamp(progress,0.0,1.0);
    float enter=smoothstep(0.0,0.18,u);
    float leave=smoothstep(0.72,1.0,u);
    float scale=mix(0.45,holdScale,enter)*mix(1.0,exitScale,leave);
    vec2 uv=(isf_FragNormCoord-0.5)/max(scale,0.001)+0.5;
    // Transparent borders prevent stretched edge pixels during zoom-out.
    if (uv.x<0.0 || uv.y<0.0 || uv.x>1.0 || uv.y>1.0) {
        gl_FragColor=vec4(0.0);
        return;
    }
    vec4 color=IMG_NORM_PIXEL(inputImage,uv);
    float fade=smoothstep(0.0,0.08,u)*(1.0-smoothstep(0.88,1.0,u));
    color.a*=fade*opacity;
    gl_FragColor=color;
}
