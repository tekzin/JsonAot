using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonAot.Ast
{
    public class JsonObject : JsonNode
    {
        public Dictionary<string, JsonNode> Properties { get; }

        public JsonObject()
        {
            Properties = new Dictionary<string, JsonNode>();
        }
    }
}
