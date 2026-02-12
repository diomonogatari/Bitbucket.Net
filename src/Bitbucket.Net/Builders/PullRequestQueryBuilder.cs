using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Builders;

/// <summary>
/// Fluent builder for querying pull requests in a repository.
/// </summary>
public sealed class PullRequestQueryBuilder
{
    private readonly IBitbucketClient _client;
    private readonly string _projectKey;
    private readonly string _repositorySlug;

    private int? _maxPages;
    private int? _limit;
    private int? _start;
    private PullRequestDirections _direction = PullRequestDirections.Incoming;
    private string? _branchId;
    private PullRequestStates _state = PullRequestStates.Open;
    private PullRequestOrders _order = PullRequestOrders.Newest;
    private bool _withAttributes = true;
    private bool _withProperties = true;

    internal PullRequestQueryBuilder(IBitbucketClient client, string projectKey, string repositorySlug)
    {
        _client = client;
        _projectKey = projectKey;
        _repositorySlug = repositorySlug;
    }

    /// <summary>Filters pull requests by state.</summary>
    public PullRequestQueryBuilder InState(PullRequestStates state) { _state = state; return this; }

    /// <summary>Sets the sort order.</summary>
    public PullRequestQueryBuilder OrderBy(PullRequestOrders order) { _order = order; return this; }

    /// <summary>Sets the direction filter (incoming/outgoing).</summary>
    public PullRequestQueryBuilder WithDirection(PullRequestDirections direction) { _direction = direction; return this; }

    /// <summary>Filters by branch ref (e.g. "refs/heads/feature").</summary>
    public PullRequestQueryBuilder AtBranch(string branchId) { _branchId = branchId; return this; }

    /// <summary>Sets the page size (items per API call).</summary>
    public PullRequestQueryBuilder PageSize(int limit) { _limit = limit; return this; }

    /// <summary>Sets the maximum number of pages to fetch.</summary>
    public PullRequestQueryBuilder MaxPages(int pages) { _maxPages = pages; return this; }

    /// <summary>Sets the start index for pagination.</summary>
    public PullRequestQueryBuilder StartAt(int start) { _start = start; return this; }

    /// <summary>Includes or excludes pull request attributes.</summary>
    public PullRequestQueryBuilder IncludeAttributes(bool include = true) { _withAttributes = include; return this; }

    /// <summary>Includes or excludes pull request properties.</summary>
    public PullRequestQueryBuilder IncludeProperties(bool include = true) { _withProperties = include; return this; }

    /// <summary>Executes the query and returns all matching pull requests.</summary>
    public Task<IReadOnlyList<PullRequest>> GetAsync(CancellationToken cancellationToken = default)
        => _client.GetPullRequestsAsync(_projectKey, _repositorySlug,
            _maxPages, _limit, _start, _direction, _branchId, _state, _order,
            _withAttributes, _withProperties, cancellationToken);

    /// <summary>Executes the query and streams matching pull requests one at a time.</summary>
    public IAsyncEnumerable<PullRequest> StreamAsync(CancellationToken cancellationToken = default)
        => _client.GetPullRequestsStreamAsync(_projectKey, _repositorySlug,
            _maxPages, _limit, _start, _direction, _branchId, _state, _order,
            _withAttributes, _withProperties, cancellationToken);
}