/*{
  "ISFVSN": "2",
  "DESCRIPTION": "Plasma clásico de demoscene: suma de ondas senoidales con paleta cíclica.",
  "CREDIT": "Raúl Hernández Herce",
  "CATEGORIES": ["Generador", "Retro"],
  "INPUTS": [
    { "NAME": "velocidad", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "escala", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 20.0 }
  ]
}*/
void main() {
    vec2 uv = isf_FragNormCoord * escala;
    float t = TIME * velocidad;
    float v = sin(uv.x + t) + sin(uv.y + t * 1.3) + sin(uv.x + uv.y + t * 0.7)
            + sin(length(uv - escala * 0.5) * 1.5 - t);
    vec3 col = 0.5 + 0.5 * cos(3.14159 * v + vec3(0.0, 2.0, 4.0));
    gl_FragColor = vec4(col, 1.0);
}
