using Bitbucket.Net.Common.Converters;
using Bitbucket.Net.Models.Core.Users;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// A participant in a Bitbucket pull request (author, reviewer, or watcher).
/// </summary>
public class Participant
{
    /// <summary>
    /// Gets or sets the participant's user details.
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Gets or sets the participant's role (e.g. AUTHOR, REVIEWER, PARTICIPANT).
    /// </summary>
    [JsonConverter(typeof(RolesConverter))]
    public Roles Role { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the participant has approved the pull request.
    /// </summary>
    public bool Approved { get; set; }

    /// <summary>
    /// Gets or sets the participant's review status (e.g. APPROVED, UNAPPROVED, NEEDS_WORK).
    /// </summary>
    [JsonConverter(typeof(ParticipantStatusConverter))]
    public ParticipantStatus Status { get; set; }

    /// <summary>
    /// Returns the participant's display name when available.
    /// </summary>
    public override string ToString() => User?.DisplayName ?? "Unknown";
}