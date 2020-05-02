using System;
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
        public async Task GetAllIndices_should_Send_Not_Found_If_Database_Is_Empty()
        {
            _indiceRetriever.GetAllIndicesFromDatabase().ReturnsForAnyArgs((List<IIndiceDefinition>)null);

            var result = await _controller.GetAllIndices();
            ((NotFoundObjectResult)result.Result).Value.Should().Be("There is no indices in the database.");
        }

        [Fact]
        public async Task GetCompositionsBySymbol_should_send_back_compositions()
        {
            var symbol = _mockCreator.GetRandomIndiceSymbol();
            var compositions = new List<IIndiceComposition>
            {
                _mockCreator.GetIndiceComposition(4),
                _mockCreator.GetIndiceComposition(2)
            };
            
            _indiceRetriever.GetAllCompositionsFromDatabase(symbol).Returns(compositions);

            var result = await _controller.GetCompositionsBySymbol(symbol);
            var finalResult = (List<IndiceCompositionModel>)((OkObjectResult)result.Result).Value;

            finalResult[0].Symbol.Should().Be(compositions[0].Symbol);
            finalResult[0].Version.Should().Be(compositions[0].Version);
            finalResult[1].Components[0].Quantity.Should().Be(compositions[1].ComponentQuantities[0].Quantity);
            finalResult[1].Components[1].Symbol.Should()
                .Be(compositions[1].ComponentQuantities[1].ComponentDefinition.Symbol);
            finalResult[0].CreationDate.Should().Be(compositions[0].CreationDate);
        }

        [Fact]
        public async Task GetCompositionsBySymbol_should_not_send_compositions_when_indice_not_in_database()
        {
            var symbol = _mockCreator.GetRandomIndiceSymbol();

            _indiceRetriever.GetAllCompositionsFromDatabase(symbol).Returns((List<IIndiceComposition>)null);

            var result = await _controller.GetCompositionsBySymbol(symbol);
            ((NotFoundObjectResult) result.Result).Value.Should()
                .Be($"The indice attached to {symbol} is not in our database.");
        }

        [Fact]
        public async Task GetCompositionsBySymbol_should_send_back_error_when_indice_has_no_compositions()
        {
            var symbol = _mockCreator.GetRandomIndiceSymbol();
            var composition = new List<IIndiceComposition>();

            _indiceRetriever.GetAllCompositionsFromDatabase(symbol).Returns(composition);

            var result= await _controller.GetCompositionsBySymbol(symbol);
            var finalResult = ((NotFoundObjectResult) result.Result).Value;
            finalResult.Should().Be("There are no compositions for this indice.");
        }

        [Fact]
        public async Task SaveIndiceDefinition_should_send_error_when_address_is_invalid()
        {
            var invalidAddress = "45gervgeg";
            var indiceDefinition = new IndiceDetailModel
            {
                Address = invalidAddress
            };
            await _controller.SaveIndiceDefinition(indiceDefinition);
            await _indiceRetriever.DidNotReceiveWithAnyArgs().SearchIndice(default);
        }

        [Fact]
        public async Task SaveIndiceDefinition_should_send_error_if_indice_already_in_database()
        {
            var address = _mockCreator.GetRandomAddressEthereum();
            var indiceDefinition = new IndiceDetailModel
            {
                Address = address
            };
            _indiceRetriever.SearchIndice(address).Returns(true);
            await _controller.SaveIndiceDefinition(indiceDefinition);
            await _indiceRetriever.DidNotReceiveWithAnyArgs().TrySaveIndice(indiceDefinition);
        }

        [Fact]
        public async Task SaveIndiceDefinition_should_send_status_code201_when_all_is_good()
        {
            var address = _mockCreator.GetRandomAddressEthereum();
            var indiceDefinition = new IndiceDetailModel
            {
                Address = address
            };
            _indiceRetriever.SearchIndice(address).Returns(false);
            _indiceRetriever.TrySaveIndice(indiceDefinition).Returns(true);
            var result = await _controller.SaveIndiceDefinition(indiceDefinition);
            ((CreatedAtActionResult) result.Result).StatusCode.Should().Be(201);
            ((CreatedAtActionResult)result.Result).Value.Should().Be(indiceDefinition);
        }

        [Fact]
        public async Task SaveIndiceDefinition_should_send_error_when_addition_failed_in_database()
        {
            var address = _mockCreator.GetRandomAddressEthereum();
            var indiceDefinition = new IndiceDetailModel
            {
                Address = address
            };
            _indiceRetriever.SearchIndice(address).Returns(false);
            _indiceRetriever.TrySaveIndice(indiceDefinition).Returns(false);
            var result = await _controller.SaveIndiceDefinition(indiceDefinition);
            ((BadRequestObjectResult) result.Result).Value.Should().Be(
                "The addition in the database has failed. Please verify the parameters of the indice and try again.");
        }

        [Fact]
        public async Task SaveIndiceComposition_should_not_saveComposition_if_EthereumAddress_is_invalid()
        {
            var invalidAddress = "45gervgeg";
            var composition = new IndiceCompositionModel
            {
                Address = invalidAddress
            };
            await _controller.SaveIndiceComposition(composition);
            await _indiceRetriever.DidNotReceiveWithAnyArgs().SearchComposition(default);
        }

        [Fact]
        public async Task SaveIndiceComposition_should_not_save_composition_if_already_in_database()
        {
            var address = _mockCreator.GetRandomAddressEthereum();
            var composition = new IndiceCompositionModel
            {
                Address = address
            };
            _indiceRetriever.SearchComposition(address).Returns(true);
            await _controller.SaveIndiceComposition(composition);
            await _indiceRetriever.DidNotReceiveWithAnyArgs().TrySaveComposition(composition);
        }

        [Fact]
        public async Task SaveIndiceComposition_should_send_status_code201_when_all_is_good()
        {
            var address = _mockCreator.GetRandomAddressEthereum();
            var composition = new IndiceCompositionModel
            {
                Address = address
            };
            _indiceRetriever.SearchComposition(address).Returns(false);
            _indiceRetriever.TrySaveComposition(composition).Returns(true);
            var result = await _controller.SaveIndiceComposition(composition);
            ((CreatedAtActionResult)result.Result).StatusCode.Should().Be(201);
            ((CreatedAtActionResult)result.Result).Value.Should().Be(composition);
        }

        [Fact]
        public async Task SaveIndiceComposition_should_send_error_when_addition_failed_in_database()
        {
            var address = _mockCreator.GetRandomAddressEthereum();
            var composition = new IndiceCompositionModel
            {
                Address = address
            };
            _indiceRetriever.SearchComposition(address).Returns(false);
            _indiceRetriever.TrySaveComposition(composition).Returns(false);
            var result = await _controller.SaveIndiceComposition(composition);
            ((BadRequestObjectResult)result.Result).Value.Should().Be(
                "The addition in the database has failed. Please verify the parameters of the composition and try again.");
        }
    }
}
