using System;
using System.Text;

namespace JsonAot.Parser
{
    public class JsonTokenizer
    {
        private readonly string _input;
        private int _pos;

        public JsonTokenizer(string input)
        {
            _input = input ?? "";
            _pos = 0;
        }

        public Token GetNextToken()
        {
            SkipWhitespace();

            if (_pos >= _input.Length)
                return new Token(TokenType.EOF);

            char c = _input[_pos];
            switch (c)
            {
                case '{':
                    _pos++;
                    return new Token(TokenType.LBrace);
                case '}':
                    _pos++;
                    return new Token(TokenType.RBrace);
                case '[':
                    _pos++;
                    return new Token(TokenType.LBracket);
                case ']':
                    _pos++;
                    return new Token(TokenType.RBracket);
                case ',':
                    _pos++;
                    return new Token(TokenType.Comma);
                case ':':
                    _pos++;
                    return new Token(TokenType.Colon);
                case '"':
                    return ParseString();
                default:
                    {
                        // Para booleans e null: 
                        // Se for 't', 'T', 'f', 'F', 'n', 'N', tentamos parsear
                        if (c == 't' || c == 'T')
                            return ParseTrue();
                        if (c == 'f' || c == 'F')
                            return ParseFalse();
                        if (c == 'n' || c == 'N')
                            return ParseNull();

                        // Caso seja dígito ou '-', parse de número
                        if (IsDigit(c) || c == '-')
                            return ParseNumber();

                        throw new Exception($"Caractere inesperado: '{c}' na posição {_pos}.");
                    }
            }
        }

        private void SkipWhitespace()
        {
            while (_pos < _input.Length && char.IsWhiteSpace(_input[_pos]))
                _pos++;
        }

        private bool IsDigit(char c) => (c >= '0' && c <= '9');

        private Token ParseString()
        {
            // Consumir aspas iniciais
            _pos++;
            var sb = new StringBuilder();

            while (_pos < _input.Length)
            {
                char c = _input[_pos];
                _pos++;

                if (c == '\\')
                {
                    // tratar escapes
                    if (_pos >= _input.Length)
                        throw new Exception("String JSON incompleta (escape no final).");

                    char escaped = _input[_pos];
                    _pos++;

                    switch (escaped)
                    {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        default:
                            // se achar algo fora do comum, apenas adiciona.
                            sb.Append(escaped);
                            break;
                    }
                }
                else if (c == '"')
                {
                    // Fim da string
                    return new Token(TokenType.String, sb.ToString());
                }
                else
                {
                    sb.Append(c);
                }
            }

            // Se saiu do while, não encontramos aspas finais:
            throw new Exception("String JSON sem aspas de fechamento.");
        }

        private Token ParseNumber()
        {
            int start = _pos;
            // se tiver sinal '-'
            if (_input[_pos] == '-') _pos++;

            while (_pos < _input.Length && (IsDigit(_input[_pos]) || _input[_pos] == '.'))
                _pos++;

            string numStr = _input.Substring(start, _pos - start);
            return new Token(TokenType.Number, numStr);
        }

        private Token ParseTrue()
        {
            // Precisamos checar se restam chars suficientes (4) para "true".
            // E converter para lowercase para comparar.
            int lengthLeft = _input.Length - _pos;
            if (lengthLeft >= 4)
            {
                string maybeTrue = _input.Substring(_pos, 4).ToLower();
                if (maybeTrue == "true")
                {
                    _pos += 4;
                    return new Token(TokenType.True, "true");
                }
            }
            throw new Exception("Token 'true' inválido na posição " + _pos);
        }

        private Token ParseFalse()
        {
            int lengthLeft = _input.Length - _pos;
            if (lengthLeft >= 5)
            {
                string maybeFalse = _input.Substring(_pos, 5).ToLower();
                if (maybeFalse == "false")
                {
                    _pos += 5;
                    return new Token(TokenType.False, "false");
                }
            }
            throw new Exception("Token 'false' inválido na posição " + _pos);
        }

        private Token ParseNull()
        {
            int lengthLeft = _input.Length - _pos;
            if (lengthLeft >= 4)
            {
                string maybeNull = _input.Substring(_pos, 4).ToLower();
                if (maybeNull == "null")
                {
                    _pos += 4;
                    return new Token(TokenType.Null, "null");
                }
            }
            throw new Exception("Token 'null' inválido na posição " + _pos);
        }
    }
}
