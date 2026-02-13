namespace Bitbucket.Net;

/// <summary>
/// Abstraction over the Bitbucket Server REST API client, enabling
/// dependency injection, unit testing with mocks, and decorator patterns.
/// </summary>
public interface IBitbucketClient :
    IProjectOperations,
    IRepositoryOperations,
    IPullRequestOperations,
    ICommitOperations,
    IBranchOperations,
    IAdminOperations,
    ISshOperations,
    IRefRestrictionOperations,
    IBuildOperations,
    IGitOperations,
    ISearchOperations,
    IBitbucketMiscOperations,
    IDisposable
{
}