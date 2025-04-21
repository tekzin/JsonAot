using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonAot.Ast;
using JsonAot.Parser;

namespace JsonAot.Serializer
{
    public static class JsonDeserializer
    {
        public static JsonNode Parse(string json)
        {
            var parser = new JsonParser(json);
            return parser.Parse();
        }

        public static T ToObject<T>(string json, Func<JsonNode, T> factory)
        {
            var root = Parse(json);
            return factory(root);
        }
    }
}
