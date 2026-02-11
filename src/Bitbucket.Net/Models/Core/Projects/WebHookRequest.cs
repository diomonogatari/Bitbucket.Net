namespace Bitbucket.Net.Models.Core.Projects;

public class WebHookRequest
{
    public string? Url { get; init; }
    public string? Method { get; init; }
}