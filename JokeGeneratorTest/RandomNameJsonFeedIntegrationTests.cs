using JokeGenerator.Feeds;
using NUnit.Framework;

namespace JokeGeneratorTest
{
    [TestFixture]
    [Explicit("These tests depend on communicating with the live API, so they shouldn't be included in CI builds.")]
    public class RandomNameJsonFeedIntegrationTests
    {
        [Test]
        public void GetRandomJokeWorksAgainstLiveApi()
        {
            var feed = new RandomNameJsonFeed();
            var nameDto = feed.GetRandomName();

            Assert.IsNotNull(nameDto, "Returned DTO was null.");

            // Since we don't know what the live server will return, just do a very simple test.
            Assert.IsFalse(string.IsNullOrWhiteSpace(nameDto.FirstName), "No FirstName was returned.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(nameDto.Surname), "No Surname was returned.");
        }
    }
}