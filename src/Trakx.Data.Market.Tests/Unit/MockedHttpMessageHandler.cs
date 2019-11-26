using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Trakx.Data.Market.Tests.Unit
{
    public class MockedHttpMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(string.Empty)
            };
            return Task.FromResult(response);
        }
    }
}
