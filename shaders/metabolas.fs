/*{
  "ISFVSN": "2",
  "DESCRIPTION": "Metabolas: gotas que se funden al acercarse, con borde nítido.",
  "CREDIT": "Raúl Hernández Herce",
  "CATEGORIES": ["Generador", "Orgánico"],
  "INPUTS": [
    { "NAME": "velocidad", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "umbral", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.3, "MAX": 3.0 },
    { "NAME": "fondo", "TYPE": "color", "DEFAULT": [0.04, 0.02, 0.1, 1.0] },
    { "NAME": "gota", "TYPE": "color", "DEFAULT": [1.0, 0.3, 0.45, 1.0] }
  ]
}*/
void main() {
    vec2 p = (gl_FragCoord.xy - 0.5 * RENDERSIZE) / RENDERSIZE.y;
    float t = TIME * velocidad;
    float campo = 0.0;
    for (int i = 0; i < 6; i++) {
        float k = float(i);
        vec2 c = 0.35 * vec2(sin(t * (0.7 + k * 0.13) + k * 2.1), cos(t * (0.9 + k * 0.11) + k * 1.3));
        campo += 0.012 / dot(p - c, p - c);
    }
    float borde = smoothstep(umbral * 0.95, umbral, campo);
    vec3 col = mix(fondo.rgb, gota.rgb * (0.7 + 0.3 * min(campo / umbral - 1.0, 1.0)), borde);
    gl_FragColor = vec4(col, 1.0);
}
