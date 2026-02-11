namespace Bitbucket.Net.Models.Core.Projects.Requests;

/// <summary>
/// Request body for creating a new branch.
/// </summary>
public sealed class CreateBranchRequest
{
    /// <summary>
    /// The branch name (e.g. "feature/my-feature"). Required.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The starting point for the branch — a commit SHA, branch name, or tag. Required.
    /// </summary>
    public required string StartPoint { get; init; }

    /// <summary>
    /// An optional message to associate with the branch creation.
    /// </summary>
    public string? Message { get; init; }
}