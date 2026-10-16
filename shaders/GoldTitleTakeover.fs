/*{
  "ISFVSN":"2.0",
  "DESCRIPTION":"Gold DJ-name takeover for the transparent title PNGs. Bright face pixels become animated metallic gold with a light sweep, edge glints and bloom; the dark outline, extrusion and shadow from the PNG are kept. Intro drives a scale punch with glitch; Pulse is a beat phase for BPM Sync. Straight-alpha output.",
  "CATEGORIES":["Stylize"],
  "INPUTS":[
    {"NAME":"inputImage","TYPE":"image"},
    {"NAME":"intro","LABEL":"Intro","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"pulse","LABEL":"Pulse (beat phase)","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"punch","LABEL":"Pulse strength","TYPE":"float","DEFAULT":0.6,"MIN":0.0,"MAX":1.0},
    {"NAME":"speed","LABEL":"Speed","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":3.0},
    {"NAME":"glow","LABEL":"Glow","TYPE":"float","DEFAULT":0.8,"MIN":0.0,"MAX":2.0},
    {"NAME":"sparkle","LABEL":"Sparkle","TYPE":"float","DEFAULT":0.6,"MIN":0.0,"MAX":1.0},
    {"NAME":"size","LABEL":"Readable size","TYPE":"float","DEFAULT":1.0,"MIN":0.5,"MAX":2.5},
    {"NAME":"opacity","LABEL":"Opacity","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":1.0}
  ]
}*/

const float TAU = 6.28318530718;

float hash12(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx)*0.1031);
    p3 += dot(p3,p3.yzx+33.33);
    return fract((p3.x+p3.y)*p3.z);
}
// Title texel; fully transparent beyond the image so zooms never smear edges.
vec4 title(vec2 p) {
    if (p.x < 0.0 || p.y < 0.0 || p.x > 1.0 || p.y > 1.0) return vec4(0.0);
    return IMG_NORM_PIXEL(inputImage,p);
}
// Bright gold face of the letters (alpha minus the dark outline and shadow).
float faceOf(vec4 c) {
    float lum = dot(c.rgb,vec3(0.299,0.587,0.114));
    return c.a*smoothstep(0.28,0.5,lum);
}
float face(vec2 p) { return faceOf(title(p)); }
vec3 goldRamp(float x) {
    vec3 c = mix(vec3(0.28,0.13,0.03),vec3(0.86,0.55,0.13),smoothstep(0.0,0.45,x));
    c = mix(c,vec3(1.0,0.84,0.42),smoothstep(0.4,0.75,x));
    return mix(c,vec3(1.0,0.97,0.86),smoothstep(0.78,1.0,x));
}
float easeOutBack(float x) {
    float y = x-1.0;
    return 1.0+2.70158*y*y*y+1.70158*y*y;
}

void main() {
    vec2 q = isf_FragNormCoord;
    vec2 px = 1.0/RENDERSIZE;
    float t = TIME*speed;

    // Intro: 0 hidden, scale punch with overshoot, glitch until ~0.7, clean hold at 1.
    float e = clamp(intro,0.0,1.0);
    float appear = smoothstep(0.0,0.12,e);
    float land = smoothstep(0.0,0.55,e);
    float glitch = (1.0-smoothstep(0.3,0.7,e))*step(0.0001,e);
    float fl = (e-0.56)/0.07;
    float flash = exp(-fl*fl);
    // Pulse: beat phase 0..1. The hit is at 0 and decays across the beat.
    float kick = punch*exp(-5.0*clamp(pulse,0.0,1.0));
    float scale = size*mix(2.4,1.0,easeOutBack(land))*(1.0+0.05*kick);

    vec2 uv = (q-0.5)/max(scale,0.001)+0.5;
    // Glitch: random horizontal slice shifts that re-roll as Intro advances.
    float slice = floor(q.y*18.0);
    float roll = floor(e*24.0);
    float r1 = hash12(vec2(slice,roll));
    float r2 = hash12(vec2(slice+7.0,roll+3.0));
    uv.x += (r1-0.5)*0.22*glitch*step(0.55,r2);

    vec4 src = title(uv);
    float f = faceOf(src);
    float a = src.a;

    // Bevel normal from the face mask, for edge highlights.
    vec2 d = 2.0*px/max(scale,0.001);
    vec2 g = vec2(face(uv+vec2(d.x,0.0))-face(uv-vec2(d.x,0.0)),
                  face(uv+vec2(0.0,d.y))-face(uv-vec2(0.0,d.y)));
    vec3 n = normalize(vec3(-g,0.35));
    float spec = pow(max(0.0,dot(n,normalize(vec3(-0.45,0.6,0.65)))),6.0)*length(g);

    // Metallic gold: PNG shading for structure plus a slowly moving reflection.
    float lum = dot(src.rgb,vec3(0.299,0.587,0.114));
    float env = 0.5+0.5*sin(TAU*(q.y*2.2-q.x*0.35)+t*1.3);
    float shade = clamp(lum*0.8+0.22*env-0.1+0.6*spec,0.0,1.0);
    vec3 gold = goldRamp(shade);
    // Light sweep: a sheared bright band crossing the letters.
    float sweepX = q.x+0.35*(q.y-0.5);
    float pos = mix(-0.3,1.3,fract(t*0.3));
    float sx = sweepX-pos;
    float band = exp(-sx*sx/0.002025)+0.35*exp(-sx*sx/0.0196);
    gold = mix(gold,vec3(1.0,0.97,0.88),clamp(band*0.9+flash*0.6,0.0,1.0));
    gold *= 1.0+0.35*kick;
    // Keep outline, extrusion and shadow from the PNG; only the face is replaced.
    vec3 letters = mix(src.rgb*0.7,gold,f/max(a,0.0001));

    // Bloom: blurred face mask over a 48-tap spiral disk (dense enough to avoid
    // ghost copies of the letters), glowing behind the dark outline.
    // A fixed per-pixel rotation turns leftover tap patterns into fine grain.
    float halo = 0.0;
    float spin = TAU*hash12(floor(q*RENDERSIZE));
    for (int k=0; k<48; ++k) {
        float fk = float(k);
        float radius = sqrt((fk+0.5)/48.0)*28.0;
        float ang = fk*2.39996323+spin;
        halo += face(uv+vec2(cos(ang),sin(ang))*radius*px/max(scale,0.001));
    }
    halo /= 48.0;
    float glowA = clamp(halo*glow*(1.6+1.5*kick+2.0*flash),0.0,0.95);
    vec3 glowC = mix(vec3(1.0,0.55,0.12),vec3(1.0,0.86,0.5),halo);

    // Glitch colour fringes from offset copies of the alpha.
    vec2 off = vec2(0.014*glitch,0.0);
    float aR = title(uv+off).a;
    float aB = title(uv-off).a;
    float fringeA = clamp(max(aR,aB)*glitch,0.0,1.0);
    vec3 fringeC = (vec3(1.0,0.1,0.6)*aR+vec3(0.1,0.9,1.0)*aB)/max(aR+aB,0.0001);

    // Sparkle glints: one candidate per 26-pixel cell, kept only on face edges.
    vec2 cellPx = q*RENDERSIZE/26.0;
    float spark = 0.0;
    for (int j=-1; j<=1; ++j) {
        for (int i=-1; i<=1; ++i) {
            vec2 cell = floor(cellPx)+vec2(float(i),float(j));
            float r = hash12(cell);
            vec2 centre = (cell+vec2(hash12(cell+19.1),hash12(cell+41.7)))*26.0*px;
            vec2 cuv = (centre-0.5)/max(scale,0.001)+0.5;
            float edge = abs(face(cuv+3.0*px)-face(cuv-3.0*px));
            float tw = pow(max(0.0,sin(t*2.6+r*TAU*7.0)),14.0);
            tw = max(tw,kick*step(0.7,r));
            float live = edge*step(1.0-sparkle*0.85,r)*tw;
            vec2 dp = (q-centre)*RENDERSIZE;
            float len = 9.0+8.0*r;
            float star = exp(-dot(dp,dp)/4.0)
                       + exp(-abs(dp.y)/0.9)*exp(-abs(dp.x)/len)
                       + exp(-abs(dp.x)/0.9)*exp(-abs(dp.y)/len);
            spark += live*star;
        }
    }
    spark = clamp(spark*(1.0+band),0.0,1.0);

    // Composite back to front in premultiplied space, then emit straight alpha.
    vec3 C = glowC*glowA;
    float A = glowA;
    C = fringeC*fringeA+C*(1.0-fringeA);
    A = fringeA+A*(1.0-fringeA);
    C = letters*a+C*(1.0-a);
    A = a+A*(1.0-a);
    C = vec3(1.0,0.96,0.82)*spark+C*(1.0-spark);
    A = spark+A*(1.0-spark);
    gl_FragColor = vec4(clamp(C/max(A,0.0001),0.0,1.0),A*appear*opacity);
}
