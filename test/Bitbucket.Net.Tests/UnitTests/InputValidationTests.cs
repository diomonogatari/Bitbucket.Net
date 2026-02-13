#nullable enable

using Xunit;

namespace Bitbucket.Net.Tests.UnitTests;

/// <summary>
/// Verifies that public methods validate URL-path string parameters
/// with ArgumentException for null, empty, and whitespace inputs.
/// </summary>
public class InputValidationTests
{
    private const string TestUrl = "https://bitbucket.example.com";
    private static BitbucketClient CreateClient() => new(TestUrl, "user", "pass");

    #region ProjectKey validation

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetProjectAsync_InvalidProjectKey_ThrowsArgumentException(string? projectKey)
    {
        var client = CreateClient();
        await Assert.ThrowsAnyAsync<ArgumentException>(
            () => client.GetProjectAsync(projectKey!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetProjectRepositoriesAsync_InvalidProjectKey_ThrowsArgumentException(string? projectKey)
    {
        var client = CreateClient();
        await Assert.ThrowsAnyAsync<ArgumentException>(
            () => client.GetProjectRepositoriesAsync(projectKey!));
    }

    #endregion

    #region RepositorySlug validation

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetBranchesAsync_InvalidRepositorySlug_ThrowsArgumentException(string? repositorySlug)
    {
        var client = CreateClient();
        await Assert.ThrowsAnyAsync<ArgumentException>(
            () => client.GetBranchesAsync("PROJ", repositorySlug!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetPullRequestsAsync_InvalidRepositorySlug_ThrowsArgumentException(string? repositorySlug)
    {
        var client = CreateClient();
        await Assert.ThrowsAnyAsync<ArgumentException>(
            () => client.GetPullRequestsAsync("PROJ", repositorySlug!));
    }

    #endregion

    #region CommitId validation

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetCommitAsync_InvalidCommitId_ThrowsArgumentException(string? commitId)
    {
        var client = CreateClient();
        await Assert.ThrowsAnyAsync<ArgumentException>(
            () => client.GetCommitAsync("PROJ", "repo", commitId!));
    }

    #endregion

    #region Combined path parameters

    [Fact]
    public async Task GetPullRequestActivitiesAsync_NullProjectKey_ThrowsBeforeHttpCall()
    {
        var client = CreateClient();
        // Should throw immediately without making any HTTP call
        var ex = await Assert.ThrowsAnyAsync<ArgumentException>(
            () => client.GetPullRequestActivitiesAsync(null!, "repo", 1));

        Assert.Contains("projectKey", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetPullRequestActivitiesAsync_NullRepoSlug_ThrowsBeforeHttpCall()
    {
        var client = CreateClient();
        var ex = await Assert.ThrowsAnyAsync<ArgumentException>(
            () => client.GetPullRequestActivitiesAsync("PROJ", null!, 1));

        Assert.Contains("repositorySlug", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion
}