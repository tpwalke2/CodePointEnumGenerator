using CodePointEnumGenerator.Helpers;
using Microsoft.CodeAnalysis;
using System;

namespace CodePointEnumGenerator;

[Generator]
public class CodePointEnumGenerator : IIncrementalGenerator
{
    private const string CodepointsExtension = ".codepoints";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var codePointFiles = GetCodePointFiles(context)
            .Select((text, token) => (
                        Name: text.GetEnumFileName(),
                        Content: text.GetText(token)!
                                     .ToString()
                                     .GetEnumValues(),
                        Namespace: text.Path.ToNamespace()));

        context.RegisterSourceOutput(codePointFiles, (spc, file) =>
        {
            spc.AddSource(
                $"{file.Name}.g.cs",
                CodeGeneration.BuildEnumFileContents(
                    file.Name,
                    file.Content,
                    file.Namespace));
        });
    }

    private static IncrementalValuesProvider<AdditionalText> GetCodePointFiles(
        IncrementalGeneratorInitializationContext context) =>
        context
            .AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(CodepointsExtension, StringComparison.OrdinalIgnoreCase));
}
