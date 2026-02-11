using Bitbucket.Net.Models.Core.Users;

namespace Bitbucket.Net.Models.Core.Admin;

public class UserPermission
{
    public User? User { get; set; }
    public Permissions Permission { get; set; }

    public override string ToString() => $"{Permission} - {User?.DisplayName}";
}