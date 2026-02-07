using Bitbucket.Net.Common.Converters;
using Bitbucket.Net.Models.Core.Admin;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.PersonalAccessTokens;

public class AccessTokenCreate
{
    public string? Name { get; set; }
    [JsonConverter(typeof(PermissionsListConverter))]
    public List<Permissions>? Permissions { get; set; }
}