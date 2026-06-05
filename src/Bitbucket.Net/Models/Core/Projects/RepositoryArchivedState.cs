namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Controls how archived repositories are included when searching repositories
/// via the <c>/repos</c> endpoint.
/// </summary>
public enum RepositoryArchivedState
{
    /// <summary>Only non-archived repositories (the Bitbucket Server default).</summary>
    Active,

    /// <summary>Only archived repositories.</summary>
    Archived,

    /// <summary>Both active and archived repositories.</summary>
    All,
}
