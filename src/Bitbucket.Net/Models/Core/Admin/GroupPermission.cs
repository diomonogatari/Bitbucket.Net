using Bitbucket.Net.Models.Core.Users;

namespace Bitbucket.Net.Models.Core.Admin;

public class GroupPermission
{
    public Named? Group { get; set; }
    public Permissions Permission { get; set; }

    public override string ToString() => $"{Permission} - {Group}";
}