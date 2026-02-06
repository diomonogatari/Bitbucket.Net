using System.Linq;
using System.Threading.Tasks;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    public class RepositoryCrudMockTests : IClassFixture<BitbucketMockFixture>
    {
        private readonly BitbucketMockFixture _fixture;

        public RepositoryCrudMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task CreateProjectRepositoryAsync_CreatesAndReturnsRepository()
        {
            _fixture.Reset();
            _fixture.Server.SetupCreateRepository(TestConstants.TestProjectKey);
            var client = _fixture.CreateClient();

            var result = await client.CreateProjectRepositoryAsync(
                TestConstants.TestProjectKey,
                "new-repo",
                "git");

            Assert.NotNull(result);
            Assert.Equal("test-repo", result.Slug);
        }

        [Fact]
        public async Task UpdateProjectRepositoryAsync_UpdatesAndReturnsRepository()
        {
            _fixture.Reset();
            _fixture.Server.SetupUpdateRepository(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var result = await client.UpdateProjectRepositoryAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                targetName: "updated-repo",
                isForkable: true);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ScheduleProjectRepositoryForDeletionAsync_DeletesRepository()
        {
            _fixture.Reset();
            _fixture.Server.SetupDeleteRepository(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var result = await client.ScheduleProjectRepositoryForDeletionAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);

            Assert.True(result);
        }

        [Fact]
        public async Task CreateProjectRepositoryForkAsync_CreatesAndReturnsFork()
        {
            _fixture.Reset();
            _fixture.Server.SetupForkRepository(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var result = await client.CreateProjectRepositoryForkAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                targetProjectKey: "FORK",
                targetName: "forked-repo");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetProjectRepositoryForksAsync_ReturnsForks()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetRepositoryForks(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var result = await client.GetProjectRepositoryForksAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetProjectRepositoryAsync_ReturnsRepository()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetRepository(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var result = await client.GetProjectRepositoryAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);

            Assert.NotNull(result);
            Assert.Equal("test-repo", result.Slug);
        }
    }
}
