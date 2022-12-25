using CodePointEnumGenerator.Helpers;
using Xunit;

namespace CodePointEnumGenerator.Tests.Helpers;

public class GetEnumFileNameTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("Arial.codepoints", "Arial")]
    [InlineData("MaterialIcons-Regular.codepoints", "MaterialIconsRegular")]
    public void GetEnumFileName(string filePath, string expectedFilename)
    {
        var file = new AdditionalFile(filePath, "");
        
        Assert.Equal(expectedFilename, file.GetEnumFileName());
    }
}
