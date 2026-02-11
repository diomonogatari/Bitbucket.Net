using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Core.Users;
using Flurl.Http;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
    /// <summary>
    /// Retrieves participants related to pull requests in a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="direction">Direction of pull requests to consider.</param>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="role">Optional role filter.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of identities.</returns>
    public async Task<IReadOnlyList<Identity>> GetRepositoryParticipantsAsync(string projectKey, string repositorySlug,
        PullRequestDirections direction = PullRequestDirections.Incoming,
        string? filter = null,
        Roles? role = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["direction"] = BitbucketHelpers.PullRequestDirectionToString(direction),
            ["filter"] = filter,
            ["role"] = BitbucketHelpers.RoleToString(role),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/participants")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<Identity>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves pull requests for a repository with optional filtering.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="direction">Pull request direction filter.</param>
    /// <param name="branchId">Optional branch filter.</param>
    /// <param name="state">Pull request state.</param>
    /// <param name="order">Ordering option.</param>
    /// <param name="withAttributes">Whether to include attributes.</param>
    /// <param name="withProperties">Whether to include properties.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of pull requests.</returns>
    public async Task<IReadOnlyList<PullRequest>> GetPullRequestsAsync(string projectKey, string repositorySlug,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        PullRequestDirections direction = PullRequestDirections.Incoming,
        string? branchId = null,
        PullRequestStates state = PullRequestStates.Open,
        PullRequestOrders order = PullRequestOrders.Newest,
        bool withAttributes = true,
        bool withProperties = true,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["direction"] = BitbucketHelpers.PullRequestDirectionToString(direction),
            ["at"] = branchId,
            ["state"] = BitbucketHelpers.PullRequestStateToString(state),
            ["order"] = BitbucketHelpers.PullRequestOrderToString(order),
            ["withAttributes"] = BitbucketHelpers.BoolToString(withAttributes),
            ["withProperties"] = BitbucketHelpers.BoolToString(withProperties),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/pull-requests")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<PullRequest>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams all pull requests for a repository as an IAsyncEnumerable.
    /// </summary>
    public IAsyncEnumerable<PullRequest> GetPullRequestsStreamAsync(string projectKey, string repositorySlug,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        PullRequestDirections direction = PullRequestDirections.Incoming,
        string? branchId = null,
        PullRequestStates state = PullRequestStates.Open,
        PullRequestOrders order = PullRequestOrders.Newest,
        bool withAttributes = true,
        bool withProperties = true,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["direction"] = BitbucketHelpers.PullRequestDirectionToString(direction),
            ["at"] = branchId,
            ["state"] = BitbucketHelpers.PullRequestStateToString(state),
            ["order"] = BitbucketHelpers.PullRequestOrderToString(order),
            ["withAttributes"] = BitbucketHelpers.BoolToString(withAttributes),
            ["withProperties"] = BitbucketHelpers.BoolToString(withProperties),
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/pull-requests")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<PullRequest>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a pull request in a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestInfo">The pull request payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created pull request.</returns>
    public async Task<PullRequest> CreatePullRequestAsync(string projectKey, string repositorySlug, PullRequestInfo pullRequestInfo, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/pull-requests")
            .SendAsync(HttpMethod.Post, CreateJsonContent(pullRequestInfo), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<PullRequest>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a pull request by ID.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested pull request.</returns>
    public async Task<PullRequest> GetPullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}")
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<PullRequest>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="pullRequestUpdate">The update payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated pull request.</returns>
    public async Task<PullRequest> UpdatePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, PullRequestUpdate pullRequestUpdate, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}")
            .SendAsync(HttpMethod.Put, CreateJsonContent(pullRequestUpdate), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<PullRequest>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="versionInfo">Version info for optimistic concurrency.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the pull request was deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeletePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, VersionInfo versionInfo, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}")
            .SendAsync(HttpMethod.Delete, CreateJsonContent(versionInfo), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }
    /// <summary>
    /// Declines a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="version">Optional version for optimistic concurrency.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the pull request was declined; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeclinePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, int version = -1, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["version"] = version,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/decline")
            .SetQueryParams(queryParamValues)
            .SendAsync(HttpMethod.Post, CreateEmptyJsonContent(), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves the merge state for a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="version">Optional version for optimistic concurrency.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The merge state.</returns>
    public async Task<PullRequestMergeState> GetPullRequestMergeStateAsync(string projectKey, string repositorySlug, long pullRequestId, int version = -1, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["version"] = version,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/merge")
            .SetQueryParams(queryParamValues)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<PullRequestMergeState>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the merge base (common ancestor) commit for a pull request.
    /// This is the best common ancestor between the latest commits of the source and target branches.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The merge base commit, or null if not found (HTTP 204 - no common ancestor exists).</returns>
    /// <remarks>
    /// This endpoint is useful for creating line-specific comments on pull requests.
    /// The returned commit ID can be used as the <c>fromHash</c> parameter when creating anchored comments,
    /// while the <c>toHash</c> can be obtained from <see cref="FromToRef.LatestCommit"/> on the pull request's FromRef.
    /// </remarks>
    public async Task<Commit?> GetPullRequestMergeBaseAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/merge-base")
            .AllowHttpStatus(204)
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        // HTTP 204 indicates no common ancestor exists (e.g., unrelated histories)
        if (response.StatusCode == 204)
        {
            return null;
        }

        return await HandleResponseAsync<Commit>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Merges a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="version">Optional version for optimistic concurrency.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The merged pull request.</returns>
    public async Task<PullRequest> MergePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, int version = -1, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["version"] = version,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/merge")
            .SetQueryParams(queryParamValues)
            .SendAsync(HttpMethod.Post, CreateEmptyJsonContent(), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<PullRequest>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Reopens a declined pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="version">Optional version for optimistic concurrency.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The reopened pull request.</returns>
    public async Task<PullRequest> ReopenPullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, int version = -1, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["version"] = version,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/reopen")
            .SetQueryParams(queryParamValues)
            .SendAsync(HttpMethod.Post, CreateEmptyJsonContent(), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<PullRequest>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Approves a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The reviewer entry reflecting the approval.</returns>
    public async Task<Reviewer> ApprovePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/approve")
            .SendAsync(HttpMethod.Post, CreateEmptyJsonContent(), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Reviewer>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes an approval from a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The reviewer entry after removal.</returns>
    public async Task<Reviewer> DeletePullRequestApprovalAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/approve")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Reviewer>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves participants for a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of participants.</returns>
    public async Task<IReadOnlyList<Participant>> GetPullRequestParticipantsAsync(string projectKey, string repositorySlug, long pullRequestId,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug)
                    .AppendPathSegment($"/pull-requests/{pullRequestId}/participants")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<Participant>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams participants for a pull request, yielding items as they are retrieved.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>An async enumerable of participants.</returns>
    public IAsyncEnumerable<Participant> GetPullRequestParticipantsStreamAsync(string projectKey, string repositorySlug, long pullRequestId,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["avatarSize"] = avatarSize,
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug)
                    .AppendPathSegment($"/pull-requests/{pullRequestId}/participants")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<Participant>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken);
    }

    /// <summary>
    /// Assigns a role to a user in a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="named">The user to assign.</param>
    /// <param name="role">The role to assign.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created participant entry.</returns>
    public async Task<Participant> AssignUserRoleToPullRequestAsync(string projectKey, string repositorySlug, long pullRequestId,
        Named named,
        Roles role,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            user = named,
            role = BitbucketHelpers.RoleToString(role),
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/participants")
            .SendAsync(HttpMethod.Post, CreateJsonContent(data), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Participant>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a participant from a pull request by username.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="userName">The username to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if removal succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeletePullRequestParticipantAsync(string projectKey, string repositorySlug, long pullRequestId, string userName, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/participants")
            .SetQueryParam("username", userName)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a participant's approval status on a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="userSlug">The user slug to update.</param>
    /// <param name="named">The user identity.</param>
    /// <param name="approved">Whether the participant approves the PR.</param>
    /// <param name="participantStatus">The participant status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated participant entry.</returns>
    public async Task<Participant> UpdatePullRequestParticipantStatus(string projectKey, string repositorySlug, long pullRequestId,
        string userSlug,
        Named named,
        bool approved,
        ParticipantStatus participantStatus,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            user = named,
            approved = BitbucketHelpers.BoolToString(approved),
            status = BitbucketHelpers.ParticipantStatusToString(participantStatus),
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/participants/{userSlug}")
            .SendAsync(HttpMethod.Put, CreateJsonContent(data), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Participant>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a participant from a pull request by user slug.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="userSlug">The user slug to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if removal succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> UnassignUserFromPullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, string userSlug, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/participants/{userSlug}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

}