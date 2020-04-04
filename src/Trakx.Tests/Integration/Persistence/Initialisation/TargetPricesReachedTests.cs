using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;
using Trakx.Data.Common.Interfaces.Pricing;
using Trakx.Data.Common.Pricing;
using Trakx.Data.Common.Sources.Coinbase;
using Trakx.Data.Common.Sources.CoinGecko;
using Trakx.Data.Common.Sources.Messari.Client;
using Trakx.Data.Persistence.Initialisation;
using Trakx.Data.Tests.Unit.Models;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Data.Tests.Integration.Persistence.Initialisation
{
    [Collection(nameof(DbContextCollection))]
    public class TargetPricesReachedTests
    {
        private readonly InMemoryIndexRepositoryContext _dbContext;
        private readonly ITestOutputHelper _output;
        private readonly INavCalculator _navCalculator;
        private readonly AsyncRetryPolicy _retryPolicy;

        public TargetPricesReachedTests(DbContextFixture fixture, ITestOutputHelper output)
        {
            _dbContext = fixture.Context;
            _output = output;
            
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddMessariClient();
            serviceCollection.AddCoinbaseClient();
            serviceCollection.AddCoinGeckoClient();
            serviceCollection.AddMemoryCache();
            serviceCollection.AddPricing();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            _navCalculator = serviceProvider.GetRequiredService<INavCalculator>();

            _retryPolicy = Policy.Handle<NavCalculator.FailedToRetrievePriceException>()
                .WaitAndRetryAsync(10, _ => TimeSpan.FromMilliseconds(500));

        }

        [Fact]
        public async Task DatabaseInitialiser_should_issue_compositions_at_target_navs()
        {
            var compositions = _dbContext.IndexCompositions.AsAsyncEnumerable();
            await foreach (var composition in compositions)
            {
                var expectedNav = DatabaseInitialiser.GetTargetIssuePrice(composition.Symbol);
                var nav = await _retryPolicy.ExecuteAsync(async () => await _navCalculator.CalculateNav(composition, composition.CreationDate));

                _output.WriteLine($"{composition.Symbol}:{Environment.NewLine}" +
                                  $"target nav {expectedNav}{Environment.NewLine}" +
                                  $"actual nav {nav}{Environment.NewLine}");

                ((double)nav).Should().BeApproximately((double)expectedNav, 1e-5);
            }
        }

        [Fact]
        public async Task DatabaseInitialiser_should_issue_rebalancings_at_previous_composition_actual_nav()
        {
            var compositionPairs = (await _dbContext.IndexCompositions.ToListAsync()).GroupBy(c => c.IndexDefinition.Symbol);
            foreach (var compositionPair in compositionPairs)
            {
                var orderedCompositions = compositionPair.OrderBy(c => c.CreationDate).ToList();

                var initial = orderedCompositions[0];
                var rebalanced = orderedCompositions[1];

                var expectedIssueNav = await _retryPolicy.ExecuteAsync(async () => await _navCalculator.CalculateNav(initial, rebalanced.CreationDate));
                var rebalanceIssueNav = await _retryPolicy.ExecuteAsync(async () => await _navCalculator.CalculateNav(rebalanced, rebalanced.CreationDate));

                _output.WriteLine($"{initial.IndexDefinition.Symbol}:{Environment.NewLine}" +
                                  $"expected issue nav {expectedIssueNav}{Environment.NewLine}" +
                                  $"rebalance issued nav {rebalanceIssueNav}{Environment.NewLine}");

                ((double)rebalanceIssueNav).Should().BeApproximately((double)expectedIssueNav, 1e-5,
                    "otherwise holders will have a pnl jump.");
            }
        }
    }
}