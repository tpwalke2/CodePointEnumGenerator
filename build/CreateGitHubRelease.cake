public class CreateGitHubReleaseRequest
{
    public string tag_name { get; set; }
    public string target_commitish { get; set; }
    public string name { get; set; }
    public bool draft { get; set; }
    public bool generate_release_notes { get; set; }
}

public class CreateGitHubReleaseResponse
{
    public string url { get; set; }
    public string html_url { get; set; }
    public string assets_url { get; set; }
    public string upload_url { get; set; }
    public int id { get; set; }
}