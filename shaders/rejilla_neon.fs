/*{
  "ISFVSN": "2",
  "DESCRIPTION": "Suelo de rejilla neón en perspectiva con sol de horizonte, estética synthwave.",
  "CREDIT": "Raúl Hernández Herce",
  "CATEGORIES": ["Generador", "Retro"],
  "INPUTS": [
    { "NAME": "velocidad", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "lineas", "TYPE": "color", "DEFAULT": [1.0, 0.2, 0.8, 1.0] },
    { "NAME": "sol", "TYPE": "color", "DEFAULT": [1.0, 0.6, 0.1, 1.0] }
  ]
}*/
void main() {
    vec2 uv = isf_FragNormCoord;
    vec3 col = mix(vec3(0.05, 0.0, 0.12), vec3(0.25, 0.0, 0.3), uv.y);
    float horizonte = 0.45;
    if (uv.y < horizonte) {
        float d = horizonte - uv.y;
        float z = 0.3 / d;
        float x = (uv.x - 0.5) * z * 2.0;
        float gz = abs(fract(z - TIME * velocidad) - 0.5);
        float gx = abs(fract(x) - 0.5);
        float g = max(1.0 - smoothstep(0.0, 0.05 * z, gz), 1.0 - smoothstep(0.0, 0.03 * z, gx));
        col = mix(vec3(0.02, 0.0, 0.05), lineas.rgb, g * smoothstep(0.0, 0.1, d));
    } else {
        vec2 c = vec2(0.5, horizonte + 0.18);
        float r = length((uv - c) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0));
        float franjas = step(0.5, fract((uv.y - horizonte) * 40.0)) + step(uv.y, c.y);
        col += sol.rgb * (1.0 - smoothstep(0.16, 0.17, r)) * min(franjas, 1.0);
    }
    gl_FragColor = vec4(col, 1.0);
}
