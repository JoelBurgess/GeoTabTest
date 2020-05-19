using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using JokeCompany.ExternalFeeds.ChuckNorris;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace JokeCompany.ExternalFeeds.Test
{
    [TestFixture]
    public class ChuckNorrisJsonFeedUnitTests : AbstractJsonFeedUnitTestBase
    {
        private const string DefaultJoke = @"Chuck Norris can unit test entire applications with a single assert.";
        private const string DefaultCreatedTime = @"2020-01-05 13:42:00.000000";
        private const string DefaultIconUrl = @"https://assets.chucknorris.host/img/avatar/chuck-norris.png";
        private const string DefaultId = @"B_INrZqPRbymsYWUp96DbQ";
        private const string DefaultUpdatedAt = @"2020-01-05 13:42:00.000000";
        private const string RealCategoryResponse = @"[""animal"",""career"",""celebrity"",""dev"",""explicit"",""fashion"",""food"",""history"",""money"",""movie"",""music"",""political"",""religion"",""science"",""sport"",""travel""]";
        private const string DefaultCategories = "";
        private const string DefaultUrl = @"https://api.chucknorris.io/jokes/B_INrZqPRbymsYWUp96DbQ";
        private const string CategoriesUri = "https://api.chucknorris.io/jokes/categories";
        private const string RandomJokesUri = "https://api.chucknorris.io/jokes/random";
        private const string RandomJokesWithCategoryUri = "https://api.chucknorris.io/jokes/random?category=";

        [Test]
        public void GetCategoriesHandlesValidResponse()
        {
            var categoriesResponse = GetCategoriesDeserializedResponse();
            var expected = JsonConvert.DeserializeObject<List<string>>(RealCategoryResponse);

            Assert.AreEqual(expected.Count, expected.Union(categoriesResponse).Count());
        }

        [Test]
        public void GetCategoriesThrowsWhenResponse()
        {
            var exception = Assert.Throws<JsonFeedException>(() => GetCategoriesDeserializedResponse(HttpStatusCode.InternalServerError));
            StringAssert.Contains($"Failed to get response ({CategoriesUri}) due to errors", exception.Message);
            StringAssert.Contains(InternalServerError, exception.Message);
        }

        [Test]
        public void GetCategoriesThrowsWhenBadJson()
        {
            var exception = Assert.Throws<JsonFeedException>(() => GetCategoriesDeserializedResponse(HttpStatusCode.OK, "This [ is } not valid JSON!>[}{"));
            StringAssert.Contains($"Failed to deserialize response ({CategoriesUri}) due to error", exception.Message);
            StringAssert.Contains("Unexpected character", exception.Message); // This snippet is from Newtonsoft, so it could potentially change in future versions.
        }

        [Test]
        public void GetRandomJokeHandlesValidResponseWithoutCategory()
        {
            var jokeResponse = GetRandomJokeDeserializedResponse();

            ValidatePropertiesAgainstDefaults(jokeResponse);
        }

        private static void ValidatePropertiesAgainstDefaults(RandomJokeResponse jokeResponse)
        {
            Assert.AreEqual(DefaultCategories, jokeResponse.Categories);
            Assert.AreEqual(DateTimeOffset.Parse(DefaultCreatedTime), jokeResponse.CreatedAt);
            Assert.AreEqual(new Uri(DefaultIconUrl), jokeResponse.IconUrl);
            Assert.AreEqual(DefaultId, jokeResponse.Id);
            Assert.AreEqual(DefaultJoke, jokeResponse.JokeText);
            Assert.AreEqual(DateTimeOffset.Parse(DefaultUpdatedAt), jokeResponse.UpdatedAt);
            Assert.AreEqual(new Uri(DefaultUrl), jokeResponse.Url);
        }

        [Test]
        public void GetRandomJokeHandlesValidResponseWithCategory()
        {
            var jokeResponse = GetRandomJokeDeserializedResponse(category:"kwijibo");

            ValidatePropertiesAgainstDefaults(jokeResponse);
        }

        [Test]
        public void GetRandomJokeWorksPropertyWithoutCategory()
        {
            const string jokeText = "When Chuck Norris throws exceptions, it’s across the room.";

            var feedMock = new Mock<ChuckNorrisJsonFeed>(null) {CallBase = true};
            feedMock.Setup(f => f.GetRandomJokeResponse(null)).Returns(new RandomJokeResponse { JokeText = jokeText});
            var joke = feedMock.Object.GetRandomJoke();

            Assert.AreEqual(jokeText, joke);
            
            // Since we have the mock, we may as well check the calls too.
            feedMock.Verify(f => f.GetRandomJokeResponse(null), Times.Once);
            feedMock.VerifyNoOtherCalls();
        }

        [Test]
        public void GetRandomJokeWorksPropertyWithCategory()
        {
            const string jokeText = "All arrays Chuck Norris declares are of infinite size, because Chuck Norris knows no bounds.";
            const string category = nameof(GetRandomJokeWorksPropertyWithCategory);

            var feedMock = new Mock<ChuckNorrisJsonFeed>(null) { CallBase = true };
            feedMock.Setup(f => f.GetRandomJokeResponse(category)).Returns(new RandomJokeResponse { JokeText = jokeText });
            var joke = feedMock.Object.GetRandomJoke(category);

            Assert.AreEqual(jokeText, joke);

            // Since we have the mock, we may as well check the calls too.
            feedMock.Verify(f => f.GetRandomJokeResponse(category), Times.Once);
            feedMock.VerifyNoOtherCalls();
        }

        [Test]
        public void GetRandomJokeThrowsWhenBadResponse()
        {
            var exception = Assert.Throws<JsonFeedException>(() => GetRandomJokeDeserializedResponse(statusCode:HttpStatusCode.InternalServerError));
            StringAssert.Contains($"Failed to get response ({RandomJokesUri}) due to errors", exception.Message);
            StringAssert.Contains(InternalServerError, exception.Message);

        }

        [Test]
        public void GetRandomJokeThrowsWhenNotFoundResponse()
        {
            // Bad category passed in returns a 404.
            var exception = Assert.Throws<JsonFeedException>(() => GetRandomJokeDeserializedResponse(statusCode: HttpStatusCode.NotFound));
            StringAssert.Contains($"Failed to get response ({RandomJokesUri}) due to errors", exception.Message);
            StringAssert.Contains(NotFound, exception.Message);

        }

        [Test]
        public void GetRandomJokeThrowsWhenBadJson()
        {
            var exception = Assert.Throws<JsonFeedException>(() => GetRandomJokeDeserializedResponse(HttpStatusCode.OK, jokeText:"This [ is } not valid JSON!>[}{"));
            StringAssert.Contains($"Failed to deserialize response ({RandomJokesUri}) due to error", exception.Message);
            StringAssert.Contains("Unexpected character", exception.Message); // This snippet is from Newtonsoft, so it could potentially change in future versions.
        }

        [Test]
        public void GetHttpClientWorksWithoutProvidedHandler()
        {
            // This can also be proven working properly by manually running the integration tests for this feed.
            var feed = new ChuckNorrisJsonFeed();
            Assert.IsNull(feed.Handler);

            feed.GetHttpClient();
            Assert.Pass("No exception was thrown, so HttpClient was constructed successfully.");
            
        }
        
        private static RandomJokeResponse GetRandomJokeDeserializedResponse(HttpStatusCode statusCode = HttpStatusCode.OK, string category = null, string jokeText = null)
        {
            var uri = category == null ? RandomJokesUri : RandomJokesWithCategoryUri + category;
            var handlerMock = CreateMessageHandlerMock(statusCode, jokeText ?? BuildRandomJokeResponse(), uri);
            var feed = new ChuckNorrisJsonFeed(handlerMock.Object);

            return feed.GetRandomJokeResponse(category);
        }

        private static string BuildRandomJokeResponse(string categories = DefaultCategories, string createdTime = DefaultCreatedTime, string iconUrl = DefaultIconUrl,
                                                      string id = DefaultId, string updatedAt = DefaultUpdatedAt, string url = DefaultUrl, string joke = DefaultJoke)
        {
            return $@"{{""categories"":[{categories}],""created_at"":""{createdTime}"",""icon_url"":""{iconUrl}"",""id"":""{id}""," + 
                   $@"""updated_at"":""{updatedAt}"",""url"":""{url}"",""value"":""{joke}""}}";
        }

        private static IList<string> GetCategoriesDeserializedResponse(HttpStatusCode statusCode = HttpStatusCode.OK, string json = RealCategoryResponse)
        {
            var handlerMock = CreateMessageHandlerMock(statusCode, json, CategoriesUri);
            var feed = new ChuckNorrisJsonFeed(handlerMock.Object);

            return feed.GetCategories();
        }
    }
}