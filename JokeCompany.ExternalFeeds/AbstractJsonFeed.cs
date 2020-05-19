using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JokeCompany.ExternalFeeds
{
    /// <summary>
    /// Base class for JSON feeds wrappers.
    /// </summary>
    public abstract class AbstractJsonFeed
    {
        private readonly string _baseUri;
        protected internal HttpMessageHandler Handler { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="baseUri">Base Uri for API endpoints.</param>
        /// <param name="handler"><see cref="HttpMessageHandler"/> instance. This should only be provided for unit testing.</param>
        protected AbstractJsonFeed(string baseUri, HttpMessageHandler handler = null)
        {
            _baseUri = baseUri;
            Handler = handler;
        }

        /// <summary>
        /// Simple factory method which returns a new <see cref="HttpClient"/>. Will insert <see cref="Handler"/> if provided for unit testing.
        /// </summary>
        /// <returns>New <see cref="HttpClient"/>.</returns>
        protected internal HttpClient GetHttpClient()
        {
            var client = Handler != null ? new HttpClient(Handler) : new HttpClient();
            client.BaseAddress = new Uri(_baseUri);
            
            return client;
        }

        /// <summary>
        /// Gets a response from the JSON API and deserializes to the requested type <paramref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to deserialize to.</typeparam>
        /// <param name="requestUri">Uri to use for request.</param>
        /// <returns>Instance of T.</returns>
        protected T GetDeserializedResponse<T>(string requestUri = "")
        {
            Task<string> result;

            using (var client = GetHttpClient())
            {
                try
                {
                    result = Task.FromResult(client.GetStringAsync(requestUri).Result);
                }
                catch (AggregateException e)
                {
                    // Provide a nicer message than the aggregate provides.
                    var message = $"Failed to get response ({_baseUri}{requestUri}) due to errors: {Environment.NewLine}{string.Join(Environment.NewLine, e.InnerExceptions.Select(i => i.Message))}";
                    
                    throw new JsonFeedException(message, e);
                }
            }

            if (result.IsCompletedSuccessfully)
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(result.Result);
                }
                catch (JsonReaderException e)
                {
                    throw new JsonFeedException($"Failed to deserialize response ({_baseUri}{requestUri}) due to error: {e.Message}", e);
                }
            }

            // It's not expected that we'd ever get here, as an exception should have been thrown first - but just in case. We'll dump some info from the result in case that helps.
            throw new JsonFeedException($"Failed to get response ({_baseUri}{requestUri}) due to unknown error.  " +
                                        $"IsCanceled: {result.IsCanceled} IsCompleted: {result.IsCompleted} IsFaulted: {result.IsFaulted}");
        }
    }
}