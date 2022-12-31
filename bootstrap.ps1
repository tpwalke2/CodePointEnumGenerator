param (
    [int]$buildNumber,
    [System.String]$branch,
    [System.String]$buildPath,
    [System.String]$gitHubApiKey,
    [System.String]$nuGetApiKey)

dotnet tool restore
dotnet cake build.cake --bootstrap --buildNumber=$buildNumber --branch="$branch" --buildPath="$buildPath" --gitHubApiKey="$gitHubApiKey" --nuGetApiKey="$nuGetApiKey"
dotnet cake build.cake --buildNumber=$buildNumber --branch="$branch" --buildPath="$buildPath" --gitHubApiKey="$gitHubApiKey" --nuGetApiKey="$nuGetApiKey"