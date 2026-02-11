namespace Bitbucket.Net.Models.Core.Projects;

public class WebHookStatisticsSummary
{
    public WebHookInvocation? LastSuccess { get; init; }
    public WebHookInvocation? LastFailure { get; init; }
    public WebHookInvocation? LastError { get; init; }
    public int Counts { get; init; }
}