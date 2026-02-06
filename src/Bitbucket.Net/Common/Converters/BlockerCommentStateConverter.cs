using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Common.Converters
{
    /// <summary>
    /// JSON converter for <see cref="BlockerCommentState"/> enum values.
    /// </summary>
    public class BlockerCommentStateConverter : JsonEnumConverter<BlockerCommentState>
    {
        protected override string ConvertToString(BlockerCommentState value)
        {
            return BitbucketHelpers.BlockerCommentStateToString(value);
        }

        protected override BlockerCommentState ConvertFromString(string s)
        {
            return BitbucketHelpers.StringToBlockerCommentState(s);
        }
    }
}
