#nullable enable

using Flurl.Http;
using Xunit;

namespace Bitbucket.Net.Tests.UnitTests;

public class BitbucketClientDisposeTests
{
    private const string TestUrl = "https://bitbucket.example.com";

    [Fact]
    public void Dispose_BasicAuth_DoesNotThrow()
    {
        var client = new BitbucketClient(TestUrl, "user", "pass");
        client.Dispose();
    }

    [Fact]
    public void Dispose_TokenAuth_DoesNotThrow()
    {
        var client = new BitbucketClient(TestUrl, () => "token");
        client.Dispose();
    }

    [Fact]
    public void Dispose_HttpClient_DisposesOwnedWrapper()
    {
        var httpClient = new HttpClient();
        var client = new BitbucketClient(httpClient, TestUrl);

        // Should not throw — disposes the internal FlurlClient wrapper
        client.Dispose();
    }

    [Fact]
    public void Dispose_FlurlClient_DoesNotDisposeExternalClient()
    {
        using var flurlClient = new FlurlClient(TestUrl);
        var client = new BitbucketClient(flurlClient);

        // The BitbucketClient should NOT dispose the external FlurlClient
        client.Dispose();

        // The FlurlClient should still be usable after BitbucketClient disposal
        Assert.NotNull(flurlClient.BaseUrl);
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        var httpClient = new HttpClient();
        var client = new BitbucketClient(httpClient, TestUrl);

        client.Dispose();
        client.Dispose(); // Should not throw on second call
    }

    [Fact]
    public async Task MethodAfterDispose_ThrowsObjectDisposedException()
    {
        var client = new BitbucketClient(TestUrl, "user", "pass");
        client.Dispose();

        await Assert.ThrowsAsync<ObjectDisposedException>(
            () => client.GetProjectsAsync());
    }
}