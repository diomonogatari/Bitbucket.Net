using Bitbucket.Net.Tests.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class ProfileMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetRecentReposAsync_ReturnsRepositories()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetRecentRepos();
        var client = _fixture.CreateClient();

        var result = await client.GetRecentReposAsync();

        Assert.NotNull(result);
        var repos = result.ToList();
        Assert.Single(repos);
        Assert.Equal("recent-repo", repos[0].Slug);
        Assert.Equal("Recent Repository", repos[0].Name);
    }
}