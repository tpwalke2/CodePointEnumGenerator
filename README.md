# CodePointEnumGenerator

A C# source generator that generates enums for all font codepoint files are contained in the project as Additional Files.

Each resulting enum can be used for strongly-typed references to the byte value of specific glyphs in the associated font.

## Links
[![Build status](https://ci.appveyor.com/api/projects/status/2lgs7mbehdvls38q?svg=true)](https://ci.appveyor.com/project/tpwalke2/codepointenumgenerator)

[![NuGet](https://img.shields.io/nuget/v/CodePointEnumGenerator.svg)](https://www.nuget.org/packages/CodePointEnumGenerator/) 

## To Use

1. Reference the Generator package as an analyzer:
```xml
<ItemGroup>
    <PackageReference Include="CodePointEnumGenerator"
                      Version="1.0.0.39"
                      OutputItemType="Analyzer"
                      ReferenceOutputAssembly="false" />
</ItemGroup>
```
1. Include codepoints files for fonts with a build action of `AddtionalFiles`.
2. Rebuild the solution.

## Troubleshooting
The `Source Generators` folder should appear under `Dependencies\.NET 6.0` for the project that references the generator. There should also
be a separate generated C# file that contains the enum for each `codepoints` file in the project. If either or both of these are not the case:
- Verify that the generator project is referenced as an `Analyzer`.
- Verify that the `codepoints` files have a build action of `AdditionalFiles`.
- Clean the solution and then rebuild.
- Close the solution, delete the bin and obj folders for the project, then reopen the solution and rebuild.