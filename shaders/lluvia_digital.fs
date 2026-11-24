/*{
  "ISFVSN": "2",
  "DESCRIPTION": "Columnas de caracteres que caen a distinta velocidad, con estela brillante.",
  "CREDIT": "Raúl Hernández Herce",
  "CATEGORIES": ["Generador", "Texto"],
  "INPUTS": [
    { "NAME": "columnas", "TYPE": "float", "DEFAULT": 40.0, "MIN": 8.0, "MAX": 120.0 },
    { "NAME": "velocidad", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0 },
    { "NAME": "color", "TYPE": "color", "DEFAULT": [0.3, 1.0, 0.5, 1.0] }
  ]
}*/
float azar(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

void main() {
    vec2 uv = isf_FragNormCoord;
    float aspecto = RENDERSIZE.x / RENDERSIZE.y;
    vec2 celdas = vec2(floor(columnas), floor(columnas / aspecto * 1.6));
    vec2 id = floor(uv * celdas);
    vec2 f = fract(uv * celdas);
    float vel = 0.3 + azar(vec2(id.x, 1.0)) * velocidad;
    float cabeza = fract(TIME * vel * 0.25 + azar(vec2(id.x, 7.0)));
    float y = 1.0 - (id.y + 0.5) / celdas.y;
    float d = fract(y - cabeza);
    float estela = pow(1.0 - d, 6.0);
    float glifo = step(0.5, azar(id + floor(TIME * 8.0 * azar(id)))) * step(0.15, f.x) * step(f.x, 0.85)
                * step(0.1, f.y) * step(f.y, 0.9);
    vec3 col = color.rgb * glifo * estela + vec3(0.8) * glifo * step(0.97, 1.0 - d);
    gl_FragColor = vec4(col, 1.0);
}
