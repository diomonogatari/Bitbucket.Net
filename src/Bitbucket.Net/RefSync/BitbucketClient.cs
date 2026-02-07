using Bitbucket.Net.Common;
using Bitbucket.Net.Models.RefSync;
using Flurl.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bitbucket.Net;

/// <summary>
/// Provides repository synchronization Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base reference synchronization URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the sync root.</returns>
    private IFlurlRequest GetRefSyncUrl() => GetBaseUrl("/sync");

    /// <summary>
    /// Gets the reference synchronization URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append to the sync root.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the sync path.</returns>
    private IFlurlRequest GetRefSyncUrl(string path) => GetRefSyncUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Retrieves the synchronization status for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="at">Optional reference to scope the status.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The repository synchronization status.</returns>
    public async Task<RepositorySynchronizationStatus> GetRepositorySynchronizationStatusAsync(string projectKey, string repositorySlug,
        string? at = null, CancellationToken cancellationToken = default)
    {
        var response = await GetRefSyncUrl($"/projects/{projectKey}/repos/{repositorySlug}")
            .SetQueryParam("at", at)
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<RepositorySynchronizationStatus>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Enables or disables repository synchronization.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="enabled">Whether synchronization should be enabled.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The updated repository synchronization status.</returns>
    public async Task<RepositorySynchronizationStatus> EnableRepositorySynchronizationAsync(string projectKey, string repositorySlug, bool enabled, CancellationToken cancellationToken = default)
    {
        var data = new
        {
            enabled = BitbucketHelpers.BoolToString(enabled),
        };

        var response = await GetRefSyncUrl($"/projects/{projectKey}/repos/{repositorySlug}")
            .PostJsonAsync(data, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<RepositorySynchronizationStatus>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Triggers synchronization for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="synchronize">The synchronization payload.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The result of the synchronization.</returns>
    public async Task<FullRef> SynchronizeRepositoryAsync(string projectKey, string repositorySlug, Synchronize synchronize, CancellationToken cancellationToken = default)
    {
        var response = await GetRefSyncUrl($"/projects/{projectKey}/repos/{repositorySlug}")
            .PostJsonAsync(synchronize, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<FullRef>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}