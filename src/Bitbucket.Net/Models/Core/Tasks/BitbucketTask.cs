namespace Bitbucket.Net.Models.Core.Tasks;

/// <summary>
/// A Bitbucket pull request task. Extends <see cref="TaskRef"/> with an anchor and state.
/// </summary>
public class BitbucketTask : TaskRef
{
    /// <summary>
    /// Gets or sets the comment anchor this task is attached to.
    /// </summary>
    public TaskAnchor? Anchor { get; set; }

    /// <summary>
    /// Gets or sets the task state (e.g. "OPEN" or "RESOLVED").
    /// </summary>
    public string? State { get; set; }
}