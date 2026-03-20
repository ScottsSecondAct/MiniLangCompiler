using System.Text;

// CSharpCodeGen walks the typed AST and emits C# source code as a string.
// It is the second phase of compilation, decoupled from ANTLR entirely.

public class CSharpCodeGen
{
  // Entry point: emit all top-level statements and return the combined C# source
  public string Generate(ProgramNode program)
  {
    var sb = new StringBuilder();
    foreach (var stmt in program.Statements)
      sb.AppendLine(EmitStmt(stmt));
    return sb.ToString();
  }

  // Dispatch to the correct emitter based on the statement type
  private string EmitStmt(StmtNode stmt) => stmt switch
  {
    DeclStmt d => d.Init != null
        ? $"{d.Type} {d.Name} = {EmitExpr(d.Init)};"   // int x = 5;
        : $"{d.Type} {d.Name};",                         // int x;
    AssignStmt a => $"{a.Name} = {EmitExpr(a.Value)};",
    ExprStmt e   => $"{EmitExpr(e.Expr)};",
    IfStmt i     => $"if ({EmitExpr(i.Condition)}) {EmitBlock(i.Body)}",
    WhileStmt w  => $"while ({EmitExpr(w.Condition)}) {EmitBlock(w.Body)}",
    _            => throw new NotSupportedException($"Unknown statement: {stmt.GetType().Name}")
  };

  // Emit a braced block of statements
  private string EmitBlock(BlockNode block)
  {
    var stmts = string.Join("\n", block.Statements.Select(EmitStmt));
    return $"{{\n{stmts}\n}}";
  }

  // Dispatch to the correct emitter based on the expression type
  private string EmitExpr(ExprNode expr) => expr switch
  {
    BinaryExpr b     => $"{EmitExpr(b.Left)} {b.Op} {EmitExpr(b.Right)}",
    // 'print' is a MiniLang built-in that maps to Console.WriteLine
    CallExpr c       => c.FunctionName == "print"
        ? $"Console.WriteLine({EmitExpr(c.Argument)})"
        : $"{c.FunctionName}({EmitExpr(c.Argument)})",
    IdentifierExpr i => i.Name,
    NumberExpr n     => n.Value,
    ParensExpr p     => $"({EmitExpr(p.Inner)})",
    _                => throw new NotSupportedException($"Unknown expression: {expr.GetType().Name}")
  };
}
