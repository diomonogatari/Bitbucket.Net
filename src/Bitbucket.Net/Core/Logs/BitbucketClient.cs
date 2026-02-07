using Bitbucket.Net.Common;
using Bitbucket.Net.Models.Core.Logs;
using Flurl.Http;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
    private IFlurlRequest GetLogsUrl() => GetBaseUrl()
        .AppendPathSegment("/logs");

    private IFlurlRequest GetLogsUrl(string path) => GetLogsUrl()
        .AppendPathSegment(path);

    public async Task<LogLevels> GetLogLevelAsync(string loggerName, CancellationToken cancellationToken = default)
    {
        var response = await GetLogsUrl($"/logger/{loggerName}")
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, s =>
        {
            using var doc = JsonDocument.Parse(s);
            return BitbucketHelpers.StringToLogLevel(doc.RootElement.GetProperty("logLevel").GetString()!);
        }, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> SetLogLevelAsync(string loggerName, LogLevels logLevel, CancellationToken cancellationToken = default)
    {
        var response = await GetLogsUrl($"/logger/{loggerName}/{BitbucketHelpers.LogLevelToString(logLevel)}")
            .PutAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<LogLevels> GetRootLogLevelAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetLogsUrl("/logger/rootLogger")
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, s =>
        {
            using var doc = JsonDocument.Parse(s);
            return BitbucketHelpers.StringToLogLevel(doc.RootElement.GetProperty("logLevel").GetString()!);
        }, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> SetRootLogLevelAsync(LogLevels logLevel, CancellationToken cancellationToken = default)
    {
        var response = await GetLogsUrl($"/logger/rootLogger/{BitbucketHelpers.LogLevelToString(logLevel)}")
            .PutAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }
}