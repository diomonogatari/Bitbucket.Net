using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Tests.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class DashboardMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetDashboardPullRequestsAsync_ReturnsPullRequests()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetDashboardPullRequests();
        var client = _fixture.CreateClient();

        var result = await client.GetDashboardPullRequestsAsync();

        Assert.NotNull(result);
        var pullRequests = result.ToList();
        Assert.Single(pullRequests);
        Assert.Equal("PR Title", pullRequests[0].Title);
        Assert.Equal(PullRequestStates.Open, pullRequests[0].State);
    }

    [Fact]
    public async Task GetDashboardPullRequestSuggestionsAsync_ReturnsSuggestions()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetDashboardPullRequestSuggestions();
        var client = _fixture.CreateClient();

        var result = await client.GetDashboardPullRequestSuggestionsAsync();

        Assert.NotNull(result);
        var suggestions = result.ToList();
        Assert.Single(suggestions);
        Assert.NotNull(suggestions[0].FromRef);
        Assert.Equal("feature/branch", suggestions[0].FromRef.DisplayId);
    }
}