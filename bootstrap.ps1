param (
    [int]$buildNumber,
    [System.String]$branch,
    [System.String]$buildPath,
    [System.String]$gitHubApiKey,
    [System.String]$nuGetApiKey)

dotnet tool restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet cake build.cake --bootstrap --buildNumber=$buildNumber --branch="$branch" --buildPath="$buildPath" --gitHubApiKey="$gitHubApiKey" --nuGetApiKey="$nuGetApiKey"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet cake build.cake --buildNumber=$buildNumber --branch="$branch" --buildPath="$buildPath" --gitHubApiKey="$gitHubApiKey" --nuGetApiKey="$nuGetApiKey"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }