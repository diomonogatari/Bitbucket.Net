using System.Linq;
using System.Threading.Tasks;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    public class WebhookAndCompareMockTests : IClassFixture<BitbucketMockFixture>
    {
        private readonly BitbucketMockFixture _fixture;

        public WebhookAndCompareMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetProjectRepositoryWebHooksAsync_ReturnsWebhooks()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetWebhooks(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var webhooks = await client.GetProjectRepositoryWebHooksAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);

            Assert.NotNull(webhooks);
            var webhookList = webhooks.ToList();
            Assert.Equal(2, webhookList.Count);
            Assert.Equal("CI/CD Webhook", webhookList[0].Name);
            Assert.True(webhookList[0].Active);
        }

        [Fact]
        public async Task GetRepositoryCompareCommitsAsync_ReturnsCommits()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetCompareCommits(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var commits = await client.GetRepositoryCompareCommitsAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                from: "HEAD~5",
                to: "HEAD");

            Assert.NotNull(commits);
            var commitList = commits.ToList();
            Assert.Equal(2, commitList.Count);
        }

        [Fact]
        public async Task GetRepositoryCompareDiffAsync_ReturnsDiff()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetCompareDiff(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var diff = await client.GetRepositoryCompareDiffAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                from: "HEAD~5",
                to: "HEAD");

            Assert.NotNull(diff);
            Assert.NotNull(diff.Diffs);
        }
    }
}
