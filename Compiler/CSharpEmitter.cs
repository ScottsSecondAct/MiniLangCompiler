using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;

// CSharpEmitter is the final phase of the pipeline.
// It wraps the generated C# statements in a complete Program.Main() shell,
// then compiles the result to a .dll using Roslyn.

public static class CSharpEmitter
{
  public static void EmitExecutable(string generatedCode, string outputPath)
  {
    // Wrap the generated statements in a minimal C# program template
    var fullCode = $@"
using System;

public class Program
{{
    public static void Main()
    {{
        {generatedCode}
    }}
}}";

    // Parse and normalize whitespace so the printed output is readable
    var syntaxTree = CSharpSyntaxTree.ParseText(fullCode);
    var root = syntaxTree.GetRoot().NormalizeWhitespace();
    Console.WriteLine("Generated C# code:");
    Console.WriteLine(new string('-', 30));
    Console.WriteLine(root.ToFullString());
    Console.WriteLine(new string('-', 30));

    // Reference the minimum set of .NET assemblies required to compile and run the output
    List<MetadataReference> references = new List<MetadataReference>()
    {
      MetadataReference.CreateFromFile(Assembly.Load("System.Private.CoreLib").Location),
      MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
      MetadataReference.CreateFromFile(Assembly.Load("System.Console").Location)
    };

    var compilation = CSharpCompilation.Create(
        Path.GetFileNameWithoutExtension(outputPath),
        syntaxTrees: new[] { syntaxTree },
        references: references,
        options: new CSharpCompilationOptions(OutputKind.ConsoleApplication)
    );

    // Write the compiled assembly to Output/<name>.dll
    Directory.CreateDirectory("Output");
    string fileName = String.Concat(Path.GetFileNameWithoutExtension(outputPath), ".dll");
    using FileStream stream = new FileStream(Path.Combine("Output", fileName), FileMode.Create);

    var result = compilation.Emit(stream);
    if (result.Success)
    {
      Console.WriteLine($"Compilation succeeded. Output: {fileName}");
    }
    else
    {
      // Print errors in red and warnings in yellow
      Console.WriteLine("Compilation failed:");
      foreach (var diagnostic in result.Diagnostics)
      {
        Console.ForegroundColor = diagnostic.Severity == DiagnosticSeverity.Error
            ? ConsoleColor.Red
            : ConsoleColor.Yellow;
        Console.WriteLine($"{diagnostic.Location.GetLineSpan()}: {diagnostic.Severity} {diagnostic.Id}: {diagnostic.GetMessage()}");
        Console.ResetColor();
      }
    }
  }
}
