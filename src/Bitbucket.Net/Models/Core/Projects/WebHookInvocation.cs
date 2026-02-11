using Bitbucket.Net.Common.Converters;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Projects;

public class WebHookInvocation
{
    public int Id { get; init; }
    public string? Event { get; init; }
    public int Duration { get; init; }
    [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
    public DateTimeOffset Start { get; init; }
    [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
    public DateTimeOffset Finish { get; init; }
    public WebHookRequest? Request { get; init; }
    public WebHookResult? Result { get; init; }
}