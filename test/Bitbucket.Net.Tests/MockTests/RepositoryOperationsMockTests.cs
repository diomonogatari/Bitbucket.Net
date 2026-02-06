using System.Linq;
using System.Threading.Tasks;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    public class RepositoryOperationsMockTests : IClassFixture<BitbucketMockFixture>
    {
        private readonly BitbucketMockFixture _fixture;

        public RepositoryOperationsMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task RecreateProjectRepositoryAsync_ReturnsRepository()
        {
            _fixture.Reset();
            _fixture.Server.SetupRecreateProjectRepository(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var result = await client.RecreateProjectRepositoryAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);

            Assert.NotNull(result);
            Assert.Equal(TestConstants.TestRepositorySlug, result.Slug);
        }

        [Fact]
        public async Task GetRelatedProjectRepositoriesAsync_ReturnsRelatedRepos()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetRelatedProjectRepositories(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var result = await client.GetRelatedProjectRepositoriesAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);

            Assert.NotNull(result);
            var repos = result.ToList();
            Assert.NotEmpty(repos);
        }

        [Fact]
        public async Task GetProjectRepositoryArchiveAsync_ReturnsBytes()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetProjectRepositoryArchive(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var result = await client.GetProjectRepositoryArchiveAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                at: "refs/heads/main",
                fileName: "archive",
                archiveFormat: ArchiveFormats.Zip,
                path: "/",
                prefix: "repo/");

            Assert.NotNull(result);
            Assert.True(result.Length > 0);
        }

        [Fact]
        public async Task GetProjectRepositoryPullRequestSettingsAsync_ReturnsSettings()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetProjectRepositoryPullRequestSettings(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var result = await client.GetProjectRepositoryPullRequestSettingsAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateProjectRepositoryPullRequestSettingsAsync_ReturnsUpdatedSettings()
        {
            _fixture.Reset();
            _fixture.Server.SetupUpdateProjectRepositoryPullRequestSettings(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var settings = new PullRequestSettings
            {
                RequiredApprovers = 2,
                RequiredSuccessfulBuilds = 1,
                RequiredAllApprovers = false,
                RequiredAllTasksComplete = true
            };

            var result = await client.UpdateProjectRepositoryPullRequestSettingsAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                settings);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetProjectRepositoryHooksSettingsAsync_ReturnsHooksSettings()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetProjectRepositoryHooksSettings(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var result = await client.GetProjectRepositoryHooksSettingsAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);

            Assert.NotNull(result);
            var hooks = result.ToList();
            Assert.NotEmpty(hooks);
        }

        [Fact]
        public async Task EnableProjectRepositoryHookAsync_ReturnsHook()
        {
            _fixture.Reset();
            _fixture.Server.SetupEnableProjectRepositoryHook(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                "com.example.hook");
            var client = _fixture.CreateClient();

            var result = await client.EnableProjectRepositoryHookAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                "com.example.hook");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task DisableProjectRepositoryHookAsync_ReturnsHook()
        {
            _fixture.Reset();
            _fixture.Server.SetupDisableProjectRepositoryHook(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                "com.example.hook");
            var client = _fixture.CreateClient();

            var result = await client.DisableProjectRepositoryHookAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                "com.example.hook");

            Assert.NotNull(result);
        }
    }
}
