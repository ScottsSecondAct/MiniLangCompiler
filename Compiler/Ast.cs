// Immutable AST node types for MiniLang, defined as C# records.
// The tree is built by AstBuilder from the ANTLR parse tree, then
// consumed by CSharpCodeGen to emit C# source code.

// Base type for all nodes
public abstract record AstNode;

// Root node — holds the ordered list of top-level statements
public record ProgramNode(List<StmtNode> Statements) : AstNode;

// --- Statements ---

public abstract record StmtNode : AstNode;

// int x = 5;  or  int x;
public record DeclStmt(string Type, string Name, ExprNode? Init) : StmtNode;

// x = expr;
public record AssignStmt(string Name, ExprNode Value) : StmtNode;

// expr;  (e.g. a standalone function call)
public record ExprStmt(ExprNode Expr) : StmtNode;

// if (condition) { ... }
public record IfStmt(ExprNode Condition, BlockNode Body) : StmtNode;

// while (condition) { ... }
public record WhileStmt(ExprNode Condition, BlockNode Body) : StmtNode;

// { statement* }  — used as the body of if/while
public record BlockNode(List<StmtNode> Statements) : AstNode;

// --- Expressions ---

public abstract record ExprNode : AstNode;

// Binary operation: arithmetic (+ - * /) or comparison (== != < > <= >=)
public record BinaryExpr(ExprNode Left, string Op, ExprNode Right) : ExprNode;

// Function call: name(argument)  — currently single-argument only
public record CallExpr(string FunctionName, ExprNode Argument) : ExprNode;

// Variable reference: x
public record IdentifierExpr(string Name) : ExprNode;

// Numeric literal: 42
public record NumberExpr(string Value) : ExprNode;

// Parenthesized expression: (expr)
public record ParensExpr(ExprNode Inner) : ExprNode;
