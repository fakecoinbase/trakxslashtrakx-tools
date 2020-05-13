using System;
using System.Collections.Generic;
using FluentAssertions;
using Trakx.Common.Extensions;
using Trakx.IndiceManager.Server.Models;
using Trakx.Persistence.DAO;
using Xunit;

namespace Trakx.IndiceManager.Server.Tests.Unit.Model
{
    public class IndiceCompositionModelTest
    {
        private readonly IndiceCompositionModel _newComposition;
        private readonly IndiceDetailModel _newIndice;

        public IndiceCompositionModelTest()
        {
            _newIndice = new IndiceDetailModel
            {
                Address = "0xdc6e10fbacf109efb74e0864cdce4876c7e729bf",
                CreationDate = new DateTime(2018, 02, 10),
                Description = "OK all is good",
                Name = "TrakxTest",
                NaturalUnit = 10,
                Symbol = "TRX"
            };
            _newComposition = new IndiceCompositionModel
            {
                CreationDate = new DateTime(2006, 10, 20),
                IndiceDetail = _newIndice,
                Version = 2,
                Components = new List<ComponentDetailModel> { new ComponentDetailModel { Quantity = 3.0m, Symbol = "btc", Address = "0xab6e10fbacf109e45646664cdce4876c7e729bf", Name = "bitcoin", CoinGeckoId = "bitcoin" } }
            };
        }

        [Fact]
        public void FromModelToIndiceComposition_should_create_composition_on_new_indice()
        {
            var result = _newComposition.FromModelToIndiceComposition(null) as IndiceCompositionDao;

            VerifyComposition(result);
            VerifyIndiceModification(result);
            VerifyComponentQuantityDao(result,_newIndice.NaturalUnit);
        }

        [Fact]
        public void FromModelToIndiceComposition_should_create_composition_on_saved_indice_and_modify_indice()
        {
            var savedIndice = new IndiceDefinitionDao(null, null, null, 2, null, null);
            var result = _newComposition.FromModelToIndiceComposition(savedIndice) as IndiceCompositionDao;

            VerifyComposition(result);
            VerifyIndiceModification(result);
            VerifyComponentQuantityDao(result, _newIndice.NaturalUnit);
        }

        
        [Fact]
        public void FromModelToIndiceComposition_should_create_composition_on_published_indice_and_dont_modify_indice()
        {
            var publishedIndice = new IndiceDefinitionDao("TRX", null, null, 2, "published address", null);
            var result = _newComposition.FromModelToIndiceComposition(publishedIndice) as IndiceCompositionDao;

            VerifyComposition(result);
            VerifyComponentQuantityDao(result, publishedIndice.NaturalUnit);

            result.IndiceDefinitionDao.Address.Should().Be(publishedIndice.Address);
            result.IndiceDefinitionDao.NaturalUnit.Should().Be(publishedIndice.NaturalUnit);
            result.IndiceDefinitionDao.Symbol.Should().Be(publishedIndice.Symbol);
            result.IndiceDefinitionDao.CreationDate.Should().Be(publishedIndice.CreationDate);
            result.IndiceDefinitionDao.Description.Should().Be(publishedIndice.Description);
        }


        private void VerifyComposition(IndiceCompositionDao result)
        {
            result.Symbol.Should().Be($"{_newIndice.Symbol}{_newComposition.CreationDate:yyMM}");
            result.CreationDate.Should().Be(_newComposition.CreationDate);
            result.Version.Should().Be(_newComposition.Version);
            result.Id.Should().Be($"{_newIndice.Symbol}|{_newComposition.Version}");
        }

        private void VerifyIndiceModification(IndiceCompositionDao result)
        {
            result.IndiceDefinitionDao.Address.Should().Be(_newIndice.Address);
            result.IndiceDefinitionDao.NaturalUnit.Should().Be(_newIndice.NaturalUnit);
            result.IndiceDefinitionDao.Symbol.Should().Be(_newIndice.Symbol);
            result.IndiceDefinitionDao.CreationDate.Should().Be(_newIndice.CreationDate);
            result.IndiceDefinitionDao.Description.Should().Be(_newIndice.Description);
        }

        private void VerifyComponentQuantityDao(IndiceCompositionDao result,ushort indiceNaturalUnit)
        {
            result.ComponentQuantityDaos.Count.Should().Be(1);
            result.ComponentQuantityDaos[0].IndiceCompositionDao.Should().Be(result);
            result.ComponentQuantityDaos[0].Quantity
                .DescaleComponentQuantity(_newComposition.Components[0].Decimals, indiceNaturalUnit).Should()
                .Be(_newComposition.Components[0].Quantity);
            result.ComponentQuantityDaos[0].ComponentDefinitionDao.Symbol.Should()
                .Be(_newComposition.Components[0].Symbol);
            result.ComponentQuantityDaos[0].Id.Should().Be($"{result.Id}|{_newComposition.Components[0].Symbol}");
        }
    }
}
