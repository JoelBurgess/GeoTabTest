using System.Collections.Generic;

namespace JokeCompany.ExternalFeeds.ChuckNorris
{
    public interface IGetRandomJokes
    {
        /// <summary>
        /// Gets a random joke.
        /// </summary>
        /// <param name="category">Category to filter by, or null for any.</param>
        /// <returns>Joke as string.</returns>
        string GetRandomJoke(string category = null);

        /// <summary>
        /// Gets available categories for jokes.
        /// </summary>
        /// <returns>Available categories.</returns>
        IList<string> GetCategories();
    }
}