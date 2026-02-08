using Bitbucket.Net.Common.Converters;
using Bitbucket.Net.Models.Core.Users;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Represents a blocker comment (task) in Bitbucket Server 9.0+.
/// Blocker comments are comments with <see cref="CommentSeverity.Blocker"/> severity 
/// that must be resolved before the pull request can be merged.
/// </summary>
/// <remarks>
/// <para>
/// In Bitbucket Server 9.0+, the legacy <c>/pull-requests/{id}/tasks</c> endpoint was deprecated
/// and replaced with the blocker comments model. Tasks are now represented as comments with
/// <c>severity: 'BLOCKER'</c> and accessed via the <c>/blocker-comments</c> endpoint.
/// </para>
/// <para>
/// Use <see cref="BitbucketClient.GetPullRequestBlockerCommentsAsync"/> to retrieve blocker comments
/// from Bitbucket Server 9.0+.
/// </para>
/// </remarks>
public class BlockerComment
{
    /// <summary>
    /// The unique identifier of the blocker comment.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The version of the blocker comment, used for optimistic locking.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// The text content of the blocker comment.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// The user who created the blocker comment.
    /// </summary>
    public User? Author { get; set; }

    /// <summary>
    /// When the blocker comment was created.
    /// </summary>
    [JsonConverter(typeof(NullableUnixDateTimeOffsetConverter))]
    public DateTimeOffset? CreatedDate { get; set; }

    /// <summary>
    /// When the blocker comment was last updated.
    /// </summary>
    [JsonConverter(typeof(NullableUnixDateTimeOffsetConverter))]
    public DateTimeOffset? UpdatedDate { get; set; }

    /// <summary>
    /// The severity level of the comment. For blocker comments, this is always <see cref="CommentSeverity.Blocker"/>.
    /// </summary>
    [JsonConverter(typeof(CommentSeverityConverter))]
    public CommentSeverity Severity { get; set; } = CommentSeverity.Blocker;

    /// <summary>
    /// The state of the blocker comment.
    /// </summary>
    [JsonConverter(typeof(BlockerCommentStateConverter))]
    public BlockerCommentState State { get; set; } = BlockerCommentState.Open;

    /// <summary>
    /// The user who resolved the blocker comment, if resolved.
    /// </summary>
    public User? Resolver { get; set; }

    /// <summary>
    /// When the blocker comment was resolved.
    /// </summary>
    [JsonConverter(typeof(NullableUnixDateTimeOffsetConverter))]
    public DateTimeOffset? ResolvedDate { get; set; }

    /// <summary>
    /// The anchor point for the comment (file, line number, etc.).
    /// Null for general pull request-level blocker comments.
    /// </summary>
    public CommentAnchor? Anchor { get; set; }

    /// <summary>
    /// The parent comment this blocker is attached to, if any.
    /// </summary>
    public CommentRef? Parent { get; set; }

    /// <summary>
    /// The permitted operations the current user can perform on this blocker comment.
    /// </summary>
    public Permittedoperations? PermittedOperations { get; set; }

    /// <summary>
    /// Additional properties associated with the blocker comment.
    /// </summary>
    public Properties? Properties { get; set; }
}