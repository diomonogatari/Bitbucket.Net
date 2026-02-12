using Bitbucket.Net.Models.Builds;
using Bitbucket.Net.Models.Builds.Requests;

namespace Bitbucket.Net;

/// <summary>
/// Build status operations.
/// </summary>
public interface IBuildOperations
{
    Task<BuildStats> GetBuildStatsForCommitAsync(string commitId, bool includeUnique = false, CancellationToken cancellationToken = default);
    Task<Dictionary<string, BuildStats>> GetBuildStatsForCommitsAsync(CancellationToken cancellationToken, params string[] commitIds);
    Task<Dictionary<string, BuildStats>> GetBuildStatsForCommitsAsync(params string[] commitIds);
    Task<IReadOnlyList<BuildStatus>> GetBuildStatusForCommitAsync(string commitId, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<bool> AssociateBuildStatusWithCommitAsync(string commitId, AssociateBuildStatusRequest request, CancellationToken cancellationToken = default);
}