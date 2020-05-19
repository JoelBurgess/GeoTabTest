using System.Net.Http;

namespace JokeCompany.ExternalFeeds.RandomName
{
    /// <summary>
    /// Provides random names.
    /// </summary>
    public class RandomNameJsonFeed : AbstractJsonFeed, IGetRandomName
    {
        private const string BaseUri = "https://names.privserv.com/api/";
        
        /// <inheritdoc />
        public RandomNameJsonFeed(HttpMessageHandler handler = null) : base(BaseUri, handler)
        {
        }

        /// <inheritdoc />
        public string GetRandomFullName()
        {
            var response = GetRandomNameResponse();

            return $"{response.FirstName ?? string.Empty} {response.Surname ?? string.Empty}";
        }

        /// <summary>
        /// Gets a random name response from the API.
        /// </summary>
        /// <returns>Random name response.</returns>
        public NameResponse GetRandomNameResponse()
        {
            return GetDeserializedResponse<NameResponse>();
        }
    }
}