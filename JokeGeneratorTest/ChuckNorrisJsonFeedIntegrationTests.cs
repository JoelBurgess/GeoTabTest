using System.Linq;
using JokeGenerator.Feeds;
using NUnit.Framework;

namespace JokeGeneratorTest
{
    [TestFixture]
    [Explicit("These tests depend on communicating with the live API, so they shouldn't be included in CI builds.")]
    public class ChuckNorrisJsonFeedIntegrationTests
    {
        [Test]
        public void GetRandomJokeWorksAgainstLiveApi()
        {
            var feed = new ChuckNorrisJsonFeed();
            var jokeDto = feed.GetRandomJoke();
            
            Assert.IsNotNull(jokeDto, "Returned DTO was null.");

            // Since we don't know what the live server will return, just do a very simple test.
            StringAssert.Contains("Chuck Norris", jokeDto.JokeText);
        }

        [Test]
        public void GetRandomJokeWithBadCategoryAgainstLiveApiGetsException()
        {
            var feed = new ChuckNorrisJsonFeed();
            Assert.That(() => feed.GetRandomJoke("badCategory"), Throws.TypeOf<JsonFeedException>());
        }

        [Test]
        public void GetCategoriesWorksAgainstLiveApi()
        {
            var feed = new ChuckNorrisJsonFeed();
            var categories = feed.GetCategories();
            Assert.IsTrue(categories.Any(), "No categories were retrieved.");

            // Test for a known category, but don't bother with entire set as it can change.
            Assert.IsTrue(categories.Contains("animal"), "Categories did not contain expected category");
        }
    }
}