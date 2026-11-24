/*{
  "ISFVSN": "2",
  "DESCRIPTION": "Anillos concéntricos que laten al tempo. Conecta 'pulso' a la envolvente de audio o al BPM.",
  "CREDIT": "Raúl Hernández Herce",
  "CATEGORIES": ["Generador", "Audio"],
  "INPUTS": [
    { "NAME": "bpm", "TYPE": "float", "DEFAULT": 120.0, "MIN": 60.0, "MAX": 200.0 },
    { "NAME": "pulso", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "color", "TYPE": "color", "DEFAULT": [0.22, 1.0, 0.53, 1.0] }
  ]
}*/
void main() {
    vec2 p = (gl_FragCoord.xy - 0.5 * RENDERSIZE) / RENDERSIZE.y;
    float r = length(p);
    float beat = fract(TIME * bpm / 60.0);
    float golpe = max(exp(-beat * 6.0), pulso);
    float onda = sin(r * 40.0 - TIME * 6.0) * 0.5 + 0.5;
    float aro = (1.0 - smoothstep(0.0, 0.02, abs(r - beat * 0.7)));
    vec3 col = color.rgb * (onda * 0.25 + aro * 1.5) * (0.4 + golpe);
    gl_FragColor = vec4(col * (1.0 - smoothstep(0.1, 0.9, r)), 1.0);
}
