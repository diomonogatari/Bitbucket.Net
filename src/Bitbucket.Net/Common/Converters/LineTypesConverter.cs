using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Common.Converters;

/// <summary>
/// JSON converter for Bitbucket line classification values.
/// </summary>
public class LineTypesConverter : JsonEnumConverter<LineTypes>
{
    /// <inheritdoc />
    protected override string ConvertToString(LineTypes value)
    {
        return BitbucketHelpers.LineTypeToString(value);
    }

    /// <inheritdoc />
    protected override LineTypes ConvertFromString(string s)
    {
        return BitbucketHelpers.StringToLineType(s);
    }
}