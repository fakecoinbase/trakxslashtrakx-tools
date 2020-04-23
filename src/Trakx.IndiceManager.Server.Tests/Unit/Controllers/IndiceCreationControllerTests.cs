using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Trakx.Common.Core;
using Trakx.Common.Interfaces.Indice;
using Trakx.IndiceManager.Server.Controllers;
using Trakx.IndiceManager.Server.Managers;
using Trakx.IndiceManager.Server.Models;
using Trakx.Tests.Data;
using Xunit;

namespace Trakx.IndiceManager.Server.Tests.Unit.Controllers
{
    public class IndiceCreationControllerTests
    {
        private readonly IComponentInformationRetriever _componentRetriever;
        private readonly IndiceCreationController _controller;
        private readonly MockCreator _mockCreator;
        private readonly IIndiceInformationRetriever _indiceRetriever;

        public IndiceCreationControllerTests()
        {
            _componentRetriever = Substitute.For<IComponentInformationRetriever>();
            _mockCreator = new MockCreator();
            _indiceRetriever = Substitute.For<IIndiceInformationRetriever>();
            _controller = new IndiceCreationController(_componentRetriever,_indiceRetriever);
            
        }

        [Fact]
        public async Task GetComponentByAddress_should_return_details_when_component_is_found()
        {
            var address = _mockCreator.GetRandomAddressEthereum();
            var symbol = "abcd";
            
            var expectedComponentDefinition = 
                new ComponentDefinition(address, "blabalba", symbol, "abcd-coin", 18);
            _componentRetriever.GetComponentDefinitionFromAddress(address)
                .Returns(expectedComponentDefinition);

            var details = await _controller.GetComponentByAddress(address);

            var result = (ComponentDetailModel)((JsonResult)details.Result).Value;
            result.Address.Should().Be(address);
            result.Symbol.Should().Be(symbol);

            await _componentRetriever.ReceivedWithAnyArgs(1)
                .GetComponentDefinitionFromAddress(default);
        }

        [Fact]
        public async Task GetComponentByAddress_should_not_try_to_retrieve_details_on_invalid_address()
        {
            var invalidAddress = _mockCreator.GetRandomAddressEthereum() + "abcde";
            await _controller.GetComponentByAddress(invalidAddress);

            await _componentRetriever.DidNotReceiveWithAnyArgs()
                .GetComponentDefinitionFromAddress(default);
        }

        [Fact]
        public async Task GetComponentByAddress_should_return_error_when_address_is_not_found_anywhere()
        {
            var address = _mockCreator.GetRandomAddressEthereum();
            await _controller.GetComponentByAddress(address);
            _componentRetriever.GetComponentDefinitionFromAddress(default).ReturnsForAnyArgs((ComponentDefinition)null);

            var details = await _controller.GetComponentByAddress(address);
            var result = (NotFoundObjectResult)details.Result;
            result.StatusCode.Should().Be(404);
            result.Value.Should().Be($"Sorry {address} doesn't correspond to any ERC20 token.");
        }

        [Fact]
        public async Task GetAllIndices_should_Send_A_List_Of_Detailed_Indices()
        {
            var listIndices = new List<IIndiceDefinition>();
            listIndices.Add(_mockCreator.GetRandomIndiceDefinition());
            listIndices.Add(_mockCreator.GetRandomIndiceDefinition());
            _indiceRetriever.GetAllIndicesFromDatabase().ReturnsForAnyArgs(listIndices);

            var result = await _controller.GetAllIndices();
            var list = (List<IndiceDetailModel>)((OkObjectResult)result.Result).Value;

            list.Count.Should().Be(2);
            list[0].Name.Should().Be(listIndices[0].Name);
            list[0].CreationDate.Should().Be(listIndices[0].CreationDate);
            list[1].Symbol.Should().Be(listIndices[1].Symbol);
        }

        [Fact]
        public async Task GettAllIndices_should_Send_Not_Found_If_Database_Is_Empty()
        {
            _indiceRetriever.GetAllIndicesFromDatabase().ReturnsForAnyArgs((List<IIndiceDefinition>)null);

            var result = await _controller.GetAllIndices();
            ((NotFoundObjectResult) result.Result).Value.Should().Be("There is no indices in the database.");

        }
    }
}