using Bitbucket.Net.Common.Converters;
using Bitbucket.Net.Models.Core.Tasks;
using Bitbucket.Net.Models.Core.Users;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// A comment on a Bitbucket pull request, file, or commit.
/// </summary>
public class Comment
{
    /// <summary>
    /// Gets or sets the server-assigned comment identifier.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets or sets the version number for optimistic locking on updates.
    /// </summary>
    public int Version { get; init; }

    /// <summary>
    /// Gets or sets the comment body text.
    /// </summary>
    public string? Text { get; init; }

    /// <summary>
    /// Bitbucket Server comment state.
    /// Common values: OPEN, PENDING, RESOLVED.
    /// </summary>
    public string? State { get; init; }

    /// <summary>
    /// Indicates whether the whole comment thread is resolved.
    /// When true, Bitbucket UI will typically collapse/hide the thread as resolved.
    /// </summary>
    public bool? ThreadResolved { get; init; }

    /// <summary>
    /// The user who resolved the comment thread (when resolved).
    /// </summary>
    public User? Resolver { get; init; }

    /// <summary>
    /// When the comment thread was resolved (when resolved).
    /// </summary>
    [JsonConverter(typeof(NullableUnixDateTimeOffsetConverter))]
    public DateTimeOffset? ResolvedDate { get; init; }

    /// <summary>
    /// Gets or sets the date and time when the comment was created.
    /// </summary>
    [JsonConverter(typeof(NullableUnixDateTimeOffsetConverter))]
    public DateTimeOffset? CreatedDate { get; init; }

    /// <summary>
    /// Gets or sets the date and time when the comment was last updated.
    /// </summary>
    [JsonConverter(typeof(NullableUnixDateTimeOffsetConverter))]
    public DateTimeOffset? UpdatedDate { get; init; }

    /// <summary>
    /// Gets or sets the user who authored the comment.
    /// </summary>
    public User? Author { get; init; }

    /// <summary>
    /// Gets or sets the nested reply comments.
    /// </summary>
    public List<Comment>? Comments { get; init; }

    /// <summary>
    /// Gets or sets the tasks associated with this comment.
    /// </summary>
    public List<BitbucketTask>? Tasks { get; init; }

    /// <summary>
    /// Gets or sets the participants in the comment thread.
    /// </summary>
    public List<Participant>? Participants { get; init; }

    /// <summary>
    /// Gets or sets the hypermedia links for this comment.
    /// </summary>
    public Links? Links { get; init; }
}