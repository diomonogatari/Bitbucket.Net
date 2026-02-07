using Flurl.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
    private IFlurlRequest GetHooksUrl() => GetBaseUrl()
        .AppendPathSegment("/hooks");

    public async Task<byte[]> GetProjectHooksAvatarAsync(string hookKey, string? version = null, CancellationToken cancellationToken = default)
    {
        return await GetHooksUrl()
            .AppendPathSegment($"/{hookKey}/avatar")
            .SetQueryParam("version", version)
            .GetBytesAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}