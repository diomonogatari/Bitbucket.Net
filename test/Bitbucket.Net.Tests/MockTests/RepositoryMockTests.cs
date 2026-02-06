using System.Linq;
using System.Threading.Tasks;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    /// <summary>
    /// Unit tests for repository-related operations using WireMock.
    /// </summary>
    public class RepositoryMockTests : IClassFixture<BitbucketMockFixture>
    {
        private readonly BitbucketMockFixture _fixture;

        public RepositoryMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetProjectRepositoryAsync_WithValidSlug_ReturnsRepository()
        {
            // Arrange
            _fixture.Reset();
            _fixture.Server.SetupGetRepository(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            // Act
            var repository = await client.GetProjectRepositoryAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);

            // Assert
            Assert.NotNull(repository);
            Assert.Equal(TestConstants.TestRepositorySlug, repository.Slug);
            Assert.Equal(TestConstants.TestRepositoryName, repository.Name);
            Assert.NotNull(repository.Project);
            Assert.Equal(TestConstants.TestProjectKey, repository.Project.Key);
        }

        [Fact]
        public async Task GetProjectRepositoriesAsync_ReturnsRepositories()
        {
            // Arrange
            _fixture.Reset();
            _fixture.Server.SetupGetRepositories(TestConstants.TestProjectKey);
            var client = _fixture.CreateClient();

            // Act
            var repositories = await client.GetProjectRepositoriesAsync(TestConstants.TestProjectKey);

            // Assert
            Assert.NotNull(repositories);
            var repoList = repositories.ToList();
            Assert.Single(repoList);
            var repo = repoList[0];
            Assert.Equal(TestConstants.TestRepositorySlug, repo.Slug);
            Assert.Equal(TestConstants.TestRepositoryName, repo.Name);
        }

        [Fact]
        public async Task GetRepositoryParticipantsAsync_ReturnsParticipants()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetRepositoryParticipants(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var participants = await client.GetRepositoryParticipantsAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);

            Assert.NotNull(participants);
            Assert.NotEmpty(participants);
        }
    }
}
