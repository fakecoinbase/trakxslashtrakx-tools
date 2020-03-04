//using System.Threading.Tasks;
//using FluentAssertions;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Trakx.Data.Common.Ethereum;
//using Trakx.Data.Common.Sources.Coinbase;
//using Trakx.Data.Common.Sources.CoinGecko;
//using Trakx.Data.Common.Sources.Messari.Client;
//using Trakx.Data.Tests.Unit.Models.Index;
//using Xunit;
//using Xunit.Abstractions;

//namespace Trakx.Data.Tests.Tools
//{
//    public class CompositionCreationTool : IClassFixture<DbContextFixture>
//    {
//        private readonly DbContextFixture _fixture;
//        private readonly ITestOutputHelper _output;
//        private readonly ServiceProvider _serviceProvider;

//        public CompositionCreationTool(DbContextFixture fixture, ITestOutputHelper output)
//        {
//            _output = output;
//            _fixture = fixture;
//            var serviceCollection = new ServiceCollection();

//            serviceCollection.AddMessariClient();
//            serviceCollection.AddCoinbaseClient();
//            serviceCollection.AddCoinGeckoClient();
//            serviceCollection.AddMemoryCache();
//            serviceCollection.AddEthereumInteraction(
//                AddYourSecretsHere.InfuraApiKey, AddYourSecretsHere.EthereumWalletSecret, "mainnet");

//            _serviceProvider = serviceCollection.BuildServiceProvider();
//            var conf = _serviceProvider.GetService<IConfiguration>();
//        }

//        [Fact(Skip = "not a test")]
//        public async Task CreateCompositionOnChain()
//        {
//            var compositionCreator = _serviceProvider.GetRequiredService<ICompositionCreator>();

//            var composition = await _fixture.Context
//                .IndexCompositions.Include(c => c.IndexDefinitionDao)
//                .Include(c => c.ComponentQuantityDaos)
//                .ThenInclude(q => q.ComponentDefinitionDao)
//                .FirstAsync();

//            var result = await ((Common.Ethereum.CompositionCreator)compositionCreator).SaveCompositionOnChain(composition);
//            _output.WriteLine(result);

//            result.Should().NotBeNullOrEmpty();
//        }
//    }
//}