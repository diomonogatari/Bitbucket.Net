using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class DiffStreamingMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private const string ApiBasePath = "/rest/api/1.0";
    private readonly BitbucketMockFixture _fixture = fixture;

    #region GetCommitDiffStreamAsync

    [Fact]
    public async Task GetCommitDiffStreamAsync_SingleDiff_YieldsDiffEntry()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetCommitDiff(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestCommitId);
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetCommitDiffStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestCommitId));

        Assert.Single(results);
        Assert.NotNull(results[0].Source);
        Assert.NotNull(results[0].Destination);
        Assert.NotNull(results[0].Hunks);
        Assert.NotEmpty(results[0].Hunks!);
    }

    [Fact]
    public async Task GetCommitDiffStreamAsync_MultipleDiffs_YieldsAllEntries()
    {
        _fixture.Reset();
        _fixture.Server.SetupDiffEndpoint(
            $"{ApiBasePath}/projects/{TestConstants.TestProjectKey}/repos/{TestConstants.TestRepositorySlug}/commits/{TestConstants.TestCommitId}/diff",
            "diff-multiple.json");
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetCommitDiffStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestCommitId));

        Assert.Equal(3, results.Count);
    }

    [Fact]
    public async Task GetCommitDiffStreamAsync_EmptyDiffs_YieldsZeroItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupDiffEndpoint(
            $"{ApiBasePath}/projects/{TestConstants.TestProjectKey}/repos/{TestConstants.TestRepositorySlug}/commits/{TestConstants.TestCommitId}/diff",
            "diff-empty.json");
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetCommitDiffStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestCommitId));

        Assert.Empty(results);
    }

    #endregion

    #region GetRepositoryDiffStreamAsync

    [Fact]
    public async Task GetRepositoryDiffStreamAsync_SingleDiff_YieldsDiffEntry()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetRepositoryDiff(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetRepositoryDiffStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, until: "HEAD"));

        Assert.Single(results);
        Assert.NotNull(results[0].Hunks);
    }

    [Fact]
    public async Task GetRepositoryDiffStreamAsync_MultipleDiffs_YieldsAllEntries()
    {
        _fixture.Reset();
        _fixture.Server.SetupDiffEndpoint(
            $"{ApiBasePath}/projects/{TestConstants.TestProjectKey}/repos/{TestConstants.TestRepositorySlug}/diff",
            "diff-multiple.json");
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetRepositoryDiffStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, until: "HEAD"));

        Assert.Equal(3, results.Count);
    }

    [Fact]
    public async Task GetRepositoryDiffStreamAsync_EmptyDiffs_YieldsZeroItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupDiffEndpoint(
            $"{ApiBasePath}/projects/{TestConstants.TestProjectKey}/repos/{TestConstants.TestRepositorySlug}/diff",
            "diff-empty.json");
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetRepositoryDiffStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, until: "HEAD"));

        Assert.Empty(results);
    }

    #endregion

    #region GetRepositoryCompareDiffStreamAsync

    [Fact]
    public async Task GetRepositoryCompareDiffStreamAsync_SingleDiff_YieldsDiffEntry()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetCompareDiff(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetRepositoryCompareDiffStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, from: "main", to: "feature"));

        Assert.Single(results);
    }

    [Fact]
    public async Task GetRepositoryCompareDiffStreamAsync_EmptyDiffs_YieldsZeroItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupDiffEndpoint(
            $"{ApiBasePath}/projects/{TestConstants.TestProjectKey}/repos/{TestConstants.TestRepositorySlug}/compare/diff",
            "diff-empty.json");
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetRepositoryCompareDiffStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, from: "main", to: "feature"));

        Assert.Empty(results);
    }

    #endregion

    #region GetPullRequestDiffStreamAsync

    [Fact]
    public async Task GetPullRequestDiffStreamAsync_SingleDiff_YieldsDiffEntry()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequestDiff(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetPullRequestDiffStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId));

        Assert.Single(results);
        Assert.NotNull(results[0].Source);
        Assert.NotNull(results[0].Destination);
    }

    [Fact]
    public async Task GetPullRequestDiffStreamAsync_MultipleDiffs_YieldsAllEntries()
    {
        _fixture.Reset();
        _fixture.Server.SetupDiffEndpoint(
            $"{ApiBasePath}/projects/{TestConstants.TestProjectKey}/repos/{TestConstants.TestRepositorySlug}/pull-requests/{TestConstants.TestPullRequestId}/diff",
            "diff-multiple.json");
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetPullRequestDiffStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId));

        Assert.Equal(3, results.Count);
    }

    [Fact]
    public async Task GetPullRequestDiffStreamAsync_EmptyDiffs_YieldsZeroItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupDiffEndpoint(
            $"{ApiBasePath}/projects/{TestConstants.TestProjectKey}/repos/{TestConstants.TestRepositorySlug}/pull-requests/{TestConstants.TestPullRequestId}/diff",
            "diff-empty.json");
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetPullRequestDiffStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId));

        Assert.Empty(results);
    }

    #endregion

    #region Diff Content Validation

    [Fact]
    public async Task GetPullRequestDiffStreamAsync_DiffContainsExpectedSegmentsAndHunks()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequestDiff(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetPullRequestDiffStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId));

        var diff = Assert.Single(results);
        var hunk = Assert.Single(diff.Hunks!);
        var segment = Assert.Single(hunk.Segments!);
        Assert.Equal("ADDED", segment.Type);
        Assert.NotEmpty(segment.Lines!);
    }

    #endregion

    private static async Task<List<T>> CollectAsync<T>(IAsyncEnumerable<T> source)
    {
        var list = new List<T>();
        await foreach (var item in source)
        {
            list.Add(item);
        }
        return list;
    }
}