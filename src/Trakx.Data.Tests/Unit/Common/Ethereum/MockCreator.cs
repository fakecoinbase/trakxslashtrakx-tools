using System.Collections.Generic;
using NSubstitute;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Tests.Unit.Common.Ethereum
{
    public class MockCreator
    {
        public IIndexComposition GetIndexComposition()
        {

            var indexDefinition = GetIndexDefinition();

            var componentQuantities = new List<IComponentQuantity>
            {
                GetComponentQuantity("0xabcdefg", 1.7m, 10), 
                GetComponentQuantity("0x123456", 200.3m, 16),
            };

            var indexComposition = Substitute.For<IIndexComposition>();
            indexComposition.ComponentQuantities.Returns(componentQuantities);
            indexComposition.IndexDefinition.Returns(indexDefinition);
            indexComposition.Address.Returns("0x7210cc724480c85b893a9febbecc24a8dc4ff1de");
            indexComposition.Symbol.Returns("idx2001");
            return indexComposition;
        }

        public IIndexDefinition GetIndexDefinition()
        {
            var indexDefinition = Substitute.For<IIndexDefinition>();
            indexDefinition.NaturalUnit.Returns((ushort) 10);
            indexDefinition.Symbol.Returns("idx");
            return indexDefinition;
        }

        public IComponentQuantity GetComponentQuantity(string address, decimal quantity, ushort decimals = 18)
        {
            var componentQuantity = Substitute.For<IComponentQuantity>();
            componentQuantity.Quantity.Returns(quantity);
            componentQuantity.ComponentDefinition.Address.Returns(address);
            componentQuantity.ComponentDefinition.Decimals.Returns(decimals);
            return componentQuantity;
        }
    }
}