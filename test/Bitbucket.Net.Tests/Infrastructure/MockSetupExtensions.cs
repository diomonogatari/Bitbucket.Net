using Bitbucket.Net.Common.Models;
using System.Net;
using System.Text.Json;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Bitbucket.Net.Tests.Infrastructure;

public static class MockSetupExtensions
{
    private const string ApiBasePath = "/rest/api/1.0";
    private const string FixturesBasePath = "Fixtures";

    public static WireMockServer SetupPagedEndpoint(this WireMockServer server, string path, string fixtureCategory, string page1File, string page2File)
    {
        server.Given(Request.Create()
                .WithPath(path)
                .WithParam("start", "0")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath(fixtureCategory, page1File)));

        server.Given(Request.Create()
                .WithPath(path)
                .WithParam("start", "2")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath(fixtureCategory, page2File)));

        return server;
    }

    public static WireMockServer SetupPagedEndpointNoStartParam(this WireMockServer server, string path, string fixtureCategory, string page1File, string page2File)
    {
        server.Given(Request.Create()
                .WithPath(path)
                .WithParam("start", false)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath(fixtureCategory, page1File)));

        server.Given(Request.Create()
                .WithPath(path)
                .WithParam("start", "2")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath(fixtureCategory, page2File)));

        return server;
    }

    public static WireMockServer SetupEmptyPagedEndpoint(this WireMockServer server, string path)
    {
        server.Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "empty-paged.json")));

        return server;
    }

    public static WireMockServer SetupDiffEndpoint(this WireMockServer server, string path, string fixtureFile)
    {
        server.Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", fixtureFile)));

        return server;
    }

    public static WireMockServer SetupGetProjects(this WireMockServer server, int? start = null)
    {
        var request = Request.Create()
            .WithPath($"{ApiBasePath}/projects")
            .UsingGet();

        if (start.HasValue)
        {
            request = request.WithParam("start", start.Value.ToString());
        }

        server.Given(request)
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "projects-list.json")));

        return server;
    }

    public static WireMockServer SetupGetProject(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "project-single.json")));

        return server;
    }

    public static WireMockServer SetupGetRepositories(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "repositories-list.json")));

        return server;
    }

    public static WireMockServer SetupGetRepository(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "repository-single.json")));

        return server;
    }

    public static WireMockServer SetupGetPullRequests(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "pull-requests-list.json")));

        return server;
    }

    public static WireMockServer SetupGetPullRequest(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "pull-request-single.json")));

        return server;
    }

    public static WireMockServer SetupGetPullRequestComments(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/comments")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "pull-request-comments.json")));

        return server;
    }

    public static WireMockServer SetupNotFound(this WireMockServer server, string path)
    {
        server.Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NotFound)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Errors", "error-404.json")));

        return server;
    }

    public static WireMockServer SetupUnauthorized(this WireMockServer server, string path)
    {
        server.Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Unauthorized)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Errors", "error-401.json")));

        return server;
    }

    public static WireMockServer SetupInternalServerError(this WireMockServer server, string path)
    {
        server.Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.InternalServerError)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Errors", "error-500.json")));

        return server;
    }

    public static WireMockServer SetupBadRequest(this WireMockServer server, string path)
    {
        return server.SetupErrorWithJsonBody(path, HttpStatusCode.BadRequest,
            new Error { Message = "Bad request" });
    }

    public static WireMockServer SetupForbidden(this WireMockServer server, string path)
    {
        return server.SetupErrorWithJsonBody(path, HttpStatusCode.Forbidden,
            new Error { Message = "Forbidden" });
    }

    public static WireMockServer SetupConflict(this WireMockServer server, string path)
    {
        return server.SetupErrorWithJsonBody(path, HttpStatusCode.Conflict,
            new Error { Message = "Conflict" });
    }

    public static WireMockServer SetupRateLimited(this WireMockServer server, string path)
    {
        return server.SetupErrorWithJsonBody(path, HttpStatusCode.TooManyRequests,
            new Error { Message = "Rate limit exceeded" });
    }

    public static WireMockServer SetupRateLimitedWithHeaders(
        this WireMockServer server,
        string path,
        string? retryAfter = null,
        string? rateLimitLimit = null,
        string? rateLimitRemaining = null,
        string? rateLimitReset = null)
    {
        var json = JsonSerializer.Serialize(
            new { errors = new[] { new Error { Message = "Rate limit exceeded" } } },
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var responseBuilder = Response.Create()
            .WithStatusCode(HttpStatusCode.TooManyRequests)
            .WithHeader("Content-Type", "application/json")
            .WithBody(json);

        if (retryAfter is not null)
        {
            responseBuilder = responseBuilder.WithHeader("Retry-After", retryAfter);
        }

        if (rateLimitLimit is not null)
        {
            responseBuilder = responseBuilder.WithHeader("X-RateLimit-Limit", rateLimitLimit);
        }

        if (rateLimitRemaining is not null)
        {
            responseBuilder = responseBuilder.WithHeader("X-RateLimit-Remaining", rateLimitRemaining);
        }

        if (rateLimitReset is not null)
        {
            responseBuilder = responseBuilder.WithHeader("X-RateLimit-Reset", rateLimitReset);
        }

        server.Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(responseBuilder);

        return server;
    }

    public static WireMockServer SetupErrorWithJsonBody(this WireMockServer server, string path, HttpStatusCode statusCode, params Error[] errors)
    {
        var json = JsonSerializer.Serialize(new { errors }, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        server.Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(statusCode)
                .WithHeader("Content-Type", "application/json")
                .WithBody(json));

        return server;
    }

    public static WireMockServer SetupErrorWithHtmlBody(this WireMockServer server, string path, HttpStatusCode statusCode, string htmlContent)
    {
        server.Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(statusCode)
                .WithHeader("Content-Type", "text/html")
                .WithBody(htmlContent));

        return server;
    }

    public static WireMockServer SetupErrorWithEmptyBody(this WireMockServer server, string path, HttpStatusCode statusCode)
    {
        server.Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(statusCode));

        return server;
    }

    public static WireMockServer SetupCustomResponse(
        this WireMockServer server,
        string path,
        HttpStatusCode statusCode,
        string fixtureCategory,
        string fixtureFileName)
    {
        server.Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(statusCode)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath(fixtureCategory, fixtureFileName)));

        return server;
    }

    public static WireMockServer SetupGetBranches(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/branches")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "branches-list.json")));

        return server;
    }

    public static WireMockServer SetupGetDefaultBranch(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/branches/default")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "branch-default.json")));

        return server;
    }

    public static WireMockServer SetupGetCommits(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/commits")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "commits-list.json")));

        return server;
    }

    public static WireMockServer SetupGetCommit(this WireMockServer server, string projectKey, string repositorySlug, string commitId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "commit-single.json")));

        return server;
    }

    public static WireMockServer SetupApprovePullRequest(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/approve")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(@"{
                    ""user"": {
                        ""name"": ""testuser"",
                        ""emailAddress"": ""testuser@example.com"",
                        ""id"": 1,
                        ""displayName"": ""Test User"",
                        ""active"": true,
                        ""slug"": ""testuser"",
                        ""type"": ""NORMAL""
                    },
                    ""role"": ""REVIEWER"",
                    ""approved"": true,
                    ""status"": ""APPROVED"",
                    ""lastReviewedCommit"": ""abc123def456789012345678901234567890abcd""
                }"));

        return server;
    }

    public static WireMockServer SetupMergePullRequest(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/merge")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "pull-request-single.json")));

        return server;
    }

    public static WireMockServer SetupDeclinePullRequest(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/decline")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(""));

        return server;
    }

    public static WireMockServer SetupGetPullRequestMergeState(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/merge")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(@"{
                    ""canMerge"": true,
                    ""conflicted"": false,
                    ""outcome"": ""CLEAN"",
                    ""vetoes"": []
                }"));

        return server;
    }

    public static WireMockServer SetupGetChanges(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/changes")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "changes-list.json")));

        return server;
    }

    public static WireMockServer SetupGetCommitChanges(this WireMockServer server, string projectKey, string repositorySlug, string commitId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/changes")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "changes-list.json")));

        return server;
    }

    public static WireMockServer SetupGetFiles(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/files")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "files-list.json")));

        return server;
    }

    public static WireMockServer SetupCreatePullRequest(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "pull-request-single.json")));

        return server;
    }

    public static WireMockServer SetupUpdatePullRequest(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "pull-request-single.json")));

        return server;
    }

    public static WireMockServer SetupGetPullRequestChanges(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/changes")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "changes-list.json")));

        return server;
    }

    public static WireMockServer SetupGetPullRequestCommits(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/commits")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "commits-list.json")));

        return server;
    }

    public static WireMockServer SetupCreateProject(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "project-single.json")));

        return server;
    }

    public static WireMockServer SetupDeleteProject(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupUpdateProject(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "project-single.json")));

        return server;
    }

    public static WireMockServer SetupGetBuildStatsForCommit(this WireMockServer server, string commitId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/build-status/1.0/commits/stats/{commitId}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Builds", "build-stats.json")));

        return server;
    }

    public static WireMockServer SetupGetBuildStatusForCommit(this WireMockServer server, string commitId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/build-status/1.0/commits/{commitId}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Builds", "build-status-list.json")));

        return server;
    }

    public static WireMockServer SetupAssociateBuildStatus(this WireMockServer server, string commitId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/build-status/1.0/commits/{commitId}")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent)
                .WithHeader("Content-Type", "application/json")
                .WithBody(""));

        return server;
    }

    public static WireMockServer SetupGetUsers(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/users")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Users", "users-list.json")));

        return server;
    }

    public static WireMockServer SetupGetUser(this WireMockServer server, string userSlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/users/{userSlug}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Users", "user-single.json")));

        return server;
    }

    public static WireMockServer SetupUpdateUser(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/users")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Users", "user-single.json")));

        return server;
    }

    public static WireMockServer SetupUpdateUserCredentials(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/users/credentials")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent)
                .WithBody(""));

        return server;
    }

    public static WireMockServer SetupDeleteUserAvatar(this WireMockServer server, string userSlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/users/{userSlug}/avatar.png")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent)
                .WithBody(""));

        return server;
    }

    public static WireMockServer SetupGetUserSettings(this WireMockServer server, string userSlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/users/{userSlug}/settings")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(@"{ ""theme"": ""dark"", ""notifications"": true }"));

        return server;
    }

    public static WireMockServer SetupUpdateUserSettings(this WireMockServer server, string userSlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/users/{userSlug}/settings")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent)
                .WithBody(""));

        return server;
    }

    public static WireMockServer SetupGetRepositoryDiff(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/diff")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "diff-response.json")));

        return server;
    }

    public static WireMockServer SetupGetTags(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/tags")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "tags-list.json")));

        return server;
    }

    public static WireMockServer SetupGetCommentsOnFile(this WireMockServer server, string projectKey, string repositorySlug, string commitId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/comments")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "comments-list.json")));

        return server;
    }

    public static WireMockServer SetupGetPullRequestDiff(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/diff")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "diff-response.json")));

        return server;
    }

    public static WireMockServer SetupGetPullRequestActivities(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/activities")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "pull-request-activities.json")));

        return server;
    }

    public static WireMockServer SetupGetPullRequestTasks(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/tasks")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "pull-request-tasks.json")));

        return server;
    }

    public static WireMockServer SetupGetWebhooks(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/webhooks")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Webhooks", "webhooks-list.json")));

        return server;
    }

    public static WireMockServer SetupGetWebhook(this WireMockServer server, string projectKey, string repositorySlug, string webhookId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/webhooks/{webhookId}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Webhooks", "webhook-single.json")));

        return server;
    }

    public static WireMockServer SetupCreateWebhook(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/webhooks")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Webhooks", "webhook-single.json")));

        return server;
    }

    public static WireMockServer SetupDeleteWebhook(this WireMockServer server, string projectKey, string repositorySlug, string webhookId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/webhooks/{webhookId}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent)
                .WithBody(""));

        return server;
    }

    public static WireMockServer SetupGetCompareCommits(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/compare/commits")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "commits-list.json")));

        return server;
    }

    public static WireMockServer SetupGetCompareDiff(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/compare/diff")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "diff-response.json")));

        return server;
    }

    public static WireMockServer SetupGetLastModified(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/last-modified")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(@"{ ""latestCommit"": { ""id"": ""abc123"", ""message"": ""Latest commit"" }, ""files"": {} }"));

        return server;
    }

    public static WireMockServer SetupForkRepository(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "repository-single.json")));

        return server;
    }

    public static WireMockServer SetupUpdateRepository(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "repository-single.json")));

        return server;
    }

    public static WireMockServer SetupDeleteRepository(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent)
                .WithBody(""));

        return server;
    }

    public static WireMockServer SetupCreateRepository(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "repository-single.json")));

        return server;
    }

    public static WireMockServer SetupGetRepositoryForks(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/forks")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "repository-forks.json")));

        return server;
    }

    #region SSH Keys

    public static WireMockServer SetupGetProjectKeysByKeyId(this WireMockServer server, int keyId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/keys/1.0/ssh/{keyId}/projects")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Ssh", "project-keys-list.json")));

        return server;
    }

    public static WireMockServer SetupGetProjectKeysByProject(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/keys/1.0/projects/{projectKey}/ssh")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Ssh", "project-keys-list.json")));

        return server;
    }

    public static WireMockServer SetupCreateProjectKey(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/keys/1.0/projects/{projectKey}/ssh")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Ssh", "project-key-single.json")));

        return server;
    }

    public static WireMockServer SetupGetRepoKeysByKeyId(this WireMockServer server, int keyId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/keys/1.0/ssh/{keyId}/repos")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Ssh", "repo-keys-list.json")));

        return server;
    }

    public static WireMockServer SetupGetRepoKeysByProjectAndRepo(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/keys/1.0/projects/{projectKey}/repos/{repositorySlug}/ssh")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Ssh", "repo-keys-list.json")));

        return server;
    }

    public static WireMockServer SetupCreateRepoKey(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/keys/1.0/projects/{projectKey}/repos/{repositorySlug}/ssh")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Ssh", "repo-key-single.json")));

        return server;
    }

    public static WireMockServer SetupGetUserKeys(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/ssh/1.0/keys")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Ssh", "user-keys-list.json")));

        return server;
    }

    public static WireMockServer SetupGetSshSettings(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/ssh/1.0/settings")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Ssh", "ssh-settings.json")));

        return server;
    }

    public static WireMockServer SetupGetProjectKey(this WireMockServer server, string projectKey, int keyId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/keys/1.0/projects/{projectKey}/ssh/{keyId}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Ssh", "project-key-single.json")));

        return server;
    }

    public static WireMockServer SetupDeleteProjectKey(this WireMockServer server, string projectKey, int keyId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/keys/1.0/projects/{projectKey}/ssh/{keyId}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent)
                .WithBody(""));

        return server;
    }

    public static WireMockServer SetupUpdateProjectKeyPermission(this WireMockServer server, string projectKey, int keyId)
    {
        server.Given(Request.Create()
                .WithPath(new WireMock.Matchers.RegexMatcher($"/rest/keys/1.0/projects/{projectKey}/ssh/{keyId}/permissions/.*"))
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Ssh", "project-key-single.json")));

        return server;
    }

    public static WireMockServer SetupGetRepoKey(this WireMockServer server, string projectKey, string repositorySlug, int keyId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/keys/1.0/projects/{projectKey}/repos/{repositorySlug}/ssh/{keyId}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Ssh", "repo-key-single.json")));

        return server;
    }

    public static WireMockServer SetupDeleteRepoKey(this WireMockServer server, string projectKey, string repositorySlug, int keyId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/keys/1.0/projects/{projectKey}/repos/{repositorySlug}/ssh/{keyId}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent)
                .WithBody(""));

        return server;
    }

    public static WireMockServer SetupUpdateRepoKeyPermission(this WireMockServer server, string projectKey, string repositorySlug, int keyId)
    {
        server.Given(Request.Create()
                .WithPath(new WireMock.Matchers.RegexMatcher($"/rest/keys/1.0/projects/{projectKey}/repos/{repositorySlug}/ssh/{keyId}/permissions/.*"))
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Ssh", "repo-key-single.json")));

        return server;
    }

    public static WireMockServer SetupCreateUserKey(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/ssh/1.0/keys")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Ssh", "user-key-single.json")));

        return server;
    }

    public static WireMockServer SetupDeleteUserKeys(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/ssh/1.0/keys")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent)
                .WithBody(""));

        return server;
    }

    public static WireMockServer SetupDeleteUserKey(this WireMockServer server, int keyId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/ssh/1.0/keys/{keyId}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent)
                .WithBody(""));

        return server;
    }

    public static WireMockServer SetupDeleteProjectsReposKeys(this WireMockServer server, int keyId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/keys/1.0/ssh/{keyId}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent)
                .WithBody(""));

        return server;
    }

    #endregion

    #region Git Operations

    public static WireMockServer SetupGetCanRebasePullRequest(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/git/1.0/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/rebase")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Git", "rebase-condition.json")));

        return server;
    }

    public static WireMockServer SetupRebasePullRequest(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/git/1.0/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/rebase")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "pull-request-single.json")));

        return server;
    }

    public static WireMockServer SetupCreateTag(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/git/1.0/projects/{projectKey}/repos/{repositorySlug}/tags")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Git", "tag-single.json")));

        return server;
    }

    public static WireMockServer SetupDeleteTag(this WireMockServer server, string projectKey, string repositorySlug, string tagName)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/git/1.0/projects/{projectKey}/repos/{repositorySlug}/tags/{tagName}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent)
                .WithBody(""));

        return server;
    }

    #endregion

    #region Default Reviewers

    public static WireMockServer SetupGetDefaultReviewerConditions(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/default-reviewers/1.0/projects/{projectKey}/conditions")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("DefaultReviewers", "reviewer-conditions.json")));

        return server;
    }

    public static WireMockServer SetupGetRepoDefaultReviewerConditions(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/default-reviewers/1.0/projects/{projectKey}/repos/{repositorySlug}/conditions")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("DefaultReviewers", "reviewer-conditions.json")));

        return server;
    }

    public static WireMockServer SetupGetDefaultReviewers(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/default-reviewers/1.0/projects/{projectKey}/repos/{repositorySlug}/reviewers")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("DefaultReviewers", "default-reviewers.json")));

        return server;
    }

    #endregion

    #region Ref Restrictions

    public static WireMockServer SetupGetProjectRefRestrictions(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/branch-permissions/2.0/projects/{projectKey}/restrictions")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("RefRestrictions", "ref-restrictions-list.json")));

        return server;
    }

    public static WireMockServer SetupGetProjectRefRestriction(this WireMockServer server, string projectKey, int restrictionId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/branch-permissions/2.0/projects/{projectKey}/restrictions/{restrictionId}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("RefRestrictions", "ref-restriction-single.json")));

        return server;
    }

    public static WireMockServer SetupCreateProjectRefRestriction(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/branch-permissions/2.0/projects/{projectKey}/restrictions")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("RefRestrictions", "ref-restriction-single.json")));

        return server;
    }

    public static WireMockServer SetupCreateProjectRefRestrictions(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/branch-permissions/2.0/projects/{projectKey}/restrictions")
                .WithHeader("Accept", "application/vnd.atl.bitbucket.bulk+json")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("RefRestrictions", "ref-restrictions-created.json")));

        return server;
    }

    public static WireMockServer SetupDeleteProjectRefRestriction(this WireMockServer server, string projectKey, int restrictionId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/branch-permissions/2.0/projects/{projectKey}/restrictions/{restrictionId}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupGetRepositoryRefRestrictions(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/branch-permissions/2.0/projects/{projectKey}/repos/{repositorySlug}/restrictions")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("RefRestrictions", "ref-restrictions-list.json")));

        return server;
    }

    public static WireMockServer SetupGetRepositoryRefRestriction(this WireMockServer server, string projectKey, string repositorySlug, int restrictionId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/branch-permissions/2.0/projects/{projectKey}/repos/{repositorySlug}/restrictions/{restrictionId}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("RefRestrictions", "ref-restriction-single.json")));

        return server;
    }

    public static WireMockServer SetupCreateRepositoryRefRestriction(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/branch-permissions/2.0/projects/{projectKey}/repos/{repositorySlug}/restrictions")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("RefRestrictions", "ref-restriction-single.json")));

        return server;
    }

    public static WireMockServer SetupCreateRepositoryRefRestrictions(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/branch-permissions/2.0/projects/{projectKey}/repos/{repositorySlug}/restrictions")
                .WithHeader("Accept", "application/vnd.atl.bitbucket.bulk+json")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("RefRestrictions", "ref-restrictions-created.json")));

        return server;
    }

    public static WireMockServer SetupDeleteRepositoryRefRestriction(this WireMockServer server, string projectKey, string repositorySlug, int restrictionId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/branch-permissions/2.0/projects/{projectKey}/repos/{repositorySlug}/restrictions/{restrictionId}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    #endregion

    #region Personal Access Tokens

    public static WireMockServer SetupGetUserAccessTokens(this WireMockServer server, string userSlug)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/access-tokens/1.0/users/{userSlug}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PersonalAccessTokens", "access-tokens-list.json")));

        return server;
    }

    public static WireMockServer SetupGetUserAccessToken(this WireMockServer server, string userSlug, string tokenId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/access-tokens/1.0/users/{userSlug}/{tokenId}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PersonalAccessTokens", "access-token-single.json")));

        return server;
    }

    public static WireMockServer SetupCreateAccessToken(this WireMockServer server, string userSlug)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/access-tokens/1.0/users/{userSlug}")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PersonalAccessTokens", "access-token-created.json")));

        return server;
    }

    public static WireMockServer SetupChangeUserAccessToken(this WireMockServer server, string userSlug, string tokenId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/access-tokens/1.0/users/{userSlug}/{tokenId}")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PersonalAccessTokens", "access-token-single.json")));

        return server;
    }

    public static WireMockServer SetupDeleteUserAccessToken(this WireMockServer server, string userSlug, string tokenId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/access-tokens/1.0/users/{userSlug}/{tokenId}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    #endregion

    #region Audit

    public static WireMockServer SetupGetProjectAuditEvents(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/audit/1.0/projects/{projectKey}/events")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Audit", "audit-events.json")));

        return server;
    }

    public static WireMockServer SetupGetProjectRepoAuditEvents(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/audit/1.0/projects/{projectKey}/repos/{repositorySlug}/events")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Audit", "audit-events.json")));

        return server;
    }

    #endregion

    #region RefSync

    public static WireMockServer SetupGetRepositorySynchronizationStatus(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/sync/1.0/projects/{projectKey}/repos/{repositorySlug}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("RefSync", "repository-sync-status.json")));

        return server;
    }

    public static WireMockServer SetupEnableRepositorySynchronization(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/sync/1.0/projects/{projectKey}/repos/{repositorySlug}")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("RefSync", "repository-sync-status.json")));

        return server;
    }

    public static WireMockServer SetupSynchronizeRepository(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/sync/1.0/projects/{projectKey}/repos/{repositorySlug}")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("RefSync", "sync-result.json")));

        return server;
    }

    #endregion

    #region CommentLikes

    public static WireMockServer SetupGetCommitCommentLikes(this WireMockServer server, string projectKey, string repositorySlug, string commitId, string commentId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/comment-likes/1.0/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/comments/{commentId}/likes")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("CommentLikes", "comment-likes-list.json")));

        return server;
    }

    public static WireMockServer SetupLikeCommitComment(this WireMockServer server, string projectKey, string repositorySlug, string commitId, string commentId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/comment-likes/1.0/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/comments/{commentId}/likes")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupUnlikeCommitComment(this WireMockServer server, string projectKey, string repositorySlug, string commitId, string commentId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/comment-likes/1.0/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/comments/{commentId}/likes")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupGetPullRequestCommentLikes(this WireMockServer server, string projectKey, string repositorySlug, string pullRequestId, string commentId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/comment-likes/1.0/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/comments/{commentId}/likes")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("CommentLikes", "comment-likes-list.json")));

        return server;
    }

    public static WireMockServer SetupLikePullRequestComment(this WireMockServer server, string projectKey, string repositorySlug, string pullRequestId, string commentId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/comment-likes/1.0/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/comments/{commentId}/likes")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupUnlikePullRequestComment(this WireMockServer server, string projectKey, string repositorySlug, string pullRequestId, string commentId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/comment-likes/1.0/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/comments/{commentId}/likes")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    #endregion

    #region Jira

    public static WireMockServer SetupGetJiraIssues(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/jira/1.0/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/issues")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Jira", "jira-issues-list.json")));

        return server;
    }

    public static WireMockServer SetupCreateJiraIssue(this WireMockServer server, string pullRequestCommentId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/jira/1.0/comments/{pullRequestCommentId}/issues")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Jira", "jira-issue-created.json")));

        return server;
    }

    public static WireMockServer SetupGetChangeSets(this WireMockServer server, string issueKey)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/jira/1.0/issues/{issueKey}/commits")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Jira", "changesets-list.json")));

        return server;
    }

    #endregion

    #region Dashboard

    public static WireMockServer SetupGetDashboardPullRequests(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/dashboard/pull-requests")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Dashboard", "pull-requests.json")));

        return server;
    }

    public static WireMockServer SetupGetDashboardPullRequestSuggestions(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/dashboard/pull-request-suggestions")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Dashboard", "pull-request-suggestions.json")));

        return server;
    }

    #endregion

    #region Inbox

    public static WireMockServer SetupGetInboxPullRequests(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/inbox/pull-requests")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Inbox", "pull-requests.json")));

        return server;
    }

    public static WireMockServer SetupGetInboxPullRequestsCount(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/inbox/pull-requests/count")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Inbox", "pull-requests-count.json")));

        return server;
    }

    #endregion

    #region Profile

    public static WireMockServer SetupGetRecentRepos(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/profile/recent/repos")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Profile", "recent-repos.json")));

        return server;
    }

    #endregion

    #region Markup

    public static WireMockServer SetupPreviewMarkup(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/markup/preview")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Markup", "preview-result.json")));

        return server;
    }

    #endregion

    #region Admin

    public static WireMockServer SetupGetAdminGroups(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/groups")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "groups.json")));

        return server;
    }

    public static WireMockServer SetupCreateAdminGroup(this WireMockServer server, string groupName)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/groups")
                .WithParam("name", groupName)
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "group.json")));

        return server;
    }

    public static WireMockServer SetupDeleteAdminGroup(this WireMockServer server, string groupName)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/groups")
                .WithParam("name", groupName)
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "group.json")));

        return server;
    }

    public static WireMockServer SetupGetAdminUsers(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/users")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "users.json")));

        return server;
    }

    public static WireMockServer SetupDeleteAdminUser(this WireMockServer server, string userName)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/users")
                .WithParam("name", userName)
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "user.json")));

        return server;
    }

    public static WireMockServer SetupGetAdminCluster(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/cluster")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "cluster.json")));

        return server;
    }

    public static WireMockServer SetupGetAdminLicense(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/license")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "license.json")));

        return server;
    }

    public static WireMockServer SetupGetAdminGroupPermissions(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/permissions/groups")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "group-permissions.json")));

        return server;
    }

    public static WireMockServer SetupGetAdminUserPermissions(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/permissions/users")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "user-permissions.json")));

        return server;
    }

    public static WireMockServer SetupGetAdminGroupMoreMembers(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/groups/more-members")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "users.json")));

        return server;
    }

    public static WireMockServer SetupGetAdminGroupMoreNonMembers(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/groups/more-non-members")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "users.json")));

        return server;
    }

    public static WireMockServer SetupGetAdminUserMoreMembers(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/users/more-members")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "more-members.json")));

        return server;
    }

    public static WireMockServer SetupGetAdminUserMoreNonMembers(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/users/more-non-members")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "more-members.json")));

        return server;
    }

    #endregion

    #region Tasks

    public static WireMockServer SetupCreateTask(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/tasks")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Tasks", "task.json")));

        return server;
    }

    public static WireMockServer SetupGetTask(this WireMockServer server, long taskId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/api/1.0/tasks/{taskId}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Tasks", "task.json")));

        return server;
    }

    public static WireMockServer SetupUpdateTask(this WireMockServer server, long taskId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/api/1.0/tasks/{taskId}")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Tasks", "task.json")));

        return server;
    }

    public static WireMockServer SetupDeleteTask(this WireMockServer server, long taskId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/api/1.0/tasks/{taskId}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    #endregion

    #region Groups

    public static WireMockServer SetupGetGroupNames(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/groups")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Groups", "groups.json")));

        return server;
    }

    #endregion

    #region Logs

    public static WireMockServer SetupGetLogLevel(this WireMockServer server, string loggerName)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/api/1.0/logs/logger/{loggerName}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Logs", "log-level.json")));

        return server;
    }

    public static WireMockServer SetupSetLogLevel(this WireMockServer server, string loggerName, string logLevel)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/api/1.0/logs/logger/{loggerName}/{logLevel}")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupGetRootLogLevel(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/logs/logger/rootLogger")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Logs", "log-level.json")));

        return server;
    }

    public static WireMockServer SetupSetRootLogLevel(this WireMockServer server, string logLevel)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/api/1.0/logs/logger/rootLogger/{logLevel}")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    #endregion

    #region WhoAmI

    public static WireMockServer SetupGetWhoAmI(this WireMockServer server, string username)
    {
        server.Given(Request.Create()
                .WithPath("/plugins/servlet/applinks/whoami")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "text/plain")
                .WithBody(username));

        return server;
    }

    #endregion

    #region Project Permissions

    public static WireMockServer SetupGetProjectUserPermissions(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/permissions/users")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Permissions", "user-permissions.json")));

        return server;
    }

    public static WireMockServer SetupDeleteProjectUserPermissions(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/permissions/users")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupUpdateProjectUserPermissions(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/permissions/users")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupGetProjectUserPermissionsNone(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/permissions/users/none")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Permissions", "licensed-users.json")));

        return server;
    }

    public static WireMockServer SetupGetProjectGroupPermissions(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/permissions/groups")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Permissions", "group-permissions.json")));

        return server;
    }

    public static WireMockServer SetupDeleteProjectGroupPermissions(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/permissions/groups")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupUpdateProjectGroupPermissions(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/permissions/groups")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupGetProjectGroupPermissionsNone(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/permissions/groups/none")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Permissions", "licensed-users.json")));

        return server;
    }

    public static WireMockServer SetupGetProjectDefaultPermission(this WireMockServer server, string projectKey, string permission)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/permissions/{permission}/all")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Permissions", "default-permission.json")));

        return server;
    }

    public static WireMockServer SetupSetProjectDefaultPermission(this WireMockServer server, string projectKey, string permission)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/permissions/{permission}/all")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    #endregion

    #region Repository Permissions

    public static WireMockServer SetupGetRepositoryUserPermissions(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/permissions/users")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Permissions", "repo-user-permissions.json")));

        return server;
    }

    public static WireMockServer SetupUpdateRepositoryUserPermissions(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/permissions/users")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupDeleteRepositoryUserPermissions(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/permissions/users")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupGetRepositoryUserPermissionsNone(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/permissions/users/none")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Users", "users-list.json")));

        return server;
    }

    public static WireMockServer SetupGetRepositoryGroupPermissions(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/permissions/groups")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Permissions", "repo-group-permissions.json")));

        return server;
    }

    public static WireMockServer SetupUpdateRepositoryGroupPermissions(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/permissions/groups")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupDeleteRepositoryGroupPermissions(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/permissions/groups")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupGetRepositoryGroupPermissionsNone(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/permissions/groups/none")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Permissions", "deletable-groups-users.json")));

        return server;
    }

    #endregion

    #region Pull Request Operations

    public static WireMockServer SetupDeletePullRequest(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupReopenPullRequest(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/reopen")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "pull-request-single.json")));

        return server;
    }

    public static WireMockServer SetupDeletePullRequestApproval(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/approve")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "reviewer-unapproved.json")));

        return server;
    }

    public static WireMockServer SetupGetPullRequestParticipants(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/participants")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "participants.json")));

        return server;
    }

    public static WireMockServer SetupAssignUserRoleToPullRequest(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/participants")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "participant.json")));

        return server;
    }

    public static WireMockServer SetupDeletePullRequestParticipant(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/participants")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupUnassignUserFromPullRequest(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId, string userSlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/participants/{userSlug}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupWatchPullRequest(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/watch")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupUnwatchPullRequest(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/watch")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupGetPullRequestTaskCount(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/tasks/count")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "task-count.json")));

        return server;
    }

    public static WireMockServer SetupGetPullRequestBlockerComments(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/blocker-comments")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "blocker-comments.json")));

        return server;
    }

    public static WireMockServer SetupGetPullRequestBlockerComment(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId, long blockerCommentId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/blocker-comments/{blockerCommentId}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "blocker-comment.json")));

        return server;
    }

    public static WireMockServer SetupCreatePullRequestBlockerComment(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/blocker-comments")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "blocker-comment.json")));

        return server;
    }

    public static WireMockServer SetupDeletePullRequestBlockerComment(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId, long blockerCommentId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/blocker-comments/{blockerCommentId}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupGetPullRequestMergeBase(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/merge-base")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "commit-single.json")));

        return server;
    }

    #endregion

    #region Branches

    public static WireMockServer SetupCreateBranch(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/branches")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Branches", "branch-created.json")));

        return server;
    }

    public static WireMockServer SetupSetDefaultBranch(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/branches")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    #endregion

    #region Pull Request Comment Operations

    public static WireMockServer SetupCreatePullRequestComment(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/comments")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "comment-created.json")));

        return server;
    }

    public static WireMockServer SetupUpdatePullRequestComment(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId, long commentId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/comments/{commentId}")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "comment-created.json")));

        return server;
    }

    public static WireMockServer SetupDeletePullRequestComment(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId, long commentId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/comments/{commentId}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupGetPullRequestComment(this WireMockServer server, string projectKey, string repositorySlug, long pullRequestId, long commentId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/comments/{commentId}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "comment-created.json")));

        return server;
    }

    #endregion

    #region Extended Admin Operations

    public static WireMockServer SetupCreateAdminUser(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/users")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupUpdateAdminUser(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/users")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "user.json")));

        return server;
    }

    public static WireMockServer SetupAddAdminGroupUsers(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/groups/add-users")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupGetAdminGroupMembers(this WireMockServer server, string context)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/groups/more-members")
                .WithParam("context", context)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "group-users.json")));

        return server;
    }

    public static WireMockServer SetupGetAdminGroupNonMembers(this WireMockServer server, string context)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/groups/more-non-members")
                .WithParam("context", context)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "group-users.json")));

        return server;
    }

    public static WireMockServer SetupAddAdminUserGroups(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/users/add-groups")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupRemoveAdminUserFromGroup(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/users/remove-group")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupDeleteAdminUserCaptcha(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/users/captcha")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupUpdateAdminUserCredentials(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/users/credentials")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupGetAdminMailServer(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/mail-server")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "mail-server.json")));

        return server;
    }

    public static WireMockServer SetupUpdateAdminMailServer(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/mail-server")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "mail-server.json")));

        return server;
    }

    public static WireMockServer SetupDeleteAdminMailServer(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/mail-server")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupGetAdminMailServerSenderAddress(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/mail-server/sender-address")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "text/plain")
                .WithBody("bitbucket@example.com"));

        return server;
    }

    public static WireMockServer SetupUpdateAdminMailServerSenderAddress(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/mail-server/sender-address")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "text/plain")
                .WithBody("new-sender@example.com"));

        return server;
    }

    public static WireMockServer SetupDeleteAdminMailServerSenderAddress(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/mail-server/sender-address")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupUpdateAdminGroupPermissions(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/permissions/groups")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupDeleteAdminGroupPermissions(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/permissions/groups")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupGetAdminGroupPermissionsNone(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/permissions/groups/none")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "groups.json")));

        return server;
    }

    public static WireMockServer SetupUpdateAdminUserPermissions(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/permissions/users")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupDeleteAdminUserPermissions(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/permissions/users")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupGetAdminUserPermissionsNone(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/permissions/users/none")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Users", "users-list.json")));

        return server;
    }

    public static WireMockServer SetupGetAdminMergeStrategies(this WireMockServer server, string scmId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/api/1.0/admin/pull-requests/{scmId}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "merge-strategies.json")));

        return server;
    }

    public static WireMockServer SetupUpdateAdminMergeStrategies(this WireMockServer server, string scmId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/api/1.0/admin/pull-requests/{scmId}")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "merge-strategies.json")));

        return server;
    }

    public static WireMockServer SetupUpdateAdminLicense(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/license")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "license.json")));

        return server;
    }

    public static WireMockServer SetupRenameAdminUser(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/api/1.0/admin/users/rename")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Admin", "user.json")));

        return server;
    }

    #endregion

    #region Extended Branch Operations

    public static WireMockServer SetupGetCommitBranchInfo(this WireMockServer server, string projectKey, string repositorySlug, string commitId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/branch-utils/1.0/projects/{projectKey}/repos/{repositorySlug}/branches/info/{commitId}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Branches", "commit-branch-info.json")));

        return server;
    }

    public static WireMockServer SetupGetBranchModel(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/branch-utils/1.0/projects/{projectKey}/repos/{repositorySlug}/branchmodel")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Branches", "branch-model.json")));

        return server;
    }

    public static WireMockServer SetupCreateRepoBranch(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/branch-utils/1.0/projects/{projectKey}/repos/{repositorySlug}/branches")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Branches", "branch-created.json")));

        return server;
    }

    public static WireMockServer SetupDeleteRepoBranch(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/branch-utils/1.0/projects/{projectKey}/repos/{repositorySlug}/branches")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    #endregion

    #region Extended Core Operations

    public static WireMockServer SetupBrowseRepository(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/browse")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "browse-item.json")));

        return server;
    }

    public static WireMockServer SetupGetCompareChanges(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/compare/changes")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "changes-list.json")));

        return server;
    }

    public static WireMockServer SetupGetRepositoryParticipants(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/participants")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Users", "users-list.json")));

        return server;
    }

    public static WireMockServer SetupGetCommitDiff(this WireMockServer server, string projectKey, string repositorySlug, string commitId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/diff")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "diff-response.json")));

        return server;
    }

    public static WireMockServer SetupCreateCommitWatch(this WireMockServer server, string projectKey, string repositorySlug, string commitId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/watch")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupDeleteCommitWatch(this WireMockServer server, string projectKey, string repositorySlug, string commitId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/watch")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupCreateCommitComment(this WireMockServer server, string projectKey, string repositorySlug, string commitId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/comments")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "comment-created.json")));

        return server;
    }

    public static WireMockServer SetupGetCommitComment(this WireMockServer server, string projectKey, string repositorySlug, string commitId, long commentId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/comments/{commentId}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "comment-created.json")));

        return server;
    }

    public static WireMockServer SetupUpdateCommitComment(this WireMockServer server, string projectKey, string repositorySlug, string commitId, long commentId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/comments/{commentId}")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("PullRequests", "comment-created.json")));

        return server;
    }

    public static WireMockServer SetupDeleteCommitComment(this WireMockServer server, string projectKey, string repositorySlug, string commitId, long commentId)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/comments/{commentId}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    #endregion

    #region Extended Builds Operations

    public static WireMockServer SetupGetBuildStatsForMultipleCommits(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath("/rest/build-status/1.0/commits/stats")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Builds", "build-stats-multiple.json")));

        return server;
    }

    #endregion

    #region Extended DefaultReviewers Operations

    public static WireMockServer SetupCreateDefaultReviewerCondition(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/default-reviewers/1.0/projects/{projectKey}/conditions")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("DefaultReviewers", "condition-single.json")));

        return server;
    }

    public static WireMockServer SetupUpdateDefaultReviewerCondition(this WireMockServer server, string projectKey, string conditionId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/default-reviewers/1.0/projects/{projectKey}/conditions/{conditionId}")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("DefaultReviewers", "condition-single.json")));

        return server;
    }

    public static WireMockServer SetupDeleteDefaultReviewerCondition(this WireMockServer server, string projectKey, string conditionId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/default-reviewers/1.0/projects/{projectKey}/conditions/{conditionId}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    public static WireMockServer SetupCreateRepoDefaultReviewerCondition(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/default-reviewers/1.0/projects/{projectKey}/repos/{repositorySlug}/conditions")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("DefaultReviewers", "condition-single.json")));

        return server;
    }

    public static WireMockServer SetupUpdateRepoDefaultReviewerCondition(this WireMockServer server, string projectKey, string repositorySlug, string conditionId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/default-reviewers/1.0/projects/{projectKey}/repos/{repositorySlug}/conditions/{conditionId}")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("DefaultReviewers", "condition-single.json")));

        return server;
    }

    public static WireMockServer SetupDeleteRepoDefaultReviewerCondition(this WireMockServer server, string projectKey, string repositorySlug, string conditionId)
    {
        server.Given(Request.Create()
                .WithPath($"/rest/default-reviewers/1.0/projects/{projectKey}/repos/{repositorySlug}/conditions/{conditionId}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }

    #endregion

    #region Hooks Operations

    public static WireMockServer SetupGetProjectHooksAvatar(this WireMockServer server, string hookKey)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/hooks/{hookKey}/avatar")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "image/png")
                .WithBody([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]));

        return server;
    }

    #endregion

    #region Repository Operations (Extended)

    public static WireMockServer SetupRecreateProjectRepository(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/recreate")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "repository-single.json")));

        return server;
    }

    public static WireMockServer SetupGetRelatedProjectRepositories(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/related")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "repository-forks.json")));

        return server;
    }

    public static WireMockServer SetupGetProjectRepositoryArchive(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/archive")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/zip")
                .WithBody([0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x00, 0x00]));

        return server;
    }

    public static WireMockServer SetupGetProjectRepositoryPullRequestSettings(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/settings/pull-requests")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(@"{""requiredApprovers"":2,""requiredSuccessfulBuilds"":1,""requiredAllApprovers"":false,""requiredAllTasksComplete"":true,""mergeConfig"":{""defaultStrategy"":{""id"":""no-ff""},""strategies"":[{""id"":""ff""},{""id"":""no-ff""}]}}"));

        return server;
    }

    public static WireMockServer SetupUpdateProjectRepositoryPullRequestSettings(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/settings/pull-requests")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(@"{""requiredApprovers"":2,""requiredSuccessfulBuilds"":1,""requiredAllApprovers"":false,""requiredAllTasksComplete"":true}"));

        return server;
    }

    public static WireMockServer SetupGetProjectRepositoryHooksSettings(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/settings/hooks")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Hooks", "hooks-list.json")));

        return server;
    }

    public static WireMockServer SetupEnableProjectRepositoryHook(this WireMockServer server, string projectKey, string repositorySlug, string hookKey)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/settings/hooks/{hookKey}/enabled")
                .UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Hooks", "hook-single.json")));

        return server;
    }

    public static WireMockServer SetupDisableProjectRepositoryHook(this WireMockServer server, string projectKey, string repositorySlug, string hookKey)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/settings/hooks/{hookKey}/enabled")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Hooks", "hook-single.json")));

        return server;
    }

    public static WireMockServer SetupGetProjectPullRequestsMergeStrategies(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath(new WireMock.Matchers.RegexMatcher($"{ApiBasePath}/projects/{projectKey}/settings/pull-requests/.*"))
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(@"{""requiredApprovers"":2,""requiredSuccessfulBuilds"":1,""mergeConfig"":{""defaultStrategy"":{""id"":""no-ff""},""strategies"":[{""id"":""ff""},{""id"":""no-ff""}]}}"));

        return server;
    }

    public static WireMockServer SetupUpdateProjectPullRequestsMergeStrategies(this WireMockServer server, string projectKey)
    {
        server.Given(Request.Create()
                .WithPath(new WireMock.Matchers.RegexMatcher($"{ApiBasePath}/projects/{projectKey}/settings/pull-requests/.*"))
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(@"{""defaultStrategy"":{""id"":""no-ff""},""strategies"":[{""id"":""ff""},{""id"":""no-ff""}]}"));

        return server;
    }

    public static WireMockServer SetupBrowseProjectRepositoryPath(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath(new WildcardMatcher($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/browse/*", ignoreCase: true))
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "browse-result.json")));

        return server;
    }

    public static WireMockServer SetupGetRawFileContentStream(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath(new WildcardMatcher($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/raw/*", ignoreCase: true))
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/octet-stream")
                .WithBody("# README\n\nThis is a sample README file."));

        return server;
    }

    public static WireMockServer SetupGetProjectRepositoryTags(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/tags")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "tags-list.json")));

        return server;
    }

    public static WireMockServer SetupCreateProjectRepositoryTag(this WireMockServer server, string projectKey, string repositorySlug)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/tags")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Created)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Git", "tag-single.json")));

        return server;
    }

    public static WireMockServer SetupDeleteProjectRepositoryTag(this WireMockServer server, string projectKey, string repositorySlug, string tagName)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/projects/{projectKey}/repos/{repositorySlug}/tags/{tagName}")
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.NoContent)
                .WithBody(""));

        return server;
    }

    #endregion

    #region Application Properties

    public static WireMockServer SetupGetApplicationProperties(this WireMockServer server)
    {
        server.Given(Request.Create()
                .WithPath($"{ApiBasePath}/application-properties")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Core", "application-properties.json")));

        return server;
    }

    #endregion

    #region Code Search

    private const string SearchBasePath = "/rest/search/latest";

    public static WireMockServer SetupSearchCode(this WireMockServer server, string fixtureFile)
    {
        server.Given(Request.Create()
                .WithPath($"{SearchBasePath}/search")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile(GetFixturePath("Search", fixtureFile)));

        return server;
    }

    public static WireMockServer SetupSearchCodeError(this WireMockServer server, HttpStatusCode statusCode)
    {
        server.Given(Request.Create()
                .WithPath($"{SearchBasePath}/search")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(statusCode)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"errors":[{"context":null,"message":"Search is not available","exceptionName":"com.atlassian.bitbucket.search.SearchUnavailableException"}]}"""));

        return server;
    }

    #endregion

    private static string GetFixturePath(string category, string fileName)
    {
        return Path.Combine(FixturesBasePath, category, fileName);
    }
}