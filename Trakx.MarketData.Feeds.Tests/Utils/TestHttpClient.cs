using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using Trakx.MarketData.Feeds.Tests.ApiClients;

namespace Trakx.MarketData.Feeds.Tests.Utils
{
    public static class HttpClientFactory
    {
        public static IHttpClientFactory GetHttpClientFactory(string baseAddress, Func<HttpResponseMessage> expectedResponse)
        {
            var httpMessageHandler = Substitute.ForPartsOf<HttpMessageHandler>();

            var sendCall = httpMessageHandler.Protected("SendAsync", Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());

            sendCall.Returns(Task.FromResult(expectedResponse()));

            var httpClient = new HttpClient(httpMessageHandler) {BaseAddress = new Uri(baseAddress) };
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);
            return httpClientFactory;
        }
    }
}
