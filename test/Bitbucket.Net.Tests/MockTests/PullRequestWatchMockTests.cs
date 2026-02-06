using System.Linq;
using System.Threading.Tasks;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    public class PullRequestWatchMockTests : IClassFixture<BitbucketMockFixture>
    {
        private readonly BitbucketMockFixture _fixture;

        public PullRequestWatchMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task WatchPullRequestAsync_ReturnsTrue()
        {
            _fixture.Reset();
            _fixture.Server.SetupWatchPullRequest(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);
            var client = _fixture.CreateClient();

            var result = await client.WatchPullRequestAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);

            Assert.True(result);
        }

        [Fact]
        public async Task UnwatchPullRequestAsync_ReturnsTrue()
        {
            _fixture.Reset();
            _fixture.Server.SetupUnwatchPullRequest(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);
            var client = _fixture.CreateClient();

            var result = await client.UnwatchPullRequestAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);

            Assert.True(result);
        }

        [Fact]
        public async Task DeletePullRequestAsync_ReturnsTrue()
        {
            _fixture.Reset();
            _fixture.Server.SetupDeletePullRequest(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);
            var client = _fixture.CreateClient();

            var versionInfo = new VersionInfo { Version = 0 };

            var result = await client.DeletePullRequestAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId,
                versionInfo);

            Assert.True(result);
        }

        [Fact]
        public async Task ReopenPullRequestAsync_ReturnsPullRequest()
        {
            _fixture.Reset();
            _fixture.Server.SetupReopenPullRequest(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);
            var client = _fixture.CreateClient();

            var pullRequest = await client.ReopenPullRequestAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);

            Assert.NotNull(pullRequest);
            Assert.Equal(TestConstants.TestPullRequestId, pullRequest.Id);
        }

        [Fact]
        public async Task DeletePullRequestApprovalAsync_ReturnsReviewer()
        {
            _fixture.Reset();
            _fixture.Server.SetupDeletePullRequestApproval(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);
            var client = _fixture.CreateClient();

            var reviewer = await client.DeletePullRequestApprovalAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);

            Assert.NotNull(reviewer);
            Assert.False(reviewer.Approved);
            Assert.Equal(ParticipantStatus.Unapproved, reviewer.Status);
        }
    }
}
