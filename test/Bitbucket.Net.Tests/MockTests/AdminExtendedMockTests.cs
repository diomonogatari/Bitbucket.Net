using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Tests.Infrastructure;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class AdminExtendedMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task CreateAdminUserAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupCreateAdminUser();
        var client = _fixture.CreateClient();

        var result = await client.CreateAdminUserAsync(
            "newuser",
            "password123",
            "New User",
            "newuser@example.com");

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAdminUserAsync_ReturnsUserInfo()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateAdminUser();
        var client = _fixture.CreateClient();

        var user = await client.UpdateAdminUserAsync(
            name: "updateduser",
            displayName: "Updated User",
            emailAddress: "updated@example.com");

        Assert.NotNull(user);
    }

    [Fact]
    public async Task AddAdminGroupUsersAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupAddAdminGroupUsers();
        var client = _fixture.CreateClient();

        var groupUsers = new GroupUsers
        {
            Group = "developers",
            Users = ["user1", "user2"]
        };

        var result = await client.AddAdminGroupUsersAsync(groupUsers);

        Assert.True(result);
    }

    [Fact]
    public async Task AddAdminUserGroupsAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupAddAdminUserGroups();
        var client = _fixture.CreateClient();

        var userGroups = new UserGroups
        {
            User = "testuser",
            Groups = ["developers", "reviewers"]
        };

        var result = await client.AddAdminUserGroupsAsync(userGroups);

        Assert.True(result);
    }

    [Fact]
    public async Task RemoveAdminUserFromGroupAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupRemoveAdminUserFromGroup();
        var client = _fixture.CreateClient();

        var result = await client.RemoveAdminUserFromGroupAsync("testuser", "old-group");

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAdminUserCaptchaAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeleteAdminUserCaptcha();
        var client = _fixture.CreateClient();

        var result = await client.DeleteAdminUserCaptcha("testuser");

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAdminUserCredentialsAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateAdminUserCredentials();
        var client = _fixture.CreateClient();

        var passwordChange = new PasswordChange
        {
            Name = "testuser",
            Password = "oldpass",
            PasswordConfirm = "newpass"
        };

        var result = await client.UpdateAdminUserCredentialsAsync(passwordChange);

        Assert.True(result);
    }

    [Fact]
    public async Task GetAdminMailServerAsync_ReturnsConfiguration()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetAdminMailServer();
        var client = _fixture.CreateClient();

        var config = await client.GetAdminMailServerAsync();

        Assert.NotNull(config);
        Assert.Equal("mail.example.com", config.HostName);
        Assert.Equal(587, config.Port);
    }

    [Fact]
    public async Task UpdateAdminMailServerAsync_ReturnsConfiguration()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateAdminMailServer();
        var client = _fixture.CreateClient();

        var config = new MailServerConfiguration
        {
            HostName = "newmail.example.com",
            Port = 465,
            Protocol = "SMTP"
        };

        var result = await client.UpdateAdminMailServerAsync(config);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteAdminMailServerAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeleteAdminMailServer();
        var client = _fixture.CreateClient();

        var result = await client.DeleteAdminMailServerAsync();

        Assert.True(result);
    }

    [Fact]
    public async Task GetAdminMailServerSenderAddressAsync_ReturnsAddress()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetAdminMailServerSenderAddress();
        var client = _fixture.CreateClient();

        var address = await client.GetAdminMailServerSenderAddressAsync();

        Assert.Equal("bitbucket@example.com", address);
    }

    [Fact]
    public async Task UpdateAdminMailServerSenderAddressAsync_ReturnsAddress()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateAdminMailServerSenderAddress();
        var client = _fixture.CreateClient();

        var address = await client.UpdateAdminMailServerSenderAddressAsync("new-sender@example.com");

        Assert.Equal("new-sender@example.com", address);
    }

    [Fact]
    public async Task DeleteAdminMailServerSenderAddressAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeleteAdminMailServerSenderAddress();
        var client = _fixture.CreateClient();

        var result = await client.DeleteAdminMailServerSenderAddressAsync();

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAdminGroupPermissionsAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateAdminGroupPermissions();
        var client = _fixture.CreateClient();

        var result = await client.UpdateAdminGroupPermissionsAsync(
            Permissions.Admin,
            "developers");

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAdminGroupPermissionsAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeleteAdminGroupPermissions();
        var client = _fixture.CreateClient();

        var result = await client.DeleteAdminGroupPermissionsAsync("developers");

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAdminUserPermissionsAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateAdminUserPermissions();
        var client = _fixture.CreateClient();

        var result = await client.UpdateAdminUserPermissionsAsync(
            Permissions.Admin,
            "testuser");

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAdminUserPermissionsAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeleteAdminUserPermissions();
        var client = _fixture.CreateClient();

        var result = await client.DeleteAdminUserPermissionsAsync("testuser");

        Assert.True(result);
    }

    [Fact]
    public async Task GetAdminPullRequestsMergeStrategiesAsync_ReturnsStrategies()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetAdminMergeStrategies("git");
        var client = _fixture.CreateClient();

        var strategies = await client.GetAdminPullRequestsMergeStrategiesAsync("git");

        Assert.NotNull(strategies);
        Assert.NotNull(strategies.DefaultStrategy);
    }

    [Fact]
    public async Task UpdateAdminPullRequestsMergeStrategiesAsync_ReturnsStrategies()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateAdminMergeStrategies("git");
        var client = _fixture.CreateClient();

        var strategies = new MergeStrategies
        {
            DefaultStrategy = new MergeStrategy { Id = "ff", Name = "Fast-forward", Enabled = true }
        };

        var result = await client.UpdateAdminPullRequestsMergeStrategiesAsync("git", strategies);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAdminLicenseAsync_ReturnsLicenseDetails()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateAdminLicense();
        var client = _fixture.CreateClient();

        var licenseInfo = new LicenseInfo { License = "NEW-LICENSE-KEY" };

        var result = await client.UpdateAdminLicenseAsync(licenseInfo);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task RenameAdminUserAsync_ReturnsUserInfo()
    {
        _fixture.Reset();
        _fixture.Server.SetupRenameAdminUser();
        var client = _fixture.CreateClient();

        var userRename = new UserRename
        {
            Name = "olduser",
            NewName = "newuser"
        };

        var result = await client.RenameAdminUserAsync(userRename);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAdminGroupMoreMembersAsync_ReturnsUsers()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetAdminGroupMoreMembers();
        var client = _fixture.CreateClient();

        var result = await client.GetAdminGroupMoreMembersAsync("test-group");

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetAdminGroupMoreNonMembersAsync_ReturnsUsers()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetAdminGroupMoreNonMembers();
        var client = _fixture.CreateClient();

        var result = await client.GetAdminGroupMoreNonMembersAsync("test-group");

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetAdminUserMoreMembersAsync_ReturnsGroupsOrUsers()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetAdminUserMoreMembers();
        var client = _fixture.CreateClient();

        var result = await client.GetAdminUserMoreMembersAsync("testuser");

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetAdminUserMoreNonMembersAsync_ReturnsGroupsOrUsers()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetAdminUserMoreNonMembers();
        var client = _fixture.CreateClient();

        var result = await client.GetAdminUserMoreNonMembersAsync("testuser");

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}