namespace Bitbucket.Net.Models.Ssh;

public class SshSettings
{
    public Accesskeys? AccessKeys { get; init; }
    public string? BaseUrl { get; init; }
    public bool Enabled { get; init; }
    public Fingerprint? Fingerprint { get; init; }
    public int Port { get; init; }
}