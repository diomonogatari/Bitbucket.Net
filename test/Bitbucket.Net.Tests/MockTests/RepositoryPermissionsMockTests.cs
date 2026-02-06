using System.Linq;
using System.Threading.Tasks;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    public class RepositoryPermissionsMockTests : IClassFixture<BitbucketMockFixture>
    {
        private readonly BitbucketMockFixture _fixture;

        public RepositoryPermissionsMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetProjectRepositoryUserPermissionsAsync_ReturnsUserPermissions()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetRepositoryUserPermissions(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var permissions = await client.GetProjectRepositoryUserPermissionsAsync(
                TestConstants.TestProjectKey, 
                TestConstants.TestRepositorySlug);

            Assert.NotNull(permissions);
            var permissionList = permissions.ToList();
            Assert.Single(permissionList);
            Assert.NotNull(permissionList[0].User);
            Assert.Equal("testuser", permissionList[0].User!.Name);
            Assert.Equal(Permissions.RepoAdmin, permissionList[0].Permission);
        }

        [Fact]
        public async Task UpdateProjectRepositoryUserPermissionsAsync_ReturnsTrue()
        {
            _fixture.Reset();
            _fixture.Server.SetupUpdateRepositoryUserPermissions(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var result = await client.UpdateProjectRepositoryUserPermissionsAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                Permissions.RepoAdmin,
                "testuser");

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteProjectRepositoryUserPermissionsAsync_ReturnsTrue()
        {
            _fixture.Reset();
            _fixture.Server.SetupDeleteRepositoryUserPermissions(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var result = await client.DeleteProjectRepositoryUserPermissionsAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                "testuser");

            Assert.True(result);
        }

        [Fact]
        public async Task GetProjectRepositoryUserPermissionsNoneAsync_ReturnsUsers()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetRepositoryUserPermissionsNone(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var users = await client.GetProjectRepositoryUserPermissionsNoneAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);

            Assert.NotNull(users);
            var userList = users.ToList();
            Assert.NotEmpty(userList);
        }

        [Fact]
        public async Task GetProjectRepositoryGroupPermissionsAsync_ReturnsGroupPermissions()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetRepositoryGroupPermissions(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var permissions = await client.GetProjectRepositoryGroupPermissionsAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);

            Assert.NotNull(permissions);
            var permissionList = permissions.ToList();
            Assert.Single(permissionList);
            Assert.NotNull(permissionList[0].Group);
            Assert.Equal("developers", permissionList[0].Group!.Name);
            Assert.Equal(Permissions.RepoWrite, permissionList[0].Permission);
        }

        [Fact]
        public async Task UpdateProjectRepositoryGroupPermissionsAsync_ReturnsTrue()
        {
            _fixture.Reset();
            _fixture.Server.SetupUpdateRepositoryGroupPermissions(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var result = await client.UpdateProjectRepositoryGroupPermissionsAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                Permissions.RepoWrite,
                "developers");

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteProjectRepositoryGroupPermissionsAsync_ReturnsTrue()
        {
            _fixture.Reset();
            _fixture.Server.SetupDeleteRepositoryGroupPermissions(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var result = await client.DeleteProjectRepositoryGroupPermissionsAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                "developers");

            Assert.True(result);
        }

        [Fact]
        public async Task GetProjectRepositoryGroupPermissionsNoneAsync_ReturnsDeletableGroupsOrUsers()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetRepositoryGroupPermissionsNone(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var entities = await client.GetProjectRepositoryGroupPermissionsNoneAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);

            Assert.NotNull(entities);
            var entityList = entities.ToList();
            Assert.NotEmpty(entityList);
        }
    }
}
