using Bitbucket.Net.Models.DefaultReviewers;
using Bitbucket.Net.Tests.Infrastructure;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class DefaultReviewersExtendedMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task CreateDefaultReviewerConditionAsync_ByProject_ReturnsCondition()
    {
        _fixture.Reset();
        _fixture.Server.SetupCreateDefaultReviewerCondition(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();

        var condition = new DefaultReviewerPullRequestCondition
        {
            SourceRefMatcher = new RefMatcher
            {
                Id = "refs/heads/feature/**",
                Type = new DefaultReviewerPullRequestConditionType { Id = "PATTERN", Name = "Pattern" }
            },
            TargetRefMatcher = new RefMatcher
            {
                Id = "refs/heads/main",
                Type = new DefaultReviewerPullRequestConditionType { Id = "BRANCH", Name = "Branch" }
            },
            RequiredApprovals = 1
        };

        var result = await client.CreateDefaultReviewerConditionAsync(
            TestConstants.TestProjectKey,
            condition);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task UpdateDefaultReviewerConditionAsync_ByProject_ReturnsCondition()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateDefaultReviewerCondition(TestConstants.TestProjectKey, "1");
        var client = _fixture.CreateClient();

        var condition = new DefaultReviewerPullRequestCondition
        {
            Id = 1,
            RequiredApprovals = 2
        };

        var result = await client.UpdateDefaultReviewerConditionAsync(
            TestConstants.TestProjectKey,
            "1",
            condition);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteDefaultReviewerConditionAsync_ByProject_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeleteDefaultReviewerCondition(TestConstants.TestProjectKey, "1");
        var client = _fixture.CreateClient();

        var result = await client.DeleteDefaultReviewerConditionAsync(
            TestConstants.TestProjectKey,
            "1");

        Assert.True(result);
    }

    [Fact]
    public async Task CreateDefaultReviewerConditionAsync_ByRepo_ReturnsCondition()
    {
        _fixture.Reset();
        _fixture.Server.SetupCreateRepoDefaultReviewerCondition(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var condition = new DefaultReviewerPullRequestCondition
        {
            SourceRefMatcher = new RefMatcher
            {
                Id = "refs/heads/feature/**",
                Type = new DefaultReviewerPullRequestConditionType { Id = "PATTERN", Name = "Pattern" }
            },
            TargetRefMatcher = new RefMatcher
            {
                Id = "refs/heads/main",
                Type = new DefaultReviewerPullRequestConditionType { Id = "BRANCH", Name = "Branch" }
            },
            RequiredApprovals = 1
        };

        var result = await client.CreateDefaultReviewerConditionAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            condition);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateDefaultReviewerConditionAsync_ByRepo_ReturnsCondition()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateRepoDefaultReviewerCondition(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            "1");
        var client = _fixture.CreateClient();

        var condition = new DefaultReviewerPullRequestCondition
        {
            Id = 1,
            RequiredApprovals = 2
        };

        var result = await client.UpdateDefaultReviewerConditionAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            "1",
            condition);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteDefaultReviewerConditionAsync_ByRepo_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeleteRepoDefaultReviewerCondition(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            "1");
        var client = _fixture.CreateClient();

        var result = await client.DeleteDefaultReviewerConditionAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            "1");

        Assert.True(result);
    }
}