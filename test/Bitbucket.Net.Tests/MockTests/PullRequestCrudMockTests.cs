using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Tests.Infrastructure;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class PullRequestCrudMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task CreatePullRequestAsync_ReturnsCreatedPullRequest()
    {
        _fixture.Reset();
        _fixture.Server.SetupCreatePullRequest(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var prInfo = new PullRequestInfo
        {
            Title = "Test PR",
            Description = "Test description",
            FromRef = new FromToRef
            {
                Id = "refs/heads/feature-test",
                Repository = new RepositoryRef
                {
                    Slug = TestConstants.TestRepositorySlug,
                    Project = new ProjectRef { Key = TestConstants.TestProjectKey }
                }
            },
            ToRef = new FromToRef
            {
                Id = "refs/heads/master",
                Repository = new RepositoryRef
                {
                    Slug = TestConstants.TestRepositorySlug,
                    Project = new ProjectRef { Key = TestConstants.TestProjectKey }
                }
            }
        };

        var pullRequest = await client.CreatePullRequestAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            prInfo);

        Assert.NotNull(pullRequest);
        Assert.Equal(TestConstants.TestPullRequestId, pullRequest.Id);
    }

    [Fact]
    public async Task UpdatePullRequestAsync_ReturnsUpdatedPullRequest()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdatePullRequest(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var prUpdate = new PullRequestUpdate
        {
            Title = "Updated Title",
            Version = 0
        };

        var pullRequest = await client.UpdatePullRequestAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId,
            prUpdate);

        Assert.NotNull(pullRequest);
        Assert.Equal(TestConstants.TestPullRequestId, pullRequest.Id);
    }
}