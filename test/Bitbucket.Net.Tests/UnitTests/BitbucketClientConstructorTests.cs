#nullable enable

using System;
using System.Net.Http;
using Flurl.Http;
using Xunit;

namespace Bitbucket.Net.Tests.UnitTests;

public class BitbucketClientConstructorTests
{
    private const string TestUrl = "https://bitbucket.example.com";

    #region Basic Auth Constructor Tests

    [Fact]
    public void Constructor_BasicAuth_CreatesClient()
    {
        var client = new BitbucketClient(TestUrl, "user", "pass");
        Assert.NotNull(client);
    }

    [Theory]
    [InlineData("https://bitbucket.example.com")]
    [InlineData("http://localhost:7990")]
    [InlineData("https://bitbucket.example.com/")]
    public void Constructor_BasicAuth_AcceptsVariousUrls(string url)
    {
        var client = new BitbucketClient(url, "user", "pass");
        Assert.NotNull(client);
    }

    #endregion

    #region Token Auth Constructor Tests

    [Fact]
    public void Constructor_TokenAuth_CreatesClient()
    {
        var client = new BitbucketClient(TestUrl, () => "test-token");
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_TokenAuth_AcceptsTokenFunction()
    {
        int callCount = 0;
        var client = new BitbucketClient(TestUrl, () =>
        {
            callCount++;
            return "test-token";
        });

        Assert.NotNull(client);
    }

    #endregion

    #region HttpClient Constructor Tests

    [Fact]
    public void Constructor_HttpClient_CreatesClient()
    {
        using var httpClient = new HttpClient();
        var client = new BitbucketClient(httpClient, TestUrl);
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_HttpClient_WithToken_CreatesClient()
    {
        using var httpClient = new HttpClient();
        var client = new BitbucketClient(httpClient, TestUrl, () => "test-token");
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_HttpClient_Null_ThrowsArgumentNullException()
    {
        HttpClient? httpClient = null;
        Assert.Throws<ArgumentNullException>(() =>
            new BitbucketClient(httpClient!, TestUrl));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_HttpClient_EmptyBaseUrl_ThrowsArgumentNullException(string? url)
    {
        using var httpClient = new HttpClient();
        Assert.Throws<ArgumentNullException>(() =>
            new BitbucketClient(httpClient, url!));
    }

    #endregion

    #region FlurlClient Constructor Tests

    [Fact]
    public void Constructor_FlurlClient_CreatesClient()
    {
        using var httpClient = new HttpClient();
        var flurlClient = new FlurlClient(httpClient, TestUrl);
        var client = new BitbucketClient(flurlClient);
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_FlurlClient_WithToken_CreatesClient()
    {
        using var httpClient = new HttpClient();
        var flurlClient = new FlurlClient(httpClient, TestUrl);
        var client = new BitbucketClient(flurlClient, () => "test-token");
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_FlurlClient_Null_ThrowsArgumentNullException()
    {
        IFlurlClient? flurlClient = null;
        Assert.Throws<ArgumentNullException>(() =>
            new BitbucketClient(flurlClient!));
    }

    [Fact]
    public void Constructor_FlurlClient_NoBaseUrl_ThrowsArgumentException()
    {
        using var httpClient = new HttpClient();
        var flurlClient = new FlurlClient(httpClient);
        Assert.Throws<ArgumentException>(() =>
            new BitbucketClient(flurlClient));
    }

    #endregion
}
