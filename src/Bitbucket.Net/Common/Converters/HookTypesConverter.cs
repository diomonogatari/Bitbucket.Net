using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Common.Converters;

/// <summary>
/// JSON converter for Bitbucket hook type values.
/// </summary>
public class HookTypesConverter : JsonEnumConverter<HookTypes>
{
    /// <inheritdoc />
    protected override string ConvertToString(HookTypes value)
    {
        return BitbucketHelpers.HookTypeToString(value);
    }

    /// <inheritdoc />
    protected override HookTypes ConvertFromString(string s)
    {
        return BitbucketHelpers.StringToHookType(s);
    }
}