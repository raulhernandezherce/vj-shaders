<div align="center">

# 🌈 vj-shaders

**Efectos visuales para VJ en formato ISF que funcionan en vivo en el navegador, y una skill para que Claude cree los tuyos.**

### [▶ Ver la galería en vivo](https://raulhernandezherce.github.io/vj-shaders/)

[![galería](docs/captura.png)](https://raulhernandezherce.github.io/vj-shaders/)

![tests](https://github.com/raulhernandezherce/vj-shaders/actions/workflows/tests.yml/badge.svg)
![ISF](https://img.shields.io/badge/ISF-v2-F5A623)
![WebGL](https://img.shields.io/badge/WebGL-990000?logo=webgl&logoColor=white)
![Claude Code plugin](https://img.shields.io/badge/Claude_Code-plugin-D97757)

</div>

---

## Qué hay

- **12 generadores ISF** listos para el directo: plasma, túnel, rejilla synthwave, caleidoscopio, ondas al BPM, lluvia digital, metabolas, Voronoi, estrellas, glitch, humo fBm y un monitor de ECG. Cada uno expone 2-4 parámetros pensados para controlarlos con MIDI o con el audio.
- **Una galería web** que los ejecuta en tiempo real con un intérprete ISF de 100 líneas en WebGL. Al abrir un efecto puedes mover sus parámetros, ver el código y descargarlo.
- **Una skill para Claude** que crea shaders nuevos a partir de una descripción, los adapta desde Shadertoy y los valida antes de entregarlos.
- **Un validador** que detecta los errores que compilan en una GPU y fallan en otra.

Funcionan en [VJ Studio](https://github.com/raulhernandezherce/vj-studio) y en cualquier software compatible con ISF (Resolume, VDMX, MadMapper, Synesthesia…): descarga el `.fs` y arrástralo a su carpeta de efectos.

## Crea los tuyos con Claude

```
/plugin marketplace add raulhernandezherce/vj-shaders
```

```
/plugin install isf-shaders@vj-shaders
```

Y pide, por ejemplo:

> *«Un generador de anillos de neón que giren con el BPM, con un parámetro para el número de anillos y otro para el color.»*

La skill ([`SKILL.md`](skills/isf-shader/SKILL.md)) sigue un método pensado para el directo: pocos parámetros con rangos útiles, que funcione en cualquier proporción de pantalla y que sea compatible con WebGL. Valida el resultado antes de entregarlo.

## El validador

```bash
python3 skills/isf-shader/scripts/validar_isf.py mi_efecto.fs
```

| Comprueba | Por qué importa |
|---|---|
| Cabecera JSON, tipos y nombres reservados | El host no carga el efecto si falla |
| `DEFAULT` dentro de `[MIN, MAX]` y colores en 0..1 | Deslizadores que arrancan fuera de rango |
| Entradas declaradas pero sin usar | Controles que no hacen nada en directo |
| `smoothstep` con los bordes al revés | Indefinido en GLSL: cada GPU da un resultado distinto |
| Bucles con límite no constante | No compilan en WebGL ni en GLSL ES |
| `texture2D` en lugar de `IMG_NORM_PIXEL` | Coordenadas erróneas en el host |

## Añadir un shader

1. Crea `shaders/nombre.fs` y valídalo.
2. `python3 tools/construir_galeria.py`
3. `python3 -m unittest discover -s tests -v`

---

<sub>Hecho por <a href="https://github.com/raulhernandezherce">Raúl Hernández Herce</a> · Licencia MIT</sub>
