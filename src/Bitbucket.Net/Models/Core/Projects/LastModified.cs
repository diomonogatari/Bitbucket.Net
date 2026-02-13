namespace Bitbucket.Net.Models.Core.Projects;

public class LastModified
{
    public Dictionary<string, Commit>? Files { get; init; }
    public Commit? LatestCommit { get; init; }
}