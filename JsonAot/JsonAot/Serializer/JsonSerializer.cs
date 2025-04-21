using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonAot.Ast;

namespace JsonAot.Serializer
{
    public static class JsonSerializer
    {
        public static string Serialize(JsonNode node)
        {
            return SerializeNode(node);
        }

        private static string SerializeNode(JsonNode node)
        {
            switch (node)
            {
                case JsonObject obj:
                    return SerializeObject(obj);
                case JsonArray arr:
                    return SerializeArray(arr);
                case JsonString str:
                    return $"\"{EscapeString(str.Value)}\"";
                case JsonNumber num:
                    return num.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                case JsonBoolean b:
                    return b.Value ? "true" : "false";
                case JsonNull:
                    return "null";
                default:
                    throw new System.Exception("Tipo de nó JSON desconhecido");
            }
        }

        private static string SerializeObject(JsonObject obj)
        {
            var props = new List<string>();
            foreach (var kvp in obj.Properties)
            {
                string key = $"\"{EscapeString(kvp.Key)}\"";
                string value = SerializeNode(kvp.Value);
                props.Add($"{key}:{value}");
            }
            return $"{{{string.Join(",", props)}}}";
        }

        private static string SerializeArray(JsonArray arr)
        {
            var items = arr.Items.Select(item => SerializeNode(item));
            return $"[{string.Join(",", items)}]";
        }

        private static string EscapeString(string value)
        {
            // se quiser tratar \n, \t, etc.
            return value.Replace("\"", "\\\"");
        }
    }
}
