using Bitbucket.Net.Tests.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

/// <summary>
/// Unit tests for project-related operations using WireMock.
/// </summary>
public class ProjectMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetProjectsAsync_ReturnsProjects()
    {
        // Arrange
        _fixture.Reset();
        _fixture.Server.SetupGetProjects();
        var client = _fixture.CreateClient();

        // Act
        var projects = await client.GetProjectsAsync();

        // Assert
        Assert.NotNull(projects);
        var projectList = projects.ToList();
        Assert.Single(projectList);
        var project = projectList[0];
        Assert.Equal(TestConstants.TestProjectKey, project.Key);
        Assert.Equal(TestConstants.TestProjectName, project.Name);
    }

    [Fact]
    public async Task GetProjectAsync_WithValidKey_ReturnsProject()
    {
        // Arrange
        _fixture.Reset();
        _fixture.Server.SetupGetProject(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();

        // Act
        var project = await client.GetProjectAsync(TestConstants.TestProjectKey);

        // Assert
        Assert.NotNull(project);
        Assert.Equal(TestConstants.TestProjectKey, project.Key);
        Assert.Equal(TestConstants.TestProjectName, project.Name);
        Assert.NotNull(project.Description);
    }

    [Fact]
    public async Task GetProjectRepositoriesAsync_ReturnsRepositories()
    {
        // Arrange
        _fixture.Reset();
        _fixture.Server.SetupGetRepositories(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();

        // Act
        var repositories = await client.GetProjectRepositoriesAsync(TestConstants.TestProjectKey);

        // Assert
        Assert.NotNull(repositories);
        var repoList = repositories.ToList();
        Assert.Single(repoList);
        var repo = repoList[0];
        Assert.Equal(TestConstants.TestRepositorySlug, repo.Slug);
        Assert.Equal(TestConstants.TestRepositoryName, repo.Name);
    }
}