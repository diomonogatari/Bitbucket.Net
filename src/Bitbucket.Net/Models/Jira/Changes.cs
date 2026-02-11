using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Models.Jira;

public class Changes
{
    public int Size { get; init; }
    public int Limit { get; init; }
    public bool IsLastPage { get; init; }
    public List<Change>? Values { get; init; }
    public int Start { get; init; }
}