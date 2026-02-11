namespace Bitbucket.Net.Models.Core.Projects.Requests;

/// <summary>
/// Request body for forking a repository.
/// </summary>
public sealed class ForkRepositoryRequest
{
    /// <summary>
    /// The slug for the forked repository. When omitted, defaults to the source repository slug.
    /// </summary>
    public string? Slug { get; init; }

    /// <summary>
    /// The display name for the forked repository.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// The target project for the fork. When omitted, the fork is created in the current user's personal project.
    /// </summary>
    public ProjectRef? Project { get; init; }
}