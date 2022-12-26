param (
    [int]$buildNumber,
    [string]$branch,
    [string]$buildPath)

dotnet tool restore
dotnet cake build.cake --bootstrap --buildNumber=$buildNumber -branch=$branch -buildPath="$buildPath"
dotnet cake build.cake --buildNumber=$buildNumber -branch=$branch -buildPath="$buildPath"