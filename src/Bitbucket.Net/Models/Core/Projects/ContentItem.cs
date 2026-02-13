namespace Bitbucket.Net.Models.Core.Projects;

public class ContentItem
{
    public Path? Path { get; init; }
    public string? ContentId { get; init; }
    public string? Type { get; init; }
    public int Size { get; init; }
}