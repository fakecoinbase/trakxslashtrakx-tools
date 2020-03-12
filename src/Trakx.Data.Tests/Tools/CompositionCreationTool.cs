﻿//using System;
//using System.Linq;
//using System.Numerics;
//using System.Threading.Tasks;
//using FluentAssertions;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Nethereum.Web3;
//using Trakx.Data.Common.Ethereum;
//using Trakx.Data.Common.Extensions;
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

//        [Fact()]
//        public async Task CreateCompositionOnChain()
//        {
//            var compositionCreator = _serviceProvider.GetRequiredService<ICompositionCreator>();
//            var web3 = _serviceProvider.GetRequiredService<IWeb3>();

//            ///web3.TransactionManager.DefaultGas.
//            var symbol = "l1amg2001";

//            var composition = await _fixture.Context
//                .IndexCompositions.Include(c => c.IndexDefinitionDao)
//                .Include(c => c.ComponentQuantityDaos)
//                .ThenInclude(q => q.ComponentDefinitionDao)
//                .FirstAsync(c => c.Symbol == symbol);

//            var units = composition.ComponentQuantities.Select(q =>
//                    new BigInteger(q.Quantity.DescaleComponentQuantity(
//                        q.ComponentDefinition.Decimals, composition.IndexDefinition.NaturalUnit)))
//                .Select(b => $"new BigNumber({b})")
//                .ToList();

//            var addresses = composition.ComponentQuantities.Select(q => $"\"{q.ComponentDefinition.Address}\"").ToList();
            
//            _output.WriteLine($"[{string.Join(", " + Environment.NewLine, addresses)}],");
//            _output.WriteLine($"[{string.Join(", " + Environment.NewLine, units)}],");
//            _output.WriteLine($"new BigNumber({composition.IndexDefinition.NaturalUnit.AsAPowerOf10()})");
//            _output.WriteLine($"\"{composition.IndexDefinitionDao.Name}\",");
//            _output.WriteLine($"\"{composition.Symbol}\",");

//            var result = await ((CompositionCreator)compositionCreator).SaveCompositionOnChain(composition);
//            _output.WriteLine(result);

//            result.Should().NotBeNullOrEmpty();
//        }
//    }
//}