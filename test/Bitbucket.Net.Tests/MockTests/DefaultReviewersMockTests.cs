using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class DefaultReviewersMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetDefaultReviewerConditionsAsync_ByProjectKey_ReturnsConditions()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetDefaultReviewerConditions(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();

        var conditions = await client.GetDefaultReviewerConditionsAsync(TestConstants.TestProjectKey);

        Assert.NotNull(conditions);
    }

    [Fact]
    public async Task GetDefaultReviewerConditionsAsync_ByProjectAndRepo_ReturnsConditions()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetRepoDefaultReviewerConditions(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var conditions = await client.GetDefaultReviewerConditionsAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);

        Assert.NotNull(conditions);
    }

    [Fact]
    public async Task GetDefaultReviewersAsync_ReturnsReviewers()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetDefaultReviewers(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var reviewers = await client.GetDefaultReviewersAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            sourceRepoId: 1,
            targetRepoId: 1,
            sourceRefId: "refs/heads/feature",
            targetRefId: "refs/heads/main");

        Assert.NotNull(reviewers);
    }
}