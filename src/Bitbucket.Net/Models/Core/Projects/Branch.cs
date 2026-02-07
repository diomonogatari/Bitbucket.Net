using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Projects;

public class Branch : BranchBase
{
    private BranchMetaData? _branchMetadata;
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public string? LatestCommit { get; set; }
    public string? LatestChangeset { get; set; }
    public bool IsDefault { get; set; }

    public BranchMetaData? BranchMetadata
    {
        get
        {
            if (_branchMetadata != null)
            {
                return _branchMetadata;
            }

            if (Metadata == null || Metadata.Value.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            _branchMetadata = new BranchMetaData();

            foreach (var metadata in Metadata.Value.EnumerateArray())
            {
                if (!metadata.TryGetProperty("Name", out var nameElement) && !metadata.TryGetProperty("name", out nameElement))
                {
                    continue;
                }

                var name = nameElement.GetString();
                if (!metadata.TryGetProperty("Value", out var valueElement) && !metadata.TryGetProperty("value", out valueElement))
                {
                    continue;
                }

                var valueJson = valueElement.GetRawText();

                if (string.Equals(name, "com.atlassian.bitbucket.server.bitbucket-branch:ahead-behind-metadata-provider", System.StringComparison.Ordinal))
                {
                    _branchMetadata.AheadBehind = JsonSerializer.Deserialize<AheadBehindMetaData>(valueJson, s_jsonOptions);
                }
                else if (string.Equals(name, "com.atlassian.bitbucket.server.bitbucket-build:build-status-metadata", System.StringComparison.Ordinal))
                {
                    _branchMetadata.BuildStatus = JsonSerializer.Deserialize<BuildStatusMetadata>(valueJson, s_jsonOptions);
                }
                else if (string.Equals(name, "com.atlassian.bitbucket.server.bitbucket-ref-metadata:outgoing-pull-request-metadata", System.StringComparison.Ordinal))
                {
                    _branchMetadata.OutgoingPullRequest = JsonSerializer.Deserialize<PullRequestMetadata>(valueJson, s_jsonOptions);
                }
            }

            return _branchMetadata;
        }
    }

    [JsonPropertyName("metadata")]
    public JsonElement? Metadata { get; set; }

    public override string ToString() => DisplayId;
}