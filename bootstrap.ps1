param (
    [int]$buildNumber,
    [System.String]$branch,
    [System.String]$buildPath,
    [System.String]$gitHubApiKey,
    [System.String]$nuGetApiKey,
    [System.String]$isTag,
    [System.String]$tagVariableName)

$tagName = ""
if ($isTag.Equals("true")) { $tagName = [System.Environment]::GetEnvironmentVariable($tagVariableName) }

dotnet tool restore
dotnet cake build.cake --bootstrap --buildNumber=$buildNumber --branch="$branch" --buildPath="$buildPath" --gitHubApiKey="$gitHubApiKey" --nuGetApiKey="$nuGetApiKey" --tagName="$tagName"
dotnet cake build.cake --buildNumber=$buildNumber --branch="$branch" --buildPath="$buildPath" --gitHubApiKey="$gitHubApiKey" --nuGetApiKey="$nuGetApiKey" --tagName="$tagName"