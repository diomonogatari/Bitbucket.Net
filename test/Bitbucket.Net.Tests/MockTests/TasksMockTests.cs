using Bitbucket.Net.Models.Core.Tasks;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class TasksMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task CreateTaskAsync_ReturnsCreatedTask()
    {
        _fixture.Reset();
        _fixture.Server.SetupCreateTask();
        var client = _fixture.CreateClient();

        var taskInfo = new TaskInfo
        {
            Anchor = new TaskBasicAnchor { Id = 101, Type = "COMMENT" },
            Text = "Fix the null pointer exception"
        };

        var task = await client.CreateTaskAsync(taskInfo);

        Assert.NotNull(task);
        Assert.Equal(1, task.Id);
        Assert.Equal("Fix the null pointer exception", task.Text);
        Assert.Equal("OPEN", task.State);
        Assert.NotNull(task.Author);
        Assert.Equal("jsmith", task.Author.Name);
    }

    [Fact]
    public async Task GetTaskAsync_ReturnsTask()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetTask(1);
        var client = _fixture.CreateClient();

        var task = await client.GetTaskAsync(1);

        Assert.NotNull(task);
        Assert.Equal(1, task.Id);
        Assert.Equal("Fix the null pointer exception", task.Text);
        Assert.Equal("OPEN", task.State);
        Assert.NotNull(task.Anchor);
    }

    [Fact]
    public async Task UpdateTaskAsync_ReturnsUpdatedTask()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateTask(1);
        var client = _fixture.CreateClient();

        var task = await client.UpdateTaskAsync(1, "Updated task text");

        Assert.NotNull(task);
        Assert.Equal(1, task.Id);
        Assert.NotNull(task.Author);
    }

    [Fact]
    public async Task DeleteTaskAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeleteTask(1);
        var client = _fixture.CreateClient();

        var result = await client.DeleteTaskAsync(1);

        Assert.True(result);
    }
}