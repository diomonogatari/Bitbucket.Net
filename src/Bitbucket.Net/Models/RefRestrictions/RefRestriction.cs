using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Core.Users;

namespace Bitbucket.Net.Models.RefRestrictions;

public class RefRestriction : RefRestrictionBase
{
    public int Id { get; init; }
    public HookScope? Scope { get; init; }
    public List<User>? Users { get; init; }
    public List<AccessKey>? AccessKeys { get; init; }
}