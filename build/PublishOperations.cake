// Addins
#addin nuget:?package=Cake.Http&version=2.0.0

// Usings
using System.Text.Json;
using System.Linq;

// Local dependencies
#load "./BuildConfig.cake";
#load "./BuildPaths.cake";
#load "./Utilities.cake";
#load "./GitHubApiModels.cake";

// Constants
const string GitHubApiBaseUrl = "https://api.github.com/repos";
const string RepositoryOwner = "tpwalke2";
const string RepositoryId = "CodePointEnumGenerator";

private async Task DoPublish(
    BuildConfig config,
    BuildPaths paths,
    DotNetMSBuildSettings dotNetMSBuildSettings,
    string solutionPath
) {
    if (!IsRelease(config.Branch, config.TagName)) return;
    
    FlushDns();
    DoPack(solutionPath, paths.Artifacts, dotNetMSBuildSettings, config.Verbose);

    var packageFile = GetFiles($"{paths.Artifacts}/**/CodePointEnumGenerator.{config.CurrentVersion}.nupkg").First();

    var releaseDownload = await DoGitHubRelease(
        config,
        packageFile
        );
    DoNuGetPublish(
        config,
        packageFile,
        releaseDownload.ReleaseUrl);
}

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

private async Task<ReleaseDownload> DoGitHubRelease(
    BuildConfig config,
    FilePath packageToUpload) {
    Information($"Creating release on GitHub for branch '{config.Branch}' and version '{config.CurrentVersion}'");

    // get GitHub release
    var getRelease = await GetGitHubRelease(
        config.GitHubApiKey,
        config.TagName,
        config.Verbose
    );

    Information($"Upload URL: {getRelease.upload_url}");

    var uploadResponse = await UploadAsset(
        config.GitHubApiKey,
        getRelease.upload_url.Replace("{?name,label}", "", StringComparison.OrdinalIgnoreCase),
        packageToUpload,
        config.Verbose);
    
    return new ReleaseDownload {
        ReleaseUrl = uploadResponse.url,
        DownloadUrl = uploadResponse.browser_download_url
    };
}

private async Task<UploadGitHubReleaseAssetResponse> UploadAsset(
    string apiKey,
    string uploadBaseUrl,
    FilePath fileToUpload,
    bool verbose
) {
    // https://docs.github.com/en/rest/releases/assets?apiVersion=2022-11-28#upload-a-release-asset
    if (string.IsNullOrEmpty(apiKey)) throw new Exception("GitHub API key was not provided.");

    var uploadAssetSettings = new HttpSettings
        {
            LogRequestResponseOutput = verbose,
            RequestBody = await System.IO.File.ReadAllBytesAsync(fileToUpload.FullPath)
        }
        .SetAccept("application/vnd.github+json")
        .SetAuthorization("Bearer", apiKey)
        .SetContentType("application/zip")
        .EnsureSuccessStatusCode(true);

    var uploadUrl = $"{uploadBaseUrl}?name={fileToUpload.Segments.Last()}";
    Information($"Uploading to URL: {uploadUrl}");

    return JsonSerializer.Deserialize<UploadGitHubReleaseAssetResponse>(await HttpPostAsync(
        uploadUrl,
        uploadAssetSettings));
}

private async Task<GetReleaseByTagResponse> GetGitHubRelease(
    string apiKey,
    string tagName,
    bool verbose
) {
    // https://docs.github.com/en/rest/releases/releases?apiVersion=2022-11-28#get-a-release-by-tag-name
    if (string.IsNullOrEmpty(apiKey)) throw new Exception("GitHub API key was not provided.");

    var getReleaseUrl = $"{GitHubApiBaseUrl}/{RepositoryOwner}/{RepositoryId}/releases/tags/{tagName}";

    return JsonSerializer.Deserialize<GetReleaseByTagResponse>(await HttpGetAsync(
        getReleaseUrl,
        new HttpSettings { LogRequestResponseOutput = verbose }
            .SetAccept("application/vnd.github+json")
            .SetAuthorization("Bearer", apiKey)
            .SetContentType("application/json")
            .EnsureSuccessStatusCode(true)));
}

private void DoNuGetPublish(
    BuildConfig config,
    FilePath packageFile,
    string releaseUrl
    ) {
    if (string.IsNullOrEmpty(config.NuGetApiKey)) throw new Exception("NuGet API key was not provided.");

    // use urls from release and artifact to push into NuGet

    var settings = new DotNetNuGetPushSettings {
        ApiKey = config.NuGetApiKey,
        DiagnosticOutput = config.Verbose,
        IgnoreSymbols = true,
        Interactive = false,
        Source = "https://api.nuget.org/v3/index.json"
    };

    DotNetNuGetPush(packageFile, settings);
}

private bool IsRelease(string branch, string tagName) {
    return branch.Equals("main") && tagName.StartsWith("Release-");
}

class ReleaseDownload {
    public string ReleaseUrl { get; set; }
    public string DownloadUrl { get; set; }
}