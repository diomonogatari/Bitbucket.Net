namespace Bitbucket.Net.Models.Core.Projects.Requests;

/// <summary>
/// Request body for updating an existing pull request.
/// </summary>
public sealed class UpdatePullRequestRequest
{
    /// <summary>
    /// The expected current version of the pull request for optimistic locking. Required.
    /// </summary>
    public required int Version { get; init; }

    /// <summary>
    /// The new pull request title.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// The new pull request description in Markdown format.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// The updated list of reviewers.
    /// </summary>
    public IReadOnlyList<Reviewer>? Reviewers { get; init; }
}