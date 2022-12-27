using CodePointEnumGenerator.Helpers;
using System.Linq;
using Xunit;

namespace CodePointEnumGenerator.Tests.Helpers;

public class GetEnumValuesTests
{
    [Fact]
    public void MultipleUniqueValues()
    {
        const string content = @"add e001
delete e002";
        var result = content.GetEnumValues().ToArray();
        
        Assert.Equal(2, result.Length);
        Assert.Equal("Add", result[0].Item1);
        Assert.Equal("e001", result[0].Item2);
        Assert.Equal("Delete", result[1].Item1);
        Assert.Equal("e002", result[1].Item2);
    }
    
    [Fact]
    public void RemoveUnderscores()
    {
        const string content = @"add_comment e001
delete_comment_by_user e002";
        var result = content.GetEnumValues().ToArray();
        
        Assert.Equal(2, result.Length);
        Assert.Equal("AddComment", result[0].Item1);
        Assert.Equal("e001", result[0].Item2);
        Assert.Equal("DeleteCommentByUser", result[1].Item1);
        Assert.Equal("e002", result[1].Item2);
    }
    
    [Fact]
    public void DuplicateValues()
    {
        const string content = @"add e001
add e002";
        var result = content.GetEnumValues().ToArray();
        
        Assert.Equal(2, result.Length);
        Assert.Equal("Add", result[0].Item1);
        Assert.Equal("e001", result[0].Item2);
        Assert.Equal("Add2", result[1].Item1);
        Assert.Equal("e002", result[1].Item2);
    }
    
    [Fact]
    public void MissingValue()
    {
        const string content = @"add
delete e002";
        var result = content.GetEnumValues().ToArray();
        
        Assert.Single(result);
        Assert.Equal("Delete", result[0].Item1);
        Assert.Equal("e002", result[0].Item2);
    }
    
    [Fact]
    public void NumericPrefix()
    {
        const string content = @"1add e001
2delete e002";
        var result = content.GetEnumValues().ToArray();
        
        Assert.Equal(2, result.Length);
        Assert.Equal("OneAdd", result[0].Item1);
        Assert.Equal("e001", result[0].Item2);
        Assert.Equal("TwoDelete", result[1].Item1);
        Assert.Equal("e002", result[1].Item2);
    }
}
