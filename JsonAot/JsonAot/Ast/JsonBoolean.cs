using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonAot.Ast
{
    public class JsonBoolean : JsonNode
    {
        public bool Value { get; }

        public JsonBoolean(bool value)
        {
            Value = value;
        }
    }
}
