using Bitbucket.Net.Common.Converters;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Full Bitbucket pull request. Extends <see cref="PullRequestInfo"/> with server-assigned identity, timestamps, and participants.
/// </summary>
public class PullRequest : PullRequestInfo
{
    /// <summary>
    /// Gets or sets the server-assigned pull request identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the version number for optimistic locking on updates.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the pull request was created.
    /// </summary>
    [JsonConverter(typeof(NullableUnixDateTimeOffsetConverter))]
    public DateTimeOffset? CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the pull request was last updated.
    /// </summary>
    [JsonConverter(typeof(NullableUnixDateTimeOffsetConverter))]
    public DateTimeOffset? UpdatedDate { get; set; }

    /// <summary>
    /// Gets or sets the pull request author.
    /// </summary>
    public Participant? Author { get; set; }

    /// <summary>
    /// Gets or sets the list of participants (author, reviewers, and watchers).
    /// </summary>
    public List<Participant>? Participants { get; set; }

    /// <summary>
    /// Gets or sets the hypermedia links for this pull request.
    /// </summary>
    public Links? Links { get; set; }

    /// <summary>
    /// Returns a human-readable label combining the author display name and title.
    /// </summary>
    public override string ToString() => $"{Author?.User?.DisplayName ?? "Unknown"}: {Title ?? "(untitled)"}";
}