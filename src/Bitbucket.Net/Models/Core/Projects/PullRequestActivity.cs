using Bitbucket.Net.Common.Converters;
using Bitbucket.Net.Models.Core.Users;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// An activity event on a Bitbucket pull request (e.g. comment, approval, merge).
/// </summary>
public class PullRequestActivity
{
    /// <summary>
    /// Gets or sets the server-assigned activity identifier.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets or sets the date and time when the activity occurred.
    /// </summary>
    [JsonConverter(typeof(NullableUnixDateTimeOffsetConverter))]
    public DateTimeOffset? CreatedDate { get; init; }

    /// <summary>
    /// Gets or sets the user who performed the activity.
    /// </summary>
    public User? User { get; init; }

    /// <summary>
    /// Gets or sets the activity action type (e.g. "COMMENTED", "APPROVED", "MERGED", "OPENED").
    /// </summary>
    public string? Action { get; init; }

    /// <summary>
    /// Gets or sets the comment-specific action (e.g. "ADDED", "UPDATED", "DELETED") when the activity involves a comment.
    /// </summary>
    public string? CommentAction { get; init; }

    /// <summary>
    /// Gets or sets the comment associated with this activity, if any.
    /// </summary>
    public Comment? Comment { get; init; }

    /// <summary>
    /// Gets or sets the anchor location for an inline comment, if applicable.
    /// </summary>
    public CommentAnchor? CommentAnchor { get; init; }

    /// <summary>
    /// Gets or sets the commit associated with this activity, if any.
    /// </summary>
    public Commit? Commit { get; init; }
}