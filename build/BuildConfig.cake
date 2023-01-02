using Cake.ArgumentBinder;

public class BuildConfig
{
    [StringArgument(
        "target",
        Description = "The target task to execute.",
        DefaultValue = "Default")]
    public string Target { get; set; }

    [IntegerArgument(
        "buildNumber",
        Description = "The current build number.",
        Required = true
    )]
    public int BuildNumber { get; set; }

    [StringArgument(
        "currentRelease",
        Description = "The current release version.",
        DefaultValue = "1.0.0"
    )]
    public string CurrentRelease { get; set; }

    [StringArgument(
        "branch",
        Description = "The current branch being built.",
        Required = true
    )]
    public string Branch { get; set; }

    [StringArgument(
        "buildPath",
        Description = "The base path where the build is being run from.",
        Required = true
    )]
    public string BuildPath { get; set; }

    [StringArgument(
        "nuGetApiKey",
        Description = "The API key for interacting with NuGet.",
        Required = true
    )]
    public string NuGetApiKey { get; set; }

    [StringArgument(
        "gitHubApiKey",
        Description = "The API key for interacting with GitHub.",
        Required = true
    )]
    public string GitHubApiKey { get; set; }

    [BooleanArgument(
        "verbose",
        Description = "Flag to turn on more verbose output.",
        DefaultValue = false
    )]
    public bool Verbose { get; set; }

    [IntegerArgument(
        "maxDegreeOfParallelism",
        Description = "Maximum degree of parallelism when running parallel tasks.",
        DefaultValue = 5
    )]
    public int MaxDegreeOfParallelism { get; set; }

    [StringArgument(
        "currentVersion",
        Description = "Override for current version.",
        DefaultValue = ""
    )]
    public string CurrentVersion { get; set; }
}