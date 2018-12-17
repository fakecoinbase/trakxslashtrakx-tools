using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CryptoCompare;

using Trakx.MarketData.Feeds.Common.ApiClients;

using Xunit.Abstractions;

namespace Trakx.MarketData.Feeds.Tests.Utils
{
    public class AggregateTotalVolumeExchanged : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private CryptoCompareClient _cryptoCompareClient;

        public AggregateTotalVolumeExchanged(ITestOutputHelper output)
        {
            _output = output;
            _cryptoCompareClient = new CryptoCompareClient(ApiConstants.CryptoCompare.ApiKey);
        }

        //[Fact]
        public async Task Extract_Data_For_Edouard()
        {
            var exchanges = await _cryptoCompareClient.Exchanges.ListAsync();
            var allExchanges = string.Join(",", exchanges.Keys);
            var response = await _cryptoCompareClient.History.ExchangeDailyAsync("USD", limit: 365*5);
            var aggregate = response.Data.ToDictionary(d => d.Time.DateTime.ToUniversalTime(), d => d.Volume.ToString());

            var csvOutput = string.Join(Environment.NewLine, aggregate.Select(p => $"{p.Key}, {p.Value}"));
            _output.WriteLine(csvOutput);
        }

        //[Fact]
        public async Task Extract_Volumes_By_Exchange()
        {
            var exchangeResponse = await _cryptoCompareClient.Exchanges.ListAsync();
            var exchangeNames = exchangeResponse.Keys.ToList();

            var volumeByDayByExchange = exchangeNames.ToDictionary(e => e, e =>
                {
                    try
                    {
                        Thread.Sleep(700);
                        var exchangeDailyReponse = _cryptoCompareClient.History.ExchangeDailyAsync("USD", exchangeName: e, limit: 1000);
                        var volumeByDate = exchangeDailyReponse.ConfigureAwait(false).GetAwaiter().GetResult().Data
                            .ToDictionary(
                                d => d.Time.Date,
                                d => d.Volume.ToString());
                        return volumeByDate;
                    }
                    catch (Exception ex)
                    {
                        _output.WriteLine($"Failed to get response for exchange {e}: {ex}");
                        return new Dictionary<DateTime, string>();
                    }
                });

            var allDates = volumeByDayByExchange.Values.SelectMany(p => p.Keys.ToList()).Distinct().ToList();

            using (var stream = File.Create("volumes.usd.by.day.csv"))
            using (var writer = new StreamWriter(stream))
            {
                var dateColumn = AddQuotes("Date");
                var columnNames = new []{dateColumn}.Concat(exchangeNames).ToList();
                writer.WriteLine(string.Join(",", columnNames));
                foreach (var date in allDates)
                {
                    var columnContent = columnNames.Select(
                        c => c == dateColumn 
                                 ? date.ToString("yyyyMMdd") 
                                 : volumeByDayByExchange[c].TryGetValue(date, out string volume) ? volume : string.Empty);
                    writer.WriteLine(string.Join(",", columnContent.Select(AddQuotes)));
                }
            }

        }

        private static string AddQuotes(string unquoted)
        {
            return $"\"{unquoted}\"";
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _cryptoCompareClient?.Dispose();
        }
    }
}
