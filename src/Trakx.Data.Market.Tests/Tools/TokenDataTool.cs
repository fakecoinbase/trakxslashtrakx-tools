using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Data.Market.Common.Indexes;
using Trakx.Data.Market.Common.Sources.BitGo;
using Trakx.Data.Market.Common.Sources.Kaiko.Client;
using Trakx.Data.Market.Common.Sources.Messari.Client;
using Trakx.Data.Market.Tests.Integration;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Data.Market.Tests.Tools
{
    public class TokenDataTool : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly IKaikoClient _kaikoClient;

        public TokenDataTool(ITestOutputHelper output)
        {
            _output = output;
            //_logger = output.BuildLoggerFor<KaikoApiClient>();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddKaikoClient();
            serviceCollection.AddMessariClient();
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _kaikoClient = _serviceProvider.GetRequiredService<IKaikoClient>();
            _messariClient = _serviceProvider.GetRequiredService<Common.Sources.Messari.Client.RequestHelper>();
        }
        
        [Fact(Skip = "just to be used on DataDumps, feel free to remove the 'Take(10)'")]
        public async Task GetAggregatedPrice_should_return_aggregated_prices()
        {
            var quotationSymbols = new[]
                {"usd", "jpy", "eur", "gbp", "krw", "btc", "eth", "bnb", "usdt", "usdc", "dai", "pax"};

            var exchangeRates = quotationSymbols.ToDictionary(s => s, s => KaikoApiClientTests.TryFindUsdExchangeRate(_kaikoClient, s));


            var bitGoTokens = new SupportedTokenProvider().SupportedTokensBySymbol;
            var indexTokens = new IndexDetailsProvider().IndexDetails
                .SelectMany(i => i.Value.Components.Select(c => c.Symbol.ToLower())).Distinct().ToList();

            var kaikoInstruments = await _kaikoClient.GetInstruments().ConfigureAwait(false);
            var symbols = kaikoInstruments.Instruments.Select(i => i.Code.ToLower().Split("-"))
                .Where(s => s.Length > 1 && quotationSymbols.Contains(s[1]))
                .Take(10) 
                .OrderBy(s => s[0]);

            var queries = symbols.Select(e => _kaikoClient.CreateSpotExchangeRateRequest(e[0], e[1], true)).ToList();

            var tempPath = "kaikoData." + DateTime.Now.ToString("yyyyMMdd.hhmmss");
            Directory.CreateDirectory(tempPath);
            var priceTasks = queries.AsParallel().WithDegreeOfParallelism(200).Select(async q =>
            {
                var aggregatedPrice = await _kaikoClient.GetSpotExchangeRate(q).ConfigureAwait(false);
                var profile = await _messariClient.GetProfileForSymbol(q.BaseAsset).ConfigureAwait(false);
                if (aggregatedPrice?.Result == "success" && aggregatedPrice.Data.Any())
                {
                    return new
                    {
                        Query = q,
                        Prices = aggregatedPrice,
                        Sector = profile?.Data?.Sector ?? "",
                        FoundOnMessari = profile?.Data != null,
                        BitGoCustody = bitGoTokens.ContainsKey(q.BaseAsset.ToLower()),
                        UsedByTrakx = indexTokens.Contains(q.BaseAsset.ToLower()),
                        AssetName = profile?.Data?.Name ?? "",
                    };
                }
                return null;
            })
                .Where(r => r?.Result != null)
                .ToArray();

            var results = priceTasks.Select(p => p.Result).Where(r => r != null).OrderBy(r => r.Query.BaseAsset).ToList();

            await using var csvOutput = File.Create(Path.Combine(tempPath, "dump.csv"));
            await using var csvWriter = new StreamWriter(csvOutput);

            var header = $"Symbol, QuoteSymbol, Name, Sector, Volume (usd), price (usd), FoundOnMessari, BitGoCustody, UsedByTrakx";
            _output.WriteLine(header);
            await csvWriter.WriteLineAsync(header);

            foreach (var result in results)
            {
                var quoteSymbol = result.Query.QuoteAsset;
                var exchangeRate = exchangeRates[quoteSymbol];
                var summedVolume = result.Prices.Data.Sum(a => decimal.Parse(a.Volume)) * exchangeRate;
                var averagePrice = result.Prices.Data.Average(a => decimal.Parse(a.Price)) * exchangeRate;
                var symbol = result.Query.BaseAsset;

                var newLine = $"{symbol}, {quoteSymbol}, {result.AssetName}, {result.Sector}, {summedVolume}, {averagePrice}, {result.FoundOnMessari}, {result.BitGoCustody}, {result.UsedByTrakx}";
                _output.WriteLine(newLine);
                await csvWriter.WriteLineAsync(newLine);
            }
        }

        private readonly ServiceProvider _serviceProvider;
        private readonly Common.Sources.Messari.Client.RequestHelper _messariClient;

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }
    }
}
