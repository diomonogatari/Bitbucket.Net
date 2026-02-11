namespace Bitbucket.Net.Models.Core.Projects;

public class WebHookResult
{
    public string? Description { get; set; }
    public WebHookOutcomes Outcome { get; set; }
}