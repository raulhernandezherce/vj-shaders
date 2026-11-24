/*{
  "ISFVSN": "2",
  "DESCRIPTION": "Barras de color con cortes horizontales, separación RGB y ruido digital a golpes.",
  "CREDIT": "Raúl Hernández Herce",
  "CATEGORIES": ["Generador", "Glitch"],
  "INPUTS": [
    { "NAME": "intensidad", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "golpes", "TYPE": "float", "DEFAULT": 4.0, "MIN": 0.5, "MAX": 16.0 }
  ]
}*/
float azar(float x) { return fract(sin(x * 91.3458) * 47453.5453); }

vec3 barras(float x) {
    float i = floor(x * 7.0);
    return vec3(step(0.5, fract(i / 2.0)), step(0.5, fract(i / 4.0)), step(0.5, fract(i / 8.0 + 0.5))) * 0.9 + 0.05;
}

void main() {
    vec2 uv = isf_FragNormCoord;
    float paso = floor(TIME * golpes);
    float banda = floor(uv.y * 24.0);
    float activo = step(1.0 - intensidad * 0.6, azar(banda + paso * 13.0));
    float desp = (azar(banda * 3.1 + paso) - 0.5) * 0.3 * activo * intensidad;
    float sep = 0.02 * intensidad * activo;
    vec3 col = vec3(barras(uv.x + desp + sep).r, barras(uv.x + desp).g, barras(uv.x + desp - sep).b);
    col *= 0.85 + 0.15 * sin(uv.y * RENDERSIZE.y * 1.5);
    col += activo * intensidad * 0.3 * azar(floor(uv.x * 80.0) + banda + paso);
    gl_FragColor = vec4(col, 1.0);
}
