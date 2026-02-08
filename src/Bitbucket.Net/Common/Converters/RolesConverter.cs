using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Common.Converters;

/// <summary>
/// JSON converter for pull request role values.
/// </summary>
public class RolesConverter : JsonEnumConverter<Roles>
{
    /// <inheritdoc />
    protected override string ConvertToString(Roles value)
    {
        return BitbucketHelpers.RoleToString(value);
    }

    /// <inheritdoc />
    protected override Roles ConvertFromString(string s)
    {
        return BitbucketHelpers.StringToRole(s);
    }
}