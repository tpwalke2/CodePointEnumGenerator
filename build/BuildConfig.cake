public class BuildConfig
{
    public string Target { get; set; }

    public int BuildNumber { get; set; }

    public string CurrentRelease { get; set; }

    public string Branch { get; set; }

    public string BuildPath { get; set; }

    public string NuGetApiKey { get; set; }

    public string GitHubApiKey { get; set; }

    public bool Verbose { get; set; }

    public int MaxDegreeOfParallelism { get; set; }

    public string CurrentVersion { get; set; }
}
