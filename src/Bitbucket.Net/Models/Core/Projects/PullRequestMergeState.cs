namespace Bitbucket.Net.Models.Core.Projects;

public class PullRequestMergeState
{
    public bool CanMerge { get; init; }
    public bool Conflicted { get; init; }
    public string? Outcome { get; init; }
    public List<Veto>? Vetoes { get; init; }
}