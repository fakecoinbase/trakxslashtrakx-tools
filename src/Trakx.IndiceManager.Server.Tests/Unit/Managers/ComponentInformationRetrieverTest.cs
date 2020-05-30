using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Models;
using Trakx.Common.Sources.Web3.Client;
using Trakx.IndiceManager.Server.Managers;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;
using ICoinGeckoClient = Trakx.Common.Sources.CoinGecko.ICoinGeckoClient;

namespace Trakx.IndiceManager.Server.Tests.Unit.Managers
{
    public class ComponentInformationRetrieverTest
    {
        private readonly IComponentInformationRetriever _componentInformationRetriever;
        private readonly IComponentDataProvider _componentDataProvider;
        private readonly MockCreator _mockCreator;
        private readonly ICoinGeckoClient _coinGeckoClient;
        private readonly IComponentDataCreator _componentDataCreator;
        private readonly IWeb3Client _web3;

        public ComponentInformationRetrieverTest(ITestOutputHelper output)
        {
            _componentDataProvider = Substitute.For<IComponentDataProvider>();
            _mockCreator = new MockCreator(output);
            _web3 = Substitute.For<IWeb3Client>();
            _coinGeckoClient = Substitute.For<ICoinGeckoClient>();
            _componentDataCreator = Substitute.For<IComponentDataCreator>();
            _componentInformationRetriever = new ComponentInformationRetriever(_web3, _coinGeckoClient, _componentDataProvider,_componentDataCreator);
        }

        [Fact]
        public async Task GetComponentFromAddress_should_return_component_from_database()
        {
            var component = GetRandomComponentDefinition(); 
            _componentDataProvider.GetComponentFromDatabaseByAddress(component.Address)
                .Returns(component);

            var result = await _componentInformationRetriever.GetComponentDefinitionFromAddress(component.Address);
            await _coinGeckoClient.DidNotReceiveWithAnyArgs().GetCoinGeckoIdFromSymbol(null);
            await _web3.DidNotReceiveWithAnyArgs().GetSymbolFromContractAddress(null);
            await _web3.DidNotReceiveWithAnyArgs().GetDecimalsFromContractAddress(null);
            result.Should().NotBeNull();
            result.Symbol.Should().Be(component.Symbol);
            result.Decimals.Should().Be(component.Decimals);
        }

        [Fact]
        public async Task GetComponentFromAddress_Should_Send_null_because_ERC20_Dont_Exist()
        {
            var address = _mockCreator.GetRandomAddressEthereum();
            _componentDataProvider.GetComponentFromDatabaseByAddress(address)
                .Returns((IComponentDefinition)null);

            _coinGeckoClient.GetCoinGeckoIdFromSymbol("here").ThrowsForAnyArgs(new Exception());

            var result = await _componentInformationRetriever.GetComponentDefinitionFromAddress(address);
            result.Should().Be(null);
        }

        [Fact]
        public async Task GetAllComponents_should_return_empty_list_if_database_empty()
        {
            _componentDataProvider.GetAllComponentsFromDatabase().Returns(new List<IComponentDefinition>());

            var result = await _componentInformationRetriever.GetAllComponents();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllComponents_should_return_list_of_IComponentDefinition()
        {
            var component = GetRandomComponentDefinition();
            _componentDataProvider.GetAllComponentsFromDatabase().Returns(new List<IComponentDefinition>{component});

            var result = await _componentInformationRetriever.GetAllComponents();
            result.Count.Should().Be(1);
            result[0].Address.Should().Be(component.Address);
            result[0].CoinGeckoId.Should().Be(component.CoinGeckoId);
            result[0].Decimals.Should().Be(component.Decimals);
            result[0].Name.Should().Be(component.Name);
        }
        
        [Fact]
        public async Task TryToSaveComponentDefinition_should_return_true_if_addition_in_database_succeed()
        {
            _componentDataCreator.TryAddComponentDefinition(default).ReturnsForAnyArgs(true);

            var result = await _componentInformationRetriever.TryToSaveComponentDefinition(
                new ComponentDetailModel(GetRandomComponentDefinition()));

            result.Should().BeTrue();
        }

        [Fact]
        public async Task TryToSaveComponentDefinition_should_return_false_if_addition_in_database_failed()
        {
            _componentDataCreator.TryAddComponentDefinition(default).ReturnsForAnyArgs(false);

            var result = await _componentInformationRetriever.TryToSaveComponentDefinition(
                new ComponentDetailModel(GetRandomComponentDefinition()));

            result.Should().BeFalse();
        }


        private IComponentDefinition GetRandomComponentDefinition()
        {
            var component = _mockCreator.GetComponentQuantity().ComponentDefinition;
            return component;
        }
    }
}
