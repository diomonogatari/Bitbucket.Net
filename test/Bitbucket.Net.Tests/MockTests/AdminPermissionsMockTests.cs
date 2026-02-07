using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class AdminPermissionsMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetAdminGroupPermissionsNoneAsync_ReturnsGroups()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetAdminGroupPermissionsNone();
        var client = _fixture.CreateClient();

        var groups = await client.GetAdminGroupPermissionsNoneAsync();

        Assert.NotNull(groups);
        var groupList = groups.ToList();
        Assert.NotEmpty(groupList);
    }

    [Fact]
    public async Task GetAdminUserPermissionsNoneAsync_ReturnsUsers()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetAdminUserPermissionsNone();
        var client = _fixture.CreateClient();

        var users = await client.GetAdminUserPermissionsNoneAsync();

        Assert.NotNull(users);
        var userList = users.ToList();
        Assert.NotEmpty(userList);
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
    }

    [Fact]
    public async Task GetAdminLicenseAsync_ReturnsLicenseDetails()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetAdminLicense();
        var client = _fixture.CreateClient();

        var license = await client.GetAdminLicenseAsync();

        Assert.NotNull(license);
    }
}