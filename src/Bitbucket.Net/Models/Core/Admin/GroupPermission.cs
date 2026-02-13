using Bitbucket.Net.Models.Core.Users;

namespace Bitbucket.Net.Models.Core.Admin;

public class GroupPermission
{
    public Named? Group { get; init; }
    public Permissions Permission { get; init; }

    public override string ToString() => $"{Permission} - {Group}";
}