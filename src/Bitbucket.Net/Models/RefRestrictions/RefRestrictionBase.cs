using Bitbucket.Net.Models.DefaultReviewers;

namespace Bitbucket.Net.Models.RefRestrictions;

public abstract class RefRestrictionBase
{
    public RefRestrictionTypes Type { get; set; }
    public RefMatcher? Matcher { get; set; }
    public List<string>? Groups { get; set; }
}