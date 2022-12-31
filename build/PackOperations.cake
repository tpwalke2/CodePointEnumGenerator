private void DoPack(
    string path,
    string outputPath,
    DotNetMSBuildSettings dotNetMSBuildSettings,
    bool verbose) {
    var settings = new DotNetPackSettings {
        Configuration = "Release",
        IncludeSource = false,
        IncludeSymbols = false,
        NoLogo = true,
        OutputDirectory = outputPath,
        MSBuildSettings = dotNetMSBuildSettings,
        DiagnosticOutput = verbose
    };

    DotNetPack(path, settings);
}