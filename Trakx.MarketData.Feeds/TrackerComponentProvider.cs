using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Trakx.MarketData.Feeds.Common;
using Trakx.MarketData.Feeds.Common.ApiClients;
using Trakx.MarketData.Feeds.Common.Models.CryptoCompare;
using Trakx.MarketData.Feeds.Common.Models.Trakx;

namespace Trakx.MarketData.Feeds
{
    public class TrackerComponentProvider : ITrackerComponentProvider
    {
        private readonly ICryptoCompareApiClient _cryptoCompareApiClient;

        private readonly ICoinMarketCapApiClient _coinMarketCapApiClient;

        private readonly ICoinSymbolMapper _coinSymbolMapper;

        private IReadOnlyDictionary<string, ICoin> _top10;

        public TrackerComponentProvider(
            ICryptoCompareApiClient cryptoCompareApiClient,
            ICoinMarketCapApiClient coinMarketCapApiClient,
            ICoinSymbolMapper coinSymbolMapper)
        {
            _cryptoCompareApiClient = cryptoCompareApiClient;
            _coinMarketCapApiClient = coinMarketCapApiClient;
            _coinSymbolMapper = coinSymbolMapper;
        }

        /// <inheritdoc />
        public async Task<IList<ICryptoCompareCoinAndMarketCap>> GetTopXMarketCapCoins(uint coinCount)
        {
            var cryptoCompareCoins = await _cryptoCompareApiClient.GetAllSupportedCoins();
            var coinMarketCapCoins = await _coinMarketCapApiClient.GetCoinsAndMarketCapListings();

            var top20CmC = coinMarketCapCoins.CoinsAndMarketCaps
                            .OrderByDescending(c => c.Quote["USD"].MarketCap)
                            .Select(c => new {
                                         CoinSymbol = _coinSymbolMapper.CoinMarketCapToCryptoCompare(c.Symbol),
                                         MarketCap = c.Quote["USD"].MarketCap
                                             })
                            .Take(20);

            var missingCoins = top20CmC
                .Where(c => !cryptoCompareCoins.Data.ContainsKey(c.CoinSymbol))
                .ToList();

            if(missingCoins.Any())
                throw new KeyNotFoundException($"Coins {string.Join(",", missingCoins)} not found on CryptoCompare");

            var top20 = top20CmC.Select(
                c => (ICryptoCompareCoinAndMarketCap)new Models.Trakx.Coin(cryptoCompareCoins.Data[c.CoinSymbol], c.MarketCap))
                .ToList();
                
            return top20;
        }
    }
}
