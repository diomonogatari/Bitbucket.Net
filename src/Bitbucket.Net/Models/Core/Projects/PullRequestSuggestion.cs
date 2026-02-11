using Bitbucket.Net.Common.Converters;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Projects;

public class PullRequestSuggestion
{
    [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
    public DateTimeOffset ChangeTime { get; init; }
    public RefChange? RefChange { get; init; }
    public Repository? Repository { get; init; }
    public Ref? FromRef { get; init; }
    public Ref? ToRef { get; init; }
}