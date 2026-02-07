#nullable enable

using Bitbucket.Net.Models.Core.Users;
using Bitbucket.Net.Tests.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class UsersMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetUsersAsync_ReturnsUsers()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetUsers();
        var client = _fixture.CreateClient();

        var users = await client.GetUsersAsync();

        var userList = users.ToList();
        Assert.NotEmpty(userList);
        Assert.Equal(2, userList.Count);
        Assert.Equal("admin", userList[0].Name);
        Assert.Equal("admin@example.com", userList[0].EmailAddress);
        Assert.Equal("developer", userList[1].Name);
    }

    [Fact]
    public async Task GetUserAsync_ReturnsUser()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetUser("admin");
        var client = _fixture.CreateClient();

        var user = await client.GetUserAsync("admin");

        Assert.NotNull(user);
        Assert.Equal("admin", user.Name);
        Assert.Equal("admin@example.com", user.EmailAddress);
        Assert.Equal("Administrator", user.DisplayName);
        Assert.True(user.Active);
    }

    [Fact]
    public async Task UpdateUserAsync_ReturnsUpdatedUser()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateUser();
        var client = _fixture.CreateClient();

        var user = await client.UpdateUserAsync(email: "newemail@example.com", displayName: "New Name");

        Assert.NotNull(user);
        Assert.Equal("admin", user.Name);
    }

    [Fact]
    public async Task UpdateUserCredentialsAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateUserCredentials();
        var client = _fixture.CreateClient();

        var passwordChange = new PasswordChange
        {
            OldPassword = "oldPassword",
            Password = "newPassword",
            PasswordConfirm = "newPassword"
        };

        var result = await client.UpdateUserCredentialsAsync(passwordChange);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteUserAvatarAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeleteUserAvatar("admin");
        var client = _fixture.CreateClient();

        var result = await client.DeleteUserAvatarAsync("admin");

        Assert.True(result);
    }

    [Fact]
    public async Task GetUserSettingsAsync_ReturnsSettings()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetUserSettings("admin");
        var client = _fixture.CreateClient();

        var settings = await client.GetUserSettingsAsync("admin");

        Assert.NotNull(settings);
        Assert.Equal("dark", settings["theme"]?.ToString());
    }

    [Fact]
    public async Task UpdateUserSettingsAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateUserSettings("admin");
        var client = _fixture.CreateClient();

        var settings = new Dictionary<string, object?> { ["theme"] = "dark" };
        var result = await client.UpdateUserSettingsAsync("admin", settings);

        Assert.True(result);
    }
}