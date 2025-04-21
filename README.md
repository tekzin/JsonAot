<h1 align="center">JsonAot</h1>
<p align="center">
  <em>Parser / Serializer JSON projetado para AOT • Zero Reflection • .NET 7 → 9</em>
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/JsonAot">
    <img alt="NuGet" src="https://img.shields.io/nuget/v/JsonAot.svg?logo=nuget">
  </a>
  <a href="https://github.com/tekzin/JsonAot/actions">
    <img alt="Build" src="https://img.shields.io/github/actions/workflow/status/tekzin/JsonAot/ci.yml?label=build">
  </a>
  <img alt="License" src="https://img.shields.io/badge/license-MIT-green">
</p>

---

## ✨ Por que JsonAot?
| 🎯 Foco | Descrição |
|---------|-----------|
| **AOT‑friendly** | Sem `System.Reflection` ou `dynamic`. Funciona em IL2CPP, iOS, WASM. |
| **Leve** | Nenhuma dependência externa. Apenas BCL. |
| **Rápido** | Tokenizador em `Span<char>` (sem alocações excessivas). |
| **Multi‑target** | `net7.0`, `net8.0`, `net9.0` (pronto para futuras versões). |
| **100 % C#** | Fácil de estender; código limpo e comentado. |

---

## 🚀 Instalação
```bash
dotnet add package JsonAot
```
> Prefere o Package Manager?  
> `Install-Package JsonAot`

---

## ⚡ Uso Rápido

```csharp
using JsonAot.Ast;
using JsonAot.Serializer;

// Parse → AST
JsonNode root = JsonDeserializer.Parse(@"{
  "name":"Neo",
  "skills": ["C#","Hacking"],
  "chosenOne": true
}");

// Navegação
var obj = (JsonObject)root;
string name  = ((JsonString)obj.Properties["name"]).Value;        // "Neo"
bool chosen  = ((JsonBoolean)obj.Properties["chosenOne"]).Value;  // true

// Construção manual + serialização
var newObj = new JsonObject
{
    Properties = {
        ["hello"] = new JsonString("world")
    }
};
string json = JsonDeserializer.Serialize(newObj); // {"hello":"world"}
```

---

## 🛠️ API Essencial

| Tipo | Papel |
|------|-------|
| `JsonTokenizer` | Converte string → tokens (`{`, `true`, `number` …) |
| `JsonParser`    | Tokens → AST (`JsonObject`, `JsonArray` …) |
| `JsonDeserializer.Parse(string)` | Atalho para obter a AST |
| `JsonDeserializer.Serialize(JsonNode)` | AST → string JSON |

### Estrutura da AST
```text
JsonNode
 ├─ JsonObject   → Dictionary<string, JsonNode>
 ├─ JsonArray    → List<JsonNode>
 ├─ JsonString   → string Value
 ├─ JsonNumber   → double Value
 ├─ JsonBoolean  → bool Value
 └─ JsonNull
```

---

## 📈 Benchmarks (micro)
| Cenário | JsonAot | System.Text.Json | Newtonsoft |
|---------|---------|------------------|------------|
| Parse 1 KB  | **1.0×** (baseline) | 1.4× | 2.1× |
| Stringify 1 KB | **1.0×** | 1.6× | 2.3× |
> *Máquina: Ryzen 5700U · .NET 8 · Release*. Números relativos (↓ é melhor).

---

## 🗺️ Roadmap
- [ ] Unicode `\uXXXX` completo  
- [ ] Gerador de código (SourceGenerator) para POCO ↔ AST  
- [ ] Validação de schema leve  

---

## 🤝 Contribuindo
1. **Fork** → `git checkout -b feature/sua-feature`  
2. `dotnet test` (garanta tudo verde)  
3. *Pull request* seguindo Conventional Commits  
4. Respeite o `.editorconfig` (`dotnet format`)

---

## 📜 Licença
Distribuído sob licença **MIT**. Consulte `LICENSE` para detalhes.

<div align="center"><sub>Feito com 💙 por José Vitor • 2025</sub></div>
