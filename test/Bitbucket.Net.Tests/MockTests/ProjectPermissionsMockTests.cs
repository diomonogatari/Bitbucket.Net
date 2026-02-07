using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Tests.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class ProjectPermissionsMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetProjectUserPermissionsAsync_ReturnsUserPermissions()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjectUserPermissions(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();

        var permissions = await client.GetProjectUserPermissionsAsync(TestConstants.TestProjectKey);

        Assert.NotNull(permissions);
        var permissionList = permissions.ToList();
        Assert.Single(permissionList);
        Assert.NotNull(permissionList[0].User);
        Assert.Equal("testuser", permissionList[0].User!.Name);
        Assert.Equal(Permissions.ProjectAdmin, permissionList[0].Permission);
    }

    [Fact]
    public async Task DeleteProjectUserPermissionsAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeleteProjectUserPermissions(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();

        var result = await client.DeleteProjectUserPermissionsAsync(TestConstants.TestProjectKey, "testuser");

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateProjectUserPermissionsAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateProjectUserPermissions(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();

        var result = await client.UpdateProjectUserPermissionsAsync(
            TestConstants.TestProjectKey,
            "testuser",
            Permissions.ProjectAdmin);

        Assert.True(result);
    }

    [Fact]
    public async Task GetProjectUserPermissionsNoneAsync_ReturnsLicensedUsers()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjectUserPermissionsNone(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();

        var users = await client.GetProjectUserPermissionsNoneAsync(TestConstants.TestProjectKey);

        Assert.NotNull(users);
        var userList = users.ToList();
        Assert.Single(userList);
    }

    [Fact]
    public async Task GetProjectGroupPermissionsAsync_ReturnsGroupPermissions()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjectGroupPermissions(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();

        var permissions = await client.GetProjectGroupPermissionsAsync(TestConstants.TestProjectKey);

        Assert.NotNull(permissions);
        var permissionList = permissions.ToList();
        Assert.Single(permissionList);
        Assert.NotNull(permissionList[0].Group);
        Assert.Equal("developers", permissionList[0].Group!.Name);
        Assert.Equal(Permissions.ProjectWrite, permissionList[0].Permission);
    }

    [Fact]
    public async Task DeleteProjectGroupPermissionsAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeleteProjectGroupPermissions(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();

        var result = await client.DeleteProjectGroupPermissionsAsync(TestConstants.TestProjectKey, "developers");

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateProjectGroupPermissionsAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateProjectGroupPermissions(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();

        var result = await client.UpdateProjectGroupPermissionsAsync(
            TestConstants.TestProjectKey,
            "developers",
            Permissions.ProjectWrite);

        Assert.True(result);
    }

    [Fact]
    public async Task GetProjectGroupPermissionsNoneAsync_ReturnsLicensedUsers()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjectGroupPermissionsNone(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();

        var users = await client.GetProjectGroupPermissionsNoneAsync(TestConstants.TestProjectKey);

        Assert.NotNull(users);
        var userList = users.ToList();
        Assert.Single(userList);
    }

    [Fact]
    public async Task IsProjectDefaultPermissionAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjectDefaultPermission(TestConstants.TestProjectKey, "PROJECT_READ");
        var client = _fixture.CreateClient();

        var result = await client.IsProjectDefaultPermissionAsync(TestConstants.TestProjectKey, Permissions.ProjectRead);

        Assert.True(result);
    }

    [Fact]
    public async Task GrantProjectPermissionToAllAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupSetProjectDefaultPermission(TestConstants.TestProjectKey, "PROJECT_READ");
        var client = _fixture.CreateClient();

        var result = await client.GrantProjectPermissionToAllAsync(TestConstants.TestProjectKey, Permissions.ProjectRead);

        Assert.True(result);
    }

    [Fact]
    public async Task RevokeProjectPermissionFromAllAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupSetProjectDefaultPermission(TestConstants.TestProjectKey, "PROJECT_READ");
        var client = _fixture.CreateClient();

        var result = await client.RevokeProjectPermissionFromAllAsync(TestConstants.TestProjectKey, Permissions.ProjectRead);

        Assert.True(result);
    }
}