const canvas = document.querySelector('#view');
const gl = canvas.getContext('webgl2', {preserveDrawingBuffer:true, alpha:false});
const status = document.querySelector('#status');
const error = document.querySelector('#error');
const shaderSelect = document.querySelector('#shader');
const textureSelect = document.querySelector('#texture');
let program, metadata, values = {}, uniforms = {}, running = true, time = 0, last = performance.now(), lastDraw = 0, loadVersion = 0;

function compile(type, source) {
  const shader = gl.createShader(type);
  gl.shaderSource(shader, source); gl.compileShader(shader);
  if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS)) {
    const message = gl.getShaderInfoLog(shader); gl.deleteShader(shader); throw new Error(message);
  }
  return shader;
}

function build(source) {
  const match = source.match(/\/\*([\s\S]*?)\*\//);
  const meta = JSON.parse(match[1]);
  const declarations = meta.INPUTS.map(i => `uniform ${i.TYPE==='image'?'sampler2D':i.TYPE==='bool'?'bool':'float'} ${i.NAME};`).join('\n');
  const vertex = compile(gl.VERTEX_SHADER, '#version 300 es\nprecision highp float;\nin vec2 position; out vec2 uv; void main(){uv=position*.5+.5;gl_Position=vec4(position,0,1);}');
  const fragment = compile(gl.FRAGMENT_SHADER, `#version 300 es\nprecision highp float;\nin vec2 uv; out vec4 outputColor;\nuniform vec2 RENDERSIZE; uniform float TIME;\n#define isf_FragNormCoord uv\n#define gl_FragColor outputColor\n#define IMG_NORM_PIXEL(img,coord) texture(img,coord)\n${declarations}\n${source.replace(match[0],'')}`);
  const result = gl.createProgram(); gl.attachShader(result, vertex); gl.attachShader(result, fragment); gl.linkProgram(result);
  gl.deleteShader(vertex); gl.deleteShader(fragment);
  if (!gl.getProgramParameter(result, gl.LINK_STATUS)) {const message=gl.getProgramInfoLog(result); gl.deleteProgram(result); throw new Error(message);}
  return {program:result, metadata:meta};
}

function setupControls() {
  document.querySelector('#controls').replaceChildren();
  for (const input of metadata.INPUTS.filter(i=>i.TYPE!=='image')) {
    values[input.NAME] = input.DEFAULT;
    const label=document.createElement('label'); label.className='control';
    const line=document.createElement('span'); const name=document.createElement('span'); name.textContent=input.LABEL||input.NAME;
    const value=document.createElement('output'); value.textContent=input.TYPE==='bool'?'':Number(input.DEFAULT).toFixed(2);
    line.append(name,value); const control=document.createElement('input');
    control.setAttribute('aria-label',input.LABEL||input.NAME);
    if(input.TYPE==='bool'){control.type='checkbox';control.checked=input.DEFAULT;}
    else{control.type='range';control.min=input.MIN;control.max=input.MAX;control.step=input.NAME==='segments'?'1':'0.01';control.value=input.DEFAULT;}
    control.addEventListener('input',()=>{values[input.NAME]=input.TYPE==='bool'?control.checked:Number(control.value);value.textContent=input.TYPE==='bool'?'':Number(control.value).toFixed(2);draw();});
    label.append(line,control); document.querySelector('#controls').append(label);
  }
}

function activate(built) {
  if(program)gl.deleteProgram(program);
  program=built.program;metadata=built.metadata;uniforms={};gl.useProgram(program);
  for(const name of ['RENDERSIZE','TIME',...metadata.INPUTS.map(i=>i.NAME)])uniforms[name]=gl.getUniformLocation(program,name);
  const position=gl.getAttribLocation(program,'position');gl.enableVertexAttribArray(position);gl.vertexAttribPointer(position,2,gl.FLOAT,false,0,0);
  document.querySelector('#description').textContent=shaderSelect.options[shaderSelect.selectedIndex].text;
  textureSelect.disabled=!metadata.INPUTS.some(i=>i.TYPE==='image');setupControls();draw();
}

function draw(overrides={}) {
  if(!program)return;gl.useProgram(program);gl.viewport(0,0,canvas.width,canvas.height);
  gl.uniform2f(uniforms.RENDERSIZE,canvas.width,canvas.height);gl.uniform1f(uniforms.TIME,time);
  for(const input of metadata.INPUTS){const value=Object.hasOwn(overrides,input.NAME)?overrides[input.NAME]:values[input.NAME];
    if(input.TYPE==='image')gl.uniform1i(uniforms[input.NAME],0);
    else if(input.TYPE==='bool')gl.uniform1i(uniforms[input.NAME],value?1:0);
    else gl.uniform1f(uniforms[input.NAME],value);
  }
  gl.drawArrays(gl.TRIANGLES,0,6);
}

async function loadShader() {
  const version=++loadVersion;
  try{const response=await fetch(`../${shaderSelect.value}.fs`);if(!response.ok)throw new Error('Shader file could not be loaded.');
    const built=build(await response.text());if(version!==loadVersion){gl.deleteProgram(built.program);return;}
    activate(built);error.textContent='';status.textContent='Shader compiled. Live preview ready.';
  }catch(e){error.textContent=e.message;status.textContent='Shader needs attention.';}
}

async function setTexture(source) {
  const image=new Image();image.src=source;await image.decode();
  gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL,true);gl.texImage2D(gl.TEXTURE_2D,0,gl.RGBA,gl.RGBA,gl.UNSIGNED_BYTE,image);draw();
}

if(!gl){status.textContent='WebGL 2 is unavailable in this browser.';throw new Error('WebGL 2 required');}
const buffer=gl.createBuffer();gl.bindBuffer(gl.ARRAY_BUFFER,buffer);gl.bufferData(gl.ARRAY_BUFFER,new Float32Array([-1,-1,1,-1,-1,1,-1,1,1,-1,1,1]),gl.STATIC_DRAW);
const texture=gl.createTexture();gl.activeTexture(gl.TEXTURE0);gl.bindTexture(gl.TEXTURE_2D,texture);
for(const axis of [gl.TEXTURE_WRAP_S,gl.TEXTURE_WRAP_T])gl.texParameteri(gl.TEXTURE_2D,axis,gl.CLAMP_TO_EDGE);
gl.texParameteri(gl.TEXTURE_2D,gl.TEXTURE_MIN_FILTER,gl.LINEAR);gl.texParameteri(gl.TEXTURE_2D,gl.TEXTURE_MAG_FILTER,gl.LINEAR);
await setTexture(`../../outputs/stills/wall/${textureSelect.value}`);await loadShader();
shaderSelect.addEventListener('change',loadShader);
textureSelect.addEventListener('change',()=>setTexture(`../../outputs/stills/wall/${textureSelect.value}`).catch(e=>error.textContent=e.message));
document.querySelector('#upload').addEventListener('change',async event=>{const file=event.target.files[0];if(!file)return;const url=URL.createObjectURL(file);try{await setTexture(url);}catch(e){error.textContent=e.message;}finally{URL.revokeObjectURL(url);}});
document.querySelector('#pause').addEventListener('click',event=>{running=!running;event.target.textContent=running?'Pause':'Play';});
document.querySelector('#check').addEventListener('click',()=>{
  const a=new Uint8Array(canvas.width*canvas.height*4),b=new Uint8Array(a.length);
  draw({animate:false,phase:0});gl.readPixels(0,0,canvas.width,canvas.height,gl.RGBA,gl.UNSIGNED_BYTE,a);
  draw({animate:false,phase:1});gl.readPixels(0,0,canvas.width,canvas.height,gl.RGBA,gl.UNSIGNED_BYTE,b);
  let maximum=0;for(let i=0;i<a.length;i++)maximum=Math.max(maximum,Math.abs(a[i]-b[i]));
  status.textContent=`Cycle check: maximum endpoint difference ${maximum}/255. ${maximum===0?'Exact match.':'Inspect the join.'}`;draw();
});
function tick(now){if(running)time+=(now-last)/1000;last=now;if(now-lastDraw>1000/30){draw();lastDraw=now;}requestAnimationFrame(tick);}requestAnimationFrame(tick);
