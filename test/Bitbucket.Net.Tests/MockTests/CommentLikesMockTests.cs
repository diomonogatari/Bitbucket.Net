using Bitbucket.Net.Tests.Infrastructure;
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
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
    public async Task GetPullRequestCommentLikesAsync_WithAvatarSize_SendsAvatarSizeParam()
    {
        _fixture.Reset();
        // Stub matches only when the request carries avatarSize=48, proving the new param is sent
        // (parity with GetCommitCommentLikesAsync).
        _fixture.Server
            .Given(Request.Create()
                .WithPath($"/rest/comment-likes/1.0/projects/{ProjectKey}/repos/{RepoSlug}/pull-requests/{PullRequestId}/comments/{CommentId}/likes")
                .WithParam("avatarSize", "48")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"size\":1,\"limit\":25,\"isLastPage\":true,\"start\":0,\"values\":[{\"name\":\"jsmith\"}]}"));
        var client = _fixture.CreateClient();

        var result = await client.GetPullRequestCommentLikesAsync(ProjectKey, RepoSlug, PullRequestId, CommentId, avatarSize: 48);

        Assert.Single(result);
        Assert.Equal("jsmith", result[0].Name);
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