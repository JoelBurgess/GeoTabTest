using JokeCompany.ExternalFeeds.RandomName;
using NUnit.Framework;

namespace JokeCompany.ExternalFeeds.Test
{
    [TestFixture]
    [Explicit("These tests depend on communicating with the live API, so they shouldn't be included in CI builds.")]
    public class RandomNameJsonFeedIntegrationTests
    {
        [Test]
        public void GetRandomNameResponseWorksAgainstLiveApi()
        {
            var feed = new RandomNameJsonFeed();
            var nameDto = feed.GetRandomNameResponse();

            Assert.IsNotNull(nameDto, "Returned DTO was null.");

            // Since we don't know what the live server will return, just do a very simple test.
            Assert.IsFalse(string.IsNullOrWhiteSpace(nameDto.FirstName), "No FirstName was returned.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(nameDto.Surname), "No Surname was returned.");
        }

        [Test]
        public void GetRandomNameWorksAgainstLiveApi()
        {
            var feed = new RandomNameJsonFeed();
            var name = feed.GetRandomFullName();

            Assert.IsNotNull(name, "Returned name was null.");

            // Since we don't know what the live server will return, just do a very simple test.
            StringAssert.IsMatch(@"^(\w*) (\w*)$", name, "Valid name was not returned");
        }
    }
}