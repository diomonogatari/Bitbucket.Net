using Bitbucket.Net.Common.Converters;
using Bitbucket.Net.Models.Core.Tasks;
using Bitbucket.Net.Models.Core.Users;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Lightweight comment reference used in task anchors and nested comment structures.
/// </summary>
public class CommentRef
{
    /// <summary>
    /// Gets or sets the additional properties bag.
    /// </summary>
    public Properties? Properties { get; init; }

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
    /// Gets or sets the user who authored the comment.
    /// </summary>
    public User? Author { get; init; }

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
    /// Gets or sets the nested reply comment references.
    /// </summary>
    public List<CommentRef>? Comments { get; init; }

    /// <summary>
    /// Gets or sets the tasks associated with this comment.
    /// </summary>
    public List<BitbucketTask>? Tasks { get; init; }

    /// <summary>
    /// Gets or sets the operations the current user is permitted to perform on this comment.
    /// </summary>
    public Permittedoperations? PermittedOperations { get; init; }
}