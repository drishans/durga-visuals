/*{
  "ISFVSN":"2.0",
  "DESCRIPTION":"Tutorial source: radial bands and rotating spokes. Not yet rendered or tested in Wire.",
  "CATEGORIES":["Generator"],
  "INPUTS":[
    {"NAME":"phase","LABEL":"Phase","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"energy","LABEL":"Energy","TYPE":"float","DEFAULT":0.4,"MIN":0.0,"MAX":1.0},
    {"NAME":"hue","LABEL":"Color offset","TYPE":"float","DEFAULT":0.1,"MIN":0.0,"MAX":1.0},
    {"NAME":"spokes","LABEL":"Spokes","TYPE":"float","DEFAULT":8.0,"MIN":2.0,"MAX":20.0},
    {"NAME":"hit","LABEL":"White hit","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"brightness","LABEL":"Brightness","TYPE":"float","DEFAULT":0.65,"MIN":0.0,"MAX":1.0}
  ]
}*/

// Drive phase from the host. No second clock or hidden automatic flashing.
const float TAU=6.28318530718;
void main() {
    vec2 p=(isf_FragNormCoord-0.5)*vec2(RENDERSIZE.x/RENDERSIZE.y,1.0);
    float radius=length(p);
    float angle=radius<0.00001 ? 0.0 : atan(p.y,p.x);
    float t=TAU*fract(phase);
    float count=floor(spokes+0.5); // Integer count avoids an angular seam.
    float rings=0.5+0.5*cos(24.0*radius-2.0*t);
    float rays=0.5+0.5*cos(count*angle+t);
    float pattern=pow(rings,mix(5.0,2.0,energy))*mix(0.18,1.0,rays);
    pattern*=smoothstep(0.0,0.035,radius); // Smooth away the polar singularity.
    vec3 tint=0.5+0.5*cos(TAU*(hue+0.12*radius+vec3(0.0,0.33,0.67)));
    vec3 rgb=tint*pattern*mix(0.5,1.0,energy);
    rgb=mix(rgb,vec3(1.0),clamp(hit,0.0,1.0));
    gl_FragColor=vec4(rgb*brightness,1.0);
}
