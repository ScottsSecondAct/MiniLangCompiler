using System.Text;

// Walks the typed AST and emits C# source code.

public class CSharpCodeGen
{
  public string Generate(ProgramNode program)
  {
    var sb = new StringBuilder();
    foreach (var stmt in program.Statements)
      sb.AppendLine(EmitStmt(stmt));
    return sb.ToString();
  }

  private string EmitStmt(StmtNode stmt) => stmt switch
  {
    DeclStmt d => d.Init != null
        ? $"{d.Type} {d.Name} = {EmitExpr(d.Init)};"
        : $"{d.Type} {d.Name};",
    AssignStmt a => $"{a.Name} = {EmitExpr(a.Value)};",
    ExprStmt e   => $"{EmitExpr(e.Expr)};",
    IfStmt i     => $"if ({EmitExpr(i.Condition)}) {EmitBlock(i.Body)}",
    WhileStmt w  => $"while ({EmitExpr(w.Condition)}) {EmitBlock(w.Body)}",
    _            => throw new NotSupportedException($"Unknown statement: {stmt.GetType().Name}")
  };

  private string EmitBlock(BlockNode block)
  {
    var stmts = string.Join("\n", block.Statements.Select(EmitStmt));
    return $"{{\n{stmts}\n}}";
  }

  private string EmitExpr(ExprNode expr) => expr switch
  {
    BinaryExpr b     => $"{EmitExpr(b.Left)} {b.Op} {EmitExpr(b.Right)}",
    CallExpr c       => c.FunctionName == "print"
        ? $"Console.WriteLine({EmitExpr(c.Argument)})"
        : $"{c.FunctionName}({EmitExpr(c.Argument)})",
    IdentifierExpr i => i.Name,
    NumberExpr n     => n.Value,
    ParensExpr p     => $"({EmitExpr(p.Inner)})",
    _                => throw new NotSupportedException($"Unknown expression: {expr.GetType().Name}")
  };
}
