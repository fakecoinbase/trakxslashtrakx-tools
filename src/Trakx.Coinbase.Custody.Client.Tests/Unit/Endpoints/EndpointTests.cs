using System;
using Flurl;
using Flurl.Http.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Coinbase.Custody.Client.Tests.Unit.Endpoints
{
    public abstract class EndpointTests : IDisposable
    {
        private const string PassPhrase = "passphrase";
        private const string ApiKey = "apiKey";

        protected HttpTest HttpTest;
        protected readonly string EndpointUrl;
        protected string SampleResponse;
        protected readonly ServiceProvider ServiceProvider;

        protected EndpointTests(string endpointPath)
        {
            EndpointUrl = Url.Combine("https://api.custody.coinbase.com/api/v1/", endpointPath);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCoinbaseLibrary(ApiKey, PassPhrase);
            ServiceProvider = serviceCollection.BuildServiceProvider();
            
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