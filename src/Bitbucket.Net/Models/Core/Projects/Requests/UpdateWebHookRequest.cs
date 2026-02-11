namespace Bitbucket.Net.Models.Core.Projects.Requests;

/// <summary>
/// Request body for updating an existing webhook on a repository.
/// </summary>
public sealed class UpdateWebHookRequest
{
    /// <summary>
    /// The webhook name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// The list of Bitbucket event types that trigger this webhook.
    /// </summary>
    public IReadOnlyList<string>? Events { get; init; }

    /// <summary>
    /// The URL that receives the webhook POST.
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// Whether the webhook is active.
    /// </summary>
    public bool? Active { get; init; }

    /// <summary>
    /// Additional configuration for the webhook (e.g. secret).
    /// </summary>
    public Dictionary<string, object?>? Configuration { get; init; }
}