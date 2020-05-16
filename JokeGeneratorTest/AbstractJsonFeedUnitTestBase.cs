using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace JokeGeneratorTest
{
    //TODO: Document
    public abstract class AbstractJsonFeedUnitTestBase
    {
        // These error snippets are from HttpClient so they could potentially change in the future (but highly unlikely).
        protected const string NotFound = "404 (Not Found)";
        protected const string InternalServerError = "500 (Internal Server Error)";
        private const string SendAsync = "SendAsync"; // Would use nameof but SendAsync is protected.

        protected static Mock<HttpMessageHandler> CreateMessageHandlerMock(HttpStatusCode httpStatusCode, string content, string expectedUri)
        {
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = httpStatusCode,
                Content = new StringContent(content),
            };

            var handlerMock = new Mock<HttpMessageHandler>();

            // Configure mock to throw exception for all SendAsync calls.
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(SendAsync, ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new Exception("Wrong method or URI was requested."))
                .Verifiable();

            // Now configure mock to handle the message that we expect (or continue to throw exception for anything else).
            // Mocking API responses is kind of pointless if we don't verify that the correct request was made.
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(SendAsync, 
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri == new Uri(expectedUri)), 
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();

            return handlerMock;
        }
    }
}