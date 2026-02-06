using System.Linq;
using System.Threading.Tasks;
using Bitbucket.Net.Models.DefaultReviewers;
using Bitbucket.Net.Models.RefRestrictions;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    public class RefRestrictionsMockTests : IClassFixture<BitbucketMockFixture>
    {
        private readonly BitbucketMockFixture _fixture;
        private const string ProjectKey = "PROJ";
        private const string RepoSlug = "repo";

        public RefRestrictionsMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetProjectRefRestrictionsAsync_ReturnsRestrictions()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetProjectRefRestrictions(ProjectKey);
            var client = _fixture.CreateClient();

            var result = await client.GetProjectRefRestrictionsAsync(ProjectKey);

            Assert.NotNull(result);
            var restrictions = result.ToList();
            Assert.Equal(2, restrictions.Count);
            Assert.Equal(1, restrictions[0].Id);
        }

        [Fact]
        public async Task GetProjectRefRestrictionAsync_ReturnsRestriction()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetProjectRefRestriction(ProjectKey, 1);
            var client = _fixture.CreateClient();

            var result = await client.GetProjectRefRestrictionAsync(ProjectKey, 1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.NotNull(result.Matcher);
        }

        [Fact]
        public async Task CreateProjectRefRestrictionAsync_ReturnsCreatedRestriction()
        {
            _fixture.Reset();
            _fixture.Server.SetupCreateProjectRefRestriction(ProjectKey);
            var client = _fixture.CreateClient();

            var restriction = new RefRestrictionCreate
            {
                Type = RefRestrictionTypes.AllChanges,
                Matcher = new RefMatcher
                {
                    Id = "refs/heads/main",
                    DisplayId = "main",
                    Active = true,
                    Type = new DefaultReviewerPullRequestConditionType { Id = "BRANCH", Name = "Branch" }
                }
            };

            var result = await client.CreateProjectRefRestrictionAsync(ProjectKey, restriction);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task CreateProjectRefRestrictionsAsync_ReturnsCreatedRestrictions()
        {
            _fixture.Reset();
            _fixture.Server.SetupCreateProjectRefRestrictions(ProjectKey);
            var client = _fixture.CreateClient();

            var restriction = new RefRestrictionCreate
            {
                Type = RefRestrictionTypes.Deletion,
                Matcher = new RefMatcher
                {
                    Id = "refs/heads/main",
                    DisplayId = "main",
                    Active = true,
                    Type = new DefaultReviewerPullRequestConditionType { Id = "BRANCH", Name = "Branch" }
                }
            };

            var result = await client.CreateProjectRefRestrictionsAsync(ProjectKey, restriction);

            Assert.NotNull(result);
            var restrictions = result.ToList();
            Assert.Single(restrictions);
            Assert.Equal(3, restrictions[0].Id);
        }

        [Fact]
        public async Task DeleteProjectRefRestrictionAsync_ReturnsSuccess()
        {
            _fixture.Reset();
            _fixture.Server.SetupDeleteProjectRefRestriction(ProjectKey, 1);
            var client = _fixture.CreateClient();

            var result = await client.DeleteProjectRefRestrictionAsync(ProjectKey, 1);

            Assert.True(result);
        }

        [Fact]
        public async Task GetRepositoryRefRestrictionsAsync_ReturnsRestrictions()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetRepositoryRefRestrictions(ProjectKey, RepoSlug);
            var client = _fixture.CreateClient();

            var result = await client.GetRepositoryRefRestrictionsAsync(ProjectKey, RepoSlug);

            Assert.NotNull(result);
            var restrictions = result.ToList();
            Assert.Equal(2, restrictions.Count);
        }

        [Fact]
        public async Task GetRepositoryRefRestrictionAsync_ReturnsRestriction()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetRepositoryRefRestriction(ProjectKey, RepoSlug, 1);
            var client = _fixture.CreateClient();

            var result = await client.GetRepositoryRefRestrictionAsync(ProjectKey, RepoSlug, 1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task CreateRepositoryRefRestrictionAsync_ReturnsCreatedRestriction()
        {
            _fixture.Reset();
            _fixture.Server.SetupCreateRepositoryRefRestriction(ProjectKey, RepoSlug);
            var client = _fixture.CreateClient();

            var restriction = new RefRestrictionCreate
            {
                Type = RefRestrictionTypes.RewritingHistory,
                Matcher = new RefMatcher
                {
                    Id = "refs/heads/main",
                    DisplayId = "main",
                    Active = true,
                    Type = new DefaultReviewerPullRequestConditionType { Id = "BRANCH", Name = "Branch" }
                }
            };

            var result = await client.CreateRepositoryRefRestrictionAsync(ProjectKey, RepoSlug, restriction);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task CreateRepositoryRefRestrictionsAsync_ReturnsCreatedRestrictions()
        {
            _fixture.Reset();
            _fixture.Server.SetupCreateRepositoryRefRestrictions(ProjectKey, RepoSlug);
            var client = _fixture.CreateClient();

            var restriction = new RefRestrictionCreate
            {
                Type = RefRestrictionTypes.ChangesWithoutPullRequest,
                Matcher = new RefMatcher
                {
                    Id = "refs/heads/main",
                    DisplayId = "main",
                    Active = true,
                    Type = new DefaultReviewerPullRequestConditionType { Id = "BRANCH", Name = "Branch" }
                }
            };

            var result = await client.CreateRepositoryRefRestrictionsAsync(ProjectKey, RepoSlug, restriction);

            Assert.NotNull(result);
            var restrictions = result.ToList();
            Assert.Single(restrictions);
        }

        [Fact]
        public async Task DeleteRepositoryRefRestrictionAsync_ReturnsSuccess()
        {
            _fixture.Reset();
            _fixture.Server.SetupDeleteRepositoryRefRestriction(ProjectKey, RepoSlug, 1);
            var client = _fixture.CreateClient();

            var result = await client.DeleteRepositoryRefRestrictionAsync(ProjectKey, RepoSlug, 1);

            Assert.True(result);
        }
    }
}
