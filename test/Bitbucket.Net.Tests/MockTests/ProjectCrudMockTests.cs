using Bitbucket.Net.Models.Core.Projects.Requests;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class ProjectCrudMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task CreateProjectAsync_ReturnsCreatedProject()
    {
        _fixture.Reset();
        _fixture.Server.SetupCreateProject();
        var client = _fixture.CreateClient();

        var request = new CreateProjectRequest
        {
            Key = TestConstants.TestProjectKey,
            Name = TestConstants.TestProjectName,
            Description = "Created by unit test"
        };

        var project = await client.CreateProjectAsync(request);

        Assert.NotNull(project);
        Assert.Equal(TestConstants.TestProjectKey, project.Key);
        Assert.Equal(TestConstants.TestProjectName, project.Name);
    }

    [Fact]
    public async Task UpdateProjectAsync_ReturnsUpdatedProject()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateProject(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();

        var request = new UpdateProjectRequest
        {
            Name = "Updated Name",
            Description = "Updated by unit test"
        };

        var project = await client.UpdateProjectAsync(TestConstants.TestProjectKey, request);

        Assert.NotNull(project);
        Assert.Equal(TestConstants.TestProjectKey, project.Key);
    }

    [Fact]
    public async Task DeleteProjectAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeleteProject(TestConstants.TestProjectKey);
        var client = _fixture.CreateClient();

        var result = await client.DeleteProjectAsync(TestConstants.TestProjectKey);

        Assert.True(result);
    }
}