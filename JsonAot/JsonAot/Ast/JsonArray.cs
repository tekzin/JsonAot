using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonAot.Ast
{
    public class JsonArray : JsonNode
    {
        public List<JsonNode> Items { get; } = new List<JsonNode>();
    }
}
