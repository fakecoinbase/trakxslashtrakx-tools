using System.Linq;
using Trakx.Common.Core;
using Xunit;

namespace Trakx.MarketData.Server.Tests.Unit.Model
{
    public class IndicePricedModelTests
    {
        public IndicePricedModelTests()
        {
            var componentDefinitions = Enumerable.Range(0, 4)
                .Select(i => new ComponentDefinition($"0xaddress{i}", $"name{i}", $"symbol{i}", $"gecko{i}", (ushort)i))
                .ToList();
            var indiceDefintion = new IndiceDefinition("idx", "indice", "test indice", 
                10, "0xidx", default);
        }

        [Fact(Skip = "Not implemented yet.")]
        public void IndiceSymbol_Should_Be_Uppercased()
        {
            //this is apparently needed by the baseapp components
        }
    }
}
