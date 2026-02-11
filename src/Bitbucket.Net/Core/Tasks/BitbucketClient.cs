using Bitbucket.Net.Common;
using Bitbucket.Net.Models.Core.Projects.Requests;
using Bitbucket.Net.Models.Core.Tasks;
using Flurl.Http;

namespace Bitbucket.Net;

/// <summary>
/// Provides task-related Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base tasks URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the tasks endpoint.</returns>
    private IFlurlRequest GetTasksUrl() => GetBaseUrl()
        .AppendPathSegment("/tasks");

    /// <summary>
    /// Gets the tasks URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append to the tasks endpoint.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the tasks path.</returns>
    private IFlurlRequest GetTasksUrl(string path) => GetTasksUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Creates a task.
    /// </summary>
    /// <param name="request">The create task request.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created task.</returns>
    public async Task<BitbucketTask> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await GetTasksUrl()
            .SendAsync(HttpMethod.Post, CreateJsonContent(request), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BitbucketTask>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a task by identifier.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="avatarSize">Optional avatar size for returned users.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The requested task.</returns>
    public async Task<BitbucketTask> GetTaskAsync(long taskId, int? avatarSize = null, CancellationToken cancellationToken = default)
    {
        var response = await GetTasksUrl($"/{taskId}")
            .SetQueryParam("avatarSize", avatarSize)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BitbucketTask>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a task.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="request">The update task request.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The updated task.</returns>
    public async Task<BitbucketTask> UpdateTaskAsync(long taskId, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var obj = new
        {
            id = taskId,
            text = request.Text,
            state = request.State,
        };

        var response = await GetTasksUrl($"/{taskId}")
            .SendAsync(HttpMethod.Put, CreateJsonContent(obj), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BitbucketTask>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the task was deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteTaskAsync(long taskId, CancellationToken cancellationToken = default)
    {
        var response = await GetTasksUrl($"/{taskId}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }
}