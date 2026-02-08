using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Common.Converters;

/// <summary>
/// JSON converter for Bitbucket participant status values.
/// </summary>
public class ParticipantStatusConverter : JsonEnumConverter<ParticipantStatus>
{
    /// <inheritdoc />
    protected override string ConvertToString(ParticipantStatus value)
    {
        return BitbucketHelpers.ParticipantStatusToString(value);
    }

    /// <inheritdoc />
    protected override ParticipantStatus ConvertFromString(string s)
    {
        return BitbucketHelpers.StringToParticipantStatus(s);
    }
}