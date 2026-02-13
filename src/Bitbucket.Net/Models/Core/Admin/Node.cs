namespace Bitbucket.Net.Models.Core.Admin;

public class Node
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public Address? Address { get; init; }
    public bool Local { get; init; }
}