using System;
using System.Collections.Generic;
using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Trakx.MarketData.Feeds.Common.Converters;
using Trakx.MarketData.Feeds.Common.Models.CoinMarketCap;

namespace Trakx.MarketData.Feeds.Models.CoinMarketCap
{
    public class CoinAndMarketCap : ICoinAndMarketCap
    {
        /// <inheritdoc />
        public CoinAndMarketCap(
            long id,
            string name,
            string symbol,
            string slug,
            double circulatingSupply,
            double totalSupply,
            long? maxSupply,
            DateTimeOffset dateAdded,
            long numMarketPairs,
            long cmcRank,
            DateTimeOffset lastUpdated,
            IDictionary<string, IQuote> quote)
        {
            Id = id;
            Name = name;
            Symbol = symbol;
            Slug = slug;
            CirculatingSupply = circulatingSupply;
            TotalSupply = totalSupply;
            MaxSupply = maxSupply;
            DateAdded = dateAdded;
            NumMarketPairs = numMarketPairs;
            CmcRank = cmcRank;
            LastUpdated = lastUpdated;
            Quote = quote;
        }

        [JsonProperty("id")]
        public long Id { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("symbol")]
        public string Symbol { get; }

        [JsonProperty("slug")]
        public string Slug { get; }

        [JsonProperty("circulating_supply")]
        public double CirculatingSupply { get; }

        [JsonProperty("total_supply")]
        public double TotalSupply { get; }

        [JsonProperty("max_supply")]
        public long? MaxSupply { get; }

        [JsonProperty("date_added")]
        public DateTimeOffset DateAdded { get; }

        [JsonProperty("num_market_pairs")]
        public long NumMarketPairs { get; }

        [JsonProperty("cmc_rank")]
        public long CmcRank { get; }

        [JsonProperty("last_updated")]
        public DateTimeOffset LastUpdated { get; }

        [JsonProperty("quote")]
        [JsonConverter(typeof(ConcreteDictionaryValueConverter<string, IQuote, Quote>))]
        public IDictionary<string, IQuote> Quote { get; }
    }

    public static class Serialize
    {
        public static string ToJson(this CoinsAndMarketCapListing self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
