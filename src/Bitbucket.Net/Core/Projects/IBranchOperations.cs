using Bitbucket.Net.Models.Branches;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Core.Projects.Requests;

namespace Bitbucket.Net;

/// <summary>
/// Branch operations.
/// </summary>
public interface IBranchOperations
{
    Task<IReadOnlyList<BranchBase>> GetCommitBranchInfoAsync(string projectKey, string repositorySlug, string fullSha, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<BranchModel> GetRepoBranchModelAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default);
    Task<Branch> CreateRepoBranchAsync(string projectKey, string repositorySlug, string branchName, string startPoint, CancellationToken cancellationToken = default);
    Task<bool> DeleteRepoBranchAsync(string projectKey, string repositorySlug, string branchName, bool dryRun, string? endPoint = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Branch>> GetBranchesAsync(string projectKey, string repositorySlug, int? maxPages = null, int? limit = null, int? start = null, string? baseBranchOrTag = null, bool? details = null, string? filterText = null, BranchOrderBy? orderBy = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Branch> GetBranchesStreamAsync(string projectKey, string repositorySlug, int? maxPages = null, int? limit = null, int? start = null, string? baseBranchOrTag = null, bool? details = null, string? filterText = null, BranchOrderBy? orderBy = null, CancellationToken cancellationToken = default);
    Task<Branch> CreateBranchAsync(string projectKey, string repositorySlug, CreateBranchRequest request, CancellationToken cancellationToken = default);
    Task<Branch> GetDefaultBranchAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default);
    Task<bool> SetDefaultBranchAsync(string projectKey, string repositorySlug, BranchRef branchRef, CancellationToken cancellationToken = default);
    Task<BrowseItem> BrowseProjectRepositoryAsync(string projectKey, string repositorySlug, string at, bool type = false, bool blame = false, bool noContent = false, CancellationToken cancellationToken = default);
    Task<BrowsePathItem> BrowseProjectRepositoryPathAsync(string projectKey, string repositorySlug, string path, string at, bool type = false, bool blame = false, bool noContent = false, CancellationToken cancellationToken = default);
    Task<Stream> GetRawFileContentStreamAsync(string projectKey, string repositorySlug, string path, string? at = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> GetRawFileContentLinesStreamAsync(string projectKey, string repositorySlug, string path, string? at = null, CancellationToken cancellationToken = default);
    Task<Commit> UpdateProjectRepositoryPathAsync(string projectKey, string repositorySlug, string path, string fileName, string branch, string? message = null, string? sourceCommitId = null, string? sourceBranch = null, CancellationToken cancellationToken = default);
}