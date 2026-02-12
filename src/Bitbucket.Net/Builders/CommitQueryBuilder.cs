using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Builders;

/// <summary>
/// Fluent builder for querying commits in a repository.
/// </summary>
public sealed class CommitQueryBuilder
{
    private readonly IBitbucketClient _client;
    private readonly string _projectKey;
    private readonly string _repositorySlug;
    private readonly string _until;

    private bool _followRenames;
    private bool _ignoreMissing;
    private MergeCommits _merges = MergeCommits.Exclude;
    private string? _path;
    private string? _since;
    private bool _withCounts;
    private int? _maxPages;
    private int? _limit;
    private int? _start;

    internal CommitQueryBuilder(IBitbucketClient client, string projectKey, string repositorySlug, string until)
    {
        _client = client;
        _projectKey = projectKey;
        _repositorySlug = repositorySlug;
        _until = until;
    }

    /// <summary>Follow file renames in history.</summary>
    public CommitQueryBuilder FollowRenames(bool follow = true) { _followRenames = follow; return this; }

    /// <summary>Ignore missing commits instead of failing.</summary>
    public CommitQueryBuilder IgnoreMissing(bool ignore = true) { _ignoreMissing = ignore; return this; }

    /// <summary>Controls inclusion of merge commits.</summary>
    public CommitQueryBuilder Merges(MergeCommits merges) { _merges = merges; return this; }

    /// <summary>Filters commits that touch the given file path.</summary>
    public CommitQueryBuilder AtPath(string path) { _path = path; return this; }

    /// <summary>Returns commits after (exclusive) the given commit or ref.</summary>
    public CommitQueryBuilder Since(string since) { _since = since; return this; }

    /// <summary>Includes commit count metadata.</summary>
    public CommitQueryBuilder WithCounts(bool include = true) { _withCounts = include; return this; }

    /// <summary>Sets the page size (items per API call).</summary>
    public CommitQueryBuilder PageSize(int limit) { _limit = limit; return this; }

    /// <summary>Sets the maximum number of pages to fetch.</summary>
    public CommitQueryBuilder MaxPages(int pages) { _maxPages = pages; return this; }

    /// <summary>Sets the start index for pagination.</summary>
    public CommitQueryBuilder StartAt(int start) { _start = start; return this; }

    /// <summary>Executes the query and returns all matching commits.</summary>
    public Task<IReadOnlyList<Commit>> GetAsync(CancellationToken cancellationToken = default)
        => _client.GetCommitsAsync(_projectKey, _repositorySlug, _until,
            _followRenames, _ignoreMissing, _merges, _path, _since, _withCounts,
            _maxPages, _limit, _start, cancellationToken);

    /// <summary>Executes the query and streams matching commits one at a time.</summary>
    public IAsyncEnumerable<Commit> StreamAsync(CancellationToken cancellationToken = default)
        => _client.GetCommitsStreamAsync(_projectKey, _repositorySlug, _until,
            _followRenames, _ignoreMissing, _merges, _path, _since, _withCounts,
            _maxPages, _limit, _start, cancellationToken);
}