using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JokeGenerator
{
    //TODO: Summaries, SORT (and other classes)
    public class ConsoleHelper
    {
        // These allow leading/trailing whitespace to be more user-friendly.
        private readonly Regex _regexNo = new Regex(@"^\s*[Nn]([Oo])?\s*$", RegexOptions.Compiled);
        private readonly Regex _regexYes = new Regex(@"^\s*[Yy]([Ee][Ss])?\s*$", RegexOptions.Compiled);

        public ConsoleHelper(Encoding encoding)
        {
            Console.OutputEncoding = encoding;
        }

        public bool IsResponseYes(string prompt)
        {
            return PromptYesNo(prompt) == Response.Yes;
        }

        private Response PromptYesNo(string prompt)
        {
            Response? response = null;

            while (!response.HasValue)
            {
                WriteLine($"{prompt} y/n");
                var line = ReadLine();
                
                response = GetYesNoResponse(line);

                if (!response.HasValue)
                {
                    WriteLine("Invalid selection.");
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
                else if (_regexNo.IsMatch(line))
                {
                    return Response.No;
                }
            }

            return default;
        }

        public short PromptForDigit(string prompt)
        {
            short? response = null;

            while (!response.HasValue)
            {
                WriteLine($"{prompt} (1-9)");
                var line = ReadLine();

                response = GetSingleDigit(line);

                if (!response.HasValue)
                {
                    WriteLine("Invalid selection.");
                }
            }

            Clear();

            return response.Value;
        }

        public virtual void Clear()
        {
            Console.Clear();
        }

        public short? GetSingleDigit(string line)
        {
            var isNullOrWhiteSpace = string.IsNullOrWhiteSpace(line);

            if (!isNullOrWhiteSpace)
            {
                if (short.TryParse(line.Trim(), out var parsedNumber) && parsedNumber > 0 && parsedNumber < 10)
                {
                    return parsedNumber;
                }
            }

            return default;
        }


        public void PrintResults(IEnumerable<string> results)
        {
            foreach (var result in results)
            {
                foreach (var line in WordWrap(result))
                {
                    WriteLine(line);
                }
                
                WriteEmptyLine();
            }

            WriteEmptyLine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="header"></param>
        /// <param name="possibleAnswers">Possible answers the user can type in. Cannot have leading/trailing whitespace or be numbers.</param>
        /// <returns></returns>
        public string PromptForListSelection(string header, IList<string> possibleAnswers)
        {
            if (possibleAnswers?.Any() != true)
            {
                throw new ArgumentException("No answers were provided to choose from.", nameof(possibleAnswers));
            }

            while (true)
            {
                WriteLine(header);
                
                for (var i = 0; i < possibleAnswers.Count; i++)
                {
                    WriteLine($"{i + 1} - {possibleAnswers[i]}");
                }

                WriteLine("Please choose by entering the name or the corresponding number:");
                var answer = ReadLine();

                if (!string.IsNullOrWhiteSpace(answer))
                {
                    answer = answer.Trim();

                    if (int.TryParse(answer, out var index))
                    {
                        if (possibleAnswers.Count >= index)
                        {
                            return possibleAnswers[index - 1];
                        }
                    }
                    else
                    {
                        var match = possibleAnswers.FirstOrDefault(a => a.Equals(answer, StringComparison.InvariantCultureIgnoreCase));

                        if (match != default)
                        {
                            return match;
                        }
                    }
                }
            }
        }

        public virtual string ReadLine()
        {
            return Console.ReadLine();
        }

        public void WriteEmptyLine()
        {
            WriteLine(string.Empty);
        }

        public virtual void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        private IEnumerable<string> WordWrap(string result)
        {
            var words = result.Split(' ');

            var wrappedLines = words.Skip(1).Aggregate(words.Take(1).ToList(), (lines, nextWord) =>
            {
                // Line would overflow, go to next line.
                if (lines.Last().Length + nextWord.Length >= Console.WindowWidth)
                {
                    lines.Add(nextWord);
                }
                else // Current line has capacity for the current word.
                {
                    lines[lines.Count - 1] += " " + nextWord;
                }

                return lines;
            });

            return wrappedLines;
        }

        public enum Response
        {
            Yes,
            No
        }
    }
}