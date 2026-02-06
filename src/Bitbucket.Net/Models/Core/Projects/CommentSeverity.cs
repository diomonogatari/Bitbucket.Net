namespace Bitbucket.Net.Models.Core.Projects
{
    /// <summary>
    /// Represents the severity of a comment in Bitbucket Server 9.0+.
    /// </summary>
    public enum CommentSeverity
    {
        /// <summary>
        /// A normal comment with no special behavior.
        /// </summary>
        Normal,

        /// <summary>
        /// A blocker comment (task) that must be resolved before merging.
        /// </summary>
        Blocker
    }
}
