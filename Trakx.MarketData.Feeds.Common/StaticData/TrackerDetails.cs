using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoCompare;
using Newtonsoft.Json;
using Trakx.MarketData.Feeds.Common.Models.Trakx;
using Trakx.MarketData.Feeds.Common.Trackers;

namespace Trakx.MarketData.Feeds.Common.StaticData
{
    public static class TrackerDetails
    {
        private static readonly Lazy<CoinListResponse> _trakxTrackersAsCoinList =
            new Lazy<CoinListResponse>(() => ReadCoinListFromResource().ConfigureAwait(false).GetAwaiter().GetResult(), LazyThreadSafetyMode.PublicationOnly);

        public static CoinListResponse TrakxTrackersAsCoinList => _trakxTrackersAsCoinList.Value;

        private static async Task<CoinListResponse> ReadCoinListFromResource()
        {
            var assembly = typeof(TrackerDetails).Assembly;
            await using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.trakx-coinlist.json");
            using var reader = new StreamReader(stream);
            var jsonResult = await reader.ReadToEndAsync();
            var response = JsonConvert.DeserializeObject<CoinListResponse>(jsonResult);
            return response;
        }

        private static readonly Lazy<CoinListResponse> _trakxTrackersAsIndexDetails =
            new Lazy<CoinListResponse>(() => ReadCoinListFromResource().ConfigureAwait(false).GetAwaiter().GetResult(), LazyThreadSafetyMode.PublicationOnly);

        public static Dictionary<KnownIndexes, IndexDetails> IndexDetails { get; } = Enum.GetNames(typeof(KnownIndexes))
            .ToDictionary(Enum.Parse<KnownIndexes>,
                n => ReadIndexDetailsFromResource(n).ConfigureAwait(false).GetAwaiter().GetResult());

        private static async Task<IndexDetails> ReadIndexDetailsFromResource(string symbol)
        {
            var assembly = typeof(TrackerDetails).Assembly;
            await using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.KnownIndexes.{symbol}.json");
            using var reader = new StreamReader(stream);
            var jsonResult = await reader.ReadToEndAsync();
            var response = JsonConvert.DeserializeObject<IndexDetails>(jsonResult);
            return response;
        }
    }
}
