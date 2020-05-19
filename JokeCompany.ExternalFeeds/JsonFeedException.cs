using System;

namespace JokeCompany.ExternalFeeds
{
    /// <summary>
    /// Represents exception which occurred in JSON feed.
    /// </summary>
    public class JsonFeedException : Exception
    {
        /// <inheritdoc />
        public JsonFeedException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }
}