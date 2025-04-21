using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonAot.Ast
{
    public class JsonString : JsonNode
    {
        public string Value { get; }

        public JsonString(string value)
        {
            Value = value;
        }
    }
}
