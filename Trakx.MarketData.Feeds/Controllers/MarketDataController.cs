using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using CryptoCompare;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Mvc;
using Trakx.MarketData.Feeds.Common.ApiClients;
using Trakx.MarketData.Feeds.Common.Cache;
using Trakx.MarketData.Feeds.Common.Helpers;
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

        [HttpGet(ApiConstants.CryptoCompare.AllCoins)]
        public ActionResult<CoinListResponse> GetAllCoins()
        {
            var trakxCoins = TrackerDetails.TrakxTrackersAsCoinList;
            return trakxCoins;
        }

        [HttpGet(ApiConstants.CryptoCompare.Price)]
        public async Task<ActionResult<PriceSingleResponse>> SingleSymbolPriceAsync(
            [FromQuery][NotNull] string fromSymbol,
            [FromQuery][NotNull] string toSymbolsAsCsvList,
            [FromQuery] bool? tryConversion = null,
            [FromQuery] string exchangeName = null)
        {
            Check.IsKnownNonNullOrWhiteSpaceKey(fromSymbol, TrackerDetails.TrakxTrackersAsCoinList.Coins, nameof(fromSymbol));
            var toSymbols = toSymbolsAsCsvList.FromCsvToDistinctNonNullOrWhiteSpaceUpperList();
            Check.NotEmpty(toSymbols, nameof(toSymbolsAsCsvList));

            var components = await _componentProvider.GetComponentTickers(fromSymbol);

            var componentsResponse = await _cryptoCompareClient.Prices.MultipleSymbolsPriceAsync(components, toSymbols, tryConversion, exchangeName);
            var response = _responseBuilder.CalculatePriceSingleResponse(fromSymbol, componentsResponse);

            return response;
        }

        [HttpGet(ApiConstants.CryptoCompare.PriceHistorical)]
        public async Task<ActionResult<PriceHistoricalReponse>> PriceHistoricalForTimestampAsync(
            [FromQuery] [NotNull] string fromSymbol,
            [FromQuery] [NotNull] string toSymbolsAsCsvList,
            [FromQuery] DateTimeOffset requestedDate,
            [FromQuery] string marketsAsCsvList = null,
            [FromQuery] CalculationType? calculationType = null,
            [FromQuery] bool? tryConversion = null)
        {
            Check.IsKnownNonNullOrWhiteSpaceKey(fromSymbol, TrackerDetails.TrakxTrackersAsCoinList.Coins, nameof(fromSymbol));
            
            var toSymbols = toSymbolsAsCsvList.FromCsvToDistinctNonNullOrWhiteSpaceUpperList();
            Check.NotEmpty(toSymbols, nameof(toSymbolsAsCsvList));

            var markets = marketsAsCsvList.FromCsvToDistinctNonNullOrWhiteSpaceUpperList();

            var components = await _componentProvider.GetComponentTickers(fromSymbol);

            var fetchComponentsResponses = components.Select(
                async (c,i) =>
                    {
                        var priceHistoricalReponse = await _cryptoCompareClient.History.HistoricalForTimestampAsync(
                                                         c,
                                                         toSymbols,
                                                         requestedDate,
                                                         markets,
                                                         calculationType,
                                                         tryConversion);
                        //current api key doesn't let us do too many queries per second :/
                        if (i != components.Count - 1) await Task.Delay(200);
                        return priceHistoricalReponse;
                    }).ToList();

            var componentsResponses = await Task.WhenAll(fetchComponentsResponses);

            var response = _responseBuilder.CalculatePriceHistoricalResponse(fromSymbol, componentsResponses.ToList());

            return response;
        }

        [HttpGet(ApiConstants.CryptoCompare.PriceMultifull)]

        public async Task<ActionResult<PriceMultiFullResponse>> PriceMultipleSymbolFullDataAsync(
            [FromQuery] [NotNull] string fromSymbolAsCsvList,
            [FromQuery] [NotNull] string toSymbolsAsCsvList,
            bool? tryConversion = null,
            string exchangeName = null)
        {
            var fromSymbols = fromSymbolAsCsvList.FromCsvToDistinctNonNullOrWhiteSpaceUpperList();
            fromSymbols = "BTC,ETH".FromCsvToDistinctNonNullOrWhiteSpaceUpperList();
            Check.NotEmpty(fromSymbols, nameof(fromSymbolAsCsvList));
            var toSymbols = toSymbolsAsCsvList.FromCsvToDistinctNonNullOrWhiteSpaceUpperList();
            Check.NotEmpty(toSymbols, nameof(toSymbolsAsCsvList));

            return await _cryptoCompareClient.Prices.MultipleSymbolFullDataAsync(
                       fromSymbols,
                       toSymbols,
                       tryConversion,
                       exchangeName);
        }

        [HttpGet(ApiConstants.CryptoCompare.TopMarketCap)]
        public async Task<ActionResult<TopMarketCapResponse>> GetTopMarketCap(
            [FromQuery][NotNull] string toSymbol,
            [FromQuery] int? limit = null,
            [FromQuery] int? page = null,
            [FromQuery] bool? sign = null)
        {
            Check.NotNullOrWhiteSpace(toSymbol, nameof(toSymbol));
            var response = await _cryptoCompareClient.Tops.CoinFullDataByMarketCap(toSymbol, limit, page, sign);
            return new ActionResult<TopMarketCapResponse>(response);
        }

        [HttpGet(ApiConstants.CryptoCompare.TopPairs)]
        public async Task<ActionResult<TopResponse>> TradingPairsAsync(
            [NotNull] string fromSymbol,
            int? limit = null)
        {
            Check.IsKnownNonNullOrWhiteSpaceKey(
                fromSymbol,
                TrackerDetails.TrakxTrackersAsCoinList.Coins,
                nameof(fromSymbol));

            fromSymbol = "ETH";
            return await _cryptoCompareClient.Tops.TradingPairsAsync(fromSymbol, limit);
        }

        [HttpGet(ApiConstants.CryptoCompare.TopTotalVol)]
        public async Task<ActionResult<TopVolumesResponse>> ByPairVolumeAsync([NotNull] string toSymbol, int? limit = null)
        {
            Check.NotNull(toSymbol, nameof(toSymbol));
            return await _cryptoCompareClient.Tops.ByPairVolumeAsync(toSymbol, limit);
        }

        [HttpGet(ApiConstants.CryptoCompare.TopVolumes)]
        public async Task<ActionResult<TopVolumesResponse>> TopTotalVol([NotNull] string toSymbol, int? limit = null)
        {
            return null;
        }
    }
}
