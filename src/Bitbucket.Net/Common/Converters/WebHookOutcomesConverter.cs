using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Common.Converters;

/// <summary>
/// JSON converter for webhook outcome values.
/// </summary>
public class WebHookOutcomesConverter : JsonEnumConverter<WebHookOutcomes>
{
    /// <inheritdoc />
    protected override string ConvertToString(WebHookOutcomes value)
    {
        return BitbucketHelpers.WebHookOutcomeToString(value);
    }

    /// <inheritdoc />
    protected override WebHookOutcomes ConvertFromString(string s)
    {
        return BitbucketHelpers.StringToWebHookOutcome(s);
    }
}