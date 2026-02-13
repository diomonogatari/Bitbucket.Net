using Bitbucket.Net.Common.Converters;
using Bitbucket.Net.Models.Core.Users;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.PersonalAccessTokens;

public class AccessToken : AccessTokenCreate
{
    public string? Id { get; init; }
    [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
    public DateTimeOffset CreatedDate { get; init; }
    [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
    public DateTimeOffset LastAuthenticated { get; init; }
    public User? User { get; init; }
    [JsonConverter(typeof(UnixDateTimeOffsetConverter))]
    public DateTimeOffset ExpiryDate { get; init; }
}