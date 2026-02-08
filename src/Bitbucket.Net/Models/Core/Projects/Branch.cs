using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Full Bitbucket branch. Extends <see cref="BranchBase"/> with commit info, default status, and parsed metadata.
/// </summary>
public class Branch : BranchBase
{
    private BranchMetaData? _branchMetadata;
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    /// <summary>
    /// Gets or sets the SHA of the latest commit on this branch.
    /// </summary>
    public string? LatestCommit { get; set; }

    /// <summary>
    /// Gets or sets the changeset identifier of the latest change on this branch.
    /// </summary>
    public string? LatestChangeset { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is the repository's default branch.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Gets parsed branch metadata (ahead/behind counts, build status, outgoing pull requests) from the raw <see cref="Metadata"/> JSON.
    /// </summary>
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

                if (string.Equals(name, "com.atlassian.bitbucket.server.bitbucket-branch:ahead-behind-metadata-provider", StringComparison.Ordinal))
                {
                    _branchMetadata.AheadBehind = JsonSerializer.Deserialize<AheadBehindMetaData>(valueJson, s_jsonOptions);
                }
                else if (string.Equals(name, "com.atlassian.bitbucket.server.bitbucket-build:build-status-metadata", StringComparison.Ordinal))
                {
                    _branchMetadata.BuildStatus = JsonSerializer.Deserialize<BuildStatusMetadata>(valueJson, s_jsonOptions);
                }
                else if (string.Equals(name, "com.atlassian.bitbucket.server.bitbucket-ref-metadata:outgoing-pull-request-metadata", StringComparison.Ordinal))
                {
                    _branchMetadata.OutgoingPullRequest = JsonSerializer.Deserialize<PullRequestMetadata>(valueJson, s_jsonOptions);
                }
            }

            return _branchMetadata;
        }
    }

    /// <summary>
    /// Gets or sets the raw JSON metadata array returned by Bitbucket Server for this branch.
    /// </summary>
    [JsonPropertyName("metadata")]
    public JsonElement? Metadata { get; set; }

    public override string ToString() => DisplayId ?? string.Empty;
}