using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Builders;

/// <summary>
/// Fluent builder for querying branches in a repository.
/// </summary>
public sealed class BranchQueryBuilder
{
    private readonly IBitbucketClient _client;
    private readonly string _projectKey;
    private readonly string _repositorySlug;

    private int? _maxPages;
    private int? _limit;
    private int? _start;
    private string? _baseBranchOrTag;
    private bool? _details;
    private string? _filterText;
    private BranchOrderBy? _orderBy;

    internal BranchQueryBuilder(IBitbucketClient client, string projectKey, string repositorySlug)
    {
        _client = client;
        _projectKey = projectKey;
        _repositorySlug = repositorySlug;
    }

    /// <summary>Sets the base branch or tag for relative listing.</summary>
    public BranchQueryBuilder Base(string baseBranchOrTag) { _baseBranchOrTag = baseBranchOrTag; return this; }

    /// <summary>Includes branch details (e.g. ahead/behind counts).</summary>
    public BranchQueryBuilder WithDetails(bool details = true) { _details = details; return this; }

    /// <summary>Filters branches by display name prefix.</summary>
    public BranchQueryBuilder FilterBy(string filterText) { _filterText = filterText; return this; }

    /// <summary>Sets the branch sort order.</summary>
    public BranchQueryBuilder OrderBy(BranchOrderBy orderBy) { _orderBy = orderBy; return this; }

    /// <summary>Sets the page size (items per API call).</summary>
    public BranchQueryBuilder PageSize(int limit) { _limit = limit; return this; }

    /// <summary>Sets the maximum number of pages to fetch.</summary>
    public BranchQueryBuilder MaxPages(int pages) { _maxPages = pages; return this; }

    /// <summary>Sets the start index for pagination.</summary>
    public BranchQueryBuilder StartAt(int start) { _start = start; return this; }

    /// <summary>Executes the query and returns all matching branches.</summary>
    public Task<IReadOnlyList<Branch>> GetAsync(CancellationToken cancellationToken = default)
        => _client.GetBranchesAsync(_projectKey, _repositorySlug,
            _maxPages, _limit, _start, _baseBranchOrTag, _details, _filterText,
            _orderBy, cancellationToken);

    /// <summary>Executes the query and streams matching branches one at a time.</summary>
    public IAsyncEnumerable<Branch> StreamAsync(CancellationToken cancellationToken = default)
        => _client.GetBranchesStreamAsync(_projectKey, _repositorySlug,
            _maxPages, _limit, _start, _baseBranchOrTag, _details, _filterText,
            _orderBy, cancellationToken);
}