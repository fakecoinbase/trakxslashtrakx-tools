using System.Linq;
using FluentAssertions;
using Xunit;

namespace Trakx.Data.Market.Tests.Unit.Models.Index
{
    public class IndexDefinitionTests
    {
        [Fact]
        public void IndexDefinition_of_known_indexes_should_have_initial_valuations_around_0_point_1_Usd()
        {
            var context = new TestIndexRepositoryContext();

            var indexes = context.IndexDefinitions.Select(x => x).ToList();
            
            indexes.Select(i => i.InitialValuation.NetAssetValue).ToList()
                .ForEach(v => v.Should().BeApproximately(0.1m, 0.01m));
        }
    }
}