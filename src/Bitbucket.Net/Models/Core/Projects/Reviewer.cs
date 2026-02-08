namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// A pull request reviewer. Extends <see cref="Participant"/> with the last-reviewed commit reference.
/// </summary>
public class Reviewer : Participant
{
    /// <summary>
    /// Gets or sets the SHA of the last commit the reviewer has reviewed.
    /// </summary>
    public string? LastReviewedCommit { get; set; }
}