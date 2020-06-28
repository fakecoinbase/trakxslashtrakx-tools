using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Trakx.Common.Composition;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Tests.Unit.Common.Composition
{
    public sealed class WeightCalculatorTests
    {
        private readonly WeightCalculator _weightCalculator;
        private readonly MockCreator _mockCreator;
        private readonly Dictionary<string, decimal> _componentDictionary;
        private readonly Dictionary<string, decimal> _equalWeightComponentDictionary;

        public WeightCalculatorTests(ITestOutputHelper output)
        {
            _mockCreator = new MockCreator(output);
            _weightCalculator = new WeightCalculator();
            _componentDictionary = new Dictionary<string, decimal>

            {
                {"bat",0.3m},{"wbtc",0.2m},{"quash",0.5m}
            };

            var equalWeight = (decimal)1 / 3;
            _equalWeightComponentDictionary = new Dictionary<string, decimal>

            {
                {"bat",equalWeight},{"wbtc",equalWeight},{"quash",equalWeight}
            };
        }

        [Fact]
        public void CalculateUsdcValuesFromNavAndWeights_should_compute_the_correct_usdc_values()
        {
            var nav = _mockCreator.GetRandomPrice();

            var expectedDictionary = _componentDictionary
                .ToDictionary(o => o.Key, o => o.Value * nav);
            
            var returnedDictionary =_weightCalculator.CalculateUsdcValuesFromNavAndWeights(_componentDictionary, nav);

            expectedDictionary.Should().BeEquivalentTo(returnedDictionary);
        }

        [Fact]
        public void CalculateWeightsFromUsdcValues_should_calculate_correct_weight_for_unprecise_weight()
        {
            _equalWeightComponentDictionary.Sum(o => o.Value).Should().NotBe(1m);
            var returnedDictionary = _weightCalculator.CalculateWeightsFromUsdcValues(_equalWeightComponentDictionary);
            returnedDictionary.Sum(o => o.Value).Should().Be(1m);
        }

        [Fact]
        public void CalculateWeightsFromUsdcValues_should_not_change_value_for_precise_weight()
        {
            var returnedDictionary = _weightCalculator.CalculateWeightsFromUsdcValues(_componentDictionary);
            _componentDictionary.Should().BeEquivalentTo(returnedDictionary);
        }

        [Fact]
        public void DistributeWeights_should_always_return_a_sum_total_of_values_equal_to_one()
        {
            var componentSymbols = new List<string> { "wbtc", "bat", "quash" };
            var returnedDictionary = _weightCalculator.DistributeWeights(componentSymbols);

            returnedDictionary.Sum(o => o.Value).Should().Be(1m);
        }


        [Fact]
        public void DistributeWeights_should_round_the_first_value_with_default_precision_if_not_suggested()
        {
            var componentSymbols = new List<string> {"wbtc", "bat", "quash"};
            var returnedDictionary = _weightCalculator.DistributeWeights(componentSymbols);
            
            returnedDictionary.First().Value.Should().Be(0.333334m);
        }

        [Fact]
        public void DistributeWeights_should_round_the_first_value_according_to_the_precision()
        {
            var componentSymbols = new List<string> { "wbtc", "bat", "quash" };
            var returnedDictionary = _weightCalculator.DistributeWeights(componentSymbols,1);

            returnedDictionary.First().Value.Should().Be(0.4m);
        }
    }
}
