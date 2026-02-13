using Bitbucket.Net.Common.Converters;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Projects;

public class TimeWindow
{
    [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
    public DateTimeOffset Start { get; init; }
    public long Duration { get; init; }
}