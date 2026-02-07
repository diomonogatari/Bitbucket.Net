using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Core.Users;
using System.Collections.Generic;

namespace Bitbucket.Net.Models.RefRestrictions;

public class RefRestriction : RefRestrictionBase
{
    public int Id { get; set; }
    public HookScope? Scope { get; set; }
    public List<User>? Users { get; set; }
    public List<AccessKey>? AccessKeys { get; set; }
}