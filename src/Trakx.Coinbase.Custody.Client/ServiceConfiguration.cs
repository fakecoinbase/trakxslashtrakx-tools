﻿using Microsoft.Extensions.DependencyInjection;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Ardalis.GuardClauses;
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
            serviceCollection.AddTransient<ICoinbaseClient, CoinbaseClient>();
            serviceCollection.AddTransient<IAddressEndpoint, AddressEndpoint>();
            serviceCollection.AddTransient<IWalletEndpoint, WalletEndpoint>();
            serviceCollection.AddTransient<ITransactionEndpoint, TransactionEndpoint>();
            return serviceCollection;
        }
    }
}
