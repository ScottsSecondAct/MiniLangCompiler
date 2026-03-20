// Walks the ANTLR parse tree and produces a typed AST.

public class AstBuilder : MiniLangBaseVisitor<AstNode>
{
  public override AstNode VisitProgram(MiniLangParser.ProgramContext ctx)
  {
    var stmts = ctx.statement().Select(s => (StmtNode)Visit(s)).ToList();
    return new ProgramNode(stmts);
  }

  public override AstNode VisitDeclStmt(MiniLangParser.DeclStmtContext ctx)
  {
    var type = ctx.type().GetText();
    var name = ctx.ID().GetText();
    var init = ctx.expr() != null ? (ExprNode)Visit(ctx.expr()) : null;
    return new DeclStmt(type, name, init);
  }

  public override AstNode VisitAssignStmt(MiniLangParser.AssignStmtContext ctx)
  {
    var name = ctx.ID().GetText();
    var value = (ExprNode)Visit(ctx.expr());
    return new AssignStmt(name, value);
  }

  public override AstNode VisitExprStmt(MiniLangParser.ExprStmtContext ctx)
  {
    var expr = (ExprNode)Visit(ctx.expr());
    return new ExprStmt(expr);
  }

  public override AstNode VisitIfStmt(MiniLangParser.IfStmtContext ctx)
  {
    var condition = (ExprNode)Visit(ctx.expr());
    var body = (BlockNode)Visit(ctx.block());
    return new IfStmt(condition, body);
  }

  public override AstNode VisitWhileStatement(MiniLangParser.WhileStatementContext ctx)
  {
    var condition = (ExprNode)Visit(ctx.expr());
    var body = (BlockNode)Visit(ctx.block());
    return new WhileStmt(condition, body);
  }

  public override AstNode VisitBlock(MiniLangParser.BlockContext ctx)
  {
    var stmts = ctx.statement().Select(s => (StmtNode)Visit(s)).ToList();
    return new BlockNode(stmts);
  }

  public override AstNode VisitMulDivExpr(MiniLangParser.MulDivExprContext ctx)
  {
    var left = (ExprNode)Visit(ctx.expr(0));
    var right = (ExprNode)Visit(ctx.expr(1));
    return new BinaryExpr(left, ctx.op.Text, right);
  }

  public override AstNode VisitAddSubExpr(MiniLangParser.AddSubExprContext ctx)
  {
    var left = (ExprNode)Visit(ctx.expr(0));
    var right = (ExprNode)Visit(ctx.expr(1));
    return new BinaryExpr(left, ctx.op.Text, right);
  }

  public override AstNode VisitCompareExpr(MiniLangParser.CompareExprContext ctx)
  {
    var left = (ExprNode)Visit(ctx.expr(0));
    var right = (ExprNode)Visit(ctx.expr(1));
    return new BinaryExpr(left, ctx.op.Text, right);
  }

  public override AstNode VisitCallExpr(MiniLangParser.CallExprContext ctx)
  {
    var name = ctx.ID().GetText();
    var arg = (ExprNode)Visit(ctx.expr());
    return new CallExpr(name, arg);
  }

  public override AstNode VisitIdExpr(MiniLangParser.IdExprContext ctx)
      => new IdentifierExpr(ctx.ID().GetText());

  public override AstNode VisitNumberExpr(MiniLangParser.NumberExprContext ctx)
      => new NumberExpr(ctx.NUMBER().GetText());

  public override AstNode VisitParensExpr(MiniLangParser.ParensExprContext ctx)
  {
    var inner = (ExprNode)Visit(ctx.expr());
    return new ParensExpr(inner);
  }
}
