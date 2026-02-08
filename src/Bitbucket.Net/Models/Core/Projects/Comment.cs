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
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the version number for optimistic locking on updates.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets the comment body text.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Bitbucket Server comment state.
    /// Common values: OPEN, PENDING, RESOLVED.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Indicates whether the whole comment thread is resolved.
    /// When true, Bitbucket UI will typically collapse/hide the thread as resolved.
    /// </summary>
    public bool? ThreadResolved { get; set; }

    /// <summary>
    /// The user who resolved the comment thread (when resolved).
    /// </summary>
    public User? Resolver { get; set; }

    /// <summary>
    /// When the comment thread was resolved (when resolved).
    /// </summary>
    [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
    public DateTimeOffset? ResolvedDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the comment was created.
    /// </summary>
    [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
    public DateTimeOffset? CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the comment was last updated.
    /// </summary>
    [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
    public DateTimeOffset? UpdatedDate { get; set; }

    /// <summary>
    /// Gets or sets the user who authored the comment.
    /// </summary>
    public User? Author { get; set; }

    /// <summary>
    /// Gets or sets the nested reply comments.
    /// </summary>
    public List<Comment>? Comments { get; set; }

    /// <summary>
    /// Gets or sets the tasks associated with this comment.
    /// </summary>
    public List<BitbucketTask>? Tasks { get; set; }

    /// <summary>
    /// Gets or sets the participants in the comment thread.
    /// </summary>
    public List<Participant>? Participants { get; set; }

    /// <summary>
    /// Gets or sets the hypermedia links for this comment.
    /// </summary>
    public Links? Links { get; set; }
}