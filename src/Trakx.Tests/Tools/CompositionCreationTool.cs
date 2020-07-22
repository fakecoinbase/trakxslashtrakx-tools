using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.Web3;
using Trakx.Common.Ethereum;
using Trakx.Common.Extensions;
using Trakx.Common.Sources.CoinGecko;
using Trakx.Common.Sources.Messari.Client;
using Trakx.Persistence.DAO;
using Trakx.Persistence.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Tests.Tools
{
    [Collection(nameof(SeededDbContextCollection))]
    public class CompositionCreationTool : IClassFixture<SeededDbContextFixture>
    {
        private readonly SeededDbContextFixture _fixture;
        private readonly ITestOutputHelper _output;
        private readonly ServiceProvider _serviceProvider;

        public CompositionCreationTool(SeededDbContextFixture fixture, ITestOutputHelper output)
        {
            _output = output;
            _fixture = fixture;
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddMessariClient();
            serviceCollection.AddCoinGeckoClient();
            serviceCollection.AddMemoryCache();
            serviceCollection.AddEthereumInteraction(
                Secrets.InfuraApiKey, Secrets.EthereumWalletSecret, "mainnet");

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Theory(Skip = "not a test")]
        //[Theory]
        [InlineData("l1cex2005")]

        public async Task CreateCompositionOnChain(string compositionSymbol)
        {
            var compositionCreator = _serviceProvider.GetRequiredService<ICompositionCreator>();

            var composition = await _fixture.Context
                .IndiceCompositions.Include(c => c.IndiceDefinitionDao)
                .Include(c => c.ComponentQuantityDaos)
                .ThenInclude(q => q.ComponentDefinitionDao)
                .FirstAsync(c => c.Symbol == compositionSymbol);

            OutputSetCallArguments(composition);

            var result = await ((CompositionCreator)compositionCreator).SaveCompositionOnChain(composition);
            _output.WriteLine(result);

            result.Should().NotBeNullOrEmpty();
        }

        private void OutputSetCallArguments(IndiceCompositionDao composition)
        {
            var units = composition.ComponentQuantities.Select(q =>
                    new BigInteger(q.Quantity.DescaleComponentQuantity(
                        q.ComponentDefinition.Decimals, composition.IndiceDefinition.NaturalUnit)))
                .Select(b => $"new BigNumber({b})")
                .ToList();

            var addresses = composition.ComponentQuantities.Select(q => $"\"{q.ComponentDefinition.Address}\"").ToList();

            _output.WriteLine($"[{string.Join(", " + Environment.NewLine, addresses)}],");
            _output.WriteLine($"[{string.Join(", " + Environment.NewLine, units)}],");
            _output.WriteLine($"new BigNumber({composition.IndiceDefinition.NaturalUnit.AsAPowerOf10()}),");
            _output.WriteLine($"\"{composition.IndiceDefinitionDao.Name}\",");
            _output.WriteLine($"\"{composition.Symbol}\",");
        }

        [Theory(Skip = "not a test")]
        //[Theory]
        [InlineData("l1mc10erc2004")]
        public async Task CreateIndiceFromCompositionOnChain(string compositionSymbol)
        {
            var indiceCreator = _serviceProvider.GetRequiredService<IIndiceCreator>();

            var composition = await _fixture.Context
                .IndiceCompositions.Include(c => c.IndiceDefinitionDao)
                .FirstAsync(c => c.Symbol == compositionSymbol);

            var result = await indiceCreator.SaveIndiceOnChain(composition);
            _output.WriteLine(result);

            result.Should().NotBeNullOrEmpty();
        }

        [Theory(Skip = "not a test")]
        //[Theory]
        [InlineData("l1sca2003")]
        public async Task RebalanceIndiceOnChain(string newCompositionSymbol)
        {
            var composition = await _fixture.Context
                .IndiceCompositions.Include(c => c.IndiceDefinitionDao)
                .FirstAsync(c => c.Symbol == newCompositionSymbol);

            var web3 = _serviceProvider.GetRequiredService<IWeb3>();
            var logger = _output.ToLogger<IndiceRebalancer>();

            var rebalancer = new IndiceRebalancer(composition.IndiceDefinition, web3, logger);

            var rebalancingProposal = await rebalancer.ProposeRebalancing(composition).ConfigureAwait(false);
            _output.WriteLine(rebalancingProposal);

            var rebalancingStart = await rebalancer.StartRebalancing().ConfigureAwait(false);
            _output.WriteLine(rebalancingStart);

            var rebalancingSettle = await rebalancer.SettleRebalancing().ConfigureAwait(false);
            _output.WriteLine(rebalancingSettle);
        }
    }
}