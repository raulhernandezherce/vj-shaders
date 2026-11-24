/*{
  "ISFVSN": "2",
  "DESCRIPTION": "Celdas de Voronoi en movimiento con bordes luminosos, como un tejido vivo.",
  "CREDIT": "Raúl Hernández Herce",
  "CATEGORIES": ["Generador", "Orgánico"],
  "INPUTS": [
    { "NAME": "escala", "TYPE": "float", "DEFAULT": 6.0, "MIN": 2.0, "MAX": 20.0 },
    { "NAME": "velocidad", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "borde", "TYPE": "color", "DEFAULT": [0.3, 0.8, 1.0, 1.0] }
  ]
}*/
vec2 azar2(vec2 p) {
    p = vec2(dot(p, vec2(127.1, 311.7)), dot(p, vec2(269.5, 183.3)));
    return fract(sin(p) * 43758.5453);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.y * escala;
    vec2 i = floor(uv), f = fract(uv);
    float d1 = 8.0, d2 = 8.0;
    for (int y = -1; y <= 1; y++) {
        for (int x = -1; x <= 1; x++) {
            vec2 g = vec2(float(x), float(y));
            vec2 o = 0.5 + 0.5 * sin(TIME * velocidad + 6.2831 * azar2(i + g));
            float d = length(g + o - f);
            if (d < d1) { d2 = d1; d1 = d; } else if (d < d2) { d2 = d; }
        }
    }
    float b = 1.0 - smoothstep(0.0, 0.08, d2 - d1);
    vec3 col = vec3(0.02, 0.03, 0.08) + borde.rgb * b + 0.15 * borde.rgb * (1.0 - d1);
    gl_FragColor = vec4(col, 1.0);
}
