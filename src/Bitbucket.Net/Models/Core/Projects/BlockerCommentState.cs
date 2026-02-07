namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Represents the state of a blocker comment (task) in Bitbucket Server 9.0+.
/// </summary>
public enum BlockerCommentState
{
    /// <summary>
    /// The blocker comment is open and must be addressed before merging.
    /// </summary>
    Open,

    /// <summary>
    /// The blocker comment has been resolved.
    /// </summary>
    Resolved,
}