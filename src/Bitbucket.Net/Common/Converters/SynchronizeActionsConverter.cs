using Bitbucket.Net.Models.RefSync;

namespace Bitbucket.Net.Common.Converters;

/// <summary>
/// JSON converter for repository synchronization actions.
/// </summary>
public class SynchronizeActionsConverter : JsonEnumConverter<SynchronizeActions>
{
    /// <inheritdoc />
    protected override string ConvertToString(SynchronizeActions value)
    {
        return BitbucketHelpers.SynchronizeActionToString(value);
    }

    /// <inheritdoc />
    protected override SynchronizeActions ConvertFromString(string s)
    {
        return BitbucketHelpers.StringToSynchronizeAction(s);
    }
}