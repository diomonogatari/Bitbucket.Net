using System.Threading.Tasks;
using WireMock.Server;
using Xunit;

namespace Bitbucket.Net.Tests.Infrastructure;

public sealed class BitbucketMockFixture : IAsyncLifetime
{
    public WireMockServer Server { get; private set; } = null!;
    public string BaseUrl => Server.Url!;

    public Task InitializeAsync()
    {
        Server = WireMockServer.Start();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Server?.Dispose();
        return Task.CompletedTask;
    }

    public BitbucketClient CreateClient()
    {
        return new BitbucketClient(BaseUrl, TestConstants.TestUsername, TestConstants.TestPassword);
    }

    public void Reset()
    {
        Server.Reset();
    }
}