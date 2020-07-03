using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Trakx.Common.Ethereum;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Sources.CoinGecko;
using Trakx.Common.Sources.Web3.Client;
using Trakx.IndiceManager.Server.Managers;
using Trakx.Tests.Tools;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.IndiceManager.Server.Tests.Integration.Managers
{
    public class ComponentInformationRetrieverTest 
    {
        private readonly IComponentInformationRetriever _componentInformationRetriever;
        private readonly IComponentDataProvider _componentDataProvider;

        public ComponentInformationRetrieverTest(ITestOutputHelper output) 
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddEthereumInteraction(Secrets.InfuraApiKey);
            serviceCollection.AddMemoryCache();
            serviceCollection.AddCoinGeckoClient();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var coinGeckoClient = serviceProvider.GetRequiredService<ICoinGeckoClient>();
            var web3 = serviceProvider.GetRequiredService<IWeb3Client>();
            _componentDataProvider = Substitute.For<IComponentDataProvider>();
            var componentDataCreator = Substitute.For<IComponentDataCreator>();
            _componentInformationRetriever = new ComponentInformationRetriever(web3,coinGeckoClient,_componentDataProvider,componentDataCreator);
        }
        
        [Fact(Skip = "Should have an Infura Key to work")]
        public async Task GetComponentFromAddress_Should_Send_back_An_Existing_ERC20_Not_In_Database()
        {
            var address = "0xdac17f958d2ee523a2206206994597c13d831ec7";
            var symbol ="USDT";
            var name = "Tether USD";
            var decimals = 6;
            _componentDataProvider.GetComponentFromDatabaseByAddress(address).Returns((IComponentDefinition)null);

            var result = await _componentInformationRetriever.GetComponentDefinitionFromAddress(address);

            result.Address.Should().Be(address);
            result.Symbol.Should().Be(symbol);
            result.Name.Should().Be(name);
            result.Decimals.Should().Be((ushort) decimals);
        }
    }
}