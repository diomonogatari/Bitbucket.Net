using System.Threading.Tasks;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    public class PullRequestActionsMockTests : IClassFixture<BitbucketMockFixture>
    {
        private readonly BitbucketMockFixture _fixture;

        public PullRequestActionsMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ApprovePullRequestAsync_ReturnsReviewer()
        {
            _fixture.Reset();
            _fixture.Server.SetupApprovePullRequest(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);
            var client = _fixture.CreateClient();

            var reviewer = await client.ApprovePullRequestAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);

            Assert.NotNull(reviewer);
            Assert.True(reviewer.Approved);
            Assert.Equal(ParticipantStatus.Approved, reviewer.Status);
        }

        [Fact]
        public async Task MergePullRequestAsync_ReturnsMergedPullRequest()
        {
            _fixture.Reset();
            _fixture.Server.SetupMergePullRequest(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);
            var client = _fixture.CreateClient();

            var pullRequest = await client.MergePullRequestAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);

            Assert.NotNull(pullRequest);
            Assert.Equal(TestConstants.TestPullRequestId, pullRequest.Id);
        }

        [Fact]
        public async Task DeclinePullRequestAsync_ReturnsTrue()
        {
            _fixture.Reset();
            _fixture.Server.SetupDeclinePullRequest(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);
            var client = _fixture.CreateClient();

            var result = await client.DeclinePullRequestAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);

            Assert.True(result);
        }

        [Fact]
        public async Task GetPullRequestMergeStateAsync_ReturnsMergeState()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetPullRequestMergeState(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);
            var client = _fixture.CreateClient();

            var mergeState = await client.GetPullRequestMergeStateAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);

            Assert.NotNull(mergeState);
            Assert.True(mergeState.CanMerge);
            Assert.False(mergeState.Conflicted);
        }
    }
}
