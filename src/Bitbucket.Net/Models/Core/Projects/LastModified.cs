namespace Bitbucket.Net.Models.Core.Projects;

public class LastModified
{
    public Dictionary<string, Commit>? Files { get; set; }
    public Commit? LatestCommit { get; set; }
}