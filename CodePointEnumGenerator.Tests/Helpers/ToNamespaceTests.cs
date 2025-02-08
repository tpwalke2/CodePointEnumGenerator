using CodePointEnumGenerator.Helpers;
using Xunit;

namespace CodePointEnumGenerator.Tests.Helpers;

public class ToNamespaceTests
{
    [Theory]
    [InlineData(@"SongBook.UI.MaterialIcons\Fonts\MaterialIcons-Regular.codepoints", "SongBook.UI.MaterialIcons.Fonts")]
    [InlineData(@"C:\Source\SongBook.UI.MaterialIcons\Fonts\MaterialIcons-Regular.codepoints",
                "SongBook.UI.MaterialIcons.Fonts")]
    [InlineData(@"Source\SongBook.UI.MaterialIcons\Fonts\MaterialIcons-Regular.codepoints",
                "SongBook.UI.MaterialIcons.Fonts")]
    [InlineData(@"C:\Source\SongBook\Fonts\", "Source.SongBook.Fonts")]
    [InlineData(@"C:\Source\SongBook\Fonts", "Source.SongBook")]
    [InlineData(@"C:\", "")]
    [InlineData("C:", "")]
    [InlineData("", "")]
    public void ToNamespace(string input, string expectedOutput)
    {
        Assert.Equal(expectedOutput, input.ToNamespace());
    }
}