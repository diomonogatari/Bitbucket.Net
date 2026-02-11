namespace Bitbucket.Net.Models.Core.Projects;

public class WebHookTestResponse
{
    public int Status { get; init; }
    public List<string>? Headers { get; init; }
    public string? Body { get; init; }
}