using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JokeCompany.Utility
{
    /// <summary>
    ///     Helper class for console apps.
    /// </summary>
    public class ConsoleHelper
    {
        private const string InvalidSelection = "Invalid selection.";

        /// <summary>
        ///     Response from Yes/No prompt (for clarity over bool, and could be extended e.g. to allow 'Quit' support at any
        ///     time).
        /// </summary>
        public enum Response
        {
            Yes,
            No
        }

        // These allow leading/trailing whitespace to be more user-friendly.
        private readonly Regex _regexNo = new Regex(@"^\s*[Nn]([Oo])?\s*$", RegexOptions.Compiled);
        private readonly Regex _regexYes = new Regex(@"^\s*[Yy]([Ee][Ss])?\s*$", RegexOptions.Compiled);


        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="encoding">Text encoding to use for output (updates <see cref="Console" /> for this process).</param>
        public ConsoleHelper(Encoding encoding)
        {
            Console.OutputEncoding = encoding;
        }

        /// <summary>
        ///     Did the user respond to the yes/no prompt with yes? Will repeat until a valid yes/no is answered.
        /// </summary>
        /// <param name="prompt">Prompt to ask user.</param>
        /// <returns>True if user answered yes, false otherwise.</returns>
        public bool IsResponseYes(string prompt)
        {
            return PromptYesNo(prompt) == Response.Yes;
        }

        /// <summary>
        ///     Prompts the user for a single digit between 1 to 9.
        /// </summary>
        /// <param name="prompt">Prompt to display.</param>
        /// <returns>Digit selected by user.</returns>
        public byte PromptForDigit(string prompt)
        {
            ValidatePrompt(prompt);

            byte? response = null;

            while (!response.HasValue)
            {
                WriteLine($"{prompt} (1-9)");
                var line = ReadLine();

                response = GetSingleDigit(line);

                if (!response.HasValue)
                {
                    WriteLine(InvalidSelection);
                }
            }

            Clear();

            return response.Value;
        }

        /// <summary>
        ///     Clears the console.
        ///     Virtual for unit testing.
        /// </summary>
        public virtual void Clear()
        {
            Console.Clear();
        }

        /// <summary>
        /// Prints the provided strings to the console (with word wrapping based on console width).
        /// </summary>
        /// <param name="results">Strings to output.</param>
        public void PrintResults(IEnumerable<string> results)
        {
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            var windowWidth = GetWindowWidth();

            foreach (var result in results.Where(r => r != null))
            {
                foreach (var line in result.WordWrap(windowWidth))
                {
                    WriteLine(line);
                }

                WriteEmptyLine();
            }

            WriteEmptyLine();
        }

        /// <summary>
        /// </summary>
        /// <param name="header"></param>
        /// <param name="possibleAnswers">Possible answers the user can type in. Cannot have leading/trailing whitespace or be numbers.
        /// </param>
        /// <returns></returns>
        public string PromptForListSelection(string header, IList<string> possibleAnswers)
        {
            if (string.IsNullOrWhiteSpace(header))
            {
                throw new ArgumentException("No header was provided.", nameof(header));
            }

            var sortedAnswers = possibleAnswers?.Where(p => p != null).OrderBy(p => p).ToList() ?? new List<string>();

            if (sortedAnswers?.Any() != true)
            {
                throw new ArgumentException("No answers were provided to choose from.", nameof(sortedAnswers));
            }

            while (true)
            {
                WriteLine(header);

                for (var i = 0; i < sortedAnswers.Count; i++)
                {
                    WriteLine($"{i + 1} - {sortedAnswers[i]}");
                }

                WriteLine("Please choose by entering the name or the corresponding number:");
                var answer = ReadLine();

                if (!string.IsNullOrWhiteSpace(answer))
                {
                    answer = answer.Trim();

                    if (int.TryParse(answer, out var index))
                    {
                        if (sortedAnswers.Count >= index && index > 0)
                        {
                            return sortedAnswers[index - 1]; // Adjust because we started at 1 instead of 0.
                        }
                    }
                    else
                    {
                        var match = sortedAnswers.FirstOrDefault(a => a.Equals(answer, StringComparison.InvariantCultureIgnoreCase));

                        if (match != default)
                        {
                            return match;
                        }
                    }
                }

                WriteLine(InvalidSelection);
            }
        }

        /// <summary>
        ///     Gets the window width for the console.
        ///     Virtual for unit testing.
        /// </summary>
        public virtual int GetWindowWidth()
        {
            return Console.WindowWidth;
        }

        /// <summary>
        ///     Reads a line from the console.
        ///     Virtual for unit testing.
        /// </summary>
        public virtual string ReadLine()
        {
            return Console.ReadLine();
        }

        public virtual void WriteEmptyLine()
        {
            WriteLine(string.Empty);
        }

        /// <summary>
        ///     Writes a line to the console.
        ///     Virtual for unit testing.
        /// </summary>
        public virtual void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        private byte? GetSingleDigit(string line)
        {
            var isNullOrWhiteSpace = string.IsNullOrWhiteSpace(line);

            if (!isNullOrWhiteSpace)
            {
                if (byte.TryParse(line.Trim(), out var parsedNumber) && parsedNumber > 0 && parsedNumber < 10)
                {
                    return parsedNumber;
                }
            }

            return default;
        }

        private Response PromptYesNo(string prompt)
        {
            ValidatePrompt(prompt);

            Response? response = null;

            while (!response.HasValue)
            {
                WriteLine($"{prompt} y/n");
                var line = ReadLine();

                response = GetYesNoResponse(line);

                if (!response.HasValue)
                {
                    WriteLine(InvalidSelection);
                }
            }

            Clear();

            return response.Value;
        }

        private Response? GetYesNoResponse(string line)
        {
            var isNullOrWhiteSpace = string.IsNullOrWhiteSpace(line);

            if (!isNullOrWhiteSpace)
            {
                if (_regexYes.IsMatch(line))
                {
                    return Response.Yes;
                }

                if (_regexNo.IsMatch(line))
                {
                    return Response.No;
                }
            }

            return default;
        }

        private static void ValidatePrompt(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentException("No prompt was provided.", nameof(prompt));
            }
        }
    }
}