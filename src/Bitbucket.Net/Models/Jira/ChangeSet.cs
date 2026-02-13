using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Models.Jira;

public class ChangeSet
{
    public CommitParent? FromCommit { get; init; }
    public Commit? ToCommit { get; init; }
    public Changes? Changes { get; init; }
    public Links? Links { get; init; }
    public Repository? Repository { get; init; }
}