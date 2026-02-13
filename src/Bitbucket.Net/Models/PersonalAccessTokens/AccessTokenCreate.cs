using Bitbucket.Net.Models.Core.Admin;

namespace Bitbucket.Net.Models.PersonalAccessTokens;

public class AccessTokenCreate
{
    public string? Name { get; set; }
    public List<Permissions>? Permissions { get; set; }
}