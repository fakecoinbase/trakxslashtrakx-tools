using System;
using System.Collections.Generic;
using FluentAssertions;
using Trakx.Common.Extensions;
using Trakx.Common.Interfaces.Indice;
using Trakx.IndiceManager.Server.Models;
using Trakx.Persistence.DAO;
using Xunit;

namespace Trakx.IndiceManager.Server.Tests.Unit.Models
{
    public class IndiceCompositionModelTests
    {
        private readonly IndiceCompositionModel _newComposition;
        private readonly IndiceDetailModel _newIndice;

        public IndiceCompositionModelTests()
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
            var result = _newComposition.ConvertToIIndiceComposition(null);

            VerifyComposition(result);
            VerifyIndiceModification(result);
            VerifyComponentQuantityDao(result);
        }

        [Fact]
        public void FromModelToIndiceComposition_should_create_composition_on_saved_indice_and_modify_indice()
        {
            var savedIndice = new IndiceDefinitionDao(null, null, null, 2, null, null);
            var result = _newComposition.ConvertToIIndiceComposition(savedIndice);

            VerifyComposition(result);
            VerifyIndiceModification(result);
            VerifyComponentQuantityDao(result);
        }


        [Fact]
        public void FromModelToIndiceComposition_should_create_composition_on_published_indice_and_dont_modify_indice()
        {
            var publishedIndice = new IndiceDefinitionDao("TRX", null, null, 2, "published address", null);
            var result = _newComposition.ConvertToIIndiceComposition(publishedIndice);

            VerifyComposition(result);
            VerifyComponentQuantityDao(result);

            result.IndiceDefinition.Address.Should().Be(publishedIndice.Address);
            result.IndiceDefinition.NaturalUnit.Should().Be(publishedIndice.NaturalUnit);
            result.IndiceDefinition.Symbol.Should().Be(publishedIndice.Symbol);
            result.IndiceDefinition.CreationDate.Should().Be(publishedIndice.CreationDate);
            result.IndiceDefinition.Description.Should().Be(publishedIndice.Description);
        }


        private void VerifyComposition(IIndiceComposition result)
        {
            result.Symbol.Should().Be($"{_newIndice.Symbol}{_newComposition.CreationDate:yyMM}");
            result.CreationDate.Should().Be(_newComposition.CreationDate);
            result.Version.Should().Be(_newComposition.Version);
        }

        private void VerifyIndiceModification(IIndiceComposition result)
        {
            result.IndiceDefinition.Address.Should().Be(_newIndice.Address);
            result.IndiceDefinition.NaturalUnit.Should().Be(_newIndice.NaturalUnit);
            result.IndiceDefinition.Symbol.Should().Be(_newIndice.Symbol);
            result.IndiceDefinition.CreationDate.Should().Be(_newIndice.CreationDate);
            result.IndiceDefinition.Description.Should().Be(_newIndice.Description);
            result.IndiceDefinition.Name.Should().Be(_newIndice.Name);
        }

        private void VerifyComponentQuantityDao(IIndiceComposition result)
        {
            result.ComponentQuantities.Count.Should().Be(1);
            result.ComponentQuantities[0].Quantity.DescaleComponentQuantity(result.ComponentQuantities[0].ComponentDefinition.Decimals,result.IndiceDefinition.NaturalUnit).Should()
                .Be(_newComposition.Components[0].Quantity);
            result.ComponentQuantities[0].ComponentDefinition.Symbol.Should()
                .Be(_newComposition.Components[0].Symbol);
        }
    }
}
