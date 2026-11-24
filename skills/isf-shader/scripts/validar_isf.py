#!/usr/bin/env python3
"""Valida shaders ISF (Interactive Shader Format) antes de cargarlos en un software de VJ.

    python3 validar_isf.py shader.fs [otro.fs ...]

Comprueba la cabecera JSON, los tipos y rangos de las entradas, que cada entrada se use, y
errores de GLSL que compilan en una GPU pero fallan o se ven distintos en otra.
Código de salida 1 si hay algún error. Sin dependencias.
"""
import json
import re
import sys
from pathlib import Path

TIPOS = {"float", "bool", "long", "color", "point2D", "event", "image", "audio", "audioFFT"}
RESERVADAS = {"TIME", "RENDERSIZE", "TIMEDELTA", "FRAMEINDEX", "DATE", "PASSINDEX", "main"}


def separar(texto):
    m = re.match(r"\s*/\*\s*(\{.*?\})\s*\*/", texto, re.S)
    if not m:
        return None, texto
    return m.group(1), texto[m.end():]


def sin_comentarios(codigo):
    codigo = re.sub(r"/\*.*?\*/", " ", codigo, flags=re.S)
    return re.sub(r"//[^\n]*", " ", codigo)


def validar(texto):
    errores, avisos = [], []
    cabecera, cuerpo = separar(texto)
    if cabecera is None:
        return ["Falta la cabecera JSON /*{ ... }*/ al principio del archivo."], avisos, None
    try:
        meta = json.loads(cabecera)
    except json.JSONDecodeError as e:
        return [f"La cabecera no es JSON válido: {e}"], avisos, None

    if str(meta.get("ISFVSN", "")) != "2":
        avisos.append('Añade "ISFVSN": "2" para que el host sepa qué versión de ISF usas.')
    if not meta.get("DESCRIPTION"):
        avisos.append("Sin DESCRIPTION: los navegadores de efectos la muestran como ayuda.")

    codigo = sin_comentarios(cuerpo)
    nombres = set()
    for i, e in enumerate(meta.get("INPUTS", [])):
        nombre, tipo = e.get("NAME"), e.get("TYPE")
        donde = f"Entrada {i} ({nombre})"
        if not nombre or not re.fullmatch(r"[A-Za-z_]\w*", nombre):
            errores.append(f"{donde}: NAME debe ser un identificador GLSL válido.")
            continue
        if nombre in nombres:
            errores.append(f"{donde}: NAME repetido.")
        if nombre in RESERVADAS or nombre.startswith(("gl_", "isf_", "IMG_")):
            errores.append(f"{donde}: '{nombre}' es un nombre reservado.")
        nombres.add(nombre)
        if tipo not in TIPOS:
            errores.append(f"{donde}: TYPE '{tipo}' no existe en ISF.")
            continue
        if tipo in ("float", "long"):
            lo, hi, d = e.get("MIN"), e.get("MAX"), e.get("DEFAULT")
            if lo is not None and hi is not None and lo >= hi:
                errores.append(f"{donde}: MIN debe ser menor que MAX.")
            elif None not in (lo, hi, d) and not lo <= d <= hi:
                errores.append(f"{donde}: DEFAULT {d} fuera de [{lo}, {hi}].")
            if lo is None or hi is None:
                avisos.append(f"{donde}: sin MIN/MAX, el deslizador del host no tendrá rango útil.")
        if tipo == "color":
            d = e.get("DEFAULT")
            if d is not None and (len(d) != 4 or not all(0 <= c <= 1 for c in d)):
                errores.append(f"{donde}: un color es [r, g, b, a] con valores entre 0 y 1.")
        if not re.search(rf"\b{re.escape(nombre)}\b", codigo):
            avisos.append(f"{donde}: se declara pero no se usa.")

    if not re.search(r"\bvoid\s+main\s*\(", codigo):
        errores.append("Falta void main().")
    if "gl_FragColor" not in codigo:
        errores.append("main() debe escribir gl_FragColor.")

    for m in re.finditer(r"smoothstep\(\s*(-?[\d.]+)\s*,\s*(-?[\d.]+)\s*,", codigo):
        if float(m.group(1)) >= float(m.group(2)):
            errores.append(f"smoothstep({m.group(1)}, {m.group(2)}, x): con edge0 >= edge1 el resultado no "
                           f"está definido en GLSL. Usa 1.0 - smoothstep({m.group(2)}, {m.group(1)}, x).")
    for m in re.finditer(r"for\s*\(([^;]*);([^;]*);", codigo):
        if not re.fullmatch(r"\s*\w+\s*(<|<=|>|>=)\s*-?\d+(\.\d+)?\s*", m.group(2)):
            errores.append(f"Bucle 'for ({m.group(1).strip()}; {m.group(2).strip()}; ...)': en GLSL ES / WebGL "
                           "el límite debe ser una constante.")
    if re.search(r"\btexture2D\s*\(", codigo):
        avisos.append("Usa IMG_NORM_PIXEL / IMG_PIXEL en lugar de texture2D: el host gestiona las coordenadas.")
    return errores, avisos, meta


def main():
    if len(sys.argv) < 2:
        sys.exit(__doc__)
    hay_error = False
    for ruta in sys.argv[1:]:
        errores, avisos, _ = validar(Path(ruta).read_text(encoding="utf-8"))
        estado = "❌" if errores else ("⚠️ " if avisos else "✅")
        print(f"{estado} {ruta}")
        for e in errores:
            print(f"   error: {e}")
        for a in avisos:
            print(f"   aviso: {a}")
        hay_error |= bool(errores)
    sys.exit(1 if hay_error else 0)


if __name__ == "__main__":
    main()
