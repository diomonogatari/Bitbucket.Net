namespace Bitbucket.Net.Models.Core.Projects
{
    public abstract class DiffInfo
    {
        /// <summary>
        /// Indicates whether the diff was truncated by the server.
        /// Note: Bitbucket Server 9.0+ returns boolean; older versions may return string.
        /// </summary>
        public bool Truncated { get; set; }

        public string ContextLines { get; set; }
        public string FromHash { get; set; }
        public string ToHash { get; set; }
        public string WhiteSpace { get; set; }
    }
}