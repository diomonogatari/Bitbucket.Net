using Bitbucket.Net.Common.Converters;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.RefSync;

public class RepositorySynchronizationStatus
{
    public bool Available { get; set; }
    public bool Enabled { get; set; }
    [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
    public DateTimeOffset LastSync { get; set; }
    public List<FullRef>? AheadRefs { get; set; }
    public List<FullRef>? DivergedRefs { get; set; }
    public List<FullRef>? OrphanedRefs { get; set; }
}