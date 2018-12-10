using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CryptoCompare;
using Newtonsoft.Json;

namespace Trakx.MarketData.Feeds.Common.StaticData
{
    public static class TrackerDetails
    {
        private static Lazy<CoinListResponse> _trakxTrackersAsCoinList =
            new Lazy<CoinListResponse>(() => ReadCoinListFromResource().ConfigureAwait(false).GetAwaiter().GetResult(), LazyThreadSafetyMode.PublicationOnly);

        public static CoinListResponse TrakxTrackersAsCoinList => _trakxTrackersAsCoinList.Value;

        private static async Task<CoinListResponse> ReadCoinListFromResource()
        {
            var assembly = typeof(TrackerDetails).Assembly;
            using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.trakx-coinlist.json"))
            using (var reader = new StreamReader(stream))
            {
                var jsonResult = await reader.ReadToEndAsync();
                var response = JsonConvert.DeserializeObject<CoinListResponse>(jsonResult);
                return response;
            }
        }
    }
}
