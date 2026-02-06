using System.Linq;
using System.Threading.Tasks;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    public class DiffAndTagMockTests : IClassFixture<BitbucketMockFixture>
    {
        private readonly BitbucketMockFixture _fixture;

        public DiffAndTagMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetRepositoryDiffAsync_ReturnsDiff()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetRepositoryDiff(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug);
            var client = _fixture.CreateClient();

            var diff = await client.GetRepositoryDiffAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                "HEAD");

            Assert.NotNull(diff);
            Assert.NotNull(diff.Diffs);
            Assert.NotEmpty(diff.Diffs);
        }

        [Fact]
        public async Task GetPullRequestDiffAsync_ReturnsDiff()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetPullRequestDiff(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);
            var client = _fixture.CreateClient();

            var diff = await client.GetPullRequestDiffAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);

            Assert.NotNull(diff);
            Assert.NotNull(diff.Diffs);
        }

        [Fact]
        public async Task GetCommitCommentsAsync_ReturnsComments()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetCommentsOnFile(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestCommitId);
            var client = _fixture.CreateClient();

            var comments = await client.GetCommitCommentsAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestCommitId,
                "src/main.cs");

            Assert.NotNull(comments);
            var commentList = comments.ToList();
            Assert.Single(commentList);
            Assert.Equal("This is a test comment", commentList[0].Text);
        }

        [Fact]
        public async Task GetPullRequestActivitiesAsync_ReturnsActivities()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetPullRequestActivities(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);
            var client = _fixture.CreateClient();

            var activities = await client.GetPullRequestActivitiesAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);

            Assert.NotNull(activities);
            var activityList = activities.ToList();
            Assert.Equal(2, activityList.Count);
        }
    }
}
