# Testing

## Framework
xunit 2.9.3 in `CodePointEnumGenerator.Tests`, targets `net10.0` (generator itself targets `netstandard2.0`). Test project references the generator via `ProjectReference`.

## Layout

| File | Covers |
|---|---|
| `CodePointEnumGeneratorTests.cs` | End-to-end: runs the generator via `GeneratorTestFactory` and checks emitted enum source |
| `GeneratorTestFactory.cs` | Test harness — builds a `CSharpCompilation`, runs `CodePointEnumGenerator` through `CSharpGeneratorDriver` against fake `AdditionalText` files, returns compilation/diagnostics/run-result |
| `AdditionalFile.cs` | Minimal `AdditionalText` implementation for feeding fake `.codepoints` content into the driver |
| `Helpers/GetEnumFileNameTests.cs` | `AdditionalTextExtensions.GetEnumFileName()` |
| `Helpers/GetEnumValuesTests.cs` | `StringExtensions.GetEnumValues()` |
| `Helpers/ToEnumEntryTests.cs` | `StringExtensions.ToEnumEntry()` |
| `Helpers/ToNamespaceTests.cs` | `StringExtensions.ToNamespace()` |

## Running the generator in tests
`GeneratorTestFactory.RunGenerator(source, additionalFiles...)`:
- Parses `source` as a syntax tree, compiles against `object` + the generator assembly.
- Pre-verifies diagnostics, ignoring a fixed set of expected errors (`CS0012`, `CS0616`, `CS0246`, `CS0103` — incomplete-reference noise from the minimal test compilation).
- Wraps each `(path, contents)` tuple as an `AdditionalFile` and runs `CodePointEnumGenerator` through `CSharpGeneratorDriver.RunGeneratorsAndUpdateCompilation`.
- Returns `(Compilation, (DiagnosticsBefore, DiagnosticsAfter), GeneratorDriverRunResult)` for assertions.

## Running tests
Via Cake: `Task("Test")` in `build.cake` runs `dotnet test` (framework `net10.0`, `trx` logger) against every `*Tests.csproj`, in parallel (`build/TestOperations.cake`, `build/TestProject.cake`). Locally: `dotnet test` from repo root or the test project directly.

---
*Last updated: 2026-09-22 | Verified against: bb6a64c*
