using System.Linq;
using Trakx.Data.Common.Core;
using Trakx.Data.Common.Interfaces.Index;
using Xunit;

namespace Trakx.Data.Tests.Unit.Server.Model
{
    public class IndexPricedMoldelTests
    {
        public IndexPricedMoldelTests()
        {
            var componentDefinitions = Enumerable.Range(0, 4)
                .Select(i => new ComponentDefinition($"0xaddress{i}", $"name{i}", $"symbol{i}", $"gecko{i}", i))
                .ToList();
            var indexDefintion = new IndexDefinition("idx", "index", "test index", 
                componentDefinitions.Select(c => (IComponentWeight)new ComponentWeight(c, 0.25m)).ToList(),
                10, "0xidx", default);
        }

        [Fact(Skip = "Not implemented yet.")]
        public void IndexSymbol_Should_Be_Uppercased()
        {
            //this is apparently needed by the baseapp components

        }
    }
}
