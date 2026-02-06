using System.Net;
using System.Threading.Tasks;
using Bitbucket.Net.Tests.Infrastructure;
using Flurl.Http;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    /// <summary>
    /// Unit tests for error handling using WireMock.
    /// Verifies that appropriate exceptions are thrown for HTTP error responses.
    /// </summary>
    /// <remarks>
    /// NOTE: The current library implementation throws FlurlHttpException directly
    /// rather than the documented BitbucketApiException types. This is because
    /// Flurl throws before the custom error handling can intercept the response.
    /// These tests verify the actual current behavior.
    /// </remarks>
    public class ErrorHandlingMockTests : IClassFixture<BitbucketMockFixture>
    {
        private const string ApiBasePath = "/rest/api/1.0";
        private readonly BitbucketMockFixture _fixture;

        public ErrorHandlingMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetProjectAsync_WhenNotFound_ThrowsException()
        {
            // Arrange
            _fixture.Reset();
            var projectKey = "NONEXISTENT";
            _fixture.Server.SetupNotFound($"{ApiBasePath}/projects/{projectKey}");
            var client = _fixture.CreateClient();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<FlurlHttpException>(
                () => client.GetProjectAsync(projectKey));

            Assert.Equal((int)HttpStatusCode.NotFound, exception.StatusCode);
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
            var exception = await Assert.ThrowsAsync<FlurlHttpException>(
                () => client.GetProjectAsync(projectKey));

            Assert.Equal((int)HttpStatusCode.Unauthorized, exception.StatusCode);
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
            var exception = await Assert.ThrowsAsync<FlurlHttpException>(
                () => client.GetProjectAsync(projectKey));

            Assert.Equal((int)HttpStatusCode.InternalServerError, exception.StatusCode);
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
            var exception = await Assert.ThrowsAsync<FlurlHttpException>(
                () => client.GetProjectRepositoryAsync(projectKey, repoSlug));

            Assert.Equal((int)HttpStatusCode.NotFound, exception.StatusCode);
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
            var exception = await Assert.ThrowsAsync<FlurlHttpException>(
                () => client.GetPullRequestAsync(projectKey, repoSlug, pullRequestId));

            Assert.Equal((int)HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}
