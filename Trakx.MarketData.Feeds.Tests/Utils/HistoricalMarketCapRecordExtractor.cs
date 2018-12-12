using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using FluentAssertions;

using Trakx.MarketData.Feeds.Common.Converters;
using Trakx.MarketData.Feeds.Models.CoinMarketCap;

using Xunit;
using Xunit.Abstractions;

namespace Trakx.MarketData.Feeds.Tests.Utils
{
    public class HistoricalMarketCapRecordExtractor : IDisposable
    {
        private const string RegexPattern =
            "<tr[^>]+>[^\\d]*<td class=\"text-center\">[^\\d]*(?<Rank>[\\d]+)[^\\d]*</td>[^\\d]*<td class=\"no-wrap currency-name\" data-sort=\\\"(?<CurrencyName>[^\\\"]+)\\\">[^\\d]*<img data-src=[^>]+>[^\\d]*<span class=\"currency-symbol [^>]+><a class=\"link-secondary\"[^>]+>(?<CurrencySymbol>[^<]+)</a></span>[^\\d]*<[^>]+>[^\\d]*<a [^>]+>[^<]+</a>[^\\d]*</td>[^\\d]*<td class=\"text-left col-symbol\">[^<]+</td>[^\\d]*<td class=\"no-wrap market-cap text-right\" data-usd=\"(?<MarketCapUsd>[e\\+\\-\\.0-9\\?]+)\" data-btc=\"(?<MarketCapBtc>[e\\+\\-\\.0-9\\?]+)\" [^>]+>[^<]+</td>[^\\d]*<td class=[^>]+>[^\\d]*<a href=\"[^\\\"]+\" class=\"price\" data-usd=\"(?<PriceUsd>[e\\+\\-\\.0-9]+)\" data-btc=\"(?<PriceBtc>[e\\+\\-\\.0-9]+)\">[^>]+>[^\\d]*</td>[^\\d]*<td[^>]+>[^\\d]*<span data-supply=\"(?<CirculatingSupply>([e\\+\\-\\.0-9]+|None))\" [^>]+>[^<]+</span>[^\\d]*</td>[^\\d]*<td[^>]+>[^\\d]*<a href=\"[^\\\"]+\" class=\"volume\" data-usd=\"(?<Volume24HUsd>[e\\+\\-\\.0-9]+)\" data-btc=\"(?<Volume24HBtc>[e\\+\\-\\.0-9]+)\">[^<]+</a>[^\\d]*</td>[^\\d]*<td class=\"[^\\\"]+\"( data-timespan=\"1h\" data-percentusd=\"[e\\+\\-\\.0-9]+\" data-symbol=\"[^\\\"]+\")* data-sort=\"(?<Change1H>[e\\+\\-\\.0-9]+)\">[^<]+</td>[^\\d]*<td class=\"[^\\\"]+\"( data-timespan=\"24h\" data-percentusd=\"[e\\+\\-\\.0-9]+\" data-symbol=\"[^\\\"]+\")* data-sort=\"(?<Change1D>[e\\+\\-\\.0-9]+)\">[^<]+</td>[^\\d]*<td class=\"[^\\\"]+\"( data-timespan=\"7d\" data-percentusd=\"[e\\+\\-\\.0-9]+\" data-symbol=\"[^\\\"]+\")* data-sort=\"(?<Change1W>[e\\+\\-\\.0-9]+)\">[^<]+</td>";
        private const string CoinMarketCapUrlStub = "https://coinmarketcap.com/historical/{0:yyyyMMdd}/";
        private Regex _regex;
        private HttpClient _httpClient;
        private ITestOutputHelper _output;

        public HistoricalMarketCapRecordExtractor(ITestOutputHelper output)
        {
            _output = output;
            _httpClient = new HttpClient();
            _regex = new Regex(RegexPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline, TimeSpan.FromMinutes(1));
            //_regex.MatchTimeout = TimeSpan.FromMinutes(1);
        }

        [Fact]
        public async Task<IDictionary<DateTime, IList<HistoricalMarketCapRecord>>> ExtractRecordsForPeriod()
        {
            var minDate = new DateTime(2018,12,10);
            var maxDate = DateTime.Today.AddDays(-1);

            var results = new Dictionary<DateTime, IList<HistoricalMarketCapRecord>>();
            var date = minDate;

            _output.WriteLine(HistoricalMarketCapRecord.GetCsvHeaders());

            while (date <= maxDate)
            {
                var records = await ExtractRecords(date, _httpClient);
                results.Add(date, records);
                date = date.AddDays(1);
            }

            results.Count.Should().Be((maxDate - minDate).Days + 1);
            return results;
        }

        public async Task<IList<HistoricalMarketCapRecord>> ExtractRecords(DateTime recordTimeStamp, HttpClient httpClient)
        {
            var timeStamp = recordTimeStamp.Date;
            var uri = string.Format(CoinMarketCapUrlStub, timeStamp);
            var response = await httpClient.GetAsync(new Uri(uri));
            var rawContent = await response.Content.ReadAsStringAsync();

            var matches = _regex.Matches(rawContent);

            var records = matches.Select(m => GetHistoricalRecordFromMatch(m, timeStamp)).ToList();

            records.ForEach(r => _output.WriteLine(r.ToCsv()));

            return records;
        }

        [Fact]
        public async Task ExtractRecordsForOneSampleDay()
        {
            using(var rawHtmlStream = TestData.CoinMarketCap.HistoricalMarketCap())
            using (var reader = new StreamReader(rawHtmlStream))
            {
                var content = await reader.ReadToEndAsync();
                var matches = _regex.Matches(content);
                var records = matches.Select(m => GetHistoricalRecordFromMatch(m, DateTime.MinValue)).OrderBy(r => r.Rank).ToList();
                records.Count.Should().BeGreaterOrEqualTo(2068);
                _output.WriteLine(string.Join(Environment.NewLine, records.Select(r => r.CurrencySymbol)));
            }
        }
        
        private static HistoricalMarketCapRecord GetHistoricalRecordFromMatch(Match m, DateTime timeStamp)
        {
            var rank = int.Parse(m.Groups[nameof(HistoricalMarketCapRecord.Rank)].Value);
            var currencyName = m.Groups[nameof(HistoricalMarketCapRecord.CurrencyName)].Value;
            var currencySymbol = m.Groups[nameof(HistoricalMarketCapRecord.CurrencySymbol)].Value;
            var marketCapUsd = m.Groups[nameof(HistoricalMarketCapRecord.MarketCapUsd)].Value.ToDecimalOrNull();
            var marketCapBtc = m.Groups[nameof(HistoricalMarketCapRecord.MarketCapBtc)].Value.ToDecimalOrNull();
            var priceUsd = m.Groups[nameof(HistoricalMarketCapRecord.PriceUsd)].Value.ToDecimalOrNull();
            var priceBtc = m.Groups[nameof(HistoricalMarketCapRecord.PriceBtc)].Value.ToDecimalOrNull();
            var circulatingSupply = m.Groups[nameof(HistoricalMarketCapRecord.CirculatingSupply)].Value.ToDecimalOrNull();
            var volume24HUsd = m.Groups[nameof(HistoricalMarketCapRecord.Volume24HUsd)].Value.ToDecimalOrNull();
            var volume24HBtc = m.Groups[nameof(HistoricalMarketCapRecord.Volume24HBtc)].Value.ToDecimalOrNull();
            var change1H = m.Groups[nameof(HistoricalMarketCapRecord.Change1H)].Value.ToDecimalOrNull();
            var change1D = m.Groups[nameof(HistoricalMarketCapRecord.Change1D)].Value.ToDecimalOrNull();
            var change1W = m.Groups[nameof(HistoricalMarketCapRecord.Change1W)].Value.ToDecimalOrNull();
            var record = new HistoricalMarketCapRecord(
                timeStamp,
                rank,
                currencyName,
                currencySymbol,
                marketCapUsd,
                marketCapBtc,
                priceUsd,
                priceBtc,
                circulatingSupply,
                volume24HUsd,
                volume24HBtc,
                change1H,
                change1D,
                change1W);
            return record;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}