using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Builders;

/// <summary>
/// Fluent builder for querying projects.
/// </summary>
public sealed class ProjectQueryBuilder
{
    private readonly BitbucketClient _client;

    private int? _maxPages;
    private int? _limit;
    private int? _start;
    private string? _name;
    private Permissions? _permission;

    internal ProjectQueryBuilder(BitbucketClient client)
    {
        _client = client;
    }

    /// <summary>Filters projects by name prefix.</summary>
    public ProjectQueryBuilder NameFilter(string name) { _name = name; return this; }

    /// <summary>Filters projects by the current user's permission level.</summary>
    public ProjectQueryBuilder WithPermission(Permissions permission) { _permission = permission; return this; }

    /// <summary>Sets the page size (items per API call).</summary>
    public ProjectQueryBuilder PageSize(int limit) { _limit = limit; return this; }

    /// <summary>Sets the maximum number of pages to fetch.</summary>
    public ProjectQueryBuilder MaxPages(int pages) { _maxPages = pages; return this; }

    /// <summary>Sets the start index for pagination.</summary>
    public ProjectQueryBuilder StartAt(int start) { _start = start; return this; }

    /// <summary>Executes the query and returns all matching projects.</summary>
    public Task<IReadOnlyList<Project>> GetAsync(CancellationToken cancellationToken = default)
        => _client.GetProjectsAsync(_maxPages, _limit, _start, _name, _permission, cancellationToken);

    /// <summary>Executes the query and streams matching projects one at a time.</summary>
    public IAsyncEnumerable<Project> StreamAsync(CancellationToken cancellationToken = default)
        => _client.GetProjectsStreamAsync(_maxPages, _limit, _start, _name, _permission, cancellationToken);
}