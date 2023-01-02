#load "./BuildConfig.cake";
#load "./Utilities.cake";

var configurations = new []{"Debug", "Release"};

private void DoBuild(
    BuildConfig config,
    string solutionPath,
    DotNetMSBuildSettings dotNetMSBuildSettings) {

    ParallelInvoke(
        configurations,
        configuration => DoDotNetClean(
            configuration,
            solutionPath,
            config.Verbose,
            dotNetMSBuildSettings),
        config.MaxDegreeOfParallelism);

    DotNetRestore(new DotNetRestoreSettings {
        DiagnosticOutput = config.Verbose,
        MSBuildSettings = dotNetMSBuildSettings
    });

    var settings = new DotNetBuildSettings
    {
        Configuration = "Debug",
        NoIncremental = true,
        NoLogo = true,
        NoRestore = true,
        MSBuildSettings = dotNetMSBuildSettings,
        DiagnosticOutput = config.Verbose
    };

    DotNetBuild(solutionPath, settings);
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