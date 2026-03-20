# MiniLangCompiler
[![Open Source](https://img.shields.io/badge/Open%20Source-Yes-green.svg)](https://github.com/ScottsSecondAct/MiniLangCompiler) [![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE) ![AI Assisted](https://img.shields.io/badge/AI%20Assisted-Claude-blue?logo=anthropic)

Just noodling around with [ANTLR4](https://www.antlr.org/) and Roslyn. This is a toy compiler for a small made-up language (MiniLang) that compiles to a .NET assembly.

## Pipeline

```
.minilang source
  → ANTLR4 Lexer/Parser     tokenize and parse
  → AstBuilder              parse tree → typed AST
  → CSharpCodeGen           AST → C# source
  → Roslyn (CSharpEmitter)  C# source → .dll
```

## Language

```
int x = 5;
int y = x + 10;

if (y > 10) {
  print(y);
}

int count = 3;
while (count > 0) {
  print(count);
  count = count - 1;
}
```

**Types:** `int`, `float`, `bool`, `string`

**Operators:** `+`, `-`, `*`, `/`, `==`, `!=`, `<`, `>`, `<=`, `>=`

**Built-in:** `print(expr)` — writes to stdout

## Usage

```bash
dotnet run -- <file.minilang> [-o <output-name>]
```

Output DLL is written to `Output/`. To run it:

```bash
dotnet exec Output/<output-name>.dll
```

> Note: `Output/<output-name>.runtimeconfig.json` must exist. See `Output/test.runtimeconfig.json` for an example.

## Regenerating the Lexer/Parser

If you modify `MiniLang.g4`, regenerate the ANTLR4 files with:

```bash
antlr4 -Dlanguage=CSharp -o ./Compiler -visitor MiniLang.g4
```

## Dependencies

- [ANTLR4](https://www.antlr.org/) — lexer/parser generation (`Antlr4.Runtime.Standard` v4.13.1)
- [Roslyn](https://github.com/dotnet/roslyn) — C# compilation (`Microsoft.CodeAnalysis.CSharp` v4.13.0)
- .NET 9.0
