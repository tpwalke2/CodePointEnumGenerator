// Addins
#addin nuget:?package=Cake.Http&version=2.0.0

// Usings
using System.Text.Json;
using System.Linq;

// Local dependencies
#load "./BuildConfig.cake";
#load "./BuildPaths.cake";
#load "./Utilities.cake";
#load "./CreateGitHubRelease.cake";
#load "./UploadGitHubReleaseAssetResponse.cake";

private async Task DoPublish(
    BuildConfig config,
    BuildPaths paths
) {
    if (!IsRelease(config.Branch)) return;
    FlushDns();

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

private async Task<ReleaseDownload> DoGitHubRelease(
    BuildConfig config,
    FilePath packageToUpload) {
    Information($"Creating release on GitHub for branch '{config.Branch}' and version '{config.CurrentVersion}'");

    var tagName = $"Release-{config.CurrentVersion}";

    var createReleaseResponse = await CreateGitHubRelease(
        config.GitHubApiKey,
        tagName,
        config.Branch,
        config.Verbose
    );

    Information($"Upload URL: {createReleaseResponse.upload_url}");

    var uploadResponse = await UploadAsset(
        config.GitHubApiKey,
        createReleaseResponse.upload_url.Replace("{?name,label}", "", StringComparison.OrdinalIgnoreCase),
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

private async Task<CreateGitHubReleaseResponse> CreateGitHubRelease(
    string apiKey,
    string tagName,
    string branch,
    bool verbose
) {
    // https://docs.github.com/en/rest/releases/releases?apiVersion=2022-11-28#create-a-release
    if (string.IsNullOrEmpty(apiKey)) throw new Exception("GitHub API key was not provided.");

    var createReleaseBody = new CreateGitHubReleaseRequest {
        tag_name = tagName,
        target_commitish = branch,
        name = tagName,
        draft = true,
        generate_release_notes = true
    };
    var createReleaseUrl = $"https://api.github.com/repos/{RepositoryOwner}/{RepositoryId}/releases";

    return JsonSerializer.Deserialize<CreateGitHubReleaseResponse>(await HttpPostAsync(
        createReleaseUrl,
        new HttpSettings { LogRequestResponseOutput = verbose }
            .SetAccept("application/vnd.github+json")
            .SetAuthorization("Bearer", apiKey)
            .SetContentType("application/json")
            .SetJsonRequestBody(createReleaseBody, false)
            .EnsureSuccessStatusCode(true)));
}

private void DoNuGetPublish(
    BuildConfig config,
    FilePath packageFile,
    string releaseUrl
    ) {
    if (string.IsNullOrEmpty(config.NuGetApiKey)) throw new Exception("NuGet API key was not provided.");
    if (!IsRelease(config.Branch)) return;

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

class ReleaseDownload {
    public string ReleaseUrl { get; set; }
    public string DownloadUrl { get; set; }
}