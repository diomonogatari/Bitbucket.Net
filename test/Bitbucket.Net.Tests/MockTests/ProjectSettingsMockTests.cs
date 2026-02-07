using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Tests.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class ProjectSettingsMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetProjectPullRequestsMergeStrategiesAsync_ReturnsSettings()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjectPullRequestsMergeStrategies(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();

        var result = await client.GetProjectPullRequestsMergeStrategiesAsync(
            TestConstants.TestProjectKey,
            "git");

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateProjectPullRequestsMergeStrategiesAsync_ReturnsStrategies()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateProjectPullRequestsMergeStrategies(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();

        var strategies = new MergeStrategies();
        var result = await client.UpdateProjectPullRequestsMergeStrategiesAsync(
            TestConstants.TestProjectKey,
            "git",
            strategies);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task BrowseProjectRepositoryPathAsync_ReturnsBrowseResult()
    {
        _fixture.Reset();
        _fixture.Server.SetupBrowseProjectRepositoryPath(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var result = await client.BrowseProjectRepositoryPathAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            path: "src",
            at: "refs/heads/main");

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetRawFileContentStreamAsync_ReturnsStream()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetRawFileContentStream(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        using var stream = await client.GetRawFileContentStreamAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            path: "README.md",
            at: "refs/heads/main");

        Assert.NotNull(stream);
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task GetProjectRepositoryTagsAsync_ReturnsTags()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetProjectRepositoryTags(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var result = await client.GetProjectRepositoryTagsAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            filterText: "",
            orderBy: Bitbucket.Net.Models.Core.Projects.BranchOrderBy.Alphabetical);

        Assert.NotNull(result);
        var tags = result.ToList();
        Assert.NotEmpty(tags);
    }

    [Fact]
    public async Task CreateProjectRepositoryTagAsync_CreatesTag()
    {
        _fixture.Reset();
        _fixture.Server.SetupCreateProjectRepositoryTag(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var result = await client.CreateProjectRepositoryTagAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            name: "v1.0.0",
            startPoint: "abc123",
            message: "Release v1.0.0");

        Assert.NotNull(result);
    }
}