#nullable enable

using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class UserMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetUsersAsync_ReturnsUsers()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetUsers();
        var client = _fixture.CreateClient();

        var users = await client.GetUsersAsync();

        Assert.NotNull(users);
        var userList = users.ToList();
        Assert.Equal(2, userList.Count);
        Assert.Equal("admin", userList[0].Name);
        Assert.Equal("admin@example.com", userList[0].EmailAddress);
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
        Assert.Equal("Administrator", user.DisplayName);
        Assert.True(user.Active);
    }

    [Fact]
    public async Task UpdateUserAsync_ReturnsUpdatedUser()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateUser();
        var client = _fixture.CreateClient();

        var user = await client.UpdateUserAsync(
            email: "newemail@example.com",
            displayName: "New Display Name");

        Assert.NotNull(user);
        Assert.Equal("admin", user.Name);
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

        var newSettings = new Dictionary<string, object?>
        {
            ["theme"] = "light",
            ["notifications"] = false
        };

        var result = await client.UpdateUserSettingsAsync("admin", newSettings);

        Assert.True(result);
    }
}