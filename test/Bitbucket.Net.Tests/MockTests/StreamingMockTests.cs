using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class StreamingMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private const string ApiBasePath = "/rest/api/1.0";
    private readonly BitbucketMockFixture _fixture = fixture;

    #region GetProjectsStreamAsync

    [Fact]
    public async Task GetProjectsStreamAsync_SinglePage_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjects();
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetProjectsStreamAsync());

        Assert.Single(results);
        Assert.Equal(TestConstants.TestProjectKey, results[0].Key);
    }

    [Fact]
    public async Task GetProjectsStreamAsync_MultiplePages_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupPagedEndpoint(
            $"{ApiBasePath}/projects", "Core", "projects-page1.json", "projects-page2.json");
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetProjectsStreamAsync(start: 0));

        Assert.Equal(3, results.Count);
        Assert.Equal("PROJ1", results[0].Key);
        Assert.Equal("PROJ2", results[1].Key);
        Assert.Equal("PROJ3", results[2].Key);
    }

    [Fact]
    public async Task GetProjectsStreamAsync_EmptyResult_YieldsZeroItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupEmptyPagedEndpoint($"{ApiBasePath}/projects");
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetProjectsStreamAsync());

        Assert.Empty(results);
    }

    #endregion

    #region GetProjectRepositoriesStreamAsync

    [Fact]
    public async Task GetProjectRepositoriesStreamAsync_SinglePage_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetRepositories(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetProjectRepositoriesStreamAsync(TestConstants.TestProjectKey));

        Assert.Single(results);
        Assert.Equal(TestConstants.TestRepositorySlug, results[0].Slug);
    }

    [Fact]
    public async Task GetProjectRepositoriesStreamAsync_MultiplePages_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupPagedEndpoint(
            $"{ApiBasePath}/projects/{TestConstants.TestProjectKey}/repos",
            "Core", "repositories-page1.json", "repositories-page2.json");
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetProjectRepositoriesStreamAsync(TestConstants.TestProjectKey, start: 0));

        Assert.Equal(3, results.Count);
        Assert.Equal("repo-alpha", results[0].Slug);
        Assert.Equal("repo-beta", results[1].Slug);
        Assert.Equal("repo-gamma", results[2].Slug);
    }

    #endregion

    #region GetRepositoriesStreamAsync

    [Fact]
    public async Task GetRepositoriesStreamAsync_SinglePage_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupCustomResponse($"{ApiBasePath}/repos", System.Net.HttpStatusCode.OK, "Core", "repositories-list.json");
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetRepositoriesStreamAsync());

        Assert.Single(results);
    }

    [Fact]
    public async Task GetRepositoriesStreamAsync_MultiplePages_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupPagedEndpoint(
            $"{ApiBasePath}/repos", "Core", "repositories-page1.json", "repositories-page2.json");
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetRepositoriesStreamAsync(start: 0));

        Assert.Equal(3, results.Count);
    }

    #endregion

    #region GetBranchesStreamAsync

    [Fact]
    public async Task GetBranchesStreamAsync_SinglePage_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetBranches(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetBranchesStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug));

        Assert.Equal(2, results.Count);
        Assert.Equal("master", results[0].DisplayId);
    }

    [Fact]
    public async Task GetBranchesStreamAsync_MultiplePages_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupPagedEndpoint(
            $"{ApiBasePath}/projects/{TestConstants.TestProjectKey}/repos/{TestConstants.TestRepositorySlug}/branches",
            "Core", "branches-page1.json", "branches-page2.json");
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetBranchesStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, start: 0));

        Assert.Equal(3, results.Count);
        Assert.Equal("main", results[0].DisplayId);
        Assert.Equal("develop", results[1].DisplayId);
        Assert.Equal("feature-x", results[2].DisplayId);
    }

    #endregion

    #region GetCommitsStreamAsync

    [Fact]
    public async Task GetCommitsStreamAsync_SinglePage_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetCommits(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetCommitsStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, until: "HEAD"));

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task GetCommitsStreamAsync_MultiplePages_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupPagedEndpoint(
            $"{ApiBasePath}/projects/{TestConstants.TestProjectKey}/repos/{TestConstants.TestRepositorySlug}/commits",
            "Core", "commits-page1.json", "commits-page2.json");
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetCommitsStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, until: "HEAD", start: 0));

        Assert.Equal(3, results.Count);
    }

    #endregion

    #region GetPullRequestsStreamAsync

    [Fact]
    public async Task GetPullRequestsStreamAsync_SinglePage_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequests(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetPullRequestsStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug));

        Assert.Single(results);
        Assert.Equal(TestConstants.TestPullRequestTitle, results[0].Title);
    }

    [Fact]
    public async Task GetPullRequestsStreamAsync_EmptyResult_YieldsZeroItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupEmptyPagedEndpoint(
            $"{ApiBasePath}/projects/{TestConstants.TestProjectKey}/repos/{TestConstants.TestRepositorySlug}/pull-requests");
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetPullRequestsStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug));

        Assert.Empty(results);
    }

    #endregion

    #region GetPullRequestCommitsStreamAsync

    [Fact]
    public async Task GetPullRequestCommitsStreamAsync_SinglePage_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequestCommits(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetPullRequestCommitsStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId));

        Assert.Equal(2, results.Count);
    }

    #endregion

    #region GetRawFileContentLinesStreamAsync

    [Fact]
    public async Task GetRawFileContentLinesStreamAsync_ReturnsContentLines()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetRawFileContentStream(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetRawFileContentLinesStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, "README.md"));

        Assert.NotEmpty(results);
        Assert.Contains(results, l => l.Contains("README"));
    }

    #endregion

    #region Phase 5 Streaming Methods

    [Fact]
    public async Task GetPullRequestActivitiesStreamAsync_SinglePage_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequestActivities(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetPullRequestActivitiesStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId));

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task GetPullRequestChangesStreamAsync_SinglePage_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupCustomResponse(
            $"{ApiBasePath}/projects/{TestConstants.TestProjectKey}/repos/{TestConstants.TestRepositorySlug}/pull-requests/{TestConstants.TestPullRequestId}/changes",
            System.Net.HttpStatusCode.OK, "PullRequests", "changes.json");
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetPullRequestChangesStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId));

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task GetPullRequestParticipantsStreamAsync_SinglePage_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequestParticipants(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetPullRequestParticipantsStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId));

        Assert.Single(results);
    }

    [Fact]
    public async Task GetPullRequestTasksStreamAsync_SinglePage_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequestTasks(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

#pragma warning disable CS0618 // Obsolete
        var results = await CollectAsync(client.GetPullRequestTasksStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId));
#pragma warning restore CS0618

        Assert.Single(results);
    }

    [Fact]
    public async Task GetPullRequestBlockerCommentsStreamAsync_SinglePage_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequestBlockerComments(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetPullRequestBlockerCommentsStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestPullRequestId));

        Assert.Single(results);
    }

    [Fact]
    public async Task GetChangesStreamAsync_SinglePage_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetChanges(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetChangesStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, until: "HEAD"));

        Assert.Single(results);
    }

    [Fact]
    public async Task GetCommitChangesStreamAsync_SinglePage_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetCommitChanges(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestCommitId);
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetCommitChangesStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, TestConstants.TestCommitId));

        Assert.Single(results);
    }

    [Fact]
    public async Task GetProjectRepositoryTagsStreamAsync_SinglePage_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjectRepositoryTags(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetProjectRepositoryTagsStreamAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug, filterText: "", orderBy: BranchOrderBy.Alphabetical));

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task GetDashboardPullRequestsStreamAsync_SinglePage_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetDashboardPullRequests();
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetDashboardPullRequestsStreamAsync());

        Assert.Single(results);
    }

    [Fact]
    public async Task GetInboxPullRequestsStreamAsync_SinglePage_YieldsAllItems()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetInboxPullRequests();
        var client = _fixture.CreateClient();

        var results = await CollectAsync(client.GetInboxPullRequestsStreamAsync());

        Assert.Single(results);
    }

    #endregion

    private static async Task<List<T>> CollectAsync<T>(IAsyncEnumerable<T> source)
    {
        var list = new List<T>();
        await foreach (var item in source)
        {
            list.Add(item);
        }
        return list;
    }
}