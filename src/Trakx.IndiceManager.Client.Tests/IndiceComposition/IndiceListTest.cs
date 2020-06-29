using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Syncfusion.Blazor;
using Trakx.Common.Models;
using Trakx.IndiceManager.ApiClient;
using Trakx.IndiceManager.Client.Pages.IndiceComposition;
using Trakx.Persistence.Tests;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.IndiceManager.Client.Tests.IndiceComposition
{
    public sealed class IndiceListTest : ComponentTest<IndiceList>
    {
        private readonly MockCreator _mockCreator;
        private readonly IIndiceCreationClient _apiClient;
        private readonly List<IndiceDetailModel> _expectedIndices;

        public IndiceListTest(ITestOutputHelper output)
        {
            Services.AddSyncfusionBlazor();
            Services.AddSingleton(Substitute.For<ILogger<IndiceList>>());
            var navigationManager = Substitute.For<NavigationManager>();
            Services.AddSingleton(navigationManager);
            
            _apiClient = Substitute.For<IIndiceCreationClient>();
            Services.AddSingleton(_apiClient);

            _mockCreator = new MockCreator(output);

            _expectedIndices = SetupMockIndicesResponse();
            Component = RenderComponent<IndiceList>();
        }

        private List<IndiceDetailModel> SetupMockIndicesResponse()
        {
            var indiceDetailModel = new IndiceDetailModel(_mockCreator.GetRandomIndiceDefinition());
            var indices = new List<IndiceDetailModel> {indiceDetailModel};
            var indicesResponse =
                new Response<List<IndiceDetailModel>>((int) HttpStatusCode.Created, default, indices);
            _apiClient.GetAllIndicesAsync().ReturnsForAnyArgs(indicesResponse);
            return indices;
        }

        [Fact]
        public void IndexList_should_be_loaded_upon_initialisation ()
        {
            _apiClient.ReceivedWithAnyArgs(1).GetAllIndicesAsync();
            Component.Instance.IndexList.Should().BeEquivalentTo(_expectedIndices);
        }

        [Fact]
        public void IndexList_should_not_preload_compositions()
        {
            _apiClient.DidNotReceiveWithAnyArgs().GetCompositionsBySymbolAsync(default);
        }

        [Fact]
        public async Task IndiceComposition_should_not_be_null_after_calling_the_server()
        {
            var firstIndex = Component.Instance.IndexList.First();
            firstIndex.IndiceCompositions.Should()
                .BeNull("otherwise api is not called");
            
            var expectedCompositions = SetupMockCompositionsResponse();

            await Component.Instance.GetCompositionsForIndex(firstIndex);
            await _apiClient.Received(1).GetCompositionsBySymbolAsync(firstIndex.Symbol);
            firstIndex.IndiceCompositions.Should().BeEquivalentTo(expectedCompositions);
        }

        private List<IndiceCompositionModel> SetupMockCompositionsResponse()
        {
            var indiceComposition = new IndiceCompositionModel(_mockCreator.GetIndiceComposition(1));
            var indiceCompositionList = new List<IndiceCompositionModel> { indiceComposition };
            var newResponse = new Response<List<IndiceCompositionModel>>((int)HttpStatusCode.Created, default, indiceCompositionList);
            _apiClient.GetCompositionsBySymbolAsync(indiceCompositionList.First().Symbol).ReturnsForAnyArgs(newResponse);
            return indiceCompositionList;
        }

        [Fact]
        public async Task IndexList_should_be_null_when_server_call_failed()
        {
            var apiException = new ApiException("wrong route", 404, "page not found", null, null);
            _apiClient.GetAllIndicesAsync().ThrowsForAnyArgs(apiException);

            Component.Instance.IndexList = default;
            Component = RenderComponent<IndiceList>();

            await Component.Instance.GetIndexList();
            Component.Instance.IndexList.Should().BeNull();
        }

        [Fact]
        public async Task IndiceComposition_should_be_null_when_server_call_failed()
        {
            var apiException = new ApiException("wrong route", 404, "page not found", null, null);
            _apiClient.GetCompositionsBySymbolAsync(default).ThrowsForAnyArgs(apiException);

            await Component.Instance.GetCompositionsForIndex(Component.Instance.IndexList.First());
            await _apiClient.ReceivedWithAnyArgs(1).GetCompositionsBySymbolAsync(default);
            Component.Instance.IndexList.First().IndiceCompositions.Should().BeNull();
        }
    }
}
