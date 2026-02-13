namespace Bitbucket.Net.Models.Core.Admin;

public class Cluster
{
    public Node? LocalNode { get; init; }
    public List<Node>? Nodes { get; init; }
    public bool Running { get; init; }
}