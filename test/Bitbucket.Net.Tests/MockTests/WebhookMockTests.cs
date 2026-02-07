using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Tests.Infrastructure;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class WebhookMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetProjectRepositoryWebHookAsync_ReturnsWebhook()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetWebhook(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, "1");
        var client = _fixture.CreateClient();

        var webhook = await client.GetProjectRepositoryWebHookAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            "1");

        Assert.NotNull(webhook);
        Assert.Equal(1, webhook.Id);
        Assert.Equal("CI/CD Webhook", webhook.Name);
    }

    [Fact]
    public async Task CreateProjectRepositoryWebHookAsync_ReturnsWebhook()
    {
        _fixture.Reset();
        _fixture.Server.SetupCreateWebhook(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var newWebhook = new WebHook
        {
            Name = "Test Webhook",
            Url = "https://example.com/webhook",
            Active = true,
            Events = ["repo:refs_changed"]
        };

        var webhook = await client.CreateProjectRepositoryWebHookAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            newWebhook);

        Assert.NotNull(webhook);
        Assert.Equal(1, webhook.Id);
    }

    [Fact]
    public async Task DeleteProjectRepositoryWebHookAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeleteWebhook(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, "1");
        var client = _fixture.CreateClient();

        var result = await client.DeleteProjectRepositoryWebHookAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            "1");

        Assert.True(result);
    }
}