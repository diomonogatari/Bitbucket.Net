using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Models.RefSync;

public class FullRef : Ref
{
    public string? State { get; init; }
    public bool Tag { get; init; }
}