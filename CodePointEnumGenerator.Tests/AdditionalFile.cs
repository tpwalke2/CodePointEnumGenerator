using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Threading;

namespace CodePointEnumGenerator.Tests;

public sealed class AdditionalFile : AdditionalText
{
    private readonly SourceText _sourceText;
        
    public AdditionalFile(string path, string contents)
    {
        Path        = path;
        _sourceText = SourceText.From(contents, Encoding.UTF8);
    }

    public override SourceText GetText(CancellationToken cancellationToken = new()) =>
        _sourceText;

    public override string Path { get; }
}
