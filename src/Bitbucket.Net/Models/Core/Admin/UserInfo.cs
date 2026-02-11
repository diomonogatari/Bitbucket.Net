using Bitbucket.Net.Models.Core.Users;

namespace Bitbucket.Net.Models.Core.Admin;

public class UserInfo : User
{
    public string? DirectoryName { get; init; }
    public bool Deletable { get; init; }
    public long LastAuthenticationTimestamp { get; init; }
    public bool MutableDetails { get; init; }
    public bool MutableGroups { get; init; }
}