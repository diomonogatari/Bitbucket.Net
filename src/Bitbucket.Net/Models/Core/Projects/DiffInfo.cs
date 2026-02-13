namespace Bitbucket.Net.Models.Core.Projects;

public abstract class DiffInfo
{
    /// <summary>
    /// Indicates whether the diff was truncated by the server.
    /// Note: Bitbucket Server 9.0+ returns boolean; older versions may return string.
    /// </summary>
    public bool Truncated { get; init; }

    public string? ContextLines { get; init; }
    public string? FromHash { get; init; }
    public string? ToHash { get; init; }
    public string? WhiteSpace { get; init; }
}