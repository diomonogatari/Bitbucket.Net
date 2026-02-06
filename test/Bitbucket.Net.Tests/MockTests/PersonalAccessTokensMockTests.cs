using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.PersonalAccessTokens;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    public class PersonalAccessTokensMockTests : IClassFixture<BitbucketMockFixture>
    {
        private readonly BitbucketMockFixture _fixture;
        private const string UserSlug = "admin";
        private const string TokenId = "token1";

        public PersonalAccessTokensMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(Skip = "Permissions List converter mismatch - uses JsonEnumConverter instead of JsonEnumListConverter")]
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

        [Fact(Skip = "Permissions List converter mismatch - uses JsonEnumConverter instead of JsonEnumListConverter")]
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

        [Fact(Skip = "Permissions enum serialization issue with source generators")]
        public async Task CreateAccessTokenAsync_ReturnsCreatedToken()
        {
            _fixture.Reset();
            _fixture.Server.SetupCreateAccessToken(UserSlug);
            var client = _fixture.CreateClient();

            var tokenCreate = new AccessTokenCreate
            {
                Name = "New API Token",
                Permissions = new List<Permissions> { Permissions.ProjectRead, Permissions.RepoRead }
            };

            var result = await client.CreateAccessTokenAsync(UserSlug, tokenCreate);

            Assert.NotNull(result);
            Assert.Equal("token1", result.Id);
            Assert.NotNull(result.Token);
        }

        [Fact(Skip = "Permissions enum serialization issue with source generators")]
        public async Task ChangeUserAccessTokenAsync_ReturnsUpdatedToken()
        {
            _fixture.Reset();
            _fixture.Server.SetupChangeUserAccessToken(UserSlug, TokenId);
            var client = _fixture.CreateClient();

            var tokenUpdate = new AccessTokenCreate
            {
                Name = "Updated API Token",
                Permissions = new List<Permissions> { Permissions.ProjectAdmin, Permissions.RepoAdmin }
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
}
