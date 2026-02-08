using Bitbucket.Net.Common.Exceptions;
using Bitbucket.Net.Tests.Infrastructure;
using System.Net;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class DiConstructorMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private const string ApiBasePath = "/rest/api/1.0";
    private readonly BitbucketMockFixture _fixture = fixture;

    #region HttpClient Constructor

    [Fact]
    public async Task HttpClientConstructor_GetProjects_Succeeds()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjects();
        var client = _fixture.CreateClientWithHttpClient();

        var projects = await client.GetProjectsAsync();

        Assert.NotNull(projects);
        var projectList = projects.ToList();
        Assert.Single(projectList);
        Assert.Equal(TestConstants.TestProjectKey, projectList[0].Key);
    }

    [Fact]
    public async Task HttpClientConstructor_GetPullRequests_Succeeds()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequests(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClientWithHttpClient();

        var pullRequests = await client.GetPullRequestsAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);

        Assert.NotNull(pullRequests);
        Assert.Single(pullRequests);
    }

    [Fact]
    public async Task HttpClientConstructor_StreamingEndpoint_Succeeds()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjects();
        var client = _fixture.CreateClientWithHttpClient();

        var results = new List<Models.Core.Projects.Project>();
        await foreach (var project in client.GetProjectsStreamAsync())
        {
            results.Add(project);
        }

        Assert.Single(results);
    }

    [Fact]
    public async Task HttpClientConstructor_ErrorHandling_ThrowsTypedException()
    {
        _fixture.Reset();
        var projectKey = "NOPE";
        _fixture.Server.SetupNotFound($"{ApiBasePath}/projects/{projectKey}");
        var client = _fixture.CreateClientWithHttpClient();

        var exception = await Assert.ThrowsAsync<BitbucketNotFoundException>(
            () => client.GetProjectAsync(projectKey));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task HttpClientConstructor_Unauthorized_ThrowsAuthenticationException()
    {
        _fixture.Reset();
        _fixture.Server.SetupUnauthorized($"{ApiBasePath}/projects/{TestConstants.TestProjectKey}");
        var client = _fixture.CreateClientWithHttpClient();

        var exception = await Assert.ThrowsAsync<BitbucketAuthenticationException>(
            () => client.GetProjectAsync(TestConstants.TestProjectKey));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    [Fact]
    public async Task HttpClientConstructor_ServerError_ThrowsServerException()
    {
        _fixture.Reset();
        _fixture.Server.SetupInternalServerError($"{ApiBasePath}/projects/{TestConstants.TestProjectKey}");
        var client = _fixture.CreateClientWithHttpClient();

        var exception = await Assert.ThrowsAsync<BitbucketServerException>(
            () => client.GetProjectAsync(TestConstants.TestProjectKey));

        Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
    }

    #endregion

    #region FlurlClient Constructor

    [Fact]
    public async Task FlurlClientConstructor_GetProjects_Succeeds()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjects();
        var client = _fixture.CreateClientWithFlurlClient();

        var projects = await client.GetProjectsAsync();

        Assert.NotNull(projects);
        var projectList = projects.ToList();
        Assert.Single(projectList);
        Assert.Equal(TestConstants.TestProjectKey, projectList[0].Key);
    }

    [Fact]
    public async Task FlurlClientConstructor_GetPullRequests_Succeeds()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequests(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClientWithFlurlClient();

        var pullRequests = await client.GetPullRequestsAsync(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);

        Assert.NotNull(pullRequests);
        Assert.Single(pullRequests);
    }

    [Fact]
    public async Task FlurlClientConstructor_StreamingEndpoint_Succeeds()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjects();
        var client = _fixture.CreateClientWithFlurlClient();

        var results = new List<Models.Core.Projects.Project>();
        await foreach (var project in client.GetProjectsStreamAsync())
        {
            results.Add(project);
        }

        Assert.Single(results);
    }

    [Fact]
    public async Task FlurlClientConstructor_ErrorHandling_ThrowsTypedException()
    {
        _fixture.Reset();
        var projectKey = "NOPE";
        _fixture.Server.SetupNotFound($"{ApiBasePath}/projects/{projectKey}");
        var client = _fixture.CreateClientWithFlurlClient();

        var exception = await Assert.ThrowsAsync<BitbucketNotFoundException>(
            () => client.GetProjectAsync(projectKey));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task FlurlClientConstructor_Unauthorized_ThrowsAuthenticationException()
    {
        _fixture.Reset();
        _fixture.Server.SetupUnauthorized($"{ApiBasePath}/projects/{TestConstants.TestProjectKey}");
        var client = _fixture.CreateClientWithFlurlClient();

        var exception = await Assert.ThrowsAsync<BitbucketAuthenticationException>(
            () => client.GetProjectAsync(TestConstants.TestProjectKey));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    #endregion

    #region Both Constructors — Consistent Behavior

    [Fact]
    public async Task BothConstructors_SameEndpoint_ReturnEquivalentResults()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjects();
        var httpClientBasedClient = _fixture.CreateClientWithHttpClient();
        var flurlBasedClient = _fixture.CreateClientWithFlurlClient();

        var httpResults = (await httpClientBasedClient.GetProjectsAsync()).ToList();

        _fixture.Reset();
        _fixture.Server.SetupGetProjects();
        var flurlResults = (await flurlBasedClient.GetProjectsAsync()).ToList();

        Assert.Equal(httpResults.Count, flurlResults.Count);
        Assert.Equal(httpResults[0].Key, flurlResults[0].Key);
        Assert.Equal(httpResults[0].Name, flurlResults[0].Name);
    }

    #endregion

    #region Token Authentication Verification

    [Fact]
    public async Task HttpClientConstructor_SendsAuthorizationHeader()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjects();
        var client = _fixture.CreateClientWithHttpClient();

        await client.GetProjectsAsync();

        var logEntry = Assert.Single(_fixture.Server.LogEntries);
        var authHeader = logEntry.RequestMessage.Headers?["Authorization"];
        Assert.NotNull(authHeader);
        Assert.Contains("Bearer test-token", authHeader.ToString());
    }

    [Fact]
    public async Task FlurlClientConstructor_SendsAuthorizationHeader()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjects();
        var client = _fixture.CreateClientWithFlurlClient();

        await client.GetProjectsAsync();

        var logEntry = Assert.Single(_fixture.Server.LogEntries);
        var authHeader = logEntry.RequestMessage.Headers?["Authorization"];
        Assert.NotNull(authHeader);
        Assert.Contains("Bearer test-token", authHeader.ToString());
    }

    #endregion
}