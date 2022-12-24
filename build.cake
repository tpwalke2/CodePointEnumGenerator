#addin nuget:?package=Cake.Incubator&version=7.0.0

var target = Argument("target", "Default");
var buildNumber = Argument("buildNumber", -1);
var verbose = Argument("verbose", false);
var maxBackupBuilds = 4;
var maxDegreeOfParallelism = Argument("maxDegreeOfParallelism", 5);

if (buildNumber == -1) throw new Exception("Build Number was not provided.");

var currentDate = System.DateTime.UtcNow;
var currentVersion = $"{currentDate.Year}.{currentDate.Month:D2}.{buildNumber}";
Information($"Current Version: {currentVersion}");

var buildOutputDir = MakeAbsolute(Directory("build"));
var deployDir = buildOutputDir.Combine(Directory("deploy"));
var testResultsDir = deployDir.Combine(currentVersion).Combine("test-results");
Information($"Build Output Path: {buildOutputDir}");

var configurations = new []{"Debug", "Release"};
var ignoredPublishExtensions = new HashSet<string> {".pdb"};

var solutionPath = "./CodePointEnumGenerator.sln";

var testProjects = GetFiles("./**/*Tests.csproj")
                    .Select(testProject => (testProject, ParseProject(testProject.FullPath, configuration: "Debug")))
                    .Select(x => new TestProject {
                        FullPath = x.testProject.FullPath,
                        AssemblyName = x.Item2.AssemblyName,
                        FrameworkVersion = x.Item2.TargetFrameworkVersions.First()
                    })
                    .ToList();

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(buildOutputDir);

        DeleteFiles("./**/*.trx");
    });

Task("DotNetClean")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        ParallelInvoke(configurations, DoDotNetClean);
    });

Task("Restore")
    .IsDependentOn("DotNetClean")
    .Does(() =>
    {
        DotNetRestore();
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DoBuild();
    });

Task("PrepareTest")
    .IsDependentOn("Build")
    .Does(() =>
    {
        CreateDirectory(testResultsDir);
    });

Task("Test")
    .IsDependentOn("PrepareTest")
    .Does(() =>
    {
        ParallelInvoke(testProjects, DoTest);
    });

Task("Publish")
    .IsDependentOn("Test")
    .Does(() =>
    {
        DoMSBuildPublish();
    });

Task("Default")
    .IsDependentOn("Publish");

RunTarget(target);

private void DoBuild() {
    var settings = new DotNetBuildSettings
    {
        Configuration = "Debug",
        NoIncremental = true,
        NoLogo = true,
        NoRestore = true,
        MSBuildSettings = new DotNetMSBuildSettings()
            .SetVersion(currentVersion)
            .WithProperty("FileVersion", currentVersion)
            .WithProperty("InformationalVersion", currentVersion)
            .SetMaxCpuCount(-1)
    };

    DotNetBuild(solutionPath, settings);
}

private void DoDotNetClean(string configuration) {
    var settings = new DotNetCleanSettings
    {
        Configuration = configuration
    };

    DotNetClean(solutionPath, settings);
}

private void DoTest(TestProject testProject) {
    var settings = new DotNetTestSettings
    {
        Configuration = "Debug",
        Framework = testProject.FrameworkVersion,
        Loggers = new []{"trx"},
        NoBuild = true,
        NoLogo = true,
        NoRestore = true,
        ResultsDirectory = testResultsDir
    };

    DotNetTest(testProject.FullPath, settings);
}

private void DoMSBuildPublish() {
    var settings = new DotNetMSBuildSettings()
            .SetVersion(currentVersion)
            .WithProperty("FileVersion", currentVersion)
            .WithProperty("InformationalVersion", currentVersion)
            .SetMaxCpuCount(-1)
            .SetConfiguration("Release");
    DotNetMSBuild($"./CodePointEnumGenerator}/CodePointEnumGenerator.csproj", settings);
}

private void ParallelInvoke<TSource>(IEnumerable<TSource> source, Action<TSource> invokeAction) {
    var actions = new List<Action>();

    foreach (var item in source)
    {
        actions.Add(() => invokeAction(item));
    }

    var options = new ParallelOptions {
        MaxDegreeOfParallelism = maxDegreeOfParallelism
    };

    Parallel.Invoke(options, actions.ToArray());
}

class TestProject
{
    public string FullPath { get; set; }
    public string AssemblyName { get; set; }
    public string FrameworkVersion { get; set; }
}