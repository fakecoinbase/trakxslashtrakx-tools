using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CsvHelper.Configuration;

using FluentAssertions;

using Trakx.MarketData.Feeds.Common.Helpers;
using Trakx.MarketData.Feeds.Tests.ApiClients;

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
        private FileStream _fileStream;
        private StreamWriter _fileWriter;

        public HistoricalMarketCapRecordExtractor(ITestOutputHelper output)
        {
            _output = output;
            _httpClient = new HttpClient();
            _regex = new Regex(RegexPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline, TimeSpan.FromMinutes(1));
            //_regex.MatchTimeout = TimeSpan.FromMinutes(1);
        }

        //[Fact]
        public async Task<IDictionary<DateTime, IList<HistoricalMarketCapRecord>>> ExtractRecordsForPeriod()
        {
            var minDate = new DateTime(2013, 01, 01);
            var maxDate = new DateTime(2013, 12, 31);

            var results = new Dictionary<DateTime, IList<HistoricalMarketCapRecord>>();
            var date = minDate;

            _fileStream = File.Create($"marketCaps.from.{minDate:yyyyMMdd}.to.{maxDate:yyyyMMdd}.csv");
            _fileWriter = new StreamWriter(_fileStream);

            //_output.WriteLine(HistoricalMarketCapRecord.GetCsvHeaders());
            _fileWriter.WriteLine(HistoricalMarketCapRecord.GetCsvHeaders());

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

            records.ForEach(r =>
                {
                    var csvLine = r.ToCsv();
                    //_output.WriteLine(csvLine);
                    _fileWriter.WriteLine(csvLine);
                });

            return records;
        }

        //[Fact]
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

        [Fact(Skip = "One off")]
        public async Task ReformatMarketCapRecords()
        {
            var filesToRead = Directory.EnumerateFileSystemEntries(
                Environment.CurrentDirectory,
                $"marketCaps.from.????????.to.????????.csv");

            var allRecords = filesToRead.SelectMany(ExtractMarketCapRecords).ToList();

            var allSymbols = allRecords.Select(ToColumnName).Distinct().ToList();

            var marketCapByDateBySymbol = allSymbols.ToDictionary(
                r => r, 
                r =>
                    {
                        var historicalMarketCapRecords = allRecords.Where(a => ToColumnName(a) == r);
                        var dates = historicalMarketCapRecords.Select(h => h.TimeStamp).ToList();
                        return historicalMarketCapRecords.ToDictionary(
                            a => a.TimeStamp,
                            a => a.MarketCapUsd);
                    });

            var allDates = allRecords.Select(r => r.TimeStamp).Distinct().ToList();

            using (var stream = File.Create("marketcap.usd.by.day.csv"))
            using (var writer = new StreamWriter(stream))
            {
                var dateColumn = AddQuotes("Date");
                var columnNames = new[] { dateColumn }.Concat(allSymbols).ToList();
                writer.WriteLine(string.Join(",", columnNames));
                foreach (var date in allDates)
                {
                    var columnContent = columnNames.Select(
                        c => c == dateColumn
                                 ? date.ToString("yyyyMMdd")
                                 : marketCapByDateBySymbol.TryGetValue(c, out Dictionary<DateTime, decimal?> d) 
                                     ? d.TryGetValue(date, out decimal? volume) 
                                           ? volume?.ToString() ?? string.Empty
                                           : string.Empty
                                     : string.Empty);
                    writer.WriteLine(string.Join(",", columnContent.Select(AddQuotes)));
                }
            }

        }

        private string ToColumnName(HistoricalMarketCapRecord record)
        {
            return $"{record.CurrencyName} - ({record.CurrencySymbol})";
        }

        private static string AddQuotes(string unquoted)
        {
            return $"\"{unquoted}\"";
        }
        
        public IList<HistoricalMarketCapRecord> ExtractMarketCapRecords(string filePath)
        {
            using(var fileStream = File.OpenRead(filePath))
            using (var reader = new StreamReader(fileStream))
            {
                var headers = reader.ReadLine();
                Assert.Equal(headers, "\"TimeStamp\",\"Rank\",\"CurrencyName\",\"CurrencySymbol\",\"MarketCapUsd\",\"MarketCapBtc\",\"PriceUsd\",\"PriceBtc\",\"CirculatingSupply\",\"Volume24HUsd\",\"Volume24HBtc\",\"Change1H\",\"Change1D\",\"Change1W\"");
                var records = new List<HistoricalMarketCapRecord>();
                while (!reader.EndOfStream)
                {
                    var recordRawLine = reader.ReadLine();
                    var fields = recordRawLine.Split(",").Select(s => s.Trim('"')).ToArray();
                    var record = new HistoricalMarketCapRecord(
                        DateTime.Parse(fields[0]), 
                        int.Parse(fields[1]),
                        fields[2], 
                        fields[3], 
                        decimal.TryParse(fields[4], out decimal marketCapUsd) ? marketCapUsd : (decimal?)null,
                        decimal.TryParse(fields[5], out decimal marketCapBtc) ? marketCapBtc : (decimal?)null,
                        null, null, null, null, null, null, null, null);
                    records.Add(record);
                }

                return records;
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
            _fileWriter?.Close();
            _fileWriter?.Dispose();
        }
    }

    public class HistoricalMarketCapRecordMap : ClassMap<HistoricalMarketCapRecord>
    {
        public HistoricalMarketCapRecordMap()
        {
            AutoMap();
        }
    }
}