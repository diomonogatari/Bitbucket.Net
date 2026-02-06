using System.Threading.Tasks;
using Bitbucket.Net.Models.Git;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    public class GitMockTests : IClassFixture<BitbucketMockFixture>
    {
        private readonly BitbucketMockFixture _fixture;
        private const string ProjectKey = "TEST";
        private const string RepoSlug = "test-repo";
        private const long PullRequestId = 1;

        public GitMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetCanRebasePullRequestAsync_ReturnsCondition()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetCanRebasePullRequest(ProjectKey, RepoSlug, PullRequestId);
            var client = _fixture.CreateClient();

            var result = await client.GetCanRebasePullRequestAsync(ProjectKey, RepoSlug, PullRequestId);

            Assert.NotNull(result);
            Assert.True(result.CanRebase);
            Assert.NotNull(result.Vetoes);
            Assert.Empty(result.Vetoes);
        }

        [Fact]
        public async Task RebasePullRequestAsync_ReturnsUpdatedPullRequest()
        {
            _fixture.Reset();
            _fixture.Server.SetupRebasePullRequest(ProjectKey, RepoSlug, PullRequestId);
            var client = _fixture.CreateClient();

            var result = await client.RebasePullRequestAsync(ProjectKey, RepoSlug, PullRequestId, version: 1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task CreateTagAsync_ReturnsCreatedTag()
        {
            _fixture.Reset();
            _fixture.Server.SetupCreateTag(ProjectKey, RepoSlug);
            var client = _fixture.CreateClient();

            var result = await client.CreateTagAsync(ProjectKey, RepoSlug, TagTypes.Annotated, "v1.0.0", "abc123");

            Assert.NotNull(result);
            Assert.Equal("v1.0.0", result.DisplayId);
            Assert.Equal("refs/tags/v1.0.0", result.Id);
        }

        [Fact]
        public async Task DeleteTagAsync_ReturnsTrue()
        {
            _fixture.Reset();
            _fixture.Server.SetupDeleteTag(ProjectKey, RepoSlug, "v1.0.0");
            var client = _fixture.CreateClient();

            var result = await client.DeleteTagAsync(ProjectKey, RepoSlug, "v1.0.0");

            Assert.True(result);
        }
    }
}
