using System.Collections.Generic;
using System.Net.Http;

namespace JokeCompany.ExternalFeeds.ChuckNorris
{
    /// <summary>
    /// Used to provide Chuck Norris jokes from <see cref="BaseUri"/>.  Documentation can be found at <see cref="BaseUri"/>.
    /// </summary>
    public class ChuckNorrisJsonFeed : AbstractJsonFeed, IGetRandomJokes
    {
        public const string ChuckNorris = "Chuck Norris";
        public const string BaseUri = "https://api.chucknorris.io/";
        public const string RandomJokeEndpoint = "jokes/random";
        public const string CategoryToken = "{category}";
        public const string RandomJokeWithCategoryEndpoint = "jokes/random?category=" + CategoryToken;
        public const string AvailableCategoriesEndpoint = "jokes/categories";


        /// <inheritdoc />
        public ChuckNorrisJsonFeed(HttpMessageHandler handler = null) : base(BaseUri,  handler)
        {
        }

        /// <inheritdoc />
        public string GetRandomJoke(string category = null)
        {
            return GetRandomJokeResponse(category).JokeText;
        }

        /// <summary>
        /// Gets a random Chuck Norris joke, with all details from API.
        /// </summary>
        /// <param name="category">Category to filter by, or null for any.</param>
        /// <returns><see cref="RandomJokeResponse"/> with values from API.</returns>
        public virtual RandomJokeResponse GetRandomJokeResponse(string category = null)
        {
            var requestUri = category != null ? RandomJokeWithCategoryEndpoint.Replace(CategoryToken, category) : RandomJokeEndpoint;

            return GetDeserializedResponse<RandomJokeResponse>(requestUri);
        }

        /// <inheritdoc />
        public IList<string> GetCategories()
        {
            return GetDeserializedResponse<List<string>>(AvailableCategoriesEndpoint);
        }
    }
}
