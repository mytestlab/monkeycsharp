using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monkey
{
  class Lexer
  {
    public string Input { get; set; }
    public int Position { get; set; }
    public int ReadPosition { get; set; }
    public char Ch { get; set; }

    public KeyWords KeyWords { get; }

    public Lexer(string input)
    {
      KeyWords = new KeyWords();
      Input = input;
      ReadChar();
    }

    private void ReadChar()
    {
      Ch = ReadPosition >= Input.Length ? '\0' : Input[ReadPosition];
      Position = ReadPosition;
      ReadPosition++;
    }

    public Token NextToken()
    {
      Token token = null;
      SkipWhitespace();

      switch (Ch)
      {
        case '=':
          if (PeekChar() == '=')
          {
            var ch = Ch;
            ReadChar();
            token = new Token { Type = TokenType.Eq, Literal = string.Concat(ch, Ch) };
          }
          else
          {
            token = NewToken(TokenType.Assign, Ch);
          }
          break;
        case ';':
          token = NewToken(TokenType.SemiColon, Ch);
          break;
        case '(':
          token = NewToken(TokenType.LParen, Ch);
          break;
        case ')':
          token = NewToken(TokenType.RParen, Ch);
          break;
        case ',':
          token = NewToken(TokenType.Comma, Ch);
          break;
        case '+':
          token = NewToken(TokenType.Plus, Ch);
          break;
        case '-':
          token = NewToken(TokenType.Minus, Ch);
          break;
        case '!':
          if (PeekChar() == '=')
          {
            var ch = Ch;
            ReadChar();
            token = new Token { Type = TokenType.NotEq, Literal = string.Concat(ch, Ch) };
          }
          else
          {
            token = NewToken(TokenType.Bang, Ch);
          }
          break;
        case '*':
          token = NewToken(TokenType.Asterisk, Ch);
          break;
        case '/':
          token = NewToken(TokenType.Slash, Ch);
          break;
        case '<':
          token = NewToken(TokenType.Lt, Ch);
          break;
        case '>':
          token = NewToken(TokenType.Gt, Ch);
          break;
        case '{':
          token = NewToken(TokenType.LBrace, Ch);
          break;
        case '}':
          token = NewToken(TokenType.RBrace, Ch);
          break;
        case '\0':
          token = new Token
          {
            Literal = "",
            Type = TokenType.Eof
          };
          break;
        default:
          if (IsLetter(Ch))
          {
            token = new Token { Literal = ReadIdentifier() };
            token.Type = KeyWords.LookupIdent(token.Literal);
            return token;
          }
          else if (IsDigit(Ch))
          {
            token = new Token {Type = TokenType.Int};
            token.Literal = ReadNumber();
            return token;
          }
          else
          {
            token = NewToken(TokenType.Illegal, Ch);
          }
          break;
      }

      ReadChar();
      return token;
    }

    private string ReadNumber()
    {
      var position = Position;
      while (IsDigit(Ch))
      {
        ReadChar();
      }

      return Input.Substring(position, Position - position);
    }

    private bool IsDigit(char ch)
    {
      return '0' <= ch && ch <= '9';
    }

    private string ReadIdentifier()
    {
      var position = Position;
      while (IsLetter(Ch))
      {
        ReadChar();
      }

      return Input.Substring(position, Position - position);
    }

    private bool IsLetter(char ch)
    {
      return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch == '_';
    }

    private char PeekChar()
    {
      if (ReadPosition >= Input.Length)
      {
        return '\0';
      }
      else
      {
        return Input[ReadPosition];
      }
    }

    private void SkipWhitespace()
    {
      while (Ch == ' ' || Ch == '\t' || Ch == '\n' || Ch == '\r')
      {
        ReadChar();
      }
    }

    private Token NewToken(string tokenType, char ch)
    {
      return new Token { Literal = ch.ToString(), Type = tokenType };
    }
  }
}
