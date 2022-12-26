param (
    [Parameter(Mandatory=$true)]
    [int]$buildNumber,
    [Parameter(Mandatory=$true)]
    [string]$branch,
    [Parameter(Mandatory=$true)]
    [string]$buildPath)

dotnet tool restore
dotnet cake build.cake --bootstrap --buildNumber=$buildNumber -branch=$branch -buildPath="$buildPath"
dotnet cake build.cake --buildNumber=$buildNumber -branch=$branch -buildPath="$buildPath"