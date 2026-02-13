using Bitbucket.Net.Models.RefRestrictions;
using Bitbucket.Net.Models.RefSync;

namespace Bitbucket.Net;

/// <summary>
/// Ref restriction and ref sync operations.
/// </summary>
public interface IRefRestrictionOperations
{
    Task<IReadOnlyList<RefRestriction>> GetProjectRefRestrictionsAsync(string projectKey, RefRestrictionTypes? type = null, RefMatcherTypes? matcherType = null, string? matcherId = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RefRestriction>> CreateProjectRefRestrictionsAsync(string projectKey, CancellationToken cancellationToken, params RefRestrictionCreate[] refRestrictions);
    Task<IReadOnlyList<RefRestriction>> CreateProjectRefRestrictionsAsync(string projectKey, params RefRestrictionCreate[] refRestrictions);
    Task<RefRestriction> CreateProjectRefRestrictionAsync(string projectKey, RefRestrictionCreate refRestriction, CancellationToken cancellationToken = default);
    Task<RefRestriction> GetProjectRefRestrictionAsync(string projectKey, int refRestrictionId, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectRefRestrictionAsync(string projectKey, int refRestrictionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RefRestriction>> GetRepositoryRefRestrictionsAsync(string projectKey, string repositorySlug, RefRestrictionTypes? type = null, RefMatcherTypes? matcherType = null, string? matcherId = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RefRestriction>> CreateRepositoryRefRestrictionsAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken, params RefRestrictionCreate[] refRestrictions);
    Task<IReadOnlyList<RefRestriction>> CreateRepositoryRefRestrictionsAsync(string projectKey, string repositorySlug, params RefRestrictionCreate[] refRestrictions);
    Task<RefRestriction> CreateRepositoryRefRestrictionAsync(string projectKey, string repositorySlug, RefRestrictionCreate refRestriction, CancellationToken cancellationToken = default);
    Task<RefRestriction> GetRepositoryRefRestrictionAsync(string projectKey, string repositorySlug, int refRestrictionId, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteRepositoryRefRestrictionAsync(string projectKey, string repositorySlug, int refRestrictionId, CancellationToken cancellationToken = default);
    Task<RepositorySynchronizationStatus> GetRepositorySynchronizationStatusAsync(string projectKey, string repositorySlug, string? at = null, CancellationToken cancellationToken = default);
    Task<RepositorySynchronizationStatus> EnableRepositorySynchronizationAsync(string projectKey, string repositorySlug, bool enabled, CancellationToken cancellationToken = default);
    Task<FullRef> SynchronizeRepositoryAsync(string projectKey, string repositorySlug, Synchronize synchronize, CancellationToken cancellationToken = default);
}