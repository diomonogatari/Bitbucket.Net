using Bitbucket.Net.Tests.Infrastructure;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class MarkupMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task PreviewMarkupAsync_ReturnsHtml()
    {
        _fixture.Reset();
        _fixture.Server.SetupPreviewMarkup();
        var client = _fixture.CreateClient();

        var result = await client.PreviewMarkupAsync("**Bold** text");

        Assert.NotNull(result);
        Assert.Contains("<strong>markdown</strong>", result);
    }
}