using System;

namespace JokeGenerator.Feeds
{
    //TODO: Summary
    public class JsonFeedException : Exception
    {
        public JsonFeedException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }
}