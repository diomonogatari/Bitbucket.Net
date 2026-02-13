using Bitbucket.Net.Models.Core.Users;

namespace Bitbucket.Net.Models.Core.Admin;

public class UserPermission
{
    public User? User { get; init; }
    public Permissions Permission { get; init; }

    public override string ToString() => $"{Permission} - {User?.DisplayName}";
}