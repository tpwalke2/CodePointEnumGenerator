using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CodePointEnumGenerator.Helpers;

public static class StringExtensions
{
    private static string FirstCharToUpper(this string input) =>
        input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => string.Concat(input[0].ToString().ToUpper(), input.ToLowerInvariant().AsSpan(1))
        };
    
    public static string ToNamespace(this string filePath)
    {
        var folders = (Path.GetDirectoryName(filePath) ?? "").Split(Path.DirectorySeparatorChar);

        var indexOfFirstSeparators = folders.Length > 0 && folders[0].Contains(':') ? 1 : 0;
        
        for (var i = 0; i < folders.Length; i++)
        {
            if (!folders[i].Contains('.')) continue;
            
            indexOfFirstSeparators = i;
            break;
        }

        return string.Join('.', folders.Skip(indexOfFirstSeparators));
    }

    private static readonly Regex EnumNameParser = new(
        "^(?<numericPrefix>[0-9]+)?(?<word>[^_]*)(_(?<word>[^_]*))*$",
        RegexOptions.Compiled);

    public static string ToEnumEntry(this string originalName)
    {
        var match = EnumNameParser.Match(originalName);
        if (!match.Success) throw new ArgumentException("Unable to parse value as enum name", nameof(originalName));

        var sb = new StringBuilder();

        var numericPrefix = match.Groups["numericPrefix"].Value.AsSpan();
        if (numericPrefix.Length > 0)
        {
            sb.Append(long.Parse(numericPrefix).ToWords());
        }

        sb.AppendJoin(
            string.Empty,
            match.Groups["word"]
                 .Captures
                 .Select(capture => capture.Value)
                 .Where(s => !string.IsNullOrEmpty(s))
                 .Select(s => s.FirstCharToUpper()));

        return sb.ToString();
    }
}
