using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Trakx.Common.Composition;
using Trakx.Common.Core;
using Trakx.Common.Interfaces.Index;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Tests.Unit.Common.Composition
{
    public class IndexCompositionCalculatorTests
    {
        private const decimal TargetIndexPrice1 = 100m;
        private const decimal TargetIndexPrice2 = 321m;
        private readonly ITestOutputHelper _output;
        private IndexDefinition _indexDefinition1;
        private IndexDefinition _indexDefinition2;
        private IComponentDefinition[] _definitions;
        private Dictionary<IComponentDefinition, PriceAndTargetWeight> _prices;

        public IndexCompositionCalculatorTests(ITestOutputHelper output)
        {
            _output = output;

            _definitions = new IComponentDefinition[]
            {
                new ComponentDefinition("0xa10", "comp10", "c10", "gecko10", 10),
                new ComponentDefinition("0xa18", "comp18", "c18", "gecko18", 18),
                new ComponentDefinition("0xa2", "comp2", "c2", "g2", 2),
                new ComponentDefinition("0xa15", "comp15", "c15", "g15", 15),
            };

            _prices = new[] {
                    new PriceAndTargetWeight(.001m, 0.3m),
                    new PriceAndTargetWeight(50m, 0.1m),
                    new PriceAndTargetWeight(3m, 0.2m),
                    new PriceAndTargetWeight(0.02m, 0.4m)
                }.Zip(_definitions)
                .ToDictionary(x => x.Second, x => x.First);

            _indexDefinition1 = new IndexDefinition("idx1", "dummy1",
                "blablabla", 10, "0xidx",
                DateTime.UtcNow);

            _indexDefinition2 = new IndexDefinition("idx2", "dummy2",
                "blablabla", 15, "0xidx",
                DateTime.UtcNow);

        }

        [Fact]
        public void CalculateQuantity_should_use_decimals_and_index_natural_unit_to_scale_quantity()
        {
            var quantities = _prices.Select((p, i) => IndexCompositionCalculator
                .CalculateQuantity(_indexDefinition1, p.Key, p.Value, TargetIndexPrice1))
                .ToList();

            foreach (var componentQuantity in quantities)
            {
                _output.WriteLine($"price {_prices[componentQuantity.ComponentDefinition]} => "
                                  + System.Text.Json.JsonSerializer.Serialize(componentQuantity));

                var valuation = new ComponentValuation(componentQuantity, "_",
                    _prices[componentQuantity.ComponentDefinition].Price, "_", DateTime.UtcNow);

                valuation.Value.Should().BeApproximately(
                    TargetIndexPrice1 * _prices[componentQuantity.ComponentDefinition].TargetWeight, 1e-2m);
            }
        }

        [Fact]
        public void CalculateIndexComposition_should_produce_composition_which_prices_close_to_target_nav()
        {
            var composition1 = IndexCompositionCalculator.CalculateIndexComposition(_indexDefinition1, _prices, TargetIndexPrice1, 0);
            ValidateNav(composition1, TargetIndexPrice1);

            var composition2 = IndexCompositionCalculator.CalculateIndexComposition(_indexDefinition2, _prices, TargetIndexPrice2, 0);
            ValidateNav(composition2, TargetIndexPrice2);
        }

        private void ValidateNav(IIndexComposition composition, decimal targetIndexPrice)
        {
            var componentValuations = composition.ComponentQuantities.Select(c =>
                (IComponentValuation)new ComponentValuation(c, "_", _prices[c.ComponentDefinition].Price, "_", DateTime.UtcNow));

            var indexValuation = new IndexValuation(composition, componentValuations.ToList(), DateTime.UtcNow);

            _output.WriteLine("Nav = {0}", System.Text.Json.JsonSerializer.Serialize(indexValuation));

            indexValuation.NetAssetValue.Should().BeApproximately(targetIndexPrice, 1e-2m);
            _output.WriteLine("Nav = {0}", indexValuation.NetAssetValue);
        }
    }
}