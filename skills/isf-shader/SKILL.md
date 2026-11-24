---
name: isf-shader
description: Crea, corrige o adapta shaders ISF (Interactive Shader Format) para software de VJ y visuales en directo como VJ Studio, Resolume, VDMX, MadMapper o Synesthesia. Úsala cuando el usuario pida un efecto visual, un generador, un "shader para VJ", un archivo .fs, o quiera convertir un shader de Shadertoy a ISF.
---

# Shaders ISF para VJ

Un shader ISF es un *fragment shader* GLSL con una cabecera JSON que declara sus parámetros. El software de VJ convierte esa cabecera en deslizadores, selectores de color y botones que se pueden controlar con MIDI o con el audio.

## Plantilla

```glsl
/*{
  "ISFVSN": "2",
  "DESCRIPTION": "Qué hace, en una frase.",
  "CREDIT": "Autor",
  "CATEGORIES": ["Generador"],
  "INPUTS": [
    { "NAME": "velocidad", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "color", "TYPE": "color", "DEFAULT": [1.0, 0.6, 0.1, 1.0] }
  ]
}*/
void main() {
    vec2 uv = isf_FragNormCoord;                                     // 0..1
    vec2 p = (gl_FragCoord.xy - 0.5 * RENDERSIZE) / RENDERSIZE.y;    // centrado, sin deformar
    float v = 0.5 + 0.5 * sin(length(p) * 20.0 - TIME * velocidad);
    gl_FragColor = vec4(color.rgb * v, 1.0);
}
```

- Variables del host: `TIME` (s), `TIMEDELTA`, `FRAMEINDEX`, `RENDERSIZE` (px), `isf_FragNormCoord` (0..1).
- Tipos de entrada: `float`, `long`, `bool`, `color` (vec4), `point2D` (vec2), `event` y, para efectos, `image`.
- **Efecto** en lugar de generador: declara `{ "NAME": "inputImage", "TYPE": "image" }` y lee la imagen con `IMG_NORM_PIXEL(inputImage, uv)` o `IMG_THIS_PIXEL(inputImage)`, nunca con `texture2D`.

## Cómo trabajar

1. **Pregunta lo justo**: generador o efecto, estética (referencias, colores) y qué debe poder controlar el VJ en directo. Si el usuario no lo especifica, expón **2-4 parámetros** con nombres claros y rangos útiles. Mejor pocos y expresivos que muchos.
2. **Pensado para directo**:
   - Que se vea bien con los valores por defecto, que todo el rango de cada deslizador sea usable y que no haya saltos bruscos.
   - Usa `p` centrado y dividido por `RENDERSIZE.y` para que funcione en 16:9, en 4:3 o en un slice estrecho.
   - Si tiene ritmo, expón `bpm` o un `pulso` (0..1) que el VJ pueda enlazar al audio.
3. **Compatible con WebGL y con cualquier GPU**:
   - Los bucles `for` necesitan un límite constante (`i < 6`, no `i < n`).
   - `smoothstep(a, b, x)` exige `a < b`. Para invertirlo, usa `1.0 - smoothstep(b, a, x)`.
   - `pow(x, y)` no está definido con `x < 0`. Para un cuadrado, `x * x`.
   - Escribe los literales con decimal (`1.0`, no `1`): GLSL ES no convierte de `int` a `float`.
4. **Valida** antes de entregar:

   ```bash
   python3 scripts/validar_isf.py shader.fs
   ```

   Corrige todos los errores y explica los avisos que dejes.
5. **Entrega** el `.fs` completo y una línea por parámetro: qué hace y un uso en directo (por ejemplo, «`pulso`: enlázalo al bombo»).

## De Shadertoy a ISF

| Shadertoy | ISF |
|---|---|
| `mainImage(out vec4 fragColor, in vec2 fragCoord)` | `void main()` con `gl_FragColor` y `gl_FragCoord` |
| `iTime` | `TIME` |
| `iResolution.xy` | `RENDERSIZE` |
| `iChannel0` + `texture()` | entrada `image` + `IMG_NORM_PIXEL()` |
| `iMouse` | entrada `point2D` |

Recuerda al usuario que respete la licencia del shader original (muchos de Shadertoy son CC BY-NC-SA) y que cite al autor en `CREDIT`.

## Ejemplos

En [`shaders/`](../../shaders) hay 12 generadores validados para tomar como referencia: plasma, túnel, rejilla synthwave, caleidoscopio, ondas al BPM, lluvia digital, metabolas, Voronoi, estrellas, glitch, humo fBm y ECG.
