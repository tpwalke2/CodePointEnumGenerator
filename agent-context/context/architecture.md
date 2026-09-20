# Architecture

## What this is
Roslyn incremental source generator. Scans project `AdditionalFiles` ending in `.codepoints`, emits one C# enum per file mapping glyph names to hex byte values (for strongly-typed font glyph references).

## Tech stack

| Layer | Technology |
|---|---|
| Generator project | `netstandard2.0`, C# 14, `Microsoft.CodeAnalysis.CSharp` 5.9.0 |
| Test project | `net10.0`, xunit 2.9.3, `Microsoft.CodeAnalysis.CSharp.SourceGenerators.Testing.XUnit` |
| Build | Cake (`build.cake` + `build/*.cake`), driven by `bootstrap.ps1` |
| CI | AppVeyor (`.appveyor.yml`), Visual Studio 2026 image, builds only `main` |
| Packaging | NuGet (`CodePointEnumGenerator` package), GitHub Releases on publish |

## Top-level layout

| Path | Purpose |
|---|---|
| `CodePointEnumGenerator/` | The generator itself, packed as analyzer |
| `CodePointEnumGenerator/CodePointEnumGenerator.cs` | `IIncrementalGenerator` entry point |
| `CodePointEnumGenerator/Helpers/` | Pure helper logic (name parsing, code emission, number-to-words) |
| `CodePointEnumGenerator.Tests/` | xunit tests against generator and helpers |
| `build/` | Cake build scripts (config, paths, build/test/publish operations, GitHub API models) |
| `build.cake` | Cake entry: Clean → Build → Test → Publish tasks |
| `bootstrap.ps1` | Restores `dotnet tool` (cake.tool) and runs `build.cake` |
| `output/artifacts/` | Packed `.nupkg` output |
| `.config/dotnet-tools.json` | Pins `cake.tool` 3.0.0 |

## Generator pipeline (`CodePointEnumGenerator/CodePointEnumGenerator.cs`)

1. `GetCodePointFiles` filters `context.AdditionalTextsProvider` to paths ending `.codepoints`.
2. For each file, project a tuple:
   - `Name` = `AdditionalTextExtensions.GetEnumFileName()` — filename minus `.codepoints`, minus `-`.
   - `Content` = file text run through `StringExtensions.GetEnumValues()` — parses `name value` lines into `(enumEntryName, hexValue)` tuples, deduping names by suffixing a counter.
   - `Namespace` = `StringExtensions.ToNamespace()` — derives a namespace from the file's folder path (skips drive letter, starts at first segment containing a `.`).
3. `context.RegisterSourceOutput` emits `{Name}.g.cs` via `CodeGeneration.BuildEnumFileContents`, which renders `namespace {ns}; public enum {Name} { ENTRY = 0xVALUE, ... }`.

## Helpers (`CodePointEnumGenerator/Helpers/`)

| File | Responsibility |
|---|---|
| `AdditionalTextExtensions.cs` | `GetEnumFileName()` — derive enum type name from file path |
| `StringExtensions.cs` | `ToNamespace()`, `ToEnumEntry()` (name → PascalCase enum member, numeric prefixes spelled out via `EnglishNumberToWordsConverter`), `GetEnumValues()` (parse `.codepoints` file contents) |
| `CodeGeneration.cs` | `BuildEnumFileContents()` — renders the final enum source text |
| `EnglishNumberToWordsConverter.cs` | Internal; spells out numeric enum-name prefixes (e.g. `1_foo` → `OneFoo`). Adapted from Humanizer |

## `.codepoints` file format
Plain text, one glyph per line: `<name> <hex-value>` (space-separated, exactly 2 tokens). Names run through `ToEnumEntry`: leading digits spelled as words, `_`-separated words PascalCased and concatenated.

## Consumer usage
Consuming projects reference the package with `OutputItemType="Analyzer"` and mark `.codepoints` files with build action `AdditionalFiles`. See root `README.md` for the exact snippet and troubleshooting steps.

---
*Last updated: 2026-09-20 | Verified against: 6b6c9cc*
