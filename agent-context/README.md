# CodePointEnumGenerator — Agent Context

Roslyn incremental source generator (C#, `netstandard2.0`). Reads font `.codepoints` files marked as `AdditionalFiles` and emits one strongly-typed enum per file mapping glyph names to hex byte values. Packed and shipped as a NuGet analyzer package. Built with Cake, CI on GitHub Actions, releases via GitHub + NuGet.

## Context docs

| Doc | Covers |
|---|---|
| `context/architecture.md` | Stack, layout, generator pipeline, helpers, `.codepoints` format |
| `context/testing.md` | xunit test layout, `GeneratorTestFactory` harness, how to run tests |
| `context/build-and-release.md` | Cake pipeline, GitHub Actions CI, versioning, GitHub/NuGet release flow |

## Existing human docs
Root `README.md` — package usage snippet, troubleshooting, build pipeline requirements.

## Document Scopes

| Document | Codebase paths watched |
|---|---|
| `context/architecture.md` | `CodePointEnumGenerator/`, `CodePointEnumGenerator.csproj`, `CodePointEnumGenerator.sln` |
| `context/testing.md` | `CodePointEnumGenerator.Tests/` |
| `context/build-and-release.md` | `build.cake`, `build/`, `bootstrap.ps1`, `.github/workflows/`, `.config/dotnet-tools.json` |

---
*Last updated: 2026-09-22 | Verified against: bb6a64c*
