using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;

namespace CodePointEnumGenerator.Helpers;

public static class AdditionalTextExtensions
{
    public static string GenerateEnumName(this AdditionalText file) => Path.GetFileName(file.Path)
                                                                           .Replace(".codepoints",
                                                                               "",
                                                                               StringComparison.OrdinalIgnoreCase)
                                                                           .Replace("-", "");
    
    public static IEnumerable<(string, string)> GetEnumValues(this AdditionalText file)
    {
        var seenEnumValues = new Dictionary<string, int>();
        var values         = new List<(string, string)>();

        foreach (var line in file.GetText()!
                                 .ToString()
                                 .Split(
                                     new[] { "\r\n", "\r", "\n" },
                                     StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            var parsed = line.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parsed.Length != 2) continue;

            var enumValueName = parsed[0].ToEnumEntry();
            if (!seenEnumValues.ContainsKey(enumValueName))
            {
                seenEnumValues[enumValueName] = 1;
            }
            else
            {
                seenEnumValues[enumValueName]++;
                enumValueName = $"{enumValueName}{seenEnumValues[enumValueName]}";
            }

            values.Add((enumValueName, parsed[1]));
        }

        return values;
    }
}
