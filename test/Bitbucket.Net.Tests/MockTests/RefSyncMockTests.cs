using Bitbucket.Net.Models.RefSync;
using Bitbucket.Net.Tests.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class RefSyncMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;
    private const string ProjectKey = "PROJ";
    private const string RepoSlug = "repo";

    [Fact]
    public async Task GetRepositorySynchronizationStatusAsync_ReturnsStatus()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetRepositorySynchronizationStatus(ProjectKey, RepoSlug);
        var client = _fixture.CreateClient();

        var result = await client.GetRepositorySynchronizationStatusAsync(ProjectKey, RepoSlug);

        Assert.NotNull(result);
        Assert.True(result.Available);
        Assert.True(result.Enabled);
        Assert.NotNull(result.AheadRefs);
        Assert.Single(result.AheadRefs);
        Assert.NotNull(result.DivergedRefs);
        Assert.Single(result.DivergedRefs);
        Assert.NotNull(result.OrphanedRefs);
        Assert.Single(result.OrphanedRefs);
    }

    [Fact]
    public async Task EnableRepositorySynchronizationAsync_ReturnsStatus()
    {
        _fixture.Reset();
        _fixture.Server.SetupEnableRepositorySynchronization(ProjectKey, RepoSlug);
        var client = _fixture.CreateClient();

        var result = await client.EnableRepositorySynchronizationAsync(ProjectKey, RepoSlug, true);

        Assert.NotNull(result);
        Assert.True(result.Enabled);
    }

    [Fact]
    public async Task SynchronizeRepositoryAsync_ReturnsFullRef()
    {
        _fixture.Reset();
        _fixture.Server.SetupSynchronizeRepository(ProjectKey, RepoSlug);
        var client = _fixture.CreateClient();

        var synchronize = new Synchronize
        {
            RefId = "refs/heads/feature/synced",
            Action = SynchronizeActions.Merge
        };

        var result = await client.SynchronizeRepositoryAsync(ProjectKey, RepoSlug, synchronize);

        Assert.NotNull(result);
        Assert.Equal("refs/heads/feature/synced", result.Id);
        Assert.Equal("SYNCED", result.State);
    }
}