using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;

namespace Bitbucket.Net
{
    public partial class BitbucketClient
    {
        private IFlurlRequest GetApplicationPropertiesUrl() => GetBaseUrl()
            .AppendPathSegment("/application-properties");

        public async Task<IDictionary<string, object?>> GetApplicationPropertiesAsync(CancellationToken cancellationToken = default)
        {
            var response = await GetApplicationPropertiesUrl()
                .GetJsonAsync<Dictionary<string, object?>>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return response;
        }
    }
}
