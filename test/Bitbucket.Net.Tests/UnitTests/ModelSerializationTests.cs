#nullable enable

using System;
using System.Collections.Generic;
using System.Text.Json;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Users;
using Bitbucket.Net.Models.Builds;
using Bitbucket.Net.Serialization;
using Xunit;

namespace Bitbucket.Net.Tests.UnitTests;

public class ModelSerializationTests
{
    #region Project Serialization Tests

    [Fact]
    public void Project_Serialization_RoundTrips()
    {
        var project = new Project
        {
            Id = 1,
            Key = "PRJ",
            Name = "Test Project",
            Description = "A test project",
            Public = true,
            Type = "NORMAL"
        };

        var json = JsonSerializer.Serialize(project, BitbucketJsonContext.Default.Project);
        var deserialized = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.Project);

        Assert.NotNull(deserialized);
        Assert.Equal(project.Id, deserialized.Id);
        Assert.Equal(project.Key, deserialized.Key);
        Assert.Equal(project.Name, deserialized.Name);
        Assert.Equal(project.Description, deserialized.Description);
        Assert.Equal(project.Public, deserialized.Public);
        Assert.Equal(project.Type, deserialized.Type);
    }

    [Fact]
    public void Project_Deserialization_FromJson()
    {
        var json = """
        {
            "id": 42,
            "key": "DEMO",
            "name": "Demo Project",
            "description": "Demo description",
            "public": false,
            "type": "PERSONAL"
        }
        """;

        var project = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.Project);

        Assert.NotNull(project);
        Assert.Equal(42, project.Id);
        Assert.Equal("DEMO", project.Key);
        Assert.Equal("Demo Project", project.Name);
        Assert.Equal("Demo description", project.Description);
        Assert.False(project.Public);
        Assert.Equal("PERSONAL", project.Type);
    }

    #endregion

    #region Repository Serialization Tests

    [Fact]
    public void Repository_Serialization_RoundTrips()
    {
        var repository = new Repository
        {
            Id = 1,
            Slug = "test-repo",
            Name = "Test Repository",
            Forkable = true,
            Public = false,
            State = "AVAILABLE",
            ScmId = "git"
        };

        var json = JsonSerializer.Serialize(repository, BitbucketJsonContext.Default.Repository);
        var deserialized = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.Repository);

        Assert.NotNull(deserialized);
        Assert.Equal(repository.Id, deserialized.Id);
        Assert.Equal(repository.Slug, deserialized.Slug);
        Assert.Equal(repository.Name, deserialized.Name);
        Assert.Equal(repository.Forkable, deserialized.Forkable);
        Assert.Equal(repository.Public, deserialized.Public);
        Assert.Equal(repository.State, deserialized.State);
        Assert.Equal(repository.ScmId, deserialized.ScmId);
    }

    [Fact]
    public void Repository_Deserialization_FromJson()
    {
        var json = """
        {
            "id": 100,
            "slug": "my-repo",
            "name": "My Repository",
            "forkable": true,
            "public": true,
            "state": "AVAILABLE",
            "scmId": "git"
        }
        """;

        var repository = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.Repository);

        Assert.NotNull(repository);
        Assert.Equal(100, repository.Id);
        Assert.Equal("my-repo", repository.Slug);
        Assert.Equal("My Repository", repository.Name);
        Assert.True(repository.Forkable);
        Assert.True(repository.Public);
    }

    #endregion

    #region PullRequest Serialization Tests

    [Fact]
    public void PullRequest_Serialization_RoundTrips()
    {
        var pullRequest = new PullRequest
        {
            Id = 1,
            Title = "Test PR",
            Description = "A test pull request",
            State = PullRequestStates.Open,
            Open = true,
            Closed = false,
            Version = 1
        };

        var json = JsonSerializer.Serialize(pullRequest, BitbucketJsonContext.Default.PullRequest);
        var deserialized = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.PullRequest);

        Assert.NotNull(deserialized);
        Assert.Equal(pullRequest.Id, deserialized.Id);
        Assert.Equal(pullRequest.Title, deserialized.Title);
        Assert.Equal(pullRequest.Description, deserialized.Description);
        Assert.Equal(pullRequest.State, deserialized.State);
        Assert.Equal(pullRequest.Open, deserialized.Open);
        Assert.Equal(pullRequest.Closed, deserialized.Closed);
        Assert.Equal(pullRequest.Version, deserialized.Version);
    }

    [Fact]
    public void PullRequest_Deserialization_FromJson()
    {
        var json = """
        {
            "id": 42,
            "title": "Feature: Add new functionality",
            "description": "This PR adds new functionality",
            "state": "MERGED",
            "open": false,
            "closed": true,
            "version": 5
        }
        """;

        var pullRequest = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.PullRequest);

        Assert.NotNull(pullRequest);
        Assert.Equal(42, pullRequest.Id);
        Assert.Equal("Feature: Add new functionality", pullRequest.Title);
        Assert.Equal("This PR adds new functionality", pullRequest.Description);
        Assert.Equal(PullRequestStates.Merged, pullRequest.State);
        Assert.False(pullRequest.Open);
        Assert.True(pullRequest.Closed);
        Assert.Equal(5, pullRequest.Version);
    }

    #endregion

    #region User Serialization Tests

    [Fact]
    public void User_Serialization_RoundTrips()
    {
        var user = new User
        {
            Id = 1,
            Name = "testuser",
            DisplayName = "Test User",
            EmailAddress = "test@example.com",
            Active = true,
            Slug = "testuser",
            Type = "NORMAL"
        };

        var json = JsonSerializer.Serialize(user, BitbucketJsonContext.Default.User);
        var deserialized = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.User);

        Assert.NotNull(deserialized);
        Assert.Equal(user.Id, deserialized.Id);
        Assert.Equal(user.Name, deserialized.Name);
        Assert.Equal(user.DisplayName, deserialized.DisplayName);
        Assert.Equal(user.EmailAddress, deserialized.EmailAddress);
        Assert.Equal(user.Active, deserialized.Active);
        Assert.Equal(user.Slug, deserialized.Slug);
        Assert.Equal(user.Type, deserialized.Type);
    }

    [Fact]
    public void User_Deserialization_FromJson()
    {
        var json = """
        {
            "id": 100,
            "name": "jdoe",
            "displayName": "John Doe",
            "emailAddress": "john.doe@example.com",
            "active": true,
            "slug": "jdoe",
            "type": "NORMAL"
        }
        """;

        var user = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.User);

        Assert.NotNull(user);
        Assert.Equal(100, user.Id);
        Assert.Equal("jdoe", user.Name);
        Assert.Equal("John Doe", user.DisplayName);
        Assert.Equal("john.doe@example.com", user.EmailAddress);
        Assert.True(user.Active);
    }

    #endregion

    #region Commit Serialization Tests

    [Fact]
    public void Commit_Serialization_RoundTrips()
    {
        var commit = new Commit
        {
            Id = "abc123def456",
            DisplayId = "abc123d",
            Message = "Fix: resolve bug in login"
        };

        var json = JsonSerializer.Serialize(commit, BitbucketJsonContext.Default.Commit);
        var deserialized = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.Commit);

        Assert.NotNull(deserialized);
        Assert.Equal(commit.Id, deserialized.Id);
        Assert.Equal(commit.DisplayId, deserialized.DisplayId);
        Assert.Equal(commit.Message, deserialized.Message);
    }

    [Fact]
    public void Commit_Deserialization_FromJson()
    {
        var json = """
        {
            "id": "0123456789abcdef",
            "displayId": "0123456",
            "message": "Initial commit",
            "authorTimestamp": 1609459200000
        }
        """;

        var commit = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.Commit);

        Assert.NotNull(commit);
        Assert.Equal("0123456789abcdef", commit.Id);
        Assert.Equal("0123456", commit.DisplayId);
        Assert.Equal("Initial commit", commit.Message);
    }

    #endregion

    #region Branch Serialization Tests

    [Fact]
    public void Branch_Serialization_RoundTrips()
    {
        var branch = new Branch
        {
            Id = "refs/heads/main",
            DisplayId = "main",
            IsDefault = true,
            Type = "BRANCH"
        };

        var json = JsonSerializer.Serialize(branch, BitbucketJsonContext.Default.Branch);
        var deserialized = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.Branch);

        Assert.NotNull(deserialized);
        Assert.Equal(branch.Id, deserialized.Id);
        Assert.Equal(branch.DisplayId, deserialized.DisplayId);
        Assert.Equal(branch.IsDefault, deserialized.IsDefault);
        Assert.Equal(branch.Type, deserialized.Type);
    }

    [Fact]
    public void Branch_Deserialization_FromJson()
    {
        var json = """
        {
            "id": "refs/heads/feature/test",
            "displayId": "feature/test",
            "isDefault": false,
            "type": "BRANCH"
        }
        """;

        var branch = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.Branch);

        Assert.NotNull(branch);
        Assert.Equal("refs/heads/feature/test", branch.Id);
        Assert.Equal("feature/test", branch.DisplayId);
        Assert.False(branch.IsDefault);
    }

    #endregion

    #region BuildStatus Serialization Tests

    [Fact]
    public void BuildStatus_Serialization_RoundTrips()
    {
        var buildStatus = new BuildStatus
        {
            State = "SUCCESSFUL",
            Key = "build-123",
            Name = "CI Build",
            Url = "https://ci.example.com/build/123",
            Description = "Build passed"
        };

        var json = JsonSerializer.Serialize(buildStatus, BitbucketJsonContext.Default.BuildStatus);
        var deserialized = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.BuildStatus);

        Assert.NotNull(deserialized);
        Assert.Equal(buildStatus.State, deserialized.State);
        Assert.Equal(buildStatus.Key, deserialized.Key);
        Assert.Equal(buildStatus.Name, deserialized.Name);
        Assert.Equal(buildStatus.Url, deserialized.Url);
        Assert.Equal(buildStatus.Description, deserialized.Description);
    }

    [Fact]
    public void BuildStatus_Deserialization_FromJson()
    {
        var json = """
        {
            "state": "FAILED",
            "key": "test-456",
            "name": "Test Suite",
            "url": "https://ci.example.com/test/456",
            "description": "3 tests failed"
        }
        """;

        var buildStatus = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.BuildStatus);

        Assert.NotNull(buildStatus);
        Assert.Equal("FAILED", buildStatus.State);
        Assert.Equal("test-456", buildStatus.Key);
        Assert.Equal("Test Suite", buildStatus.Name);
    }

    #endregion

    #region Comment Serialization Tests

    [Fact]
    public void Comment_Serialization_RoundTrips()
    {
        var comment = new Comment
        {
            Id = 1,
            Text = "LGTM!",
            Version = 0
        };

        var json = JsonSerializer.Serialize(comment, BitbucketJsonContext.Default.Comment);
        var deserialized = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.Comment);

        Assert.NotNull(deserialized);
        Assert.Equal(comment.Id, deserialized.Id);
        Assert.Equal(comment.Text, deserialized.Text);
        Assert.Equal(comment.Version, deserialized.Version);
    }

    [Fact]
    public void Comment_Deserialization_FromJson()
    {
        var json = """
        {
            "id": 42,
            "text": "Please fix this issue",
            "version": 2
        }
        """;

        var comment = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.Comment);

        Assert.NotNull(comment);
        Assert.Equal(42, comment.Id);
        Assert.Equal("Please fix this issue", comment.Text);
        Assert.Equal(2, comment.Version);
    }

    #endregion

    #region Participant Serialization Tests

    [Fact]
    public void Participant_Serialization_RoundTrips()
    {
        var participant = new Participant
        {
            Approved = true,
            Status = ParticipantStatus.Approved,
            Role = Roles.Reviewer
        };

        var json = JsonSerializer.Serialize(participant, BitbucketJsonContext.Default.Participant);
        var deserialized = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.Participant);

        Assert.NotNull(deserialized);
        Assert.Equal(participant.Approved, deserialized.Approved);
        Assert.Equal(participant.Status, deserialized.Status);
        Assert.Equal(participant.Role, deserialized.Role);
    }

    [Fact]
    public void Participant_Deserialization_FromJson()
    {
        var json = """
        {
            "approved": false,
            "status": "NEEDS_WORK",
            "role": "AUTHOR"
        }
        """;

        var participant = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.Participant);

        Assert.NotNull(participant);
        Assert.False(participant.Approved);
        Assert.Equal(ParticipantStatus.NeedsWork, participant.Status);
        Assert.Equal(Roles.Author, participant.Role);
    }

    #endregion

    #region Hook Serialization Tests

    [Fact]
    public void Hook_Serialization_RoundTrips()
    {
        var hook = new Hook
        {
            Enabled = true,
            Configured = true
        };

        var json = JsonSerializer.Serialize(hook, BitbucketJsonContext.Default.Hook);
        var deserialized = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.Hook);

        Assert.NotNull(deserialized);
        Assert.Equal(hook.Enabled, deserialized.Enabled);
        Assert.Equal(hook.Configured, deserialized.Configured);
    }

    #endregion

    #region LicenseDetails Serialization Tests

    [Fact]
    public void LicenseDetails_Serialization_RoundTrips()
    {
        var license = new LicenseDetails
        {
            MaximumNumberOfUsers = 100
        };

        var json = JsonSerializer.Serialize(license, BitbucketJsonContext.Default.LicenseDetails);
        var deserialized = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.LicenseDetails);

        Assert.NotNull(deserialized);
        Assert.Equal(license.MaximumNumberOfUsers, deserialized.MaximumNumberOfUsers);
    }

    #endregion

    #region Complex Object Serialization Tests

    [Fact]
    public void Repository_WithProject_Serialization_RoundTrips()
    {
        var repository = new Repository
        {
            Id = 1,
            Slug = "test-repo",
            Name = "Test Repository",
            Project = new ProjectRef
            {
                Key = "PRJ"
            }
        };

        var json = JsonSerializer.Serialize(repository, BitbucketJsonContext.Default.Repository);
        var deserialized = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.Repository);

        Assert.NotNull(deserialized);
        Assert.NotNull(deserialized.Project);
        Assert.Equal("PRJ", deserialized.Project.Key);
    }

    [Fact]
    public void PullRequest_WithFromRef_Serialization_RoundTrips()
    {
        var pullRequest = new PullRequest
        {
            Id = 1,
            Title = "Test PR",
            FromRef = new FromToRef
            {
                Id = "refs/heads/feature",
                DisplayId = "feature"
            },
            ToRef = new FromToRef
            {
                Id = "refs/heads/main",
                DisplayId = "main"
            }
        };

        var json = JsonSerializer.Serialize(pullRequest, BitbucketJsonContext.Default.PullRequest);
        var deserialized = JsonSerializer.Deserialize(json, BitbucketJsonContext.Default.PullRequest);

        Assert.NotNull(deserialized);
        Assert.NotNull(deserialized.FromRef);
        Assert.NotNull(deserialized.ToRef);
        Assert.Equal("refs/heads/feature", deserialized.FromRef.Id);
        Assert.Equal("refs/heads/main", deserialized.ToRef.Id);
    }

    #endregion
}
