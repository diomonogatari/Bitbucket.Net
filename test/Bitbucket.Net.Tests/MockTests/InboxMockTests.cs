using Bitbucket.Net.Tests.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class InboxMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetInboxPullRequestsAsync_ReturnsPullRequests()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetInboxPullRequests();
        var client = _fixture.CreateClient();

        var result = await client.GetInboxPullRequestsAsync();

        Assert.NotNull(result);
        var pullRequests = result.ToList();
        Assert.Single(pullRequests);
        Assert.Equal("Inbox PR Title", pullRequests[0].Title);
    }

    [Fact]
    public async Task GetInboxPullRequestsCountAsync_ReturnsCount()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetInboxPullRequestsCount();
        var client = _fixture.CreateClient();

        var result = await client.GetInboxPullRequestsCountAsync();

        Assert.Equal(5, result);
    }
}