using Bitbucket.Net.Common.Converters;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Core pull request data used when creating or updating a pull request.
/// </summary>
public class PullRequestInfo
{
    /// <summary>
    /// Gets or sets the pull request title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the pull request description (supports Markdown).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the pull request state (e.g. OPEN, MERGED, DECLINED).
    /// </summary>
    [JsonConverter(typeof(PullRequestStatesConverter))]
    public PullRequestStates State { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the pull request is open.
    /// </summary>
    public bool Open { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the pull request is closed.
    /// </summary>
    public bool Closed { get; set; }

    /// <summary>
    /// Gets or sets the source branch reference.
    /// </summary>
    public FromToRef? FromRef { get; set; }

    /// <summary>
    /// Gets or sets the target branch reference.
    /// </summary>
    public FromToRef? ToRef { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the pull request is locked from further changes.
    /// </summary>
    public bool Locked { get; set; }

    /// <summary>
    /// Gets or sets the list of assigned reviewers.
    /// </summary>
    public List<Reviewer>? Reviewers { get; set; }
}