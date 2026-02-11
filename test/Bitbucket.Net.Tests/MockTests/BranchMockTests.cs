using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Core.Projects.Requests;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class BranchMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetBranchesAsync_ReturnsBranches()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetBranches(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var branches = await client.GetBranchesAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);

        Assert.NotNull(branches);
        var branchList = branches.ToList();
        Assert.Equal(2, branchList.Count);
        Assert.Contains(branchList, b => b.DisplayId == "master");
        Assert.Contains(branchList, b => b.DisplayId == "feature-test");
    }

    [Fact]
    public async Task GetDefaultBranchAsync_ReturnsMasterBranch()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetDefaultBranch(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var branch = await client.GetDefaultBranchAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);

        Assert.NotNull(branch);
        Assert.Equal("master", branch.DisplayId);
        Assert.True(branch.IsDefault);
    }

    [Fact]
    public async Task CreateBranchAsync_ReturnsBranch()
    {
        _fixture.Reset();
        _fixture.Server.SetupCreateBranch(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var request = new CreateBranchRequest
        {
            Name = "feature-test",
            StartPoint = "refs/heads/master"
        };

        var branch = await client.CreateBranchAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            request);

        Assert.NotNull(branch);
        Assert.Equal("refs/heads/feature-test", branch.Id);
        Assert.Equal("feature-test", branch.DisplayId);
        Assert.False(branch.IsDefault);
    }

    [Fact]
    public async Task SetDefaultBranchAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupSetDefaultBranch(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var branchRef = new BranchRef
        {
            Id = "refs/heads/develop"
        };

        var result = await client.SetDefaultBranchAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            branchRef);

        Assert.True(result);
    }
}