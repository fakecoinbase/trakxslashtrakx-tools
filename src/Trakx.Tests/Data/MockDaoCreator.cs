using System;
using System.Collections.Generic;
using System.Linq;
using Trakx.Persistence.DAO;
using Xunit.Abstractions;

namespace Trakx.Tests.Data
{
    public class MockDaoCreator : MockCreator
    {
        /// <inheritdoc />
        public MockDaoCreator(ITestOutputHelper output) : base(output) { }
        public IndiceDefinitionDao GetRandomIndiceDefinitionDao(string? indiceSymbol = default, string? name = default)
        {
            indiceSymbol ??= GetRandomIndiceSymbol();
            name ??= "indice name " + GetRandomString(15);
            var description = "description " + GetRandomString(15);
            var indiceDefinition = new IndiceDefinitionDao(indiceSymbol,
                name, description, GetRandomNaturalUnit(), GetRandomAddressEthereum(), GetRandomDateTime());

            return indiceDefinition;
        }

        public IndiceCompositionDao GetRandomCompositionDao(IndiceDefinitionDao? indiceDefinition = default)
        {
            indiceDefinition ??= GetRandomIndiceDefinitionDao();
            return new IndiceCompositionDao(indiceDefinition,
                GetRandomCompositionVersion(),
                GetRandomDateTime(),
                GetRandomAddressEthereum());
        }

        public IndiceValuationDao GetRandomIndiceValuationDao(
            bool isInitialValuation = false, List<ComponentValuationDao>? componentValuations = default)
        {
            componentValuations ??= new List<ComponentValuationDao>();
            if (componentValuations.Count != 0) return new IndiceValuationDao(componentValuations);

            var composition = new IndiceCompositionDao(GetIndiceComposition(3));
            var valuationTimeStamp = isInitialValuation
                ? composition.CreationDate
                : composition.CreationDate.Add(GetRandomTimeSpan());

            componentValuations = composition.ComponentQuantities.Select(q =>
                    new ComponentValuationDao(
                        composition.ComponentQuantityDaos.Single(p => q.ComponentDefinition.Symbol == p.ComponentDefinition.Symbol),
                        valuationTimeStamp, "usdc", GetRandomPrice(),
                        "fakeDataSource"))
                .ToList();

            return new IndiceValuationDao(componentValuations);
        }

        public UserAddressDao GetRandomUserAddressDao(decimal verificationAmount=0)
        {
            
            return new UserAddressDao(GetRandomIndiceSymbol(),GetRandomString(5),GetRandomAddressEthereum(),verificationAmount,DateTime.Now);
        }
    }
}
