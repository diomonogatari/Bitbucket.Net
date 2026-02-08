using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Common.Converters;

/// <summary>
/// JSON converter for Bitbucket pull request states.
/// </summary>
public class PullRequestStatesConverter : JsonEnumConverter<PullRequestStates>
{
    /// <inheritdoc />
    protected override string ConvertToString(PullRequestStates value)
    {
        return BitbucketHelpers.PullRequestStateToString(value);
    }

    /// <inheritdoc />
    protected override PullRequestStates ConvertFromString(string s)
    {
        return BitbucketHelpers.StringToPullRequestState(s);
    }
}