using Microsoft.Extensions.DependencyInjection;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Ardalis.GuardClauses;
using Flurl.Http;
using Trakx.Coinbase.Custody.Client.Endpoints;

namespace Trakx.Coinbase.Custody.Client
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddCoinbaseLibrary(
            this IServiceCollection serviceCollection,
            string apiKey, string apiPassPhrase)
        {
            Guard.Against.NullOrEmpty(apiKey,nameof(apiKey));
            Guard.Against.NullOrEmpty(apiPassPhrase, nameof(apiPassPhrase));

            serviceCollection.AddSingleton<IApiKeyConfig, ApiKeyConfig>(serviceProvider =>
                new ApiKeyConfig(apiKey, apiPassPhrase));

            serviceCollection.AddSingleton<IFlurlClient>(serviceProvider =>
            {
                var client = new FlurlClient("https://api.custody.coinbase.com/api/v1/");
                var config = serviceProvider.GetService<IApiKeyConfig>();
                config.Configure(client);
                return client;
            });

            serviceCollection.AddSingleton<IAddressEndpoint, AddressEndpoint>();
            serviceCollection.AddSingleton<ICurrencyEndpoint, CurrencyEndpoint>();
            serviceCollection.AddSingleton<ITransactionEndpoint, TransactionEndpoint>();
            serviceCollection.AddSingleton<IWalletEndpoint, WalletEndpoint>();

            serviceCollection.AddTransient<ICoinbaseClient, CoinbaseClient>();

            return serviceCollection;
        }
    }
}
