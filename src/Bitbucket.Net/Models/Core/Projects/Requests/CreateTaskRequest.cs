using Bitbucket.Net.Models.Core.Tasks;

namespace Bitbucket.Net.Models.Core.Projects.Requests;

/// <summary>
/// Request body for creating a new task on a pull request comment.
/// </summary>
public sealed class CreateTaskRequest
{
    /// <summary>
    /// The task description text. Required.
    /// </summary>
    public required string Text { get; init; }

    /// <summary>
    /// The anchor comment to which this task is attached.
    /// </summary>
    public TaskBasicAnchor? Anchor { get; init; }
}