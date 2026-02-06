using System.Linq;
using System.Threading.Tasks;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests
{
    public class GroupsMockTests : IClassFixture<BitbucketMockFixture>
    {
        private readonly BitbucketMockFixture _fixture;

        public GroupsMockTests(BitbucketMockFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetGroupNamesAsync_ReturnsGroupNames()
        {
            _fixture.Reset();
            _fixture.Server.SetupGetGroupNames();
            var client = _fixture.CreateClient();

            var groups = await client.GetGroupNamesAsync();

            var groupList = groups.ToList();
            Assert.NotEmpty(groupList);
            Assert.Equal(3, groupList.Count);
            Assert.Equal("developers", groupList[0]);
            Assert.Equal("administrators", groupList[1]);
            Assert.Equal("testers", groupList[2]);
        }
    }
}
