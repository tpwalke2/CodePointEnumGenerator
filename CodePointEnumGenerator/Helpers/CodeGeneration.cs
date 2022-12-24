using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodePointEnumGenerator.Helpers;

public static class CodeGeneration
{
    public static string BuildEnumFileContents(
        string enumName,
        IEnumerable<(string, string)> values,
        string @namespace) => new StringBuilder()
                              .Append($@"
namespace {@namespace};

public enum {enumName} {{
")
                              .AppendJoin(",\n", values.Select(tuple => $@"    {tuple.Item1} = 0x{tuple.Item2}"))
                              .Append(@"
}")
                              .ToString();
}
