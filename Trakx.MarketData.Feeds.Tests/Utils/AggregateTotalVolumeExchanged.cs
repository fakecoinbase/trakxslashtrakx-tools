using System;
using System.Linq;
using System.Threading.Tasks;

using CryptoCompare;

using Trakx.MarketData.Feeds.Common.ApiClients;

using Xunit;
using Xunit.Abstractions;

namespace Trakx.MarketData.Feeds.Tests.Utils
{
    public class AggregateTotalVolumeExchanged
    {
        private readonly ITestOutputHelper _output;

        public AggregateTotalVolumeExchanged(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task Extract_Data_For_Edouard()
        {
            var client = new CryptoCompareClient(ApiConstants.CryptoCompare.ApiKey);
            var exchanges = await client.Exchanges.ListAsync();
            var allExchanges = string.Join(",", exchanges.Keys);
            var response = await client.History.ExchangeDailyAsync("USD", limit: 365*5);
            var aggregate = response.Data.ToDictionary(d => d.Time.DateTime.ToUniversalTime(), d => d.Volume.ToString());

            var csvOutput = string.Join(Environment.NewLine, aggregate.Select(p => $"{p.Key}, {p.Value}"));
            _output.WriteLine(csvOutput);
        }
    }
}
