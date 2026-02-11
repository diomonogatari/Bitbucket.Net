namespace Bitbucket.Net.Models.Core.Projects.Requests;

/// <summary>
/// Request body for creating a new repository in a project.
/// </summary>
public sealed class CreateRepositoryRequest
{
    /// <summary>
    /// The repository name. Required.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The SCM type identifier. Defaults to "git".
    /// </summary>
    public string ScmId { get; init; } = "git";

    /// <summary>
    /// Whether the repository may be forked.
    /// </summary>
    public bool? Forkable { get; init; }
}