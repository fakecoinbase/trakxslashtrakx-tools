using System;
using Flurl;
using Flurl.Http.Testing;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Coinbase.Custody.Client.Interfaces;

namespace Trakx.Coinbase.Custody.Client.Tests.Unit.Endpoints
{
    public abstract class EndpointTests : IDisposable
    {
        private const string BaseUrl = "https://api.custody.coinbase.com/api/v1/";
        private const string PassPhrase = "passphrase";
        private const string ApiKey = "apiKey";

        protected HttpTest HttpTest;
        protected readonly string EndpointUrl;
        protected CoinbaseClient CoinbaseClient;

        protected EndpointTests(string endpointUrl)
        {
            EndpointUrl = Url.Combine(BaseUrl, endpointUrl);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCoinbaseLibrary(ApiKey, PassPhrase);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var apiConfig = serviceProvider.GetRequiredService<IApiKeyConfig>();
            CoinbaseClient = new CoinbaseClient(apiConfig);
            HttpTest = new HttpTest();
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            HttpTest.ShouldHaveCorrectHeader(ApiKey, PassPhrase);
            HttpTest?.Dispose();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}