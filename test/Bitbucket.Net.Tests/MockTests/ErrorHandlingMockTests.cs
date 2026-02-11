using Bitbucket.Net.Common.Exceptions;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Tests.Infrastructure;
using System.Net;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

/// <summary>
/// Tests that the typed exception hierarchy fires correctly for HTTP
/// error responses.
/// </summary>
public class ErrorHandlingMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private const string ApiBasePath = "/rest/api/1.0";
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetProjectAsync_WhenNotFound_ThrowsException()
    {
        // Arrange
        _fixture.Reset();
        var projectKey = "NONEXISTENT";
        _fixture.Server.SetupNotFound($"{ApiBasePath}/projects/{projectKey}");
        var client = _fixture.CreateClient();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BitbucketNotFoundException>(
            () => client.GetProjectAsync(projectKey));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Contains(projectKey, exception.RequestUrl ?? string.Empty);
    }

    [Fact]
    public async Task GetProjectAsync_WhenUnauthorized_ThrowsException()
    {
        // Arrange
        _fixture.Reset();
        var projectKey = "TEST";
        _fixture.Server.SetupUnauthorized($"{ApiBasePath}/projects/{projectKey}");
        var client = _fixture.CreateClient();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BitbucketAuthenticationException>(
            () => client.GetProjectAsync(projectKey));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    [Fact]
    public async Task GetProjectAsync_WhenServerError_ThrowsException()
    {
        // Arrange
        _fixture.Reset();
        var projectKey = "TEST";
        _fixture.Server.SetupInternalServerError($"{ApiBasePath}/projects/{projectKey}");
        var client = _fixture.CreateClient();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BitbucketServerException>(
            () => client.GetProjectAsync(projectKey));

        Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
    }

    [Fact]
    public async Task GetProjectRepositoryAsync_WhenNotFound_ThrowsException()
    {
        // Arrange
        _fixture.Reset();
        var projectKey = "TEST";
        var repoSlug = "nonexistent-repo";
        _fixture.Server.SetupNotFound($"{ApiBasePath}/projects/{projectKey}/repos/{repoSlug}");
        var client = _fixture.CreateClient();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BitbucketNotFoundException>(
            () => client.GetProjectRepositoryAsync(projectKey, repoSlug));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task GetPullRequestAsync_WhenNotFound_ThrowsException()
    {
        // Arrange
        _fixture.Reset();
        var projectKey = "TEST";
        var repoSlug = "test-repo";
        var pullRequestId = 99999L;
        _fixture.Server.SetupNotFound($"{ApiBasePath}/projects/{projectKey}/repos/{repoSlug}/pull-requests/{pullRequestId}");
        var client = _fixture.CreateClient();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BitbucketNotFoundException>(
            () => client.GetPullRequestAsync(projectKey, repoSlug, pullRequestId));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task GetProjectAsync_WhenForbidden_ThrowsException()
    {
        _fixture.Reset();
        var projectKey = "TEST";
        _fixture.Server.SetupForbidden($"{ApiBasePath}/projects/{projectKey}");
        var client = _fixture.CreateClient();

        var exception = await Assert.ThrowsAsync<BitbucketForbiddenException>(
            () => client.GetProjectAsync(projectKey));

        Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
    }

    [Fact]
    public async Task GetProjectAsync_WhenBadRequest_ThrowsException()
    {
        _fixture.Reset();
        var projectKey = "TEST";
        _fixture.Server.SetupBadRequest($"{ApiBasePath}/projects/{projectKey}");
        var client = _fixture.CreateClient();

        var exception = await Assert.ThrowsAsync<BitbucketBadRequestException>(
            () => client.GetProjectAsync(projectKey));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task GetProjectAsync_WhenConflict_ThrowsException()
    {
        _fixture.Reset();
        var projectKey = "TEST";
        _fixture.Server.SetupConflict($"{ApiBasePath}/projects/{projectKey}");
        var client = _fixture.CreateClient();

        var exception = await Assert.ThrowsAsync<BitbucketConflictException>(
            () => client.GetProjectAsync(projectKey));

        Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
    }

    [Fact]
    public async Task GetProjectAsync_WhenRateLimited_ThrowsException()
    {
        _fixture.Reset();
        var projectKey = "TEST";
        _fixture.Server.SetupRateLimited($"{ApiBasePath}/projects/{projectKey}");
        var client = _fixture.CreateClient();

        var exception = await Assert.ThrowsAsync<BitbucketRateLimitException>(
            () => client.GetProjectAsync(projectKey));

        Assert.Equal(HttpStatusCode.TooManyRequests, exception.StatusCode);
    }

    [Fact]
    public async Task GetProjectAsync_WhenRateLimitedWithHeaders_ExposesAllProperties()
    {
        _fixture.Reset();
        var projectKey = "RATELIMIT";
        _fixture.Server.SetupRateLimitedWithHeaders(
            $"{ApiBasePath}/projects/{projectKey}",
            retryAfter: "30",
            rateLimitLimit: "100",
            rateLimitRemaining: "0",
            rateLimitReset: "1700000000");
        var client = _fixture.CreateClient();

        var exception = await Assert.ThrowsAsync<BitbucketRateLimitException>(
            () => client.GetProjectAsync(projectKey));

        Assert.Equal(HttpStatusCode.TooManyRequests, exception.StatusCode);
        Assert.Equal(TimeSpan.FromSeconds(30), exception.RetryAfter);
        Assert.Equal(100, exception.RateLimit);
        Assert.Equal(0, exception.RateLimitRemaining);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1700000000), exception.RateLimitReset);
    }

    [Fact]
    public async Task GetProjectAsync_WhenRateLimitedWithPartialHeaders_MissingAreNull()
    {
        _fixture.Reset();
        var projectKey = "PARTIALRL";
        _fixture.Server.SetupRateLimitedWithHeaders(
            $"{ApiBasePath}/projects/{projectKey}",
            retryAfter: "60");
        var client = _fixture.CreateClient();

        var exception = await Assert.ThrowsAsync<BitbucketRateLimitException>(
            () => client.GetProjectAsync(projectKey));

        Assert.Equal(TimeSpan.FromSeconds(60), exception.RetryAfter);
        Assert.Null(exception.RateLimit);
        Assert.Null(exception.RateLimitRemaining);
        Assert.Null(exception.RateLimitReset);
    }

    [Fact]
    public async Task GetProjectAsync_WhenRateLimitedWithNoHeaders_PropertiesAreNull()
    {
        _fixture.Reset();
        var projectKey = "NORLHDR";
        _fixture.Server.SetupRateLimited($"{ApiBasePath}/projects/{projectKey}");
        var client = _fixture.CreateClient();

        var exception = await Assert.ThrowsAsync<BitbucketRateLimitException>(
            () => client.GetProjectAsync(projectKey));

        Assert.Null(exception.RetryAfter);
        Assert.Null(exception.RateLimit);
        Assert.Null(exception.RateLimitRemaining);
        Assert.Null(exception.RateLimitReset);
    }

    [Fact]
    public async Task GetProjectAsync_WhenErrorHasJsonBody_PopulatesErrorsAndContext()
    {
        _fixture.Reset();
        var projectKey = "CTX";
        var error = new Error { Context = "projectKey", Message = "Invalid project key", ExceptionName = "TestException" };
        _fixture.Server.SetupErrorWithJsonBody($"{ApiBasePath}/projects/{projectKey}", HttpStatusCode.NotFound, error);
        var client = _fixture.CreateClient();

        var exception = await Assert.ThrowsAsync<BitbucketNotFoundException>(
            () => client.GetProjectAsync(projectKey));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.NotEmpty(exception.Errors);
        Assert.Equal("projectKey", exception.Context);
    }

    [Fact]
    public async Task GetProjectAsync_WhenErrorHasHtmlBody_PreservesMessage()
    {
        _fixture.Reset();
        var projectKey = "HTML";
        var htmlBody = "<html><body>Bad Gateway</body></html>";
        _fixture.Server.SetupErrorWithHtmlBody($"{ApiBasePath}/projects/{projectKey}", HttpStatusCode.BadGateway, htmlBody);
        var client = _fixture.CreateClient();

        var exception = await Assert.ThrowsAsync<BitbucketServerException>(
            () => client.GetProjectAsync(projectKey));

        Assert.Equal(HttpStatusCode.BadGateway, exception.StatusCode);
        Assert.Single(exception.Errors);
        Assert.Contains("Bad Gateway", exception.Errors[0].Message);
    }

    [Fact]
    public async Task GetProjectAsync_WhenErrorHasEmptyBody_UsesEmptyErrors()
    {
        _fixture.Reset();
        var projectKey = "EMPTY";
        _fixture.Server.SetupErrorWithEmptyBody($"{ApiBasePath}/projects/{projectKey}", HttpStatusCode.InternalServerError);
        var client = _fixture.CreateClient();

        var exception = await Assert.ThrowsAsync<BitbucketServerException>(
            () => client.GetProjectAsync(projectKey));

        Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
        Assert.Empty(exception.Errors);
    }
}