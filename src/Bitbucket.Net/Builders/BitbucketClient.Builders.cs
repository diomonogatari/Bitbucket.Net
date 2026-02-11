using Bitbucket.Net.Builders;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
    /// <summary>
    /// Returns a fluent builder for querying pull requests in a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    public PullRequestQueryBuilder PullRequests(string projectKey, string repositorySlug)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);
        return new PullRequestQueryBuilder(this, projectKey, repositorySlug);
    }

    /// <summary>
    /// Returns a fluent builder for querying commits in a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="until">The commit or ref to walk back from (required).</param>
    public CommitQueryBuilder Commits(string projectKey, string repositorySlug, string until)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(until);
        return new CommitQueryBuilder(this, projectKey, repositorySlug, until);
    }

    /// <summary>
    /// Returns a fluent builder for querying branches in a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    public BranchQueryBuilder Branches(string projectKey, string repositorySlug)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);
        return new BranchQueryBuilder(this, projectKey, repositorySlug);
    }

    /// <summary>
    /// Returns a fluent builder for querying projects.
    /// </summary>
    public ProjectQueryBuilder Projects()
    {
        return new ProjectQueryBuilder(this);
    }
}