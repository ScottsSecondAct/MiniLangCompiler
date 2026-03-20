// AstBuilder walks the ANTLR parse tree and produces a typed AST.
// It extends the generated MiniLangBaseVisitor<AstNode>, overriding
// one method per grammar rule to construct the corresponding AST node.

public class AstBuilder : MiniLangBaseVisitor<AstNode>
{
  // Visit the top-level program rule; collect all statements into a ProgramNode
  public override AstNode VisitProgram(MiniLangParser.ProgramContext ctx)
  {
    var stmts = ctx.statement().Select(s => (StmtNode)Visit(s)).ToList();
    return new ProgramNode(stmts);
  }

  // Variable declaration: type ID ('=' expr)?
  public override AstNode VisitDeclStmt(MiniLangParser.DeclStmtContext ctx)
  {
    var type = ctx.type().GetText();
    var name = ctx.ID().GetText();
    // Init expression is optional — null when no '= expr' is present
    var init = ctx.expr() != null ? (ExprNode)Visit(ctx.expr()) : null;
    return new DeclStmt(type, name, init);
  }

  // Assignment: ID '=' expr
  public override AstNode VisitAssignStmt(MiniLangParser.AssignStmtContext ctx)
  {
    var name = ctx.ID().GetText();
    var value = (ExprNode)Visit(ctx.expr());
    return new AssignStmt(name, value);
  }

  // Expression used as a statement: expr ';'
  public override AstNode VisitExprStmt(MiniLangParser.ExprStmtContext ctx)
  {
    var expr = (ExprNode)Visit(ctx.expr());
    return new ExprStmt(expr);
  }

  // If statement: 'if' '(' expr ')' block
  public override AstNode VisitIfStmt(MiniLangParser.IfStmtContext ctx)
  {
    var condition = (ExprNode)Visit(ctx.expr());
    var body = (BlockNode)Visit(ctx.block());
    return new IfStmt(condition, body);
  }

  // While loop: 'while' '(' expr ')' block
  public override AstNode VisitWhileStatement(MiniLangParser.WhileStatementContext ctx)
  {
    var condition = (ExprNode)Visit(ctx.expr());
    var body = (BlockNode)Visit(ctx.block());
    return new WhileStmt(condition, body);
  }

  // Block: '{' statement* '}' — used as the body of if/while
  public override AstNode VisitBlock(MiniLangParser.BlockContext ctx)
  {
    var stmts = ctx.statement().Select(s => (StmtNode)Visit(s)).ToList();
    return new BlockNode(stmts);
  }

  // Multiplication or division: expr ('*'|'/') expr
  public override AstNode VisitMulDivExpr(MiniLangParser.MulDivExprContext ctx)
  {
    var left = (ExprNode)Visit(ctx.expr(0));
    var right = (ExprNode)Visit(ctx.expr(1));
    return new BinaryExpr(left, ctx.op.Text, right);
  }

  // Addition or subtraction: expr ('+'|'-') expr
  public override AstNode VisitAddSubExpr(MiniLangParser.AddSubExprContext ctx)
  {
    var left = (ExprNode)Visit(ctx.expr(0));
    var right = (ExprNode)Visit(ctx.expr(1));
    return new BinaryExpr(left, ctx.op.Text, right);
  }

  // Comparison: expr ('=='|'!='|'<'|'>'|'<='|'>=') expr
  public override AstNode VisitCompareExpr(MiniLangParser.CompareExprContext ctx)
  {
    var left = (ExprNode)Visit(ctx.expr(0));
    var right = (ExprNode)Visit(ctx.expr(1));
    return new BinaryExpr(left, ctx.op.Text, right);
  }

  // Function call: ID '(' expr ')'
  public override AstNode VisitCallExpr(MiniLangParser.CallExprContext ctx)
  {
    var name = ctx.ID().GetText();
    var arg = (ExprNode)Visit(ctx.expr());
    return new CallExpr(name, arg);
  }

  // Variable reference: ID
  public override AstNode VisitIdExpr(MiniLangParser.IdExprContext ctx)
      => new IdentifierExpr(ctx.ID().GetText());

  // Numeric literal: NUMBER
  public override AstNode VisitNumberExpr(MiniLangParser.NumberExprContext ctx)
      => new NumberExpr(ctx.NUMBER().GetText());

  // Parenthesized expression: '(' expr ')'
  public override AstNode VisitParensExpr(MiniLangParser.ParensExprContext ctx)
  {
    var inner = (ExprNode)Visit(ctx.expr());
    return new ParensExpr(inner);
  }
}
