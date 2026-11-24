/*{
  "ISFVSN": "2",
  "DESCRIPTION": "Viaje a través de un campo de estrellas en varias capas de profundidad.",
  "CREDIT": "Raúl Hernández Herce",
  "CATEGORIES": ["Generador", "Espacio"],
  "INPUTS": [
    { "NAME": "velocidad", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "densidad", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.5, "MAX": 0.99 }
  ]
}*/
float azar(vec2 p) { return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453); }

void main() {
    vec2 p = (gl_FragCoord.xy - 0.5 * RENDERSIZE) / RENDERSIZE.y;
    vec3 col = vec3(0.0);
    for (int i = 0; i < 4; i++) {
        float capa = float(i);
        float z = fract(capa * 0.25 + TIME * velocidad * 0.1);
        float escala = mix(20.0, 0.5, z);
        vec2 q = p * escala + capa * 17.0;
        vec2 id = floor(q), f = fract(q) - 0.5;
        float s = step(densidad, azar(id));
        float brillo = s * (1.0 - smoothstep(0.0, 0.08, length(f))) * smoothstep(0.0, 0.3, z) * (1.0 - smoothstep(0.8, 1.0, z));
        col += brillo * mix(vec3(0.6, 0.7, 1.0), vec3(1.0, 0.9, 0.7), azar(id + 3.0));
    }
    gl_FragColor = vec4(col, 1.0);
}
