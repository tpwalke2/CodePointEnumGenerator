using System.Collections.Generic;

// Based on https://github.com/Humanizr/Humanizer/blob/main/src/Humanizer/Localisation/NumberToWords/EnglishNumberToWordsConverter.cs

namespace CodePointEnumGenerator.Helpers;

internal static class EnglishNumberToWordsConverter
{
    private const long Quintillion = 1000000000000000000;
    private const long Quadrillion = 1000000000000000;
    private const long Trillion = 1000000000000;
    private const int Billion = 1000000000;
    private const int Million = 1000000;
    private const int Thousand = 1000;
    private const int Hundred = 100;
    private const int Twenty = 20;
    private const int Ten = 10;
    
    private const string Negative = "Negative";

    private static readonly string[] UnitsMap =
    {
        "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve",
        "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"
    };

    private static readonly string[] TensMap =
        { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

    private static readonly (long, string)[] MultipliersMap =
    {
        (Quintillion, nameof(Quintillion)),
        (Quadrillion, nameof(Quadrillion)),
        (Trillion, nameof(Trillion)),
        (Billion, nameof(Billion)),
        (Million, nameof(Million)),
        (Thousand, nameof(Thousand)),
        (Hundred, nameof(Hundred))
    };

    public static string ToWords(this long number)
    {
        switch (number)
        {
            case 0:
                return GetUnitValue(0);
            case < 0:
                return $"{Negative}{ToWords(-number)}";
        }

        var parts = new List<string>();

        foreach (var multiplier in MultipliersMap)
        {
            ExtractMultiplier(ref number, multiplier.Item1, multiplier.Item2, parts);
        }

        if (number > 0) parts.Add(GetLastPart(number));

        return string.Join("", parts.ToArray());
    }

    private static string GetLastPart(long number)
    {
        if (number < Twenty) return GetUnitValue(number);

        var lastPart = TensMap[number / Ten];
        if (number % Ten > 0)
        {
            lastPart += $"{GetUnitValue(number % Ten)}";
        }

        return lastPart;
    }

    private static void ExtractMultiplier(
        ref long number,
        long multiplier,
        string multiplierName,
        ICollection<string> parts)
    {
        if (number / multiplier <= 0) return;

        parts.Add($"{ToWords(number / multiplier)}{multiplierName}");
        number %= multiplier;
    }

    private static string GetUnitValue(long number) => UnitsMap[number];
}