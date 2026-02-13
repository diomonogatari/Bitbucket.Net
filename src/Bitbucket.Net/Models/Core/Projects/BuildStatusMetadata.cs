namespace Bitbucket.Net.Models.Core.Projects;

public class BuildStatusMetadata
{
    public int Successful { get; init; }
    public int InProgress { get; init; }
    public int Failed { get; init; }
}