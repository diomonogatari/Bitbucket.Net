using Bitbucket.Net.Common.Models;

namespace Bitbucket.Net.Models.Core.Projects;

public class BrowseItem
{
    public Path? Path { get; init; }
    public string? Revision { get; init; }
    public PagedResults<ContentItem>? Children { get; init; }
}