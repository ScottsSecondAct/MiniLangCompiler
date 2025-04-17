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

  public override string VisitDeclStmt(MiniLangParser.DeclStmtContext context)
  {
    var type = context.type().GetText();
    var id = context.ID().GetText();
    _variables[id] = type;

    if (context.expr() != null)
    {
      var value = Visit(context.expr());
      return $"{type} {id} = {value};";
    }
    return $"{type} {id};";
  }

  public override string VisitAssignStmt(MiniLangParser.AssignStmtContext context)
  {
    var id = context.ID().GetText();
    var value = Visit(context.expr());
    return $"{id} = {value};";
  }

  public override string VisitExprStmt(MiniLangParser.ExprStmtContext context)
  {
    return Visit(context.expr()) + ";";
  }

  public override string VisitIfStmt(MiniLangParser.IfStmtContext ctx)
  {
    var condition = Visit(ctx.expr());
    var block = Visit(ctx.block());
    return $"if ({condition}) {block}";
  }

  public override string VisitWhileStatement(MiniLangParser.WhileStatementContext ctx)
  {
    var cond = Visit(ctx.expr());
    var body = Visit(ctx.block());
    return $"while ({cond}) {body}";
  }

  public override string VisitBlock(MiniLangParser.BlockContext ctx)
  {
    var stmts = ctx.statement().Select(Visit);
    var body = string.Join("\n", stmts);
    return $"{{\n{body}\n}}";
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

  public override string VisitCallExpr(MiniLangParser.CallExprContext ctx)
  {
    var id = ctx.ID().GetText();
    var arg = Visit(ctx.expr());
    if (id == "print")
      return $"Console.WriteLine({arg})";
    return $"{id}({arg})";
  }

  public override string VisitParensExpr(MiniLangParser.ParensExprContext ctx)
      => $"({Visit(ctx.expr())})";
}

