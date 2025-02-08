using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Threading;

namespace CodePointEnumGenerator.Tests;

public sealed class AdditionalFile(string path, string contents) : AdditionalText
{
    private readonly SourceText _sourceText = SourceText.From(contents, Encoding.UTF8);

    public override SourceText GetText(CancellationToken cancellationToken = new()) =>
        _sourceText;

    public override string Path { get; } = path;
}