using Bitbucket.Net.Models.Builds.Requests;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class BuildMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetBuildStatsForCommitAsync_ReturnsStats()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetBuildStatsForCommit(TestConstants.TestCommitId);
        var client = _fixture.CreateClient();

        var stats = await client.GetBuildStatsForCommitAsync(TestConstants.TestCommitId);

        Assert.NotNull(stats);
        Assert.Equal(3, stats.Successful);
        Assert.Equal(1, stats.InProgress);
        Assert.Equal(0, stats.Failed);
    }

    [Fact]
    public async Task GetBuildStatusForCommitAsync_ReturnsStatuses()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetBuildStatusForCommit(TestConstants.TestCommitId);
        var client = _fixture.CreateClient();

        var statuses = await client.GetBuildStatusForCommitAsync(TestConstants.TestCommitId);

        Assert.NotNull(statuses);
        var statusList = statuses.ToList();
        Assert.Equal(2, statusList.Count);
        Assert.Equal("build-123", statusList[0].Key);
        Assert.Equal("SUCCESSFUL", statusList[0].State);
    }

    [Fact]
    public async Task AssociateBuildStatusWithCommitAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupAssociateBuildStatus(TestConstants.TestCommitId);
        var client = _fixture.CreateClient();

        var request = new AssociateBuildStatusRequest
        {
            Key = "build-125",
            State = "SUCCESSFUL",
            Name = "Test Build",
            Description = "Build completed",
            Url = "https://build-server/builds/125"
        };

        var result = await client.AssociateBuildStatusWithCommitAsync(
            TestConstants.TestCommitId,
            request);

        Assert.True(result);
    }
}