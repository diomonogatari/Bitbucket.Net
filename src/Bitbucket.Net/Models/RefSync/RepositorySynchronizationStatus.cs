using Bitbucket.Net.Common.Converters;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.RefSync;

public class RepositorySynchronizationStatus
{
    public bool Available { get; init; }
    public bool Enabled { get; init; }
    [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
    public DateTimeOffset LastSync { get; init; }
    public List<FullRef>? AheadRefs { get; init; }
    public List<FullRef>? DivergedRefs { get; init; }
    public List<FullRef>? OrphanedRefs { get; init; }
}