using Bitbucket.Net.Common.Converters;
using Bitbucket.Net.Models.Core.Projects;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Tasks;

/// <summary>
/// A task anchor that represents the comment a task is attached to. Extends <see cref="TaskRef"/> with versioning and nested content.
/// </summary>
public class TaskAnchor : TaskRef
{
    /// <summary>
    /// Gets or sets the version number for optimistic locking on updates.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the anchor was last updated.
    /// </summary>
    [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
    public DateTimeOffset? UpdatedDate { get; set; }

    /// <summary>
    /// Gets or sets the nested comment references on this anchor.
    /// </summary>
    public List<CommentRef>? Comments { get; set; }

    /// <summary>
    /// Gets or sets the tasks associated with this anchor.
    /// </summary>
    public List<BitbucketTask>? Tasks { get; set; }
}