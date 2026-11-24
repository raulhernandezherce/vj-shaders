import json
import re
import subprocess
import sys
import unittest
from pathlib import Path

RAIZ = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(RAIZ / "skills" / "isf-shader" / "scripts"))
from validar_isf import validar  # noqa: E402

SHADERS = sorted((RAIZ / "shaders").glob("*.fs"))


class ShadersTest(unittest.TestCase):
    def test_todos_los_shaders_son_validos_y_sin_avisos(self):
        self.assertGreaterEqual(len(SHADERS), 12)
        for f in SHADERS:
            errores, avisos, meta = validar(f.read_text(encoding="utf-8"))
            self.assertEqual((errores, avisos), ([], []), f.name)
            self.assertTrue(meta["DESCRIPTION"] and meta["CATEGORIES"], f.name)


class ValidadorTest(unittest.TestCase):
    BASE = '/*{"ISFVSN":"2","DESCRIPTION":"x","INPUTS":[%s]}*/\nvoid main(){ %s gl_FragColor = vec4(1.0); }'

    def err(self, inputs="", cuerpo=""):
        return validar(self.BASE % (inputs, cuerpo))

    def test_detecta_errores_tipicos(self):
        self.assertIn("cabecera", validar("void main(){}")[0][0])
        self.assertIn("JSON", validar("/*{ roto }*/ void main(){}")[0][0])
        self.assertTrue(self.err('{"NAME":"a","TYPE":"float","DEFAULT":5.0,"MIN":0.0,"MAX":1.0}', "float x = a;")[0])
        self.assertTrue(self.err('{"NAME":"a","TYPE":"color","DEFAULT":[2.0,0.0,0.0,1.0]}', "vec4 c = a;")[0])
        self.assertTrue(self.err('{"NAME":"TIME","TYPE":"float"}', "")[0])
        self.assertTrue(self.err('{"NAME":"a","TYPE":"vec3"}', "")[0])
        self.assertTrue(self.err("", "float s = smoothstep(1.0, 0.0, 0.5);")[0])
        self.assertTrue(self.err("", "int n = 3; for (int i = 0; i < n; i++) {}")[0])

    def test_avisos(self):
        e, a, _ = self.err('{"NAME":"sin_uso","TYPE":"float","DEFAULT":0.5,"MIN":0.0,"MAX":1.0}')
        self.assertEqual(e, [])
        self.assertTrue(any("no se usa" in x for x in a))

    def test_cli(self):
        r = subprocess.run([sys.executable, str(RAIZ / "skills/isf-shader/scripts/validar_isf.py"), *map(str, SHADERS)],
                           capture_output=True, text=True)
        self.assertEqual(r.returncode, 0, r.stdout)


class PluginTest(unittest.TestCase):
    def test_manifiestos_y_skill(self):
        plugin = json.loads((RAIZ / ".claude-plugin/plugin.json").read_text())
        market = json.loads((RAIZ / ".claude-plugin/marketplace.json").read_text())
        self.assertEqual(market["plugins"][0]["name"], plugin["name"])
        skill = RAIZ / "skills/isf-shader/SKILL.md"
        fm = re.match(r"^---\n(.*?)\n---\n", skill.read_text(encoding="utf-8"), re.S).group(1)
        self.assertIn("name: isf-shader", fm)


if __name__ == "__main__":
    unittest.main()
