/*{
  "ISFVSN": "2",
  "DESCRIPTION": "Caleidoscopio de simetría radial sobre un patrón generativo.",
  "CREDIT": "Raúl Hernández Herce",
  "CATEGORIES": ["Generador", "Simetría"],
  "INPUTS": [
    { "NAME": "segmentos", "TYPE": "float", "DEFAULT": 8.0, "MIN": 2.0, "MAX": 24.0 },
    { "NAME": "giro", "TYPE": "float", "DEFAULT": 0.2, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "zoom", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.5, "MAX": 10.0 }
  ]
}*/
void main() {
    vec2 p = (gl_FragCoord.xy - 0.5 * RENDERSIZE) / RENDERSIZE.y;
    float a = atan(p.y, p.x) + TIME * giro;
    float r = length(p);
    float seg = 6.28318 / floor(segmentos);
    a = abs(mod(a, seg) - seg * 0.5);
    vec2 q = vec2(cos(a), sin(a)) * r * zoom;
    float v = sin(q.x * 3.0 + TIME) * cos(q.y * 4.0 - TIME * 0.7) + sin(r * 10.0 - TIME * 2.0);
    vec3 col = 0.5 + 0.5 * cos(v * 2.0 + vec3(0.0, 1.0, 2.0) + TIME * 0.3);
    gl_FragColor = vec4(col * (1.0 - smoothstep(0.2, 1.2, r)), 1.0);
}
