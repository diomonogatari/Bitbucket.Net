using Bitbucket.Net.Models.Core.Projects.Requests;
using Bitbucket.Net.Tests.Infrastructure;
using Flurl.Http;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class CancellationMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private const string ApiBasePath = "/rest/api/1.0";
    private readonly BitbucketMockFixture _fixture = fixture;

    #region Buffered Methods — Pre-cancelled Token

    [Fact]
    public async Task GetProjectsAsync_PreCancelledToken_ThrowsOperationCanceled()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjects();
        var client = _fixture.CreateClient();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.GetProjectsAsync(cancellationToken: cts.Token));
    }

    [Fact]
    public async Task GetProjectAsync_PreCancelledToken_ThrowsOperationCanceled()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProject(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await AssertCancellationPropagatedAsync(
            () => client.GetProjectAsync(TestConstants.TestProjectKey, cts.Token));
    }

    [Fact]
    public async Task CreateProjectAsync_PreCancelledToken_ThrowsOperationCanceled()
    {
        _fixture.Reset();
        var client = _fixture.CreateClient();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var definition = new CreateProjectRequest { Key = TestConstants.TestProjectKey, Name = TestConstants.TestProjectName };

        await AssertCancellationPropagatedAsync(
            () => client.CreateProjectAsync(definition, cts.Token));
    }

    [Fact]
    public async Task DeleteProjectAsync_PreCancelledToken_ThrowsOperationCanceled()
    {
        _fixture.Reset();
        var client = _fixture.CreateClient();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await AssertCancellationPropagatedAsync(
            () => client.DeleteProjectAsync(TestConstants.TestProjectKey, cts.Token));
    }

    [Fact]
    public async Task GetPullRequestsAsync_PreCancelledToken_ThrowsOperationCanceled()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequests(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.GetPullRequestsAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, cancellationToken: cts.Token));
    }

    #endregion

    #region Streaming Methods — Pre-cancelled Token

    [Fact]
    public async Task GetProjectsStreamAsync_PreCancelledToken_ThrowsOnMoveNext()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjects();
        var client = _fixture.CreateClient();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await foreach (var _ in client.GetProjectsStreamAsync(cancellationToken: cts.Token))
            {
            }
        });
    }

    [Fact]
    public async Task GetPullRequestsStreamAsync_PreCancelledToken_ThrowsOnMoveNext()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequests(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await foreach (var _ in client.GetPullRequestsStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, cancellationToken: cts.Token))
            {
            }
        });
    }

    [Fact]
    public async Task GetBranchesStreamAsync_PreCancelledToken_ThrowsOnMoveNext()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetBranches(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await foreach (var _ in client.GetBranchesStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, cancellationToken: cts.Token))
            {
            }
        });
    }

    #endregion

    #region Streaming Methods — Cancel During Multi-page

    [Fact]
    public async Task GetProjectsStreamAsync_CancelAfterFirstPage_StopsEnumeration()
    {
        _fixture.Reset();
        _fixture.Server.SetupPagedEndpoint(
            $"{ApiBasePath}/projects", "Core", "projects-page1.json", "projects-page2.json");
        var client = _fixture.CreateClient();
        using var cts = new CancellationTokenSource();
        var results = new List<Models.Core.Projects.Project>();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await foreach (var project in client.GetProjectsStreamAsync(start: 0, cancellationToken: cts.Token))
            {
                results.Add(project);
                if (results.Count >= 2)
                    cts.Cancel();
            }
        });

        Assert.Equal(2, results.Count);
    }

    #endregion

    #region Diff Streaming — Pre-cancelled Token

    [Fact]
    public async Task GetPullRequestDiffStreamAsync_PreCancelledToken_ThrowsOperationCanceled()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequestDiff(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await AssertCancellationPropagatedAsync(async () =>
        {
            await foreach (var _ in client.GetPullRequestDiffStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId, cancellationToken: cts.Token))
            {
            }
        });
    }

    [Fact]
    public async Task GetCommitDiffStreamAsync_PreCancelledToken_ThrowsOperationCanceled()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetCommitDiff(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestCommitId);
        var client = _fixture.CreateClient();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await AssertCancellationPropagatedAsync(async () =>
        {
            await foreach (var _ in client.GetCommitDiffStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestCommitId, cancellationToken: cts.Token))
            {
            }
        });
    }

    #endregion

    #region Pre-cancelled Token — No HTTP Call Observed

    [Fact]
    public async Task GetProjectsAsync_PreCancelledToken_NoHttpCallMade()
    {
        _fixture.Reset();
        var client = _fixture.CreateClient();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.GetProjectsAsync(cancellationToken: cts.Token));

        Assert.Empty(_fixture.Server.LogEntries);
    }

    #endregion

    /// <summary>
    /// Asserts that a cancelled token propagates correctly, whether the runtime throws
    /// <see cref="OperationCanceledException"/> directly or Flurl wraps it in a
    /// <see cref="FlurlHttpException"/>.
    /// </summary>
    private static async Task AssertCancellationPropagatedAsync(Func<Task> action)
    {
        try
        {
            await action();
            Assert.Fail("Expected cancellation to be propagated, but the operation completed normally.");
        }
        catch (OperationCanceledException)
        {
            // Direct cancellation — expected (paged methods check token first)
        }
        catch (FlurlHttpException ex) when (ex.InnerException is OperationCanceledException)
        {
            // Flurl-wrapped cancellation — also expected (non-paged methods hit Flurl first)
        }
    }
}