using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Common.Converters
{
    /// <summary>
    /// JSON converter for <see cref="CommentSeverity"/> enum values.
    /// </summary>
    public class CommentSeverityConverter : JsonEnumConverter<CommentSeverity>
    {
        protected override string ConvertToString(CommentSeverity value)
        {
            return BitbucketHelpers.CommentSeverityToString(value);
        }

        protected override CommentSeverity ConvertFromString(string s)
        {
            return BitbucketHelpers.StringToCommentSeverity(s);
        }
    }
}
