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
                var getOrCreateTask = this.GetOrCreateAsync(
                    CacheEntries.Top20UsdMarketCap,
                    async e =>
                        {
                            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                            var topMarketCapResponse = await _cryptoCompareClient.Tops.CoinFullDataByMarketCap("USD", 20);
                            e.Value = topMarketCapResponse;
                            return topMarketCapResponse;
                        });
                return getOrCreateTask;
            }
        }

        /// <inheritdoc />
        public async Task<IList<string>> GetComponentsForTracker(string trackerTicker, Func<ICryptoCompareClient, Task<IList<string>>> trackerComponentExtractor)
        {
            var getOrCreateTask = this.GetOrCreateAsync($"{trackerTicker}.{Components}",
                async e =>
                    {
                        e.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                        var response = await trackerComponentExtractor(_cryptoCompareClient);
                        e.Value = response;
                        return response;
                    });
            return await getOrCreateTask;
        }

        public class CacheEntries
        {
            public static string Top20UsdMarketCap { get { return $"_{nameof(Top20UsdMarketCap)}"; } }
        }
    }
}
