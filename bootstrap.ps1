param ($buildNumber)

dotnet tool restore
dotnet cake build.cake --bootstrap --buildNumber=$buildNumber
dotnet cake build.cake --buildNumber=$buildNumber