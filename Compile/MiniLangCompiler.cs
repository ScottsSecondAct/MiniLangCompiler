using System.Text;

public class MiniLangCompiler : MiniLangBaseVisitor<string>
{
  private readonly Dictionary<string, string> _variables = new();

  public override string VisitProgram(MiniLangParser.ProgramContext context)
  {
    var sb = new StringBuilder();
    foreach (var stmt in context.statement())
      sb.AppendLine(Visit(stmt));
    return sb.ToString();
  }

  public override string VisitAssignStmt(MiniLangParser.AssignStmtContext context)
  {
    var id = context.ID().GetText();
    var value = Visit(context.expr());
    _variables[id] = "int";
    return $"int {id} = {value};";
  }

  public override string VisitPrintStmt(MiniLangParser.PrintStmtContext context)
  {
    var expr = Visit(context.expr());
    return $"Console.WriteLine({expr});";
  }

  public override string VisitIfStmt(MiniLangParser.IfStmtContext ctx)
  {
    var condition = Visit(ctx.expr());
    var block = new StringBuilder();

    foreach (var stmt in ctx.statement())
    {
      var translated = Visit(stmt);
      // Indent nested block lines
      foreach (var line in translated.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        block.AppendLine("    " + line.Trim());
    }

    return $"if ({condition})\n{{\n{block.ToString()}}}";
  }

  public override string VisitAddSubExpr(MiniLangParser.AddSubExprContext ctx)
      => $"{Visit(ctx.expr(0))} {ctx.op.Text} {Visit(ctx.expr(1))}";

  public override string VisitMulDivExpr(MiniLangParser.MulDivExprContext ctx)
      => $"{Visit(ctx.expr(0))} {ctx.op.Text} {Visit(ctx.expr(1))}";

  public override string VisitIdExpr(MiniLangParser.IdExprContext ctx)
      => ctx.ID().GetText();

  public override string VisitNumberExpr(MiniLangParser.NumberExprContext ctx)
      => ctx.NUMBER().GetText();

  public override string VisitCompareExpr(MiniLangParser.CompareExprContext ctx)
    => $"{Visit(ctx.expr(0))} {ctx.op.Text} {Visit(ctx.expr(1))}";

  public override string VisitParensExpr(MiniLangParser.ParensExprContext ctx)
      => $"({Visit(ctx.expr())})";
}

