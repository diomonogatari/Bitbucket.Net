using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Core.Users;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class PullRequestParticipantsMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetPullRequestParticipantsAsync_ReturnsParticipants()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequestParticipants(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var participants = await client.GetPullRequestParticipantsAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);

        Assert.NotNull(participants);
        var participantList = participants.ToList();
        Assert.Single(participantList);
        Assert.NotNull(participantList[0].User);
        Assert.Equal("testuser", participantList[0].User!.Name);
        Assert.Equal(Roles.Author, participantList[0].Role);
    }

    [Fact]
    public async Task AssignUserRoleToPullRequestAsync_ReturnsParticipant()
    {
        _fixture.Reset();
        _fixture.Server.SetupAssignUserRoleToPullRequest(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var named = new Named { Name = "reviewer" };

        var participant = await client.AssignUserRoleToPullRequestAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId,
            named,
            Roles.Reviewer);

        Assert.NotNull(participant);
        Assert.NotNull(participant.User);
        Assert.Equal(Roles.Reviewer, participant.Role);
    }

    [Fact]
    public async Task DeletePullRequestParticipantAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeletePullRequestParticipant(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var result = await client.DeletePullRequestParticipantAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId,
            "testuser");

        Assert.True(result);
    }

    [Fact]
    public async Task UnassignUserFromPullRequestAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupUnassignUserFromPullRequest(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId,
            "testuser");
        var client = _fixture.CreateClient();

        var result = await client.UnassignUserFromPullRequestAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId,
            "testuser");

        Assert.True(result);
    }
}