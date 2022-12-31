using Cake.Common.IO;

public class BuildPaths {
    private string _rootPath;
    private DirectoryPath _buildOutputPath;
    private DirectoryPath _deployPath;
    private DirectoryPath _testResultsPath;
    private DirectoryPath _artifactsPath;

    public BuildPaths(ICakeContext context, string rootPath) {
        _rootPath = rootPath;

        _buildOutputPath = DirectoryAliases.MakeAbsolute(context, DirectoryAliases.Directory(context, rootPath)).Combine(DirectoryAliases.Directory(context, "output"));
        _deployPath = _buildOutputPath.Combine(DirectoryAliases.Directory(context, "deploy"));
        _testResultsPath = _deployPath.Combine(DirectoryAliases.Directory(context, "test-results"));
        _artifactsPath = _buildOutputPath.Combine(DirectoryAliases.Directory(context, "artifacts"));
    }

    public string Root => _rootPath;
    public string BuildOutput => _buildOutputPath.FullPath;
    public string Deploy => _deployPath.FullPath;
    public string TestResults => _testResultsPath.FullPath;
    public string Artifacts => _artifactsPath.FullPath;

    public override string ToString() {
        return $"Root:\t\t{_rootPath}\nBuild Output:\t{BuildOutput}\nDeploy:\t\t{Deploy}\nTest Results:\t{TestResults}\nArtifacts:\t{Artifacts}";
    }
}