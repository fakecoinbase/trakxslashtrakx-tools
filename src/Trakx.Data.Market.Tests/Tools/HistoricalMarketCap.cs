using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinGecko.Clients;
using CoinGecko.Entities.Response.Coins;
using CsvHelper;
using Polly;
using Polly.Retry;
using Xunit;

namespace Trakx.Data.Market.Tests.Tools
{
    public class HistoricalMarketCap : IDisposable
    {
        private readonly CoinsClient _coinsClient;
        private readonly string[] _tokens;
        private readonly HttpClientHandler _httpClientHandler;
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy _retryPolicy;

        public HistoricalMarketCap()
        {
            _tokens = new[] { "chsb", "cnd", "gvt", "mln", "nmr", "omg", "poly", "snx", "tct", "tnb" };
            _httpClientHandler = _httpClientHandler = new HttpClientHandler();
            _httpClient = new HttpClient(_httpClientHandler);
            _coinsClient = new CoinGecko.Clients.CoinsClient(_httpClient);
            _retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(50, c => TimeSpan.FromSeconds(c));
        }

        public class HistoricalData
        {
            public string Symbol { get; set; }
            public double? Price { get; set; }
            public double? MarketCap { get; set; }
            //public string CirculatingSupply { get; set; }
            //public long? TotalSupply { get; set; }
            public double? TotalVolume { get; set; }
            public DateTime Date { get; set; }
        }

        [Fact(Skip = "not a test")]
        public async Task RetrieveHistoricalMarketCapsForTokens()
        {
            var coinData = await _coinsClient.GetCoinList();
            var coinIds = coinData.Where(d => _tokens.Contains(d.Symbol)).Select(d => d.Id);

            await using var csvOutput = File.Create(Path.Combine($"historical.{DateTime.Today:yyyyMMdd}.csv"));
            await using var writer = new StreamWriter(csvOutput);
            using var csvWriter = new CsvWriter(writer);

            csvWriter.WriteHeader<HistoricalData>();
            csvWriter.NextRecord();
            csvWriter.Flush();

            foreach (var coinId in coinIds)
            {
                await foreach (var historicalData in GetHistoryByCoinId(coinId, 
                    DateTime.Today.AddYears(-2),
                    DateTime.Today))
                {
                    csvWriter.WriteRecord(historicalData);
                    csvWriter.NextRecord();
                }
            }
        }



        public async IAsyncEnumerable<HistoricalData> GetHistoryByCoinId(string coinId, DateTime startDate, DateTime endDate)
        {
            var currentDate = endDate.Date;
            while (currentDate >= startDate)
            {
                var history = await  _retryPolicy.ExecuteAsync(() =>
                    _coinsClient.GetHistoryByCoinId(coinId, currentDate.ToString("dd-MM-yyyy"), "false"));
                
                if (history?.MarketData == null) continue;

                var historicalData = new HistoricalData
                {

                    Symbol = history.Symbol,
                    MarketCap = history.MarketData.MarketCap["usd"],
                    Price = history.MarketData.CurrentPrice["usd"],
                    //CirculatingSupply = historicalData.MarketData.CirculatingSupply,
                    //TotalSupply = historicalData.MarketData.TotalSupply,
                    TotalVolume = history.MarketData.TotalVolume["usd"],
                    Date = currentDate
                };
                yield return historicalData;
                currentDate = currentDate.AddDays(-1);
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _httpClientHandler.Dispose();
        }
    }
}