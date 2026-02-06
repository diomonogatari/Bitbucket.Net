using System.Threading.Tasks;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    public class ProjectCrudMockTests : IClassFixture<BitbucketMockFixture>
    {
        private readonly BitbucketMockFixture _fixture;

        public ProjectCrudMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task CreateProjectAsync_ReturnsCreatedProject()
        {
            _fixture.Reset();
            _fixture.Server.SetupCreateProject();
            var client = _fixture.CreateClient();

            var projectDef = new ProjectDefinition
            {
                Key = TestConstants.TestProjectKey,
                Name = TestConstants.TestProjectName,
                Description = "Created by unit test"
            };

            var project = await client.CreateProjectAsync(projectDef);

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

            var projectDef = new ProjectDefinition
            {
                Name = "Updated Name",
                Description = "Updated by unit test"
            };

            var project = await client.UpdateProjectAsync(TestConstants.TestProjectKey, projectDef);

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
}
