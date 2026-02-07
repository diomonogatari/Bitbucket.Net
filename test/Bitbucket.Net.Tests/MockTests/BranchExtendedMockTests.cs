using Bitbucket.Net.Tests.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class BranchExtendedMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetCommitBranchInfoAsync_ReturnsBranches()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetCommitBranchInfo(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId);
        var client = _fixture.CreateClient();

        var branches = await client.GetCommitBranchInfoAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId);

        Assert.NotNull(branches);
        var branchList = branches.ToList();
        Assert.Equal(2, branchList.Count);
    }

    [Fact]
    public async Task GetRepoBranchModelAsync_ReturnsBranchModel()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetBranchModel(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var model = await client.GetRepoBranchModelAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);

        Assert.NotNull(model);
    }

    [Fact]
    public async Task CreateRepoBranchAsync_ReturnsBranch()
    {
        _fixture.Reset();
        _fixture.Server.SetupCreateRepoBranch(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var branch = await client.CreateRepoBranchAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            "feature/new-branch",
            "refs/heads/master");

        Assert.NotNull(branch);
    }

    [Fact]
    public async Task DeleteRepoBranchAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeleteRepoBranch(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var result = await client.DeleteRepoBranchAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            "feature/old-branch",
            dryRun: false);

        Assert.True(result);
    }
}