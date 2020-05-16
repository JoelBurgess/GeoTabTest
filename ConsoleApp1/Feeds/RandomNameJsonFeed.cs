using System.Net.Http;

namespace JokeGenerator.Feeds
{
    //TODO: Summaries
    //TODO: Error handling
    public class RandomNameJsonFeed : AbstractJsonFeed
    {
        private const string BaseUri = "https://names.privserv.com/api/";

        public RandomNameJsonFeed(HttpMessageHandler handler = null) : base(BaseUri, handler)
        {
        }

        public NameResponse GetRandomName()
        {
            return GetDeserializedResponse<NameResponse>();
        }
    }
}