namespace Bitbucket.Net.Models.Core.Projects;

public class WebHookResult
{
    public string? Description { get; init; }
    public WebHookOutcomes Outcome { get; init; }
}