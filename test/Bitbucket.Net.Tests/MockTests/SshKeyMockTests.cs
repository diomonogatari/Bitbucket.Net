using System.Linq;
using System.Threading.Tasks;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    public class SshKeyMockTests : IClassFixture<BitbucketMockFixture>
    {
        private readonly BitbucketMockFixture _fixture;

        public SshKeyMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetProjectKeysAsync_ByKeyId_ReturnsKeys()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetProjectKeysByKeyId(1);
            var client = _fixture.CreateClient();

            var keys = await client.GetProjectKeysAsync(keyId: 1);

            Assert.NotNull(keys);
            var keyList = keys.ToList();
            Assert.Equal(2, keyList.Count);
        }

        [Fact]
        public async Task GetProjectKeysAsync_ByProjectKey_ReturnsKeys()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetProjectKeysByProject(TestConstants.TestProjectKey);
            var client = _fixture.CreateClient();

            var keys = await client.GetProjectKeysAsync(projectKey: TestConstants.TestProjectKey);

            Assert.NotNull(keys);
        }

        [Fact]
        public async Task CreateProjectKeyAsync_CreatesKey()
        {
            _fixture.Reset();
            _fixture.Server.SetupCreateProjectKey(TestConstants.TestProjectKey);
            var client = _fixture.CreateClient();

            var key = await client.CreateProjectKeyAsync(
                TestConstants.TestProjectKey,
                "ssh-rsa AAAAB3...",
                Permissions.RepoRead);

            Assert.NotNull(key);
        }

        [Fact]
        public async Task GetProjectKeyAsync_ReturnsKey()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetProjectKey(TestConstants.TestProjectKey, 1);
            var client = _fixture.CreateClient();

            var key = await client.GetProjectKeyAsync(TestConstants.TestProjectKey, 1);

            Assert.NotNull(key);
        }

        [Fact]
        public async Task DeleteProjectKeyAsync_DeletesKey()
        {
            _fixture.Reset();
            _fixture.Server.SetupDeleteProjectKey(TestConstants.TestProjectKey, 1);
            var client = _fixture.CreateClient();

            var result = await client.DeleteProjectKeyAsync(TestConstants.TestProjectKey, 1);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateProjectKeyPermissionAsync_UpdatesPermission()
        {
            _fixture.Reset();
            _fixture.Server.SetupUpdateProjectKeyPermission(TestConstants.TestProjectKey, 1);
            var client = _fixture.CreateClient();

            var key = await client.UpdateProjectKeyPermissionAsync(
                TestConstants.TestProjectKey,
                1,
                Permissions.RepoWrite);

            Assert.NotNull(key);
        }

        [Fact]
        public async Task GetRepoKeysAsync_ByKeyId_ReturnsKeys()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetRepoKeysByKeyId(1);
            var client = _fixture.CreateClient();

            var keys = await client.GetRepoKeysAsync(keyId: 1);

            Assert.NotNull(keys);
        }

        [Fact]
        public async Task GetRepoKeysAsync_ByProjectAndRepo_ReturnsKeys()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetRepoKeysByProjectAndRepo(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var keys = await client.GetRepoKeysAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);

            Assert.NotNull(keys);
        }

        [Fact]
        public async Task CreateRepoKeyAsync_CreatesKey()
        {
            _fixture.Reset();
            _fixture.Server.SetupCreateRepoKey(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var key = await client.CreateRepoKeyAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                "ssh-rsa AAAAB3...",
                Permissions.RepoRead);

            Assert.NotNull(key);
        }

        [Fact]
        public async Task GetRepoKeyAsync_ReturnsKey()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetRepoKey(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                1);
            var client = _fixture.CreateClient();

            var key = await client.GetRepoKeyAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                1);

            Assert.NotNull(key);
        }

        [Fact]
        public async Task DeleteRepoKeyAsync_DeletesKey()
        {
            _fixture.Reset();
            _fixture.Server.SetupDeleteRepoKey(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                1);
            var client = _fixture.CreateClient();

            var result = await client.DeleteRepoKeyAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                1);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateRepoKeyPermissionAsync_UpdatesPermission()
        {
            _fixture.Reset();
            _fixture.Server.SetupUpdateRepoKeyPermission(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                1);
            var client = _fixture.CreateClient();

            var key = await client.UpdateRepoKeyPermissionAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                1,
                Permissions.RepoWrite);

            Assert.NotNull(key);
        }

        [Fact]
        public async Task GetUserKeysAsync_ReturnsKeys()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetUserKeys();
            var client = _fixture.CreateClient();

            var keys = await client.GetUserKeysAsync();

            Assert.NotNull(keys);
        }

        [Fact]
        public async Task CreateUserKeyAsync_CreatesKey()
        {
            _fixture.Reset();
            _fixture.Server.SetupCreateUserKey();
            var client = _fixture.CreateClient();

            var key = await client.CreateUserKeyAsync("ssh-rsa AAAAB3...");

            Assert.NotNull(key);
        }

        [Fact]
        public async Task DeleteUserKeysAsync_DeletesKeys()
        {
            _fixture.Reset();
            _fixture.Server.SetupDeleteUserKeys();
            var client = _fixture.CreateClient();

            var result = await client.DeleteUserKeysAsync();

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUserKeyAsync_DeletesKey()
        {
            _fixture.Reset();
            _fixture.Server.SetupDeleteUserKey(1);
            var client = _fixture.CreateClient();

            var result = await client.DeleteUserKeyAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteProjectsReposKeysAsync_DeletesKeys()
        {
            _fixture.Reset();
            _fixture.Server.SetupDeleteProjectsReposKeys(1);
            var client = _fixture.CreateClient();

            var result = await client.DeleteProjectsReposKeysAsync(1, "PROJECT:TEST", "REPO:test-repo");

            Assert.True(result);
        }

        [Fact]
        public async Task GetSshSettingsAsync_ReturnsSettings()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetSshSettings();
            var client = _fixture.CreateClient();

            var settings = await client.GetSshSettingsAsync();

            Assert.NotNull(settings);
        }
    }
}
