using Bitbucket.Net.Common.Converters;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.RefRestrictions;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Ssh
{
    public abstract class KeyBase
    {
        public Key Key { get; set; }
        [JsonConverter(typeof(PermissionsConverter))]
        public Permissions Permission { get; set; }
    }
}