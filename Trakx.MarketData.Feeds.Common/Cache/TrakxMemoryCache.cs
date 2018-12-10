using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CryptoCompare;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Trakx.MarketData.Feeds.Common.Cache
{
    public class TrakxMemoryCache : MemoryCache, ITrakxMemoryCache

    {
        private const string Components = nameof(Components);
        private readonly ICryptoCompareClient _cryptoCompareClient;

        /// <inheritdoc />
        public TrakxMemoryCache(IOptions<MemoryCacheOptions> optionsAccessor, ICryptoCompareClient cryptoCompareClient)
            : base(optionsAccessor)
        {
            _cryptoCompareClient = cryptoCompareClient;
        }

        public Task<TopMarketCapResponse> Top20UsdMarketCap
        {
            get
            {
                return this.GetOrCreate(
                    CacheEntries.Top20UsdMarketCap,
                    async e =>
                        {
                            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                            var topMarketCapResponse = await _cryptoCompareClient.Tops.CoinFullDataByMarketCap("USD", 20);
                            e.Value = topMarketCapResponse;
                            return topMarketCapResponse;
                        });
            }
        }

        /// <inheritdoc />
        public IList<string> GetComponentsForTracker(string trackerTicker, Func<ICryptoCompareClient, IList<string>> trackerComponentExtractor)
        {
            return this.GetOrCreate($"{trackerTicker}.{Components}", e =>
                {
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    var response = trackerComponentExtractor(_cryptoCompareClient);
                    e.Value = response;
                    return response;
                });
        }

        public class CacheEntries
        {
            public static string Top20UsdMarketCap { get { return $"_{nameof(Top20UsdMarketCap)}"; } }
        }
    }
}
