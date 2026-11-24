#!/usr/bin/env python3
"""Copia los shaders de shaders/ a docs/shaders.js para la galería de GitHub Pages."""
import json
from pathlib import Path

RAIZ = Path(__file__).resolve().parents[1]


def generar():
    datos = [{"id": f.stem, "fuente": f.read_text(encoding="utf-8")} for f in sorted((RAIZ / "shaders").glob("*.fs"))]
    return "// Generado por tools/construir_galeria.py — no editar a mano.\nwindow.SHADERS = " + \
        json.dumps(datos, ensure_ascii=False, indent=1) + ";\n"


if __name__ == "__main__":
    (RAIZ / "docs" / "shaders.js").write_text(generar(), encoding="utf-8")
    print("docs/shaders.js actualizado")
