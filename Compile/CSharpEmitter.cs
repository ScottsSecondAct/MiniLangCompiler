using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

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

    var tree = CSharpSyntaxTree.ParseText(fullCode);

    var compilation = CSharpCompilation.Create(
        Path.GetFileNameWithoutExtension(outputPath),
        new[] { tree },
        new[] {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location)
        },
        new CSharpCompilationOptions(OutputKind.ConsoleApplication)
    );

    // 3. Emit the Assembly
    var result = compilation.Emit(outputPath);
    if (result.Success)
    {
      Console.WriteLine($"Compilation succeeded. Output: {outputPath}");
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



