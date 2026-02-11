namespace Bitbucket.Net.Models.Core.Projects;

public class RefChange
{
    public Ref? Ref { get; init; }
    public string? RefId { get; init; }
    public string? FromHash { get; init; }
    public string? ToHash { get; init; }
    public string? Type { get; init; }
}