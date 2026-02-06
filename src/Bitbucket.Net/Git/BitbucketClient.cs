using System.Threading;
using System.Threading.Tasks;
using Bitbucket.Net.Common;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Git;
using Flurl.Http;

namespace Bitbucket.Net
{
    public partial class BitbucketClient
    {
        private IFlurlRequest GetGitUrl() => GetBaseUrl("/git");

        private IFlurlRequest GetGitUrl(string path) => GetGitUrl()
            .AppendPathSegment(path);

        public async Task<RebasePullRequestCondition> GetCanRebasePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default)
        {
            var response = await GetGitUrl($"/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/rebase")
                .GetAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<RebasePullRequestCondition>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<PullRequest> RebasePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, int version, CancellationToken cancellationToken = default)
        {
            var data = new { version };
            var response = await GetGitUrl($"/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/rebase")
                .PostJsonAsync(data, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<PullRequest>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<Tag> CreateTagAsync(string projectKey, string repositorySlug, TagTypes tagType, string tagName, string startPoint, CancellationToken cancellationToken = default)
        {
            var data = new
            {
                type = BitbucketHelpers.TagTypeToString(tagType),
                name = tagName,
                startPoint
            };

            var response = await GetGitUrl($"/projects/{projectKey}/repos/{repositorySlug}/tags")
                .PostJsonAsync(data, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<Tag>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> DeleteTagAsync(string projectKey, string repositorySlug, string tagName, CancellationToken cancellationToken = default)
        {
            var response = await GetGitUrl($"/projects/{projectKey}/repos/{repositorySlug}/tags/{tagName}")
                .DeleteAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }
    }
}
