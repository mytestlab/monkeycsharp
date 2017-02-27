using System;
using System.Diagnostics;
using Xunit;

namespace monkey.tests
{
  public class ParserTest
  {
    [Fact]
    public void TestLetStatement()
    {
      var input = "let x = 5;\n\rlet y = 10;\n\rlet foobar = 838383;";

      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      var program = parser.ParseProgram();
      CheckParserErrors(parser);
      Assert.NotNull(program);
      Assert.Equal(3, program.Statements.Count);

      AssertLetStatement(program.Statements[0], "x");
      AssertLetStatement(program.Statements[1], "y");
      AssertLetStatement(program.Statements[2], "foobar");
    }

    private void AssertLetStatement(IStatement statement, string name)
    {
      Assert.Equal("let", statement.TokenLiteral());
      var stmt = statement as LetStatement;
      Assert.NotNull(stmt);
      Assert.Equal(name, stmt.Name.Value);
      Assert.Equal(name, stmt.Name.TokenLiteral());
    }

    [Fact]
    public void TestReturnStatements()
    {
      var input = "return 5;\r\nreturn 10;\r\nreturn 993322;";

      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      var program = parser.ParseProgram();
      CheckParserErrors(parser);

      Assert.Equal(3, program.Statements.Count);
      foreach (var statement in program.Statements)
      {
        Assert.True(statement is ReturnStatement);
        Assert.Equal("return", statement.TokenLiteral());
      }
    }

    [Fact]
    public void TestIdentifierExpression()
    {
      var input = "foobar;";

      var lexer = new Lexer(input);
      var parser = new Parser(lexer);
      var program = parser.ParseProgram();
      CheckParserErrors(parser);

      Assert.Equal(1, program.Statements.Count);
      var stmt = program.Statements[0] as ExpressionStatement;
      Assert.NotNull(stmt);
      var ident = stmt.Expression as Identifier;      
      Assert.NotNull(ident);
      Assert.Equal("foobar", ident.Value);
      Assert.Equal("foobar", ident.TokenLiteral());
    }

    [Fact]
    public void TestIntegerLiteralExpression()
    {
      var input = "5;";

      var lexer = new Lexer(input);
      var parser = new Parser(lexer);
      var program = parser.ParseProgram();
      CheckParserErrors(parser);

      Assert.Equal(1, program.Statements.Count);
      var stmt = program.Statements[0] as ExpressionStatement;
      Assert.NotNull(stmt);
      var literal = stmt.Expression as IntegerLiteral;
      Assert.NotNull(literal);
      Assert.Equal(5, literal.Value);
      Assert.Equal("5", literal.TokenLiteral());
    }

    [Fact]
    public void TestParsingPrefixExpressions()
    {
      var prefixTests = new[]
      {
        new { Input = "!5;", Operator = "!", IntegerValue = 5 },
        new { Input = "-15;", Operator = "-", IntegerValue = 15 }
      };

      foreach (var prefixTest in prefixTests)
      {
        var lexer = new Lexer(prefixTest.Input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        CheckParserErrors(parser);

        Assert.Equal(1, program.Statements.Count);
        var stmt = program.Statements[0] as ExpressionStatement;
        Assert.NotNull(stmt);
        var exp = stmt.Expression as PrefixExpression;
        Assert.NotNull(exp);
        Assert.Equal(prefixTest.Operator, exp.Operator);
        TestIntegerLiteral(exp.Right, prefixTest.IntegerValue);
      }
    }

    [Fact]
    public void TestParsingInfixExpressions()
    {
      var infixtests = new[]
      {
        new { Input = "5 + 5;", LeftValue = 5, Operator = "+", RightValue = 5 },
        new { Input = "5 - 5;", LeftValue = 5, Operator = "-", RightValue = 5 },
        new { Input = "5 * 5;", LeftValue = 5, Operator = "*", RightValue = 5 },
        new { Input = "5 / 5;", LeftValue = 5, Operator = "/", RightValue = 5 },
        new { Input = "5 > 5;", LeftValue = 5, Operator = ">", RightValue = 5 },
        new { Input = "5 < 5;", LeftValue = 5, Operator = "<", RightValue = 5 },
        new { Input = "5 == 5;", LeftValue = 5, Operator = "==", RightValue = 5 },
        new { Input = "5 != 5;", LeftValue = 5, Operator = "!=", RightValue = 5 }
      };

      foreach (var tt in infixtests)
      {
        var lexer = new Lexer(tt.Input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        CheckParserErrors(parser);

        Assert.Equal(1, program.Statements.Count);
        var stmt = program.Statements[0] as ExpressionStatement;
        Assert.NotNull(stmt);

        var exp = stmt.Expression as InfixExpression;
        Assert.NotNull(exp);
        TestIntegerLiteral(exp.Left, tt.LeftValue);
        Assert.Equal(exp.Operator, tt.Operator);
        TestIntegerLiteral(exp.Right, tt.RightValue);
      }
    }

    private bool TestIntegerLiteral(IExpression il, int value)
    {
      var integ = il as IntegerLiteral;
      Assert.NotNull(integ);
      Assert.Equal(integ.Value, value);
      Assert.Equal(integ.TokenLiteral(), $"{value:d}");

      return true;
    }

    private void CheckParserErrors(Parser p)
    {
      var errors = p.Errors;
      if (errors.Count == 0)
        return;

      Debug.WriteLine($"parser has {errors.Count} errors");
      foreach (var error in errors)
      {
        Debug.WriteLine($"parser error: %q", error);
      }

      throw new Exception();
    }
  }
}
