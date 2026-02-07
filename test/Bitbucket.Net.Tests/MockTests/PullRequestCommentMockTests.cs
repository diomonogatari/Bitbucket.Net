using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class PullRequestCommentMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task CreatePullRequestCommentAsync_ReturnsComment()
    {
        _fixture.Reset();
        _fixture.Server.SetupCreatePullRequestComment(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var result = await client.CreatePullRequestCommentAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId,
            "This is a new comment");

        Assert.NotNull(result);
        Assert.Equal(101, result.Id);
        Assert.Equal("This is a new comment", result.Text);
    }

    [Fact]
    public async Task GetPullRequestCommentAsync_ReturnsComment()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequestComment(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId,
            101);
        var client = _fixture.CreateClient();

        var result = await client.GetPullRequestCommentAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId,
            101);

        Assert.NotNull(result);
        Assert.Equal(101, result.Id);
    }

    [Fact]
    public async Task UpdatePullRequestCommentAsync_ReturnsUpdatedComment()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdatePullRequestComment(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId,
            101);
        var client = _fixture.CreateClient();

        var result = await client.UpdatePullRequestCommentAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId,
            101,
            0,
            "Updated comment text");

        Assert.NotNull(result);
        Assert.Equal(101, result.Id);
    }

    [Fact]
    public async Task DeletePullRequestCommentAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeletePullRequestComment(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId,
            101);
        var client = _fixture.CreateClient();

        var result = await client.DeletePullRequestCommentAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId,
            101,
            0);

        Assert.True(result);
    }
}