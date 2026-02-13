namespace Bitbucket.Net.Models.Builds;

public class BuildStats
{
    public int Successful { get; init; }
    public int InProgress { get; init; }
    public int Failed { get; init; }
}