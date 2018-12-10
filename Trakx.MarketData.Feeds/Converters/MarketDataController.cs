using System.Collections.Generic;
using System.Threading.Tasks;

using CryptoCompare;

using Microsoft.AspNetCore.Mvc;
using Trakx.MarketData.Feeds.Common.ApiClients;
using Trakx.MarketData.Feeds.Common.Cache;
using Trakx.MarketData.Feeds.Common.Pricing;
using Trakx.MarketData.Feeds.Common.StaticData;
using Trakx.MarketData.Feeds.Common.Trackers;

namespace Trakx.MarketData.Feeds.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketDataController : ControllerBase
    {
        private readonly ICryptoCompareClient _cryptoCompareClient;

        private readonly ITrakxMemoryCache _memoryCache;
        private readonly ITrackerComponentProvider _componentProvider;
        private readonly IResponseBuilder _responseBuilder;

        public MarketDataController(ICryptoCompareClient cryptoCompareClient, 
                                    ITrakxMemoryCache memoryCache,
                                    ITrackerComponentProvider componentProvider,
                                    IResponseBuilder responseBuilder)
        {
            _cryptoCompareClient = cryptoCompareClient;
            _memoryCache = memoryCache;
            _componentProvider = componentProvider;
            _responseBuilder = responseBuilder;
        }

        [HttpGet]
        public async Task<ActionResult<TopMarketCapResponse>> GetTop20Tickers()
        {
            var top20 = await _memoryCache.Top20UsdMarketCap;
            return new ActionResult<TopMarketCapResponse>(top20);
        }

        [HttpGet(ApiConstants.CryptoCompare.AllCoins)]
        public ActionResult<CoinListResponse> GetAllCoins()
        {
            var trakxCoins = TrackerDetails.TrakxTrackersAsCoinList;
            var response = new ActionResult<CoinListResponse>(trakxCoins);
            return response;
        }

        [HttpGet(ApiConstants.CryptoCompare.Price)]
        public async Task<ActionResult<PriceSingleResponse>> SingleSymbolPriceAsync(
            string fromSymbol,
            IEnumerable<string> toSymbols,
            bool? tryConversion = null,
            string exchangeName = null)
        {
            if(!TrackerDetails.TrakxTrackersAsCoinList.Coins.ContainsKey(fromSymbol))
                throw new KeyNotFoundException($"Unable to retrieve price for ticker {fromSymbol}");
            _responseBuilder.CalculatePriceSingleResponse(fromSymbol, null);
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.PriceHistorical)]
        public async Task<ActionResult<PriceHistoricalReponse>> GetPriceHistorical()
        {
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.PriceMultifull)]

        public async Task<ActionResult<PriceMultiFullResponse>> GetPriceMultifull()
        {
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.TopMarketCap)]
        public async Task<ActionResult<TopMarketCapResponse>> GetTopMarketCap()
        {
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.TopPair)]
        public async Task<ActionResult<TopResponse>> GetTopPair()
        {
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.TopTotalVol)]
        public async Task<ActionResult<TopVolumesResponse>> GetTopTotalVol()
        {
            return null;
        }

        [HttpGet(ApiConstants.CryptoCompare.TopVolumes)]
        public async Task<ActionResult<TopResponse>> GetTopVolumes()
        {
            return null;
        }
    }
}
