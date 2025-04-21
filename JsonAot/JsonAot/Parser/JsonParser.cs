using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonAot.Ast;

namespace JsonAot.Parser
{
    public class JsonParser
    {
        private readonly JsonTokenizer _tokenizer;
        private Token _currentToken;

        public JsonParser(string input)
        {
            _tokenizer = new JsonTokenizer(input);
            _currentToken = _tokenizer.GetNextToken();
        }

        private void Advance() => _currentToken = _tokenizer.GetNextToken();

        public JsonNode Parse()
        {
            return ParseValue();
        }

        private JsonNode ParseValue()
        {
            switch (_currentToken.Type)
            {
                case TokenType.LBrace:
                    return ParseObject();
                case TokenType.LBracket:
                    return ParseArray();
                case TokenType.String:
                    var strValue = _currentToken.Value ?? "";
                    Advance();
                    return new JsonString(strValue);
                case TokenType.Number:
                    var numValue = double.Parse(_currentToken.Value!);
                    Advance();
                    return new JsonNumber(numValue);
                case TokenType.True:
                    Advance();
                    return new JsonBoolean(true);
                case TokenType.False:
                    Advance();
                    return new JsonBoolean(false);
                case TokenType.Null:
                    Advance();
                    return new JsonNull();
                default:
                    throw new System.Exception($"Token inesperado: {_currentToken.Type}");
            }
        }

        private JsonNode ParseObject()
        {
            // consumindo '{'
            Advance();
            var obj = new JsonObject();

            // se vier '}', é objeto vazio
            if (_currentToken.Type == TokenType.RBrace)
            {
                Advance();
                return obj;
            }

            while (true)
            {
                if (_currentToken.Type != TokenType.String)
                    throw new System.Exception("Esperado string para chave do objeto");

                var propName = _currentToken.Value!;
                Advance(); // consome a string

                if (_currentToken.Type != TokenType.Colon)
                    throw new System.Exception("Esperado ':' após chave do objeto");

                Console.WriteLine($"[ParseObject] _currentToken={_currentToken.Type}, value='{_currentToken.Value}'");

                Advance(); // consome ':'
                var value = ParseValue();

                obj.Properties[propName] = value;

                if (_currentToken.Type == TokenType.Comma)
                {
                    Advance(); // consome ','
                    continue;
                }
                else if (_currentToken.Type == TokenType.RBrace)
                {
                    Advance(); // consome '}'
                    break;
                }
                else
                {
                    throw new System.Exception("Esperado ',' ou '}' no objeto");
                }
            }

            return obj;
        }

        private JsonNode ParseArray()
        {
            // consumindo '['
            Advance();
            var arr = new JsonArray();

            if (_currentToken.Type == TokenType.RBracket)
            {
                Advance();
                return arr;
            }

            while (true)
            {
                var value = ParseValue();
                arr.Items.Add(value);

                if (_currentToken.Type == TokenType.Comma)
                {
                    Advance(); // consome ','
                    continue;
                }
                else if (_currentToken.Type == TokenType.RBracket)
                {
                    Advance(); // consome ']'
                    break;
                }
                else
                {
                    throw new System.Exception("Esperado ',' ou ']' no array");
                }
            }

            return arr;
        }
    }
}
