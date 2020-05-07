using System.Threading.Tasks;
using FluentAssertions;
using Trakx.Common.Sources.CoinGecko;
using Trakx.IndiceManager.Server.Managers;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Common.Ethereum;
using Trakx.Common.Sources.Web3.Client;
using Trakx.Persistence;
using Trakx.Persistence.DAO;
using Trakx.Tests.Tools;
using Trakx.Tests.Unit.Models;
using Xunit;


namespace Trakx.IndiceManager.Server.Tests.Integration.Managers
{
    public class ComponentInformationRetrieverTest 
    {
        private readonly IComponentInformationRetriever _componentInformationRetriever;

        public ComponentInformationRetrieverTest() 
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddEthereumInteraction(Secrets.InfuraApiKey);
            serviceCollection.AddMemoryCache();
            serviceCollection.AddCoinGeckoClient();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var coinGeckoClient = serviceProvider.GetRequiredService<ICoinGeckoClient>();
            var web3 = serviceProvider.GetRequiredService<IWeb3Client>();
            var fixture = new SeededDbContextFixture();
            IndiceRepositoryContext context = fixture.Context;

            _componentInformationRetriever = new ComponentInformationRetriever(context, web3,coinGeckoClient);
        }


        [Fact]
        public async Task GetComponentFromAddress_Should_Send_Back_A_Component_In_Database()
        {
            var componentInDatabase = new ComponentDefinitionDao("0x50d1c9771902476076ecfc8b2a83ad6b9355a4c9", "FTX Token", "ftt",
                "ftx-token", 18);

            var result =
                await _componentInformationRetriever.GetComponentDefinitionFromAddress(componentInDatabase.Address);

            result.Address.Should().Be(componentInDatabase.Address);
            result.Name.Should().Be(componentInDatabase.Name);
            result.Decimals.Should().Be(componentInDatabase.Decimals); 
            result.CoinGeckoId.Should().Be(componentInDatabase.CoinGeckoId);
            result.Symbol.Should().Be(componentInDatabase.Symbol);
        }

        [Fact(Skip = "Should have an Infura Key to work")]
        public async Task GetComponentFromAddress_Should_Send_back_An_Existing_ERC20_Not_In_Database()
        { 
            var address = "0xdac17f958d2ee523a2206206994597c13d831ec7";
            var symbol ="USDT";
            var name = "Tether USD";
            var decimals = 6;
            var result = await _componentInformationRetriever.GetComponentDefinitionFromAddress(address);

            result.Address.Should().Be(address);
            result.Symbol.Should().Be(symbol);
            result.Name.Should().Be(name);
            result.Decimals.Should().Be((ushort) decimals);
        }

        [Fact]
        public async Task GetComponentFromAddress_Should_Send_null_because_ERC20_Dont_Exist()
        {
            var address = "0x7ba6Fd7806eFbaD2b65F8c2Bd64A0918e365C581";

            var result = await _componentInformationRetriever.GetComponentDefinitionFromAddress(address);
            result.Should().Be(null);
        }
    }
}