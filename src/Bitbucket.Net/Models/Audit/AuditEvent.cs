using Bitbucket.Net.Models.Core.Users;

namespace Bitbucket.Net.Models.Audit;

public class AuditEvent
{
    public string? Action { get; init; }
    public long Timestamp { get; init; }
    public string? Details { get; init; }
    public User? User { get; init; }
}