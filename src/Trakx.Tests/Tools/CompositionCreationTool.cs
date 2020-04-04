using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Common.Ethereum;
using Trakx.Common.Extensions;
using Trakx.Common.Sources.Coinbase;
using Trakx.Common.Sources.CoinGecko;
using Trakx.Common.Sources.Messari.Client;
using Trakx.Persistence.DAO;
using Trakx.Tests.Unit.Models;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Tests.Tools
{
    [Collection(nameof(DbContextCollection))]
    public class CompositionCreationTool : IClassFixture<DbContextFixture>
    {
        private readonly DbContextFixture _fixture;
        private readonly ITestOutputHelper _output;
        private readonly ServiceProvider _serviceProvider;

        public CompositionCreationTool(DbContextFixture fixture, ITestOutputHelper output)
        {
            _output = output;
            _fixture = fixture;
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddMessariClient();
            serviceCollection.AddCoinbaseClient();
            serviceCollection.AddCoinGeckoClient();
            serviceCollection.AddMemoryCache();
            serviceCollection.AddEthereumInteraction(
                Secrets.InfuraApiKey, Secrets.EthereumWalletSecret, "mainnet");

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Theory(Skip = "not a test")]
        //[Theory]
        //[InlineData("l1amg2001")]
        //[InlineData("l1cex2001")]
        //[InlineData("l1dex2001")]
        //[InlineData("l1len2001")]
        //[InlineData("l1sca2001")]
        //[InlineData("l1amg2003")]
        //[InlineData("l1cex2003")]
        //[InlineData("l1dex2003")]
        //[InlineData("l1len2003")]
        [InlineData("l1sca2003")]
        public async Task CreateCompositionOnChain(string compositionSymbol)
        {
            var compositionCreator = _serviceProvider.GetRequiredService<ICompositionCreator>();

            var composition = await _fixture.Context
                .IndexCompositions.Include(c => c.IndexDefinitionDao)
                .Include(c => c.ComponentQuantityDaos)
                .ThenInclude(q => q.ComponentDefinitionDao)
                .FirstAsync(c => c.Symbol == compositionSymbol);

            OutputSetCallArguments(composition);

            var result = await ((CompositionCreator)compositionCreator).SaveCompositionOnChain(composition);
            _output.WriteLine(result);

            result.Should().NotBeNullOrEmpty();
        }

        private void OutputSetCallArguments(IndexCompositionDao composition)
        {
            var units = composition.ComponentQuantities.Select(q =>
                    new BigInteger(q.Quantity.DescaleComponentQuantity(
                        q.ComponentDefinition.Decimals, composition.IndexDefinition.NaturalUnit)))
                .Select(b => $"new BigNumber({b})")
                .ToList();

            var addresses = composition.ComponentQuantities.Select(q => $"\"{q.ComponentDefinition.Address}\"").ToList();

            _output.WriteLine($"[{string.Join(", " + Environment.NewLine, addresses)}],");
            _output.WriteLine($"[{string.Join(", " + Environment.NewLine, units)}],");
            _output.WriteLine($"new BigNumber({composition.IndexDefinition.NaturalUnit.AsAPowerOf10()}),");
            _output.WriteLine($"\"{composition.IndexDefinitionDao.Name}\",");
            _output.WriteLine($"\"{composition.Symbol}\",");
        }

        [Theory(Skip = "not a test")]
        //[Theory]
        //[InlineData("l1amg2001")]
        //[InlineData("l1cex2001")]
        //[InlineData("l1dex2001")]
        //[InlineData("l1len2001")]
        [InlineData("l1sca2001")]
        public async Task CreateIndexFromCompositionOnChain(string compositionSymbol)
        {
            var indexCreator = _serviceProvider.GetRequiredService<IIndexCreator>();

            var composition = await _fixture.Context
                .IndexCompositions.Include(c => c.IndexDefinitionDao)
                .FirstAsync(c => c.Symbol == compositionSymbol);

            var result = await indexCreator.SaveIndexOnChain(composition);
            _output.WriteLine(result);

            result.Should().NotBeNullOrEmpty();
        }
    }
}