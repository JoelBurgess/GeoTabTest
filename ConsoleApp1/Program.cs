using System;
using System.Collections.Generic;
using System.Text;
using JokeGenerator.Feeds;

namespace JokeGenerator
{
    //TODO: Summaries
    //TODO: Error handling

    class Program
    {
        private static readonly Lazy<IList<string>> LazyCategories = new Lazy<IList<string>>(() => new ChuckNorrisJsonFeed().GetCategories());
        private static IList<string> Categories => LazyCategories.Value;

        //TODO: Args
        static void Main(string[] args)
        {
            var consoleHelper = new ConsoleHelper(Encoding.UTF8); // Windows console defaults to ASCII, which doesn't play nice with some of the random names.
            consoleHelper.WriteLine($"Welcome to Joke Company's Chuck Norris Joke Generator.{Environment.NewLine}");

            var getMoreJokes = true;
            
            while (getMoreJokes)
            {
                try
                {
                    var useReplacementName = consoleHelper.IsResponseYes("Would you like to use a random name?");
                    var useCategory = consoleHelper.IsResponseYes("Would you like to specify a category?");
                    var category = useCategory ? consoleHelper.PromptForListSelection("Available categories:", Categories) : null;
                    var numberOfJokes = consoleHelper.PromptForDigit("How many jokes would you like?");

                    consoleHelper.WriteLine("Thinking up some good jokes..."); // API could be a little slow, so show a 'Reticulating Splines' message :).

                    var jokes = JokeHelper.GetRandomJokes(category, numberOfJokes, useReplacementName);
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
        }

        private static void Log(Exception exception)
        {
            //TODO: This
            throw new NotImplementedException();
        }
    }
}
