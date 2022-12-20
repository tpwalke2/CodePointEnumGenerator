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
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "GENERATING",
                        $"Generating enum for {file.Path}",
                        "",
                        "",
                        DiagnosticSeverity.Info,
                        true),
                    null));
            var (enumName, contents) = GenerateEnumFile(file);
            context.AddSource($"{enumName}.g.cs", SourceText.From(contents, Encoding.UTF8));
        }
    }

    private static IEnumerable<AdditionalText> GetCodePointFiles(GeneratorExecutionContext context) =>
        context.AdditionalFiles.Where(file => Path.GetExtension(file.Path)
                                                  .Equals(".codepoints", StringComparison.OrdinalIgnoreCase));

    private static (string enumName, string contents) GenerateEnumFile(AdditionalText file)
    {
        var enumName = Path.GetFileName(file.Path)
                           .Replace(".codepoints", "", StringComparison.OrdinalIgnoreCase)
                           .Replace("-", "");
        
        var seenEnums = new Dictionary<string, int>();
        var values    = new List<(string, string)>();

        foreach (var line in file.GetText()!
                                 .ToString()
                                 .Split(
                                     new[] { "\r\n", "\r", "\n" },
                                     StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            var parsed = line.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parsed.Length != 2) continue;

            var enumValueName = parsed[0].ToEnumEntry();
            if (!seenEnums.ContainsKey(enumValueName))
            {
                seenEnums[enumValueName] = 1;
            }
            else
            {
                seenEnums[enumValueName]++;
                enumValueName = $"{enumValueName}{seenEnums[enumValueName]}";
            }

            values.Add((enumValueName, parsed[1]));
        }

        var sb = new StringBuilder();

        sb.Append($@"
namespace {file.Path.ToNamespace()};

public enum {enumName} {{
");

        sb.AppendJoin(",\n", values.Select(tuple => $@"    {tuple.Item1} = 0x{tuple.Item2}"));

        sb.Append(@"
}");
        
        return (enumName, sb.ToString());
    }
}