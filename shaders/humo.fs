/*{
  "ISFVSN": "2",
  "DESCRIPTION": "Humo y nubes con ruido fractal (fBm) deformado sobre sí mismo.",
  "CREDIT": "Raúl Hernández Herce",
  "CATEGORIES": ["Generador", "Orgánico"],
  "INPUTS": [
    { "NAME": "velocidad", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "color_a", "TYPE": "color", "DEFAULT": [0.1, 0.05, 0.3, 1.0] },
    { "NAME": "color_b", "TYPE": "color", "DEFAULT": [1.0, 0.55, 0.3, 1.0] }
  ]
}*/
float azar(vec2 p) { return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453); }

float ruido(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(azar(i), azar(i + vec2(1.0, 0.0)), u.x),
               mix(azar(i + vec2(0.0, 1.0)), azar(i + vec2(1.0, 1.0)), u.x), u.y);
}

float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) { v += a * ruido(p); p = p * 2.02 + 3.1; a *= 0.5; }
    return v;
}

void main() {
    vec2 p = gl_FragCoord.xy / RENDERSIZE.y * 3.0;
    float t = TIME * velocidad;
    vec2 q = vec2(fbm(p + t), fbm(p + vec2(5.2, 1.3) - t));
    float v = fbm(p + 4.0 * q);
    vec3 col = mix(color_a.rgb, color_b.rgb, smoothstep(0.3, 0.8, v));
    col += 0.15 * vec3(q.x, q.y * 0.5, 0.0);
    gl_FragColor = vec4(col, 1.0);
}
