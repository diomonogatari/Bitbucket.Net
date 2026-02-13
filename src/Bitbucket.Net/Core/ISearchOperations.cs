using Bitbucket.Net.Common.Models.Search;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net;

/// <summary>
/// Search and repository listing operations.
/// </summary>
public interface ISearchOperations
{
    Task<IReadOnlyList<Repository>> GetRepositoriesAsync(int? maxPages = null, int? limit = null, int? start = null, string? name = null, string? projectName = null, Permissions? permission = null, bool isPublic = false, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Repository> GetRepositoriesStreamAsync(int? maxPages = null, int? limit = null, int? start = null, string? name = null, string? projectName = null, Permissions? permission = null, bool isPublic = false, CancellationToken cancellationToken = default);
    Task<CodeSearchResponse> SearchCodeAsync(string query, int primaryLimit = 25, int secondaryLimit = 10, CancellationToken cancellationToken = default);
    Task<bool> IsSearchAvailableAsync(CancellationToken cancellationToken = default);
}