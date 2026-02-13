namespace Bitbucket.Net.Models.Core.Projects;

public class Hook
{
    public HookDetails? Details { get; init; }
    public bool Enabled { get; init; }
    public bool Configured { get; init; }
    public HookScope? Scope { get; init; }
}