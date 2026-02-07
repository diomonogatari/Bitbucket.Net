using Bitbucket.Net.Tests.Infrastructure;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class GitAndTagMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetCanRebasePullRequestAsync_ReturnsCondition()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetCanRebasePullRequest(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var condition = await client.GetCanRebasePullRequestAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);

        Assert.NotNull(condition);
    }

    [Fact]
    public async Task RebasePullRequestAsync_RebasesSucessfully()
    {
        _fixture.Reset();
        _fixture.Server.SetupRebasePullRequest(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var result = await client.RebasePullRequestAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId,
            version: 1);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateTagAsync_CreatesTag()
    {
        _fixture.Reset();
        _fixture.Server.SetupCreateTag(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var tag = await client.CreateTagAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            Bitbucket.Net.Models.Git.TagTypes.LightWeight,
            "v1.0.0",
            "abc123");

        Assert.NotNull(tag);
    }

    [Fact]
    public async Task DeleteTagAsync_DeletesTag()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeleteTag(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            "v1.0.0");
        var client = _fixture.CreateClient();

        var result = await client.DeleteTagAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            "v1.0.0");

        Assert.True(result);
    }
}