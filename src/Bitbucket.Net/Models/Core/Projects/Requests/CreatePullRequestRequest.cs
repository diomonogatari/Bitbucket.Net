namespace Bitbucket.Net.Models.Core.Projects.Requests;

/// <summary>
/// Request body for creating a new pull request.
/// </summary>
public sealed class CreatePullRequestRequest
{
    /// <summary>
    /// The pull request title. Required.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// An optional description in Markdown format.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// The source branch reference. Required.
    /// </summary>
    public required FromToRef FromRef { get; init; }

    /// <summary>
    /// The target branch reference. Required.
    /// </summary>
    public required FromToRef ToRef { get; init; }

    /// <summary>
    /// An optional list of reviewers to add to the pull request.
    /// </summary>
    public IReadOnlyList<Reviewer>? Reviewers { get; init; }
}