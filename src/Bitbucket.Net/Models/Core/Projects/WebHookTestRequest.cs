namespace Bitbucket.Net.Models.Core.Projects;

public class WebHookTestRequest : WebHookRequest
{
    public string? Body { get; init; }
    public List<string>? Headers { get; init; }
}