
**Detalhes de estilo**

| Elemento | Dica |
|----------|------|
| **Badges** | NuGet, build (GitHub Actions), cobertura (Codecov). |
| **Títulos curtos** | `<h1>` com tagline; subseções `##`. |
| **Blocos de código** | Use ```csharp ``` para sintaxe. |
| **Tabela de recursos** | 🗸 Visual clean, bullet emojis opcionais. |
| **TOC automático** | GitHub gera; ou use `markdown-toc` se quiser fixo. |

---

### 4 ▪ Visão de futuro

1. **GitHub Actions** – workflow yaml:  
   - Trigger `on: push tags`.  
   - Jobs: `dotnet test`, `dotnet pack`, `dotnet nuget push`.  
2. **Docs site** – Ative **GitHub Pages** + MkDocs ou apenas README como landing.  
3. **SemVer disciplinado** – `MAJOR.MINOR.PATCH` alinhado a tag Git + versão do pacote.

Com esses quatro blocos (NuGet, Git, README, automação) você lança o **JsonAot** de forma profissional e escalável. 🚀
