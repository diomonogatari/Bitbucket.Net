using System.Linq;
using System.Threading.Tasks;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    public class AdminMockTests : IClassFixture<BitbucketMockFixture>
    {
        private readonly BitbucketMockFixture _fixture;

        public AdminMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetAdminGroupsAsync_ReturnsGroups()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetAdminGroups();
            var client = _fixture.CreateClient();

            var groups = await client.GetAdminGroupsAsync();

            var groupList = groups.ToList();
            Assert.NotEmpty(groupList);
            Assert.Equal(2, groupList.Count);
            Assert.Equal("developers", groupList[0].Name);
            Assert.True(groupList[0].Deletable);
            Assert.Equal("administrators", groupList[1].Name);
            Assert.False(groupList[1].Deletable);
        }

        [Fact]
        public async Task CreateAdminGroupAsync_ReturnsCreatedGroup()
        {
            _fixture.Reset();
            _fixture.Server.SetupCreateAdminGroup("new-group");
            var client = _fixture.CreateClient();

            var group = await client.CreateAdminGroupAsync("new-group");

            Assert.NotNull(group);
            Assert.Equal("new-group", group.Name);
            Assert.True(group.Deletable);
        }

        [Fact]
        public async Task DeleteAdminGroupAsync_ReturnsDeletedGroup()
        {
            _fixture.Reset();
            _fixture.Server.SetupDeleteAdminGroup("old-group");
            var client = _fixture.CreateClient();

            var group = await client.DeleteAdminGroupAsync("old-group");

            Assert.NotNull(group);
            Assert.Equal("new-group", group.Name);
        }

        [Fact]
        public async Task GetAdminUsersAsync_ReturnsUsers()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetAdminUsers();
            var client = _fixture.CreateClient();

            var users = await client.GetAdminUsersAsync();

            var userList = users.ToList();
            Assert.NotEmpty(userList);
            Assert.Equal(2, userList.Count);
            Assert.Equal("admin", userList[0].Name);
            Assert.Equal("admin@example.com", userList[0].EmailAddress);
            Assert.Equal("jsmith", userList[1].Name);
        }

        [Fact]
        public async Task DeleteAdminUserAsync_ReturnsDeletedUser()
        {
            _fixture.Reset();
            _fixture.Server.SetupDeleteAdminUser("olduser");
            var client = _fixture.CreateClient();

            var user = await client.DeleteAdminUserAsync("olduser");

            Assert.NotNull(user);
            Assert.Equal("newuser", user.Name);
            Assert.Equal("newuser@example.com", user.EmailAddress);
        }

        [Fact]
        public async Task GetAdminClusterAsync_ReturnsClusterInfo()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetAdminCluster();
            var client = _fixture.CreateClient();

            var cluster = await client.GetAdminClusterAsync();

            Assert.NotNull(cluster);
            Assert.True(cluster.Running);
            Assert.NotNull(cluster.LocalNode);
            Assert.Equal("node-1", cluster.LocalNode.Id);
            Assert.Equal("bitbucket-node-1", cluster.LocalNode.Name);
            Assert.True(cluster.LocalNode.Local);
            Assert.Equal(2, cluster.Nodes.Count);
        }

        [Fact]
        public async Task GetAdminLicenseAsync_ReturnsLicenseDetails()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetAdminLicense();
            var client = _fixture.CreateClient();

            var license = await client.GetAdminLicenseAsync();

            Assert.NotNull(license);
            Assert.Equal("SERV-1234-5678", license.ServerId);
            Assert.Equal("SEN-12345", license.SupportEntitlementNumber);
            Assert.Equal(500, license.MaximumNumberOfUsers);
            Assert.False(license.UnlimitedNumberOfUsers);
            Assert.Equal(365, license.NumberOfDaysBeforeExpiry);
        }

        [Fact]
        public async Task GetAdminGroupPermissionsAsync_ReturnsPermissions()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetAdminGroupPermissions();
            var client = _fixture.CreateClient();

            var permissions = await client.GetAdminGroupPermissionsAsync();

            var permList = permissions.ToList();
            Assert.NotEmpty(permList);
            Assert.Equal(2, permList.Count);
            Assert.Equal("administrators", permList[0].Group.Name);
            Assert.Equal("developers", permList[1].Group.Name);
        }

        [Fact]
        public async Task GetAdminUserPermissionsAsync_ReturnsPermissions()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetAdminUserPermissions();
            var client = _fixture.CreateClient();

            var permissions = await client.GetAdminUserPermissionsAsync();

            var permList = permissions.ToList();
            Assert.NotEmpty(permList);
            Assert.Equal(2, permList.Count);
            Assert.Equal("admin", permList[0].User.Name);
            Assert.Equal("jsmith", permList[1].User.Name);
        }
    }
}
