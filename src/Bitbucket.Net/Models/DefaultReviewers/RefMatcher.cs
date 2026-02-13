namespace Bitbucket.Net.Models.DefaultReviewers;

public class RefMatcher
{
    public bool Active { get; init; }
    public string? Id { get; init; }
    public string? DisplayId { get; init; }
    public DefaultReviewerPullRequestConditionType? Type { get; init; }
}