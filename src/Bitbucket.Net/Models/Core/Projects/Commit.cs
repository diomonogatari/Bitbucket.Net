using Bitbucket.Net.Common.Converters;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Full Git commit. Extends <see cref="CommitParent"/> with author, committer, message, and parent references.
/// </summary>
public class Commit : CommitParent
{
    /// <summary>
    /// Gets or sets the commit author.
    /// </summary>
    public Author? Author { get; set; }

    /// <summary>
    /// Gets or sets the author timestamp (Unix epoch milliseconds from the Bitbucket API).
    /// </summary>
    [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
    public DateTimeOffset AuthorTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the committer (may differ from author in cherry-picks or patches).
    /// </summary>
    public Author? Committer { get; set; }

    /// <summary>
    /// Gets or sets the committer timestamp (Unix epoch milliseconds from the Bitbucket API).
    /// </summary>
    [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
    public DateTimeOffset CommitterTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the commit message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the parent commits.
    /// </summary>
    public List<CommitParent>? Parents { get; set; }

    /// <summary>
    /// Gets or sets the number of unique authors in the commit range.
    /// </summary>
    public int AuthorCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of commits in the range.
    /// </summary>
    public int TotalCount { get; set; }
}