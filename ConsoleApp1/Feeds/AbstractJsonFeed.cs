using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

//TODO: Namespaces
namespace JokeGenerator.Feeds
{
    //TODO: Summaries
    public abstract class AbstractJsonFeed
    {
        private readonly string _baseUri;
        private readonly HttpMessageHandler _handler;

        protected AbstractJsonFeed(string baseUri, HttpMessageHandler handler = null)
        {
            _baseUri = baseUri;
            _handler = handler;
        }

        protected HttpClient GetHttpClient()
        {
            return new HttpClient(_handler ?? new HttpClientHandler()) { BaseAddress = new Uri(_baseUri) };
        }

        protected T GetDeserializedResponse<T>(string requestUri = "")
        {
            Task<string> result;

            //TODO: Configurable timeout?

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