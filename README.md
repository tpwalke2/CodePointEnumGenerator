# CodePointEnumGenerator

Source generator that creates enums for any font codepoint files that are contained in the project as Additional Files.

The resulting enum can be used to generate strongly-typed references to the byte value of specific glyphs in the associate font.

## To Use

1. Reference the Generator project as an analyzer:
```xml
<ItemGroup>
    <ProjectReference Include="..\CodePointEnumGenerator\CodePointEnumGenerator.csproj"
                      OutputItemType="Analyzer"
                      ReferenceOutputAssembly="false" />
</ItemGroup>
```
2. Include codepoints files for fonts with a build action of `AddtionalFiles`.
3. Rebuild the solution.

## Troubleshooting
The `Source Generators` folder should appear under `Dependencies\.NET 6.0` for the project that references the generator. There should also
be a separate generated C# file that contains the enum for each `codepoints` file in the project. If either or both of these are not the case:
- Verify that the generator project is referenced as an `Analyzer`.
- Verify that the `codepoints` files have a build action of `AdditionalFiles`.
- Clean the solution and then rebuild.
- Close the solution, delete the bin and obj folders for the project, then reopen the solution and rebuild.