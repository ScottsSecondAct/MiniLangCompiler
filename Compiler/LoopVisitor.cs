using System;
//using MiniLangParser = MiniLang.MiniLangParser;
//using MiniLangBaseVisitor = MiniLang.MiniLangBaseVisitor<object>;

public class LoopVisitor : MiniLangBaseVisitor<object>
{
  public override object? VisitWhileStatement(MiniLangParser.WhileStatementContext context)
  {
    Console.WriteLine("While Loop:");
    Console.WriteLine("Condition: " + context.expr().GetText());
    Visit(context.block());
    return null;
  }

  //public override object? VisitDoWhileStatement(MiniLangParser.DoWhileStatementContext context)
  //{
  //  Console.WriteLine("Do-While Loop:");
  //  Console.WriteLine("Condition: " + context.expression().GetText());
  //  Visit(context.block());
  //  return null;
  //}

  //public override object? VisitForStatement(MiniLangParser.ForStatementContext context)
  //{
  //  Console.WriteLine("For Loop:");
  //  if (context.forInit() != null)
  //    Console.WriteLine("Init: " + context.forInit().GetText());
  //  if (context.expression() != null)
  //    Console.WriteLine("Condition: " + context.expression().GetText());
  //  if (context.forUpdate() != null)
  //    Console.WriteLine("Update: " + context.forUpdate().GetText());
  //  Visit(context.block());
  //  return null;
  //}

  //public override object? VisitForEachStatement(MiniLangParser.ForEachStatementContext context)
  //{
  //  Console.WriteLine("For-Each Loop:");
  //  Console.WriteLine("Variable: " + context.ID().GetText());
  //  Console.WriteLine("Iterable: " + context.expression().GetText());
  //  Visit(context.block());
  //  return null;
  //}
}

