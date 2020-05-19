using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JokeCompany.ExternalFeeds.ChuckNorris;
using JokeCompany.ExternalFeeds.RandomName;
using JokeCompany.Utility;

namespace JokeGenerator
{
    public class Program
    {
        private const string ErrorLog = "Error.log";
        private static readonly Lazy<IList<string>> LazyCategories = new Lazy<IList<string>>(() => new ChuckNorrisJsonFeed().GetCategories());
        private static IList<string> Categories => LazyCategories.Value;

        /// <summary>
        /// Main program logic/loop.
        /// </summary>
        static void Main()
        {
            var consoleHelper = new ConsoleHelper(Encoding.UTF8); // Windows console defaults to ASCII, which doesn't play nice with some of the random names.
            consoleHelper.WriteLine($"Welcome to Joke Company's Chuck Norris Joke Generator.{Environment.NewLine}");

            var jokeHelper = new JokeHelper(new ChuckNorrisJsonFeed(), new RandomNameJsonFeed());
            var getMoreJokes = true;

            while (getMoreJokes)
            {
                try
                {
                    var useReplacementName = consoleHelper.IsResponseYes("Would you like to use a random name?");
                    var useCategory = consoleHelper.IsResponseYes("Would you like to specify a category?");
                    var category = useCategory ? consoleHelper.PromptForListSelection("Available categories:", Categories) : null;
                    var numberOfJokes = consoleHelper.PromptForDigit("How many jokes would you like?");

                    consoleHelper.WriteLine("Thinking up some good jokes..."); // API may cause delay, so show a message (similar to the famous 'Reticulating Splines').

                    var jokes = jokeHelper.GetRandomJokes(numberOfJokes, category, useReplacementName ? ChuckNorrisJsonFeed.ChuckNorris : null);
                    consoleHelper.PrintResults(jokes);

                    getMoreJokes = consoleHelper.IsResponseYes("Would you like more jokes?");
                }
                catch (Exception e)
                {
                    consoleHelper.WriteLine("Sorry, an error occurred. If this continues, please contact Joke Company support.");
                    consoleHelper.WriteLine("\"It works on my machine\" always holds true for Chuck Norris.");

                    Log(e);
                }

            }

            consoleHelper.WriteLine("Thanks for playing Chuck Norris Joke Generator.");
        }

        private static void Log(Exception exception)
        {
            // In future, consider creating a wrapping library (e.g. log4net, NLog) in shared class, but for now this should be all we need.
            File.WriteAllText(ErrorLog, exception.ToString());
        }
    }
}
