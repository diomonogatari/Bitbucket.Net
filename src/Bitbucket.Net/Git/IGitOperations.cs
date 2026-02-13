using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Git;

namespace Bitbucket.Net;

/// <summary>
/// Git-specific operations.
/// </summary>
public interface IGitOperations
{
    Task<RebasePullRequestCondition> GetCanRebasePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default);
    Task<PullRequest> RebasePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, int version, CancellationToken cancellationToken = default);
    Task<Tag> CreateTagAsync(string projectKey, string repositorySlug, TagTypes tagType, string tagName, string startPoint, CancellationToken cancellationToken = default);
    Task<bool> DeleteTagAsync(string projectKey, string repositorySlug, string tagName, CancellationToken cancellationToken = default);
}