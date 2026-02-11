using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.RefRestrictions;

namespace Bitbucket.Net.Models.Ssh;

public abstract class KeyBase
{
    public Key? Key { get; set; }
    public Permissions Permission { get; set; }
}