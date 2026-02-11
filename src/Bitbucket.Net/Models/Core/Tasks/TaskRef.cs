using Bitbucket.Net.Common.Converters;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Core.Users;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Tasks;

/// <summary>
/// Abstract base class for Bitbucket tasks with identity, text, author, and creation date.
/// </summary>
public abstract class TaskRef
{
    /// <summary>
    /// Gets or sets the additional properties bag.
    /// </summary>
    public Properties? Properties { get; init; }

    /// <summary>
    /// Gets or sets the server-assigned task identifier.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets or sets the task description text.
    /// </summary>
    public string? Text { get; init; }

    /// <summary>
    /// Gets or sets the user who created the task.
    /// </summary>
    public User? Author { get; init; }

    /// <summary>
    /// Gets or sets the date and time when the task was created.
    /// </summary>
    [JsonConverter(typeof(NullableUnixDateTimeOffsetConverter))]
    public DateTimeOffset? CreatedDate { get; init; }

    /// <summary>
    /// Gets or sets the operations the current user is permitted to perform on this task.
    /// </summary>
    public Permittedoperations? PermittedOperations { get; init; }
}