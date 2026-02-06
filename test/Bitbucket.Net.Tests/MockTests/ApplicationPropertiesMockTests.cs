using System.Threading.Tasks;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    public class ApplicationPropertiesMockTests : IClassFixture<BitbucketMockFixture>
    {
        private readonly BitbucketMockFixture _fixture;

        public ApplicationPropertiesMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetApplicationPropertiesAsync_ReturnsProperties()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetApplicationProperties();
            var client = _fixture.CreateClient();

            var result = await client.GetApplicationPropertiesAsync();

            Assert.NotNull(result);
            Assert.True(result.ContainsKey("version"));
            Assert.True(result.ContainsKey("buildNumber"));
            Assert.True(result.ContainsKey("displayName"));
            Assert.Equal("8.14.0", result["version"]?.ToString());
        }
    }
}
