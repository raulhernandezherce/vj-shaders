// MotorISF: ejecuta shaders ISF generadores en WebGL 1, sin dependencias.
// Soporta las entradas float, long, bool, color y point2D, y TIME, TIMEDELTA, FRAMEINDEX,
// RENDERSIZE e isf_FragNormCoord.
class MotorISF {
  constructor(canvas) {
    this.canvas = canvas;
    const gl = canvas.getContext("webgl", { preserveDrawingBuffer: true, antialias: false });
    if (!gl) throw new Error("WebGL no disponible");
    this.gl = gl;
    const buf = gl.createBuffer();
    gl.bindBuffer(gl.ARRAY_BUFFER, buf);
    gl.bufferData(gl.ARRAY_BUFFER, new Float32Array([-1, -1, 1, -1, -1, 1, 1, 1]), gl.STATIC_DRAW);
    this.frame = 0;
    this.ultimo = 0;
  }

  static separar(fuente) {
    const m = fuente.match(/^\s*\/\*\s*(\{[\s\S]*?\})\s*\*\//);
    if (!m) return { meta: {}, cuerpo: fuente };
    return { meta: JSON.parse(m[1]), cuerpo: fuente.slice(m[0].length) };
  }

  compilar(fuente, nombre) {
    const gl = this.gl;
    const { meta, cuerpo } = MotorISF.separar(fuente);
    const tipos = { float: "float", long: "int", bool: "bool", color: "vec4", point2D: "vec2" };
    const valores = {};
    let decl = "";
    for (const e of meta.INPUTS || []) {
      if (!tipos[e.TYPE]) continue;
      decl += `uniform ${tipos[e.TYPE]} ${e.NAME};\n`;
      valores[e.NAME] = e.DEFAULT ?? (e.TYPE === "color" ? [1, 1, 1, 1] : e.TYPE === "point2D" ? [0.5, 0.5] : 0);
    }
    const vs = `attribute vec2 p; varying vec2 isf_FragNormCoord;
      void main() { isf_FragNormCoord = p * 0.5 + 0.5; gl_Position = vec4(p, 0.0, 1.0); }`;
    const fs = `precision highp float;
      uniform float TIME; uniform float TIMEDELTA; uniform int FRAMEINDEX; uniform vec2 RENDERSIZE;
      varying vec2 isf_FragNormCoord;
      ${decl}
      ${cuerpo}`;
    const prog = gl.createProgram();
    let error = null;
    for (const [tipo, src] of [[gl.VERTEX_SHADER, vs], [gl.FRAGMENT_SHADER, fs]]) {
      const s = gl.createShader(tipo);
      gl.shaderSource(s, src);
      gl.compileShader(s);
      if (!gl.getShaderParameter(s, gl.COMPILE_STATUS)) error = gl.getShaderInfoLog(s);
      gl.attachShader(prog, s);
    }
    gl.bindAttribLocation(prog, 0, "p");
    gl.linkProgram(prog);
    if (!error && !gl.getProgramParameter(prog, gl.LINK_STATUS)) error = gl.getProgramInfoLog(prog);
    if (error) console.error(`[ISF] ${nombre}: ${error}`);
    const loc = n => gl.getUniformLocation(prog, n);
    const u = { TIME: loc("TIME"), TIMEDELTA: loc("TIMEDELTA"), FRAMEINDEX: loc("FRAMEINDEX"), RENDERSIZE: loc("RENDERSIZE") };
    for (const e of meta.INPUTS || []) u[e.NAME] = loc(e.NAME);
    return { prog, meta, valores, u, error, nombre };
  }

  dibujar(sh, t, w, h) {
    const gl = this.gl;
    w = w || this.canvas.width; h = h || this.canvas.height;
    if (this.canvas.width < w) this.canvas.width = w;
    if (this.canvas.height < h) this.canvas.height = h;
    if (sh.error) return;
    gl.viewport(0, this.canvas.height - h, w, h);
    gl.useProgram(sh.prog);
    gl.enableVertexAttribArray(0);
    gl.vertexAttribPointer(0, 2, gl.FLOAT, false, 0, 0);
    gl.uniform1f(sh.u.TIME, t);
    gl.uniform1f(sh.u.TIMEDELTA, Math.max(0, t - this.ultimo));
    gl.uniform1i(sh.u.FRAMEINDEX, this.frame++);
    gl.uniform2f(sh.u.RENDERSIZE, w, h);
    this.ultimo = t;
    for (const e of sh.meta.INPUTS || []) {
      const v = sh.valores[e.NAME], l = sh.u[e.NAME];
      if (l == null) continue;
      if (e.TYPE === "float") gl.uniform1f(l, v);
      else if (e.TYPE === "long") gl.uniform1i(l, Math.round(v));
      else if (e.TYPE === "bool") gl.uniform1i(l, v ? 1 : 0);
      else if (e.TYPE === "color") gl.uniform4fv(l, v);
      else if (e.TYPE === "point2D") gl.uniform2fv(l, v);
    }
    gl.drawArrays(gl.TRIANGLE_STRIP, 0, 4);
  }
}
window.MotorISF = MotorISF;
