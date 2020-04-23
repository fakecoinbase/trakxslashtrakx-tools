using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Trakx.Common.Sources.Web3;
using Trakx.Contracts.Set;
using Trakx.Contracts.Set.Core;

namespace Trakx.Common.Ethereum
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddEthereumInteraction(
            this IServiceCollection serviceCollection,
            string infuraApiKey = null,
            string accountPrivateKey = null,
            string network = "mainnet")
        {
            serviceCollection.AddSingleton<Nethereum.Web3.IWeb3, Nethereum.Web3.Web3>(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var apiKey = configuration?["INFURA_API_KEY"] ?? infuraApiKey;
                var ethereumNetwork = configuration?["ETH_NETWORK"] ?? network;
                var privateKey = configuration?["ETH_ACCOUNT_PRIVATE_KEY"] ?? accountPrivateKey;

                if (privateKey == null)
                    return new Web3($"https://{ethereumNetwork}.infura.io/v3/{apiKey}");

                var account = new Account(privateKey);
                return new Web3(account, $"https://{ethereumNetwork}.infura.io/v3/{apiKey}");

            });
            serviceCollection.AddWeb3Client();
            serviceCollection.AddTransient<ICoreService, CoreService>(serviceProvider
                => new CoreService(serviceProvider.GetService<IWeb3>(), DeployedContractAddresses.Core));
            serviceCollection.AddSingleton<ICompositionCreator, CompositionCreator>();
            serviceCollection.AddSingleton<IIndiceCreator, IndiceCreator>();
            return serviceCollection;
        }
    }
}
