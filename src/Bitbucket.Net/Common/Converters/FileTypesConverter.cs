using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Common.Converters;

/// <summary>
/// JSON converter for Bitbucket <see cref="FileTypes"/> values.
/// </summary>
public class FileTypesConverter : JsonEnumConverter<FileTypes>
{
    /// <inheritdoc />
    protected override string ConvertToString(FileTypes value)
    {
        return BitbucketHelpers.FileTypeToString(value);
    }

    /// <inheritdoc />
    protected override FileTypes ConvertFromString(string s)
    {
        return BitbucketHelpers.StringToFileType(s);
    }
}