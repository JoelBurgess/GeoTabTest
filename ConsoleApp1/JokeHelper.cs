using System.Collections.Generic;
using JokeGenerator.Feeds;

namespace JokeGenerator
{
    //TODO: Summaries/tests
    public class JokeHelper
    {
        public static IEnumerable<string> GetRandomJokes(string category, int jokeCount, bool useReplacementName)
        {
            var feed = new ChuckNorrisJsonFeed();
            var replacementName = useReplacementName ? GetRandomName() : null;

            for (var i = 0; i < jokeCount; i++)
            {
                var result = feed.GetRandomJoke(category);

                var finalJoke = useReplacementName ? result.JokeText.Replace(ChuckNorrisJsonFeed.ChuckNorris, replacementName) : result.JokeText;

                yield return finalJoke; // Return jokes as we get them for nicer UX.
            }
        }

        private static string GetRandomName()
        {
            var feed = new RandomNameJsonFeed();
            var result = feed.GetRandomName();

            return result.FullName;
        }
    }
}