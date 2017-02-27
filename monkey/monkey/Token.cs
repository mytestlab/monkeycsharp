using System.Collections.Generic;

namespace monkey
{
  public class TokenType
  {
    public const string Illegal = "ILLEGAL";
    public const string Eof = "EOF";
    public const string Ident = "IDENT";
    public const string Int = "INT";
    public const string Assign = "=";
    public const string Plus = "+";
    public const string Minus = "-";
    public const string Bang = "!";
    public const string Asterisk = "*";
    public const string Slash = "/";
    public const string Comma = ",";
    public const string SemiColon = ";";
    public const string LParen = "(";
    public const string RParen = ")";
    public const string LBrace = "[";
    public const string RBrace = "]";
    public const string Lt = "<";
    public const string Gt = ">";
    public const string Function = "FUNCTION";
    public const string Let = "LET";
    public const string True = "TRUE";
    public const string False = "FALSE";
    public const string If = "IF";
    public const string Else = "ELSE";
    public const string Return = "RETURN";
    public const string Eq = "==";
    public const string NotEq = "!=";
  }

  public class KeyWords : Dictionary<string, string>
  {
    public KeyWords()
    {
      Add("fn", TokenType.Function);
      Add("let", TokenType.Let);
      Add("true", TokenType.True);
      Add("false", TokenType.False);
      Add("if", TokenType.If);
      Add("else", TokenType.Else);
      Add("return", TokenType.Return);
    }

    public string LookupIdent(string identifier)
    {
      if (ContainsKey(identifier))
      {
        return this[identifier];
      }

      return TokenType.Ident;
    }
  }

  class Token
  {
    public string Type { get; set; }
    public string Literal { get; set; }
  }
}
