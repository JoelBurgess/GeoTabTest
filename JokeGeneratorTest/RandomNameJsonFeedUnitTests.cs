using System.Net;
using JokeGenerator.Feeds;
using NUnit.Framework;

namespace JokeGeneratorTest
{
    [TestFixture]
    public class RandomNameJsonFeedUnitTests : AbstractJsonFeedUnitTestBase
    {
        private const string ExpectedUri = "https://names.privserv.com/api/";

        [Test]
        public void GetRandomNameWorksHandlesValidResponse()
        {
            TestValidResponse("Aureliana", "Varzar", "female", "Romania"); // Values from real API response
        }

        [Test]
        public void GetRandomNameWorksHandlesValidResponseWithUnicode()
        {
            TestValidResponse("Ζώσιμος", "Παπαστεφάνου", "male", "Greece"); // Values from real API response
        }

        [Test]
        public void GetRandomNameWorksHandlesValidResponseWithPunctuationInNames()
        {
            TestValidResponse("Aure,Liana", "D'Sousa-Smith", "female", "Romania");
        }

        [Test]
        public void GetRandomNameWorksHandlesValidResponseWithEmptyStrings()
        {
            TestValidResponse("", "", "", "");
        }

        [Test]
        public void GetRandomNameWorksHandlesValidResponseWithVeryLargeStrings()
        {
            var largeString = new string('a', 50000);
            TestValidResponse(largeString, largeString, largeString, largeString);
        }

        [Test]
        public void GetRandomNameWorksThrowsWhenJson()
        {
            var handlerMock = CreateMessageHandlerMock(HttpStatusCode.OK, "This is not { valid json ].", ExpectedUri);
            var feed = new RandomNameJsonFeed(handlerMock.Object);

            var exception = Assert.Throws<JsonFeedException>(() => feed.GetRandomName());
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

        private static void TestValidResponse(string name, string surname, string gender, string region)
        {
            var nameDto = GetRandomNameResponse(name, surname, gender, region);

            Assert.AreEqual(name, nameDto.FirstName, "First name was wrong.");
            Assert.AreEqual(surname, nameDto.Surname, "Surname was wrong.");
            Assert.AreEqual(gender, nameDto.Gender, "Gender was wrong.");
            Assert.AreEqual(region, nameDto.Region, "Region was wrong.");

            Assert.AreEqual($"{name} {surname}", nameDto.FullName, "Full name was wrong."); // FullName is generated, not from API.
        }

        private static NameResponse GetRandomNameResponse(string name, string surname, string gender, string region, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var handlerMock = CreateMessageHandlerMock(statusCode, MakeJson(name, surname, gender, region), ExpectedUri);
            var feed = new RandomNameJsonFeed(handlerMock.Object);
            
            return feed.GetRandomName();
        }

        private static string MakeJson(string name, string surname, string gender, string region)
        {
            // This was a real response that was tokenized for testing.
            return $@"{{""name"":""{name}"",""surname"":""{surname}"",""gender"":""{gender}"",""region"":""{region}""}}";
        }
    }
}