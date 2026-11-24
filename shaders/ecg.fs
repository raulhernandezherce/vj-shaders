/*{
  "ISFVSN": "2",
  "DESCRIPTION": "Trazado de ECG en un monitor de rejilla, con el barrido y la estela del fósforo de los monitores clásicos.",
  "CREDIT": "Raúl Hernández Herce",
  "CATEGORIES": ["Generador", "Línea"],
  "INPUTS": [
    { "NAME": "lpm", "TYPE": "float", "DEFAULT": 72.0, "MIN": 30.0, "MAX": 180.0 },
    { "NAME": "color", "TYPE": "color", "DEFAULT": [0.22, 1.0, 0.53, 1.0] }
  ]
}*/
float pico(float x, float centro, float ancho) {
    float d = (x - centro) / ancho;
    return exp(-d * d);
}

float onda(float x) {
    return 0.12 * pico(x, 0.18, 0.035)    // onda P
         - 0.12 * pico(x, 0.37, 0.01)     // Q
         + 1.00 * pico(x, 0.40, 0.012)    // R
         - 0.25 * pico(x, 0.43, 0.012)    // S
         + 0.30 * pico(x, 0.65, 0.06);    // onda T
}

void main() {
    vec2 uv = isf_FragNormCoord;
    float latidos = 3.0;
    float x = uv.x * latidos;
    float y = 0.35 + 0.35 * onda(fract(x));
    float barrido = fract(TIME * lpm / 60.0 / latidos);
    float edad = fract(barrido - uv.x);
    float linea = (1.0 - smoothstep(0.0, 0.012, abs(uv.y - y))) * pow(1.0 - edad, 2.0) * step(0.02, edad);
    vec2 g = abs(fract(uv * vec2(30.0, 15.0)) - 0.5);
    float rejilla = 0.08 * (1.0 - smoothstep(0.0, 0.04, min(g.x, g.y)));
    vec3 col = color.rgb * (linea * 1.4 + rejilla) + vec3(0.0, 0.02, 0.01);
    gl_FragColor = vec4(col, 1.0);
}
