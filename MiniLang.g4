grammar MiniLang;

program     : statement+ EOF ;

statement   :
  assignStmt
  | printStmt
  | ifStmt
  ;

assignStmt  : 'let' ID '=' expr ';' ;
printStmt   : 'print' expr ';' ;
ifStmt      : 'if' '(' expr ')' '{' statement+ '}' ;

expr
    : expr op=('*'|'/') expr    # MulDivExpr
    | expr op=('+'|'-') expr    # AddSubExpr
    | expr op=('=='|'!='|'<'|'>'|'<='|'>=') expr # CompareExpr
    | ID                        # IdExpr
    | NUMBER                    # NumberExpr
    | '(' expr ')'              # ParensExpr
    ;

ID      : [a-zA-Z_][a-zA-Z0-9_]* ;
NUMBER  : [0-9]+ ;
WS      : [ \t\r\n]+ -> skip ;

