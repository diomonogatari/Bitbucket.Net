using Bitbucket.Net.Models.Core.Admin;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Projects;

public class PullRequestSettings
{
    public MergeStrategies? MergeConfig { get; set; }
    public bool RequiredAllApprovers { get; set; }
    public bool RequiredAllTasksComplete { get; set; }
    [JsonPropertyName("com.atlassian.bitbucket.server.bitbucket-bundled-hooks:requiredApprovers")]
    public MergeHookRequiredApprovers? ComatlassianbitbucketserverbundledhooksrequiredApprovers { get; set; }
    public int RequiredApprovers { get; set; }
    [JsonPropertyName("com.atlassian.bitbucket.server.bitbucket-build:requiredBuilds")]
    public MergeCheckRequiredBuilds? ComatlassianbitbucketserverbitbucketbuildrequiredBuilds { get; set; }
    public int RequiredSuccessfulBuilds { get; set; }
}