using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Projects
{
    public class BranchMetaData
    {
        [JsonPropertyName("com.atlassian.bitbucket.server.bitbucket-branch:ahead-behind-metadata-provider")]
        public AheadBehindMetaData? AheadBehind { get; set; }

        [JsonPropertyName("com.atlassian.bitbucket.server.bitbucket-build:build-status-metadata")]
        public BuildStatusMetadata? BuildStatus { get; set; }

        [JsonPropertyName("com.atlassian.bitbucket.server.bitbucket-ref-metadata:outgoing-pull-request-metadata")]
        public PullRequestMetadata? OutgoingPullRequest { get; set; }
    }
}
