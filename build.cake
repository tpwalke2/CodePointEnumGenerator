// Addins
#addin nuget:?package=Cake.ArgumentBinder&version=3.0.0

// Usings
using System.IO;
using System.Threading.Tasks;

// Other includes
#load "./build/BuildConfig.cake";
#load "./build/BuildPaths.cake";
#load "./build/Utilities.cake";
#load "./build/BuildOperations.cake";
#load "./build/TestOperations.cake";
#load "./build/PublishOperations.cake";

// Arguments
var config = CreateFromArguments<BuildConfig>();

if (string.IsNullOrEmpty(config.CurrentVersion)) config.CurrentVersion = GetVersion(config);
Information($"Current Version: {config.CurrentVersion}");

var paths = new BuildPaths(Context, config.BuildPath);
Information($"Build Output Paths\n{paths}");

const string SolutionPath = "./CodePointEnumGenerator.sln";
const string FrameworkVersion = "net9.0";

var testProjects = GetFiles("./**/*Tests.csproj")
                    .Select(testProject => new TestProject {
                        FullPath = testProject.FullPath,
                        FrameworkVersion = FrameworkVersion
                    })
                    .ToList();

var msBuildSettings = new DotNetMSBuildSettings()
            .SetVersion(config.CurrentVersion)
            .WithProperty("FileVersion", config.CurrentVersion)
            .WithProperty("InformationalVersion", config.CurrentVersion)
            .WithProperty("AssemblyVersion", config.CurrentVersion)
            .SetMaxCpuCount(-1);

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(paths.BuildOutput);

        DeleteFiles("./**/*.trx");
    });

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DoBuild(
            config,
            SolutionPath,
            msBuildSettings);
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        CreateDirectory(paths.TestResults);

        ParallelInvoke(
            testProjects,
            testProject => DoTest(
                testProject,
                paths.TestResults,
                config.Verbose),
            config.MaxDegreeOfParallelism);
    });

Task("Publish")
    .IsDependentOn("Test")
    .Does(async () => {
        await DoPublish(config, paths, msBuildSettings, SolutionPath);
    });

Task("Default")
    .IsDependentOn("Publish")
    .DescriptionFromArguments<BuildConfig>("Runs the build");

RunTarget(config.Target);