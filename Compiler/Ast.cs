// AST node definitions for MiniLang

public abstract record AstNode;

// Program
public record ProgramNode(List<StmtNode> Statements) : AstNode;

// Statements
public abstract record StmtNode : AstNode;

public record DeclStmt(string Type, string Name, ExprNode? Init) : StmtNode;
public record AssignStmt(string Name, ExprNode Value) : StmtNode;
public record ExprStmt(ExprNode Expr) : StmtNode;
public record IfStmt(ExprNode Condition, BlockNode Body) : StmtNode;
public record WhileStmt(ExprNode Condition, BlockNode Body) : StmtNode;

public record BlockNode(List<StmtNode> Statements) : AstNode;

// Expressions
public abstract record ExprNode : AstNode;

public record BinaryExpr(ExprNode Left, string Op, ExprNode Right) : ExprNode;
public record CallExpr(string FunctionName, ExprNode Argument) : ExprNode;
public record IdentifierExpr(string Name) : ExprNode;
public record NumberExpr(string Value) : ExprNode;
public record ParensExpr(ExprNode Inner) : ExprNode;
