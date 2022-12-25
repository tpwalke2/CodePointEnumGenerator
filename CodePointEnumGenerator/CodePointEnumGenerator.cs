using CodePointEnumGenerator.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodePointEnumGenerator;

[Generator]
public class CodePointEnumGenerator : ISourceGenerator
{
    private const string CodepointsExtension = ".codepoints";

    public void Initialize(GeneratorInitializationContext context)
    {
        // initialization not required
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var files = GetCodePointFiles(context).ToArray();
        if (!files.Any())
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "NOFILES",
                        "No Codepoint files found",
                        "",
                        "",
                        DiagnosticSeverity.Warning,
                        true),
                    null));
        
        foreach (var file in files)
        {
            var enumName = file.GetEnumFileName();

            context.AddSource(
                $"{enumName}.g.cs",
                SourceText.From(
                    CodeGeneration.BuildEnumFileContents(
                        enumName,
                        file.GetEnumValues(),
                        file.Path.ToNamespace()),
                    Encoding.UTF8));
        }
    }

    private static IEnumerable<AdditionalText> GetCodePointFiles(GeneratorExecutionContext context) =>
        context.AdditionalFiles.Where(file => Path.GetExtension(file.Path)
                                                  .Equals(
                                                      CodepointsExtension,
                                                      StringComparison.OrdinalIgnoreCase));
}