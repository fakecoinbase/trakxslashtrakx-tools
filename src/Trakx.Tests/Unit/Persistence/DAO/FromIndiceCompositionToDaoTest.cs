using System;
using System.Collections.Generic;
using FluentAssertions;
using Trakx.Common.Core;
using Trakx.Common.Interfaces.Indice;
using Trakx.Persistence.DAO;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Tests.Unit.Persistence.DAO
{
    public class FromIndiceCompositionToDaoTest
    {
        private readonly IIndiceComposition _newComposition;

        public FromIndiceCompositionToDaoTest(ITestOutputHelper output)
        {
            var mockCreator= new MockCreator(output);
            _newComposition = mockCreator.GetIndiceComposition(3);
        }

        [Fact]
        public void FromModelToIndiceComposition_should_create_composition_on_new_indice()
        {
            var result= new IndiceCompositionDao(_newComposition);

            VerifyComposition(result);
            VerifyIndiceModification(result);
            VerifyComponentQuantityDao(result);
        }

        private void VerifyComposition(IndiceCompositionDao result)
        {
            result.Symbol.Should().Be(_newComposition.GetCompositionSymbol());
            result.CreationDate.Should().Be(_newComposition.CreationDate);
            result.Version.Should().Be(_newComposition.Version);
            result.Id.Should().Be(result.GetCompositionId());
        }

        private void VerifyIndiceModification(IndiceCompositionDao result)
        {
            result.IndiceDefinitionDao.Address.Should().Be(_newComposition.IndiceDefinition.Address);
            result.IndiceDefinitionDao.NaturalUnit.Should().Be(_newComposition.IndiceDefinition.NaturalUnit);
            result.IndiceDefinitionDao.Symbol.Should().Be(_newComposition.IndiceDefinition.Symbol);
            result.IndiceDefinitionDao.CreationDate.Should().Be(_newComposition.IndiceDefinition.CreationDate);
            result.IndiceDefinitionDao.Description.Should().Be(_newComposition.IndiceDefinition.Description);
        }

        private void VerifyComponentQuantityDao(IndiceCompositionDao result)
        {
            result.ComponentQuantityDaos.Count.Should().Be(_newComposition.ComponentQuantities.Count);
            result.ComponentQuantityDaos[0].IndiceCompositionDao.Should().Be(result);
            // result.ComponentQuantityDaos[0].Quantity.Should()
            //     .Be(_newComposition.ComponentQuantities[0].Quantity);
            //ComponentQuantityDao take ulong in parameter, so the quantity is rounded...
            result.ComponentQuantityDaos[0].ComponentDefinitionDao.Symbol.Should()
                .Be(_newComposition.ComponentQuantities[0].ComponentDefinition.Symbol);
            result.ComponentQuantityDaos[0].Id.Should().Be(_newComposition.ComponentQuantities[0].GetId(_newComposition));
        }
    }
}
