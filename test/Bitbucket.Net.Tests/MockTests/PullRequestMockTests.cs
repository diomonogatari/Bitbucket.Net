using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Tests.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

/// <summary>
/// Unit tests for pull request-related operations using WireMock.
/// </summary>
public class PullRequestMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetPullRequestsAsync_ReturnsPullRequests()
    {
        // Arrange
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequests(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        // Act
        var pullRequests = await client.GetPullRequestsAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);

        // Assert
        Assert.NotNull(pullRequests);
        var prList = pullRequests.ToList();
        Assert.Single(prList);
        var pr = prList[0];
        Assert.Equal(TestConstants.TestPullRequestId, pr.Id);
        Assert.Equal(TestConstants.TestPullRequestTitle, pr.Title);
        Assert.Equal(PullRequestStates.Open, pr.State);
    }

    [Fact]
    public async Task GetPullRequestAsync_WithValidId_ReturnsPullRequest()
    {
        // Arrange
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequest(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        // Act
        var pullRequest = await client.GetPullRequestAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);

        // Assert
        Assert.NotNull(pullRequest);
        Assert.Equal(TestConstants.TestPullRequestId, pullRequest.Id);
        Assert.Equal(TestConstants.TestPullRequestTitle, pullRequest.Title);
        Assert.NotNull(pullRequest.FromRef);
        Assert.NotNull(pullRequest.ToRef);
    }

    [Fact]
    public async Task GetPullRequestCommentsAsync_ReturnsComments()
    {
        // Arrange
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequestComments(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        // Act
        var comments = await client.GetPullRequestCommentsAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId,
            "/");

        // Assert
        Assert.NotNull(comments);
        var commentList = comments.ToList();
        Assert.Single(commentList);
        var comment = commentList[0];
        Assert.Equal(TestConstants.TestCommentId, comment.Id);
    }
}