private void DoBuild(
    string path,
    bool verbose,
    DotNetMSBuildSettings dotNetMSBuildSettings) {
    var settings = new DotNetBuildSettings
    {
        Configuration = "Debug",
        NoIncremental = true,
        NoLogo = true,
        NoRestore = true,
        MSBuildSettings = dotNetMSBuildSettings,
        DiagnosticOutput = verbose
    };

    DotNetBuild(path, settings);
}

private void DoDotNetClean(
    string configuration,
    string path,
    bool verbose,
    DotNetMSBuildSettings dotNetMSBuildSettings) {
    var settings = new DotNetCleanSettings
    {
        Configuration = configuration,
        DiagnosticOutput = verbose,
        MSBuildSettings = dotNetMSBuildSettings
    };

    DotNetClean(path, settings);
}