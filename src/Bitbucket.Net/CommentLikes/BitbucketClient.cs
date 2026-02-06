using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Users;
using Flurl.Http;

namespace Bitbucket.Net
{
    public partial class BitbucketClient
    {
        private IFlurlRequest GetCommentLikesUrl() => GetBaseUrl("/comment-likes");

        private IFlurlRequest GetCommentLikesUrl(string path) => GetCommentLikesUrl()
            .AppendPathSegment(path);

        public async Task<IEnumerable<User>> GetCommitCommentLikesAsync(string projectKey, string repositorySlug, string commitId, string commentId,
            int? maxPages = null,
            int? limit = null,
            int? start = null,
            int? avatarSize = null,
            CancellationToken cancellationToken = default)
        {
            var queryParamValues = new Dictionary<string, object?>
            {
                ["limit"] = limit,
                ["start"] = start,
                ["avatarSize"] = avatarSize
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetCommentLikesUrl($"/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/comments/{commentId}/likes")
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<User>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<bool> LikeCommitCommentAsync(string projectKey, string repositorySlug, string commitId, string commentId, CancellationToken cancellationToken = default)
        {
            var response = await GetCommentLikesUrl($"/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/comments/{commentId}/likes")
                .PostJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> UnlikeCommitCommentAsync(string projectKey, string repositorySlug, string commitId, string commentId, CancellationToken cancellationToken = default)
        {
            var response = await GetCommentLikesUrl($"/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/comments/{commentId}/likes")
                .DeleteAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<User>> GetPullRequestCommentLikesAsync(string projectKey, string repositorySlug, string pullRequestId, string commentId,
            int? maxPages = null,
            int? limit = null,
            int? start = null,
            CancellationToken cancellationToken = default)
        {
            var queryParamValues = new Dictionary<string, object?>
            {
                ["limit"] = limit,
                ["start"] = start
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetCommentLikesUrl($"/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/comments/{commentId}/likes")
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<User>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<bool> LikePullRequestCommentAsync(string projectKey, string repositorySlug, string pullRequestId, string commentId, CancellationToken cancellationToken = default)
        {
            var response = await GetCommentLikesUrl($"/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/comments/{commentId}/likes")
                .PostJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> UnlikePullRequestCommentAsync(string projectKey, string repositorySlug, string pullRequestId, string commentId, CancellationToken cancellationToken = default)
        {
            var response = await GetCommentLikesUrl($"/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/comments/{commentId}/likes")
                .DeleteAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }
    }
}
