#addin nuget:?package=Cake.Git&version=3.0.0

var target = Argument("target", "Default");
var buildNumber = Argument("buildNumber", -1);
var currentRelease = Argument("currentRelease", "0.1.0");
var branch = Argument("branch", "");
var buildPath = Argument("buildPath", "");
var verbose = Argument("verbose", false);
var maxDegreeOfParallelism = Argument("maxDegreeOfParallelism", 5);

if (buildNumber == -1) throw new Exception("Build Number was not provided.");
if (string.IsNullOrEmpty(branch)) throw new Exception("Branch was not provided.");
if (string.IsNullOrEmpty(buildPath)) throw new Exception("Build Path was not provided.");

var currentDate = System.DateTime.UtcNow;
var buildVersion = $"{currentRelease}-{currentDate.Year}{currentDate.Month:D2}{buildNumber}";
Information($"Current Version: {buildVersion}");

var buildOutputDir = MakeAbsolute(Directory("build"));
var deployDir = buildOutputDir.Combine(Directory("deploy"));
var testResultsDir = deployDir.Combine(buildVersion).Combine("test-results");
Information($"Build Output Path: {buildOutputDir}");

var configurations = new []{"Debug", "Release"};
var ignoredPublishExtensions = new HashSet<string> {".pdb"};

var solutionPath = "./CodePointEnumGenerator.sln";

var frameworkVersion = "net6.0";

var testProjects = GetFiles("./**/*Tests.csproj")
                    .Select(testProject => new TestProject {
                        FullPath = testProject.FullPath,
                        FrameworkVersion = frameworkVersion
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
        ParallelInvoke(configurations, configuration => DoDotNetClean(configuration, solutionPath));
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
        DoBuild(buildVersion, solutionPath);
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
        ParallelInvoke(testProjects, testProject => DoTest(testProject, testResultsDir));
    });

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
    {
        DoPack(buildVersion, solutionPath);
    });

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);

private void DoBuild(string version, string path) {
    var settings = new DotNetBuildSettings
    {
        Configuration = "Debug",
        NoIncremental = true,
        NoLogo = true,
        NoRestore = true,
        MSBuildSettings = new DotNetMSBuildSettings()
            .SetVersion(version)
            .WithProperty("FileVersion", version)
            .WithProperty("InformationalVersion", version)
            .SetMaxCpuCount(-1)
    };

    DotNetBuild(path, settings);
}

private void DoDotNetClean(string configuration, string path) {
    var settings = new DotNetCleanSettings
    {
        Configuration = configuration
    };

    DotNetClean(path, settings);
}

private void DoTest(TestProject testProject, string outputPath) {
    var settings = new DotNetTestSettings
    {
        Configuration = "Debug",
        Framework = testProject.FrameworkVersion,
        Loggers = new []{"trx"},
        NoBuild = true,
        NoLogo = true,
        NoRestore = true,
        ResultsDirectory = outputPath
    };

    DotNetTest(testProject.FullPath, settings);
}

private void DoPack(string version, string path) {
    var settings = new DotNetPackSettings {
        Configuration = "Release",
        IncludeSource = false,
        IncludeSymbols = false,
        NoLogo = true,
        OutputDirectory = "./artifacts/",
        MSBuildSettings = new DotNetMSBuildSettings()
            .SetVersion(version)
            .WithProperty("FileVersion", version)
            .WithProperty("InformationalVersion", version)
            .SetMaxCpuCount(-1)
    };

    DotNetPack(path, settings);
}

private void DoTag(
    string branch,
    string buildDirectory,
    string tagName) {
    if (!IsRelease(branch)) return;

    Information($"Tagging '{branch}' with '{buildVersion}'");

    GitTag(buildDirectory, tagName);
}

private bool IsRelease(string branch) {
    return branch.Equals("main", StringComparison.OrdinalIgnoreCase);
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
    public string FrameworkVersion { get; set; }
}