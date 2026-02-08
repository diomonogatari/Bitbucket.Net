using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Common.Converters;

/// <summary>
/// JSON converter for Bitbucket permission scope types.
/// </summary>
public class ScopeTypesConverter : JsonEnumConverter<ScopeTypes>
{
    /// <inheritdoc />
    protected override string ConvertToString(ScopeTypes value)
    {
        return BitbucketHelpers.ScopeTypeToString(value);
    }

    /// <inheritdoc />
    protected override ScopeTypes ConvertFromString(string s)
    {
        return BitbucketHelpers.StringToScopeType(s);
    }
}