namespace Bitbucket.Net.Models.RefSync;

public class Synchronize
{
    public string? RefId { get; set; }
    public SynchronizeActions Action { get; set; }
    public SynchronizeContext? Context { get; set; }
}