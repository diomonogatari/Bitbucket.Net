using Bitbucket.Net.Models.RefRestrictions;

namespace Bitbucket.Net.Common.Converters;

/// <summary>
/// JSON converter for repository ref restriction types.
/// </summary>
public class RefRestrictionTypesConverter : JsonEnumConverter<RefRestrictionTypes>
{
    /// <inheritdoc />
    protected override string ConvertToString(RefRestrictionTypes value)
    {
        return BitbucketHelpers.RefRestrictionTypeToString(value);
    }

    /// <inheritdoc />
    protected override RefRestrictionTypes ConvertFromString(string s)
    {
        return BitbucketHelpers.StringToRefRestrictionType(s);
    }
}