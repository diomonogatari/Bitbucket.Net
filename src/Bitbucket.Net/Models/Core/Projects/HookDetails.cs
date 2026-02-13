namespace Bitbucket.Net.Models.Core.Projects;

public class HookDetails
{
    public string? Key { get; init; }
    public string? Name { get; init; }
    public HookTypes Type { get; init; }
    public string? Description { get; init; }
    public string? Version { get; init; }
    public object? ConfigFormKey { get; init; }
    public List<ScopeTypes>? ScopeTypes { get; init; }
}