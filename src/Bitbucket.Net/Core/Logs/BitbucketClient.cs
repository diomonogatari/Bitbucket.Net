using Bitbucket.Net.Common;
using Bitbucket.Net.Models.Core.Logs;
using Flurl.Http;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Bitbucket.Net;

/// <summary>
/// Provides log-level management Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base logs URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the logs endpoint.</returns>
    private IFlurlRequest GetLogsUrl() => GetBaseUrl()
        .AppendPathSegment("/logs");

    /// <summary>
    /// Gets the logs URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append to the logs endpoint.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the logs path.</returns>
    private IFlurlRequest GetLogsUrl(string path) => GetLogsUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Retrieves the log level for a specific logger.
    /// </summary>
    /// <param name="loggerName">The logger name.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The configured log level.</returns>
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

    /// <summary>
    /// Sets the log level for a specific logger.
    /// </summary>
    /// <param name="loggerName">The logger name.</param>
    /// <param name="logLevel">The log level to set.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the update succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> SetLogLevelAsync(string loggerName, LogLevels logLevel, CancellationToken cancellationToken = default)
    {
        var response = await GetLogsUrl($"/logger/{loggerName}/{BitbucketHelpers.LogLevelToString(logLevel)}")
            .PutAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves the root logger's log level.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The root log level.</returns>
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

    /// <summary>
    /// Sets the root logger's log level.
    /// </summary>
    /// <param name="logLevel">The log level to set.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the update succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> SetRootLogLevelAsync(LogLevels logLevel, CancellationToken cancellationToken = default)
    {
        var response = await GetLogsUrl($"/logger/rootLogger/{BitbucketHelpers.LogLevelToString(logLevel)}")
            .PutAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }
}