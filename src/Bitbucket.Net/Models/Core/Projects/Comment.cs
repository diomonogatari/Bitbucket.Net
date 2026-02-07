using Bitbucket.Net.Common.Converters;
using Bitbucket.Net.Models.Core.Tasks;
using Bitbucket.Net.Models.Core.Users;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Projects;

public class Comment : PullRequestInfo
{
    public int Id { get; set; }
    public int Version { get; set; }
    public string? Text { get; set; }

    /// <summary>
    /// Bitbucket Server comment state.
    /// Common values: OPEN, PENDING, RESOLVED.
    /// </summary>
    /// <remarks>
    /// This intentionally hides <see cref="PullRequestInfo.State"/>. Although inheriting from <see cref="PullRequestInfo"/>
    /// is not ideal for a comment model, using the same CLR member name avoids System.Text.Json property-name collisions.
    /// </remarks>
    public new string? State { get; set; }

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
    [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
    public DateTimeOffset? CreatedDate { get; set; }
    [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
    public DateTimeOffset? UpdatedDate { get; set; }
    public User? Author { get; set; }
    public List<Comment>? Comments { get; set; }
    public List<BitbucketTask>? Tasks { get; set; }
    public List<Participant>? Participants { get; set; }
    public Links? Links { get; set; }
}