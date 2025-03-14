﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

/* 
 * Adapted from BlazorSourceGeneratorTests
 * https://github.com/b-straub/BlazorSourceGeneratorTests/blob/master/Tests/GeneratorTestFactory.cs
 *
 * MIT License
 * Copyright (c) 2020 b-straub
 */

namespace CodePointEnumGenerator.Tests;

public static class GeneratorTestFactory
{
    private static readonly HashSet<string> IgnoredPreDiagnosticErrors = ["CS0012", "CS0616", "CS0246", "CS0103"];

    public static (Compilation? Compilation,
        (ImmutableArray<Diagnostic> Before, ImmutableArray<Diagnostic> After) Diagnostics,
        GeneratorDriverRunResult? RunResult) RunGenerator(string source, params (string, string)[] additionalFiles)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(source, Encoding.UTF8));

        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                                 .WithOptimizationLevel(OptimizationLevel.Debug)
                                 .WithGeneralDiagnosticOption(ReportDiagnostic.Default);

        var references = new MetadataReference[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(CodePointEnumGenerator).Assembly.Location)
        };

        var compilation = CSharpCompilation.Create(
            "TestGenerator",
            [syntaxTree],
            references,
            compilationOptions);
        var diagnostics = compilation.GetDiagnostics();
        if (!VerifyDiagnostics(diagnostics))
        {
            // this will make the test fail, check the input source code!
            return (null, (diagnostics, default), null);
        }

        var generator    = new CodePointEnumGenerator();
        var parseOptions = syntaxTree.Options as CSharpParseOptions;

        var driver = CSharpGeneratorDriver.Create(
                                              ImmutableArray.Create(generator.AsSourceGenerator()),
                                              additionalFiles
                                                  .Select(tuple => new AdditionalFile(tuple.Item1, tuple.Item2))
                                                  .ToImmutableArray(),
                                              parseOptions)
                                          .RunGeneratorsAndUpdateCompilation(compilation,
                                                                             out var outputCompilation,
                                                                             out var generatorDiagnostics);

        return (outputCompilation, (diagnostics, generatorDiagnostics), driver.GetRunResult());
    }

    private static bool VerifyDiagnostics(ImmutableArray<Diagnostic> actual)
    {
        return actual.Where(d => d.Severity == DiagnosticSeverity.Error)
                     .Select(d => d.Id.ToString())
                     .All(IgnoredPreDiagnosticErrors.Contains);
    }
}