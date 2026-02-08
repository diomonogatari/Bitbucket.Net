using Bitbucket.Net.Models.Core.Admin;

namespace Bitbucket.Net.Common.Converters;

/// <summary>
/// JSON converter for Bitbucket permission values.
/// </summary>
public class PermissionsConverter : JsonEnumConverter<Permissions>
{
    /// <inheritdoc />
    protected override string ConvertToString(Permissions value)
    {
        return BitbucketHelpers.PermissionToString(value);
    }

    /// <inheritdoc />
    protected override Permissions ConvertFromString(string s)
    {
        return BitbucketHelpers.StringToPermission(s);
    }
}

/// <summary>
/// JSON converter for lists of Bitbucket permission values.
/// </summary>
public class PermissionsListConverter : JsonEnumListConverter<Permissions>
{
    /// <inheritdoc />
    protected override string ConvertToString(Permissions value)
    {
        return BitbucketHelpers.PermissionToString(value);
    }

    /// <inheritdoc />
    protected override Permissions ConvertFromString(string s)
    {
        return BitbucketHelpers.StringToPermission(s);
    }
}