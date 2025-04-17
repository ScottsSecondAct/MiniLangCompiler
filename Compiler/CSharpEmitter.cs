using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;

public static class CSharpEmitter
{
  public static void EmitExecutable(string generatedCode, string outputPath)
  {
    var fullCode = $@"
using System;

public class Program
{{
    public static void Main()
    {{
        {generatedCode}
    }}
}}";

    var syntaxTree = CSharpSyntaxTree.ParseText(fullCode);
    var root = syntaxTree.GetRoot().NormalizeWhitespace();
    Console.WriteLine("Generated C# code:");
    Console.WriteLine(new string('-', 30));
    Console.WriteLine(root.ToFullString());
    Console.WriteLine(new string('-', 30));

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

    // 3. Emit the Assembly
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



