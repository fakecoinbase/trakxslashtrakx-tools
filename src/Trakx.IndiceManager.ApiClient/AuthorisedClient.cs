using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Trakx.Common.Utils;

namespace Trakx.IndiceManager.ApiClient
{
    /// <summary>
    /// Credits to https://github.com/sanderaernouts/autogenerate-api-client-with-nswag 👍
    /// </summary>
    internal abstract class AuthorisedClient
    {
        public Func<Task<string>> RetrieveAuthorizationToken = MockJwtTokens.GenerateJwtToken();

        // Called by implementing swagger client classes
        protected async Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken cancellationToken)
        {
            var msg = new HttpRequestMessage();

            if (RetrieveAuthorizationToken == null) return msg;

            var token = await RetrieveAuthorizationToken().ConfigureAwait(false);
            msg.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            return msg;
        }

    }
}
