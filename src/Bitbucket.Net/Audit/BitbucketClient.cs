using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Audit;
using Flurl.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
    private IFlurlRequest GetAuditUrl() => GetBaseUrl("/audit");

    private IFlurlRequest GetAuditUrl(string path) => GetAuditUrl()
        .AppendPathSegment(path);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage", Justification = "<Pending>")]
    public async Task<IEnumerable<AuditEvent>> GetProjectAuditEventsAsync(string projectKey,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetAuditUrl($"/projects/{projectKey}/events")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<AuditEvent>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage", Justification = "<Pending>")]
    public async Task<IEnumerable<AuditEvent>> GetProjectRepoAuditEventsAsync(string projectKey, string repositorySlug,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetAuditUrl($"/projects/{projectKey}/repos/{repositorySlug}/events")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<AuditEvent>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }
}