using System.Collections.Generic;
using System.Net.Http;

namespace JokeGenerator.Feeds
{
    //TODO: Summaries
    //TODO: Error handling
    public class ChuckNorrisJsonFeed : AbstractJsonFeed
    {
        public const string BaseUri = "https://api.chucknorris.io/";
        public const string RandomJokeEndpoint = "jokes/random";
        public const string CategoryToken = "{category}";
        public const string RandomJokeWithCategoryEndpoint = "jokes/random?category=" + CategoryToken;
        public const string AvailableCategoriesEndpoint = "jokes/categories";
        public const string ChuckNorris = "Chuck Norris";

        public ChuckNorrisJsonFeed(HttpMessageHandler handler = null) : base(BaseUri,  handler)
        {
        }

        public RandomJokeResponse GetRandomJoke(string category = null)
        {
            var requestUri = category != null
                ? RandomJokeWithCategoryEndpoint.Replace(CategoryToken, category)
                : RandomJokeEndpoint;

            return GetDeserializedResponse<RandomJokeResponse>(requestUri);
        }

        public IList<string> GetCategories()
        {
            return GetDeserializedResponse<List<string>>(AvailableCategoriesEndpoint);
        }
    }
}
