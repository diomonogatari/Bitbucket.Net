using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class PullRequestExtendedMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

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

    [Fact]
    public async Task GetPullRequestChangesAsync_ReturnsChanges()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequestChanges(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var changes = await client.GetPullRequestChangesAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);

        Assert.NotNull(changes);
        var changeList = changes.ToList();
        Assert.Single(changeList);
    }

    [Fact]
    public async Task GetPullRequestCommitsAsync_ReturnsCommits()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequestCommits(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var commits = await client.GetPullRequestCommitsAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);

        Assert.NotNull(commits);
        var commitList = commits.ToList();
        Assert.Equal(2, commitList.Count);
        Assert.Contains(commitList, c => c.Message == "Initial commit");
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
    public async Task MergePullRequestAsync_ReturnsPullRequest()
    {
        _fixture.Reset();
        _fixture.Server.SetupMergePullRequest(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var result = await client.MergePullRequestAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);

        Assert.NotNull(result);
        Assert.Equal(TestConstants.TestPullRequestId, result.Id);
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
    }

    [Fact]
    public async Task CreatePullRequestAsync_ReturnsPullRequest()
    {
        _fixture.Reset();
        _fixture.Server.SetupCreatePullRequest(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var prInfo = new PullRequestInfo
        {
            Title = "New PR",
            Description = "Description",
            FromRef = new FromToRef { Id = "refs/heads/feature" },
            ToRef = new FromToRef { Id = "refs/heads/main" }
        };

        var result = await client.CreatePullRequestAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            prInfo);

        Assert.NotNull(result);
        Assert.Equal(TestConstants.TestPullRequestId, result.Id);
    }

    [Fact]
    public async Task UpdatePullRequestAsync_ReturnsPullRequest()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdatePullRequest(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var update = new PullRequestUpdate
        {
            Id = (int)TestConstants.TestPullRequestId,
            Version = 0,
            Title = "Updated Title"
        };

        var result = await client.UpdatePullRequestAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId,
            update);

        Assert.NotNull(result);
    }
}