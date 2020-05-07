using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using Trakx.Common.Extensions;
using Trakx.Common.Interfaces;
using Trakx.IndiceManager.Server.Managers;
using Trakx.IndiceManager.Server.Models;
using Trakx.Persistence;
using Trakx.Persistence.DAO;
using Trakx.Tests.Unit.Models;
using Xunit;

namespace Trakx.IndiceManager.Server.Tests.Unit.Managers
{
    public class IndiceInformationRetrieverTest
    {
        private readonly IIndiceDataProvider _dataProvider;
        private readonly IIndiceInformationRetriever _indiceInformationRetriever;

        public IndiceInformationRetrieverTest()
        {
            _dataProvider = Substitute.For<IIndiceDataProvider>();
            _indiceInformationRetriever=new IndiceInformationRetriever(_dataProvider);
        }

        [Fact]
        public async Task GetAllIndicesFromDatabase_should_call_dataProvider_getAllIndices()
        {
            await _indiceInformationRetriever.GetAllIndicesFromDatabase();
            await _dataProvider.Received(1).GetAllIndices();
        }

        [Fact]
        public async Task GetAllCompositionForIndiceFromDatabase_should_call_dataProvider_getAllCompositionForIndices()
        {
            await _indiceInformationRetriever.GetAllCompositionForIndiceFromDatabase("symbol");
            await _dataProvider.Received(1).GetAllCompositionForIndice("symbol");
        }

        [Fact]
        public async Task SearchIndiceByAddress_should_call_dataProvider_TryToGetIndiceByAddress()
        {
            await _indiceInformationRetriever.SearchIndiceByAddress("address");
            await _dataProvider.Received(1).TryToGetIndiceByAddress("address");
        }

        [Fact]
        public async Task SearchCompositionByAddress_should_call_dataProvider_TryToGetCompositionByAddress()
        {
            await _indiceInformationRetriever.SearchCompositionByAddress("address");
            await _dataProvider.Received(1).TryToGetCompositionByAddress("address");
        }
    }
}
