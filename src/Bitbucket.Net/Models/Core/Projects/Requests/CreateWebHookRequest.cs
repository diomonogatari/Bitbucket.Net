namespace Bitbucket.Net.Models.Core.Projects.Requests;

/// <summary>
/// Request body for creating a new webhook on a repository.
/// </summary>
public sealed class CreateWebHookRequest
{
    /// <summary>
    /// The webhook name. Required.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The list of Bitbucket event types that trigger this webhook (e.g. "repo:refs_changed").
    /// </summary>
    public IReadOnlyList<string>? Events { get; init; }

    /// <summary>
    /// The URL that receives the webhook POST. Required.
    /// </summary>
    public required string Url { get; init; }

    /// <summary>
    /// Whether the webhook is active. Defaults to <c>true</c>.
    /// </summary>
    public bool Active { get; init; } = true;

    /// <summary>
    /// Additional configuration for the webhook (e.g. secret).
    /// </summary>
    public Dictionary<string, object?>? Configuration { get; init; }
}