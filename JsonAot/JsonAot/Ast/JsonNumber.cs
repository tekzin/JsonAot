using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonAot.Ast
{
    public class JsonNumber : JsonNode
    {
        public double Value { get; }

        public JsonNumber(double value)
        {
            Value = value;
        }
    }
}
