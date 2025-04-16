using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

public static class CSharpEmitter
{
  private static string IndentCode(string code, int spaces = 4)
  {
    var indent = new string(' ', spaces);
    return string.Join(Environment.NewLine,
        code.Split('\n').Select(line => indent + line.TrimEnd()));
  }

  private static string GetAssemblyPath(string assemblyName)
  {
    Assembly loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);
    if (loadedAssembly != null)
    {
      return loadedAssembly.Location;
    }

    string runtimeDir = RuntimeEnvironment.GetRuntimeDirectory();
    if (!string.IsNullOrEmpty(runtimeDir))
    {
      string assemblyPath = Path.Combine(runtimeDir, assemblyName + ".dll");
      if (File.Exists(assemblyPath))
      {
        return assemblyPath;
      }
    }

    // As a last resort, check the current directory.
    string currentDir = Directory.GetCurrentDirectory();
    string localAssemblyPath = Path.Combine(currentDir, assemblyName + ".dll");
    if (File.Exists(localAssemblyPath))
    {
      return localAssemblyPath;
    }

    return null;
  }

  public static bool EmitExecutable(
      string generatedCode,
      string outputPath,
      LanguageVersion languageVersion = LanguageVersion.Latest,
      OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary,
      Platform platform = Platform.AnyCpu,
      ImmutableArray<PortableExecutableReference> additionalReferences = default)
  {
    var fullCode = $@"
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

public class Program
{{
    public static void Main()
    {{
        try
        {{
            // Set the base path to help resolve assemblies
            string basePath = AppContext.BaseDirectory;
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {{
                string assemblyName = args.Name.Split(',')[0];
                string assemblyPath = Path.Combine(basePath, assemblyName + "".dll"");
                if (File.Exists(assemblyPath))
                {{
                    try
                    {{
                       return Assembly.LoadFrom(assemblyPath);
                    }}
                    catch(Exception ex)
                    {{
                        Console.WriteLine($""Error loading assembly {{assemblyPath}}: {{ex.Message}}"");
                        return null;
                    }}
                }}
                return null;
            }};

            {IndentCode(generatedCode, 12)}
        }}
        catch (Exception ex)
        {{
            Console.WriteLine($""Exception: {{ex.Message}}"");
        }}
    }}
}}";

    // 1. Create a Syntax Tree
    var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(languageVersion);
    var tree = CSharpSyntaxTree.ParseText(fullCode, parseOptions, path: "generated.cs", encoding: Encoding.UTF8);

    var diagnostics = tree.GetDiagnostics();
    if (diagnostics.Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error))
    {
      Console.WriteLine("Syntax Errors in Generated Code:");
      foreach (var diagnostic in diagnostics)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(diagnostic);
        Console.ResetColor();
      }
      return false;
    }

    // 2. Create a Compilation
    CSharpCompilation? compilation = null;
    try
    {
      // Resolve essential assembly paths
      string systemRuntimePath = GetAssemblyPath("System.Runtime");
      string coreLibPath = GetAssemblyPath("System.Private.CoreLib");
      string consolePath = GetAssemblyPath("System.Console");


      if (string.IsNullOrEmpty(systemRuntimePath) || string.IsNullOrEmpty(coreLibPath) || string.IsNullOrEmpty(consolePath))
      {
        Console.WriteLine("Error: Could not find required .NET assemblies.  Make sure the .NET runtime is installed correctly.");
        return false;
      }

      var references = ImmutableArray.Create(
          MetadataReference.CreateFromFile(coreLibPath),
          MetadataReference.CreateFromFile(systemRuntimePath),
          MetadataReference.CreateFromFile(consolePath)
      );

      if (!additionalReferences.IsDefaultOrEmpty)
      {
        references = references.AddRange(additionalReferences);
      }

      compilation = CSharpCompilation.Create(
          Path.GetFileNameWithoutExtension(outputPath),
          new[] { tree },
          references,
          new CSharpCompilationOptions(outputKind)
              .WithPlatform(platform)
              .WithOptimizationLevel(OptimizationLevel.Release)
      );
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An exception occurred during compilation creation: {ex}");
      return false;
    }

    if (compilation == null)
    {
      Console.WriteLine("Compilation object was not created.");
      return false;
    }

    // 3. Emit the Assembly
    using (var peStream = new MemoryStream())
    using (var pdbStream = new MemoryStream())
    {
      var emitResult = compilation.Emit(peStream, pdbStream);

      if (emitResult.Success)
      {
        peStream.Seek(0, SeekOrigin.Begin);
        File.WriteAllBytes(outputPath, peStream.ToArray());

        var pdbPath = Path.ChangeExtension(outputPath, ".pdb");
        pdbStream.Seek(0, SeekOrigin.Begin);
        File.WriteAllBytes(pdbPath, pdbStream.ToArray());

        Console.WriteLine($"Compilation succeeded. Output: {outputPath}");
        return true;
      }
      else
      {
        Console.WriteLine("Compilation failed:");
        foreach (var diagnostic in emitResult.Diagnostics)
        {
          Console.ForegroundColor = diagnostic.Severity == DiagnosticSeverity.Error
              ? ConsoleColor.Red
              : ConsoleColor.Yellow;
          Console.WriteLine($"{diagnostic.Location.GetLineSpan()}: {diagnostic.Severity} {diagnostic.Id}: {diagnostic.GetMessage()}");
          Console.ResetColor();
        }
        return false;
      }
    }
  }
}
