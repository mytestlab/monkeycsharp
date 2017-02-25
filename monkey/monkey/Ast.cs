using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monkey
{
  interface INode
  {
    string TokenLiteral();

    string ComposeString();
  }

  interface IStatement : INode
  {
    void StatementNode();
  }

  interface IExpression : INode
  {
    void ExpressionNode();
  }

  class AstProgram : INode
  {
    private IList<IStatement> Statements { get; set; }

    public string TokenLiteral()
    {
      return Statements.Count > 0 ? Statements[0].TokenLiteral() : string.Empty;
    }

    public string ComposeString()
    {
      var buffer = new StringBuilder();

      foreach (var statement in Statements)
      {
        buffer.Append(statement.ComposeString());
      }

      return buffer.ToString();
    }
  }

  class Identifier : INode
  {
    public Token Token { get; set; }
    public IExpression Value { get; set; }
    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public string ComposeString()
    {
      return Value.ComposeString();
    }
  }

  class LetStatement : IStatement
  {
    public Token Token { get; set; }
    public Identifier Name { get; set; }
    public IExpression Value { get; set; }
    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public string ComposeString()
    {
      var buffer = new StringBuilder();

      buffer.Append(Token.Literal + " ");
      buffer.Append(Name.ComposeString());
      buffer.Append(" = ");

      if (Value != null)
      {
        buffer.Append(Value.ComposeString());
      }

      buffer.Append(";");

      return buffer.ToString();
    }

    public void StatementNode()
    {
    }
  }

  class ReturnStatement : IStatement
  {
    public Token Token { get; set; }

    public IExpression ReturnValue { get; set; }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public string ComposeString()
    {
      var buffer = new StringBuilder();

      buffer.Append(TokenLiteral() + " ");
      if (ReturnValue != null)
      {
        buffer.Append(ReturnValue.ComposeString());
      }
      buffer.Append(";");

      return buffer.ToString();
    }

    public void StatementNode()
    {
      throw new NotImplementedException();
    }
  }

  class ExpressionStatement : IStatement
  {
    public Token Token { get; set; }
    public IExpression Expression { get; set; }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public string ComposeString()
    {
      if (Expression != null)
      {
        return Expression.ComposeString();
      }

      return string.Empty;
    }

    public void StatementNode()
    {
      throw new NotImplementedException();
    }
  }

  class IntegerLiteral : IExpression
  {
    public Token Token { get; set; }
    public int Value { get; set; }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public string ComposeString()
    {
      return Token.Literal;
    }

    public void ExpressionNode()
    {
    }
  }

  class PrefixExpression : IExpression
  {
    public Token Token { get; set; }
    public string Operator { get; set; }
    public IExpression Right { get; set; }

    public string TokenLiteral()
    {
      return Token.Literal;
    }

    public string ComposeString()
    {
      var buffer = new StringBuilder();

      buffer.Append("(");
      buffer.Append(Operator);
      buffer.Append(Right.ComposeString());
      buffer.Append(")");

      return buffer.ToString();
    }

    public void ExpressionNode()
    {
      throw new NotImplementedException();
    }
  }
}
