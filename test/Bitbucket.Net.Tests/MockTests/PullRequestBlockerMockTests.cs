using System.Linq;
using System.Threading.Tasks;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    public class PullRequestBlockerMockTests : IClassFixture<BitbucketMockFixture>
    {
        private readonly BitbucketMockFixture _fixture;

        public PullRequestBlockerMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

#pragma warning disable CS0618 // Type or member is obsolete
        [Fact]
        public async Task GetPullRequestTaskCountAsync_ReturnsTaskCount()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetPullRequestTaskCount(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);
            var client = _fixture.CreateClient();

            var taskCount = await client.GetPullRequestTaskCountAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);

            Assert.NotNull(taskCount);
            Assert.Equal(2, taskCount.Open);
            Assert.Equal(1, taskCount.Resolved);
        }
#pragma warning restore CS0618

        [Fact]
        public async Task GetPullRequestBlockerCommentsAsync_ReturnsBlockerComments()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetPullRequestBlockerComments(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);
            var client = _fixture.CreateClient();

            var blockerComments = await client.GetPullRequestBlockerCommentsAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);

            Assert.NotNull(blockerComments);
            var commentList = blockerComments.ToList();
            Assert.Single(commentList);
            Assert.Equal(1, commentList[0].Id);
            Assert.Equal(BlockerCommentState.Open, commentList[0].State);
        }

        [Fact]
        public async Task GetPullRequestBlockerCommentAsync_ReturnsBlockerComment()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetPullRequestBlockerComment(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId,
                1);
            var client = _fixture.CreateClient();

            var blockerComment = await client.GetPullRequestBlockerCommentAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId,
                1);

            Assert.NotNull(blockerComment);
            Assert.Equal(1, blockerComment.Id);
            Assert.Equal("Please fix this issue before merging", blockerComment.Text);
        }

        [Fact]
        public async Task CreatePullRequestBlockerCommentAsync_ReturnsBlockerComment()
        {
            _fixture.Reset();
            _fixture.Server.SetupCreatePullRequestBlockerComment(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);
            var client = _fixture.CreateClient();

            var blockerComment = await client.CreatePullRequestBlockerCommentAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId,
                "Please fix this issue before merging");

            Assert.NotNull(blockerComment);
            Assert.Equal(1, blockerComment.Id);
        }

        [Fact]
        public async Task DeletePullRequestBlockerCommentAsync_ReturnsTrue()
        {
            _fixture.Reset();
            _fixture.Server.SetupDeletePullRequestBlockerComment(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId,
                1);
            var client = _fixture.CreateClient();

            var result = await client.DeletePullRequestBlockerCommentAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId,
                blockerCommentId: 1,
                version: 0);

            Assert.True(result);
        }

        [Fact]
        public async Task GetPullRequestMergeBaseAsync_ReturnsCommit()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetPullRequestMergeBase(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);
            var client = _fixture.CreateClient();

            var commit = await client.GetPullRequestMergeBaseAsync(
                TestConstants.TestProjectKey,
                TestConstants.TestRepositorySlug,
                TestConstants.TestPullRequestId);

            Assert.NotNull(commit);
            Assert.NotNull(commit.Id);
        }
    }
}
