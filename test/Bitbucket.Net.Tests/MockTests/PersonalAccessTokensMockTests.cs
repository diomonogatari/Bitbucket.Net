using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.PersonalAccessTokens;
using Bitbucket.Net.Tests.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class PersonalAccessTokensMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;
    private const string UserSlug = "admin";
    private const string TokenId = "token1";

    [Fact]
    public async Task GetUserAccessTokensAsync_ReturnsTokens()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetUserAccessTokens(UserSlug);
        var client = _fixture.CreateClient();

        var result = await client.GetUserAccessTokensAsync(UserSlug);

        Assert.NotNull(result);
        var tokens = result.ToList();
        Assert.Equal(2, tokens.Count);
        Assert.Equal("token1", tokens[0].Id);
        Assert.Equal("API Token", tokens[0].Name);
    }

    [Fact]
    public async Task GetUserAccessTokenAsync_ReturnsToken()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetUserAccessToken(UserSlug, TokenId);
        var client = _fixture.CreateClient();

        var result = await client.GetUserAccessTokenAsync(UserSlug, TokenId);

        Assert.NotNull(result);
        Assert.Equal("token1", result.Id);
        Assert.Equal("API Token", result.Name);
    }

    [Fact]
    public async Task CreateAccessTokenAsync_ReturnsCreatedToken()
    {
        _fixture.Reset();
        _fixture.Server.SetupCreateAccessToken(UserSlug);
        var client = _fixture.CreateClient();

        var tokenCreate = new AccessTokenCreate
        {
            Name = "New API Token",
            Permissions = [Permissions.ProjectRead, Permissions.RepoRead]
        };

        var result = await client.CreateAccessTokenAsync(UserSlug, tokenCreate);

        Assert.NotNull(result);
        Assert.Equal("token1", result.Id);
        Assert.NotNull(result.Token);
    }

    [Fact]
    public async Task ChangeUserAccessTokenAsync_ReturnsUpdatedToken()
    {
        _fixture.Reset();
        _fixture.Server.SetupChangeUserAccessToken(UserSlug, TokenId);
        var client = _fixture.CreateClient();

        var tokenUpdate = new AccessTokenCreate
        {
            Name = "Updated API Token",
            Permissions = [Permissions.ProjectAdmin, Permissions.RepoAdmin]
        };

        var result = await client.ChangeUserAccessTokenAsync(UserSlug, TokenId, tokenUpdate);

        Assert.NotNull(result);
        Assert.Equal("token1", result.Id);
    }

    [Fact]
    public async Task DeleteUserAccessTokenAsync_ReturnsSuccess()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeleteUserAccessToken(UserSlug, TokenId);
        var client = _fixture.CreateClient();

        var result = await client.DeleteUserAccessTokenAsync(UserSlug, TokenId);

        Assert.True(result);
    }
}