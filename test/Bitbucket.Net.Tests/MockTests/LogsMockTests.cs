using Bitbucket.Net.Models.Core.Logs;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class LogsMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetLogLevelAsync_ReturnsLogLevel()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetLogLevel("com.atlassian.bitbucket");
        var client = _fixture.CreateClient();

        var logLevel = await client.GetLogLevelAsync("com.atlassian.bitbucket");

        Assert.Equal(LogLevels.Debug, logLevel);
    }

    [Fact]
    public async Task SetLogLevelAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupSetLogLevel("com.atlassian.bitbucket", "INFO");
        var client = _fixture.CreateClient();

        var result = await client.SetLogLevelAsync("com.atlassian.bitbucket", LogLevels.Info);

        Assert.True(result);
    }

    [Fact]
    public async Task GetRootLogLevelAsync_ReturnsLogLevel()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetRootLogLevel();
        var client = _fixture.CreateClient();

        var logLevel = await client.GetRootLogLevelAsync();

        Assert.Equal(LogLevels.Debug, logLevel);
    }

    [Fact]
    public async Task SetRootLogLevelAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupSetRootLogLevel("WARN");
        var client = _fixture.CreateClient();

        var result = await client.SetRootLogLevelAsync(LogLevels.Warn);

        Assert.True(result);
    }
}