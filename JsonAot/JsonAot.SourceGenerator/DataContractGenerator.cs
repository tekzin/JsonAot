using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Esse namespace e classe podem ter outro nome, mas o essencial é implementar `ISourceGenerator`.
namespace JsonAot.SourceGenerator
{
    [Generator]
    public class DataContractGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // Registra um "SyntaxReceiver" que localiza todas as classes marcadas [DataContract].
            context.RegisterForSyntaxNotifications(() => new DataContractSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // Recupera o receiver com os candidatos localizados
            if (!(context.SyntaxReceiver is DataContractSyntaxReceiver receiver)) return;

            // Itera cada classe candidata
            foreach (var classDecl in receiver.Classes)
            {
                // Faz o parse semântico
                var model = context.Compilation.GetSemanticModel(classDecl.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(classDecl) as ITypeSymbol;
                if (symbol == null) continue;

                // Verifica se tem [DataContract] de fato
                if (!HasDataContractAttribute(symbol)) continue;

                // Monta o nome completo (namespace + classe)
                string fullName = symbol.ToString(); // ex: "MyNamespace.UserData"
                string className = symbol.Name;       // ex: "UserData"
                string namespaceName = symbol.ContainingNamespace.ToString(); // ex: "MyNamespace"

                // Mapeia propriedades [DataMember]
                var propInfos = new List<PropertyInfo>();
                foreach (var memberSymbol in symbol.GetMembers().OfType<IPropertySymbol>())
                {
                    if (HasDataMemberAttribute(memberSymbol))
                    {
                        // Tenta descobrir se é string, bool, etc.
                        var propType = memberSymbol.Type.ToString();
                        var dataMemberName = GetDataMemberName(memberSymbol)
                            ?? memberSymbol.Name; // fallback no nome normal

                        propInfos.Add(new PropertyInfo
                        {
                            Name = memberSymbol.Name,
                            TypeName = propType,
                            JsonKey = dataMemberName
                        });
                    }
                }

                // Gera o código-fonte
                string generatedCode = GenerateClassCode(namespaceName, className, propInfos);
                // Adiciona ao compilation
                context.AddSource($"{className}_JsonAotGenerated.cs", generatedCode);
            }
        }

        // Verifica se a classe está marcada com [DataContract]
        private bool HasDataContractAttribute(ISymbol symbol)
        {
            return symbol.GetAttributes().Any(a => a.AttributeClass.ToString().Contains("DataContract"));
        }

        // Verifica se a propriedade tem [DataMember]
        private bool HasDataMemberAttribute(ISymbol symbol)
        {
            return symbol.GetAttributes().Any(a => a.AttributeClass.ToString().Contains("DataMember"));
        }

        // Se tiver [DataMember(Name = "xxx")] pega esse "xxx"
        private string GetDataMemberName(ISymbol symbol)
        {
            var attr = symbol.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass.ToString().Contains("DataMember"));
            if (attr == null) return null;

            foreach (var arg in attr.NamedArguments)
            {
                if (arg.Key == "Name")
                {
                    return arg.Value.Value as string;
                }
            }
            return null;
        }

        private string GenerateClassCode(string namespaceName, string className, List<PropertyInfo> props)
        {
            // Exemplo: gerar 2 métodos estáticos: 
            // 1) T FromJson(JsonNode) => popula as props. 
            // 2) JsonNode ToJson(T).
            // Esse é um mini-exemplo; substitua pela lógica do seu parser/serializer AST.

            var sb = new StringBuilder();
            sb.AppendLine("using JsonAot.Ast;");
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {namespaceName}");
            sb.AppendLine("{");
            sb.AppendLine($"    public static partial class {className}JsonAotGenerated");
            sb.AppendLine("    {");
            sb.AppendLine($"        public static " +
                          $"{className} FromJson(JsonNode node)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var result = new {className}();");
            sb.AppendLine("            if (node is JsonObject obj)");
            sb.AppendLine("            {");
            foreach (var p in props)
            {
                // Exemplo: if (obj.Properties.TryGetValue("myKey", out var n) && n is JsonString s) { result.MyProp = s.Value; }
                // Precisamos tratar cada tipo especificamente. Aqui, simplificado.
                sb.AppendLine(
$@"                if (obj.Properties.TryGetValue(""{p.JsonKey}"", out var {p.Name.ToLower()}Node))
                {{
                    {(p.TypeName == "System.String" ?
                        $@"if ({p.Name.ToLower()}Node is JsonString str) result.{p.Name} = str.Value;" :
                    p.TypeName == "System.Boolean" ?
                        $@"if ({p.Name.ToLower()}Node is JsonBoolean b) result.{p.Name} = b.Value;" :
                    p.TypeName == "System.Double" || p.TypeName == "double" ?
                        $@"if ({p.Name.ToLower()}Node is JsonNumber num) result.{p.Name} = num.Value;" :
                    $@"// Exemplo simplificado. Precisaria tratar arrays, objetos custom, etc."
                    )}
                }}
");
            }
            sb.AppendLine("            }");
            sb.AppendLine("            return result;");
            sb.AppendLine("        }");
            sb.AppendLine();
            // Gerar o ToJson
            sb.AppendLine($"        public static JsonNode ToJson({className} obj)");
            sb.AppendLine("        {");
            sb.AppendLine("            var jObj = new JsonObject();");
            foreach (var p in props)
            {
                // Exemplo para string/bool/numero. 
                sb.AppendLine(
                    p.TypeName == "System.String" ?
                    $@"            jObj.Properties[""{p.JsonKey}""] = obj.{p.Name} == null ? new JsonNull() : new JsonString(obj.{p.Name});" :
                    p.TypeName == "System.Boolean" ?
                    $@"            jObj.Properties[""{p.JsonKey}""] = new JsonBoolean(obj.{p.Name});" :
                    (p.TypeName == "System.Double" || p.TypeName == "double") ?
                    $@"            jObj.Properties[""{p.JsonKey}""] = new JsonNumber(obj.{p.Name});" :
                    $@"            // Precisaria tratar arrays, objetos custom, etc. 
            jObj.Properties[""{p.JsonKey}""] = new JsonNull();"
                );
            }
            sb.AppendLine("            return jObj;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }

    // Esse "receiver" apenas armazena todas as classes que tenham [DataContract] em nível de sintaxe
    internal class DataContractSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> Classes { get; } = new List<ClassDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // Pega class
            if (syntaxNode is ClassDeclarationSyntax cds)
            {
                // verifica se tem [DataContract]
                // mas sem semântica, a gente só olha se tem ANY attribute "DataContract"
                var hasDataContract = cds.AttributeLists
                    .SelectMany(a => a.Attributes)
                    .Any(attr => attr.Name.ToString().Contains("DataContract"));

                if (hasDataContract)
                    Classes.Add(cds);
            }
        }
    }

    internal class PropertyInfo
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public string JsonKey { get; set; }
    }
}
