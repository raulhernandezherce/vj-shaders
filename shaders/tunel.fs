/*{
  "ISFVSN": "2",
  "DESCRIPTION": "Túnel infinito con anillos que avanzan hacia la cámara.",
  "CREDIT": "Raúl Hernández Herce",
  "CATEGORIES": ["Generador", "3D"],
  "INPUTS": [
    { "NAME": "velocidad", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "anillos", "TYPE": "float", "DEFAULT": 8.0, "MIN": 2.0, "MAX": 32.0 },
    { "NAME": "color", "TYPE": "color", "DEFAULT": [0.95, 0.65, 0.14, 1.0] }
  ]
}*/
void main() {
    vec2 p = (gl_FragCoord.xy - 0.5 * RENDERSIZE) / RENDERSIZE.y;
    float r = length(p) + 0.0001;
    float a = atan(p.y, p.x);
    float z = 1.0 / r + TIME * velocidad;
    float aro = smoothstep(0.35, 0.5, abs(fract(z * anillos * 0.25) - 0.5));
    float rayos = 0.5 + 0.5 * sin(a * 12.0 + TIME);
    vec3 col = color.rgb * aro * (0.6 + 0.4 * rayos) * smoothstep(0.0, 0.6, r);
    gl_FragColor = vec4(col, 1.0);
}
