namespace Bitbucket.Net.Models.Core.Projects;

public class RepositoryOrigin
{
    public string? Slug { get; init; }
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? ScmId { get; init; }
    public string? State { get; init; }
    public string? StatusMessage { get; init; }
    public bool Forkable { get; init; }
    public Project? Project { get; init; }
    public bool Public { get; init; }
    public Links? Links { get; init; }
}