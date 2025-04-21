using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonAot.Parser
{
    public enum TokenType
    {
        LBrace,    // {
        RBrace,    // }
        LBracket,  // [
        RBracket,  // ]
        Comma,     // ,
        Colon,     // :
        String,
        Number,
        True,
        False,
        Null,
        EOF        // Fim do arquivo
    }

    public class Token
    {
        public TokenType Type { get; }
        public string? Value { get; } // usado para String, Number, etc.

        public Token(TokenType type, string? value = null)
        {
            Type = type;
            Value = value;
        }
    }
}
