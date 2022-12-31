#load "./TestProject.cake";

private void DoTest(
    TestProject testProject,
    string outputPath,
    bool verbose) {
    var settings = new DotNetTestSettings
    {
        Configuration = "Debug",
        Framework = testProject.FrameworkVersion,
        Loggers = new []{"trx"},
        NoBuild = true,
        NoLogo = true,
        NoRestore = true,
        ResultsDirectory = outputPath,
        DiagnosticOutput = verbose
    };

    DotNetTest(testProject.FullPath, settings);
}