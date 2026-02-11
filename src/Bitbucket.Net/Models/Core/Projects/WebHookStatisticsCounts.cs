namespace Bitbucket.Net.Models.Core.Projects;

public class WebHookStatisticsCounts
{
    public int Errors { get; init; }
    public int Failures { get; init; }
    public int Successes { get; init; }
    public TimeWindow? Window { get; init; }
}