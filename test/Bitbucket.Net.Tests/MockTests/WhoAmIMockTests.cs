using Bitbucket.Net.Tests.Infrastructure;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class WhoAmIMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetWhoAmIAsync_ReturnsUsername()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetWhoAmI("jsmith");
        var client = _fixture.CreateClient();

        var username = await client.GetWhoAmIAsync();

        Assert.Equal("jsmith", username);
    }

    [Fact]
    public async Task GetWhoAmIAsync_WithWhitespace_ReturnsTrimmedUsername()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetWhoAmI("  jsmith  ");
        var client = _fixture.CreateClient();

        var username = await client.GetWhoAmIAsync();

        Assert.Equal("jsmith", username);
    }
}