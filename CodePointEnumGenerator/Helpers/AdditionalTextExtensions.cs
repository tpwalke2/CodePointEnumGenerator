﻿using Microsoft.CodeAnalysis;
using System;
using System.IO;

namespace CodePointEnumGenerator.Helpers;

public static class AdditionalTextExtensions
{
    public static string GetEnumFileName(this AdditionalText file) => Path.GetFileName(file.Path)
                                                                          .Replace(".codepoints",
                                                                              "",
                                                                              StringComparison.OrdinalIgnoreCase)
                                                                          .Replace("-", "");
}
