using Bitbucket.Net.Common.Exceptions;
using Bitbucket.Net.Tests.Infrastructure;
using System.Diagnostics;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class TracingMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private const string ApiBasePath = "/rest/api/1.0";
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetProjectAsync_CreatesActivityWithOTelTags()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProject(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();
        var activities = new List<Activity>();

        using var listener = CreateListener(activities);

        await client.GetProjectAsync(TestConstants.TestProjectKey);

        var activity = Assert.Single(activities);
        Assert.Equal("GET", activity.DisplayName);
        Assert.Equal(ActivityKind.Client, activity.Kind);
        Assert.Equal("GET", activity.GetTagItem("http.request.method"));
        Assert.Equal(200, activity.GetTagItem("http.response.status_code"));
        Assert.NotNull(activity.GetTagItem("url.full"));
        Assert.NotNull(activity.GetTagItem("server.address"));
        Assert.Equal(ActivityStatusCode.Unset, activity.Status);
    }

    [Fact]
    public async Task GetProjectAsync_SetsBitbucketProjectKeyTag()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProject(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();
        var activities = new List<Activity>();

        using var listener = CreateListener(activities);

        await client.GetProjectAsync(TestConstants.TestProjectKey);

        var activity = Assert.Single(activities);
        Assert.Equal(TestConstants.TestProjectKey, activity.GetTagItem("bitbucket.project_key"));
    }

    [Fact]
    public async Task GetRepositoryAsync_SetsBitbucketRepoSlugTag()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetRepository(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();
        var activities = new List<Activity>();

        using var listener = CreateListener(activities);

        await client.GetProjectRepositoryAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);

        var activity = Assert.Single(activities);
        Assert.Equal(TestConstants.TestProjectKey, activity.GetTagItem("bitbucket.project_key"));
        Assert.Equal(TestConstants.TestRepositorySlug, activity.GetTagItem("bitbucket.repository_slug"));
    }

    [Fact]
    public async Task ApiCall_WhenErrorStatusCode_SetsActivityStatusToError()
    {
        _fixture.Reset();
        _fixture.Server.SetupNotFound($"{ApiBasePath}/projects/MISSING");
        var client = _fixture.CreateClient();
        var activities = new List<Activity>();

        using var listener = CreateListener(activities);

        await Assert.ThrowsAsync<BitbucketNotFoundException>(
            () => client.GetProjectAsync("MISSING"));

        var activity = Assert.Single(activities);
        Assert.Equal(ActivityStatusCode.Error, activity.Status);
        Assert.Equal(404, activity.GetTagItem("http.response.status_code"));
        Assert.Equal("404", activity.GetTagItem("error.type"));
    }

    [Fact]
    public async Task ApiCall_WhenServerError_SetsActivityStatusToError()
    {
        _fixture.Reset();
        _fixture.Server.SetupInternalServerError($"{ApiBasePath}/projects/{TestConstants.TestProjectKey}");
        var client = _fixture.CreateClient();
        var activities = new List<Activity>();

        using var listener = CreateListener(activities);

        await Assert.ThrowsAsync<BitbucketServerException>(
            () => client.GetProjectAsync(TestConstants.TestProjectKey));

        var activity = Assert.Single(activities);
        Assert.Equal(ActivityStatusCode.Error, activity.Status);
        Assert.Equal(500, activity.GetTagItem("http.response.status_code"));
        Assert.Equal("500", activity.GetTagItem("error.type"));
    }

    [Fact]
    public void NoListeners_ActivitySourceHasNoOverhead()
    {
        var activity = BitbucketClient.ActivitySource.StartActivity("test");
        Assert.Null(activity);
    }

    private ActivityListener CreateListener(List<Activity> activities)
    {
        var baseUrl = _fixture.BaseUrl;
        var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Bitbucket.Net",
            Sample = static (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity =>
            {
                // Filter to only activities targeting our fixture's server to avoid
                // cross-contamination from parallel test classes using BitbucketClient.
                if (activity.GetTagItem("url.full")?.ToString()?.StartsWith(baseUrl) == true)
                {
                    activities.Add(activity);
                }
            },
        };
        ActivitySource.AddActivityListener(listener);
        return listener;
    }
}