using Bitbucket.Net.Tests.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class AuditMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;
    private const string ProjectKey = "PROJ";
    private const string RepoSlug = "repo";

    [Fact]
    public async Task GetProjectAuditEventsAsync_ReturnsEvents()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjectAuditEvents(ProjectKey);
        var client = _fixture.CreateClient();

        var result = await client.GetProjectAuditEventsAsync(ProjectKey);

        Assert.NotNull(result);
        var events = result.ToList();
        Assert.Equal(2, events.Count);
        Assert.Equal("PROJECT_CREATED", events[0].Action);
    }

    [Fact]
    public async Task GetProjectRepoAuditEventsAsync_ReturnsEvents()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjectRepoAuditEvents(ProjectKey, RepoSlug);
        var client = _fixture.CreateClient();

        var result = await client.GetProjectRepoAuditEventsAsync(ProjectKey, RepoSlug);

        Assert.NotNull(result);
        var events = result.ToList();
        Assert.Equal(2, events.Count);
        Assert.Equal("PROJECT_CREATED", events[0].Action);
    }
}