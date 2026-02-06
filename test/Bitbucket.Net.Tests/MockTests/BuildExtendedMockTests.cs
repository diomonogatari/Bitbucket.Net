using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bitbucket.Net.Models.Builds;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    public class BuildExtendedMockTests : IClassFixture<BitbucketMockFixture>
    {
        private readonly BitbucketMockFixture _fixture;

        public BuildExtendedMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetBuildStatsForCommitsAsync_WithCancellationToken_ReturnsDictionaryWithStats()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetBuildStatsForMultipleCommits();
            var client = _fixture.CreateClient();

            var stats = await client.GetBuildStatsForCommitsAsync(
                cancellationToken: CancellationToken.None,
                commitIds: ["abc123def456", "def456ghi789"]);

            Assert.NotNull(stats);
            Assert.Equal(2, stats.Count);
            Assert.True(stats.ContainsKey("abc123def456"));
            Assert.True(stats.ContainsKey("def456ghi789"));
            Assert.Equal(2, stats["abc123def456"].Successful);
            Assert.Equal(1, stats["def456ghi789"].Failed);
        }

        [Fact]
        public async Task GetBuildStatsForCommitsAsync_WithoutCancellationToken_ReturnsDictionaryWithStats()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetBuildStatsForMultipleCommits();
            var client = _fixture.CreateClient();

            var stats = await client.GetBuildStatsForCommitsAsync(commitIds: ["abc123def456", "def456ghi789"]);

            Assert.NotNull(stats);
            Assert.Equal(2, stats.Count);
        }
    }
}
