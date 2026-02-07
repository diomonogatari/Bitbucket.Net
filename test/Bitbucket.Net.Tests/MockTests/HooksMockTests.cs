using Bitbucket.Net.Tests.Infrastructure;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class HooksMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetProjectHooksAvatarAsync_ReturnsBytes()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjectHooksAvatar("com.example.myhook");
        var client = _fixture.CreateClient();

        var avatar = await client.GetProjectHooksAvatarAsync("com.example.myhook");

        Assert.NotNull(avatar);
        Assert.True(avatar.Length > 0);
    }

    [Fact]
    public async Task GetProjectHooksAvatarAsync_WithVersion_ReturnsBytes()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjectHooksAvatar("com.example.myhook");
        var client = _fixture.CreateClient();

        var avatar = await client.GetProjectHooksAvatarAsync("com.example.myhook", version: "1.0.0");

        Assert.NotNull(avatar);
        Assert.True(avatar.Length > 0);
    }
}