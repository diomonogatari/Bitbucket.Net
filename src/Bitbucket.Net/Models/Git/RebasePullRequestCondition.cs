namespace Bitbucket.Net.Models.Git;

public class RebasePullRequestCondition
{
    public bool CanRebase { get; init; }
    public bool CanWrite { get; init; }
    public List<Veto>? Vetoes { get; init; }
}