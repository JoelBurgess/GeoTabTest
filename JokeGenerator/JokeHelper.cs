using System.Collections.Generic;
using JokeCompany.ExternalFeeds.ChuckNorris;
using JokeCompany.ExternalFeeds.RandomName;

namespace JokeGenerator
{
    /// <summary>
    /// Provides jokes for JokeHelper."/>
    /// </summary>
    public class JokeHelper
    {
        private readonly IGetRandomJokes _jokeFeed;
        private readonly IGetRandomName _randomNameFeed;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jokeFeed">Feed to use for jokes.</param>
        /// <param name="randomNameFeed">Feed to use for random names.</param>
        public JokeHelper(IGetRandomJokes jokeFeed, IGetRandomName randomNameFeed)
        {
            _jokeFeed = jokeFeed;
            _randomNameFeed = randomNameFeed;
        }

        /// <summary>
        /// Gets <paramref name="jokeCount"/> random jokes, in the category <paramref name="category"/> (or any if null), with support for replacement names <paramref name="useReplacementName"/>.
        /// </summary>
        /// <param name="jokeCount">How many jokes?</param>
        /// <param name="category">Category to filter by (or null for any).</param>
        /// <param name="nameToReplace">Name to replace if replacing name, null otherwise.</param>
        /// <returns>Generated jokes.</returns>
        public IEnumerable<string> GetRandomJokes(int jokeCount, string category = null, string nameToReplace = null)
        {
            var useReplacementName = nameToReplace != null;
            var replacementName = useReplacementName ? _randomNameFeed.GetRandomFullName() : null;

            for (var i = 0; i < jokeCount; i++)
            {
                var result = _jokeFeed.GetRandomJoke(category);

                var finalJoke = useReplacementName ? result.Replace(nameToReplace, replacementName) : result;

                yield return finalJoke; // Return jokes as we get them for nicer UX.
            }
        }
    }
}