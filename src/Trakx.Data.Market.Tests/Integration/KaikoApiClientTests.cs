using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Data.Market.Common.Sources.Kaiko.Client;
using Trakx.Data.Market.Common.Sources.Kaiko.DTOs;
using Trakx.Data.Market.Common.Sources.Messari.Client;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Data.Market.Tests.Integration
{
    public class KaikoApiClientTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly IKaikoClient _kaikoClient;

        public KaikoApiClientTests(ITestOutputHelper output)
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


        [Fact]
        public async Task GetExchanges_should_return_exchanges()
        {
            var exchanges = await _kaikoClient.GetExchanges();

            _output.WriteLine(exchanges.ToString());

            exchanges.Exchanges.ForEach(e => _output.WriteLine($"{e.Code}, {e.Name}"));
        }

        [Fact]
        public async Task GetInstruments_should_return_instruments()
        {
            var instruments = await _kaikoClient.GetInstruments();

            _output.WriteLine(instruments.ToString());

            instruments.Instruments.ForEach(i => 
                _output.WriteLine($"{i.Code}, {i.ExchangeCode}, {i.ExchangePairCode}, {i.Class}, {i.BaseAsset}, {i.QuoteAsset}"));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetSpotExchangeRate_for_one_token_should_return_aggregated_prices(bool direct)
        {
            var coinSymbol = "eth";
            var quoteSymbol = "usd";
            var query = _kaikoClient
                .CreateSpotExchangeRateRequest(coinSymbol, quoteSymbol, direct);

            var response = await _kaikoClient.GetSpotExchangeRate(query)
                .ConfigureAwait(false);
            
            CheckExchangeRateResponse(query, response);
        }

        private void CheckExchangeRateResponse(SpotExchangeRateRequest request, SpotDirectExchangeRateResponse response)
        {
            var results = response.Data.OrderByDescending(d => d.Timestamp).ToList();
            results.Count.Should().BeGreaterOrEqualTo(1);

            decimal.TryParse(results[0].Price, out decimal price).Should().BeTrue();
            price.Should().BeGreaterThan(0);

            DateTimeOffset.FromUnixTimeMilliseconds(results.Last().Timestamp).UtcDateTime
                .Should().BeCloseTo(request.StartTime.UtcDateTime, TimeSpan.FromHours(1));


            DateTimeOffset.FromUnixTimeMilliseconds(results.First().Timestamp).UtcDateTime
                .Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromHours(1));


            if (!request.DirectExchangeRate) return;

            decimal.TryParse(response.Data[0].Volume, out decimal volume).Should().BeTrue();
            volume.Should().BeGreaterThan(0);

            _output.WriteLine(JsonSerializer.Serialize(results[0]));
        }

        private class AllTokensUsedAsComponents : TheoryData
        {
            public AllTokensUsedAsComponents()
            {
                new List<string> { "IDX1", "IDX2" }.ForEach(s => AddRow(s));
            }
        }

        [Theory(Skip = "Moving away from Kaiko")]
        [ClassData(typeof(AllTokensUsedAsComponents))]
        public async Task GetSpotExchangeRate_for_trakx_tokens_should_return_aggregated_prices(string tokenSymbol)
        {
            var quoteSymbol = "usd";
            var query = _kaikoClient.CreateSpotExchangeRateRequest(tokenSymbol, quoteSymbol, false);

            var response = await _kaikoClient.GetSpotExchangeRate(query).ConfigureAwait(false);

            CheckExchangeRateResponse(query, response);
        }

        public static decimal TryFindUsdExchangeRate(IKaikoClient kaikoClient, string baseSymbol)
        {
            try
            {
                if (baseSymbol == "usd") return 1m;
                if (baseSymbol == "jpy") return 1/108.905m;
                if (baseSymbol == "eur") return 1/0.907934m;
                if (baseSymbol == "gbp") return 1/0.776741m;
                if (baseSymbol == "krw") return 1/1177.23m;
                var request = kaikoClient.CreateSpotExchangeRateRequest(baseSymbol, "usd", true);
                var result = kaikoClient.GetSpotExchangeRate(request).GetAwaiter().GetResult();
                var exchangeRate = result.Data.Average(d => decimal.Parse(d.Price));
                return exchangeRate;
            }
            catch (Exception e)
            {
                throw new InvalidDataException($"Failed to find exchange rate for {baseSymbol}", e);
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
