using Bitbucket.Net.Tests.Infrastructure;
using System.Net;
using System.Text;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

/// <summary>
/// Covers <c>UpdateProjectRepositoryPathAsync</c> — the existing file-on-disk overload and the new
/// stream overload that lets callers push in-memory content without writing a physical file.
/// </summary>
public class RepositoryFileUpdateMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private const string ProjectKey = "PRJ";
    private const string RepoSlug = "repo";
    private const string FilePath = "README.md";
    private const string CommitJson = "{\"id\":\"abc123def456\",\"displayId\":\"abc123d\"}";

    private readonly BitbucketMockFixture _fixture = fixture;

    private string BrowsePath => $"/rest/api/1.0/projects/{ProjectKey}/repos/{RepoSlug}/browse/{FilePath}";

    private void StubBrowsePut()
    {
        _fixture.Server
            .Given(Request.Create().WithPath(BrowsePath).UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(CommitJson));
    }

    private string SingleRequestBody(out string method)
    {
        var message = Assert.Single(_fixture.Server.LogEntries).RequestMessage;
        method = message.Method;
        return message.Body ?? (message.BodyAsBytes is { } bytes ? Encoding.UTF8.GetString(bytes) : string.Empty);
    }

    [Fact]
    public async Task UpdateProjectRepositoryPathAsync_FromStream_PutsContentAndReturnsCommit()
    {
        _fixture.Reset();
        StubBrowsePut();
        var client = _fixture.CreateClient();

        using var content = new MemoryStream(Encoding.UTF8.GetBytes("hello from a stream"));
        var commit = await client.UpdateProjectRepositoryPathAsync(
            ProjectKey, RepoSlug, FilePath, content, "main", message: "update readme");

        Assert.NotNull(commit);
        Assert.Equal("abc123def456", commit.Id);

        var body = SingleRequestBody(out var method);
        Assert.Equal("PUT", method, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("hello from a stream", body);
        Assert.Contains("main", body);
        Assert.Contains("update readme", body);
    }

    [Fact]
    public async Task UpdateProjectRepositoryPathAsync_FromFile_ReadsFileAndReturnsCommit()
    {
        _fixture.Reset();
        StubBrowsePut();
        var client = _fixture.CreateClient();

        var tempFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(tempFile, "hello from a file");

            var commit = await client.UpdateProjectRepositoryPathAsync(
                ProjectKey, RepoSlug, FilePath, tempFile, "main");

            Assert.NotNull(commit);
            Assert.Equal("abc123def456", commit.Id);

            var body = SingleRequestBody(out var method);
            Assert.Equal("PUT", method, StringComparer.OrdinalIgnoreCase);
            Assert.Contains("hello from a file", body);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task UpdateProjectRepositoryPathAsync_FromStream_NullContent_Throws()
    {
        var client = _fixture.CreateClient();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            client.UpdateProjectRepositoryPathAsync(ProjectKey, RepoSlug, FilePath, (Stream)null!, "main"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateProjectRepositoryPathAsync_FromStream_BlankPath_Throws(string path)
    {
        var client = _fixture.CreateClient();
        using var content = new MemoryStream(Encoding.UTF8.GetBytes("x"));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.UpdateProjectRepositoryPathAsync(ProjectKey, RepoSlug, path, content, "main"));
    }

    [Fact]
    public async Task UpdateProjectRepositoryPathAsync_FromFile_MissingFile_Throws()
    {
        var client = _fixture.CreateClient();
        var missing = Path.Combine(Path.GetTempPath(), $"bbnet-missing-{Guid.NewGuid():N}.txt");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.UpdateProjectRepositoryPathAsync(ProjectKey, RepoSlug, FilePath, missing, "main"));
    }
}