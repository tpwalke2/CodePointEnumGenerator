#load "./BuildConfig.cake";

private void FlushDns() {
    var process = new System.Diagnostics.Process();
    var startInfo = new System.Diagnostics.ProcessStartInfo();
    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
    startInfo.FileName = "ipconfig.exe";
    startInfo.Arguments = "/flushdns";
    process.StartInfo = startInfo;
    process.Start();
}

private string GetVersion(BuildConfig buildConfig) {
    var currentDate = System.DateTime.UtcNow;
    return $"{buildConfig.CurrentRelease}.{buildConfig.BuildNumber}";
}

private void ParallelInvoke<TSource>(
    IEnumerable<TSource> source,
    Action<TSource> invokeAction,
    int maxDegreeOfParallelism) {
    var actions = new List<Action>();

    foreach (var item in source)
    {
        actions.Add(() => invokeAction(item));
    }

    var options = new ParallelOptions {
        MaxDegreeOfParallelism = maxDegreeOfParallelism
    };

    Parallel.Invoke(options, actions.ToArray());
}