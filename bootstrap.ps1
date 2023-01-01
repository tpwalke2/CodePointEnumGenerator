param (
    [int]$buildNumber,
    [System.String]$branch,
    [System.String]$buildPath,
    [System.String]$gitHubApiKey,
    [System.String]$nuGetApiKey,
    [bool]$isTag,
    [System.String]$tagVariableName)

$tagName = ""
if ($isTag) { $tagName = [System.Environment]::GetEnvironmentVariable($tagVariableName) }

dotnet tool restore
dotnet cake build.cake --bootstrap --buildNumber=$buildNumber --branch="$branch" --buildPath="$buildPath" --gitHubApiKey="$gitHubApiKey" --nuGetApiKey="$nuGetApiKey" --tagName="$tagName"
dotnet cake build.cake --buildNumber=$buildNumber --branch="$branch" --buildPath="$buildPath" --gitHubApiKey="$gitHubApiKey" --nuGetApiKey="$nuGetApiKey" --tagName="$tagName"