using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monkey
{
  enum Const
  {
    Lowest,
    Equals,
    LessGreater,
    Sum,
    Product,
    Prefix,
    Call
  }

  public delegate IExpression PrefixParseFn();

  public delegate IExpression InfixParseFn(IExpression expression);

  class Parser
  {
    public Lexer Lexer { get; set; }

    public List<string> Errors { get; set; }
    public Token CurToken { get; set; }
    public Token PeekToken { get; set; }

    public Dictionary<string, PrefixParseFn> PrefixParseFns { get; set; }
    public Dictionary<string, InfixParseFn> InfixParseFns { get; set; }

    private Dictionary<string, int> precedences = new Dictionary<string, int>
    {
      { TokenType.Eq, (int)Const.Equals },
      { TokenType.NotEq, (int)Const.Equals },
      { TokenType.Lt, (int)Const.LessGreater },
      { TokenType.Gt, (int)Const.LessGreater },
      { TokenType.Plus, (int)Const.Sum },
      { TokenType.Minus, (int)Const.Sum },
      { TokenType.Slash, (int)Const.Product },
      { TokenType.Asterisk, (int)Const.Product }
    };

    public Parser(Lexer lexer)
    {
      Lexer = lexer;
      Errors = new List<string>();
    }

    private void PeekError(string t)
    {
      var msg = $"Expected next token to be {t}, got {PeekToken.Type} instead";
      Errors.Add(msg);
    }

    public AstProgram ParseProgram()
    {
      var program = new AstProgram();
      program.Statements = new List<IStatement>();

      while (CurToken.Type != TokenType.Eof)
      {
        var stmt = ParseStatement();
        if (stmt != null)
        {
          program.Statements.Add(stmt);
        }
        NextToken();
      }

      return program;
    }

    private IStatement ParseStatement()
    {
      switch (CurToken.Type)
      {
        case TokenType.Let:
          return ParseLetStatement();
        case TokenType.Return:
          return ParseReturnStatement();
        default:
          return ParseExpressionStatement();
      }
    }

    private LetStatement ParseLetStatement()
    {
      var stmt = new LetStatement { Token = CurToken };
      if (!ExpectPeek(TokenType.Ident))
      {
        return null;
      }

      stmt.Name = new Identifier {Token = CurToken, Value = CurToken.Literal};
      if (!ExpectPeek(TokenType.Assign))
      {
        return null;
      }

      while (!CurTokenIs(TokenType.SemiColon))
      {
        NextToken();
      }

      return stmt;
    }

    private ReturnStatement ParseReturnStatement()
    {
      var stmt = new ReturnStatement { Token = CurToken };
      NextToken();

      while (!CurTokenIs(TokenType.SemiColon))
      {
        NextToken();
      }

      return stmt;
    }

    private ExpressionStatement ParseExpressionStatement()
    {
      var stmt = new ExpressionStatement { Token = CurToken };
      stmt.Expression = ParseExpression((int) Const.Lowest);
      if (PeekTokenIs(TokenType.SemiColon))
      {
        NextToken();
      }

      return stmt;
    }

    private IExpression ParseExpression(int precedence)
    {
      var prefix = PrefixParseFns[CurToken.Type];
      if (prefix == null)
      {
        NoPrefixParseFnError(CurToken.Type);
        return null;
      }
      var leftExp = prefix();

      while (!PeekTokenIs(TokenType.SemiColon) && precedence < PeekPrecedence())
      {
        var infix = InfixParseFns[PeekToken.Type];
        if (infix == null)
        {
          return leftExp;
        }
        NextToken();
        leftExp = infix(leftExp);
      }

      return leftExp;
    }

    private IExpression ParseIdentifier()
    {
      return new Identifier {Token = CurToken, Value = CurToken.Literal};
    }

    private IExpression ParseIntegerLiteral()
    {
      var lit = new IntegerLiteral { Token = CurToken };
      int value;
      if (!int.TryParse(CurToken.Literal, out value))
      {
        var msg = $"could not parse {CurToken.Literal} as integer";
        Errors.Add(msg);
        return null;
      }
      lit.Value = value;

      return lit;
    }

    private IExpression ParseInfixExpression(IExpression left)
    {
      var expression = new InfixExpression
      {
        Token = CurToken,
        Operator = CurToken.Literal,
        Left = left
      };

      var precedence = CurPrecedence();
      NextToken();
      expression.Right = ParseExpression(precedence);

      return expression;
    }

    private void NoPrefixParseFnError(string t)
    {
      var msg = $"no prefix parse function for {t} found";
      Errors.Add(msg);
    }

    private IExpression ParsePrefixExpression()
    {
      var expression = new PrefixExpression { Token = CurToken, Operator = CurToken.Literal };
      NextToken();
      expression.Right = ParseExpression((int)Const.Prefix);

      return expression;
    }

    private bool CurTokenIs(string t)
    {
      return CurToken.Type == t;
    }

    private bool PeekTokenIs(string t)
    {
      return PeekToken.Type == t;
    }

    private bool ExpectPeek(string t)
    {
      if (PeekTokenIs(t))
      {
        NextToken();
        return true;
      }
      else
      {
        PeekError(t);
        return false;
      }
    }

    private void NextToken()
    {
      CurToken = PeekToken;
      PeekToken = Lexer.NextToken();
    }

    private void RegisterPrefix(string tokenType, PrefixParseFn prefixParseFn)
    {
      PrefixParseFns[tokenType] = prefixParseFn;
    }

    private void RegisterInfix(string tokenType, InfixParseFn infixParseFn)
    {
      InfixParseFns[tokenType] = infixParseFn;
    }

    private int PeekPrecedence()
    {
      var p = precedences.ContainsKey(PeekToken.Type);
      if (p)
      {
        return precedences[PeekToken.Type];
      }

      return (int) Const.Lowest;
    }

    private int CurPrecedence()
    {
      var p = precedences.ContainsKey(CurToken.Type);
      if (p)
      {
        return precedences[CurToken.Type];
      }

      return (int) Const.Lowest;
    }
  }
}
