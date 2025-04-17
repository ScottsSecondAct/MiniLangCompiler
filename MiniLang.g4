grammar MiniLang;

program
  : statement+ EOF ;

statement
  : declStmt
  | assignStmt
  | exprStmt
  | ifStmt
  | whileStatement
  ;

declStmt
  : type ID ('=' expr)? ';'
  ;

assignStmt
  : ID '=' expr ';'
  ;

exprStmt
  : expr ';'
  ;

ifStmt
  : 'if' '(' expr ')' block
  ;

whileStatement
  : 'while' '(' expr ')' block
  ;

  block
  : '{' statement* '}'
  ;

expr
  : expr op=('*'|'/') expr                      # MulDivExpr
  | expr op=('+'|'-') expr                      # AddSubExpr
  | expr op=('=='|'!='|'<'|'>'|'<='|'>=') expr  # CompareExpr
  | ID '(' expr ')'                             # CallExpr
  | ID                                          # IdExpr
  | NUMBER                                      # NumberExpr
  | '(' expr ')'                                # ParensExpr
  ;

type
  : 'int'
  | 'float'
  | 'bool'
  | 'string'
  ;

ID
  : [a-zA-Z_][a-zA-Z0-9_]*
  ;

NUMBER
  : [0-9]+
  ;

WS
  : [ \t\r\n]+ -> skip
  ;