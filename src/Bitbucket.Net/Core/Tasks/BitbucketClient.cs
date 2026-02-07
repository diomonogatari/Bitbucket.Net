using Bitbucket.Net.Models.Core.Tasks;
using Flurl.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
    private IFlurlRequest GetTasksUrl() => GetBaseUrl()
        .AppendPathSegment("/tasks");

    private IFlurlRequest GetTasksUrl(string path) => GetTasksUrl()
        .AppendPathSegment(path);

    public async Task<BitbucketTask> CreateTaskAsync(TaskInfo taskInfo, CancellationToken cancellationToken = default)
    {
        var response = await GetTasksUrl()
            .PostJsonAsync(taskInfo, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BitbucketTask>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<BitbucketTask> GetTaskAsync(long taskId, int? avatarSize = null, CancellationToken cancellationToken = default)
    {
        return await GetTasksUrl($"/{taskId}")
            .SetQueryParam("avatarSize", avatarSize)
            .GetJsonAsync<BitbucketTask>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<BitbucketTask> UpdateTaskAsync(long taskId, string text, CancellationToken cancellationToken = default)
    {
        var obj = new
        {
            id = taskId,
            text,
        };

        var response = await GetTasksUrl($"/{taskId}")
            .PutJsonAsync(obj, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BitbucketTask>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> DeleteTaskAsync(long taskId, CancellationToken cancellationToken = default)
    {
        var response = await GetTasksUrl($"/{taskId}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }
}