using System.Net;
using JokeCompany.ExternalFeeds.RandomName;
using NUnit.Framework;

namespace JokeCompany.ExternalFeeds.Test
{
    [TestFixture]
    public class RandomNameJsonFeedUnitTests : AbstractJsonFeedUnitTestBase
    {
        private const string ExpectedUri = "https://names.privserv.com/api/";

        [Test]
        public void GetRandomNameResponseWorksHandlesValidResponse()
        {
            TestValidResponse("Aureliana", "Varzar", "female", "Romania"); // Values from real API response
        }

        [Test]
        public void GetRandomNameWorksHandlesValidResponse()
        {
            const string firstName = "Grace";
            const string surName = "Hopper";

            var handlerMock = CreateMessageHandlerMock(HttpStatusCode.OK, MakeJson(firstName, surName, "female", "USA"), ExpectedUri);
            var feed = new RandomNameJsonFeed(handlerMock.Object);
            var name = feed.GetRandomFullName();

            Assert.AreEqual($"{firstName} {surName}", name, "Name was wrong.");
        }

        [Test]
        public void GetRandomNameResponseWorksHandlesValidResponseWithUnicode()
        {
            TestValidResponse("Ζώσιμος", "Παπαστεφάνου", "male", "Greece"); // Values from real API response
        }

        [Test]
        public void GetRandomNameResponseWorksHandlesValidResponseWithPunctuationInNames()
        {
            TestValidResponse("Aure,Liana", "D'Sousa-Smith", "female", "Romania");
        }

        [Test]
        public void GetRandomNameResponseWorksHandlesValidResponseWithEmptyStrings()
        {
            TestValidResponse("", "", "", "");
        }

        [Test]
        public void GetRandomNameResponseWorksHandlesValidResponseWithVeryLargeStrings()
        {
            var largeString = new string('a', 50000);
            TestValidResponse(largeString, largeString, largeString, largeString);
        }

        [Test]
        public void GetRandomNameResponseWorksThrowsWhenJson()
        {
            var handlerMock = CreateMessageHandlerMock(HttpStatusCode.OK, "This is not { valid json ].", ExpectedUri);
            var feed = new RandomNameJsonFeed(handlerMock.Object);

            var exception = Assert.Throws<JsonFeedException>(() => feed.GetRandomNameResponse());
            StringAssert.Contains($"Failed to deserialize response ({ExpectedUri}) due to error", exception.Message);
            StringAssert.Contains("Unexpected character", exception.Message); // This snippet is from Newtonsoft, so it could potentially change in future versions.
        }

        [Test]
        public void GetRandomNameWorksThrowsWhenResponse()
        {
            // Tests using valid JSON (which would never be expected on an Internal Server Error) to ensure that the error code is evaluated.
            var exception = Assert.Throws<JsonFeedException>(() => GetRandomNameResponse("Aureliana", "Varzar", "female", "Romania", HttpStatusCode.InternalServerError));
            StringAssert.Contains($"Failed to get response ({ExpectedUri}) due to errors", exception.Message);
            StringAssert.Contains(InternalServerError, exception.Message);
        }

        [Test]
        public void GetHttpClientWorksWithoutProvidedHandler()
        {
            // This can also be proven working properly by manually running the integration tests for this feed.
            var feed = new RandomNameJsonFeed();
            Assert.IsNull(feed.Handler);

            feed.GetHttpClient();
            Assert.Pass("No exception was thrown, so HttpClient was constructed successfully.");

        }

        private static void TestValidResponse(string name, string surname, string gender, string region)
        {
            var nameDto = GetRandomNameResponse(name, surname, gender, region);

            Assert.AreEqual(name, nameDto.FirstName, "First name was wrong.");
            Assert.AreEqual(surname, nameDto.Surname, "Surname was wrong.");
            Assert.AreEqual(gender, nameDto.Gender, "Gender was wrong.");
            Assert.AreEqual(region, nameDto.Region, "Region was wrong.");
        }

        private static NameResponse GetRandomNameResponse(string name, string surname, string gender, string region, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var handlerMock = CreateMessageHandlerMock(statusCode, MakeJson(name, surname, gender, region), ExpectedUri);
            var feed = new RandomNameJsonFeed(handlerMock.Object);
            
            return feed.GetRandomNameResponse();
        }

        private static string MakeJson(string name, string surname, string gender, string region)
        {
            // This was a real response that was tokenized for testing.
            return $@"{{""name"":""{name}"",""surname"":""{surname}"",""gender"":""{gender}"",""region"":""{region}""}}";
        }
    }
}