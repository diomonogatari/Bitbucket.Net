namespace Bitbucket.Net.Models.Core.Projects;

public class HookDetails
{
    public string? Key { get; set; }
    public string? Name { get; set; }
    public HookTypes Type { get; set; }
    public string? Description { get; set; }
    public string? Version { get; set; }
    public object? ConfigFormKey { get; set; }
    public List<ScopeTypes>? ScopeTypes { get; set; }
}