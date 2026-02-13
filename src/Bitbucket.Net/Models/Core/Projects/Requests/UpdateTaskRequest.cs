namespace Bitbucket.Net.Models.Core.Projects.Requests;

/// <summary>
/// Request body for updating an existing task.
/// </summary>
public sealed class UpdateTaskRequest
{
    /// <summary>
    /// The new task description text. Required.
    /// </summary>
    public required string Text { get; init; }

    /// <summary>
    /// The desired task state (e.g. "OPEN", "RESOLVED").
    /// </summary>
    public string? State { get; init; }
}