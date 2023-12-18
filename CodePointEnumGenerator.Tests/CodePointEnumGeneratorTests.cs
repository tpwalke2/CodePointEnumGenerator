using System.Linq;
using Xunit;

namespace CodePointEnumGenerator.Tests;

public class CodePointEnumGeneratorTests
{
    [Fact]
    public void ShouldBuildEnumForCodePoints()
    {
        var result = GeneratorTestFactory.RunGenerator(
            "",
            ("TestEnum.codepoints", """
                                    9mp e979
                                    abc eb94
                                    ac_unit eb3b
                                    access_alarm e190
                                    access_alarms e191
                                    access_time e192
                                    access_time_filled efd6
                                    """));
        Assert.NotNull(
            result.RunResult?.GeneratedTrees.FirstOrDefault(t => t.FilePath.Contains("TestEnum")));
    }
}
