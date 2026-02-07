using Bitbucket.Net.Models.Core.Users;
using Bitbucket.Net.Models.DefaultReviewers;
using Flurl.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
    private IFlurlRequest GetDefaultReviewersUrl() => GetBaseUrl("/default-reviewers");

    private IFlurlRequest GetDefaultReviewersUrl(string path) => GetDefaultReviewersUrl()
        .AppendPathSegment(path);

    public async Task<IEnumerable<DefaultReviewerPullRequestCondition>> GetDefaultReviewerConditionsAsync(string projectKey,
        int? avatarSize = null, CancellationToken cancellationToken = default)
    {
        var response = await GetDefaultReviewersUrl($"/projects/{projectKey}/conditions")
            .SetQueryParam("avatarSize", avatarSize)
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<IEnumerable<DefaultReviewerPullRequestCondition>>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<DefaultReviewerPullRequestCondition> CreateDefaultReviewerConditionAsync(string projectKey, DefaultReviewerPullRequestCondition condition, CancellationToken cancellationToken = default)
    {
        var response = await GetDefaultReviewersUrl($"/projects/{projectKey}/conditions")
            .PostJsonAsync(condition, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<DefaultReviewerPullRequestCondition>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<DefaultReviewerPullRequestCondition> UpdateDefaultReviewerConditionAsync(string projectKey, string defaultReviewerPullRequestConditionId, DefaultReviewerPullRequestCondition condition, CancellationToken cancellationToken = default)
    {
        var response = await GetDefaultReviewersUrl($"/projects/{projectKey}/conditions/{defaultReviewerPullRequestConditionId}")
            .PutJsonAsync(condition, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<DefaultReviewerPullRequestCondition>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> DeleteDefaultReviewerConditionAsync(string projectKey, string defaultReviewerPullRequestConditionId, CancellationToken cancellationToken = default)
    {
        var response = await GetDefaultReviewersUrl($"/projects/{projectKey}/conditions/{defaultReviewerPullRequestConditionId}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<DefaultReviewerPullRequestCondition>> GetDefaultReviewerConditionsAsync(string projectKey, string repositorySlug,
        int? avatarSize = null, CancellationToken cancellationToken = default)
    {
        var response = await GetDefaultReviewersUrl($"/projects/{projectKey}/repos/{repositorySlug}/conditions")
            .SetQueryParam("avatarSize", avatarSize)
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<IEnumerable<DefaultReviewerPullRequestCondition>>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<DefaultReviewerPullRequestCondition> CreateDefaultReviewerConditionAsync(string projectKey, string repositorySlug, DefaultReviewerPullRequestCondition condition, CancellationToken cancellationToken = default)
    {
        var response = await GetDefaultReviewersUrl($"/projects/{projectKey}/repos/{repositorySlug}/conditions")
            .PostJsonAsync(condition, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<DefaultReviewerPullRequestCondition>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<DefaultReviewerPullRequestCondition> UpdateDefaultReviewerConditionAsync(string projectKey, string repositorySlug, string defaultReviewerPullRequestConditionId, DefaultReviewerPullRequestCondition condition, CancellationToken cancellationToken = default)
    {
        var response = await GetDefaultReviewersUrl($"/projects/{projectKey}/repos/{repositorySlug}/conditions/{defaultReviewerPullRequestConditionId}")
            .PutJsonAsync(condition, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<DefaultReviewerPullRequestCondition>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> DeleteDefaultReviewerConditionAsync(string projectKey, string repositorySlug, string defaultReviewerPullRequestConditionId, CancellationToken cancellationToken = default)
    {
        var response = await GetDefaultReviewersUrl($"/projects/{projectKey}/repos/{repositorySlug}/conditions/{defaultReviewerPullRequestConditionId}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<User>> GetDefaultReviewersAsync(string projectKey, string repositorySlug,
        int? sourceRepoId = null,
        int? targetRepoId = null,
        string? sourceRefId = null,
        string? targetRefId = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["sourceRepoId"] = sourceRepoId,
            ["targetRepoId"] = targetRepoId,
            ["sourceRefId"] = sourceRefId,
            ["targetRefId"] = targetRefId,
            ["avatarSize"] = avatarSize,
        };

        var response = await GetDefaultReviewersUrl($"/projects/{projectKey}/repos/{repositorySlug}/reviewers")
            .SetQueryParams(queryParamValues)
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<IEnumerable<User>>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}