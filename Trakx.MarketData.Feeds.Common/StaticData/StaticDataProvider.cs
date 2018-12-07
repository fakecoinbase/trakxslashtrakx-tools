using System;
using System.IO;
using System.Threading;

using CryptoCompare;

using Newtonsoft.Json;

using Trakx.MarketData.Feeds.Common.ApiClients;

namespace Trakx.MarketData.Feeds.Common.StaticData
{
    public static class StaticDataProvider
    {
        private static Lazy<CoinListResponse> _trakxTrackersAsCoinList = 
            new Lazy<CoinListResponse>(ReadCoinListFromResource, LazyThreadSafetyMode.PublicationOnly);

        public static CoinListResponse TrakxTrackersAsCoinList => _trakxTrackersAsCoinList.Value;

        public static CoinListResponse ReadCoinListFromResource()
        {
            var assembly = typeof(StaticDataProvider).Assembly;
            using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.trakx.coinlist.json"))
            using (var reader = new StreamReader(stream))
            {
                var jsonResult = reader.ReadToEndAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                var response = JsonConvert.DeserializeObject<CoinListResponse>(jsonResult);
                return response;
            }
        }
    }
}
