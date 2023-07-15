using CodePointEnumGenerator.Helpers;
using Xunit;

namespace CodePointEnumGenerator.Tests.Helpers;

public class ToEnumEntryTests
{
    [Theory]
    [InlineData("add_comment", "AddComment")]
    [InlineData("add", "Add")]
    // ReSharper disable twice StringLiteralTypo
    [InlineData("addcomment", "Addcomment")]
    [InlineData("1k_plus", "OneKPlus")]
    [InlineData("2k_plus", "TwoKPlus")]
    [InlineData("10k_plus", "TenKPlus")]
    [InlineData("11k_plus", "ElevenKPlus")]
    public void ToEnumEntry(string input, string expectedOutput)
    {
        Assert.Equal(expectedOutput, input.ToEnumEntry());
    }
}
