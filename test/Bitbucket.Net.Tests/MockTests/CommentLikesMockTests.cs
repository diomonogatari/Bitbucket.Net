using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class CommentLikesMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;
    private const string ProjectKey = "PROJ";
    private const string RepoSlug = "repo";
    private const string CommitId = "abc123";
    private const string CommentId = "100";
    private const string PullRequestId = "1";

    [Fact]
    public async Task GetCommitCommentLikesAsync_ReturnsUsers()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetCommitCommentLikes(ProjectKey, RepoSlug, CommitId, CommentId);
        var client = _fixture.CreateClient();

        var result = await client.GetCommitCommentLikesAsync(ProjectKey, RepoSlug, CommitId, CommentId);

        Assert.NotNull(result);
        var users = result.ToList();
        Assert.Equal(2, users.Count);
        Assert.Equal("jsmith", users[0].Name);
        Assert.Equal("jdoe", users[1].Name);
    }

    [Fact]
    public async Task LikeCommitCommentAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupLikeCommitComment(ProjectKey, RepoSlug, CommitId, CommentId);
        var client = _fixture.CreateClient();

        var result = await client.LikeCommitCommentAsync(ProjectKey, RepoSlug, CommitId, CommentId);

        Assert.True(result);
    }

    [Fact]
    public async Task UnlikeCommitCommentAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupUnlikeCommitComment(ProjectKey, RepoSlug, CommitId, CommentId);
        var client = _fixture.CreateClient();

        var result = await client.UnlikeCommitCommentAsync(ProjectKey, RepoSlug, CommitId, CommentId);

        Assert.True(result);
    }

    [Fact]
    public async Task GetPullRequestCommentLikesAsync_ReturnsUsers()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequestCommentLikes(ProjectKey, RepoSlug, PullRequestId, CommentId);
        var client = _fixture.CreateClient();

        var result = await client.GetPullRequestCommentLikesAsync(ProjectKey, RepoSlug, PullRequestId, CommentId);

        Assert.NotNull(result);
        var users = result.ToList();
        Assert.Equal(2, users.Count);
    }

    [Fact]
    public async Task LikePullRequestCommentAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupLikePullRequestComment(ProjectKey, RepoSlug, PullRequestId, CommentId);
        var client = _fixture.CreateClient();

        var result = await client.LikePullRequestCommentAsync(ProjectKey, RepoSlug, PullRequestId, CommentId);

        Assert.True(result);
    }

    [Fact]
    public async Task UnlikePullRequestCommentAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupUnlikePullRequestComment(ProjectKey, RepoSlug, PullRequestId, CommentId);
        var client = _fixture.CreateClient();

        var result = await client.UnlikePullRequestCommentAsync(ProjectKey, RepoSlug, PullRequestId, CommentId);

        Assert.True(result);
    }
}