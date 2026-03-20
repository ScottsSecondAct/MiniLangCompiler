using Antlr4.Runtime;

string? inputFile = null;
string outputFile = "no_file_name_provided.dll";
bool showHelp = false;

// --- Parse command-line arguments ---
for (int i = 0; i < args.Length; i++)
{
  switch (args[i])
  {
    case "--help":
    case "-h":
      showHelp = true;
      break;
    case "--out":
    case "-o":
      // Consume the next argument as the output file name
      if (i + 1 < args.Length)
        outputFile = args[++i];
      else
        Console.WriteLine("Missing output file name after --out");
      break;
    default:
      if (args[i].EndsWith(".minilang"))
        inputFile = args[i];
      else
        Console.WriteLine($"Unknown argument: {args[i]}");
      break;
  }
}

if (showHelp)
{
  Console.WriteLine("""
        MiniLangCompiler - A minimal compiler for the MiniLang language

        Usage:
          dotnet run -- [<file.minilang>] [--out <output.exe>] [--run] [--debug]

        Options:
          -o, --out       Set output file name (default: MiniLangApp.exe)
          -h, --help      Show this help message

        Notes:
          If no file is passed, source is read from standard input (Ctrl+D to end)
        """);
  return;
}

// --- Read source code ---
string source;
if (inputFile != null)
{
  if (!File.Exists(inputFile))
  {
    Console.WriteLine($"Error: File not found: {inputFile}");
    return;
  }
  source = File.ReadAllText(inputFile);
}
else
{
  // Fall back to stdin when no file argument is provided
  Console.WriteLine("Reading MiniLang from stdin. Press Ctrl+D (Linux/macOS) or Ctrl+Z (Windows) to finish.");
  source = Console.In.ReadToEnd();
}

// --- Lex & Parse ---
// Feed source into ANTLR to produce a parse tree
var inputStream = new AntlrInputStream(source);
var lexer = new MiniLangLexer(inputStream);
var tokens = new CommonTokenStream(lexer);
var parser = new MiniLangParser(tokens);

tokens.Reset();
var tree = parser.program();

// --- Build AST ---
// Convert the ANTLR parse tree into a typed, immutable AST
var ast = (ProgramNode)new AstBuilder().Visit(tree);

// --- Generate C# from AST ---
// Walk the AST and emit equivalent C# source code
var generatedCode = new CSharpCodeGen().Generate(ast);

// --- Emit executable ---
// Compile the C# source to a .dll via Roslyn
CSharpEmitter.EmitExecutable(generatedCode, outputFile);
